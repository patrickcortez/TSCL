using static TSCL.utils.Utility;
using ArrObj = (string key, string[] data);
using Obj = (string key, object data); // our object data type
/*
 * Tez's Simple Configuration Language
 * - is a simple config language that is flat
 * in nature. That way its easy to serialize and deserialize data.
 */

namespace TSCL; // list of dictionary of lists (List<Dictionary<string,List<Obj>>> tokens) <- this acts as a whole ass map of the entire file!

/*
 * TODO's:
 *  - Write Modify class to handle modification of TSCL files
 *  - Write a better string tokenizer than string splitting
 *  - Handle qoutes and dont disrupt user data.
 */

public enum Types // our types of tokens: Sections, Objects, array(list of objects) and pointer (an object which points to a section aka a map)
{
    SECTION,
    OBJECT,
    ARRAY,
    POINTER
}

public struct Token //token struct for tokenization
{
   public Types tokentype; // to determine our token type
   public Obj obj; //object handling
    public ArrObj arr; //array of objects

#nullable enable

    public Token(Types ttype, string key = "", object? data = null, string[]? datas = null, bool isArr = false)
    {
        tokentype = ttype;
        if (isArr)
        {
            arr = new ArrObj(key,datas);
        }
        else
        {
            obj = new Obj(key, data);
        }
                
    }
}

public class Read : IDisposable// read from tscl file
{
    string filename = string.Empty; //source file
    Dictionary<string,List<Token>> tokens = new Dictionary<string,List<Token>>(); //list of sections: tokens["Section-name"]
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
        foreach(string line in File.ReadAllLines(filename)) //read through every line and skip whitespaces
        {

            if (string.IsNullOrWhiteSpace(line)) //skip if line is a empty newline
            {
                continue;
            }

           string nline = clearSpace(line); //clear any space from line before continuing

            string[] words = nline.Split("=");

            if (words[0][0] == '[' && words[0][words[0].Length - 1] == ']') //Section
            {
                if(tmp.Count > 0)
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

                tmp.Add(new Token(Types.ARRAY, words[0], string.Empty, subs,true));
            }
            else if (!words[1].Contains(',')) // non array object
            {
                bool res;
                if (isInt(words[1])) //integer
                {
                    tmp.Add(new Token(Types.OBJECT, words[0], Strint(words[1])));
                }else if (bool.TryParse(words[1],out res)) //boolean 
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

    public object getObjectData(string key,string PointerObjKey = "") // key and Pointer Object key for grabbing objects in another section(Empty by default)
    {
        List<Token> tmp = tokens[pos];
        object data = null;


        foreach(Token tok in tmp)
        {
            if(tok.tokentype == Types.OBJECT && tok.obj.key == key)
            {
                data = tok.obj.data;
                break;
            }else if(tok.tokentype == Types.POINTER && tok.obj.key == key)
            {
                string target = ((string)tok.obj.data).TrimStart('@');
                data = handlePointer(target);
                 
            }
        }

        if(data is Token[] tokarr) // pointer handling for all its objects
        {
            foreach(Token tok in tokarr)
            {
                if(tok.tokentype == Types.OBJECT && tok.obj.key == PointerObjKey) // if target is obj
                {
                    return tok.obj.data;
                }else if(tok.tokentype == Types.ARRAY && tok.arr.key == PointerObjKey) // if its an array
                {
                    return tok.obj.data;
                }
            }
        }

        if(data is null)
        {
            throw new Exception($"Object: {key} or {PointerObjKey} is null!");
        }
        
        if(data is int)
        {
            return (int)data;
        }else if(data is bool)
        {
            return (bool)data;
        }

        return data.ToString(); // we return the data in a string by default
    }

    public string[] getArrayData(string key) //array handler
    {
        List<Token> tmp = tokens[pos]; // get our list of objects to find the said array
        string[]? datas = null;

        foreach(Token tok in tmp) // iterate through every token, to find the said array:
        {
            if(tok.tokentype == Types.ARRAY && tok.arr.key == key)
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

public class Write // write to tscle file
{
    string filename = string.Empty; //source file
    Dictionary<string, List<Token>> TSCL;

    string pos = string.Empty;

    public Write(string src)
    {
        if (!File.Exists(src))
        {
            throw new FileNotFoundException($"TSCL File: {src} does not exist!");
        }else if(Path.GetExtension(src) != ".tscl")
        {
            throw new Exception($"File: {Path.GetFileName(src)} is not a TSCL File!");
        }

        filename = src; // pass sourcefile on Initialization
        TSCL = new Dictionary<string, List<Token>>();
    }

    public void AddSection(string name) // Adding sections to TSCL Map
    {
        if(name == string.Empty) // if sec name is empty
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

    public void AddObject(string key,object data,Types tokentype) //adding objects
    {
        if(tokentype == Types.OBJECT)
        {
            Current().Add(new Token(tokentype, key, data));
        }else if(tokentype == Types.POINTER)
        {
            Current().Add(new Token(tokentype, key, data));
        }
    }

    public void AddArray(string key, string[] datas) //for adding arrays 
    {
        Current().Add(new Token(Types.ARRAY, key, null, datas,true));
    }

    public void WriteToFile() //Write current section to file, programmers mut manually change section then write to file
    {
        List<string> lines = new List<string>();

        foreach(KeyValuePair<string,List<Token>> section in TSCL) //our line writer which grabs the current keyvalue in our dict, then iterates through all our tokens
        {
            lines.Add($"[{section.Key}]");
            foreach(Token tok in section.Value)
            {
                if(tok.tokentype == Types.OBJECT) //write object to file
                {
                    lines.Add($"{tok.obj.key}={tok.obj.data}"); 
                }else if(tok.tokentype == Types.ARRAY) //write array
                {
                    lines.Add($"{tok.arr.key}={string.Join(',', tok.arr.data)}");
                }else if(tok.tokentype == Types.POINTER) //write Pointer
                {
                    lines.Add($"{tok.obj.key}=@{tok.obj.data}");
                }
            }

            lines.Add(Environment.NewLine); //newline after every section for proper formatting
        }

        File.WriteAllLines(filename,lines); //after adding all tokens to our list of strings, we write the strings to the file
    }

    
}