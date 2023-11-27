namespace NumericalMethods
{
    public class PointXF
    {
        internal PointXF(double x, double f) 
        { 
            X = x;
            F = f; 
        }

        internal double X { get; private set; } = double.NaN;

        internal double F { get; private set; } = double.NaN;

        internal string ToPrint() => $"    ({X,16:F10},{F,16:F10} )";
    }
}
