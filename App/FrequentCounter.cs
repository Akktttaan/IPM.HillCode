using System.Collections.Generic;
using System.Linq;
using System;

namespace App
{
    public static class FrequentCounter
    {
        public static Dictionary<char, double> countAppearencesOfLetter(string text)
        {
            var freqDict = new Dictionary<char, double>(Settings.ALPHABET.Length);
            var lowerText = text.ToLower();

            foreach (var ch in Settings.ALPHABET)
            {
                freqDict[ch] = 0;
            }

            foreach (var letter in lowerText)
            {
                if (Settings.ALPHABET.Contains(letter.ToString()))
                {
                    var counter = lowerText.Count(ch => ch == letter);
                    freqDict[letter] = Math.Round(((double)counter / lowerText.Length) * 100, 2);
                }
            }

            return freqDict;
        }
    }
}

