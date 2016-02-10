using System;
using System.Collections.Generic;
using System.Text;

namespace TSP
{
    class PriorityQueue
    {
        private List<Node> heap; //binary heap

        public PriorityQueue()
        {
            this.heap = new List<Node>();
        }

        public void insert(Node node)
        {
            heap.Add(node);

            bubbleUp(heap.Count - 1);
        }

        //moving up smaller values towards top of heap
        private void bubbleUp(int startIndex)
        {
            int childIndex = startIndex;

            while (childIndex > 0)//bubble up if not top node
            {
                //compare child to parent and act accordingly
                int parentIndex = (childIndex - 1) / 2;
                if (heap[childIndex].lowerBound < heap[parentIndex].lowerBound)
                {

                    //swap child and parent while updating index array
                    Node temp = heap[parentIndex];

                    heap[parentIndex] = heap[childIndex];

                    heap[childIndex] = temp;

                    childIndex = parentIndex;
                }
                else//if nothing needs updating, break
                {
                    break;
                }
            }
        }

        //return element with lowest index
        public Node deleteMin()
        {
            //save top node to be returned after heap stuff if adjusted
            Node toReturn = heap[0];

            //grab last thing in heap and put it as the first thing
            int lastIndex = heap.Count - 1;
            heap[0] = heap[lastIndex];
            
            heap.RemoveAt(lastIndex);
            lastIndex--;

            //adjust heap as neccessary
            bubbleDown();

            return toReturn;
        }

        //move first node down as necessary
        private void bubbleDown()
        {
            int parentIndex = 0;

            while (true)
            {
                int childIndex = parentIndex * 2 + 1;//leftChildIndex
                if (childIndex > heap.Count - 1) { break; }

                //if right child is less than left child, use right child. Otherwise, just use left child.
                int rightChildIndex = childIndex + 1;
                if (rightChildIndex < heap.Count && heap[rightChildIndex].lowerBound < heap[childIndex].lowerBound)
                {
                    childIndex = rightChildIndex;
                }

                //swap parent and child if child is less than parent
                if (heap[childIndex].lowerBound < heap[parentIndex].lowerBound)
                {
                    Node temp = heap[parentIndex];

                    heap[parentIndex] = heap[childIndex];

                    heap[childIndex] = temp;

                    parentIndex = childIndex;
                }
                else//break if nothing changes
                {
                    break;
                }
            }
        }

        public int getSize()
        {
            return heap.Count;
        }

    }
}
