using System;

namespace NumericalMethods
{
    public enum MatrixType { Simple, Identity, Test, Upper, Lower }

    public class Matrix
    {
        private const int MinimalCofactorSize = 2;
        private static int s_widthIndex, s_heightIndex;
        private enum Operator { Plus, Minus, Multiply }
        public double[,] MatrixValues { get; set; }
        public int Size { get; set; }
        public double Determinant { get; internal set; } = double.NaN;
        public double this[int widthIndex, int heightIndex]
        {
            get
            {
                if (ValuesInsideMatrix(widthIndex, heightIndex))
                    return MatrixValues[widthIndex - 1, heightIndex - 1];
                else return double.NaN;
            }
            set
            {
                if (ValuesInsideMatrix(widthIndex, heightIndex))
                    MatrixValues[widthIndex - 1, heightIndex - 1] = value;
            }
        }

        public Matrix(int size, MatrixType type)
        {
            Size = size;
            MatrixValues = new double[Size, Size];
            switch (type)
            {
                case MatrixType.Simple:
                    for (s_widthIndex = 1; s_widthIndex <= Size; s_widthIndex++)
                        for (s_heightIndex = 1; s_heightIndex <= Size; s_heightIndex++)
                            this[s_widthIndex, s_heightIndex] = double.NaN;
                    break;
                case MatrixType.Identity:
                    for (int index = 1; index <= Size; index++)
                        this[index, index] = 1.0;
                    break;
                case MatrixType.Test:
                    for (s_widthIndex = 1; s_widthIndex <= Size; s_widthIndex++)
                        for (s_heightIndex = 1; s_heightIndex <= Size; s_heightIndex++)
                            this[s_widthIndex, s_heightIndex] = 10.0 * s_widthIndex + s_heightIndex;
                    break;
                case MatrixType.Upper:
                    for (s_widthIndex = 1; s_widthIndex <= Size; s_widthIndex++)
                        for (s_heightIndex = s_widthIndex; s_heightIndex <= Size; s_heightIndex++)
                            this[s_widthIndex, s_heightIndex] = s_heightIndex - s_widthIndex + 1.0;
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
            if (!FilesController.SetMatrixFromFile(matrix: this, path))
            {
                MatrixValues = null; 
                Size = 0;
            }
        }

        public double Cofactor(int widthIndex, int heightIndex)
        {
            if (Size == MinimalCofactorSize)
            {
                if (widthIndex == heightIndex)
                    if (widthIndex == MinimalCofactorSize - 1)
                        return this[MinimalCofactorSize, MinimalCofactorSize];
                    else return this[MinimalCofactorSize - 1, MinimalCofactorSize - 1];
                else
                    if (widthIndex == MinimalCofactorSize - 1)
                    return -this[MinimalCofactorSize, 1];
                else return -this[MinimalCofactorSize - 1, MinimalCofactorSize];
            }
            if ((widthIndex + heightIndex) % 2 == 0)
                return HeightDeterminant(Minor(widthIndex, heightIndex));
            else return -WidthDeterminant(Minor(widthIndex, heightIndex));
        }

        public Matrix Attached()
        {
            Matrix attachedMatrix = new Matrix(Size);
            // don't use s_widthIndex, s_heightIndex or loop becomes infinite
            for (int widthIndex = 1; widthIndex <= Size; widthIndex++)
                for (int heightIndex = 1; heightIndex <= Size; heightIndex++)
                    attachedMatrix[heightIndex, widthIndex] = Cofactor(widthIndex, heightIndex);
            return attachedMatrix;
        }

        public Matrix FirstInversion(out double error)
        {
            if (double.IsNaN(Determinant)) WidthDeterminant(this);
            if (Determinant == 0) { error = double.NaN; return null; }
            double reversedDeterminant = 1.0 / Determinant;
            Matrix attachedMatrix = reversedDeterminant * Attached();
            error = InversionError(attachedMatrix, this);
            return attachedMatrix;
        }

        public Matrix SecondInversion(out double error)
        {
            if (double.IsNaN(Determinant)) WidthDeterminant(this);
            if (Determinant == 0) { error = double.NaN; return null; }
            Vector vector, vectorB; Matrix inversedMatrix = new Matrix(Size);
            for (int k = 1; k <= Size; k++)
            {
                vectorB = new Vector(Size); vectorB[k] = 1.0;
                vector = LinearEquationsSystems.GaussMethod(this, vectorB);
                for (int index = 1; index <= Size; index++) 
                    inversedMatrix[index, k] = vector[index];
            }
            error = InversionError(inversedMatrix, this);
            return inversedMatrix;
        }

        public static double InversionError(Matrix attachedMatrix, Matrix matrix)
        {
            int N = attachedMatrix.Size; double error = -10.0; Matrix E = attachedMatrix * matrix;
            for (s_widthIndex = 1; s_widthIndex <= N; s_widthIndex++)
                for (s_heightIndex = 1; s_heightIndex <= N; s_heightIndex++)
                {
                    if (s_widthIndex == s_heightIndex)
                        error = Math.Max(error, Math.Abs(E[s_widthIndex, s_widthIndex] - 1.0));
                    else error = Math.Max(error, Math.Abs(E[s_widthIndex, s_heightIndex]));
                }
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
            int firstMatrixSize = firstMatrix.Size;
            Matrix operatedMatrix = new Matrix(firstMatrixSize);
            for (s_widthIndex = 1; s_widthIndex <= firstMatrixSize; s_widthIndex++)
                for (s_heightIndex = 1; s_heightIndex <= firstMatrixSize; s_heightIndex++)
                {
                    double toAdd = firstMatrix[s_widthIndex, s_heightIndex];
                    operatedMatrix[s_widthIndex, s_heightIndex] = 0.0;
                    switch (@operator)
                    {
                        case Operator.Plus:
                            toAdd += secondMatrix[s_widthIndex, s_heightIndex];
                            break;
                        case Operator.Minus:
                            toAdd -= secondMatrix[s_widthIndex, s_heightIndex];
                            break;
                        case Operator.Multiply:
                            for (int index = 1; index <= firstMatrixSize; index++)
                                operatedMatrix[s_widthIndex, s_heightIndex] +=
                                    firstMatrix[s_widthIndex, index] * secondMatrix[index, s_heightIndex];
                            break;
                    }
                    if (@operator != Operator.Multiply) operatedMatrix[s_widthIndex, s_heightIndex] += toAdd;
                }
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
            int matrixSize = matrix.Size;
            Matrix multipliedMatrix = new Matrix(matrixSize);
            for (s_widthIndex = 1; s_widthIndex <= matrixSize; s_widthIndex++)
                for (s_heightIndex = 1; s_heightIndex <= matrixSize; s_heightIndex++)
                {
                    multipliedMatrix[s_widthIndex, s_heightIndex] = 0.0;
                    multipliedMatrix[s_widthIndex, s_heightIndex] += matrix[s_widthIndex, s_heightIndex] * numeric;
                }
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
                determinant =
                    matrix[MinimalCofactorSize - 1, MinimalCofactorSize - 1] *
                    matrix[MinimalCofactorSize, MinimalCofactorSize] -
                    matrix[MinimalCofactorSize, MinimalCofactorSize - 1] *
                    matrix[MinimalCofactorSize - 1, MinimalCofactorSize];
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
                matrix.Determinant = determinant;
            return determinant;
        }

        public Matrix Minor(int outsideWidthIndex, int outsideHeightIndex)
        {
            Matrix minor = new Matrix(Size - 1);
            for (s_widthIndex = 1; s_widthIndex < Size; ++s_widthIndex)
                for (s_heightIndex = 1; s_heightIndex < Size; ++s_heightIndex)
                    minor[s_widthIndex, s_heightIndex] = 
                        this[s_widthIndex < outsideWidthIndex ? s_widthIndex : s_widthIndex + 1,
                            s_heightIndex < outsideHeightIndex ? s_heightIndex : s_heightIndex + 1];
            return minor;
        }

        public void DecompositionLU(out Matrix lowerMatrix, out Matrix upperMatrix)
        {
            lowerMatrix = new Matrix(Size); upperMatrix = new Matrix(Size);
            for (s_heightIndex = 1; s_heightIndex <= Size; s_heightIndex++)
                upperMatrix[1, s_heightIndex] = this[1, s_heightIndex];
            upperMatrix.Determinant = upperMatrix[1, 1];
            for (s_widthIndex = 1; s_widthIndex <= Size; s_widthIndex++) 
                lowerMatrix[s_widthIndex, 1] = this[s_widthIndex, 1] / this[1, 1];
            int minIndex;
            for (int widthIndex = MinimalCofactorSize; widthIndex <= Size; widthIndex++)
            {
                for (int heightIndex = MinimalCofactorSize; heightIndex <= Size; heightIndex++)
                {
                    minIndex = Math.Min(widthIndex, heightIndex) - 1; 
                    double currentValue = this[widthIndex, heightIndex];
                    for (int k = 1; k <= minIndex; k++) 
                        currentValue -= lowerMatrix[widthIndex, k] * upperMatrix[k, heightIndex];

                    if (widthIndex > heightIndex)
                        lowerMatrix[widthIndex, heightIndex] = currentValue / upperMatrix[heightIndex, heightIndex];
                    else upperMatrix[widthIndex, heightIndex] = currentValue;
                }
                lowerMatrix[widthIndex, widthIndex] = 1.0; 
                upperMatrix.Determinant *= upperMatrix[widthIndex, widthIndex];
            }
            Determinant = upperMatrix.Determinant; 
            lowerMatrix.Determinant = 1.0;
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
            Matrix matrix = new Matrix(Size);
            for (s_widthIndex = 1; s_widthIndex <= Size; s_widthIndex++)
                for (s_heightIndex = 1; s_heightIndex <= Size; s_heightIndex++)
                    matrix[s_widthIndex, s_heightIndex] = transposed ? 
                        this[s_heightIndex, s_widthIndex] : this[s_widthIndex, s_heightIndex];
            return matrix;
        }

        public Matrix Rotation(bool toRight)
        {
            Matrix rotatedMatrix = new Matrix(Size);
             for (s_widthIndex = 1; s_widthIndex <= Size; s_widthIndex++)
                    for (s_heightIndex = 1; s_heightIndex <= Size; s_heightIndex++)
                        rotatedMatrix[s_heightIndex, s_widthIndex] = toRight ? 
                        this[s_widthIndex, Size - s_heightIndex + 1] : 
                        this[Size - s_widthIndex + 1, s_heightIndex];
            return rotatedMatrix;
        }
        
        public string GetTextFormat(bool form, int fs, int fd, string title)
        {
            return FilesController.GetMatrixTextFormat(matrix: this, form, fs, fd, title);
        }
    }
}
