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
/// State Manager of TSCL, sets file for the 3 classes to use if Universal is true which in default it is.
/// Note: File is automatically added to the Filenames list
/// </summary>
public static class Initialize //Sets the file for all 3 classes, instead of having to declare the file you want to use
{
    /// <summary>
    /// File TSCL will perform operations on, (Read only once outside of Initializer)
    /// </summary>
    public static string FileName { get; private set; } // public get but cant be written outside of this class

    public static List<string> FileNames { get; private set; } = new List<string>(); // File names incase the user wants to use more than one files

    internal static int lineN0 { get; private set; } = 1; // we always start at line 1;

    internal static List<int> markedLines { get; private set; } = new List<int>(); // list of files a user may use.

    /// <summary>
    /// Indicator if TSCL will use its unified Filename by
    /// setting it in SetFile or instance based. 
    /// </summary>
    public static bool isUniversal { internal get; set; } = false; //if the user wants to pass the file Universally or per instance
    public static bool isVerbose { internal get; set; } = false;


    /// <summary>
    /// Specifies the file you want TSCL to use.
    /// </summary>
    /// <param name="src"> TSCL source file you want to use</param>
    /// <exception cref="FileNotFoundException"></exception>
    /// <exception cref="Exception"></exception>
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

        AddFile(src);
        FileName = src;
    }

    /// <summary>
    /// Adds filepath to the FileNames list
    /// </summary>
    /// <param name="src">file you want to use</param>
    /// <exception cref="FileNotFoundException"></exception>
    /// <exception cref="Exception"></exception>

    ///<summary>
    /// Adds file to the list of tscl files.
    ///</summary>
    public static void AddFile(string src)
    {

        if (!File.Exists(src))
        {
            throw new FileNotFoundException($"File: {src}does not exist!");
        }

        if(Path.GetExtension(src) != ".tscl")
        {
            throw new Exception($"File: {Path.GetFileName(src)} is not a .tscl file!");
        }

        FileNames.Add(src);
    }

    /// <summary>
    /// Sets the current file name to the file you want to use
    /// by changing its index to that file.
    /// </summary>
    /// <param name="index"></param>
    /// <exception cref="Exception"></exception>
    public static void UseFile(int index)
    {

        if(FileNames.Count < 1)
        {
            throw new Exception("File names is empty!");
        }

        FileName = FileNames[index];
    }

    //For Debugging

    internal static void advanceline()
    {
        lineN0++;
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


