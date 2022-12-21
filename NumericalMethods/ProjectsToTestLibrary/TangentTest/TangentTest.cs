using System;
using System.IO;
using Equations = NumericalMethods.Equations;
using FilesController = NumericalMethods.FilesController;
using FunctionTable = NumericalMethods.FunctionTable;

namespace TangentTest
{
    class TangentTest
    {
        const double Error = 1.0E-12;

        static void Main()
        {
            using (StreamWriter writer = FilesController.WriteResultToFile("TangentResult.txt"))
            {
                const double xByZero = -0.5, xByIteration = 0.5;
                const int iterationsCount = 340;
                FunctionTable T_Fx = new FunctionTable(xByZero, xByIteration, iterationsCount, Fx, "Fx");
                T_Fx.RootsCorrection(Error);
                writer.Write(T_Fx.RootsTable(" -------- TangentTest -------- "));
                double xr = double.NaN;
                writer.WriteLine($"  x0 = {xByZero}, xn = {xByIteration}, n = {iterationsCount}");
                for (int j = 0; j < T_Fx.Roots.Count; j++)
                {
                    xr = Equations.Tangent(Fx, Determinant1F, Determinant2F, 
                        T_Fx.Roots[j].LeftEdgeX, T_Fx.Roots[j].RightEdgeX, 
                        Error, out int K);
                    writer.WriteLine($"  d1f = {Determinant1F(xr)}");
                    writer.WriteLine($"  d2f = {Determinant2F(xr)}");
                }
            }
        }

        public static double Fx(double x) // constant function we are testing
        {
            return Math.Cos(Math.Sqrt(7.0) * x - 1.0) + 1.0 - Math.Exp(-x / Math.PI);
        }

        public static double Determinant1F(double x)
        {
            return -Math.Sqrt(7) * Math.Sin(Math.Sqrt(7) * x - 1) + (Math.Exp(-x / Math.PI) / Math.PI);
        }

        public static double Determinant2F(double x)
        {
            return -(7.0 * Math.Cos(Math.Sqrt(7) * x - 1) + (Math.Exp(-x / Math.PI) / (Math.PI * Math.PI)));
        }
    }
}
