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
 *  - Improve Tokenizer with StreamReader.
 *  - Improve Error Handling
 *  - Refine Pointers.
 */


/// <summary>
///  Specifies the token type defitions for our operations
/// </summary>
public enum Types // our types of tokens: Sections, Objects, array(list of objects) and pointer (an object which points to a section aka a map)
{
    SECTION,
    OBJECT,
    ARRAY,
    POINTER
}


/// <summary>
/// Specificies the token struct for tokenization
/// </summary>
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

/// <summary>
/// Set FilePath for all 3 classes to use: Read,Write and Modify
/// </summary>
public static class Initialize //Sets the file for all 3 classes, instead of having to declare the file you want to use
{
    public static string FileName { get; private set; } // public get but cant be written outside of this class
    internal static int lineN0 { get; private set; } = 1; // we always start at line 1;

    internal static List<int> markedLines { get; private set; } = new List<int>();

    public static void setFile(string src) //sets the path for the 3 classes to use.
    {

        if (!File.Exists(src)) //guard clause if the file does not exist
        {
            throw new FileNotFoundException($"File: {src} does not exist!");
        }

        if(Path.GetExtension(src) != ".tscl") //it exists but its not a tscl file.
        {
            throw new Exception($"File: {src} is not a .tscl file!");
        }

        FileName = src;
    }

    //For Debugging

    internal static void advanceline()
    {
        lineN0++;
    }

    internal static int getLine()
    {
        return lineN0;
    }

    internal static void resetLine()
    {
        lineN0 = 1;
    }

    internal static void markLine(int line)
    {
        markedLines.Add(line);
    }

    internal static void ClearLine()
    {
        markedLines.Clear();
    }
}


