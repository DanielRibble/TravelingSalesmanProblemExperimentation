using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TSP
{
    class Algorithms
    {
        public ArrayList greedy(City[] cities)
        {

            ArrayList route = new ArrayList();//route to return
            int length = cities.Length;//number of nodes needed to be in route
            List<int> unvisitedCities = new List<int>();//cities not visited in a single trial
            List<int> allCities = new List<int>();//all cities for resetting the unvisited cities list
            for (int i = 0; i < length; i++)
            {
                unvisitedCities.Add(i);
                allCities.Add(i);
            }

            //start greedy algorithm with node 0 then start over at node 1 and so on, if unsolvable
            bool notDone = true;
            int startPosition = 0;
            while (notDone)
            {
                notDone = false;
                int currentPosition = startPosition;
                route.Add(cities[currentPosition]);
                unvisitedCities.Remove(currentPosition);

                //must finish this for-loop if a greedy path is reachable
                for (int i = 0; i < length-1; i++)
                {
                    //look for shortest path out of node
                    int minPathIndex = -1;
                    double minPathDistance = Double.PositiveInfinity;
                    for (int j = 0; j < unvisitedCities.Count; j++)
                    {
                        double currentDistance = cities[currentPosition].costToGetTo(cities[unvisitedCities[j]]);
                        if (currentDistance < minPathDistance)
                        {
                            minPathDistance = currentDistance;
                            minPathIndex = unvisitedCities[j];
                        }
                    }

                    if (minPathDistance == Double.PositiveInfinity)//unsolvable from this point so reset and start from a new node
                    {
                        route.Clear();
                        unvisitedCities = new List<int>(allCities);
                        startPosition++;
                        notDone = true;
                        break;
                    }
                    else//include path and eliminate from unvisited cities list
                    {
                        unvisitedCities.Remove(minPathIndex);
                        currentPosition = minPathIndex;
                        route.Add(cities[currentPosition]);
                    }

                }

                //check to see if last node has a valid path back to the starting node
                if(cities[currentPosition].costToGetTo(cities[startPosition]) == Double.PositiveInfinity)
                {
                    route.Clear();
                    unvisitedCities = new List<int>(allCities);
                    startPosition++;
                    notDone = true;
                }
            }
            
            return route;
        }


        public ArrayList random(City[] cities)
        {
            Random random = new Random();
            ArrayList route = new ArrayList();//route to return
            int length = cities.Length;//number of nodes needed to be in route
            List<int> unvisitedCities = new List<int>();//cities not visited in a single trial
            List<int> allCities = new List<int>();//all cities for resetting the unvisited cities list
            for (int i = 0; i < length; i++)
            {
                unvisitedCities.Add(i);
                allCities.Add(i);
            }

            //start random algorithm with node 0 then start over at node 1 and so on, if unsolvable
            bool notDone = true;
            int startPosition = 0;
            while (notDone)
            {
                notDone = false;
                int currentPosition = startPosition;
                route.Add(cities[currentPosition]);
                unvisitedCities.Remove(currentPosition);

                //must finish this for-loop if a greedy path is reachable
                for (int i = 0; i < length - 1; i++)
                {
                    //look for shortest path out of node
                    int pathIndex = -1;
                    double pathDistance = Double.PositiveInfinity;
                    for (int j = 0; j < unvisitedCities.Count; j++)
                    {
                        int randomIndex = random.Next(unvisitedCities.Count);
                        double currentDistance = cities[currentPosition].costToGetTo(cities[unvisitedCities[randomIndex]]);
                        if (currentDistance < Double.PositiveInfinity)
                        {
                            pathDistance = currentDistance;
                            pathIndex = unvisitedCities[randomIndex];
                            break;
                        }
                    }

                    if (pathDistance == Double.PositiveInfinity)//unsolvable from this point so reset and start from a new node
                    {
                        route.Clear();
                        unvisitedCities = new List<int>(allCities);
                        startPosition++;
                        notDone = true;
                        break;
                    }
                    else//include path and eliminate from unvisited cities list
                    {
                        unvisitedCities.Remove(pathIndex);
                        currentPosition = pathIndex;
                        route.Add(cities[currentPosition]);
                    }

                }

                //check to see if last node has a valid path back to the starting node
                if (cities[currentPosition].costToGetTo(cities[startPosition]) == Double.PositiveInfinity)
                {
                    route.Clear();
                    unvisitedCities = new List<int>(allCities);
                    startPosition++;
                    notDone = true;
                }
            }

            return route;
        }

    }
}
