// TSCL - Tezz's Simple Configuration Language
// Copyright (C) 2026 Patrick Cortez
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

// TSCL Modify Operation
using static TSCL.Initialize;
using static TSCL.utils.Utility;

namespace TSCL.operations
{
    /// <summary>
    /// This class handles all file modification in TSCL.
    /// If you want to change a value of an object.
    /// </summary>
    public class Modify //Modify object in file
    {
        string fname,Section;
        List<string> lines;
        bool found = false;
        bool isValid = true;

        /// <summary>
        /// Initializes the file and reads the file for modification.
        /// </summary>
        /// <param name="src">Your file you want to modify</param>
        /// <param name="TargetSection">Target section you want to modify, so TSCL will only read that section</param>
        /// <exception cref="FileNotFoundException">if the file doesnt exist then we cant modify it</exception>
        /// <exception cref="Exception">if the file exists but the section does not, so we cant modify a non existent section</exception>
        public Modify(string src,string TargetSection) // Modify class constructor
        {
            if (FileName == null && isUniversal)
            {
                Warn("File not set!");
            }

            if (isUniversal)
            {
                fname = FileName; // if universal is true, then we use the filename from the initializer
            }
            else
            {
                if (!File.Exists(src))
                {
                    Warn($"File: {src} does not exist!");
                }

                if (Path.GetExtension(src) != ".tscl")
                {
                    Warn($"File: {src} is not a tscl file");
                }

                fname = src;
            }

            Section = TargetSection;
            lines = new List<string>();
            initiateRead(); //we start our file reading immediately

            if (!found) //if Section Name doesnt exist we throw in an error
            {
                throw new Exception($"Section: {Section} does not exist!");
            }
        }
        /// <summary>
        /// Sets the current section you want to modify
        /// </summary>
        /// <param name="SectionName">Name of the sectiob you want to modify</param>
        /// <exception cref="ArgumentNullException">if the section name is empty, we throw an error</exception>

        public void SetSection(string SectionName) // This is where we set our target section.
        {

            if(SectionName == string.Empty) 
            {
                throw new ArgumentNullException("SectionName cannot be null!");
            }

            Section = SectionName;
        }

        /// <summary>
        /// starts reading the file line by line.
        /// </summary>

        private void initiateRead() // File Reading Line by line
        {
            string curr = string.Empty,line = string.Empty;
            bool othersection = true;
            using (StreamReader read = new StreamReader(fname))
            {
                while ((line = read.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(line)) // We skip every white space
                    {
                        continue;
                    }

                    if (line.StartsWith('[')) // Section handling
                    {
                        curr = line.TrimStart('[').TrimEnd(']');


                        if (curr == Section) // we must check if the current section is our target section, otherwise we are in a different section
                        {
                            othersection = false;
                            found = true;
                        }

                        othersection = true;
                    }

                    if (!othersection) // we only append to our line list if we are on our target section
                    {
                        lines.Add(line);
                    }
                    else
                    {
                        continue;
                    }
                }
            }

            
        }

        /// <summary>
        /// Modifies the value of an existing object identified by the specified key within the current section.
        /// </summary>
        /// <param name="key">Name of the object you want to modify</param>
        /// <param name="newvalue">Tnew value of the object</param>
        /// <exception cref="Exception">Thrown if the current section does not contain any objects.</exception>

        public void ModifyObject(string key, string newvalue) // Modify an Object in a section.
        {
            string newLine = string.Empty;
            if (lines.Count < 1)
            {
                throw new Exception($"Section: {Section} does not have any objects!");
            }

            for (int x = 0; x < lines.Count(); x++)
            {
                char[] sep = { '=' };
                string[] words = Tokenize(lines[x], sep,ref isValid);

                if (words[0] == key) // We modify if were at the right object.
                {
                    words[1] = newvalue;
                    newLine = string.Join("=", words); // join our tokenized objects.
                    lines[x] = newLine; //then we replace the old with the new.
                    break;
                }
            }

            using(StreamWriter sw = new StreamWriter(fname,false))
            {
                foreach(var line in lines)
                {
                    sw.WriteLine(line);
                }
            }

        }
        
        /// <summary>
        /// To modify a section name.
        /// </summary>
        /// <param name="oldName">Target section</param>
        /// <param name="newName">New name for the target section</param>
        public void modifySectionName(string oldName,string newName)
        {
            for(int x = 0;x < lines.Count; x++)
            {
               
                if (isSect(lines[x]))
                {
                    string curr = lines[x].TrimStart('[').TrimEnd(']');
                    if (curr == oldName)
                    {
                        string nStr = String.Concat('[', newName, ']');
                        lines[x] = nStr;
                        break;
                    }
                }
            }

            using (StreamWriter sw = new StreamWriter(fname, false))
            {
                foreach (var line in lines)
                {
                    sw.WriteLine(line);
                }
            }
        }

    }
}
