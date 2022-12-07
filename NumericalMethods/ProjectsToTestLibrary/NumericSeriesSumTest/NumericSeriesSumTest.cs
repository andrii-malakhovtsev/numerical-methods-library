using System;
using System.IO;
using NumericSeriesSum = NumericalMethods.NumericSeriesSum;

namespace NumericSeriesSumTest
{
    class NumericSeriesSumTest
    {
        private const int N = 22500;
        private const double a = 1.13, b = -2.56, c = 0.84,
            d = -3.79, error = 1.0E-9, dlt = 1.0E-8;

        static void Main(string[] args)
        {
            using (StreamWriter writer = 
                NumericalMethods.FilesController.WriteResultToFile("NumericSeriesSumResult.txt",
                "NumericSeriesSumTest.cs"))
            {
                double SN = NumericSeriesSum.NumberSeriesSum(1, N, Sk);
                writer.WriteLine($"    N = {N,10}  SN ={SN,15:F10}");

                int iMax = 0;
                double S1 = NumericSeriesSum.NumberSeriesSumCountingAbsoluteError(0, error, Si, ref iMax);
                writer.WriteLine($"i max = {iMax,10}  S1 ={S1,15:F10}");

                int jMax = 0;
                double S2 = NumericSeriesSum.NumberSeriesSumCountingRelativeError(1, dlt, Sj, ref jMax);
                writer.WriteLine($"j max = {jMax,10}  S2 ={S2,15:F10}");
            }
        }

        public static double Sk(int k)
        {
            return Math.Pow((k + 5.0 / k), 1.0/4.0) / (Math.Sqrt(5.0 * k) - 3.0) / (2.0 * k - 7.0);
        }

        public static double Si(int i)
        {
            double x1 = a / ((i * i + 9) * (i - b));
            double x2 = (i + 2 * a); x2 = b / x2 / x2 / x2;
            return x1 - x2;
        }

        public static double Sj(int j)
        {
            double j2 = 1.0 * j * j;
            double x1 = (d - 2.0 * Math.Pow(j,-1));
            double x2 = Math.Sqrt(j + c) * (j2 + 9.0);
            if ((j % 2) == 0) return -x1 / x2; else return x1 / x2;
        }
    }
}
