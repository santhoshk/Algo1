using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MedianMaintenance {
    class Program {
        static List<int> elts = new List<int>();
        static List<int> lowHeap = new List<int>();
        static List<int> highHeap = new List<int>();

        static void Main(string[] args) {
            string inputFile = "Median.txt";
            FillElts(inputFile);

            int medianSum = 0;

            for (int i = 0; i < elts.Count; i++) {
                int currElt = elts[i];
                if (HighHeapCount() > 0 &&  currElt >= PeekHighHeap()) {
                    AddToMinHeap(highHeap, currElt);
                } else {
                    AddToMaxHeap(lowHeap, currElt);
                }

                if (LowHeapCount() == HighHeapCount() + 2) {
                    AddToMinHeap(highHeap, ExtractMax(lowHeap));
                } else if (LowHeapCount() == HighHeapCount() - 1) {
                    AddToMaxHeap(lowHeap, ExtractMin(highHeap));
                } else if (LowHeapCount() == HighHeapCount() + 1) {
                    //this is just fine
                } else if (HighHeapCount() == LowHeapCount()) {
                    //this is just fine.
                } else {
                    throw new Exception("Invalid heap state.");
                }

                //some checks
                int lowCount = LowHeapCount();
                int highCount = HighHeapCount();
                if((lowCount != highCount) && (lowCount != highCount+1)) {
                    throw new Exception("Invalid heap state at index :  " + i);
                }
                if (lowCount > 0 && highCount > 0) {
                    if (PeekLowHeap() > PeekHighHeap()) {
                        throw new Exception("Low heap contains a larger value than high heap at index " + i);
                    }
                }

                medianSum += PeekLowHeap();

                //int mod = 10000;
                //if(medianSum > mod) medianSum -= mod;

            }

            Console.WriteLine("The mod median sum is {0}", medianSum % 10000);
        }

        private static int HighHeapCount() {
            return highHeap.Count;
        }

        private static int LowHeapCount() {
            return lowHeap.Count;
        }

        private static int PeekLowHeap() {
            return lowHeap[0];
        }

        private static int PeekHighHeap() {
            return highHeap[0];
        }

        private static int ExtractMax(List<int> heap) {
            int max = heap[0];
            heap[0] = heap[heap.Count-1];
            heap.RemoveAt(heap.Count - 1);

            int curr = 0;
            while(true) {
                int lIndex = curr * 2 + 1;
                int rIndex = curr * 2 + 2;

                if (rIndex < heap.Count) {
                    if (heap[curr] < heap[lIndex] || heap[curr] < heap[rIndex]) {
                        if (heap[lIndex] > heap[rIndex]) {
                            Swap(heap, curr, lIndex);
                            curr = lIndex;
                        } else {
                            Swap(heap, curr, rIndex);
                            curr = rIndex;
                        }
                    } else break;
                } else if (lIndex < heap.Count) {
                    if (heap[curr] < heap[lIndex]) {
                        Swap(heap, curr, lIndex);
                        curr = lIndex;
                    } else break;
                } else break;
            }
            return max;
        }

        private static int ExtractMin(List<int> heap) {
            int min = heap[0];
            heap[0] = heap[heap.Count - 1];
            heap.RemoveAt(heap.Count - 1);

            int curr = 0;
            while (true) {
                int lIndex = curr * 2 + 1;
                int rIndex = curr * 2 + 2;

                if (rIndex < heap.Count) {
                    if (heap[curr] > heap[lIndex] || heap[curr] > heap[rIndex]) {
                        if (heap[lIndex] < heap[rIndex]) {
                            Swap(heap, curr, lIndex);
                            curr = lIndex;
                        } else {
                            Swap(heap, curr, rIndex);
                            curr = rIndex;
                        }
                    } else break;
                } else if (lIndex < heap.Count) {
                    if (heap[curr] > heap[lIndex]) {
                        Swap(heap, curr, lIndex);
                        curr = lIndex;
                    } else break;
                } else break;
            }
            return min;
        }

        private static void AddToMaxHeap(List<int> heap, int data) {
            heap.Add(data);
            int curr = heap.Count - 1;
            int parentIndex = (int)Math.Ceiling((double)curr / 2) - 1;
            while (parentIndex >= 0) {
                if (heap[parentIndex] < heap[curr]) {
                    Swap(heap, curr, parentIndex);
                    curr = parentIndex;
                    parentIndex = (int)Math.Ceiling((double)parentIndex / 2) - 1;
                } else break;
            }
        }

        private static void AddToMinHeap(List<int> heap, int data) {
            heap.Add(data);
            int curr = heap.Count - 1;
            int parentIndex = (int)Math.Ceiling((double)curr / 2) - 1;
            while (parentIndex >= 0) {
                if (heap[parentIndex] > heap[curr]) {
                    Swap(heap, curr, parentIndex);
                    curr = parentIndex;
                    parentIndex = (int)Math.Ceiling((double)parentIndex / 2) - 1;
                } else break;
            }
        }

        private static void Swap(List<int> heap, int i, int j) {
            int temp = heap[i];
            heap[i] = heap[j];
            heap[j] = temp;
        }

        private static void FillElts(string inputFile) {
            using (StreamReader sr = new StreamReader(inputFile)) {
                string line;
                while ((line = sr.ReadLine()) != null) {
                    if (!string.IsNullOrWhiteSpace(line)) {
                        elts.Add(Int32.Parse(line));
                    }
                }
            }
        }
    }
}
