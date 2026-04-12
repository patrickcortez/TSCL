namespace TSCL.utils;

internal class InvalidSectionNameException : Exception // If User ever has an invalid Section Name: [Sec=Name] or [Sec,Name]
{
    
    public InvalidSectionNameException(string secname) : base($"Section Name: {secname} is")  
    {
        
    }
    
}