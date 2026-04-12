// TSCL Read Operation
using static TSCL.utils.Utility;
using static TSCL.utils.Typecaster;
using static TSCL.Initialize;

namespace TSCL.operations
{

    /// <summary>
    /// This class is responsible for Deserializing/Reading contents of a .tscl file.
    /// </summary>
    public class Read // read from file
    {
        string filename = string.Empty; //source file
        Dictionary<string, List<Token>> tokens = new Dictionary<string, List<Token>>(); //list of sections: tokens["Section-name"]
        string pos = "";
        HashSet<string> visited;
        bool isvalid = true;

        /// <summary>
        /// an indicator of the which section, its currently in
        /// </summary>
        /// <returns>It returns the current sections objects</returns>
        private List<Token> current()
        {
            return tokens[pos];
        }

        /// <summary>
        /// changes the current section to what the programmer/dev wants
        /// </summary>
        /// <param name="nextsec">Name of the section the programmer/dev wants to advance</param>
        private void advance(string nextsec) //Advance to the next section in the map.
        {
            pos = nextsec;
        }

        /// <summary>
        /// Initializes a new instance by reading the .tscl file.
        /// </summary>
        /// <param name="fname">name of File(if universal is false)</param>
        /// <exception cref="FileNotFoundException"></exception>
        public Read(string fname = "")
        {

            if (FileName == null && isUniversal)
            {
                Warn("File not set!");
            }

            if (isUniversal == true) //if universal option is true, we use the file path from initializer
            {

                filename = FileName; // pass sourcefile on Initialization
            }
            else
            {
                if (!File.Exists(fname))
                {
                    Warn($"File: {fname} does not exist!");
                }

                if (Path.GetExtension(fname) != ".tscl")
                {
                    Warn($"File: {fname} is not a tscl file");
                }

                filename = fname;
            }

            visited = new HashSet<string>();
            initilaizeRead();
        }

        /// <summary>
        /// Reads entire tscl file line by line and tokenizes it.
        /// </summary>
        private void initilaizeRead() // TSCL tokenizer, THIS took me an hour to think of. 
        {
            List<Token> tmp = new List<Token>(); // list of all objects in the section
            string SectionName,line;
            using (StreamReader read = new StreamReader(filename))
            {
                while ((line = read.ReadLine()) != null) //read through every line and skip whitespaces
                {

                    if (string.IsNullOrWhiteSpace(line)) //skip if line is a empty newline
                    {
                        continue;
                    }

                    if (line.StartsWith('#')) //We skip comments
                    {
                        continue;
                    }


                    char[] seperators = { '=', ',' };
                    string[] words = Tokenize(line, seperators,ref isvalid);

                    if (words[0].StartsWith('[') && words[0].EndsWith(']')) //Section
                    {
                        if (tmp.Count > 0)
                        {
                            current().AddRange(tmp);
                            tmp.Clear();
                        }

                        SectionName = words[0].TrimStart('[').TrimEnd(']');
                        pos = SectionName;

                        if (!tokens.ContainsKey(SectionName)) //initialize the section in the map
                        {
                            tokens[SectionName] = new List<Token>();
                        }

                        if (isVerbose)
                        {
                            advanceline();
                        }
                    }
                    else if (words.Length > 1)
                    {
                        if (words[1].StartsWith('@')) // Section poiner: points to other sections
                        {
                            tmp.Add(new Token(Types.POINTER, words[0], words[1].TrimStart(']')));
                            continue;
                        }
                        else if (words[1].Contains(',')) //arrays (they are string and string only)
                        {
                            string[] subs = words[1].Split(',');

                            tmp.Add(new Token(Types.ARRAY, words[0], string.Empty, subs, true));

                            if (isVerbose)
                            {
                                advanceline();
                            }

                            continue;
                        }
                        else if (!words[1].Contains(',')) // non array object
                        {
                            bool res;
                            if (isInt(words[1])) //integer
                            {
                                tmp.Add(new Token(Types.OBJECT, words[0], Strint(words[1])));
                            }
                            else if (bool.TryParse(words[1], out res)) //boolean 
                            {
                                tmp.Add(new Token(Types.OBJECT, words[0], ConvertBool(words[1])));
                            }
                            else //string
                            {
                                tmp.Add(new Token(Types.OBJECT, words[0], words[1]));
                            }
                            if (isVerbose)
                            {
                                advanceline();
                            }
                            continue;

                        }
                    }
                    else // if line is invalid we just mark them
                    {
                        if (isVerbose)
                        {
                            advanceline();
                            markLine(lineN0);
                        }
                        continue;
                    }


                }
            }

            if (tmp.Count > 0)
            {
                current().AddRange(tmp); //final flush before resetting our current position
            }

            if(markedLines.Count > 0 && isVerbose)
            {
                Console.WriteLine("Invalid Lines Ignored:",Console.Error);

                foreach (int lin in markedLines)
                {
                    Console.WriteLine($"Line {lin}", Console.Error);
                }
            }
            //reset line for the next script
            if (isVerbose)
            {
                resetLine();
                ClearLine();
            }
            pos = string.Empty; //reset position once done

        }

        /// <summary>
        /// Gets all of the objects in a section
        /// </summary>
        /// <param name="section_name">Name of the section you want to the objects of</param>
        /// <returns>It returns the objects of the sections as an array of Tokens</returns>
        public Token[] getSectionObjects(string section_name = "") // get all Objects from a section, User can either use the current section or traverse to another.
        {

            if (section_name != "") { // if User wants objects from other sections we advance to that section.
                advance(section_name);
            }

            Token[] toks = current().ToArray(); 

            return toks;
        }

        /// <summary>
        /// Sets the current section to the specified name.
        /// </summary>
        /// <param name="Secname">Name of the section you want to start with</param>
        /// <exception cref="IndexOutOfRangeException">Thrown if section is not in Dictionary</exception>

        public void setSection(string Secname) // manual Section setup, to control the flow and starting
        {
            if (!tokens.ContainsKey(Secname))
            {
                throw new IndexOutOfRangeException($"Section Name: {Secname} doesnt exist!");
            }

            pos = Secname;
        }
        /// <summary>
        /// It handles Pointers by advancing to that section
        /// </summary>
        /// <param name="Point">Name of the section</param>
        /// <returns>It returns the entire objects of the said section</returns>
        /// <exception cref="Exception">Thrown if the pointers value isnt a section</exception>
        private Token[] handlePointer(string Point)
        {
            string oldPos = pos;
            advance(Point);

            if (visited.Contains(Point)) // infinite loop safeguard
            {
                throw new Exception("Invalid Pointer Value!");
            }
            else // Add every section traversed
            {
                visited.Add(Point);
            }

            Token[] tmp = current().ToArray();
            pos = oldPos;
            return tmp;
        }

        /// <summary>
        /// It grabs the specified value of an object in the current section
        /// </summary>
        /// <param name="key">Name of the object</param>
        /// <param name="PointerObjKey">Name of the object if the object is in a pointer</param>
        /// <returns>returns value of the objects</returns>
        /// <exception cref="Exception">Thrown if the object is of null value</exception>
        internal object getObjectData(string key,string targect_section) // key and Pointer Object key for grabbing objects in another section(Empty by default)
        {
            object data = null;

            foreach (KeyValuePair<string, List<Token>> keyval in tokens)
            {
                if(keyval.Key != targect_section)
                {
                    continue;
                }

                foreach (Token tok in keyval.Value)
                {
                    if (tok.tokentype == Types.OBJECT && tok.obj.key == key)
                    {
                        data = tok.obj.data;
                        break;
                    }
                    else if (tok.tokentype == Types.POINTER && tok.obj.key == key)
                    {
                        string target = ((string)tok.obj.data).TrimStart('@');
                        data = handlePointer(target);

                    }
                }
            }

            if (data is Token[] tokarr) // pointer handling for all its objects
            {
                return tokarr;
            }

            if (data is null)
            {
                throw new Exception($"Object: {key} is null!");
            }

            if(data is int || data is bool)
            {
                return data;
            }

            return data.ToString(); // we return the data in a string by default
        }

        /// <summary>
        /// It gets the values of an array object
        /// </summary>
        /// <param name="key">name of the array</param>
        /// <returns>It returns an Array of strings</returns>
        /// <exception cref="Exception">Thrown if the arrays size is 0</exception>

        internal string[] getArrayData(string key,string target_section) //array handler
        {

            if(target_section != pos)
            {
                advance(target_section);
            }

            List<Token> tmp = current(); // get our list of objects to find the said array
            string[]? datas = null;

            foreach (Token tok in tmp) // iterate through every token, to find the said array:
            {
                if (tok.tokentype == Types.ARRAY && tok.arr.key == key)
                {
                    datas = tok.arr.data.ToArray();
                    break;
                }
            }

            if (datas == null)
            {
                throw new Exception("Array size cannot be empty!");
            }

            return datas; // once array is found we return the array to the user
        }


        /// <summary>
        /// Retrieves the value of a specified object in a specific section
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">Name of the object</param>
        /// <param name="target_section">Name of the section</param>
        /// <returns></returns>
        public T GetValue<T>(string key,string target_section) // Gets objects value then typecasts it  
        {
                object raw = getObjectData(key, target_section); //we call the internal method to pass the data to cast objects to convert it to the type of the object
                return CastObject<T>(raw); // converts the type of the object
            
        }

        /// <summary>
        /// Retrieves the value of the array in a specific section.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T[] GetArrayValue<T>(string key,string target_section) //gets arrays value then type casts it
        {
            string[] rawstr = getArrayData(key,target_section); // we call the internal method to store the arrays contents

            return rawstr.Select(str => CastObject<T>(str)).ToArray(); // then each one is being determined the type based on the type of the object
        }



    }
}
