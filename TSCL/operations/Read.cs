// TSCL Read Operation
using static TSCL.utils.Utility;

namespace TSCL.operations
{
    public class Read : IDisposable// read from tscl file
    {
        string filename = string.Empty; //source file
        Dictionary<string, List<Token>> tokens = new Dictionary<string, List<Token>>(); //list of sections: tokens["Section-name"]
        string pos = "";
        HashSet<string> visited;
        int index = 0;

        private List<Token> current()
        {
            return tokens[pos];
        }

        public void Dispose() //Clean up after an instance is finished
        {
            pos = string.Empty;
            tokens.Clear();
            visited.Clear();
            index = 0;
            filename = string.Empty;
        }

        private void advance(string nextsec) //Advance to the next section in the map.
        {
            pos = nextsec;
        }

        public Read(string src)
        {

            if (!File.Exists(src))
            {
                throw new FileNotFoundException($"File: {src} does not exist!");
            }

            filename = src; // pass sourcefile on Initialization
            visited = new HashSet<string>();

            initilaizeRead();
        }

        private void initilaizeRead() // TSCL tokenizer, THIS took me an hour to think of. 
        {
            List<Token> tmp = new List<Token>(); // list of all objects in the section
            string SectionName;
            foreach (string line in File.ReadAllLines(filename)) //read through every line and skip whitespaces
            {

                if (string.IsNullOrWhiteSpace(line)) //skip if line is a empty newline
                {
                    continue;
                }


                char[] seperators = { ' ', ',' };
                string[] words = Tokenize(line,seperators);

                if (words[0][0] == '[' && words[0][words[0].Length - 1] == ']') //Section
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

                }
                else if (words[1].Contains('@')) // Section poiner: points to other sections
                {
                    tmp.Add(new Token(Types.POINTER, words[0], words[1].TrimStart(']')));
                }
                else if (words[1].Contains(',')) //arrays (they are string and string only)
                {
                    string[] subs = words[1].Split(',');

                    tmp.Add(new Token(Types.ARRAY, words[0], string.Empty, subs, true));
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


                }


            }

            current().AddRange(tmp); //final flush before resetting our current position
            pos = string.Empty; //reset position once done

        }

        public Token[] getSectionObjects(string section_name = "") // get all Objects from a section, User can either use the current section or traverse to another.
        {

            if (section_name != "") { // if User wants objects from other sections we advance to that section.
                advance(section_name);
            }

            Token[] toks = current().ToArray(); 

            return toks;
        }

        public void setSection(string Secname) // manual Section setup, to control the flow and starting
        {
            if (!tokens.ContainsKey(Secname))
            {
                throw new IndexOutOfRangeException($"Section Name: {Secname} doesnt exist!");
            }

            pos = Secname;
        }

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

            //  PlaceHolder
            Token[] tmp = current().ToArray();
            pos = oldPos;
            return tmp;
        }

        public object getObjectData(string key, string PointerObjKey = "") // key and Pointer Object key for grabbing objects in another section(Empty by default)
        {
            List<Token> tmp = tokens[pos];
            object data = null;


            foreach (Token tok in tmp)
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

            if (data is Token[] tokarr) // pointer handling for all its objects
            {
                foreach (Token tok in tokarr)
                {
                    if (tok.tokentype == Types.OBJECT && tok.obj.key == PointerObjKey) // if target is obj
                    {
                        return tok.obj.data;
                    }
                    else if (tok.tokentype == Types.ARRAY && tok.arr.key == PointerObjKey) // if its an array
                    {
                        return tok.obj.data;
                    }
                }
            }

            if (data is null)
            {
                throw new Exception($"Object: {key} or {PointerObjKey} is null!");
            }

            if (data is int)
            {
                return (int)data;
            }
            else if (data is bool)
            {
                return (bool)data;
            }

            return data.ToString(); // we return the data in a string by default
        }

        public string[] getArrayData(string key) //array handler
        {
            List<Token> tmp = tokens[pos]; // get our list of objects to find the said array
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



    }
}
