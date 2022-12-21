using System;

namespace NumericalMethods
{
    public static class LinearEquationsSystems
    {
        private static readonly int MaxNumericPower = 10000;

        public static Vector CramerRule(Matrix matrix, Vector vector, out Vector determinantVector)
        {
            if (double.IsNaN(matrix.Determinant))
            {
                Matrix.HeightDeterminant(matrix);
            }
            if (matrix.Determinant == 0)
            {
                determinantVector = null;
                return null;
            }
            int matrixSize = matrix.Size;
            Matrix matrixCopy = matrix.Copy();
            Vector vectorX = new Vector(matrixSize);
            determinantVector = new Vector(matrixSize);
            int widthIndex;
            for (int matrixIndex = 1; matrixIndex <= matrixSize; matrixIndex++)
            {
                for (widthIndex = 1; widthIndex <= matrixSize; widthIndex++)
                {
                    matrixCopy[widthIndex, matrixIndex] = vector[widthIndex];
                }
                determinantVector[matrixIndex] =
                    Matrix.GetDeterminant(matrixCopy, matrixIndex, indexForWidth: false);
                vectorX[matrixIndex] = determinantVector[matrixIndex] / matrix.Determinant;
                for (widthIndex = 1; widthIndex <= matrixSize; widthIndex++)
                {
                    matrixCopy[widthIndex, matrixIndex] = matrix[widthIndex, matrixIndex];
                }
            }
            return vectorX;
        }

        public static Vector SimpleIterationMethod(Matrix matrix, Vector vector, double accuracy,
            out int numericPower)
        {
            if (double.IsNaN(matrix.Determinant))
            {
                matrix.Determinant = Matrix.HeightDeterminant(matrix);
            }
            if (matrix.Determinant == 0) 
            { 
                numericPower = -1;
                return null; 
            }
            int matrixSize = matrix.Size, widthIndex, heightIndex;
            Matrix alpha = new Matrix(matrixSize);
            Vector vectorX = new Vector(matrixSize);
            for (widthIndex = 1; widthIndex <= matrixSize; widthIndex++)
            {
                for (heightIndex = 1; heightIndex <= matrixSize; heightIndex++)
                {
                    alpha[widthIndex, heightIndex] = -matrix[widthIndex, heightIndex];
                }
                vectorX[widthIndex] = vector[widthIndex];
                alpha[widthIndex, widthIndex] += 1.0;
            }
            numericPower = 0; double error;
            while (true)
            {
                numericPower++;
                Vector vectorX1 = Vector.Multiply(alpha, vectorX) + vector;
                error = 0.0;
                for (int index = 1; index <= matrixSize; index++)
                {
                    error = Math.Max(error, Math.Abs(vectorX1[index] - vectorX[index]));
                    vectorX[index] = vectorX1[index];
                }
                if (numericPower > MaxNumericPower) 
                {
                    return null;
                }
                if (error < accuracy) 
                {
                    return vectorX1;
                }
            }
        }

        public static Vector SeidelMethod(Matrix matrix, Vector vector,
            double accuracy, out int numericPower)
        {
            if (double.IsNaN(matrix.Determinant))
            {
                matrix.Determinant = Matrix.HeightDeterminant(matrix);
            }
            if (matrix.Determinant == 0)
            {
                numericPower = -1;
                return null;
            }
            int matrixSize = matrix.Size, widthIndex, heightIndex;
            Matrix transposedMatrix = matrix.Transpose();
            Matrix symmetricMatrix = transposedMatrix * matrix;
            Vector transposedVector = Vector.Multiply(transposedMatrix, vector);
            Matrix alpha = new Matrix(matrixSize);
            Vector betta = new Vector(matrixSize);
            for (widthIndex = 1; widthIndex <= matrixSize; widthIndex++)
            {
                for (heightIndex = 1; heightIndex <= matrixSize; heightIndex++)
                {
                    if (widthIndex != heightIndex)
                    {
                        alpha[widthIndex, heightIndex] =
                              -symmetricMatrix[widthIndex, heightIndex] / symmetricMatrix[widthIndex, widthIndex];
                    }
                    else alpha[widthIndex, widthIndex] = 0.0;
                }
            }
            Vector vectorX = new Vector(matrixSize);
            for (widthIndex = 1; widthIndex <= matrixSize; widthIndex++)
            {
                betta[widthIndex] = transposedVector[widthIndex] / symmetricMatrix[widthIndex, widthIndex];
                vectorX[widthIndex] = betta[widthIndex];
            }
            Vector vectorX1 = new Vector(matrixSize);
            double sum, error; numericPower = 0;
            while (true)
            {
                for (widthIndex = 1; widthIndex <= matrixSize; widthIndex++)
                {
                    sum = betta[widthIndex];
                    for (heightIndex = 1; heightIndex < widthIndex; heightIndex++)
                        sum += vectorX1[heightIndex] * alpha[widthIndex, heightIndex];
                    for (heightIndex = widthIndex; heightIndex <= matrixSize; heightIndex++)
                        sum += vectorX[heightIndex] * alpha[widthIndex, heightIndex];
                    vectorX1[widthIndex] = sum;
                }
                error = 0.0; numericPower++;
                for (int index = 1; index <= matrixSize; index++)
                {
                    error = Math.Max(error, Math.Abs(vectorX1[index] - vectorX[index]));
                    vectorX[index] = vectorX1[index];
                }
                if (error < accuracy) 
                {
                    return vectorX;
                }
                if (numericPower > MaxNumericPower) 
                {
                    return null;
                }
            }
        }

        public static Vector GaussMethod(Matrix matrix, Vector vector)
        {
            GaussAlgorithm(matrix, vector, out double determinant, out int matrixSize,
                out int sign, out Matrix matrixCopy, out Vector vectorCopy);
            Vector result = new Vector(matrixSize);
            result[matrixSize] = vectorCopy[matrixSize];
            for (int widthIndex = matrixSize - 1; widthIndex >= 1; widthIndex--)
            {
                result[widthIndex] = vectorCopy[widthIndex];
                for (int heightIndex = matrixSize; heightIndex >= (widthIndex + 1); heightIndex--)
                {
                    result[widthIndex] -= result[heightIndex] * matrixCopy[widthIndex, heightIndex];
                }
            }
            if (double.IsNaN(matrix.Determinant))
            {
                matrix.Determinant = determinant * sign;
            }
            return result;
        }

        public static Vector GaussJordanElimination(Matrix matrix, Vector vector)
        {
            GaussAlgorithm(matrix, vector, out double determinant, out int matrixSize,
                out int sign, out Matrix matrixCopy, out Vector vectorCopy);
            for (int heightIndex = matrixSize; heightIndex >= 2; heightIndex--)
            {
                for (int widthIndex = 1; widthIndex <= (heightIndex - 1); widthIndex++)
                {
                    vectorCopy[widthIndex] -= vectorCopy[heightIndex] * matrixCopy[widthIndex, heightIndex];
                }
            }
            if (double.IsNaN(matrix.Determinant))
            {
                matrix.Determinant = determinant * sign;
            }
            return vectorCopy.Copy();
        }

        private static void GaussAlgorithm(Matrix matrix, Vector vector, out double determinant,
            out int matrixSize, out int sign, out Matrix matrixCopy, out Vector vectorCopy)
        {
            int widthIndex, heightIndex;
            double absoluteNumber, matrixMain;
            determinant = 1.0;
            matrixSize = matrix.Size;
            sign = 1;
            matrixCopy = matrix.Copy();
            vectorCopy = vector.Copy();
            for (int index = 1; index <= matrixSize; index++)
            {
                SetHeightIndexCopy(matrixSize, matrixCopy, index, out int heightIndexCopy);
                SetCorrectHeightIndexes(matrixSize, ref sign, matrixCopy, vectorCopy, index, 
                    heightIndexCopy);
                matrixMain = matrixCopy[index, index];
                for (heightIndex = index; heightIndex <= matrixSize; heightIndex++)
                {
                    matrixCopy[index, heightIndex] /= matrixMain;
                }
                vectorCopy[index] /= matrixMain;
                determinant *= matrixMain;
                for (widthIndex = index + 1; widthIndex <= matrixSize; widthIndex++)
                {
                    absoluteNumber = matrixCopy[widthIndex, index];
                    for (heightIndex = index; heightIndex <= matrixSize; heightIndex++)
                    {
                        matrixCopy[widthIndex, heightIndex] -=
                            matrixCopy[index, heightIndex] * absoluteNumber;
                    }
                    vectorCopy[widthIndex] -= vectorCopy[index] * absoluteNumber;
                }
            }
        }

        private static void SetCorrectHeightIndexes(int matrixSize, ref int sign, Matrix matrixCopy, 
            Vector vectorCopy, int index, int heightIndexCopy)
        {
            double absoluteNumber;
            if (heightIndexCopy != index)
            {
                for (int heightIndex = 1; heightIndex <= matrixSize; heightIndex++)
                {
                    absoluteNumber = matrixCopy[heightIndexCopy, heightIndex];
                    matrixCopy[heightIndexCopy, heightIndex] =
                        matrixCopy[index, heightIndex];
                    matrixCopy[index, heightIndex] = absoluteNumber;
                }
                absoluteNumber = vectorCopy[heightIndexCopy];
                vectorCopy[heightIndexCopy] = vectorCopy[index];
                vectorCopy[index] = absoluteNumber; sign = -sign;
            }
        }

        private static void SetHeightIndexCopy(int matrixSize, Matrix matrixCopy, int index, 
            out int heightIndexCopy)
        {
            double absoluteNumber, matrixMain;
            matrixMain = Math.Abs(matrixCopy[index, index]);
            heightIndexCopy = index;
            for (int widthIndex = index + 1; widthIndex <= matrixSize; widthIndex++)
            {
                absoluteNumber = Math.Abs(matrixCopy[widthIndex, index]);
                if (absoluteNumber > matrixMain)
                {
                    matrixMain = absoluteNumber;
                    heightIndexCopy = widthIndex;
                }
            }
        }

        public static double AccuracyError(Matrix matrix, Vector vector, Vector vectorB)
        {
            double error = 0.0;
            Vector matrixByVector = Vector.Multiply(matrix, vector);
            for (int index = 1; index <= matrix.Size; index++)
            {
                error += (vectorB[index] - matrixByVector[index]) * (vectorB[index] - matrixByVector[index]);
            }
            return Math.Sqrt(error) / matrix.Size;
        }
    }
}
