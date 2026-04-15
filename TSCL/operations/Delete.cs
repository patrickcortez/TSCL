using System.Security.AccessControl;
using static TSCL.utils.Utility;

namespace TSCL.operations
{
    /// <summary>
    /// For Delete operations in tscl.
    /// This Class is responsible for all
    /// Object and Section Deletions
    /// </summary>
    internal class Delete // For Deletion Operations
    {
        private readonly string file; // File path
        bool isvalid = true; // isvalid checker
        string[] Skip; // Sections to skip
        List<string> lines; // store entire lines 

        /// <summary>
        /// Initializer of Delete class, upon initializing
        /// the class immediately reads the file provided
        /// </summary>
        /// <param name="src"> .tscl file you want to work with</param>
        /// <param name="SkipSections">Sections you want to skip over</param>
        public Delete(string src,params string[] SkipSections) // Constructor
        {
            file = src; // Store Filepath the user provided
            Skip = SkipSections; //As well as the skip sections
            InitializeRead(); // we immediately read the whole file when initializing
        }

        /// <summary>
        /// File reading of Delete class
        /// </summary>
        private void InitializeRead() // Start reading the whole file
        {
            using (StreamReader sr = new StreamReader(file)) // We read the entire file line by line and skip everything in skip array
            {
                List<string> tmp = new List<string>();
                string line = string.Empty;
                bool inSkipSections = false;
                while((line = sr.ReadLine()) != null)
                {

                    if (string.IsNullOrWhiteSpace(line)) // we skip new lines
                    {
                        continue;
                    }

                    if(line.StartsWith('[') && line.EndsWith(']')) // if the current line is a section we check if the current is in the skip
                    {
                        string current = line.TrimStart('[').TrimEnd(']');

                        if (Skip.Contains(current))
                        {
                            inSkipSections = true;
                        }
                        else
                        {
                            inSkipSections = false;
                        }
                    }

                    if (inSkipSections) // if so we ignore the entire Section until we hit a new one.
                    {
                        continue;
                    }

                    lines.Add(line);
        
                }
            }

        }
        /// <summary>
        /// Delete an object from tscl file
        /// </summary>
        /// <param name="objectname">Name of the object</param>
        public void DeleteObject(string objectname,string Section) //delete a singular object in a section
        {
            char[] sep = { '=' }; //for objects: obj="value"
            for(int x = 0;x < lines.Count; x++)
            {
                

                if (lines[x].StartsWith('[') && lines[x].EndsWith(']'))
                {
                    string current = lines[x].TrimStart('[').TrimEnd(']');
                    List<string> line = Tokenize(lines[x], sep, ref isvalid).ToList(); // store each obj value in a List of strings

                    if (current == Section)
                    {
                        if (line[0] == objectname) //then compare the first element which is the Object name to the parameter
                        {
                            lines.RemoveAt(x); // if it is, then we remove.
                        }
                    }
                }
            }

            using (StreamWriter sw = new StreamWriter(file,false)) // finally we store everything
            {
                foreach(var line in lines)
                {
                    sw.WriteLine(line);
                }
            }
        }

        /// <summary>
        /// Delete an Entire Section from the .tscl file
        /// </summary>
        /// <param name="Section">name of the section</param>
        public void DeleteSection(string Section) // remove every object in a section
        {
            bool isInSect = false; // indicator
            for (int x = 0; x < lines.Count; x++)
            {
  

                if (lines[x].StartsWith('[') && lines[x].EndsWith(']')) // check if we are in a section
                {
                    string current = lines[x].TrimStart('[').TrimEnd(']'); // if we are we store the current section in a string

                    if (current == Section) // if we are in the target, set the indicator to true.
                    {
                        isInSect = true;

                    }
                    else
                    {
                        isInSect = false;
                    }

                }

                if (isInSect == true) // remove everything in the Section.
                {
                    lines.RemoveAt(x);
                }
            }

            using (StreamWriter sw = new StreamWriter(file, false)) // then save everything
            {
                foreach (var line in lines)
                {
                    sw.WriteLine(line);
                }
            }
        }
    }
}
