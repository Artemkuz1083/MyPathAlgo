using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sort
{
    public class NaturalOuterSort
    {
        private string? _headers;
        private readonly int _chosenField;
        //В этом списке будут храниться длины всех серий в обоих подфайлах
        private readonly List<int> _series = new();

        public NaturalOuterSort(int chosenField)
        {
            _chosenField = chosenField;
        }

        private void SplitToFiles()
        {
            using var fileA = new StreamReader("C:\\Users\\artem\\source\\repos\\MyPathAlgo\\Tabels\\FileA.csv");
            _headers = fileA.ReadLine();

            using var fileB = new StreamWriter("C:\\Users\\artem\\source\\repos\\MyPathAlgo\\Tabels\\FileB.csv");
            using var fileC = new StreamWriter("C:\\Users\\artem\\source\\repos\\MyPathAlgo\\Tabels\\FileC.csv");
            //считываем строку, которую будем записывать в подфайл, и следующую за ней, чтобы 
            //сравнить их и проверить, закончилась серия или нет
            string? firstStr = fileA.ReadLine();
            string? secondStr = fileA.ReadLine();
            bool flag = true;
            int counter = 0;
            while (firstStr is not null)
            {
                //Доп. флаг нужен, чтобы при окончании серии не потерять последнюю запись в этой
                //самой серии
                bool tempFlag = flag;
                if (secondStr is not null)
                {
                    if (CompareElements(firstStr, secondStr))
                    {
                        counter++;
                    }
                    else
                    {
                        //Если серия прервалась, то записываем её длину в список и обнуляем счётчик
                        tempFlag = !tempFlag;
                        _series.Add(counter + 1);
                        counter = 0;
                    }
                }

                if (flag)
                {
                    fileB.WriteLine(firstStr);
                }
                else
                {
                    fileC.WriteLine(firstStr);
                }

                //движемся к следующей записи
                firstStr = secondStr;
                secondStr = fileA.ReadLine();
                flag = tempFlag;
            }

            _series.Add(counter + 1);
        }

        private void MergePairs()
        {
            using var writerA = new StreamWriter("C:\\Users\\artem\\source\\repos\\MyPathAlgo\\Tabels\\FileA.csv");
            using var readerB = new StreamReader("C:\\Users\\artem\\source\\repos\\MyPathAlgo\\Tabels\\FileB.csv");
            using var readerC = new StreamReader("C:\\Users\\artem\\source\\repos\\MyPathAlgo\\Tabels\\FileC.csv");

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
                    writerA.WriteLine(elementC);
                    elementC = readerC.ReadLine();
                    counterC++;
                    continue;
                }

                if (indexC == _series.Count || counterC == _series[indexC])
                {
                    //Случай, когда мы дошли до конца серии в подфайле C
                    writerA.WriteLine(elementB);
                    elementB = readerB.ReadLine();
                    counterB++;
                    continue;
                }

                //Сравниваем записи по заданному полю и вписывам в исходный файл меньшую из них
                if (CompareElements(elementB, elementC))
                {
                    writerA.WriteLine(elementB);
                    elementB = readerB.ReadLine();
                    counterB++;
                }
                else
                {
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
