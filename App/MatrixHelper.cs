using MathNet.Numerics;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using System;

namespace App
{
    public static class MatrixHelper
    {
        public static Matrix<double> Inverse(Matrix<double> matrix)
        {
            var outputMatrix = DenseMatrix.Build.DenseDiagonal(matrix.RowCount, matrix.ColumnCount, 0);
            var matrixDet = (int)Math.Round(matrix.Determinant());

            while (matrixDet < 0)
            {
                matrixDet += Settings.ALPHABET_LENGTH;
            }

            for (var i = 0; i < matrix.RowCount; i++)
            {
                for (var j = 0; j < matrix.ColumnCount; j++)
                {
                    var tempMatrix = matrix.RemoveRow(i).RemoveColumn(j);
                    var insertedValue = Math.Pow(-1, i + j) * tempMatrix.Determinant();

                    while (insertedValue < 0)
                    {
                        insertedValue += Settings.ALPHABET_LENGTH;
                    }

                    while (insertedValue % matrixDet != 0)
                    {
                        insertedValue += Settings.ALPHABET_LENGTH;
                    }
                    outputMatrix.At(j, i, Math.Round(insertedValue / matrixDet) % Settings.ALPHABET_LENGTH);
                }
            }
            return (Matrix)outputMatrix;
        }

        public static bool CheckConstraints(Matrix<double> matrix)
        {
            var determinant = (long)Math.Round(matrix.Determinant(), 0);

            var firstCondition = determinant != 0;
            var secondCondition = Euclid.GreatestCommonDivisor(determinant, Settings.ALPHABET_LENGTH) == 1;

            return firstCondition && secondCondition;
        }

        public static Matrix<double> GetRandomMatrix(int size)
        {
            if (size < 0 || size > 5) return null;

            var outputMatrix = DenseMatrix.CreateRandom(size, size, new Chi(100));

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    outputMatrix.At(i, j, Math.Round(outputMatrix.At(i, j), 0));
                }
            }

            return outputMatrix;
        }

        public static Matrix<double> GetMatrixFromString(string text)
        {
            var size = int.Parse(Math.Sqrt(text.Length).ToString());
            var matrix = DenseMatrix.Build.DenseDiagonal(size, size, 0);

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    matrix.At(i, j, Settings.ALPHABET.IndexOf(text[i * size + j]));
                }
            }

            return matrix;
        }

        public static bool IsKeyMatrixFullyFilled(Matrix<double> matrix)
        {
            if (matrix is null) return false;
            for (int i = 0; i < matrix.RowCount; i++)
            {
                for (int j = 0; j < matrix.ColumnCount; j++)
                {
                    if (matrix[i, j] == 0.0) // Проверяем, что элемент не равен 0 (или другому значению по умолчанию)
                    {
                        return false; // Если хотя бы один элемент равен 0, матрица не полностью заполнена
                    }
                }
            }

            return true; // Все элементы матрицы заполнены
        }
    }
}
