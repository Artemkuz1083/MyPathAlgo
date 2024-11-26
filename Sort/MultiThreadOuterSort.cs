using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sort
{
    public class MultiThreadOuterSort
    {
        private string? _headers;
        private readonly int _chosenField;
        private long _iterations, _segments;
        
        public MultiThreadOuterSort(int chosenField)
        {
            _chosenField = chosenField;
            _iterations = 1;
        }

        private void SplitToFiles()
        {
            _segments = 1;
            using var fileA = new StreamReader("A.csv");
            _headers = fileA.ReadLine();

            using var fileB = new StreamWriter("B.csv");
            using var fileC = new StreamWriter("C.csv");
            using var fileD = new StreamWriter("D.csv");
            string? currentRecord = fileA.ReadLine();
            //переменная flag поменяла свой тип с bool на int, т.к. теперь нам нужно больше
            //двух значений
            int flag = 0;
            int counter = 0;
            while (currentRecord is not null)
            {
                if (counter == _iterations)
                {
                    counter = 0;
                    flag = GetNextFileIndexToWrite(flag);
                    _segments++;
                }

                switch (flag)
                {
                    case 0:
                        fileB.WriteLine(currentRecord);
                        break;
                    case 1:
                        fileC.WriteLine(currentRecord);
                        break;
                    case 2:
                        fileD.WriteLine(currentRecord);
                        break;
                }

                currentRecord = fileA.ReadLine();
                counter++;
            }
        }

        //Метод получения следующего индекса файла для записи (B = 0, C = 1, D = 2)
        private static int GetNextFileIndexToWrite(int currentIndex)
                => currentIndex switch
                {
                    0 => 1,
                    1 => 2,
                    2 => 0,
                    _ => throw new Exception("Что-то вышло из под контроля. Будем разбираться")
                };

        private void MergePairs()
        {

        }
    }
}
