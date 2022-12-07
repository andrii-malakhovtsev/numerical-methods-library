using System;

namespace NumericalMethods
{
    public static class LinearEquationsSystems
    {
        private static readonly int MaxNumericPower = 10000;

        public static Vector CramersRule(Matrix matrix, Vector vector, out Vector determinantVector)
        {
            if (double.IsNaN(matrix.Determinant)) Matrix.HeightDeterminant(matrix);
            if (matrix.Determinant == 0) { determinantVector = null; return null; }
            int matrixSize = matrix.Size;
            Matrix matrixCopy = matrix.Copy();
            Vector vectorX = new Vector(matrixSize);
            determinantVector = new Vector(matrixSize);
            for (int matrixIndex = 1; matrixIndex <= matrixSize; matrixIndex++)
            {
                for (int widthIndex = 1; widthIndex <= matrixSize; widthIndex++) 
                    matrixCopy[widthIndex, matrixIndex] = vector[widthIndex];
                determinantVector[matrixIndex] = 
                    Matrix.GetDeterminant(matrixCopy, matrixIndex, indexForWidth: false);
                vectorX[matrixIndex] = determinantVector[matrixIndex] / matrix.Determinant;
                for (int widthIndex = 1; widthIndex <= matrixSize; widthIndex++) 
                    matrixCopy[widthIndex, matrixIndex] = matrix[widthIndex, matrixIndex];
            }
            return vectorX;
        }

        public static Vector SimpleIterationMethod(Matrix matrix, Vector vector, double accuracy, 
            out int numericPower)
        {
            if (double.IsNaN(matrix.Determinant)) 
                matrix.Determinant = Matrix.HeightDeterminant(matrix);
            if (matrix.Determinant == 0) { numericPower = -1; return null; }
            int matrixSize = matrix.Size, widthIndex, heightIndex;
            Matrix alpha = new Matrix(matrixSize); 
            Vector vectorX = new Vector(matrixSize);
            for (widthIndex = 1; widthIndex <= matrixSize; widthIndex++)
            {
                for (heightIndex = 1; heightIndex <= matrixSize; heightIndex++)
                    alpha[widthIndex, heightIndex] = -matrix[widthIndex, heightIndex];
                vectorX[widthIndex] = vector[widthIndex]; 
                alpha[widthIndex, widthIndex] += 1.0;
            }
            _ = new Vector(matrixSize); 
            numericPower = 0; double error;
            while(true)
            {
                numericPower++; 
                Vector vectorX1 = Vector.Multiply(alpha, vectorX) + vector;
                error = 0.0;
                for(int index = 1; index <= matrixSize; index++)
                {
                    error = Math.Max(error, Math.Abs(vectorX1[index] - vectorX[index])); 
                    vectorX[index] = vectorX1[index];
                }
                if (numericPower > MaxNumericPower) return null; 
                if (error < accuracy) return vectorX1;
            }
        }

        public static Vector SeidelMethod(Matrix matrix, Vector vector, 
            double accuracy, out int numericPower)
        {
            if (double.IsNaN(matrix.Determinant)) matrix.Determinant = Matrix.HeightDeterminant(matrix);
            if (matrix.Determinant == 0) { numericPower = -1; return null; }
            int matrixSize = matrix.Size, widthIndex, heightIndex;
            Matrix transposedMatrix = matrix.Transpose();
            Matrix symmetricMatrix = transposedMatrix * matrix; 
            Vector transposedVector = Vector.Multiply(transposedMatrix, vector);
            Matrix alpha = new Matrix(matrixSize); 
            Vector betta = new Vector(matrixSize);
            for (widthIndex = 1; widthIndex <= matrixSize; widthIndex++)
                for (heightIndex = 1; heightIndex <= matrixSize; heightIndex++)
                    if (widthIndex != heightIndex) alpha[widthIndex, heightIndex] = 
                            -symmetricMatrix[widthIndex, heightIndex] / symmetricMatrix[widthIndex, widthIndex]; 
                    else alpha[widthIndex, widthIndex] = 0.0;
            Vector vectorX = new Vector(matrixSize);
            for (widthIndex = 1; widthIndex<= matrixSize; widthIndex++) 
            { 
                betta[widthIndex] = transposedVector[widthIndex] / symmetricMatrix[widthIndex, widthIndex]; 
                vectorX[widthIndex] = betta[widthIndex]; 
            }
            Vector vectorX1 = new Vector(matrixSize); 
            double sum, error; numericPower = 0;
            while(true)
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
                if (error < accuracy) return vectorX;
                if (numericPower > MaxNumericPower) return null;
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
                    result[widthIndex] -= result[heightIndex] * matrixCopy[widthIndex, heightIndex];
            }
            if (double.IsNaN(matrix.Determinant))
                matrix.Determinant = determinant * sign;
            return result;
        }

        public static Vector GaussJordanElimination(Matrix matrix, Vector vector)
        {
            GaussAlgorithm(matrix, vector, out double determinant, out int matrixSize,
                out int sign, out Matrix matrixCopy, out Vector vectorCopy);
            for (int heightIndex = matrixSize; heightIndex >= 2; heightIndex--)
                for (int widthIndex = 1; widthIndex <= (heightIndex - 1); widthIndex++)
                    vectorCopy[widthIndex] -= vectorCopy[heightIndex] * matrixCopy[widthIndex, heightIndex];
            if (double.IsNaN(matrix.Determinant)) matrix.Determinant = determinant * sign;
            return vectorCopy.Copy();
        }

        private static void GaussAlgorithm(Matrix matrix, Vector vector, out double determinant, 
            out int matrixSize, out int sign, out Matrix matrixCopy, out Vector vectorCopy)
        {
            int widthIndex, heightIndex;
            double absoluteNumber, matrixMain;
            determinant = 1.0;
            int secondHeightIndex, heightIndexCopy;
            matrixSize = matrix.Size;
            sign = 1;
            matrixCopy = matrix.Copy();
            vectorCopy = vector.Copy();
            for (heightIndex = 1; heightIndex <= matrixSize; heightIndex++)
            {
                matrixMain = Math.Abs(matrixCopy[heightIndex, heightIndex]);
                heightIndexCopy = heightIndex;
                for (widthIndex = heightIndex + 1; widthIndex <= matrixSize; widthIndex++)
                {
                    absoluteNumber = Math.Abs(matrixCopy[widthIndex, heightIndex]);
                    if (absoluteNumber > matrixMain)
                    {
                        matrixMain = absoluteNumber;
                        heightIndexCopy = widthIndex;
                    }
                }
                if (heightIndexCopy != heightIndex)
                {
                    for (secondHeightIndex = 1; secondHeightIndex <= matrixSize; secondHeightIndex++)
                    {
                        absoluteNumber = matrixCopy[heightIndexCopy, secondHeightIndex];
                        matrixCopy[heightIndexCopy, secondHeightIndex] =
                            matrixCopy[heightIndex, secondHeightIndex];
                        matrixCopy[heightIndex, secondHeightIndex] = absoluteNumber;
                    }
                    absoluteNumber = vectorCopy[heightIndexCopy];
                    vectorCopy[heightIndexCopy] = vectorCopy[heightIndex];
                    vectorCopy[heightIndex] = absoluteNumber; sign = -sign;
                }
                matrixMain = matrixCopy[heightIndex, heightIndex];
                for (secondHeightIndex = heightIndex; secondHeightIndex <= matrixSize; secondHeightIndex++)
                    matrixCopy[heightIndex, secondHeightIndex] /= matrixMain;
                vectorCopy[heightIndex] /= matrixMain;
                determinant *= matrixMain;
                for (widthIndex = heightIndex + 1; widthIndex <= matrixSize; widthIndex++)
                {
                    absoluteNumber = matrixCopy[widthIndex, heightIndex];
                    for (secondHeightIndex = heightIndex; secondHeightIndex <= matrixSize; secondHeightIndex++)
                        matrixCopy[widthIndex, secondHeightIndex] -=
                            matrixCopy[heightIndex, secondHeightIndex] * absoluteNumber;
                    vectorCopy[widthIndex] -= vectorCopy[heightIndex] * absoluteNumber;
                }
            }
        }

        public static double AccuracyError(Matrix matrix, Vector vector, Vector vectorB)
        {
            double error = 0.0; 
            Vector bc = Vector.Multiply(matrix, vector);
            for (int index = 1; index <= matrix.Size; index++) 
                error += (vectorB[index] - bc[index]) * (vectorB[index] - bc[index]);
            return Math.Sqrt(error) / matrix.Size;
        }
    }
}
