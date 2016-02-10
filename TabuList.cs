using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TSP
{
    class TabuList
    {

        public int capacity;
        LinkedList<ProblemAndSolver.TSPSolution> tabuList;

        public TabuList(int capacity)
        {
            this.capacity = capacity;
            tabuList = new LinkedList<ProblemAndSolver.TSPSolution>();
        }

        public void addSolution(ProblemAndSolver.TSPSolution solution)
        {
            tabuList.AddFirst(solution);
            if (tabuList.Count == capacity)
            {
                tabuList.RemoveLast();
            }
        }

        public bool contains(ProblemAndSolver.TSPSolution otherSolution)
        {
            bool foundSolution = false;
            foreach (ProblemAndSolver.TSPSolution solution in tabuList)
            {
                if (solution.cost == otherSolution.cost)
                {
                    if (solution.Equals(otherSolution))
                    {
                        foundSolution = true;
                    }
                }
            }
            return foundSolution;
        }

        public ProblemAndSolver.TSPSolution getLast()
        {
            return tabuList.Last();
        }

        public void setCapacity(int capacity)
        {
            this.capacity = capacity;
        }
    }
}
