using MathNet.Numerics.LinearAlgebra;
using System;
using System.Linq;

namespace App
{
    public static class HillEncoder
    {
        public static string Encrypt(string inputText, Matrix<double> matrixOfKey)
        {
            inputText = PrepareTextToEncrypting(inputText, inputText.Length);
            var outputText = "";
            var portionSize = matrixOfKey.RowCount;

            while (inputText.Length % portionSize != 0)
            {
                inputText += 'x';
            }

            for (int i = 0; i < inputText.Length; i += portionSize)
            {
                var portion = inputText.Substring(i, portionSize);
                var arrayOfIndexes = portion.Select(x => (double)Settings.ALPHABET.IndexOf(x)).ToArray();
                Vector<double> vector = Vector<double>.Build.DenseOfArray(arrayOfIndexes);
                foreach (var elem in matrixOfKey.Multiply(vector))
                {
                    outputText += Settings.ALPHABET[((int)elem) % Settings.ALPHABET_LENGTH];
                }
            }

            return outputText;
        }

        private static string PrepareTextToEncrypting(string original, int length, int startIndex = 0)
        {
            var text = "";
            var symbolsInAlphabet = original.Where(x => Settings.ALPHABET.Contains(x)).ToArray();
            for (int i = startIndex; i < Math.Min(length + startIndex, symbolsInAlphabet.Length); i++)
            {
                text += symbolsInAlphabet[i];
            }
            //foreach (var ch in original.Where(x => Settings.ALPHABET.Contains(x)).ToArray())
            //{
            //    text += ch;
            //    if (length != null && text.Length == length) break;
            //}
            return text;
        }

        public static Matrix<double> CalculateMatrixOfKey(string originalText, string encryptedText, int size)
        {
            Matrix<double> matrixX, matrixY;
            string originalTextPortion, encryptedTextPortion;
            var i = 0;
            do
            {
                originalTextPortion = PrepareTextToEncrypting(originalText, size * size, i);
                encryptedTextPortion = PrepareTextToEncrypting(encryptedText, size * size, i);

                matrixX = MatrixHelper.GetMatrixFromString(originalTextPortion);
                matrixY = MatrixHelper.GetMatrixFromString(encryptedTextPortion);

                i++;

            } while (!MatrixHelper.CheckConstraints(matrixX));

            return MatrixHelper.Inverse(matrixX).Multiply(matrixY).Modulus(Settings.ALPHABET_LENGTH);

            //originalText = PrepareTextToEncrypting(originalText, size * size);
            //encryptedText = PrepareTextToEncrypting(encryptedText, size * size);

            //var matrixX = MatrixHelper.GetMatrixFromString(originalText);
            //var matrixY = MatrixHelper.GetMatrixFromString(encryptedText);
            //if (MatrixHelper.CheckConstraints(matrixX))
            //{
            //    return MatrixHelper.Inverse(matrixX).Multiply(matrixY).Modulus(Settings.ALPHABET_LENGTH);
            //}

            //return Matrix<double>.Build.Dense(size, size, 0);
        }
    }
}
