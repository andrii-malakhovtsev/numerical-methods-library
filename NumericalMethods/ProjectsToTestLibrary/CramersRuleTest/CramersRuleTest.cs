using System.IO;
using NumericalMethods;
using PrintType = NumericalMethods.PrintType;
using FilesController = NumericalMethods.FilesController;

namespace CramersRuleTest
{
    class CramersRuleTest
    {
        static void Main(string[] args)
        {
            using (StreamWriter SW = FilesController.WriteResultToFile("CramersRuleResult.txt"))
            {
                const string testTableFile = "TestTable.txt";
                SW.WriteLine($"\r\n {testTableFile}");

                FilesController.ReadMatrixWithVectorFromFile(testTableFile, out Matrix A, out Vector b, out int n);
                SW.Write(FilesController.GetMatrixAndVectorTextFormat(A, b, true, 2, 1, "Matrix Ab"));

                Vector X = LinearEquationsSystems.CramerRule(A, b, out Vector Dk);
                SW.Write(Dk.GetTextFormat(PrintType.Vertical, true, 2, 2, "Vector Dk"));

                SW.Write("\r\n Solving Linear Equations Systems with Cramers Rule: ");
                SW.Write(X.GetTextFormat(PrintType.Vertical, true, 2, 10, "Vector X"));

                double error = LinearEquationsSystems.AccuracyError(A, X, b);
                SW.WriteLine($"\r\n Deteminant|A| = {A.Determinant,12:F2}  Error = {error,10:E1}");
            }
        }
    }
}
