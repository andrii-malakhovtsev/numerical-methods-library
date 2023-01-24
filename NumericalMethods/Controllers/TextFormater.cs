using System;

namespace NumericalMethods
{
    public static class TextFormater
    {
        private const int NormalIndent = 3, SmallIndent = 1;
        private static int s_heightIndex, s_widthIndex;

        public static string GetMatrixAndVectorTextFormat(Matrix matrix, Vector vector, bool form,
            int forwardFormatIndent, int formatIndent, string title)
        {
            int matrixPower, vectorPower, size = matrix.Size;
            string text = "\r\n", formatA, formatB, formatName;
            if (title != "")
            {
                text += $"  {title}   Size = {size}\r\n";
            }
            if (form)
            {
                FormMatrixAndVectorText(matrix, vector, forwardFormatIndent, formatIndent, 
                    out matrixPower, out vectorPower, size, out formatName);
            }
            else
            {
                matrixPower = forwardFormatIndent + NormalIndent + formatIndent + SmallIndent + SmallIndent + NormalIndent;
                vectorPower = matrixPower + SmallIndent + SmallIndent;
                formatName = "E";
            }
            formatA = GetFormatString(formatIndent, matrixPower, formatName);
            formatB = GetFormatString(formatIndent, vectorPower, formatName);
            for (s_widthIndex = 1; s_widthIndex <= size; s_widthIndex++)
            {
                for (s_heightIndex = 1; s_heightIndex <= size; s_heightIndex++)
                {
                    text += string.Format(formatA, matrix[s_widthIndex, s_heightIndex]);
                }
                text += " " + string.Format(formatB, vector[s_widthIndex]) + "\r\n";
            }
            return text;
        }

        private static void FormMatrixAndVectorText(Matrix matrix, Vector vector, int forwardFormatIndent, int formatIndent, 
            out int matrixPower, out int vectorPower, int size, out string formatName)
        {
            int maxMatrixPower = 0, maxVectorPower = 0;
            for (s_widthIndex = 1; s_widthIndex <= size; s_widthIndex++)
            {
                for (s_heightIndex = 1; s_heightIndex <= size; s_heightIndex++)
                {
                    RefreshMaxPower(matrix, vector: null, out _, ref maxMatrixPower);
                }
                RefreshMaxPower(matrix: null, vector, out _, ref maxVectorPower);
            }
            matrixPower = forwardFormatIndent + SmallIndent + maxMatrixPower + SmallIndent + formatIndent;
            vectorPower = forwardFormatIndent + SmallIndent + maxVectorPower + SmallIndent + formatIndent
                + SmallIndent + SmallIndent;
            formatName = "F";
        }

        private static string GetTextFormat(Matrix matrix, Vector vector, bool form, int forwardFormatIndent, 
            int formatIndent, string title, ref string format)
        {
            bool getMatrix = matrix != null;
            int size = getMatrix ? matrix.Size : vector.Size, power = forwardFormatIndent;
            string text = "\r\n";
            if (title != "")
            {
                text += $" {title}  Size = {size}\r\n";
            }
            if (form)
            {
                int maxPower = 0;
                if (getMatrix)
                    RefreshMatrixValues(matrix, ref power, ref maxPower);
                else
                    RefreshVectorValues(vector, ref power, ref maxPower);
                power += SmallIndent + maxPower + SmallIndent + formatIndent;
            }
            else
            {
                power += NormalIndent + formatIndent + SmallIndent + SmallIndent + NormalIndent;
            }
            format = GetFormatString(formatIndent, power, formatF: form);
            return text;
        }

        private static string GetFormatString(int formatIndent, int power, string formatName)
        {
            return "{0," + $"{power}" + ":" + formatName + $"{formatIndent}" + "}";
        }

        private static string GetFormatString(int formatIndent, int power, bool formatF)
        {
            string format = formatF ? ":F" : $":E";
            return "{0," + $"{power}" + format + $"{formatIndent}" + "}";
        }

        public static string GetMatrixTextFormat(Matrix matrix, bool form, int forwardFormatIndent,
            int formatIndent, string title)
        {
            string format = "",
                text = GetTextFormat(matrix, vector: null, form, forwardFormatIndent, formatIndent, 
                title, ref format);
            for (s_widthIndex = 1; s_widthIndex <= matrix.Size; s_widthIndex++)
            {
                for (s_heightIndex = 1; s_heightIndex <= matrix.Size; s_heightIndex++)
                {
                    text += string.Format(format, matrix[s_widthIndex, s_heightIndex]);
                }
                text += "\r\n";
            }
            return text;
        }

        public static string GetVectorTextFormat(Vector vector, PrintType type, bool form,
            int forwardFormatIndent, int formatIndent, string title)
        {
            string format = "",
                   text = GetTextFormat(matrix: null, vector, form, forwardFormatIndent, formatIndent, 
                   title, ref format);
            for (int index = 1; index <= vector.Size; index++)
            {
                switch (type)
                {
                    case PrintType.Horizontal:
                        text += string.Format(format, vector[index]);
                        break;
                    case PrintType.Vertical:
                        text += $"{index,3}" + string.Format(format, vector[index]) + "\r\n";
                        break;
                }
            }
            if (type == PrintType.Horizontal)
            {
                text += "\r\n";
            }
            return text;
        }

        private static void RefreshMatrixValues(Matrix matrix, ref int power, ref int maxPower)
        {
            for (s_widthIndex = 1; s_widthIndex <= matrix.Size; s_widthIndex++)
            {
                for (s_heightIndex = 1; s_heightIndex <= matrix.Size; s_heightIndex++)
                {
                    RefreshVectorValue(ref maxPower, ref power,
                        absoluteValue: Math.Abs(matrix[s_widthIndex, s_heightIndex]));
                }
            }
        }

        private static void RefreshVectorValues(Vector vector, ref int power, ref int maxPower)
        {
            for (int index = 1; index <= vector.Size; index++)
            {
                RefreshVectorValue(ref maxPower, ref power, absoluteValue: Math.Abs(vector[index]));
            }
        }

        private static void RefreshVectorValue(ref int maxPower, ref int power, double absoluteValue)
        {
            power = absoluteValue < Vector.MaxValue ? 1 : (int)Math.Ceiling(Math.Log10(absoluteValue));
            maxPower = Math.Max(maxPower, power);
        }

        private static void RefreshMaxPower(Matrix matrix, Vector vector, out int power, ref int maxPower)
        {
            double value = matrix != null ? matrix[s_widthIndex, s_heightIndex] : vector[s_widthIndex];
            value = Math.Abs(value);
            power = 0;
            RefreshVectorValue(ref maxPower, ref power, value);
        }
    }
}
