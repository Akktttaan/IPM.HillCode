using Microsoft.Win32;
using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using App;
using System.Linq;

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Matrix<double> _keyMatrix;
        private Matrix<double> _inverseKeyMatrix;
        private int _matrixSize;

        public MainWindow()
        {
            InitializeComponent();
            InitializeMatrixDimension();
        }

        private void InitializeMatrixDimension()
        {
            MatrixDimension.ItemsSource = new[] { 2, 3, 4, 5, 6 };
        }

        private void MatrixDimensionChanged(object sender, SelectionChangedEventArgs e)
        {
            _matrixSize = int.Parse(MatrixDimension.SelectedItem.ToString());
            EnableMatrixInputs(_matrixSize);
            AutoGenerationBtn.IsEnabled = true;
            ValidateEncryptButtons();
        }


        private void GuessMatrixOfKey_Click(object sender, RoutedEventArgs e)
        {
            var matrixOfKey = HillEncoder.CalculateMatrixOfKey(InitialText.Text, ConvertedText.Text, _matrixSize);
            MessageBox.Show(matrixOfKey.Transpose().ToMatrixString(_matrixSize, _matrixSize));
        }


        private void LoadTextFromFile(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() != true) return;
            try
            {
                var filePath = openFileDialog.FileName;

                var encoding = DetectFileEncoding(filePath);

                var fileContent = File.ReadAllText(filePath, encoding);

                InitialText.Text = fileContent;
                ConvertedText.Text = string.Empty;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка при загрузке файла: {ex.Message}");
            }
        }

        private void SaveTextInFile(object sender, RoutedEventArgs e)
        {
            try
            {
                // Получаем текст из TextBox
                string textToSave = ConvertedText.Text;

                // Открываем диалоговое окно для выбора места сохранения файла
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*";

                if (saveFileDialog.ShowDialog() == true)
                {
                    // Получаем путь к выбранному файлу
                    string filePath = saveFileDialog.FileName;

                    // Сохраняем текст в файл
                    File.WriteAllText(filePath, textToSave);

                    MessageBox.Show("Текст успешно сохранен в файл.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка при сохранении файла: {ex.Message}");
            }
        }


        private void Encrypt(object sender, RoutedEventArgs e)
        {
            if (_keyMatrix != null)
            {
                ConvertedText.Text = MergeEncryptAndOriginalStrings(HillEncoder.Encrypt(InitialText.Text, _keyMatrix), InitialText.Text);
            }
        }

        private void Decrypt(object sender, RoutedEventArgs e)
        {
            if (_inverseKeyMatrix != null)
            {
                ConvertedText.Text = MergeEncryptAndOriginalStrings(HillEncoder.Encrypt(InitialText.Text, _inverseKeyMatrix), InitialText.Text);
            }
        }

        private string MergeEncryptAndOriginalStrings(string encryptString, string originalString)
        {
            var mergedString = "";
            foreach (var ch in originalString)
            {
                if (Settings.ALPHABET.Contains(ch))
                {
                    mergedString += encryptString[0];
                    encryptString = encryptString.Remove(0, 1);
                }
                else
                {
                    mergedString += ch;
                }
            }

            return mergedString;
        }

        private void ShowFrequencyDictionary(object sender, RoutedEventArgs e)
        {
            var frequentDict = new FrequentDict(FrequentCounter.countAppearencesOfLetter(ConvertedText.Text));
            frequentDict.Show();
        }

        private void InitialTextChanged(object sender, TextChangedEventArgs e)
        {
            ValidateEncryptButtons();
        }

        private Encoding DetectFileEncoding(string filePath)
        {
            // Определите кодировку файла на основе его содержимого
            // Можно использовать различные методы, например, на основе магических байтов файла

            var buffer = new byte[4];
            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                fileStream.Read(buffer, 0, 4);
            }

            return buffer[0] switch
            {
                0xef when buffer[1] == 0xbb && buffer[2] == 0xbf => Encoding.UTF8,
                0xff when buffer[1] == 0xfe => Encoding.Unicode,
                0xfe when buffer[1] == 0xff => Encoding.BigEndianUnicode,
                _ => Encoding.Default
            };
        }

        private void ValidateEncryptButtons()
        {
            var textLengthState = InitialText.Text.Length > 0;
            var matrixDimensionState = !string.IsNullOrEmpty(MatrixDimension.SelectedItem?.ToString());
            var keyState = MatrixHelper.IsKeyMatrixFullyFilled(_keyMatrix);
            EncryptButton.IsEnabled = textLengthState && keyState && matrixDimensionState;
            DecryptButton.IsEnabled = textLengthState && keyState && matrixDimensionState;
        }

        private void CopyInitialText_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var text = InitialText.Text;
                Clipboard.SetDataObject(text);
                MessageBox.Show("Текст успешно скопирован в буфер обмена.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка при копировании текста: {ex.Message}");
            }
        }

        private void CopyConvertedText_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var text = ConvertedText.Text;
                Clipboard.SetDataObject(text);
                MessageBox.Show("Текст успешно скопирован в буфер обмена.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка при копировании текста: {ex.Message}");
            }
        }


        private void ShowGraphics_Click(object sender, RoutedEventArgs e)
        {
            var dict = FrequentCounter.countAppearencesOfLetter(ConvertedText.Text);
            new Graphics(dict.OrderByDescending(x => x.Value).Select(x => x.Value).ToArray()).Show();
        }

        private void EnableMatrixInputs(int size)
        {
            DisableMatrixInputs();

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    string matrixName = "matrix" + i + j;
                    string inverseName = "inverse" + i + j;

                    // Найдем элементы в XAML с использованием привязки данных
                    var matrixControl = FindName(matrixName) as UIElement;
                    var inverseControl = FindName(inverseName) as UIElement;

                    // Включим элементы, если они были найдены
                    if (matrixControl != null)
                    {
                        matrixControl.IsEnabled = true;
                    }

                    if (inverseControl != null)
                    {
                        inverseControl.IsEnabled = true;
                    }
                }
            }
        }

        private void DisableMatrixInputs()
        {
            int size = int.Parse(MatrixDimension.SelectedItem.ToString());

            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    string matrixName = "matrix" + i + j;
                    string inverseName = "inverse" + i + j;

                    // Найдем элементы в XAML с использованием FindName
                    var matrixControl = FindName(matrixName) as TextBox;
                    var inverseControl = FindName(inverseName) as TextBox;

                    // Отключим элементы и очистим текст, если i или j больше или равно выбранному размеру
                    if (matrixControl != null)
                    {
                        matrixControl.IsEnabled = false;
                        if (i >= size || j >= size)
                        {
                            matrixControl.Text = "";
                        }
                    }

                    if (inverseControl != null)
                    {
                        inverseControl.IsEnabled = false;
                        if (i >= size || j >= size)
                        {
                            inverseControl.Text = "";
                        }
                    }
                }
            }
        }

        private void FindInverseMatrix_Click(object sender, EventArgs e)
        {
            try
            {
                FillMatrixFromInterface();
            }
            catch (FormatException)
            {
                MessageBox.Show("Заполните матрицу ключа!");
                return;
            }

            if (MatrixHelper.CheckConstraints(_keyMatrix))
            {
                _inverseKeyMatrix = MatrixHelper.Inverse(_keyMatrix);
                FillInputsForMatrix("inverse");
            }
            else
            {
                MessageBox.Show("Матрица не удовлетворяет ограничениям!");
            }

        }

        private void FillInputsForMatrix(string matrixName)
        {
            Matrix<double> tempMatrix;

            if (matrixName == "matrix")
            {
                tempMatrix = _keyMatrix;
            }
            else if (matrixName == "inverse")
            {
                tempMatrix = _inverseKeyMatrix;
            }
            else
            {
                return;
            }

            for (int i = 0; i < _matrixSize; i++)
            {
                for (int j = 0; j < _matrixSize; j++)
                {
                    string controlName = matrixName + i + j;

                    // Найдем элемент TextBox с соответствующим именем
                    var control = FindName(controlName) as TextBox;

                    if (control != null)
                    {
                        var text = tempMatrix.At(i, j).ToString();
                        if (text.Contains("-")) text = text.Replace("-", "");
                        control.Text = text;
                    }
                }
            }
        }


        private void FillMatrixFromInterface()
        {
            _keyMatrix = DenseMatrix.Build.DenseDiagonal(_matrixSize, _matrixSize, 0);

            for (int i = 0; i < _matrixSize; i++)
            {
                for (int j = 0; j < _matrixSize; j++)
                {
                    string matrixName = "matrix" + i + j;

                    // Найдем элемент TextBox с соответствующим именем
                    var matrixControl = FindName(matrixName) as TextBox;

                    if (matrixControl != null)
                    {
                        double value;
                        if (double.TryParse(matrixControl.Text, out value))
                        {
                            _keyMatrix[i, j] = value;
                        }
                        else
                        {
                            // Обработка ошибки, если в TextBox был введен неверный формат
                        }
                    }
                }
            }
        }

        private void AutoGenerationBtn_Click(object sender, RoutedEventArgs e)
        {
            do
            {
                _keyMatrix = MatrixHelper.GetRandomMatrix(_matrixSize);

            } while (!MatrixHelper.CheckConstraints(_keyMatrix));

            FillInputsForMatrix("matrix");
            ValidateEncryptButtons();
        }
    }
}
