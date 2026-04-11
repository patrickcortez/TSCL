// Main TSCL Definitions: Token and Types
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

    public Token(Types ttype, string key = "", object? data = null, string[]? datas = null, bool isArr = false) // token definition Token(type,obj-key,value(if object),values(ifarr),true/fase)
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


