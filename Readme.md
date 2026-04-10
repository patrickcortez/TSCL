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

## License

This Project is under the GNU General Public License. See the License File for more details.