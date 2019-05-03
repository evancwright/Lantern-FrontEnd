using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace XTAC
{
    class Builder
    {
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
                  
                Process.Start("build.sh");

                if (!File.Exists(newName))
                {
                    MessageBox.Show("Build failed. Run build.sh manually in the project directory and check console for errors.");
                }

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
