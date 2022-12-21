using System.IO;
using DataTable = NumericalMethods.DataTable;
using FilesController = NumericalMethods.FilesController;
using GraphicsForm = NumericalMethods.GraphicsForm;

namespace DataTableTest
{
    class DataTableTest
    {
        static void Main()
        {
            using (StreamWriter writer = FilesController.WriteResultToFile("DataTableResult.txt"))
            {
                DataTable table = new DataTable("TestTable.txt", "Data Table Graphic");
                string txt = $"\r\n\r\n\r\n Read lines: {table.Length,0}";
                txt += table.ToPrint("   Processing file *.txt");
                writer.WriteLine(txt);
                GraphicsForm.SingleGraphic(table, 300, 500);
                table.ToTxtFile("DataTableAdditionalResult.txt", " New results form");
            }
        }
    }
}
