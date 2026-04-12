namespace TSCL.utils
{
    internal class Typecaster
    {
        public static T CastObject<T>(object data) // typecast handler, so user can automatically get the value of object
        {
            if(data == null) // if data is null then we throw an error
            {
                throw new NullReferenceException("data cannot be empty!");
            }

            try
            {
                return (T)Convert.ChangeType(data, typeof(T)); // type cast to what the user wants
            }
            catch (InvalidCastException)
            {
                throw new InvalidCastException($"Cannot cast {data} to {typeof(T).Name}"); // if we cant we throw an error..
            }
        }
    }
}
