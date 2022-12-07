using System;

namespace NumericalMethods
{
    public class FunctionTable : Table
    {
        protected Func<double, double> function;

        public FunctionTable(double xFirst, double xSpecific, int specificValue,
            Func<double, double> function, string title)
        {
            Title = title; this.function = function; Points = new PointXF[specificValue + 1];
            Min = new PointXF(double.NaN, double.MaxValue);
            Max = new PointXF(double.NaN, double.MinValue);
            double hx = (xSpecific - xFirst) / specificValue, xByIndex;
            for (int index = 0; index <= specificValue; index++)
            {
                xByIndex = xFirst + index * hx; 
                Points[index] = new PointXF(xByIndex, this.function(xByIndex));
                if (Min.F > Points[index].F) { Min = Points[index]; }
                if (Max.F < Points[index].F) { Max = Points[index]; }
            }
            RootsLocation();
        }

        public override void RootsCorrection(double eps)
        {
            if (Roots == null) return;
            foreach (Root root in Roots)
                Equations.Dichotomy(function, root, eps);
        }

        public override string ToPrint(string comment)
        {
            return comment + "\r\n" + FunctionTable();
        }
    }
}
