namespace TSCL.utils;

public class InvalidSectionNameException : Exception
{
    
    public InvalidSectionNameException(string secname) : base($"Section Name: {secname} is")  
    {
        
    }
    
}