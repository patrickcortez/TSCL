# TSCL

**TSCL** or *Tezz's Simple Configuration Language* is a flat Configuration langauge that is
mainly used for configurations and other similar ventures. **TSCL** doesn't have braces and nested braces which
could look confusing and messy in the long run. 

Instead **TSCL** has *Pointers* that points to a *Section*. In **TSCL**, this simple langauge contains only 4 parts:

- Sections
- Objects
- Arrays
- Pointers


## Sections

**Sections** are basically namespaces that you can group your *objects*,*arrays* and *pointers*.

``` TSCL

[Section-Name]
ObjectName="Value"
Object2= 21
Array="Dave","John","Micheal"
Next=@NextSection

[NextSection]
isActive=false
Members=30

```

## Objects

You can add as many **objects** as you want in any section, **TSCL** has 3 *object* types:

- String
- Integer
- Boolean

## Arrays

**TSCL** *arrays* are strictly strings, so it can be parsed serialized and deserialized easily.
You can declare an array by simply Adding commas after you declared your array name:

```	TSCL

[Section]
Array="One","Two","Three"

```

## Pointers

Instead of nesting with messy curly braces or retyping a section inside a section, You can just use pointers instead.
*Pointers* are basically objects that point to a *Section*.

``` TSCL

[Section1]
Point=@Section2


[Section2]
isActive=true

```

Note: Your Pointer value has to always have a '@' symbol at the very start, other wise its just a regular object.
And also your *Pointer* has to have the same name as your target *Section*


---

## Usage

To use **TSCL** just add the namespace on top of your file and call *Write* to serialize and *Read* to deserialize.
You can also use the *Token* struct if you want to parse an entire sections objects and use em on your program/app.

``` C#

using TSCL; // Just Include this namespace to use: Read,Write and Modify

namespace Test{

	Public Class Test{
		string[] arr;
	
		public Test(string filename){

			Initializer.setFile("FilePath.tscl") //make sure you set your file before using: Read, Write and Modify

			Token[] tokens; 
			//Deserializing
			Read read = new Read()// Initialize read
				
			read.setSection("Section1"); //We then set which section to start with

			object data = read.getObjectData("Object_Name"); //after setting we can grab our datas value with the getObjectData()

			arr = read.getArrayData("Array_Name"); //Or if its an array we can go ahead and use the getArrayData(), since it returns a string Array.

			tokens = read.getSectionObjects("NextSection"); // We can also grab objects of Sections and manually use them.
			// in the parameters you can put a section name to advance to that section or use the current section by
			// leaving the parameter blank.
			
			

			initiateFile();
	
		}

		public void initiateFile(){ // Serializing
		
			using(Write write = new Write()// Initialize Write

			write.AddSection("SectionName"); // This is how we add our section

			write.SetSection("SectionName"); //After adding our section, we need to set to use it, otherwise our current section is null

			write.AddObject("key","value",Types.OBJECT) // this is how add our objects, arrays and pointers to our section, object value can be
					//int, bool or string. But array values are only string.

			write.WriteToFile(); //After we are done, we call the writetofile method, to confirm it.
			
		
		
		}

		public void FileModification() // File Modification
		{
			Modify mod = new Modify() //always make sure the file exists
			
			mod.SetSection("Section1"); // Always set your target section before modifying
				
			mod.ModifyObject("Object_Name","NewValue"); // Modify
				
			
		
		}
	
	}


}


```

---

## License

This Project is under the GNU General Public License. See the License File for more details.