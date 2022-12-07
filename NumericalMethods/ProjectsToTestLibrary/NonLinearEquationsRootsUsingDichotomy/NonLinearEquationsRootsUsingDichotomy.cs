using System;
using System.IO;
using Equations = NumericalMethods.Equations;
using FilesController = NumericalMethods.FilesController;
using FunctionTable = NumericalMethods.FunctionTable;
using GraphicsForm = NumericalMethods.GraphicsForm;

namespace NonLinearEquationsRootsUsingDichotomy
{
    class NonLinearEquationsRootsUsingDichotomy
    {
        static void Main(string[] args)
        {
            //Test1();
            //Test2(error: 1.0E-12);
            Test3(error: 1.0E-12);
        }

        static void Test1()
        {
            using (StreamWriter SW = FilesController.WriteResultToFile("Test1.txt"))
            {
                double root = Equations.Dichotomy(0.3, 0.6, 1.0E-12, CosPiOnX, out int k);
                SW.WriteLine($" x ={root,18:F15}   err = {CosPiOnX(root),7:E1}  K = {k}");
            }
        }

        static double Test2Function(double x)
        {
            return Math.Tanh(x) - 2.0 * Math.Cos(Math.Sqrt(10.0) * x);
        }

        static void Test2(double error)
        {
            const string testName = "Test2";
            FunctionTable table = new FunctionTable(0.0, 15.0, 300, Test2Function, testName);
            using (StreamWriter SW = FilesController.WriteResultToFile(testName + ".txt"))
            {
                table.RootsCorrection(error);
                SW.WriteLine(table.RootsTable("Equation"));
            }
            GraphicsForm.SingleGraphic(table, 300, 500);
        }

        static double Test3Function(double x)
        {
            const double a = 0.09, b = -1.4;
            return Math.Tanh(a * x) + b * Math.Cos(x * Math.Sqrt(Math.Log10(5) + Math.PI * Math.PI));
        }

        static void Test3(double error)
        {
            const string testName = "Test3";
            FunctionTable table = new FunctionTable(-9.3, -6.1, 800, Test3Function, testName);
            using (StreamWriter SW = FilesController.WriteResultToFile(testName + ".txt"))
            {
                table.RootsCorrection(error);
                SW.WriteLine(table.RootsTable("Equation"));
            }
            GraphicsForm.SingleGraphic(table, 300, 500);
        }

        static double CosPiOnX(double x) { return Math.Cos(Math.PI * x); }
    }
}
