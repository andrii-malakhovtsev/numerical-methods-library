using System;

namespace NumericalMethods
{
    public static class Equations
    {
        public static double Dichotomy(double leftGraphEdge, double rightGraphEdge, double error,
            Func<double, double> function, out int iterationCount)
        {
            return DichotomyAlgorithm(function, root: null, error, leftGraphEdge, rightGraphEdge,
                out iterationCount);
        }

        public static void Dichotomy(Func<double, double> function, Root root, double error)
        {
            root.Iterations = 0;
            root.X = DichotomyAlgorithm(function, root, error, leftGraphEdge: root.LeftEdgeX,
                rightGraphEdge: root.RightEdgeX, out root.Iterations);
            return;
        }

        private static double DichotomyAlgorithm(Func<double, double> function, Root root,
            double error, double leftGraphEdge, double rightGraphEdge, out int iterationsCount)
        {
            double graphExtremum = 0.0;
            iterationsCount = 0;
            const int maxIterationsCount = 70;
            while (iterationsCount < maxIterationsCount)
            {
                double correctRightGraphEdge = rightGraphEdge;
                if ((leftGraphEdge * rightGraphEdge) < 0)
                {
                    correctRightGraphEdge -= leftGraphEdge;
                }
                graphExtremum = (leftGraphEdge + correctRightGraphEdge) * 0.5;
                iterationsCount++;
                if ((function(leftGraphEdge) * function(graphExtremum)) < 0)
                {
                    rightGraphEdge = graphExtremum;
                }
                else
                {
                    leftGraphEdge = graphExtremum;
                }
                double rootError = Math.Abs(function(graphExtremum));
                if (root != null)
                {
                    root.Iterations = iterationsCount;
                    root.Error = rootError;
                }
                if ((rootError < error) || ((rightGraphEdge - leftGraphEdge) * 10 < error)) break;
            }
            return graphExtremum;
        }

        public static double Tangent(Func<double, double> function, 
            Func<double, double> functionFirstDerivative, Func<double, double> functionSecondDerivative, 
            double leftGraphEdge, double rightGraphEdge, double error, out int iterationCount)
        {
            const int maxIterationsCount = 15;
            double xByIteration = GetXByIteration(leftGraphEdge, rightGraphEdge);
            if ((function(leftGraphEdge) * functionSecondDerivative(leftGraphEdge)) > 0)
            {
                xByIteration = leftGraphEdge;
            }
            if ((function(rightGraphEdge) * functionSecondDerivative(rightGraphEdge)) > 0)
            {
                xByIteration = rightGraphEdge;
            }
            iterationCount = 0;
            return TangentIterations(function, functionFirstDerivative, error, 
                ref iterationCount, maxIterationsCount, xByIteration, functionDerivative: 0, 
                firstDerivative: true);
        }

        public static double Tangent(Func<double, double> function, double leftGraphEdge,
            double rightGraphEdge, double error, out int iterationCount)
        {
            const int maxIterationsCount = 25;
            double xByIteration = GetXByIteration(leftGraphEdge, rightGraphEdge),
                functionDerivative = (function(rightGraphEdge) - function(leftGraphEdge))
                / (rightGraphEdge - leftGraphEdge);
            iterationCount = 0;
            return TangentIterations(function, functionFirstDerivative: null, error, ref iterationCount, 
                maxIterationsCount, xByIteration, functionDerivative, firstDerivative: false);
        }

        private static double TangentIterations(Func<double, double> function, 
            Func<double, double> functionFirstDerivative,
            double error, ref int iterationCount, int maxIterationCount, double xByIteration, 
            double functionDerivative, bool firstDerivative)
        {
            while (Math.Abs(function(xByIteration)) > error)
            {
                double derivativeFunction = firstDerivative ?
                    functionFirstDerivative(xByIteration) : functionDerivative;
                xByIteration -= function(xByIteration) / derivativeFunction;
                iterationCount++;
                if (iterationCount > maxIterationCount) break;
            }
            return xByIteration;
        }

        public static void Tangent(Func<double, double> function, 
            Func<double, double> functionFirstDerivative,
            Func<double, double> functionSecondDerivative, Root root, double error)
        {
            const int maxIterationCount = 15;
            root.X = GetXByIteration(root.LeftEdgeX, root.RightEdgeX);
            if ((function(root.LeftEdgeX) * functionSecondDerivative(root.LeftEdgeX)) > 0)
            {
                root.X = root.LeftEdgeX;
            }
            if ((function(root.RightEdgeX) * functionSecondDerivative(root.RightEdgeX)) > 0)
            {
                root.X = root.RightEdgeX;
            }
            root.Iterations = 0;
            while (Math.Abs(function(root.X)) > error)
            {
                root.X -= function(root.X) / functionFirstDerivative(root.X); 
                root.Iterations++;
                if (root.Iterations > maxIterationCount) break;
            }
            root.Error = Math.Abs(function(root.X));
        }

        private static double GetXByIteration(double leftGraphEdge, double rightGraphEdge)
        {
            return (rightGraphEdge + leftGraphEdge) * 0.5;
        }
    }
}
