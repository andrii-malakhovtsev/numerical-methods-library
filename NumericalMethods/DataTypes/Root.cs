namespace NumericalMethods
{
    public class Root
    {
        internal double LeftEdgeX, RightEdgeX, X, Error;
        internal int Iterations;

        internal Root(double leftX, double rightX)
        {
            LeftEdgeX = leftX; 
            RightEdgeX = rightX;
            X = double.NaN;
            Error = double.NaN;
            Iterations = 0; 
        }

        internal string ToPrint()
        {
            return $"   [{LeftEdgeX,10:F5},{RightEdgeX,10:F5}   ]   " +
                $"Root ={X,16:F12}  Error ={Error,10:E1}  Iterations = {Iterations}";
        }
    }
}
