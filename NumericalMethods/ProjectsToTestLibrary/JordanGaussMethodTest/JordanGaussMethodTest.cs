using System.IO;
using NumericalMethods;
using PrintType = NumericalMethods.PrintType;
using FilesController = NumericalMethods.FilesController;

namespace JordanGaussMethodTest
{
    class JordanGaussMethodTest
    {
        static void Main(string[] args)
        {
            TestJordanGaussMethod();
        }

        static void TestJordanGaussMethod()
        {
            using (StreamWriter writer = FilesController.WriteResultToFile("JordanGaussMethodResult.txt"))
            {
                string file = "MatrixAndVector.txt";
                FilesController.ReadMatrixWithVectorFromFile(file, out Matrix A, out Vector b, out int n);
                writer.Write(FilesController.GetMatrixAndVectorTextFormat(A, b, true, 2, 1, "Matrix Ab"));

                Vector X = LinearEquationsSystems.GaussJordanElimination(A, b);

                writer.Write("\r\n Solving Linear Equations Systems with Jordan Gauss Method:");
                writer.Write(X.GetTextFormat(PrintType.Vertical, true, 3, 12, "Vector X"));

                writer.WriteLine($"\r\n Deteminant|A| = {A.Determinant,12:F2}");

                double error = LinearEquationsSystems.AccuracyError(A, X, b);
                writer.WriteLine($"\r\n error = {error,10:E1}");
            }
        }
    }
}
