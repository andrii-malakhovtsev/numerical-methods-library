using System.IO;
using NumericalMethods;
using PrintType = NumericalMethods.PrintType;
using FilesController = NumericalMethods.FilesController;

namespace GaussMethodTest
{
    class GaussMethodTest
    {
        static void Main()
        {
            using (StreamWriter writer = FilesController.WriteResultToFile("GaussMethodResult.txt"))
            {
                const string testTableFile = "TestTable.txt";
                FilesController.ReadMatrixWithVectorFromFile(testTableFile, out Matrix A, out Vector b, out int n);
                writer.Write(TextFormater.GetMatrixAndVectorTextFormat(A, b, 
                    form: true, forwardFormatIndent: 2, formatIndent: 1, "Matrix Ab"));

                Vector X = LinearEquationsSystems.GaussMethod(A, b);

                writer.Write("\r\n  Solving Linear Equations Systems with Gauss Method: ");
                writer.Write(X.GetTextFormat(PrintType.Vertical,
                    form: true, forwardFormatIndent: 3, formatIndent: 12, "Vector X"));

                writer.WriteLine($"\r\n Determinant|A| = {A.Determinant,12:F2}");

                double error = LinearEquationsSystems.AccuracyError(A, X, b);
                writer.WriteLine($"\r\n error = {error,10:E1}");
            }
        }
    }
}
