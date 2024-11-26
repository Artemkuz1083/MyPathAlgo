using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sort;

namespace TestMyAlgoPath
{
    internal class Test
    {

        public static void Main()
        {
            //NaturalOuterSort naturalOuterSort = new NaturalOuterSort(3);

            //naturalOuterSort.Sort();

            DirectOuterSort directOuterSort = new DirectOuterSort(1);
            directOuterSort.Sort();

        }
    }
}
