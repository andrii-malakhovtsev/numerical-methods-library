using System.IO;
using FilesController = NumericalMethods.FilesController;
using Matrix = NumericalMethods.Matrix;

namespace MatrixFunctionsTest
{
    class MatrixFunctionsTest
    {
        static StreamWriter writer;

        static void Main(string[] args)
        {
            MinorTest(n: 4, ii: 2, jj: 2); 
            DeterminantTest();
            CofactorTest("TestMatrixB.txt", I: 2, J: 3);
        }

        static void MinorTest(int n, int ii, int jj)
        {
            using (writer = FilesController.WriteResultToFile("MinorResults.txt"))
            {
                Matrix T = new Matrix(n, NumericalMethods.MatrixType.Test);
                writer.Write(T.GetTextFormat(true, 2, 0, " Test Array  T:"));

                for (int j = 1; j <= n; j++)
                    writer.Write(T.Minor(ii, j).GetTextFormat(true, 2, 0, $" Minor to T[{ii},{j}]"));
                for (int i = 1; i <= n; i++)
                    writer.Write(T.Minor(i, jj).GetTextFormat(true, 2, 0, $" Minor to T[{i},{jj}]"));
            }
        }

        static void DeterminantTest()
        {
            using (writer = FilesController.WriteResultToFile("DeterminantResults.txt"))
            {
                Matrix A = new Matrix("TestMatrixA.txt");
                writer.Write(A.GetTextFormat(true, 2, 1, " Matrix A:"));
                writer.WriteLine($"\r\n  Determinant(A) = {A.Determinant}");

                writer.WriteLine($"   Determinant_J = {Matrix.HeightDeterminant(A)}");
                writer.WriteLine($"   Determinant_I = {Matrix.WidthDeterminant(A)}");

                A.LowerUpperDecomposition(out Matrix L, out Matrix U);
                writer.WriteLine($" Determinant(LU) = {A.Determinant}");
            }
        }

        static void CofactorTest(string file, int I, int J)
        {
            using (writer = FilesController.WriteResultToFile("CofactorResults.txt"))
            {
                Matrix B = new Matrix(file);
                writer.WriteLine($" Determinant(B) = {B.Determinant}\r\n");

                for (int j = 1; j <= B.Size; j++)
                    writer.WriteLine($"  Cofactor[{I},{J}] = {B.Cofactor(I, j),9}");

                double determinantB = Matrix.GetDeterminant(B, I, true);
                writer.WriteLine($"\r\n Determinant(B, {I}, true) = {determinantB}\r\n");

                for (int i = 1; i <= B.Size; i++)
                    writer.WriteLine($"  Cofactor[{i},{J}] = {B.Cofactor(i, J),9}");

                determinantB = Matrix.GetDeterminant(B, J, false);
                writer.WriteLine($"\r\n Determinant(B, {J}, false)  = {determinantB}\r\n");
            }
        }
    }
}
