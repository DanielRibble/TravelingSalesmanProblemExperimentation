using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace TSP
{
//node for Include/Exclude Branch and Bound
    class Node
    {
        public static double bssf = Double.PositiveInfinity;
        public static Node bssfNode = null;
        public static int prunes = 0;
        public static int bssfUpdates = 0;

        public int[] entered;
        public int[] exited;

        public double[,] rCmatrix;
        public int matrixLength;
        public double lowerBound = 0;
        public int totalEdges;

        //constructor for creating first node
        public Node(City[] cities)
        {
            
            matrixLength = cities.Length;
            entered = new int[matrixLength];
            exited = new int[matrixLength];
            for (int i = 0; i < matrixLength; i++)
            {
                entered[i] = -1;
                exited[i] = -1;
            }

            rCmatrix = new double[matrixLength, matrixLength];
            for (int i = 0; i < matrixLength; i++)
            {
                for (int j = 0; j < matrixLength; j++)
                {
                    rCmatrix[i, j] = cities[i].costToGetTo(cities[j]);
                }
            }

            for(int i = 0; i < matrixLength; i++)
            {
                rCmatrix[i, i] = Double.PositiveInfinity;
            }

            lowerBound = 0;
            totalEdges = 0;
            reduceMatrix(rCmatrix);
        }

        //constructor for creating all nodes but the first
        public Node(Node prev, bool isInclude, int exit, int enter)
        {
            //copy info needed from parent node
            this.lowerBound = prev.lowerBound;
            this.matrixLength = prev.matrixLength;
            this.rCmatrix = copy2Darray(prev.rCmatrix);
            this.entered = copyArray(prev.entered);
            this.exited = copyArray(prev.exited);

            if (isInclude)//code only for an include node
            {
                this.totalEdges = prev.totalEdges + 1;
                entered[enter] = exit;
                exited[exit] = enter;
                rCmatrix[enter, exit] = Double.PositiveInfinity;
                eliminateEdges(rCmatrix, exit, enter);
                if(this.totalEdges < matrixLength)
                {
                    eliminatePCycles(rCmatrix, exit, enter);
                }
                
            }
            else//code only for an exclude node
            {
                this.totalEdges = prev.totalEdges;
                rCmatrix[exit, enter] = Double.PositiveInfinity;
            }

            reduceMatrix(rCmatrix);
        }

        //set row/col to infinity
        private void eliminateEdges(double[,] matrix, int exit, int enter)
        {
            for (int i = 0; i < matrixLength; i++)
            {
                matrix[exit, i] = Double.PositiveInfinity;
                matrix[i, enter] = Double.PositiveInfinity;
            }
        }

        //eliminate premature cycles in include nodes
        private void eliminatePCycles(double[,] matrix, int exit, int enter)
        {
            int start = exit;
            int end = enter;
            int partialPathLength = 1;
            while(exited[end] != -1)
            {
                end = exited[end];
                partialPathLength++;
            }
            while(entered[start] != -1)
            {
                start = entered[start];
                partialPathLength++;
            }
            if(partialPathLength < (matrixLength - 1))
            {
                while(start != enter)
                {
                    matrix[end, start] = Double.PositiveInfinity;
                    matrix[enter, start] = Double.PositiveInfinity;
                    start = exited[start];
                }
            }
        }

        private void reduceMatrix(double[,] matrix)
        {
            //iterate through rows (exits)
            for (int row = 0; row < matrixLength; row++)
            {
                //only go down row if it is not already all infinities
                if(exited[row] == -1)
                {
                    //find minimum
                    double currentMin = Double.PositiveInfinity;
                    for (int col = 0; col < matrixLength; col++)
                    {
                        if (matrix[row, col] < currentMin)
                        {
                            currentMin = matrix[row, col];
                        }
                    }

                    //subtract minimum from each entry in row if needed
                    if (currentMin > 0)
                    {
                        lowerBound += currentMin;
                        if(currentMin < Double.PositiveInfinity)
                        {
                            for (int col = 0; col < matrixLength; col++)
                            {
                                matrix[row, col] -= currentMin;
                            }
                        }
                        
                    }
                }
            }

            //iterate through columns (entrances)
            for(int col = 0; col < matrixLength; col++)
            {
                if(entered[col] == -1)
                {
                    //find minimum
                    double currentMin = Double.PositiveInfinity;
                    for (int row = 0; row < matrixLength; row++)
                    {
                        if (matrix[row, col] < currentMin)
                        {
                            currentMin = matrix[row, col];
                        }
                    }

                    //subtract minimum from each entry in col if needed
                    if (currentMin > 0)
                    {
                        lowerBound += currentMin;
                        if(currentMin < Double.PositiveInfinity)
                        {
                            for (int row = 0; row < matrixLength; row++)
                            {
                                matrix[row, col] -= currentMin;
                            }
                        }
                        
                    }
                }
                
            }
        }

        private int[] copyArray(int[] toCopy)
        {
            int[] toReturn = new int[toCopy.Length];
            for (int i = 0; i < toCopy.Length; i++)
            {
                toReturn[i] = toCopy[i];
            }

            return toReturn;
        }

        private double[,] copy2Darray(double[,] toCopy)
        {
            double[,] toReturn = new double[toCopy.GetLength(0), toCopy.GetLength(1)];
            for (int i = 0; i < toCopy.GetLength(0); i++)
            {
                for (int j = 0; j < toCopy.GetLength(1); j++)
                {
                    toReturn[i, j] = toCopy[i, j];
                }
            }
            return toReturn;
        }

        public ArrayList getRoute(City[] cities)
        {
            int iterator = 0;
            ArrayList toReturn = new ArrayList();
            toReturn.Add(cities[iterator]);
            while (iterator != entered[0])
            {
                iterator = exited[iterator];
                toReturn.Add(cities[iterator]);
            }
            return toReturn;
        }

        public static void resetStaticVariables()
        {
            Node.bssf = Double.PositiveInfinity;
            Node.bssfNode = null;
            Node.prunes = 0;
            Node.bssfUpdates = 0;
        }
    }
}
