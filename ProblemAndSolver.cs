using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Diagnostics;

namespace TSP
{

    class ProblemAndSolver
    {

        public class TSPSolution
        {
            /// <summary>
            /// we use the representation [cityB,cityA,cityC] 
            /// to mean that cityB is the first city in the solution, cityA is the second, cityC is the third 
            /// and the edge from cityC to cityB is the final edge in the path.  
            /// You are, of course, free to use a different representation if it would be more convenient or efficient 
            /// for your node data structure and search algorithm. 
            /// </summary>
            public ArrayList
                Route;
            public double cost;

            public TSPSolution(ArrayList iroute)
            {
                Route = new ArrayList(iroute);
                cost = costOfRoute();
            }


            /// <summary>
            /// Compute the cost of the current route.  
            /// Note: This does not check that the route is complete.
            /// It assumes that the route passes from the last city back to the first city. 
            /// </summary>
            /// <returns></returns>
            public double costOfRoute()
            {
                // go through each edge in the route and add up the cost. 
                int x;
                City here;
                double cost = 0D;

                for (x = 0; x < Route.Count - 1; x++)
                {
                    here = Route[x] as City;
                    cost += here.costToGetTo(Route[x + 1] as City);
                }

                // go from the last city to the first. 
                here = Route[Route.Count - 1] as City;
                cost += here.costToGetTo(Route[0] as City);
                return cost;
            }

            public override bool Equals(System.Object obj)
            {
                TSPSolution otherSolution = (TSPSolution)obj;
                for (int i = 0; i < Route.Count; i++)
                {
                    if (Route[i] != otherSolution.Route[i])
                    {
                        return false;
                    }
                }
                return true;
            }

            public override int GetHashCode()//If weird things start happening with equals function, change this!
            {
                return base.GetHashCode();
            }
        }

        #region Private members 

        /// <summary>
        /// Default number of cities (unused -- to set defaults, change the values in the GUI form)
        /// </summary>
        // (This is no longer used -- to set default values, edit the form directly.  Open Form1.cs,
        // click on the Problem Size text box, go to the Properties window (lower right corner), 
        // and change the "Text" value.)
        private const int DEFAULT_SIZE = 25;

        private const int CITY_ICON_SIZE = 5;

        // For normal and hard modes:
        // hard mode only
        private const double FRACTION_OF_PATHS_TO_REMOVE = 0.20;

        /// <summary>
        /// the cities in the current problem.
        /// </summary>
        private City[] Cities;
        /// <summary>
        /// a route through the current problem, useful as a temporary variable. 
        /// </summary>
        private ArrayList Route;
        /// <summary>
        /// best solution so far. 
        /// </summary>
        private TSPSolution bssf; 

        /// <summary>
        /// how to color various things. 
        /// </summary>
        private Brush cityBrushStartStyle;
        private Brush cityBrushStyle;
        private Pen routePenStyle;


        /// <summary>
        /// keep track of the seed value so that the same sequence of problems can be 
        /// regenerated next time the generator is run. 
        /// </summary>
        private int _seed;
        /// <summary>
        /// number of cities to include in a problem. 
        /// </summary>
        private int _size;

        /// <summary>
        /// Difficulty level
        /// </summary>
        private HardMode.Modes _mode;

        /// <summary>
        /// random number generator. 
        /// </summary>
        private Random rnd;
        #endregion

        #region Public members
        public int Size
        {
            get { return _size; }
        }

        public int Seed
        {
            get { return _seed; }
        }
        #endregion

        #region Constructors
        public ProblemAndSolver()
        {
            this._seed = 1; 
            rnd = new Random(1);
            this._size = DEFAULT_SIZE;

            this.resetData();
        }

        public ProblemAndSolver(int seed)
        {
            this._seed = seed;
            rnd = new Random(seed);
            this._size = DEFAULT_SIZE;

            this.resetData();
        }

        public ProblemAndSolver(int seed, int size)
        {
            this._seed = seed;
            this._size = size;
            rnd = new Random(seed); 
            this.resetData();
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// Reset the problem instance.
        /// </summary>
        private void resetData()
        {

            Cities = new City[_size];
            Route = new ArrayList(_size);
            bssf = null;

            if (_mode == HardMode.Modes.Easy)
            {
                for (int i = 0; i < _size; i++)
                    Cities[i] = new City(rnd.NextDouble(), rnd.NextDouble());
            }
            else // Medium and hard
            {
                for (int i = 0; i < _size; i++)
                    Cities[i] = new City(rnd.NextDouble(), rnd.NextDouble(), rnd.NextDouble() * City.MAX_ELEVATION);
            }

            HardMode mm = new HardMode(this._mode, this.rnd, Cities);
            if (_mode == HardMode.Modes.Hard)
            {
                int edgesToRemove = (int)(_size * FRACTION_OF_PATHS_TO_REMOVE);
                mm.removePaths(edgesToRemove);
            }
            City.setModeManager(mm);

            cityBrushStyle = new SolidBrush(Color.Black);
            cityBrushStartStyle = new SolidBrush(Color.Red);
            routePenStyle = new Pen(Color.Blue,1);
            routePenStyle.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// make a new problem with the given size.
        /// </summary>
        /// <param name="size">number of cities</param>
        //public void GenerateProblem(int size) // unused
        //{
        //   this.GenerateProblem(size, Modes.Normal);
        //}

        /// <summary>
        /// make a new problem with the given size.
        /// </summary>
        /// <param name="size">number of cities</param>
        public void GenerateProblem(int size, HardMode.Modes mode)
        {
            this._size = size;
            this._mode = mode;
            resetData();
        }

        /// <summary>
        /// return a copy of the cities in this problem. 
        /// </summary>
        /// <returns>array of cities</returns>
        public City[] GetCities()
        {
            City[] retCities = new City[Cities.Length];
            Array.Copy(Cities, retCities, Cities.Length);
            return retCities;
        }

        /// <summary>
        /// draw the cities in the problem.  if the bssf member is defined, then
        /// draw that too. 
        /// </summary>
        /// <param name="g">where to draw the stuff</param>
        public void Draw(Graphics g)
        {
            float width  = g.VisibleClipBounds.Width-45F;
            float height = g.VisibleClipBounds.Height-45F;
            Font labelFont = new Font("Arial", 10);

            // Draw lines
            if (bssf != null)
            {
                // make a list of points. 
                Point[] ps = new Point[bssf.Route.Count];
                int index = 0;
                foreach (City c in bssf.Route)
                {
                    if (index < bssf.Route.Count -1)
                        g.DrawString(" " + index +"("+c.costToGetTo(bssf.Route[index+1]as City)+")", labelFont, cityBrushStartStyle, new PointF((float)c.X * width + 3F, (float)c.Y * height));
                    else 
                        g.DrawString(" " + index +"("+c.costToGetTo(bssf.Route[0]as City)+")", labelFont, cityBrushStartStyle, new PointF((float)c.X * width + 3F, (float)c.Y * height));
                    ps[index++] = new Point((int)(c.X * width) + CITY_ICON_SIZE / 2, (int)(c.Y * height) + CITY_ICON_SIZE / 2);
                }

                if (ps.Length > 0)
                {
                    g.DrawLines(routePenStyle, ps);
                    g.FillEllipse(cityBrushStartStyle, (float)Cities[0].X * width - 1, (float)Cities[0].Y * height - 1, CITY_ICON_SIZE + 2, CITY_ICON_SIZE + 2);
                }

                // draw the last line. 
                g.DrawLine(routePenStyle, ps[0], ps[ps.Length - 1]);
            }

            // Draw city dots
            foreach (City c in Cities)
            {
                g.FillEllipse(cityBrushStyle, (float)c.X * width, (float)c.Y * height, CITY_ICON_SIZE, CITY_ICON_SIZE);
            }

        }

        /// <summary>
        ///  return the cost of the best solution so far. 
        /// </summary>
        /// <returns></returns>
        public double costOfBssf ()
        {
            if (bssf != null)
                return (bssf.costOfRoute());
            else
                return -1D; 
        }

        /// <summary>
        ///  solve the problem.  This is the entry point for the solver when the run button is clicked
        /// right now it just picks a simple solution. 
        /// </summary>
        public void solveProblemBandB()
        {
            //initialize BSSF with a greedy algorithm
            Algorithms algorithms = new Algorithms();
            bssf = new TSPSolution(algorithms.greedy(Cities));
            Node.bssf = bssf.costOfRoute();

            int maxQsize = 0;
            int totalStates = 0;

            int timeSeconds = Convert.ToInt32(Program.MainForm.textBoxTime.Text);

            //set up priority queue and stopwatch
            PriorityQueue PQ = new PriorityQueue();
            PQ.insert(new Node(Cities));
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            while(PQ.getSize() > 0 && stopwatch.Elapsed.TotalSeconds < timeSeconds)
            {
                //pop node off of queue and check lower bound against bssf
                Node node = PQ.deleteMin();
                if(node.lowerBound > Node.bssf)
                {
                    Node.prunes++;
                    break;
                }

                Node include = null;
                Node exclude = null;
                double maxDif = Double.NegativeInfinity;

                //search for include/exclude edge that gives max difference in lower bound
                double[,] matrix = node.rCmatrix;
                for (int i = 0; i < node.matrixLength; i++)
                {
                    if (node.exited[i] == -1)
                    {
                        for (int j = 0; j < node.matrixLength; j++)
                        {
                            if (matrix[i, j] == 0)
                            {
                                Node tempInclude = new Node(node, true, i, j);
                                Node tempExclude = new Node(node, false, i, j);
                                double potentialMaxDif = tempExclude.lowerBound - tempInclude.lowerBound;
                                if (potentialMaxDif > maxDif)
                                {
                                    maxDif = potentialMaxDif;
                                    include = tempInclude;
                                    exclude = tempExclude;
                                }
                            }
                        }
                    }
                    
                }
                
                //check if found a bssf
                if(include.totalEdges == include.matrixLength && include.lowerBound < Node.bssf)
                {
                    Node.bssfUpdates++;
                    Node.bssf = include.lowerBound;
                    Node.bssfNode = include;
                }
                else if(include.lowerBound < Node.bssf)//add include node to queue
                {
                    PQ.insert(include);
                    int currentQSize = PQ.getSize();
                    if(currentQSize > maxQsize)
                    {
                        maxQsize = currentQSize;
                    }
                }
                else//prune include node
                {
                    Node.prunes++;
                }

                if(exclude.lowerBound < Node.bssf)//add exclude node to queue
                {
                    PQ.insert(exclude);
                    int currentQSize = PQ.getSize();
                    if (currentQSize > maxQsize)
                    {
                        maxQsize = currentQSize;
                    }
                }
                else//prune exclude node
                {
                    Node.prunes++;
                }

                totalStates += 2;//2 states are created per while-loop iteration

            }

            stopwatch.Stop();

            //if stopwatch is < 30, then we have found an optimal solution
            bool isOptimal = false;
            if(stopwatch.Elapsed.TotalSeconds < timeSeconds)
            {
                isOptimal = true;
            }

            //prune number of items left in the queue
            Node.prunes += PQ.getSize();
            
            //if a bssf has been found better than the greedy solution
            if(Node.bssfNode != null)
            {
                Node solution = Node.bssfNode;

                ArrayList route = solution.getRoute(Cities);

                // call this the best solution so far.  bssf is the route that will be drawn by the Draw method. 
                bssf = new TSPSolution(route);
            }



            //display stats
            if (isOptimal)
            {
                Program.MainForm.tbCostOfTour.Text = " " + bssf.costOfRoute() + "*";
            }
            else
            {
                Program.MainForm.tbCostOfTour.Text = " " + bssf.costOfRoute();
            }
            
            Program.MainForm.tbElapsedTime.Text = " " + stopwatch.Elapsed.TotalSeconds;

            // do a refresh. 
            Program.MainForm.Invalidate();

            //print more stats
            Console.WriteLine();
            Console.WriteLine("Max # of stored states: " + maxQsize);
            Console.WriteLine("# of BSSF updates: " + Node.bssfUpdates);
            Console.WriteLine("Total # of states created: " + totalStates);
            Console.WriteLine("Total # of states pruned: " + Node.prunes);

            Node.resetStaticVariables();
        }

        public void solveProblemGreedy()
        {
            //initialize BSSF with a greedy algorithm

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            Algorithms algorithms = new Algorithms();
            bssf = new TSPSolution(algorithms.greedy(Cities));

            stopwatch.Stop();

            Program.MainForm.tbCostOfTour.Text = " " + bssf.costOfRoute();
            Program.MainForm.tbElapsedTime.Text = " " + stopwatch.Elapsed.TotalSeconds;

            // do a refresh. 
            Program.MainForm.Invalidate();
        }

        public void solveProblemRandom()
        {
            //initialize BSSF with a greedy algorithm

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            Algorithms algorithms = new Algorithms();
            bssf = new TSPSolution(algorithms.random(Cities));

            stopwatch.Stop();

            Program.MainForm.tbCostOfTour.Text = " " + bssf.costOfRoute();
            Program.MainForm.tbElapsedTime.Text = " " + stopwatch.Elapsed.TotalSeconds;

            // do a refresh. 
            Program.MainForm.Invalidate();
        }

        //Our TSP 
        internal void solveProblemTabu()
        {

            int bssfUpdates = 0;
            Algorithms algorithms = new Algorithms();
            bssf = new TSPSolution(algorithms.greedy(Cities));
            TSPSolution currentSolution = new TSPSolution(bssf.Route);

            int size = Convert.ToInt32(Program.MainForm.textBoxTabuSize.Text);
            TabuList tabuList = new TabuList(size);//set capacity of tabuList
            int timeSeconds = Convert.ToInt32(Program.MainForm.textBoxTime.Text);
            double totalTime = 0;

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            while(stopwatch.Elapsed.TotalSeconds < timeSeconds)//run for 600 seconds, 10 minutes
            {
                currentSolution = findBestCandidate(currentSolution, tabuList);
                if(currentSolution.cost < bssf.cost)
                {
                    bssf = new TSPSolution(currentSolution.Route);
                    totalTime = stopwatch.Elapsed.TotalSeconds;
                    bssfUpdates++;                    
                }
                tabuList.addSolution(currentSolution);
            }

            stopwatch.Stop();

            Program.MainForm.tbCostOfTour.Text = " " + bssf.cost;
            Program.MainForm.tbElapsedTime.Text = " " + totalTime;
            //Program.MainForm.tbElapsedTime.Text = " " + stopwatch.Elapsed.TotalSeconds;
            Program.MainForm.toolStripTextBox1.Text = " " + bssfUpdates;

            //print bssf update number

            // do a refresh. 
            Program.MainForm.Invalidate();
        }

        public TSPSolution findBestCandidate(TSPSolution currentSolution, TabuList tabuList)
        {
            TSPSolution bestCandidate = null;
            TSPSolution candidate = new TSPSolution(new ArrayList(currentSolution.Route));

            for (int swapA = 0; swapA < Cities.Length - 1; swapA++)
            {
                // only consider swapping forward so we don't double the space
                for (int swapB = swapA + 1; swapB < Cities.Length; swapB++)
                {
                    //candidate.Route.Clear();

                    // perform swap
                    City tmp = (City)candidate.Route[swapA];
                    candidate.Route[swapA] = candidate.Route[swapB];
                    candidate.Route[swapB] = tmp;

                    // normalize to make TabuList.Contains() easier
                    if (swapA == 0)
                    {
                        ArrayList normalRoute = new ArrayList();
                        for (int i = swapB; i < Cities.Length; i++)
                            normalRoute.Add(candidate.Route[i]);
                        for (int i = 0; i < swapB; i++)
                            normalRoute.Add(candidate.Route[i]);
                        candidate.Route = normalRoute;
                    }

                    // recalc cost
                    candidate.cost = candidate.costOfRoute();

                    // get best candidate
                    if (!tabuList.contains(candidate))
                    {
                        if (bestCandidate == null || candidate.cost < bestCandidate.cost){
                            bestCandidate = new TSPSolution(new ArrayList(candidate.Route));
                        }
                            
                    }

                    // revert swap for next candidate
                    if (swapA == 0)
                    {
                        // it's simpler to recopy
                        candidate.Route = new ArrayList(currentSolution.Route);
                    }
                    else
                    {
                        candidate.Route[swapB] = candidate.Route[swapA];
                        candidate.Route[swapA] = tmp;
                    }
                }
            }

            if (bestCandidate == null)
            {
                tabuList.setCapacity(tabuList.capacity * 2);
                return tabuList.getLast(); // or whatever this function ends up being called
            }
               
            return bestCandidate;
        }





        #endregion
    }

}
