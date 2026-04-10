using System;
using System.Collections.Generic;
using System.Text;

namespace TSCL.utils
{
    internal static class Utility
    {
        public static int Strint(string src)
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
        
        public static bool isInt(string data)
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

        public static bool ConvertBool(string data)
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
    }
}
