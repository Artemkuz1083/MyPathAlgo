using System.Reflection.Emit;
using System.Reflection.PortableExecutable;

namespace Sort
{
    public class DirectOuterSort
    {
        //Сюда будет сохраняться строка с заголовками таблицы
        private string? _header;
        //поле _iterations указывает, сколько элементов в одной отсортированной цепочке 
        //на данный момент.
        //поле _segments показывает, сколько отсортированных цепочек у нас есть в обоих
        //файлах в сумме
        private long _iterations, _segments;

        public readonly List<String> _result;

        //Индекс выбранного столбца, по которому будем сортировать
        private readonly int _chosenField;

        private readonly string _pathFile;

        public DirectOuterSort(int choosenField, string pathFile)
        {
            _chosenField = choosenField;
            _iterations = 1;
            _pathFile = pathFile;
            _result = new List<String>();

        }


        private void SplitToFiles()
        {
            _segments = 1;
            using var fileA = new StreamReader(_pathFile);

            _header = fileA.ReadLine()!;

            using var fileB = new StreamWriter("Tabels\\FileB.csv");
            using var fileC = new StreamWriter("Tabels\\FileC.csv");

            string? currentRecord = fileA.ReadLine();

            bool flag = true;
            int counter = 0;

            _result.Add($"Делим массив данных по {_iterations}");

            while (currentRecord is not null)
            {
           
                if (counter == _iterations)
                {
                    counter = 0;
                    flag = !flag;
                    _segments++;
                }

                if (flag)
                {
                    _result.Add($"Добавляем в файл B строку: {currentRecord}");
                    fileB.WriteLine(currentRecord);
                }
                else
                {                 
                    _result.Add($"Добавляем в файл C строку: {currentRecord}");
                    fileC.WriteLine(currentRecord);
                }

                currentRecord = fileA.ReadLine();
                counter++;
            }
        }

        private void MergePairs()
        {
            _result.Add($"По элементно будем сравнивать по {_chosenField} столбу элементы из файла B и C и будем записывать их в файл A");

            using var writerA = new StreamWriter(_pathFile);
            using var readerB = new StreamReader("Tabels\\FileB.csv");
            using var readerC = new StreamReader("Tabels\\FileC.csv");


            _result.Add($"добавляем в файл заголовок {_header}");
            writerA.WriteLine(_header);

            string? strB = readerB.ReadLine();
            string? strC = readerC.ReadLine();

            int counterB = 0;
            int counterC = 0;
            
            while (strB is not null || strC is not null)
            {
                string? currentRecord;
                bool flag = false;

                if (strB is null || counterB == _iterations)
                {
                    _result.Add("Так как файл B пустой будем работать с файлом C");
                    currentRecord = strC;
                }
                else if (strC is null || counterC == _iterations)
                {
                    _result.Add("Так как файл C пустой будем работать с файлом B");
                    currentRecord = strB;
                    flag = true;
                }
                else
                {
                    if (CompareElements(strB, strC))
                    {
                        _result.Add($"Элемент {strB.Split(';')[_chosenField]} из файла B меньше чем {strC.Split(';')[_chosenField]} из файла C");
                        currentRecord = strB;
                        flag = true;
                    }
                    else
                    {
                        _result.Add($"Элемент {strC.Split(';')[_chosenField]} из файла C меньше чем {strB.Split(';')[_chosenField]} из файла B");
                        currentRecord = strC;
                    }
                }

                writerA.WriteLine(currentRecord);

                if (flag)
                {
                    _result.Add($"Записываем {strB} в файл A");
                    strB = readerB.ReadLine();
                    counterB++;
                }
                else
                {
                    _result.Add($"Записываем {strC} в файл A");
                    strC = readerC.ReadLine();
                    counterC++;
                }

                if (counterB != _iterations || counterC != _iterations)
                {
                    continue;
                }

                counterC = 0;
                counterB = 0;
            }

            _iterations *= 2;
        }

        private bool CompareElements(string? element1, string? element2)
        {
            string el1 = element1!.Split(';')[_chosenField];
            string el2 = element2!.Split(";")[_chosenField];
            if (int.TryParse(el1, out int el1Int) && int.TryParse(el2, out int el2Int))
            {
                return el1Int.CompareTo(el2Int) < 0;
            }

            return string.Compare(el1, el2, StringComparison.Ordinal) < 0;
        }

        public void Sort()
        {
            while (true)
            {
                SplitToFiles();
                if (_segments == 1)
                {
                    break;
                }

                MergePairs();
            }
        }
    }
}
