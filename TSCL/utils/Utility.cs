using System;
using System.Collections.Generic;
using System.Text;

namespace TSCL.utils
{
    internal static class Utility
    {
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
        
        public static bool isInt(string data) // is integer helper
        {
            foreach(char c in data)
            {
                if (!char.IsDigit(c))
                {
                    return false;
                }
            }

            return true;
        }

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

        // TSCL manual tokenizer
        public static string[] Tokenize(string data,char[] seperators) // data and seperators parameter, so i can define more than 1 seperators: ',',' ' ' or '.' and etc...
        { 
            string tmp = string.Empty;
            List<string> arr = new List<string>();
            bool qoutes = false; // Our state determiner for qoutes

            foreach(char c in data)
            {
                if(c == '"') //if we are in a qoute,we reverse the current value of qoutes then continue to the next
                {
                    qoutes = !qoutes;
                    continue;
                }

                if (qoutes) // Qoute Handling: if its still in qoutes we simply store the current character and continue to the next.
                {
                    tmp += c;
                    continue;
                }
                else if(!qoutes)
                {
                    arr.Add(tmp);
                    tmp = string.Empty;
                }



                if (seperators.Contains(c) && tmp != string.Empty)
                {
                    arr.Add(tmp);
                    tmp = string.Empty;
                    continue;
                }
                else
                {
                    continue;
                }

                if(c == '#') // We ignore comments by breaking the loop so the rest of the text will be ignored.
                {
                    break;
                }

                tmp += c;
            }

            return arr.ToArray();

        }
    }
}
