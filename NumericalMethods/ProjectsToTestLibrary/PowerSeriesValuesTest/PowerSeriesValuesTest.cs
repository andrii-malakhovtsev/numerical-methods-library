using System;
using System.IO;
using PowerSeries = NumericalMethods.PowerSeries;
using FilesController = NumericalMethods.FilesController;

namespace PowerSeriesValuesTest
{
    internal class PowerSeriesValuesTest
    {
        static double A, B, C, e, d0, d1, d2, d3, d4, error;

        static void Main(string[] args)
        {
            Console.Write(TestSinCos());
            Console.WriteLine();
            Console.Write(TestSinHyperbolicCosHyperbolic());
            Console.WriteLine();
            Console.WriteLine("Exponent test");
            Console.WriteLine($"{Math.Log(PowerSeries.Exponent(-10.3, 1.0e-20)), 36:F22}");

            A = 1.0; B = 1.7; C = -1.1; e = 1.0E-20;
            Console.WriteLine();
            Console.WriteLine(TestNumericalMethodsLibraryExperiment());
            Console.WriteLine();
            Console.WriteLine(TestMicrosoftMathLibrary());
            PowerSeriesValues("PowerSeriesValuesResult.txt");
            Console.ReadKey();
        }

        static void PowerSeriesValues(string file)
        {
            using (StreamWriter writer = FilesController.WriteResultToFile(file, "PowerSeriesValuesTest.cs"))
            {
                writer.WriteLine("\r\n" + file + "\r\n");
                A = -13.60; B = 24.70; e = 1.0E-19;
                writer.WriteLine("Test 1 " + NumericalMethodsLibraryExperiment());
                writer.WriteLine("Test 1 " + MathFunction() + "\r\n");
                A = 3.35; B = -7.91;
                writer.WriteLine("Test 2 " + NumericalMethodsLibraryExperiment());
                writer.WriteLine("Test 2 " + MathFunction());
            }
        }

        static string NumericalMethodsLibraryExperiment()
        {
            d0 = PowerSeries.Cos(A, e) - PowerSeries.Cos(B, e);
            d1 = PowerSeries.Sin((A + B) / 2.0, e);
            d2 = PowerSeries.Sin((B - A) / 2.0, e);
            error = Math.Abs(d0 - (2.0 * (d1 * d2)));
            return ($"  NumericalMethodsLibrary ={d0,19:F16}    error ={error,10:E2}");
        }

        static string MathFunction()
        {
            d0 = Math.Cos(A) - Math.Cos(B);
            d1 = Math.Sin((A + B) / 2.0);
            d2 = Math.Sin((B - A) / 2.0);
            error = Math.Abs(d0 - (2.0 * (d1 * d2)));
            return ($"  Math                    ={d0,19:F16}    error ={error,10:E2}");
        }

        static string TestNumericalMethodsLibraryExperiment()
        {
            d0 = PowerSeries.Sin(A + B + C, e);
            d1 = PowerSeries.Sin(A, e) * PowerSeries.Cos(B, e) * PowerSeries.Cos(C, e);
            d2 = PowerSeries.Cos(A, e) * PowerSeries.Sin(B, e) * PowerSeries.Cos(C, e);
            d3 = PowerSeries.Cos(A, e) * PowerSeries.Cos(B, e) * PowerSeries.Sin(C, e);
            d4 = PowerSeries.Sin(A, e) * PowerSeries.Sin(B, e) * PowerSeries.Sin(C, e);
            error = Math.Abs(d0 - (d1 + d2 + d3 - d4));
            return ($"  NumericalMethodsLibrary ={d0,19:F16}    error ={error,10:E2}");
        }

        static string TestMicrosoftMathLibrary()
        {
            d0 = Math.Sin(A + B + C);
            d1 = Math.Sin(A) * Math.Cos(B) * Math.Cos(C);
            d2 = Math.Cos(A) * Math.Sin(B) * Math.Cos(C);
            d3 = Math.Cos(A) * Math.Cos(B) * Math.Sin(C);
            d4 = Math.Sin(A) * Math.Sin(B) * Math.Sin(C);
            error = Math.Abs(d0 - (d1 + d2 + d3 - d4));
            return ($"  Microsoft.Math          ={d0,19:F16}    error ={error,10:E2}");
        }

        static string TestSinHyperbolicCosHyperbolic()
        {
            string txt = "    Test of SinHyperbolicCosHyperbolic() \r\n";
            double unit, error, e = 1.0E-20;
            for (double x = 0.0; x <= 20.0; x += 1.0)
            {
                unit = (PowerSeries.CosHyperbolic(x, e) - PowerSeries.SinHyperbolic(x, e))
                    * (PowerSeries.CosHyperbolic(x, e) + PowerSeries.SinHyperbolic(x, e));
                error = Math.Abs(1.0 - unit);
                txt += $"{x,7:F1} {unit,22:F16} {error,22:F16}\r\n";
            }
            return txt;
        }

        static string TestSinCos()
        {
            string txt = "    Test of SinCos() \r\n";
            double unit, error, e = 1.0E-20;
            for (double x = 1.0; x <= 40.0; x += 1.0)
            {
                unit = PowerSeries.Cos(x, e) * PowerSeries.Cos(x, e) + 
                    PowerSeries.Sin(x, e) * PowerSeries.Sin(x, e);
                error = Math.Abs(1.0 - unit);
                txt += $"{x,7:F1}{unit,20:F16}{error,20:F16}\r\n";
            }
            return txt;
        }
    }
}
