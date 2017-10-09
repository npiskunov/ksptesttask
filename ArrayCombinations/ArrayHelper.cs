using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArrayCombinations
{
    public static class ArrayHelper
    {
        public static IEnumerable<Tuple<int, int>> ExtractPairs(int[] array, long expectedSum)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array), "Array is null");
            }
            var result = new List<Tuple<int, int>>();
            if (array.Length < 2)
            {
                return result;
            }
            Array.Sort(array);
            var il = 0; //left index
            var ir = array.Length - 1; //right index

            while (il < ir)
            {
                long sum = (long)array[il] + array[ir];
                if (sum == expectedSum)
                {
                    result.Add(new Tuple<int, int>(array[il], array[ir]));
                    il++;
                    ir--;
                    continue;
                }
                if (sum < expectedSum)
                {
                    il++;
                }
                else
                {
                    ir--;
                }
            }
            return result;
        }
    }
}
