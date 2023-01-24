using System;

namespace NumericalMethods
{
    public enum MatrixType { Simple, Identity, Test, Upper, Lower }

    public class Matrix
    {
        private const int MinimalCofactorSize = 2;
        private static int s_widthIndex, s_heightIndex;
        private enum Operator { Plus, Minus, Multiply }

        public Matrix(int size, MatrixType type)
        {
            Size = size;
            MatrixValues = new double[Size, Size];
            switch (type)
            {
                case MatrixType.Simple:
                    MatrixLoop(delegate (int widthIndex, int heightIndex)
                    {
                        this[widthIndex, heightIndex] = double.NaN;
                    });
                    break;
                case MatrixType.Identity:
                    for (int index = 1; index <= Size; index++)
                    {
                        this[index, index] = 1.0;
                    }
                    break;
                case MatrixType.Test:
                    MatrixLoop(delegate (int widthIndex, int heightIndex)
                    {
                        this[widthIndex, heightIndex] = 10.0 * widthIndex + heightIndex;
                    });
                    break;
                case MatrixType.Upper:
                    for (s_widthIndex = 1; s_widthIndex <= Size; s_widthIndex++)
                    {
                        for (s_heightIndex = s_widthIndex; s_heightIndex <= Size; s_heightIndex++)
                        {
                            this[s_widthIndex, s_heightIndex] = s_heightIndex - s_widthIndex + 1.0;
                        }
                    }
                    break;
            }
        }

        public Matrix(int size)
        {
            Size = size;
            MatrixValues = new double[Size, Size];
        }

        public Matrix(string path)
        {
            if (FilesController.SetMatrixFromFile(matrix: this, path)) return;
            Size = 0;
            MatrixValues = null;
        }

        public double[,] MatrixValues { get; set; }
        public double Determinant { get; internal set; } = double.NaN;
        public int Size { get; set; }
        public double this[int widthIndex, int heightIndex]
        {
            get
            {
                if (ValuesInsideMatrix(widthIndex, heightIndex))
                    return MatrixValues[widthIndex - 1, heightIndex - 1];
                else
                    return double.NaN;
            }
            set
            {
                if (ValuesInsideMatrix(widthIndex, heightIndex))
                    MatrixValues[widthIndex - 1, heightIndex - 1] = value;
            }
        }

        private void MatrixLoop(Action<int, int> method)
        {
            MatrixLoop(method, Size);
        }

        private static void MatrixLoop(Action<int, int> method, int size)
        {
            for (int widthIndex = 1; widthIndex <= size; widthIndex++)
            {
                for (int heightIndex = 1; heightIndex <= size; heightIndex++)
                {
                    method(widthIndex, heightIndex);
                }
            }
        }

        public double Cofactor(int widthIndex, int heightIndex)
        {
            if (Size != MinimalCofactorSize)
            {
                if ((widthIndex + heightIndex) % 2 == 0)
                {
                    return HeightDeterminant(Minor(widthIndex, heightIndex));
                }
                else
                {
                    return -WidthDeterminant(Minor(widthIndex, heightIndex));
                }
            }
            return GetMatrixCofactor(widthIndex, heightIndex);
        }

        private double GetMatrixCofactor(int widthIndex, int heightIndex)
        {
            bool isSquare = widthIndex == heightIndex;
            int width = MinimalCofactorSize, height = MinimalCofactorSize;
            if (widthIndex == MinimalCofactorSize - 1)
            {
                if (!isSquare) height--;
            }
            else
            {
                width--;
                if (isSquare) height--;
            }
            int sign = isSquare ? 1 : -1;
            return sign * this[width, height];
        }

        public Matrix GetAttached()
        {
            var attachedMatrix = new Matrix(Size);
            MatrixLoop(delegate (int widthIndex, int heightIndex)
            {
                attachedMatrix[heightIndex, widthIndex] = Cofactor(widthIndex, heightIndex);
            });
            return attachedMatrix;
        }

        public Matrix FirstInversion(out double error)
        {
            error = 0.0;
            if (NoDeterminantExceptionCheck(ref error))
            {
                return null;
            }
            double reversedDeterminant = 1.0 / Determinant;
            Matrix attachedMatrix = reversedDeterminant * GetAttached();
            error = InversionError(attachedMatrix, this);
            return attachedMatrix;
        }

        public Matrix SecondInversion(out double error)
        {
            error = 0.0;
            if (NoDeterminantExceptionCheck(ref error))
            {
                return null;
            }
            Vector vector, vectorB;
            var inversedMatrix = new Matrix(Size);
            for (s_heightIndex = 1; s_heightIndex <= Size; s_heightIndex++)
            {
                vectorB = new Vector(Size); vectorB[s_heightIndex] = 1.0;
                vector = LinearEquationsSystems.GaussMethod(this, vectorB);
                for (s_widthIndex = 1; s_widthIndex <= Size; s_widthIndex++)
                {
                    inversedMatrix[s_widthIndex, s_heightIndex] = vector[s_widthIndex];
                }
            }
            error = InversionError(inversedMatrix, this);
            return inversedMatrix;
        }

        private bool NoDeterminantExceptionCheck(ref double error)
        {
            if (double.IsNaN(Determinant))
            {
                WidthDeterminant(this);
            }
            if (Determinant == 0)
            {
                error = double.NaN;
                return true;
            }
            return false;
        }

        public static double InversionError(Matrix attachedMatrix, Matrix matrix)
        {
            double error = -10.0;
            Matrix E = attachedMatrix * matrix;
            MatrixLoop(delegate (int widthIndex, int heightIndex)
            {
                double indent = widthIndex == heightIndex ? 1.0 : 0.0,
                        cycleError = Math.Abs(E[widthIndex, heightIndex] - indent);
                error = Math.Max(error, cycleError);
            }
            , size: attachedMatrix.Size);
            return error;
        }

        private bool ValuesInsideMatrix(int firstValue, int secondValue)
        {
            return ValueInsideMatrix(firstValue) && ValueInsideMatrix(secondValue);
        }

        private bool ValueInsideMatrix(int value)
        {
            return value >= 1 && value <= Size;
        }

        public static Matrix operator +(Matrix firstMatrix, Matrix secondMatrix)
        {
            return MathOperation(firstMatrix, secondMatrix, Operator.Plus);
        }

        public static Matrix operator -(Matrix firstMatrix, Matrix secondMatrix)
        {
            return MathOperation(firstMatrix, secondMatrix, Operator.Minus);
        }

        public static Matrix operator *(Matrix firstMatrix, Matrix secondMatrix)
        {
            return MathOperation(firstMatrix, secondMatrix, Operator.Multiply);
        }

        private static Matrix MathOperation(Matrix firstMatrix, Matrix secondMatrix, Operator @operator)
        {
            int size = firstMatrix.Size;
            var operatedMatrix = new Matrix(size);
            MatrixLoop(delegate (int widthIndex, int heightIndex)
            {
                double toAdd = firstMatrix[widthIndex, heightIndex];
                operatedMatrix[widthIndex, heightIndex] = 0.0;
                switch (@operator)
                {
                    case Operator.Plus:
                        toAdd += secondMatrix[widthIndex, heightIndex];
                        break;
                    case Operator.Minus:
                        toAdd -= secondMatrix[widthIndex, heightIndex];
                        break;
                    case Operator.Multiply:
                        for (int index = 1; index <= size; index++)
                        {
                            operatedMatrix[widthIndex, heightIndex] +=
                                firstMatrix[widthIndex, index] * secondMatrix[index, heightIndex];
                        }
                        break;
                }
                if (@operator != Operator.Multiply)
                {
                    operatedMatrix[widthIndex, heightIndex] += toAdd;
                }
            }
            , size: size);
            return operatedMatrix;
        }

        public static Matrix operator *(double numeric, Matrix matrix)
        {
            return MultiplyMatrixByNumeric(matrix, numeric);
        }

        public static Matrix operator *(Matrix matrix, double numeric)
        {
            return MultiplyMatrixByNumeric(matrix, numeric);
        }

        private static Matrix MultiplyMatrixByNumeric(Matrix matrix, double numeric)
        {
            int size = matrix.Size;
            var multipliedMatrix = new Matrix(size);
            MatrixLoop(delegate (int widthIndex, int heightIndex)
            {
                multipliedMatrix[widthIndex, heightIndex] = 0.0;
                multipliedMatrix[widthIndex, heightIndex] += matrix[widthIndex, heightIndex] * numeric;
            }
            , size: size);
            return multipliedMatrix;
        }

        public static double GetDeterminant(Matrix matrix, int dimensionIndex, bool indexForWidth)
        {
            double determinant = SetDeterminantStandartValue(matrix);
            if (double.IsNaN(matrix.Determinant))
            {
                matrix.Determinant = determinant;
                return determinant;
            }
            else
            {
                for (int index = 1; index <= matrix.Size; index++)
                {
                    int widthIndex = indexForWidth ? dimensionIndex : index,
                        heightIndex = indexForWidth ? index : dimensionIndex;
                    determinant += matrix[widthIndex, heightIndex] * matrix.Cofactor(widthIndex, heightIndex);
                }
            }
            return determinant;
        }

        private static double SetDeterminantStandartValue(Matrix matrix)
        {
            double determinant = 0.0;
            if (matrix.Size == MinimalCofactorSize)
            {
                determinant =
                    matrix[MinimalCofactorSize - 1, MinimalCofactorSize - 1] *
                    matrix[MinimalCofactorSize, MinimalCofactorSize] -
                    matrix[MinimalCofactorSize, MinimalCofactorSize - 1] *
                    matrix[MinimalCofactorSize - 1, MinimalCofactorSize];
            }
            return determinant;
        }

        public static double HeightDeterminant(Matrix matrix)
        {
            return DimensionRelatedDeterminant(matrix, heightDimension: true);
        }

        public static double WidthDeterminant(Matrix matrix)
        {
            return DimensionRelatedDeterminant(matrix, heightDimension: false);
        }

        private static double DimensionRelatedDeterminant(Matrix matrix, bool heightDimension)
        {
            double determinant = SetDeterminantStandartValue(matrix);
            if (matrix.Size != MinimalCofactorSize)
            {
                int sign = -1;
                for (int index = 1; index <= matrix.Size; index++)
                {
                    sign = -sign;
                    double toAdd = heightDimension ?
                        matrix[1, index] * WidthDeterminant(matrix.Minor(1, index)) :
                        matrix[index, 1] * HeightDeterminant(matrix.Minor(index, 1));
                    determinant += sign * toAdd;
                }
            }
            if (double.IsNaN(matrix.Determinant))
            {
                matrix.Determinant = determinant;
            }
            return determinant;
        }

        public Matrix Minor(int outsideWidthIndex, int outsideHeightIndex)
        {
            var minor = new Matrix(Size - 1);
            MatrixLoop(delegate (int widthIndex, int heightIndex)
            {
                minor[widthIndex, heightIndex] =
                        this[widthIndex < outsideWidthIndex ? widthIndex : widthIndex + 1,
                            heightIndex < outsideHeightIndex ? heightIndex : heightIndex + 1];
            }, size: Size - 1);
            return minor;
        }

        public void LowerUpperDecomposition(out Matrix lowerMatrix, out Matrix upperMatrix)
        {
            lowerMatrix = new Matrix(Size); 
            upperMatrix = new Matrix(Size);
            for (s_heightIndex = 1; s_heightIndex <= Size; s_heightIndex++)
            {
                upperMatrix[1, s_heightIndex] = this[1, s_heightIndex];
            }
            upperMatrix.Determinant = upperMatrix[1, 1];
            for (s_widthIndex = 1; s_widthIndex <= Size; s_widthIndex++)
            {
                lowerMatrix[s_widthIndex, 1] = this[s_widthIndex, 1] / this[1, 1];
            }
            for (s_widthIndex = MinimalCofactorSize; s_widthIndex <= Size; s_widthIndex++)
            {
                for (s_heightIndex = MinimalCofactorSize; s_heightIndex <= Size; s_heightIndex++)
                {
                    SetMatricesValues(lowerMatrix, upperMatrix);
                }
                lowerMatrix[s_widthIndex, s_widthIndex] = 1.0;
                upperMatrix.Determinant *= upperMatrix[s_widthIndex, s_widthIndex];
            }
            Determinant = upperMatrix.Determinant;
            lowerMatrix.Determinant = 1.0;
        }

        private void SetMatricesValues(Matrix lowerMatrix, Matrix upperMatrix)
        {
            int minIndex = Math.Min(s_widthIndex, s_heightIndex) - 1;
            double currentValue = this[s_widthIndex, s_heightIndex];
            for (int index = 1; index <= minIndex; index++)
            {
                currentValue -= lowerMatrix[s_widthIndex, index] * upperMatrix[index, s_heightIndex];
            }
            if (s_widthIndex > s_heightIndex)
            {
                lowerMatrix[s_widthIndex, s_heightIndex] =
                    currentValue / upperMatrix[s_heightIndex, s_heightIndex];
            }
            else
            {
                upperMatrix[s_widthIndex, s_heightIndex] = currentValue;
            }
        }

        public Matrix Transpose()
        {
            return GetMatrix(transposed: true);
        }

        public Matrix Copy()
        {
            return GetMatrix(transposed: false);
        }

        private Matrix GetMatrix(bool transposed)
        {
            var matrix = new Matrix(Size);
            MatrixLoop(delegate (int widthIndex, int heightIndex)
            {
                matrix[widthIndex, heightIndex] = transposed ?
                    this[heightIndex, widthIndex] : this[widthIndex, heightIndex];
            });
            return matrix;
        }

        public Matrix Rotation(bool toRight)
        {
            var rotatedMatrix = new Matrix(Size);
            MatrixLoop(delegate (int widthIndex, int heightIndex)
            {
                rotatedMatrix[heightIndex, widthIndex] = toRight ?
                    this[widthIndex, Size - heightIndex + 1] :
                    this[Size - widthIndex + 1, heightIndex];
            });
            return rotatedMatrix;
        }

        public string GetTextFormat(bool form, int forwardFormatIndent, int formatIndent, string title)
        {
            return TextFormater.GetMatrixTextFormat(matrix: this, form, forwardFormatIndent, formatIndent, title);
        }
    }
}
