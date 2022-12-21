using System;
using GraphicsForm = NumericalMethods.GraphicsForm;

namespace SinusGraphicTest
{
    class SinusGraphicTest
    {
        static void Main()
        {
            SinusGraphic(iterations: 100, 
                xByZero: - Math.PI, 
                xByIteration: Math.PI, 
                width: 800, height: 300);
        }

        static void SinusGraphic(int iterations, double xByZero, double xByIteration, 
            int width, int height)
        {
            double h = (xByIteration - xByZero) / iterations;
            double[] x = new double[iterations + 1]; 
            double[] f = new double[iterations + 1];
            for (int index = 0; index <= iterations; index++)
            {
                x[index] = xByZero + index * h; 
                f[index] = Math.Sin(x[index]);
            }
            GraphicsForm.SingleGraphic(x, f, "Graphic of Sin(x)", width, height);
        }
    }
}
