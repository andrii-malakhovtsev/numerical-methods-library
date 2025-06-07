# Numerical Analysis Library for C# Projects

Library for numerical analysis, designed to help solve a variety of mathematical and computational problems. Additionally, the repository includes a folder named *ProjectsToTestLibrary*, which contains example projects demonstrating how to use the library.

## Installation

1. Download the latest release from the [Releases](https://github.com/andrii-malakhovtsev/numerical-methods-library/releases) page. Click on `numerical-methods.dll`, or download it directly [here](https://github.com/andrii-malakhovtsev/numerical-methods-library/releases/download/v1.0/numerical-methods.dll).

2. In Visual Studio, right-click on the **References** section of your project, select **Add Reference...**, and click the **Browse...** button in the lower-right corner of the window. Choose the downloaded `numerical-methods.dll` file.
   - **Path:** References (right-click) ➔ Add Reference... ➔ Browse... ➔ Choose `numerical-methods.dll`.
   
3. Include the following line in your C# project:
   ```csharp
   using NumericalMethods;

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

For example let's test "`Data table`" with the project to test a library located below:
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
Running this example generates files `DataTableResult.txt` and `DataTableAdditionalResult.txt`, in the same folder as the `.exe` file. </br>
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

Running the app will also open a window displaying the graphic representation picture of the `.txt`-file data.

![graphic](https://user-images.githubusercontent.com/111363234/206312290-afcc5019-f3b3-46b4-8e56-44b1aa2f2910.png)
