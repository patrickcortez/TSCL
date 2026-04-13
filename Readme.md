# TSCL

**TSCL** or *Tezz's Simple Configuration Language* is a flat Configuration langauge that is
mainly used for configurations and other similar ventures. **TSCL** doesn't have braces and nested braces which
could look confusing and messy in the long run. 

Instead **TSCL** has *Pointers* that points to a *Section*. In **TSCL**, this simple langauge contains only 4 parts:

- Sections
- Objects
- Arrays
- Pointers

**TSCL** has 3 major operations you can use:

- Write : Serializing
- Read : Deserializing
- Modify : File Modification


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

You can add as many **objects** as you want in any section, **TSCL** can have any object type
because after parsing its Typecaster will handle the conversion of the objects value.

```TSCL

[Section1]

Obj=true #boolean

Obj2=200 #Integer

obj3="Name" #String

Obj4=20.80 #float

# And so forth....


```

## Arrays

**TSCL** *arrays* can be any data type now. declaring arrays is simple. You just have to put
commas to add a new element to your array:


```	TSCL

[Section]
Array="One","Two","Three"
Array2=1,2,3
Array3=3.2,3.1,3.0

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

## Comments

You can also insert comments in your *tscl* file with the '#' character, because the Parser will ignore any comments and whitespaces. 
So you can put as much whitespace in between your objects.


## Modes

TSCL has two modes you can toggle in the Initializer class. Simply set the static bool "isUniversal" to true or false,
*true* if you want to use TSCL's SetFile in Initializer to pass your file, that way all 3 classes will use Initializers
Filename instead of their own in their constructor. However when *false* the 3 classes wont use the initializers, instead
it will use what you pass on its constructor.

There is also "isVerbose", which basically prints all the errors in your tscl file. by setting it to true, it
prints the errors on your console, but if its false it does not.

Note: Both isVerbose and isUniversal is **false** by default.


---

## Data Pipeline

**TSCL**'s data pipeline is relatively simple. Depending on your config whether *isUniversal* is set to true or false.
**TSCL**'s 3 main operations will first scan and tokenize your file/s upon initialization of their classes.

After you have initialized the operations. you can then begin deserializing, serializing or modifying your
tscl file.

File/s->Tokenizer(Scans and Tokenizes)->Parser->TypeCaster

---

## Usage

To use **TSCL** just add the namespace on top of your file and call *Write* to serialize and *Read* to deserialize.
You can also use the *Token* struct if you want to parse an entire sections objects and use em on your program/app.

``` C#

using TSCL; // Just Include this namespace to use: Read,Write and Modify

namespace Test{

	public class Test{
		string[] arr;
	
		public Test(string filename){

			Initialize.setFile("FilePath.tscl") //make sure you set your file before using: Read, Write and Modify

			Token[] tokens; 
			//Deserializing
			Read read = new Read()// Initialize read
				
			read.setSection("Section1"); //We then set which section to start with

			int data = read.GetValue<int>("Object_Name"); //after setting we can grab our datas value with the GetValue().

			arr = read.GetArrayValue<string>("Array_Name"); //Or if its an array we can go ahead and use the GetArrayValue().

			tokens = read.getSectionObjects("NextSection"); // We can also grab objects of Sections and manually use them.
			// in the parameters you can put a section name to advance to that section or use the current section by
			// leaving the parameter blank.
			
			

			initiateFile();
	
		}

		public void initiateFile(){ // Serializing
		
			using(Write write = new Write()) // Initialize Write

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

## Installation

To install **TSCL** you can simple download this on [Nuget](https://www.nuget.org/packages/TSCL.Parser/1.3.3) or
in Visual Studio, just go to **Manage NuGet Packages** and search up **TSCL**.


---

## Logo

![Official](https://i.imgur.com/zmjeqsE.png)

---

## License

This Project is under the GNU General Public License. See the License File for more details.