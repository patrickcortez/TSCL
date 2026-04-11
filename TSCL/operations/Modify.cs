// TSCL Modify Operation
namespace TSCL.operations
{
    public class Modify
    {
        string fname;
        List<string> lines;

        public Modify(string filename)
        {
            if (!File.Exists(filename))
            {
                throw new FileNotFoundException($"File: {filename} not found!");
            }

            fname = filename;
        }
    }
}
