using System.Diagnostics;

namespace DZ07_Parallelism
{
    public class Program
    {
        static void Main(string[] args)
        {
            var arr1 = CreateArray(100_000);
            var arr2 = CreateArray(1_000_000);
            var arr3 = CreateArray(10_000_000);
            Show(arr1);
            Show(arr2);
            Show(arr3);
            Console.Read();
        }


        private static void Show(int[] x)
        {
            var timer = new Stopwatch();
            Console.WriteLine($"array lenght: {x.Length}");

            timer.Start();
            Sum(x);
            timer.Stop();
            Console.WriteLine("Simple " + timer.ElapsedMilliseconds);
            timer.Reset();


            timer.Start();
            ThreadSum(x);
            timer.Stop();
            Console.WriteLine("Thread " + timer.ElapsedMilliseconds);
            timer.Reset();

            timer.Start();
            ParallelSum(x);
            timer.Stop();
            Console.WriteLine("Parallel " + timer.ElapsedMilliseconds);

            Console.WriteLine();
        }

        private static long ParallelSum(int[] array)
        {
            var newArr = array.Select(i => (long)i).ToArray();
            var result = newArr.AsParallel().Sum();
            return result;
        }


        private static long ThreadSum(int[] array)
        {
            long result = 0;

            var splitArray = new List<int[]>();
            var z = array.Length / 10;

            for (int i = 0; i < 10; i++)
                splitArray.Add(array.Skip(z * i).Take(z).ToArray());

            List<Thread> threads = new List<Thread>();
            foreach (var tempArray in splitArray)
            {
                var thread = new Thread(() => result += Sum(tempArray))
                {
                    IsBackground = true
                };
                threads.Add(thread);
                thread.Start();
            }
            threads.ForEach(x => x.Join());

            return result;
        }

        private static long Sum(int[] array)
        {
            long result = 0;
            for (int i = 0; i < array.Length; i++)
                result += array[i];
            return result;
        }

        private static int[] CreateArray(int max)
        {
            var result = new int[max];
            for (int i = 1; i < max; i++)
                result[i] = i;
            return result;
        }

    }
}