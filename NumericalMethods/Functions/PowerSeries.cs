using System;

namespace NumericalMethods
{
    public static class PowerSeries
    {
        public static double Sin(double x, double error)
        {
            return GetSinCos(x, error, isSin: true, hyperbolic: false);
        }

        public static double Cos(double x, double error)
        {
            return GetSinCos(x, error, isSin: false, hyperbolic: false);
        }

        public static double SinHyperbolic(double x, double error)
        {
            return GetSinCos(x, error, isSin: true, hyperbolic: true);
        }

        public static double CosHyperbolic(double x, double error)
        {
            return GetSinCos(x, error, isSin: false, hyperbolic: true);
        }

        private static double GetSinCos(double x, double error, bool isSin, bool hyperbolic)
        {
            if (x == 0) return isSin ? 0.0 : 1.0;
            double firstIterationMember = isSin ? x : 1.0, 
                   toReturn = firstIterationMember, 
                   powerSeriesMemberByIndex = firstIterationMember, 
                   halfX = 0.5 * x,
                   indexSubtraction = isSin ? 0.5 : 0.0, 
                   sign = hyperbolic ? 1.0 : -1.0;
            for (int index = isSin ? 2 : 1; Math.Abs(powerSeriesMemberByIndex) > error; index++)
            {
                // powerSeriesMemberByIndex *= ... gives wrong results for some reason
                powerSeriesMemberByIndex = powerSeriesMemberByIndex * sign * 
                    (halfX / (index - 0.5 - indexSubtraction))  * (halfX / (index - indexSubtraction));
                toReturn += powerSeriesMemberByIndex;
            }
            return toReturn;
        }

        public static double Exponent(double x, double error = 1.0e-20)
        {
            double exponent = 1.0;
            if (x == 0) return exponent;
            double powerSeriesMemberByIndex = exponent;
            for (int index = 1; Math.Abs(powerSeriesMemberByIndex) > error; index++)
            {
                powerSeriesMemberByIndex *= (x / index); 
                exponent += powerSeriesMemberByIndex; 
                Console.WriteLine($"{index,6}{powerSeriesMemberByIndex,30:F22}");
            }
            return exponent;
        }
    }
}
