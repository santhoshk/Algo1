using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Threading;

namespace TwoSum {
    class Program {
        public static HashSet<int> data = new HashSet<int>();
        static void Main(string[] args) {

            Console.WriteLine(" ==== Beginning the 2 sum problem ===");

            string inputFile = "HashInt.txt";
            ReadFile(inputFile);

            Console.WriteLine("Finished Reading the input..");
            
            CancellationTokenSource cts = new CancellationTokenSource();
            Task progressTask = Task.Factory.StartNew(() => {
                while (true) {
                    if (cts.Token.IsCancellationRequested) {
                        throw new OperationCanceledException(cts.Token);
                    }
                    Thread.Sleep(50);
                    Console.Write(".");
                }
            }, cts.Token);

            int targetCount = 0;
            for (int sum = 2500; sum <= 4000; sum++) {
                if (DistinctTwoSumEquals(sum)) targetCount++;
            }

            cts.Cancel();

            Console.WriteLine("\n The number of distinct numbers whose sum falls within range is {0}", targetCount);
        }

        private static bool DistinctTwoSumEquals(int sum) {
            foreach (var datum in data) {
                int requiredValue = sum-datum;
                if ((requiredValue >= 0) && data.Contains(requiredValue) && (requiredValue != datum)) return true;
            }
            return false;
        }

        private static void ReadFile(string inputFile) {
            using (StreamReader sr = new StreamReader(inputFile)) {
                string line;
                while ((line = sr.ReadLine()) != null) {
                    if (!string.IsNullOrWhiteSpace(line)) {
                        data.Add(Int32.Parse(line));
                    }
                }
            }
        }
    }
}
