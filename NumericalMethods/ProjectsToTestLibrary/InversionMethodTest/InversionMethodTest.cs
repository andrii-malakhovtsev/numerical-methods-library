using System.IO;
using NumericalMethods;
using PrintType = NumericalMethods.PrintType;
using FilesController = NumericalMethods.FilesController;

namespace InversionMethodTest
{
    class InversionMethodTest
    {
        static StreamWriter writer;

        static void Main(string[] args)
        {
            TestInversion();
            TestLinearEquationSystems();
        }

        static void TestInversion()
        {
            using (writer = FilesController.WriteResultToFile("InversionResult.txt"))
            {
                const string file = "TestMatrix.txt";
                writer.WriteLine($"\r\n {file} ");
                Matrix A = new Matrix(file);
                writer.Write(A.GetTextFormat(true, 2, 1, "Matrix A"));

                Matrix V = A.FirstInversion(out double err1);
                writer.WriteLine($"  Deteminant|A| = {A.Determinant,12:F2}");
                writer.Write("\r\n Inversion_1 : " + V.GetTextFormat(true, 2, 7, "Matrix V"));
                writer.WriteLine($"\r\n Error 1 = {err1,10:E1}");

                V = A.SecondInversion(out double err2);
                writer.Write("\r\n Inversion_2 : " + V.GetTextFormat(true, 2, 7, "Matrix V"));
                writer.WriteLine($"\r\n Error 2 = {err2,10:E1}");
            }
        }

        static void TestLinearEquationSystems()
        {
            using (writer = FilesController.WriteResultToFile("TestLinearEquationSystems.txt"))
            {
                const string file = "MatrixAndVector.txt";
                writer.WriteLine($"\r\n {file}");

                FilesController.ReadMatrixWithVectorFromFile(file, out Matrix A, out Vector b, out int n);
                writer.Write(FilesController.GetMatrixAndVectorTextFormat(A, b, true, 2, 3, "Matrix Ab"));

                Matrix V = A.FirstInversion(out double err);
                writer.WriteLine($"  Deteminant|A| = {A.Determinant,12:F2}");
                writer.Write("\r\n Inversion_1 :" + V.GetTextFormat(true, 2, 7, "Matrix V"));
                writer.WriteLine($"   Error 1 = {err,10:E1}");

                Vector X = Vector.Multiply(V, b);

                writer.Write("\r\n Solving Linear Equations Systems with Inversion Method:");
                writer.Write(X.GetTextFormat(PrintType.Vertical, true, 2, 12, "Vector X"));
                double error = LinearEquationsSystems.AccuracyError(A, X, b);
                writer.WriteLine($"\r\n Error_of_SLAE = {error,10:E1}");
            }
        }
    }
}
