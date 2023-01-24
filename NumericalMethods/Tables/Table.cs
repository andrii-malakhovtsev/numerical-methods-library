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
        public string Title { get; protected set; } = null;

        public double RegionX {  get => Points != null ? Points[Length - 1].X - Points[0].X : double.NaN; }
        public double RegionF { get => Points != null ? Max.F - Min.F : double.NaN; }
        public double X(int index)
        {
            if ((index >= 0) && (index < Length)) 
                return Points[index].X;
            return double.NaN;
        }
        public double F(int index)
        {
            if ((index >= 0) && (index < Length)) 
                return Points[index].F;
            return double.NaN;
        }
        public int Length { get => Points == null ? 0 : Points.Length; }

        public void ToArrays(out double[] x, out double[] f)
        {
            x = new double[Length];
            f = new double[Length];
            for (int i = 0; i < Length; i++) 
            { 
                x[i] = Points[i].X; f[i] = Points[i].F; 
            }
        }

        public virtual void ToTxtFile(string path, string comment)
        {
            if (path == "") {
                path = "Table.txt";
            }
            FileInfo file = new FileInfo(path);
            if (file.Exists) { 
                file.Delete(); 
            }
            StreamWriter writer = new StreamWriter(file.OpenWrite());
            writer.Write(comment + "\r\n" + FunctionTable()); 
            writer.Close();
        }

        public virtual string ToPrint(string comment) 
        { 
            return comment; 
        }

        public virtual void RootsCorrection(double eps) { } // don't use abstract

        protected void RootsLocation()
        {
            int counter = 0;
            for (int index = 1; index < Length; index++)
            {
                if (Points[index - 1].F * Points[index].F < 0)
                {
                    counter++; 
                    if (counter == 1) 
                    { 
                        Roots = new List<Root>(); 
                    }
                    Roots.Add(new Root(Points[index - 1].X, Points[index].X));
                }
            }
        }

        public string RootsTable(string comment)
        {
            string table = comment + "\r\n";
            table += " Zeros Table of function: " + Title;
            if (Roots != null)
            {
                for (int index = 0; index < Roots.Count; index++)
                {
                    table += "\r\n" + $"{index,3}" + Roots[index].ToPrint();
                }
            }
            else
            { 
                table += " is empty!"; 
            }
            table += "\r\n";
            return table;
        }

        public virtual string FunctionTable()
        {
            string txt = "\r\n Function table " + Title + " : \r\n";
            for (int i = 0; i < Length; i++) 
            { 
                txt += $"{i,4}" + Points[i].ToPrint() + "\r\n"; 
            }
            txt += $"\r\n x = [{Points[0].X,17:F12}   :{Points[Length - 1].X,17:F12}  ]";
            txt += $"   x_Reg = {RegionX,16:F12}\r\n";
            txt += $"\r\n    Min ({Min.X,18:F12}, {Min.F,18:F12} )";
            txt += $"\r\n    Max ({Max.X,18:F12}, {Max.F,18:F12} )";
            txt += $"    f_Reg = {RegionF,16:F12}\r\n";
            return txt + RootsTable("");
        }
    }
}
