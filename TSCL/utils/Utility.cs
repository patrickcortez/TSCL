using System.Linq.Expressions;
using System.Text;
using static TSCL.Initialize;

using isSection = (bool start, bool end);

namespace TSCL.utils
{
    /// <summary>
    /// This class is responsible for all the Tokenizing,String operations across TSCL
    /// </summary>
    internal static class Utility
    {
        /// <summary>
        /// Basic string to int conversion
        /// </summary>
        /// <param name="src">String Source</param>
        /// <returns>It returns an integer</returns>
        /// <exception cref="Exception">Thrown if there are any Letters or Special characters</exception>
        public static int Strint(string src) //string to int converter
        {
            int data = 0;

            foreach(char c in src)
            {
                if (!char.IsDigit(c))
                {
                    throw new Exception($"Invalid Character: {c}");
                }

                data = (data * 10) + ((int)c - '0');
            }

            return data;
        }
        
        /// <summary>
        /// Basic integer boolean operator
        /// </summary>
        /// <param name="data"></param>
        /// <returns>It returns true or false</returns>
        public static bool isInt(string data) // is integer helper
        {
            foreach(char c in data)
            {
                if (!char.IsDigit(c))
                {
                    if(c == '-')
                    {
                        continue;
                    }

                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Basic string to boolean converter
        /// </summary>
        /// <param name="data"></param>
        /// <returns>It returns true or false</returns>
        /// <exception cref="Exception">if the string is not a boolean, its thrown</exception>
        public static bool ConvertBool(string data) // string to boolean converter
        {
            bool res;

            if (bool.TryParse(data,out res))
            {
                return res;
            }

            throw new Exception($"Data is not Boolean: {data}");
        }

        public static string clearSpace(string data) //clear all white spaces manually in a string 
        {
            StringBuilder newString = new StringBuilder(); // our stringbuilder init

            if(data == string.Empty) //safety guard
            {
                throw new ArgumentNullException("String cannot be empty!");
            }

            foreach(char c in data)
            {
                if (char.IsWhiteSpace(c))
                {
                    continue;
                }
                else
                {
                    newString.Append(c);
                }
            }

            return newString.ToString();
        }

        /// <summary>
        /// Manual tokenizer for better control over tokenization
        /// </summary>
        /// <param name="data">string variable you want to tokenize</param>
        /// <param name="seperators"></param>
        /// <returns>It returns a string array</returns>

        // TSCL manual tokenizer
        public static string[] Tokenize(string data,char[] seperators,bool Nospaces = false) // data and seperators parameter, so i can define more than 1 seperators: ',',' ' ' or '.' and etc...
        { 
            StringBuilder tmp = new StringBuilder();
            List<string> arr = new List<string>();
            bool qoutes = false; // Our state determiner for qoutes
            bool hasSeperator = false,hasSpace = false;
            isSection confirm = new isSection(false,false);

            if(data.StartsWith('[') && data.EndsWith(']'))
            {
                confirm = (true, true);
            }

            foreach(char c in data)
            {
                if(c == '"') //if we are in a qoute,we reverse the current value of qoutes then continue to the next
                {
                    qoutes = !qoutes;
                    continue;
                }

                if (qoutes) // Qoute Handling: if its still in qoutes we simply store the current character and continue to the next.
                {
                    tmp.Append(c);
                    continue;
                }

                if (c == '#') // We ignore comments by breaking the loop so the rest of the text will be ignored.
                {
                    break;
                }

                if (char.IsWhiteSpace(c))
                {
                    hasSpace = true;

                    if(Nospaces == true && !qoutes)
                    {
                        continue;
                    }
                }


                if (seperators.Contains(c))
                {
                    hasSeperator = true;
                    if (tmp.Length > 0)
                    {
                        arr.Add(tmp.ToString());
                        tmp.Clear();
                    }
                    continue;
                }
                tmp.Append(c);
            }

            if(tmp.Length > 0)
            {
                arr.Add(tmp.ToString());
            }

            if (!hasSeperator && (!confirm.start && !confirm.end)) //warn user about line instead of stopping execution
            {
                Console.WriteLine("Invalid lines: ",Console.Error);
                foreach(int line in markedLines)
                {
                    Console.Write(line + ",", Console.Error);
                }
            }

            if(confirm == (true, true) && hasSeperator)
            {
                throw new InvalidSectionNameException(tmp.ToString());
            }

            return arr.ToArray();

        }

        /// <summary>
        /// Basic prefix trimmer
        /// </summary>
        /// <param name="data"></param>
        /// <param name="prefix"></param>
        /// <returns>It returns a string without the prefix</returns>

        public static string TrimPrefix(string data,char prefix) //Trim by Prefix
        {
            StringBuilder nStr = new StringBuilder();

            foreach(char c in data)
            {
                if(c == prefix)
                {
                    continue;
                }

                nStr.Append(c);
            }

            return nStr.ToString();

        }

        internal static Exception Warn(string msg)
        {
            throw new Exception(msg);
        }
    }
}
