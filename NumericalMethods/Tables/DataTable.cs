using System.Collections.Generic;

namespace NumericalMethods
{
    public class DataTable : Table
    {
        public DataTable(string path, string title)
        {
            List<PointXF> temporaryTable =
                FilesController.WriteDataTableToFile(dataTable: this, path, title);
            if (temporaryTable != null)
            {
                temporaryTable.Sort((point_i, point_j) => point_i.X.CompareTo(point_j.X));
                Points = temporaryTable.ToArray();
                Min = new PointXF(double.NaN, double.MaxValue);
                Max = new PointXF(double.NaN, double.MinValue);
                for (int i = 0; i < Length; i++)
                {
                    if (Min.F > Points[i].F) { Min = Points[i]; }
                    if (Max.F < Points[i].F) { Max = Points[i]; }
                }
            }
            else
            {
                Points = null;
                Min = null;
                Max = null;
            }
            Title = title;
            RootsLocation();
        }

        public string FileTable { get; set; }

        public override string ToPrint(string comment)
        {
            return comment + "\r\n" + FileTable + FunctionTable();
        }
    }
}
