using System.IO;
using FilesController = NumericalMethods.FilesController;
using Matrix = NumericalMethods.Matrix;

namespace LUdecompositionTest
{
    class LUdecompositionTest
    {
        static void Main(string[] args)
        {
            using(StreamWriter SW = FilesController.WriteResultToFile("LUdecompositionResult.txt"))
            {
                Matrix A = new Matrix("TestMatrix.txt");

                A.LowerUpperDecomposition(out Matrix L, out Matrix U);

                SW.WriteLine(L.GetTextFormat(true, 1, 2, " Matrix L"));
                SW.WriteLine($"   L.Det = {L.Determinant:F3}");
                SW.WriteLine(U.GetTextFormat(true, 1, 2, " Matrix U")); 
                SW.WriteLine($"   U.Det = {U.Determinant:F3}");
                SW.WriteLine(A.GetTextFormat(true, 1, 2, " Matrix A")); 
                SW.WriteLine($"   A.Det = {A.Determinant:F3}");
                SW.WriteLine((L * U).GetTextFormat(true, 1, 2, " Matrix L*U"));
            }
        }
    }
}
