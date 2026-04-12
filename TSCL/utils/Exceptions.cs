namespace TSCL.utils;

internal class InvalidSectionNameException : Exception
{
    public InvalidSectionNameException(string secname) : base($"Invalid section name: {secname}") { }

    public  InvalidSectionNameException() : base("Invalid section name!") { }
    
}