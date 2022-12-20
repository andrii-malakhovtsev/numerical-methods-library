using System.IO;
using NumericalMethods;
using PrintType = NumericalMethods.PrintType;
using FilesController = NumericalMethods.FilesController;

namespace SeidelMethodTest
{
    class SeidelMethodTest
    {
        static void Main(string[] args)
        {
            using (StreamWriter writer = FilesController.WriteResultToFile("SeidelMethodResult.txt"))
            {
                const string matrixAndVectorFileName = "MatrixAndVector.txt";
                writer.WriteLine($"\r\n {matrixAndVectorFileName}");

                FilesController.ReadMatrixWithVectorFromFile(matrixAndVectorFileName, out Matrix A, 
                    out Vector b, out int n);
                writer.Write(FilesController.GetMatrixAndVectorTextFormat(A, b, true, 3, 3, "Matrix Ab"));

                Vector X = LinearEquationsSystems.SeidelMethod(A, b, 1.0E-8, out int K);

                writer.Write("\r\n  Solving Linear Equations Systems with Seidel Method: ");
                writer.Write(X.GetTextFormat(PrintType.Horizontal, true, 3, 7, "Vector X"));

                double error = LinearEquationsSystems.AccuracyError(A, X, b);
                writer.WriteLine($"\r\n Error = {error,10:E1} Iterations = {K}");
            }
        }
    }
}
