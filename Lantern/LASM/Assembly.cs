using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using LangDef;

namespace LASM
{
    public partial class Assembly
    {
        string outputFile = "";
        string listFile = "";

        public Assembly(string fileName)
        {

            outputFile = fileName.Substring(0, fileName.IndexOf('.')) + ".bin";
            listFile = fileName.Substring(0, fileName.IndexOf('.')) + ".lst";

            IncludeFile(fileName);
        }

        public void Assemble()
        {
            statements = new List<Statement>();

            RewriteDefines();
            RewriteLabels();
            FindGlobalLabels();
            WriteGlobalSymbols();
            BuildDefineTable();
            ReplaceDefines();
            RewriteOpCodes();
            WritePreprocessedFile("preprocessed.asm");
            CreateStatements();
            AssignLineNumbers();
            WriteIntermediateFile("fixlabels.txt");
            SplitOperands();
            WriteParsedOperands("operands.txt");
            ValidateOpCodes();
            ValidateOperands();
            
            AssignModuleNumbers();
            AssignAddresses();
            GetLabelAddresses();
            
            SetAddresses();
            FixRelativeAddresses();
            ReplaceDBWithAddresses();
            WriteIntermediateFile("pass2.txt");
            SetImmediateSizes();
            //        ValidateBinaryLengths();
            FixBitInstructions();
            SetHexText();
            WriteListFile(listFile);
            WriteLabelTable(listFile);
            WriteBinary(outputFile);
        }

        void WriteIntermediateFile(string filename)
        {
            File.WriteAllLines(filename, lines);

        }


        void WriteGlobalSymbols()
        {
            globalLabels.Sort();

            using (StreamWriter sw = new StreamWriter("globalsyms.txt", false))
            {
                foreach (var s in globalLabels)
                {
                    sw.WriteLine(s);
                }

            }
        }

        void WriteSymbolTable(StreamWriter sw)
        {
            sw.WriteLine("Symbol table____________________");

            foreach (string s in defines.Keys)
            {
                sw.WriteLine("{0}\t{1}", s, defines[s]);
            }

            sw.WriteLine("End symbol table____________________");
        }

        void WritePreprocessedFile(string fileName)
        {
            File.WriteAllLines(fileName, lines.ToArray());
        }


        void WriteLabelTable(string fileName)
        {
            using (StreamWriter sw = new StreamWriter(File.Open(fileName, FileMode.Append)))
            {
                int count = 0;
                sw.WriteLine();
                sw.WriteLine();
                sw.WriteLine("GLOBAL SYMOBLS:");
                foreach (string s in  labelOffsets.Keys)
                {
                    sw.Write(
                        String.Format("{0}:{1:X4}\t", s, labelOffsets[s])
                        );


                    count++;
                    if (count == 10)
                        sw.WriteLine();
                }

                count = 0;
                sw.WriteLine();
                sw.WriteLine();
                sw.WriteLine();
                sw.WriteLine("LOCAL SYMOBLS:");
                foreach (string s in labelOffsets.Keys)
                {
                    sw.Write(s + ":" + labelOffsets[s] + "\t");
                    count++;
                    if (count == 10)
                        sw.WriteLine();
                }

            }




        }

        /// <summary>
        /// Assigns a line number to each statement
        /// </summary>
        void AssignLineNumbers()
        {
             
            for (int i = 0; i < statements.Count; i++)
            {
                statements[i].LineNumber = i + 1;
            }
        }

        /// <summary>
        /// Creates a statement for every line in the file
        /// </summary>
        public void CreateStatements()
        {
            try
            {
                int i = 0;
                foreach (string str in lines)
                {
                    try
                    {
                        i++;
                        string label = "";
                        string s = "";
                        //if this line starts with a label, save it and replace it with a tab
                        if (str.StartsWithLabel() || str.StartsWith("@"))
                        {
                            label = str.GetFirstPart();
                            s = "\t" + str.GetRest();
                        }
                        else
                        {
                            s = str;
                        }

                        if (s.Trim() == "")
                            statements.Add(new BlankLine());
                        else if (Regex.IsMatch(s, "[\t ]ORG[\t ]"))
                            statements.Add(new OrgStatement(s));
                        else if (s.Trim().StartsWith(";"))
                            statements.Add(new Comment(s));
                        else if (s.Trim().StartsWith("*MOD"))
                            statements.Add(new Module(s));
                        else if (s.StartsWith("#DEFINE"))
                            statements.Add(new DefineStatement(s));
                        else if (Regex.IsMatch(s, @"(^|[\t ])DB[\t ]"))
                            statements.Add(new DBStatement(s, defines));
                        else if (Regex.IsMatch(s, @"[\t ]DW[\t ]"))
                            statements.Add(new DWStatement(s, defines));
                        else if (Regex.IsMatch(s, @"[\t ]DS[\t ]"))
                            statements.Add(new RESBStatement(s));
                        else if (Char.IsLetter(s.First()))
                            statements.Add(new GlobalLabel(s));
                        else
                            statements.Add(new ExecutableStatement(s, globalLabels));

                        //put the label back on the statement
                        if (label != "")
                            statements.Last().Label = label;
                        //could also have EQU or include     
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Line " + i + ": Unable to parse:" + str, ex);
                    }
                }
            }
            catch (Exception x)
            {
                throw new Exception("Error creating statements.", x);
            }
        }

        List<Statement> statements;
        List<String> lines = new List<string>();
        Dictionary<string, string> defines = new Dictionary<string, string>();
        
    }
}
