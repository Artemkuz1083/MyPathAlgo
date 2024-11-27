using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Sort
{
    public class NaturalOuterSort
    {
        private string? _headers;
        private readonly int _chosenField;
        //В этом списке будут храниться длины всех серий в обоих подфайлах
        private readonly List<int> _series = new();

        private readonly string _pathFile;

        public readonly List<string> _result = new();

        public NaturalOuterSort(int chosenField, string pathFile)
        {
            _chosenField = chosenField;
            _pathFile = pathFile;
        }

        private void SplitToFiles()
        {
            using var fileA = new StreamReader(_pathFile);

            _headers = fileA.ReadLine();

            using var fileB = new StreamWriter("Tabels\\FileB.csv");
            using var fileC = new StreamWriter("Tabels\\FileC.csv");

            string? firstStr = fileA.ReadLine();
            string? secondStr = fileA.ReadLine();
            bool flag = true;
            int counter = 0;
            while (firstStr is not null)
            {

                bool tempFlag = flag;
                if (secondStr is not null)
                {
                    if (CompareElements(firstStr, secondStr))
                    {
                        counter++;
                    }
                    else
                    {
                        tempFlag = !tempFlag;
                        _series.Add(counter + 1);
                        counter = 0;
                    }
                }

                if (flag)
                {
                    _result.Add($"Добавляем {firstStr} в файл B");
                    fileB.WriteLine(firstStr);
                }
                else
                {

                    _result.Add($"Добавляем {firstStr} в файл C");
                    fileC.WriteLine(firstStr);
                }

                firstStr = secondStr;
                secondStr = fileA.ReadLine();
                flag = tempFlag;
            }

            _series.Add(counter + 1);
        }

        private void MergePairs()
        {
            using var writerA = new StreamWriter(_pathFile);
            using var readerB = new StreamReader("Tabels\\FileB.csv");
            using var readerC = new StreamReader("Tabels\\FileC.csv");

            //Не забываем про заголовки
            writerA.WriteLine(_headers);
            //Индекс, по которому находится очередная серия в подфайле B
            int indexB = 0;
            //Индекс, по которому находится очередная серия в подфайле С
            int indexC = 1;
            //Счётчики, чтобы случайно не выйти за пределы серии
            int counterB = 0;
            int counterC = 0;
            string? elementB = readerB.ReadLine();
            string? elementC = readerC.ReadLine();
            //Цикл закончит выполнение только когда 
            while (elementB is not null || elementC is not null)
            {
                if (counterB == _series[indexB] && counterC == _series[indexC])
                {
                    //Случай, когда мы дошли до конца серий в обоих подфайлах
                    counterB = 0;
                    counterC = 0;
                    indexB += 2;
                    indexC += 2;
                    continue;
                }

                if (indexB == _series.Count || counterB == _series[indexB])
                {
                    //Случай, когда мы дошли до конца серии в подфайле B
                    _result.Add($"Записываем {elementC} в A, т.к. закончилась серия в подфайле B");
                    writerA.WriteLine(elementC);
                    elementC = readerC.ReadLine();
                    counterC++;
                    continue;
                }

                if (indexC == _series.Count || counterC == _series[indexC])
                {
                    //Случай, когда мы дошли до конца серии в подфайле C
                    _result.Add($"Записываем {elementB} в A, т.к. закончилась серия в подфайле C");
                    writerA.WriteLine(elementB);
                    elementB = readerB.ReadLine();
                    counterB++;
                    continue;
                }

                //Сравниваем записи по заданному полю и вписывам в исходный файл меньшую из них
                if (CompareElements(elementB, elementC))
                {
                    _result.Add($"Элемент {elementB.Split(';')[_chosenField]} из файла B меньше чем {elementC.Split(';')[_chosenField]} из файла C");
                    writerA.WriteLine(elementB);
                    elementB = readerB.ReadLine();
                    counterB++;
                }
                else
                {
                    _result.Add($"Элемент {elementC.Split(';')[_chosenField]} из файла C меньше чем {elementB.Split(';')[_chosenField]} из файла B");
                    writerA.WriteLine(elementC);
                    elementC = readerC.ReadLine();
                    counterC++;
                }
            }
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
                _series.Clear();
                SplitToFiles();

                if (_series.Count == 1)
                {
                    break;
                }

                MergePairs();
            }
        }
    }
}
