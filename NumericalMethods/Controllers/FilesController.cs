using System;
using System.IO;
using System.Collections.Generic;
using CultureInfo = System.Globalization.CultureInfo;

namespace NumericalMethods
{
    public static class FilesController
    {
        private static int s_heightIndex, s_widthIndex;

        public static StreamWriter WriteResultToFile(string fileName, params string[] files)
        {
            string projectDirectory =
                   Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
            var writer = new StreamWriter(fileName);
            foreach (string file in files)
            {
                writer.WriteLine(file + ":\r\n");
                writer.Flush();
                using (var reader = new StreamReader(projectDirectory + '\\' + file))
                {
                    reader.BaseStream.CopyTo(writer.BaseStream);
                }
                writer.WriteLine("\r\n// " + new string('-', 38) + " //\r\n");
            }
            writer.Write(fileName);
            writer.WriteLine("\n");
            return writer;
        }

        public static void ReadMatrixWithVectorFromFile(string path, out Matrix matrix,
            out Vector vector, out int readInt)
        {
            var file = new FileInfo(path);
            if (CheckForTextExtension(file.Extension))
            {
                var reader = new StreamReader(file.OpenRead());
                readInt = Convert.ToInt32(reader.ReadLine()); 
                matrix = new Matrix(readInt); 
                vector = new Vector(readInt);
                for (s_widthIndex = 1; s_widthIndex <= readInt; s_widthIndex++)
                {
                    string[] dataFromFile = GetDataFromFile(reader);
                    for (s_heightIndex = 1; s_heightIndex <= readInt; s_heightIndex++)
                    {
                        matrix[s_widthIndex, s_heightIndex] = Convert.ToDouble(dataFromFile[s_heightIndex - 1]);
                    }
                    vector[s_widthIndex] = Convert.ToDouble(dataFromFile[readInt]);
                }
                reader.Close(); return;
            }
            else
            {
                matrix = null; 
                vector = null; 
                readInt = 0; 
            }
        }

        private static bool SetMatrixOrVectorFromFile(string path, Matrix matrix = null, Vector vector = null)
        {
            bool setMatrix = matrix != null;
            var file = new FileInfo(path);
            bool rightFileExtension = CheckForTextExtension(file.Extension);
            if (rightFileExtension)
            {
                var reader = new StreamReader(file.OpenRead());
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
                GetMatrixAndVectorFromFile(matrix, vector, setMatrix, reader, size);
                reader.Close();
            }
            return rightFileExtension;
        }

        private static void GetMatrixAndVectorFromFile(Matrix matrix, Vector vector, bool setMatrix, 
            StreamReader reader, int size)
        {
            for (s_widthIndex = 1; s_widthIndex <= size; s_widthIndex++)
            {
                string[] dataFromFile = GetDataFromFile(reader);
                if (setMatrix)
                {
                    for (s_heightIndex = 1; s_heightIndex <= size; s_heightIndex++)
                    {
                        matrix[s_widthIndex, s_heightIndex] =
                            Convert.ToDouble(dataFromFile[s_heightIndex - 1]);
                    }
                }
                else vector[s_widthIndex] = Convert.ToDouble(dataFromFile[s_widthIndex - 1]);
            }
        }

        internal static bool SetMatrixFromFile(Matrix matrix, string path)
            => SetMatrixOrVectorFromFile(path, matrix);

        internal static bool SetVectorFromFile(Vector vector, string path)
            => SetMatrixOrVectorFromFile(path, matrix: null, vector);

        private static string[] GetDataFromFile(StreamReader reader)
        {
            string line = reader.ReadLine().Trim();
            bool dotOverComma = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator == ".";
            line = dotOverComma ? line.Replace(",", ".") : line.Replace(".", ",");
            string[] numbers = line.Split(new char[] { ' ', ';' }, StringSplitOptions.RemoveEmptyEntries);
            return numbers;
        }

        private static bool CheckForTextExtension(string extension)
        {
            const string textExtension = ".txt";
            return extension == textExtension;
        }

        internal static List<PointXF> WriteDataTableToFile(DataTable dataTable, string path, string title)
        {
            var temporaryTable = new List<PointXF>();
            var file = new FileInfo(path);
            int index = -1;
            dataTable.FileTable = $"\r\n Table {title} in file {file.Name}:\r\n";
            if (CheckForTextExtension(file.Extension))
            {
                var reader = new StreamReader(file.OpenRead());
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
    }
}
