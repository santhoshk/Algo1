using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace QuickSort1 {
    class Program {
        static int[] array = null;
        static void Main(string[] args) {
            try {
                List<int> data = new List<int>();
                string allData;
                using (StreamReader sr = new StreamReader("QuickSort.txt")) {
                    allData = sr.ReadToEnd();
                }
                string[] stringRep = allData.Split(new char[] { '\n' });
                foreach (string str in stringRep) {
                    if (String.IsNullOrWhiteSpace(str)) continue;
                    data.Add(Int32.Parse(str));
                }
                array = data.ToArray();
                
                //QuickSortLeftPivot(0, array.Length-1);
                //QuickSortRightPivot(0, array.Length - 1);
                QuickSortMedianPivot(0, array.Length - 1);

                Console.WriteLine("Total number of comparisions  : {0}", TotalComparisions);

            } catch (Exception ex) {
                Console.WriteLine(ex.ToString());
            }
        }
        
        static void QuickSortLeftPivot(int left, int right) {
            //always choose the first element as pivot
            //for counting the number of comparisions add m-1 to the running total when the subproblem size is m
            if (left >= right) return;
            TotalComparisions += right-left;
            //choose pivot as first element, default behaviour
            //Partition array around pivot
            int index = Partition(left, right);
            QuickSortLeftPivot(left, index - 1);
            QuickSortLeftPivot(index + 1, right);
        }

        static void QuickSortRightPivot(int left, int right) {
            //always choose the first element as pivot
            //for counting the number of comparisions add m-1 to the running total when the subproblem size is m
            if (left >= right) return;
            TotalComparisions += right - left;
            //choose pivot as last element
            Swap(left, right);
            //Partition array around pivot
            int index = Partition(left, right);
            QuickSortRightPivot(left, index - 1);
            QuickSortRightPivot(index + 1, right);
        }

        static void QuickSortMedianPivot(int left, int right) {
            //always choose the first element as pivot
            //for counting the number of comparisions add m-1 to the running total when the subproblem size is m
            if (left >= right) return;
            TotalComparisions += right - left;
            //choose pivot as median element
            int medianIndex = MedianIndex(left, right);
            Swap(left, medianIndex);

            //Partition array around pivot
            int index = Partition(left, right);
            QuickSortMedianPivot(left, index - 1);
            QuickSortMedianPivot(index + 1, right);
        }

        private static int Partition(int left, int right) {
            int pivot = array[left];
            int i = left + 1;
            for (int j = left + 1; j <= right; j++) {
                if (array[j] < pivot) {
                    Swap(i,j);
                    i++;
                }
            }
            Swap(left, i-1);
            return i - 1;
        }

        static void Swap(int i, int j) {
            int temp = array[i];
            array[i] = array[j];
            array[j] = temp;
        }

        static int MedianIndex(int left, int right) {
            if (left == right) return left;
            if (left == right - 1) return left;

            int mid = (left + right) / 2;
            
            if((array[left] <= array[mid]) && (array[mid] <= array[right])) return mid;
            if ((array[right] <= array[mid]) && (array[mid] <= array[left])) return mid;

            if ((array[mid] <= array[left]) && (array[left] <= array[right])) return left;
            if ((array[right] <= array[left]) && (array[left] <= array[mid])) return left;

            if ((array[mid] <= array[right]) && (array[right] <= array[left])) return right;
            if ((array[left] <= array[right]) && (array[right] <= array[mid])) return right;

            throw new NotSupportedException("This should not happen.");
        }

        static int TotalComparisions = 0;
    }
}
