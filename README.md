# Numerical analysis library for C# projects
Project repository also contains folder *"ProjectsToTestLibrary"* where the </br>
projects to test the library are located showing how it works.

## Installation
1. Go to <a href="https://github.com/malandrii/numerical-methods-library/releases">Releases</a>, choose the latest release and click the "numerical-methods.dll" or
just click <a href="https://github.com/malandrii/numerical-methods-library/releases/download/v1.0/numerical-methods.dll">here</a>.
2. In Visual Studio right click the "References" of your project, then "Add Reference..." and then "Browse..." button in the right bottom corner of the opened window and choose the downloaded "numerical-methods.dll" (uses .txt files to save and get the information) </br>
> References (right click) => Add Reference... => Browse... => Choose "numerical-methods.dll" you have downloaded
3. Write next line in your C# project:
```c#
// C#
using NumericalMethods;
```

## Features

Library has methods that help solving:
* Linear Equation Systems
  * Cramers rule
  * Gauss Method
  * Jordan Gauss Method
  * Inversion Method
  * Seidel Method
  * Simple Iteration Method
* Data tables
* LU-decomposition
* Matrix functions and operations
* Non-linear equations roots (using dichotomy)
* Numeric Serius Sum
* Tangent

## Example of usage

For example let's test "*Data table*" with the project to test a library located below:
> NumericalMethods\ProjectsToTestLibrary\DataTableTest\DataTableTest.cs
```c#
using System.IO;
using DataTable = NumericalMethods.DataTable;
using FilesController = NumericalMethods.FilesController;
using GraphicsForm = NumericalMethods.GraphicsForm;

namespace DataTableTest
{
    class DataTableTest
    {
        static void Main(string[] args)
        {
            using (StreamWriter writer = FilesController.WriteResultToFile("DataTableResult.txt"))
            {
                DataTable table = new DataTable("TestTable.txt", "Data Table Graphic");
                string txt = $"\r\n\r\n\r\n Read lines: {table.Length,0}";
                txt += table.ToPrint("   Processing file *.txt");
                writer.WriteLine(txt);
                GraphicsForm.SingleGraphic(table, widthPointsCount: 300, heightPointsCount: 500);
                table.ToTxtFile("DataTableAdditionalResult.txt", " New results form");
            }
        }
    }
}
```
[See README_TestTable.txt](https://raw.githubusercontent.com/malandrii/numerical-methods-library/refs/heads/master/README_TestTable.txt)
## Running the app </br>
And we get files *DataTableResult.txt* and *DataTableAdditionalResult.txt* in the same folder as the .exe to run the app is located in the project</br>
> DataTableResult.txt
```
DataTableResult.txt

 Read lines: 200   Processing file *.txt

 Table Data Table Graphic in file TestTable.txt:
 
  x = [   4.300000000000   :   8.600000000000  ]   x_Reg =   4.300000000000

    Min (    4.710552763819,     0.467901053830 )
    Max (    7.908542713568,     6.751486519887 )    f_Reg =   6.283585466057

 Zeros Table of Data Table Graphic is empty!

```
> DataTableAdditionalResult.txt (being the same as it should)
```
New results form

 Function table Data Table Graphic : 
 
  x = [   4.300000000000   :   8.600000000000  ]   x_Reg =   4.300000000000

    Min (    4.710552763819,     0.467901053830 )
    Max (    7.908542713568,     6.751486519887 )    f_Reg =   6.283585466057

 Zeros Table of Data Table Graphic is empty!
 
```

and the graphic picture of the txt-file data will apear in the opened window

![graphic](https://user-images.githubusercontent.com/111363234/206312290-afcc5019-f3b3-46b4-8e56-44b1aa2f2910.png)
