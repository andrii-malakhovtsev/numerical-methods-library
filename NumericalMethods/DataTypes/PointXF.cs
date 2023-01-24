namespace NumericalMethods
{
    public class PointXF
    {
        public PointXF(double x, double f) { X = x; F = f; }

        public double X { get; internal set; } = double.NaN;
        public double F { get; internal set; } = double.NaN;

        public string ToPrint()
        {
            return $"    ({X,16:F10},{F,16:F10} )";
        }
    }
}
