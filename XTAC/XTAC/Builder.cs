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
using System.Runtime.InteropServices;

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

                RenameBinary("main.bin", binName);
                MoveOutputFiles();

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

    
        /// <summary>
        /// Writes out a cmd file with a 01 02 loadLo loadHi header
        /// </summary>
        /// <param name="fileName"></param>
        public static string BuildTRS80(string xmlFileName)
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
            string binFile = outputName + ".cmd";

            try
            {
                string workingDirectory = list[0].InnerText + "_TRS80";

                Environment.CurrentDirectory = workingDirectory;
                Assembly asm = new Assembly("main.asm");
                asm.Assemble();

                //add the header to make it a .cmd file
                byte[] fileData = File.ReadAllBytes("main.bin");
                ushort loadAddr = 0x5200;

                BinaryWriter bw = new BinaryWriter(File.OpenWrite(binFile));
 
                try
                {
                    for (int i=0; i < fileData.Length; i++)
                    {
                        if (i % 256 == 0)
                        {//write out a record header
                            bw.Write((byte)0x01); //record type
                            int remaining = fileData.Length - i;
                            if (remaining >=256)
                            {
                                bw.Write((byte)0x02); //number of bytes to follow
                            }
                            else
                            {
                                bw.Write((byte) (remaining+2)); //number of bytes to follow
                            }
                            bw.Write((byte)(loadAddr % 256)); //lo
                            bw.Write((byte)(loadAddr / 256)); //hi
                            loadAddr += 256;
                            bw.Flush();
                        }

                        bw.Write((byte)fileData[i]);

                    }
                    bw.Write((byte)2);
                    bw.Write((byte)2);
                    bw.Write((byte)0);
                    bw.Write((byte)(0x52));
                    bw.Flush();
                }
                catch (Exception ex1)
                {
                    Console.WriteLine("Huh?");
                }
                finally
                {
                    bw.Flush();
                    bw.Dispose();
                }
                MoveOutputFiles();

                return binFile;
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

        /// <summary>
        /// Assembles code and runs CPCDiskXP to create disk file
        /// </summary>
        /// <param name="xmlFileName"></param>
        /// <returns></returns>
        public static string BuildCPC464(string xmlFileName)
        {
            string oldDir = Environment.CurrentDirectory;
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(xmlFileName);
                XmlNodeList list = doc.SelectNodes("//project/projname");

                if (list.Count == 0)
                {
                    throw new Exception("Project XML does not have a name element!");
                }

                string projName = doc.SelectNodes("//project/projname")[0].InnerText.Trim();
                string outputName = doc.SelectNodes("//project/output")[0].InnerText.Trim();

                Environment.CurrentDirectory = projName + "_CPC464";

                string diskName = outputName + ".dsk";
                string fileName = outputName + ".bin";

                try
                {
                    File.Delete(fileName);

                    Assembly lasm = new Assembly("main.asm");
                    lasm.Assemble();
                    //rename output to 'data'
                    File.Move("main.bin", fileName);
                }
                catch (Exception ex)
                {
                    throw new Exception("Error assembling main.asm", ex);
                }

                //try to run the CPCDiskXP 

                string diskCmd = string.Format(
                        "{0} -File {1} -AddAmsdosHeader 4000 -AddToNewDsk {2} -NewDSKFormat 1",
                        @"..\bin\CPCDiskXP",
                        fileName,
                        diskName
                        );

                File.WriteAllText("build.bat", diskCmd);

                Process.Start("build.bat").WaitForExit();

                if (!File.Exists(diskName))
                {
                    throw new Exception("Error creating disk.  Couldn't find " + diskName);
                }
                
                MoveOutputFiles();
                return outputName;
            }
            catch (Exception ex)
            {
                throw new Exception("CPC464 build failed.", ex);
            }
            finally
            {
                Environment.CurrentDirectory = oldDir;
            }

        }

        /// <summary>
        ///Assemble the files produced by the converter 
        /// </summary>
        /// <param name="xmlFileName"></param>
        public static string BuildSpeccy(string xmlFileName)
        {
            string oldDir = Environment.CurrentDirectory;
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(xmlFileName);
                XmlNodeList list = doc.SelectNodes("//project/projname");

                if (list.Count == 0)
                {
                    throw new Exception("Project XML does not have a name element!");
                }

                string projName = doc.SelectNodes("//project/projname")[0].InnerText.Trim();
                string outputName = doc.SelectNodes("//project/output")[0].InnerText.Trim();

                Environment.CurrentDirectory = projName + "_Spectrum";

                try
                {
                    File.Delete("data");

                    Assembly lasm = new Assembly("main.asm");
                    lasm.Assemble();
                    //rename output to 'data'
                    File.Move("main.bin", "data");
                }
                catch (Exception ex)
                {
                    throw new Exception("Error assembling main.asm", ex);
                }

                //try to run the mctrd script    
                Process.Start("build.bat").WaitForExit();

                if (File.Exists("game.tap"))
                {
                    RenameBinary("game.tap", outputName + ".tap");
                }
                else
                {
                    throw new Exception("Couldn't find game.tap. Must be on Windows to run mctrd?");
                }

                File.Delete("loader.tap");
                File.Delete("sloader.tap");
                File.Delete("build.bat");

                MoveOutputFiles();
                return outputName + ".tap";

            }
            catch (Exception ex)
            {
                throw new Exception("Spectrum build failed.", ex);
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

            
        static void RenameBinary(string oldName, string newName)
        {
            if (File.Exists(newName))
                File.Delete(newName);

            File.Copy(oldName, newName);
            File.Delete(oldName);
        }

        static void MoveOutputFiles()
        {
            try
            {

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
        }
        
    }
}
