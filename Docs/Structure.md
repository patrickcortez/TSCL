# Structure

The Structure of this project is relatively simple. In the root folder theres only 2 folders:

- operations : Read,Modify and Write.
- utils : Utilies of TSCL such as type conversions, String operations,custom Exceptions and etc...
- Core.cs : basically this cs file contains the TSCL token types and the Token definition.

*Read.cs* handles all Deserializing Operations, *Write.cs* handles all Serializing operations,
while *Modify.cs* handles all File Modifications.


## Data Pipeline

**TSCL**'s current data pipeline is relatively simple, the file goes gets scanned line by line 
then each line is evaluated and tokenized, depending on which operation is being used. The current limitation
is that the user has to typecast the objects data, because its in a object return type. 

So my proposed solution is to Create a internal class that handles typecasting so that the data returned to the user
is the type they were expecting.

 Developer: *Tezzz*