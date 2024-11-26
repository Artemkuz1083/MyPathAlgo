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

        //Индекс выбранного столбца, по которому будем сортировать
        private readonly int _chosenField;

        public DirectOuterSort(int choosenField)
        {
            _chosenField = choosenField;
            _iterations = 1;
        }


        private void SplitToFiles()
        {
            _segments = 1;
            using var fileA = new StreamReader("C:\\Users\\artem\\source\\repos\\MyPathAlgo\\Tabels\\FileA.csv");
            _header = fileA.ReadLine()!;

            using var fileB = new StreamWriter("C:\\Users\\artem\\source\\repos\\MyPathAlgo\\Tabels\\FileB.csv");
            using var fileC = new StreamWriter("C:\\Users\\artem\\source\\repos\\MyPathAlgo\\Tabels\\FileC.csv");

            string? currentRecord = fileA.ReadLine();

            bool flag = true;
            int counter = 0;
           
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
                    fileB.WriteLine(currentRecord);
                }
                else
                {                 
                    fileC.WriteLine(currentRecord);
                }

                currentRecord = fileA.ReadLine();
                counter++;
            }
        }

        private void MergePairs()
        {
            using var writerA = new StreamWriter(@"C:\Users\artem\source\repos\MyPathAlgo\Tabels\FileA.csv");
            using var readerB = new StreamReader(@"C:\Users\artem\source\repos\MyPathAlgo\Tabels\FileB.csv");
            using var readerC = new StreamReader(@"C:\Users\artem\source\repos\MyPathAlgo\Tabels\FileC.csv");

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
                    currentRecord = strC;
                }
                else if (strC is null || counterC == _iterations)
                {
                    currentRecord = strB;
                    flag = true;
                }
                else
                {                   
                    if (CompareElements(strB, strC))
                    {                        
                        currentRecord = strB;
                        flag = true;
                    }
                    else
                    {                       
                        currentRecord = strC;
                    }
                }

                writerA.WriteLine(currentRecord);

                if (flag)
                {
                    strB = readerB.ReadLine();
                    counterB++;
                }
                else
                {
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
