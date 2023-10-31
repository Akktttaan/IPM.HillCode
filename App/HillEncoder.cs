using System;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;

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
        
            for (var i = 0; i < inputText.Length; i += portionSize)
            {
                var portion = inputText.Substring(i, portionSize);
                var arrayOfIndexes = portion.Select(x => (double)Settings.ALPHABET.IndexOf(x)).ToArray();
                var vector = Vector<double>.Build.DenseOfArray(arrayOfIndexes);
                outputText = matrixOfKey
                    .Multiply(vector)
                    .Aggregate(outputText, 
                        (current, elem) => current + Settings.ALPHABET[(int)elem % Settings.ALPHABET_LENGTH]);
            }
        
            return outputText;
        }

        private static string PrepareTextToEncrypting(string original, int length, int startIndex = 0)
        {
            var text = "";
            var symbolsInAlphabet = original.Where(x => Settings.ALPHABET.Contains(x)).ToArray();
            for (var i = startIndex; i < Math.Min(length + startIndex, symbolsInAlphabet.Length); i++)
            {
                text += symbolsInAlphabet[i];
            }
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
        }
    }
}