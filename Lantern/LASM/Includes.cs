using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace LASM
{
    partial class Assembly
    {
          
        /// <summary>
        /// Recursively inserst a file's contents into lines list.
        /// </summary>
        /// <param name="fileName"></param>
        void IncludeFile(string includeStatement)
        {
            string fileName = "";
            
            if (fileName.StartsWith("#define"))
            {
                fileName = includeStatement.Substring(8).Trim();
            }
            else
            {
                fileName = includeStatement.Trim();
            }

            if (files.Contains(fileName))
            {
                throw new Exception("File " + fileName + " has already been included!");
            }

            files.Add(fileName);



            if (File.Exists(fileName))
            {
                string[] moreLines = File.ReadAllLines(fileName);

                for (int i=0; i < moreLines.Length; i++)
                {
                    if(moreLines[i].StartsWith("*INCLUDE"))
                    {
                        string newFileName = moreLines[i].Substring(8).Trim();
                        IncludeFile(newFileName);
                    }
                    else
                    {
                        lines.Add( UpperCase(moreLines[i]));
                    }
                }

            }
            else
                throw new Exception("Unable to open file: " + fileName);
        }

    }
}
