using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для Task1.xaml
    /// </summary>
    public partial class Task1 : Window
    {
        public Task1()
        {
            InitializeComponent();
        }

       
        private void InputArray_GotFocus(object sender, RoutedEventArgs e)
        {
            if (InputArray.Text == "Введите значения через запятую")
            {
                InputArray.Text = "";
                InputArray.Foreground = Brushes.Black;
            }
        }

        
        private void InputArray_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(InputArray.Text))
            {
                InputArray.Text = "Введите значения через запятую";
                InputArray.Foreground = Brushes.Gray;
            }
        }

        private async void SortButton_Click(object sender, RoutedEventArgs e)
        {
            int[] array = ParseInputArray();
            if (array == null) return;

            int delay = ParseDelay();
            if (delay < 0)
            {
                MessageBox.Show("Введите корректное значение задержки в миллисекундах (неотрицательное число).");
                return;
            }

            LogBox.Clear();
            ArrayState.Text = "";

            var selectedMethod = (SortMethodsComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

            switch (selectedMethod)
            {
                case "Bubble Sort":
                    await BubbleSort(array, delay);
                    break;
                case "Selection Sort":
                    await SelectSort(array, delay);
                    break;
                case "Heap Sort":
                    await HeapSort(array, delay);
                    break;
                case "Quick Sort":
                    await QuickSort(array, 0, array.Length - 1, delay);
                    break;
                default:
                    MessageBox.Show("Выберите метод сортировки.");
                    break;
            }
        }

        private int[] ParseInputArray()
        {
            try
            {
                return InputArray.Text.Split(',')
                    .Select(int.Parse)
                    .ToArray();
            }
            catch
            {
                MessageBox.Show("Введите массив чисел, разделенных запятыми.");
                return null;
            }
        }

        private int ParseDelay()
        {
            if (int.TryParse(DelayInput.Text, out int delay) && delay >= 0)
                return delay;

            return -1;
        }

        private void UpdateArrayState(int[] array)
        {
            ArrayState.Text = string.Join(", ", array);
        }

        private void Log(string message)
        {
            Dispatcher.Invoke(() =>
            {
                LogBox.AppendText(message + Environment.NewLine);
                LogBox.ScrollToEnd();
            });
        }

        private async Task BubbleSort(int[] array, int delay)
        {
            int n = array.Length;
            for (int i = 0; i < n - 1; i++)
            {
                Log($"Проход {i + 1}: начинаем сравнивать пары чисел.");
                bool swapped = false;

                for (int j = 0; j < n - i - 1; j++)
                {
                    Log($"Сравниваем {array[j]} и {array[j + 1]}.");

                    if (array[j] > array[j + 1]) 
                    {
                        Log($"Меняем местами {array[j]} и {array[j + 1]}.");
                        (array[j], array[j + 1]) = (array[j + 1], array[j]);
                        swapped = true;
                        UpdateArrayState(array);
                        await Task.Delay(delay);
                    }
                    else
                    {
                        Log($"{array[j]} меньше {array[j + 1]}. Оставляем как есть.");
                    }
                }

                if (!swapped)
                {
                    Log("Больше ничего менять не нужно. Массив уже отсортирован!");
                    break;
                }
            }

            Log("Массив полностью отсортирован!");
        }

        private async Task SelectSort(int[] array, int delay)
        {
            int n = array.Length;
            for (int i = 0; i < n - 1; i++)
            {
                int minIndex = i;
                Log($"Ищем самое маленькое число от {i + 1} до конца массива.");

                for (int j = i + 1; j < n; j++)
                {
                    Log($"Сравниваем {array[j]} и {array[minIndex]}.");
                    if (array[j] < array[minIndex])
                    {
                        Log($"Нашли меньшее число: {array[j]}.");
                        minIndex = j;
                    }
                }

                if (minIndex != i)
                {
                    Log($"Меняем местами {array[i]} и {array[minIndex]}.");
                    (array[i], array[minIndex]) = (array[minIndex], array[i]);
                    UpdateArrayState(array);
                    await Task.Delay(delay);
                }
            }
            Log("Массив полностью отсортирован!");
        }

        private async Task HeapSort(int[] array, int delay)
        {
            int n = array.Length;

            Log("Создаем кучу из массива...");
            for (int i = n / 2 - 1; i >= 0; i--)
                await Heapify(array, n, i, delay);

            Log("Начинаем сортировку кучи...");
            for (int i = n - 1; i >= 0; i--)
            {
                Log($"Меняем корень ({array[0]}) с последним элементом ({array[i]}).");
                (array[0], array[i]) = (array[i], array[0]);
                UpdateArrayState(array);
                await Task.Delay(delay);

                await Heapify(array, i, 0, delay);
            }
            Log("Массив полностью отсортирован!");
        }

        private async Task Heapify(int[] array, int n, int i, int delay)
        {
            int largest = i;
            int left = 2 * i + 1;
            int right = 2 * i + 2;

            if (left < n && array[left] > array[largest])
                largest = left;

            if (right < n && array[right] > array[largest])
                largest = right;

            if (largest != i)
            {
                Log($"Меняем {array[i]} с {array[largest]}, чтобы восстановить кучу.");
                (array[i], array[largest]) = (array[largest], array[i]);
                UpdateArrayState(array);
                await Task.Delay(delay);

                await Heapify(array, n, largest, delay);
            }
        }

        private async Task QuickSort(int[] array, int low, int high, int delay)
        {
            if (low < high)
            {
                int pi = await Partition(array, low, high, delay);
                Log($"Число {array[pi]} заняло свое место в массиве.");

                await QuickSort(array, low, pi - 1, delay);
                await QuickSort(array, pi + 1, high, delay);
            }
        }

        private async Task<int> Partition(int[] array, int low, int high, int delay)
        {
            int pivot = array[high];
            Log($"Опорное число: {pivot}. Разделим массив на две части.");
            int i = low - 1;

            for (int j = low; j < high; j++)
            {
                Log($"Сравниваем {array[j]} с опорным числом {pivot}.");
                if (array[j] <= pivot)
                {
                    i++;
                    Log($"Ставим {array[j]} в левую часть.");
                    (array[i], array[j]) = (array[j], array[i]);
                    UpdateArrayState(array);
                    await Task.Delay(delay);
                }
            }
            Log($"Ставим опорное число {pivot} на место.");
            (array[i + 1], array[high]) = (array[high], array[i + 1]);
            UpdateArrayState(array);
            await Task.Delay(delay);

            return i + 1;
        }
    }
}

