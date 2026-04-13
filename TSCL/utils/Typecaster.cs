// TSCL - Tezz's Simple Configuration Language
// Copyright (C) 2026 Patrick Cortez
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

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
