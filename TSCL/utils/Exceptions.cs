namespace TSCL.utils;

internal class InvalidSectionNameException : Exception
{
    public InvalidSectionNameException(string secname) : base($"Invalid section name: {secname}") {
    }

    public  InvalidSectionNameException() : base("Invalid section name!") 
    {
    }
    
}

internal class EmptyArraySizeException : Exception
{
    public  EmptyArraySizeException(string msg) : base(msg)
    {
    }

    public EmptyArraySizeException() : base(" Array Size cannot be empty!")
    {
    }
}