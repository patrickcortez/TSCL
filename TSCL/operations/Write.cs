// TSCL Write Operation
using static TSCL.utils.Utility;
using static TSCL.Initialize;



namespace TSCL.operations
{
    /// <summary>
    ///  This Class is responsible for Serializing TSCL Format/Structure 
    ///  to a .tscl file.
    /// </summary>
    public class Write // write to tscle file
    {
        string filename = string.Empty; //source file
        Dictionary<string, List<Token>> TSCL;

        string pos = string.Empty;

        /// <summary>
        /// Constructor Initializes the members.
        /// </summary>
        /// 
        /// <exception cref="Exception"> If the file doesn't have the .tscl extension then we throw in an Error</exception>

        public Write(string src = "")
        {
            if(FileName == null && isUniversal)
            {
                Warn("File not set!");
            }

            if (isUniversal) // if universal is true, we use initializers file path
            {
                filename = FileName; // pass sourcefile on Initialization
            }
            else
            {
                if (!File.Exists(src))
                {
                    Warn($"File: {src} does not exist!");
                }

                if(Path.GetExtension(src) != ".tscl")
                {
                    Warn($"File: {src} is not a tscl file");
                }

                filename = src;
            }


            TSCL = new Dictionary<string, List<Token>>();
        }

        /// <summary>
        /// Adds a Section to the Dictionary 'TSCL'
        /// </summary>
        /// <param name="name"> The name of the section you want to add</param>
        /// <exception cref="ArgumentNullException">The section name cannot be empty</exception>

        public void AddSection(string name) // Adding sections to TSCL Map
        {
            if (name == string.Empty) // if sec name is empty
            {
                throw new ArgumentNullException("Section Name Cannot be NUll!");
            }

            name = clearSpace(name); //clear all spaces just to be sure.

            TSCL[name] = new List<Token>();
        }

        /// <summary>
        /// Setting the current section to add objects in.
        /// </summary>
        /// <param name="name">Name of the section you want to add objects in</param>
        /// <exception cref="IndexOutOfRangeException">if Section Name isnt on the map, we throw in an error</exception>

        public void SetSection(string name) //to change current position
        {

            if (!TSCL.ContainsKey(name)) //but make sure it exists
            {
                throw new IndexOutOfRangeException($"Section: {name} does not exist!");
            }

            pos = name;
        }
        /// <summary>
        /// A Tracker for the Write class that tracks the current session in the dictionary.
        /// </summary>
        /// <returns>It returns the current list of tokens</returns>
        private List<Token> Current() // our tracker
        {
            return TSCL[pos];
        }

        /// <summary>
        /// Adds an object to the current section.
        /// </summary>
        /// <param name="key">Name of object</param>
        /// <param name="data">Value of the object</param>
        /// <param name="tokentype">Token type of the object: Object or Pointer</param>

        public void AddObject(string key, object data, Types tokentype) //adding objects
        {
            if (tokentype == Types.OBJECT)
            {
                Current().Add(new Token(tokentype, key, data));
            }
            else if (tokentype == Types.POINTER)
            {
                Current().Add(new Token(tokentype, key, data));
            }
        }

        /// <summary>
        /// Adds an array of string to the section
        /// </summary>
        /// <param name="key">Name of the array</param>
        /// <param name="datas">Elements of the array</param>

        public void AddArray(string key, string[] datas) //for adding arrays 
        {
            Current().Add(new Token(Types.ARRAY, key, null, datas, true));
        }

        /// <summary>
        /// Write the entire Section to file once
        /// changes are done and final.
        /// </summary>
        /// <param name="append">Set to true if you want to append or not(default is false)</param>

        public void WriteToFile(bool append = false) //Write current section to file, programmers mut manually change section then write to file
        {
            List<string> lines = new List<string>();

            foreach (KeyValuePair<string, List<Token>> section in TSCL) //our line writer which grabs the current keyvalue in our dict, then iterates through all our tokens
            {
                lines.Add($"[{section.Key}]");
                foreach (Token tok in section.Value)
                {
                    if (tok.tokentype == Types.OBJECT) //write object to file
                    {
                        lines.Add($"{tok.obj.key}={tok.obj.data}");
                    }
                    else if (tok.tokentype == Types.ARRAY) //write array
                    {
                        lines.Add($"{tok.arr.key}={string.Join(',', tok.arr.data)}");
                    }
                    else if (tok.tokentype == Types.POINTER) //write Pointer
                    {
                        lines.Add($"{tok.obj.key}=@{tok.obj.data}");
                    }
                }

                lines.Add(Environment.NewLine); //newline after every section for proper formatting
            }

            using (StreamWriter sw = new StreamWriter(filename, append)) // write all changes to file.
            {           
                foreach(var line in lines)
                {
                    sw.WriteLine(line);
                }

                sw.Close(); // close the stream after we wrote all the lines to file
            }
        }
        


    }
}
