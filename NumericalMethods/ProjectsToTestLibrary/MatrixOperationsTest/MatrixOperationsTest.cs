using System.IO;
using FilesController = NumericalMethods.FilesController;
using Matrix = NumericalMethods.Matrix;
using MatrixType = NumericalMethods.MatrixType;

namespace MatrixOperationsTest
{
    class MatrixOperationsTest
    {
        static StreamWriter writer;

        static void Main()
        {
            MatrixGenerationTest();
            MatrixRotationTest();
            SummAndSubtractTest(); 
            MultiplyTest(); 
        }

        static void MultiplyTest()
        {
            using (writer = FilesController.WriteResultToFile("MultiplyResult.txt"))
            {
                Matrix T = new Matrix(4, MatrixType.Test);
                Matrix E = new Matrix(4, MatrixType.Identity); E = E.Rotation(true);
                Matrix A = T * E;
                writer.WriteLine(A.GetTextFormat(true, 3, 0, "Test of Multiply: T * E = A \r\n"));
                A *= -2.0;
                writer.WriteLine(A.GetTextFormat(true, 3, 0, "Test of Multiply: -2 * A => A \r\n"));
            }
        }

        static void SummAndSubtractTest()
        {
            using (writer = FilesController.WriteResultToFile("SummAndSubtractResult.txt"))
            {
                Matrix E = new Matrix(5, MatrixType.Identity); Matrix U = E.Rotation(true);
                Matrix A = E + U;
                writer.WriteLine(A.GetTextFormat(true, 3, 0, "Test of: E + U = A \r\n"));
                A = E - U;
                writer.WriteLine(A.GetTextFormat(true, 3, 0, "Test of: E - U = A \r\n"));
            }
        }

        static void MatrixRotationTest()
        {
            using (writer = FilesController.WriteResultToFile("MatrixRotationResult.txt"))
            {
                Matrix U = new Matrix(5, MatrixType.Upper);
                writer.WriteLine(U.GetTextFormat(true, 2, 0, "Test of Generation Matrix U \r\n"));

                Matrix A = U.Rotation(true);
                writer.WriteLine(A.GetTextFormat(true, 2, 0, "Test of true-Rotation Matrix U. Result = A \r\n"));

                U = U.Rotation(false);
                writer.WriteLine(U.GetTextFormat(true, 2, 0, "Test of false-Rotation Matrix U. Result = U \r\n"));
            }
        }

        static void MatrixGenerationTest()
        {
            using (writer = FilesController.WriteResultToFile("MatrixGenerationResult.txt"))
            {
                Matrix E = new Matrix(4, MatrixType.Identity);
                writer.WriteLine(E.GetTextFormat(true, 2, 1, "Gen Identity Matrix. Result = E \r\n"));

                Matrix T = new Matrix(4, MatrixType.Test);
                writer.WriteLine(T.GetTextFormat(true, 2, 0, "Gen Test Matrix. Result = T \r\n"));

                Matrix A = T.Copy();
                writer.WriteLine(A.GetTextFormat(true, 2, 0, "Gen Matrix T. Result = A \r\n"));

                Matrix AT = A.Transpose();
                writer.WriteLine(AT.GetTextFormat(true, 2, 0, "Transpose Matrix A. Result = AT \r\n"));

                T = T.Transpose();
                writer.WriteLine(T.GetTextFormat(true, 2, 0, "Transpose Matrix T. Result = T \r\n"));
            }
        }
    }
}
