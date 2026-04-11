// TSCL Write Operation
using static TSCL.utils.Utility;


namespace TSCL.operations
{
    public class Write : IDisposable // write to tscle file
    {
        string filename = string.Empty; //source file
        Dictionary<string, List<Token>> TSCL;

        string pos = string.Empty;

        public void Dispose() // clean up after instance is finished
        {
            TSCL.Clear();
            filename = string.Empty;
        }

        public Write(string src)
        {
            if (!File.Exists(src))
            {
                File.Create(src); // make the file for the user
            }
            else if (Path.GetExtension(src) != ".tscl")
            {
                throw new Exception($"File: {Path.GetFileName(src)} is not a TSCL File!");
            }

            filename = src; // pass sourcefile on Initialization
            TSCL = new Dictionary<string, List<Token>>();
        }

        public void AddSection(string name) // Adding sections to TSCL Map
        {
            if (name == string.Empty) // if sec name is empty
            {
                throw new ArgumentNullException("Section Name Cannot be NUll!");
            }

            name = clearSpace(name); //clear all spaces just to be sure.

            TSCL[name] = new List<Token>();
        }

        public void SetSection(string name) //to change current position
        {

            if (!TSCL.ContainsKey(name)) //but make sure it exists
            {
                throw new IndexOutOfRangeException($"Section: {name} does not exist!");
            }

            pos = name;
        }

        private List<Token> Current() // our tracker
        {
            return TSCL[pos];
        }

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

        public void AddArray(string key, string[] datas) //for adding arrays 
        {
            Current().Add(new Token(Types.ARRAY, key, null, datas, true));
        }

        public void WriteToFile() //Write current section to file, programmers mut manually change section then write to file
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

           File.WriteAllLines(filename, lines); //after adding all tokens to our list of strings, we write the strings to the file
        }
        


    }
}
