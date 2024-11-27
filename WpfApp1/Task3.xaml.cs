using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Логика взаимодействия для Task3.xaml
    /// </summary>
    public partial class Task3 : Window
    {
        public Task3()
        {
            InitializeComponent();
        }

        private void SortButton_Click(object sender, RoutedEventArgs e)
        {
            string inputText = InputTextBox.Text;
            if (string.IsNullOrWhiteSpace(inputText))
            {
                MessageBox.Show("Please enter some text to sort.");
                return;
            }

            // Разбиваем текст на слова
            List<string> words = TokenizeText(inputText);

            // Замеряем время сортировки с QuickSort
            Stopwatch stopwatch = Stopwatch.StartNew();
            List<string> sortedWordsQuickSort = QuickSort.Sort(words.ToList());
            stopwatch.Stop();
            string quickSortTime = stopwatch.ElapsedMilliseconds.ToString() + " ms";

            // Подсчитываем количество слов для QuickSort
            var wordCountQuickSort = WordCounter.CountOccurrences(sortedWordsQuickSort);

            // Замеряем время сортировки с RadixSort
            stopwatch.Restart();
            List<string> sortedWordsRadixSort = RadixSort.Sort(words.ToList());
            stopwatch.Stop();
            string radixSortTime = stopwatch.ElapsedMilliseconds.ToString() + " ms";

            // Подсчитываем количество слов для RadixSort
            var wordCountRadixSort = WordCounter.CountOccurrences(sortedWordsRadixSort);

            // Подсчитываем общее количество слов и уникальных слов
            int totalWords = wordCountQuickSort.Values.Sum();
            int uniqueWords = wordCountQuickSort.Count;

            // Отображаем результаты для QuickSort
            DisplayResults(wordCountQuickSort, "QuickSort", quickSortTime);

            // Отображаем результаты для RadixSort
            DisplayResults(wordCountRadixSort, "RadixSort", radixSortTime);

            // Сохраняем результаты для эксперимента в таблицу
            SaveResultsToTable(totalWords, uniqueWords, quickSortTime, radixSortTime);
        }

        // Метод для разбивки текста на слова
        private List<string> TokenizeText(string text)
        {
            return Regex.Matches(text.ToLower(), @"\b\w+\b")
                        .Cast<Match>()
                        .Select(m => m.Value)
                        .ToList();
        }

        // Метод для отображения результатов подсчета слов
        private void DisplayResults(Dictionary<string, int> wordCount, string algorithmName, string time)
        {
            // Подсчитываем общее количество слов и уникальных слов
            int totalWordCount = wordCount.Values.Sum();
            int uniqueWordCount = wordCount.Count;

            // Отображаем результаты
            ResultLabel.Content = $"{algorithmName} - Time: {time}\n";
            ResultLabel.Content += $"Total Words: {totalWordCount}\n";
            ResultLabel.Content += $"Unique Words: {uniqueWordCount}\n\n";

            // Отображаем каждое слово и количество его вхождений
            foreach (var entry in wordCount.OrderBy(entry => entry.Key))
            {
                ResultLabel.Content += $"{entry.Key}: {entry.Value}\n";
            }
        }

        // Метод для сохранения данных в таблицу
        private void SaveResultsToTable(int totalWords, int uniqueWords, string quickSortTime, string radixSortTime)
        {
            ResultsDataGrid.Items.Add(new ExperimentResult
            {
                TotalWords = totalWords,
                UniqueWords = uniqueWords,
                QuickSortTime = quickSortTime,
                RadixSortTime = radixSortTime
            });
        }
    }


    // Класс для хранения данных эксперимента
    public class ExperimentResult
    {
        public int TotalWords { get; set; }
        public int UniqueWords { get; set; }
        public string? QuickSortTime { get; set; }
        public string? RadixSortTime { get; set; }
    }

    // Класс для подсчета вхождений слов
    public static class WordCounter
    {
        public static Dictionary<string, int> CountOccurrences(List<string> words)
        {
            Dictionary<string, int> wordCount = new Dictionary<string, int>();

            foreach (string word in words)
            {
                if (wordCount.ContainsKey(word))
                    wordCount[word]++;
                else
                    wordCount[word] = 1;
            }

            return wordCount;
        }
    }

    // Класс для реализации алгоритма QuickSort
    public static class QuickSort
    {
        public static List<string> Sort(List<string> list)
        {
            if (list.Count <= 1)
                return list;

            string pivot = list[list.Count / 2];
            List<string> left = new List<string>();
            List<string> right = new List<string>();
            List<string> middle = new List<string>();

            foreach (string str in list)
            {
                int comparison = str.CompareTo(pivot);
                if (comparison < 0)
                    left.Add(str);
                else if (comparison > 0)
                    right.Add(str);
                else
                    middle.Add(str);
            }

            List<string> sortedList = new List<string>();
            sortedList.AddRange(Sort(left));
            sortedList.AddRange(middle);
            sortedList.AddRange(Sort(right));

            return sortedList;
        }
    }

    // Класс для реализации алгоритма RadixSort
    public static class RadixSort
    {
        public static List<string> Sort(List<string> list)
        {
            int maxLength = GetMaxStringLength(list);
            for (int i = maxLength - 1; i >= 0; i--)
            {
                list = CountSort(list, i);
            }
            return list;
        }

        private static int GetMaxStringLength(List<string> list)
        {
            int maxLength = 0;
            foreach (string str in list)
            {
                if (str.Length > maxLength)
                    maxLength = str.Length;
            }
            return maxLength;
        }

        private static List<string> CountSort(List<string> list, int index)
        {
            List<string> sortedList = new List<string>();
            Dictionary<char, List<string>> buckets = new Dictionary<char, List<string>>();

            foreach (var word in list)
            {
                char currentChar = index < word.Length ? word[word.Length - index - 1] : ' ';
                if (!buckets.ContainsKey(currentChar))
                {
                    buckets[currentChar] = new List<string>();
                }
                buckets[currentChar].Add(word);
            }

            foreach (var key in buckets.Keys)
            {
                sortedList.AddRange(buckets[key]);
            }

            return sortedList;
        }
    }
}
