using System.Collections.Generic;
using System.IO;

namespace NumericalMethods
{
    public abstract class Table
    {
        public PointXF[] Points { get; protected set; } = null;
        public PointXF Max { get; protected set; } = null;
        public PointXF Min { get; protected set; } = null;
        public List<Root> Roots { get; internal set; }
        public int Length { get { if (Points == null) return 0; else return Points.Length; } }
        public double Epsilon { get; set; }
        public string Title { get; protected set; } = null;

        public double RegionX
        {
            get { if (Points == null) return double.NaN; return Points[Length - 1].X - Points[0].X; }
        }

        public double RegionF
        {
            get { if (Points == null) return double.NaN; return Max.F - Min.F; }
        }

        public double X(int index)
        {
            if ((index >= 0) && (index < Length)) return Points[index].X;
            else return double.NaN;
        }

        public double F(int index)
        {
            if ((index >= 0) && (index < Length)) return Points[index].F;
            else return double.NaN;
        }

        public void ToArrays(out double[] x, out double[] f)
        {
            x = new double[Length]; f = new double[Length];
            for (int i = 0; i < Length; i++) { x[i] = Points[i].X; f[i] = Points[i].F; }
        }

        public virtual void ToTxtFile(string path, string comment)
        {
            if (path == "") path = "Table.txt";
            FileInfo file = new FileInfo(path); if (file.Exists) file.Delete();
            StreamWriter SW = new StreamWriter(file.OpenWrite());
            SW.Write(comment + "\r\n" + FunctionTable()); SW.Close();
        }

        public virtual string ToPrint(string comment) { return comment; }

        public virtual void RootsCorrection(double eps) { }

        protected void RootsLocation()
        {
            int counter = 0;
            for (int i = 1; i < Length; i++)
            {
                if (Points[i - 1].F * Points[i].F < 0)
                {
                    counter++; if (counter == 1) { Roots = new List<Root>(); }
                    Roots.Add(new Root(Points[i - 1].X, Points[i].X));
                }
            }
        }

        public string RootsTable(string comment)
        {
            string table = comment + "\r\n";
            if (Roots != null)
            {
                table += " Zeros Table of function: " + Title + "\r\n";
                for (int j = 0; j < Roots.Count; j++)
                    table += $"{j,3}" + Roots[j].ToPrint() + "\r\n";
            }
            else { table += " Zeros Table of " + Title + " is empty!\r\n"; }
            return table;
        }

        public virtual string FunctionTable()
        {
            string txt = "\r\n Function table " + Title + " : \r\n";
            for (int i = 0; i < Length; i++) { txt += $"{i,4}" + Points[i].ToPrint() + "\r\n"; }
            txt += $"\r\n x = [{Points[0].X,17:F12}   :{Points[Length - 1].X,17:F12}  ]";
            txt += $"   x_Reg = {RegionX,16:F12}\r\n";
            txt += $"\r\n    Min ({Min.X,18:F12}, {Min.F,18:F12} )";
            txt += $"\r\n    Max ({Max.X,18:F12}, {Max.F,18:F12} )";
            txt += $"    f_Reg = {RegionF,16:F12}\r\n";
            return txt + RootsTable("");
        }
    }
}
