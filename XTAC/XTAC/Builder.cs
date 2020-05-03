using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using LASM;

namespace XTAC
{
    class Builder
    {
        public static void BuildPortable(string xmlFileName, out string name)
        {
            
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlFileName);
            XmlNodeList list = doc.SelectNodes("//project/projname");

            if (list.Count == 0)
            {
                throw new Exception("Project XML does not have a name element!");
            }

            string oldDir = Environment.CurrentDirectory;
            string projName = doc.SelectNodes("//project/projname")[0].InnerText.Trim();
            string outputName = doc.SelectNodes("//project/output")[0].InnerText.Trim();

            try
            {
                string workingDirectory = list[0].InnerText + "_VM";

                Environment.CurrentDirectory = workingDirectory;
                Assembly asm = new Assembly("main.asm");
                asm.Assemble();

                //rename the output
                string binName = outputName;
                if (outputName == "")
                {
                    binName = projName.Replace(" ", "_");
                }

                try
                {
                    if (File.Exists(binName))
                        File.Delete(binName);

                    File.Copy("main.bin", binName);
                    File.Delete("main.bin");

                    if (!Directory.Exists("src"))
                        Directory.CreateDirectory("src");
    
                    //delete any existing files or move will fail
                    string[] files = Directory.GetFiles("src");
                    foreach (string f in files)
                    {
                        File.Delete(f);
                    }

                    files = Directory.GetFiles(".");
                    foreach (string f in files)
                    {
                        if (f.EndsWith(".asm") || f.EndsWith(".lst") || f.EndsWith(".txt"))
                            File.Move(f, "src\\" + f);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Unable to rename output file", ex);
                }
                //return name of file created to the caller                
                name = binName;
            }
            catch (Exception e)
            {
                throw new Exception("Couldn't assemble main.asm", e);
            }
            finally
            {
                Environment.CurrentDirectory = oldDir;
            }
            
        }
        

        public static bool Build(string xmlFileName, string platform, string outputName, string extension)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlFileName);
            XmlNodeList list = doc.SelectNodes("//project/projname");

            if (list.Count == 0)
            {
                throw new Exception("Project XML does not have a name element!");
            }

            string oldDir = Environment.CurrentDirectory;
            bool success = false; 
            try
            {
                string workingDirectory = list[0].InnerText + platform;
                
                Environment.CurrentDirectory = workingDirectory;

                string defaultName = "main" + "." + extension;
                string newName = outputName.ToLower() + "." + extension;

                File.Delete(defaultName);
                File.Delete(newName);

                if (File.Exists("template.sh"))
                {
                    ReplaceNames("template.sh", outputName.ToLower(), "build.sh");
                }
                success = true;
            }
            finally
            {
                Environment.CurrentDirectory = oldDir;
            }
            return success;
        }

        /// <summary>
        /// Renames the loader and dat file to the OUPUT name
        /// Creates a copy of the post build script, then
        /// replaces the generic file names with the OUTPUT name.
        /// Finally, the post-build script is run to attach the
        /// files to the disk image, and the disk image is renamed
        /// to the OUTPUT name.
        /// </summary>
        /// <param name="xmlFileName"></param>
        /// <param name="outputName"></param>
        /// <returns></returns>
        public static bool CoCoBuild(string xmlFileName, string outputName)
        {
            bool success = false;
            string oldDir = Environment.CurrentDirectory;

            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(xmlFileName);
                XmlNodeList list = doc.SelectNodes("//project/projname");

                string workingDirectory = list[0].InnerText + "_CoCo";

                Environment.CurrentDirectory = workingDirectory;
                
                string text = File.ReadAllText("template.sh");
                StringBuilder sb = new StringBuilder(text);
                sb.Replace("OUTPUT", outputName.ToLower());
                File.WriteAllText("build.sh", sb.ToString());

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                Environment.CurrentDirectory = oldDir;
            }
            return success;
        }
        
        

        public static void ReplaceNames(string infile, string outName, string outFile)
        {
            string text = File.ReadAllText(infile);
            StringBuilder sb = new StringBuilder(text);
            sb.Replace("OUTPUT", outName.ToLower());
            File.WriteAllText(outFile, sb.ToString());
        }

    
    }
}
