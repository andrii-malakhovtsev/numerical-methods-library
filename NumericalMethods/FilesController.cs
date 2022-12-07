using System;
using System.IO;
using System.Collections.Generic;
using CultureInfo = System.Globalization.CultureInfo;

namespace NumericalMethods
{
    public static class FilesController
    {
        private const double MaxVectorValue = 10.0;
        private const int Indent = 3, SmallIndent = 1;
        private static int s_heightIndex, s_widthIndex;

        public static StreamWriter WriteResultToFile(string fileName, params string[] files)
        {
            string projectDirectory =
                   Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
            StreamWriter writer = new StreamWriter(fileName);
            foreach (string file in files)
            {
                writer.WriteLine(file + ":\r\n");
                writer.Flush();
                using (StreamReader reader = new StreamReader(projectDirectory + '\\' + file))
                    reader.BaseStream.CopyTo(writer.BaseStream);
                writer.WriteLine("\r\n// -------------------------------------- //\r\n");
            }
            writer.Write(fileName);
            writer.WriteLine("\n");
            return writer;
        }

        public static void ReadMatrixAndVectorFromFile(string path, out Matrix matrix,
            out Vector vector, out int readInt)
        {
            FileInfo file = new FileInfo(path);
            if (CheckForRightExtension(file.Extension))
            {
                StreamReader reader = new StreamReader(file.OpenRead());
                readInt = Convert.ToInt32(reader.ReadLine()); 
                matrix = new Matrix(readInt); vector = new Vector(readInt);
                for (s_widthIndex = 1; s_widthIndex <= readInt; s_widthIndex++)
                {
                    string[] dataFromFile = GetDataFromFile(reader);
                    for (s_heightIndex = 1; s_heightIndex <= readInt; s_heightIndex++) 
                        matrix[s_widthIndex, s_heightIndex] = Convert.ToDouble(dataFromFile[s_heightIndex - 1]);
                    vector[s_widthIndex] = Convert.ToDouble(dataFromFile[readInt]);
                }
                reader.Close(); return;
            }
            else { matrix = null; vector = null; readInt = 0; }
        }

        private static bool SetMatrixOrVectorFromFile(Matrix matrix, Vector vector, string path)
        {
            bool setMatrix = matrix != null;
            if (!setMatrix && vector == null) throw new ArgumentNullException(); 
            FileInfo file = new FileInfo(path);
            bool rightFileExtension = CheckForRightExtension(file.Extension);
            if (rightFileExtension)
            {
                StreamReader reader = new StreamReader(file.OpenRead());
                if (setMatrix)
                {
                    matrix.Size = Convert.ToInt32(reader.ReadLine());
                    matrix.MatrixValues = new double[matrix.Size, matrix.Size];
                }
                else 
                { 
                    vector.Size = Convert.ToInt32(reader.ReadLine());
                    vector.VectorValues = new double[vector.Size];
                }
                int size = setMatrix ? matrix.Size : vector.Size;
                for (s_widthIndex = 1; s_widthIndex <= size; s_widthIndex++)
                {
                    string[] dataFromFile = GetDataFromFile(reader);
                    if (setMatrix)
                        for (s_heightIndex = 1; s_heightIndex <= size; s_heightIndex++)
                            matrix[s_widthIndex, s_heightIndex] = 
                                Convert.ToDouble(dataFromFile[s_heightIndex - 1]);
                    else vector[s_widthIndex] = Convert.ToDouble(dataFromFile[s_widthIndex - 1]);
                }
                reader.Close();
            }
            return rightFileExtension;
        }

        public static bool SetMatrixFromFile(Matrix matrix, string path)
        {
            return SetMatrixOrVectorFromFile(matrix, vector: null, path);
        }

        public static bool SetVectorFromFile(Vector vector, string path)
        {
            return SetMatrixOrVectorFromFile(matrix: null, vector, path);
        }

        private static string[] GetDataFromFile(StreamReader reader)
        {
            string line = reader.ReadLine().Trim();
            bool dotOverComma = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator == ".";
            line = dotOverComma ? line.Replace(",", ".") : line.Replace(".", ",");
            string[] numbers = line.Split(new char[] { ' ', ';' }, StringSplitOptions.RemoveEmptyEntries);
            return numbers;
        }

        private static bool CheckForRightExtension(string extension)
        {
            const string rightExtension = ".txt";
            return extension == rightExtension;
        }

        public static List<PointXF> WriteDataTableToFile(DataTable dataTable, string path, string title)
        {
            List<PointXF> temporaryTable = new List<PointXF>();
            FileInfo file = new FileInfo(path);
            int index = -1;
            dataTable.FileTable = $"\r\n Table {title} in file {file.Name}:\r\n";
            if (CheckForRightExtension(file.Extension))
            {
                StreamReader reader = new StreamReader(file.OpenRead());
                while (!reader.EndOfStream)
                {
                    index++;
                    string[] dataFromFile = GetDataFromFile(reader);
                    temporaryTable.Add(new PointXF(Convert.ToDouble(dataFromFile[0]), 
                        Convert.ToDouble(dataFromFile[1])));
                    dataTable.FileTable += $"{index,4}" + temporaryTable[index].ToPrint() + "\r\n";
                }
                reader.Close();
            }
            return temporaryTable;
        }

        public static string GetMatrixAndVectorTextFormat(Matrix matrix, Vector vector, bool form,
            int fs, int fd, string title)
        {
            int kMatrix, kVector, n = matrix.Size;
            string text = "\r\n", frmta, frmtb, formName;
            if (title != "") text += "  " + title + $"   Size = {n}\r\n";
            if (form)
            {
                int maxKMatrix = 0, maxKVector = 0;
                for (s_widthIndex = 1; s_widthIndex <= n; s_widthIndex++)
                {
                    for (s_heightIndex = 1; s_heightIndex <= n; s_heightIndex++)
                        RefreshMaxK(matrix, vector: null, out kMatrix, ref maxKMatrix);
                    RefreshMaxK(matrix: null, vector, out kVector, ref maxKVector);
                }
                kMatrix = fs + SmallIndent + maxKMatrix + SmallIndent + fd;
                kVector = fs + SmallIndent + maxKVector + SmallIndent + fd + SmallIndent + SmallIndent;
                formName = "F";
            }
            else
            {
                kMatrix = fs + Indent + fd + SmallIndent + SmallIndent + Indent; 
                kVector = kMatrix + SmallIndent + SmallIndent;
                formName = "E";
            }
            frmta = GetFormatString(fd, kMatrix, formName);
            frmtb = GetFormatString(fd, kVector, formName);
            for (s_widthIndex = 1; s_widthIndex <= n; s_widthIndex++)
            {
                for (s_heightIndex = 1; s_heightIndex <= n; s_heightIndex++)
                    text += string.Format(frmta, matrix[s_widthIndex, s_heightIndex]);
                text += " " + string.Format(frmtb, vector[s_widthIndex]) + "\r\n";
            }
            return text;
        }

        private static void RefreshMaxK(Matrix matrix, Vector vector, out int k, ref int maxK)
        {
            double value = matrix != null ?
                Math.Abs(matrix[s_widthIndex, s_heightIndex]) : Math.Abs(vector[s_widthIndex]);
            if (value < MaxVectorValue) k = 1;
            else k = (int)Math.Ceiling(Math.Log10(value));
            maxK = Math.Max(maxK, k);
        }

        private static string GetFormatString(int fd, int k, string formName)
        {
            return "{0," + $"{k}" + ":" + formName + $"{fd}" + "}";
        }

        public static string GetMatrixTextFormat(Matrix matrix, bool form, int fs, int fd, string title)
        {
            string format = "", text = GetTextFormat(matrix, vector: null, form, fs, fd, title, ref format);
            for (s_widthIndex = 1; s_widthIndex <= matrix.Size; s_widthIndex++)
            {
                for (s_heightIndex = 1; s_heightIndex <= matrix.Size; s_heightIndex++)
                    text += string.Format(format, matrix[s_widthIndex, s_heightIndex]);
                text += "\r\n";
            }
            return text;
        }

        private static string GetTextFormat(Matrix matrix, Vector vector, bool form, int fs, int fd, 
            string title, ref string format)
        {
            bool getMatrix = matrix != null;
            if (!getMatrix && vector == null) throw new ArgumentNullException();
            int size = getMatrix ? matrix.Size : vector.Size, ka = 0;
            string text = "\r\n";
            if (title != "") text += " " + title + $"  Size = {size}\r\n";
            if (form)
            {
                int maxKa = 0;
                if (getMatrix) GetMatrixValues(matrix, ref ka, ref maxKa);
                else GetVectorValues(vector, ref ka, ref maxKa);
                ka = fs + SmallIndent + maxKa + SmallIndent + fd;
                format = GetFormatString(fd, ka, formatF: true);
            }
            else
            {
                ka = fs + Indent + fd + SmallIndent + SmallIndent + Indent;
                format = GetFormatString(fd, ka, formatF: false);
            }
            return text;
        }

        private static string GetFormatString(int fd, int ka, bool formatF)
        {
            string format = formatF ? ":F" : $":E";
            return "{0," + $"{ka}" + format + $"{fd}" + "}";
        }

        public static string GetVectorTextFormat(Vector vector, PrintType type, bool form, 
            int fs, int fd, string title)
        {
            string format = "", text = GetTextFormat(matrix: null, vector, form, fs, fd, title, ref format);
            for (int index = 1; index <= vector.Size; index++)
            {
                switch (type)
                {
                    case PrintType.Horizontal:
                        text += string.Format(format, vector[index]); break;
                    case PrintType.Vertical:
                        text += $"{index,3}" + string.Format(format, vector[index]) + "\r\n"; break;
                }
            }
            switch (type)
            {
                case PrintType.Horizontal: return text + "\r\n";
                case PrintType.Vertical: return text;
            }
            return "";
        }

        private static void GetMatrixValues(Matrix matrix, ref int ka, ref int maxKa)
        {
            for (s_widthIndex = 1; s_widthIndex <= matrix.Size; s_widthIndex++)
            {
                for (s_heightIndex = 1; s_heightIndex <= matrix.Size; s_heightIndex++)
                {
                    double a = Math.Abs(matrix[s_widthIndex, s_heightIndex]);
                    if (a < MaxVectorValue) ka = 1;
                    else ka = (int)Math.Ceiling(Math.Log10(a));
                    maxKa = Math.Max(maxKa, ka);
                }
            }
        }

        private static void GetVectorValues(Vector vector, ref int ka, ref int maxKa)
        {
            for (int index = 1; index <= vector.Size; index++)
            {
                double a = Math.Abs(vector[index]);
                if (a < MaxVectorValue) ka = 1;
                else ka = (int)Math.Ceiling(Math.Log10(a));
                maxKa = Math.Max(maxKa, ka);
            }
        }
    }
}
