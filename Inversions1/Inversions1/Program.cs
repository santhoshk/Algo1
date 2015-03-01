using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Linq;

namespace Inversions1 {
    class Program {
        static void Main(string[] args) {
            try {
                List<int> data = new List<int>();
                string allData;
                using (StreamReader sr = new StreamReader("IntegerArray.txt")) {
                    allData = sr.ReadToEnd();
                }
                string[] stringRep = allData.Split(new char[] { '\n' });
                foreach (string str in stringRep) {
                    if (String.IsNullOrWhiteSpace(str)) continue;
                    data.Add(Int32.Parse(str));
                }
                int[] unsortedArray = data.ToArray();

                //Int64 numberOfInversions = CountInversionsBruteForce(unsortedArray);
                //Console.WriteLine("The total number of inversions is {0}", numberOfInversions);

                int[] sortedArray = MergeSort(unsortedArray, 0, unsortedArray.Length - 1);
                Console.WriteLine("Total Inversions is {0}", totalSplitInversions);

            } catch (Exception ex) {
                Console.WriteLine(ex.ToString());
            }
        }


        public static int[] CountInversionsDivideAndConquer(int[] data) {
            return MergeSort(data, 0, data.Length - 1);
        }


        static int[] MergeSort(int[] data, int left, int right) {
            if(left > right) return new int[] {};
            if (left == right) return new int[] { data[left] };
            int mid = (left + right) / 2;
            int[] leftArray = MergeSort(data, left, mid);
            int[] rightArray = MergeSort(data, mid + 1, right);
            return Merge(leftArray, rightArray);
        }

        static int[] Merge(int[] left, int[] right) {
            if (left == null || left.Length == 0) return right;
            if (right == null || right.Length == 0) return left;
            int leftLen = left.Length;
            int rightLen = right.Length;
            int n = leftLen+rightLen;
            int i = 0, j = 0, k = 0;
            int[] mergedArray = new int[n];
            for (k = 0; k < n; k++) {
                if (i == leftLen) {
                    mergedArray[k] = right[j];
                    j++;
                    continue;
                }
                if (j == rightLen) {
                    mergedArray[k] = left[i];
                    i++;
                    continue;
                }

                if (left[i] < right[j]) { //left gets copied to output
                    mergedArray[k] = left[i];
                    i++;
                } else { //right gets copied to output
                    mergedArray[k] = right[j];
                    j++;
                    totalSplitInversions += leftLen - i;
                }
            }
            return mergedArray;
        }

        public static Int64 totalSplitInversions = 0;

        //public static int SortAndCount(int[] data, int start, int end) {
        //    if (end >= start) return 0;
        //    int mid = (start+end)/2;
        //    int x = SortAndCount(data, start, mid);
        //    int y = SortAndCount(data, mid + 1, end);
        //    int z = MergeAndCount(data, start, end);
        //    return x + y + z;
        //}


        public static Int64 CountInversionsBruteForce(int[] data) {
            Int64 totalInversions = 0;
            if (data == null || data.Length == 0) return totalInversions;
            int totalLength = data.Length;
            for (Int64 i = 0; i < totalLength; i++) {
                if (i % 1000 == 0) Console.WriteLine(i);
                for (Int64 j = i + 1; j < totalLength; j++) {
                    if (j >= totalLength) continue;
                    if (data[j] < data[i]) totalInversions++;
                }
            }

            return totalInversions;
        }

    }
}
