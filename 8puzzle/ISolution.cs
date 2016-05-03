using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace _8puzzle
{
    abstract class ISolution
    {
        // Abstract class ISolution
        //------------------------------------------
        // This is a base class  which implements the common
        // methods for all solutions as well as a common interface.
        // All the solution classes inherit this class and use the functions
        // implemented here. Solution classes only implement the solution methods
        // separate from this class.

        // A list to keep which states are visited
        protected LinkedList<PuzzleState> visitedStates;
        
        // This pointer will keep last (goal state) and will be
        // used to track back the solution.
        protected PuzzleState lastState;

        // This is the starting point for exploration
        protected PuzzleState startState;
        
        // This will be used for displaying the states on screen
        protected PuzzleState currentPuzzleState;

        // Calculations are operated in a separate thread from main program.
        // Also it will be easy to pause and stop.
        protected Thread workingThread;

        // Whether the solution has been found.
        protected bool finished = false;

        // This value is for prompting a single step of iteration.
        protected bool singleStep = false;

        // Property for accessing currentPuzzleState: read-only.
        public PuzzleState CurrentPuzzleState
        {
            get { return currentPuzzleState; }
        }
        
        // A string to keep and return what the code is currently doing.
        protected string currentState = "Waiting for start";

        // Start solving.
        public void Start()
        {
            if (workingThread != null)
            {
                // Start the current thread.
                workingThread.Start();
                currentState = "Processing";
            }
            return;
        }
        // For aborting solution.
        public void Stop()
        {

            if (workingThread != null)
            {
                // System cannot abort if suspended.
                if (workingThread.ThreadState == ThreadState.Suspended)
                    workingThread.Resume();
                // Stop the thread.
                workingThread.Abort();
                currentState = "Aborted";
            }
        }
        // For pasuing solution.
        public void Pause()
        {
            if (workingThread != null)
            {
                workingThread.Suspend();
                currentState = "Paused";
            }
        }
        // For stopping solution.
        public void Continue()
        {
            if (workingThread != null)
            {
                workingThread.Resume();
                currentState = "Processing";
            }
        }
        
        // For propting a single step in the iterations.
        public void SingleStep()
        {
            // Set variable for prompting. Each solution checks this value 
            // in an appropriate time.
            singleStep = true;
            // If suspended, resume.
            if (workingThread.ThreadState == ThreadState.Suspended)
                workingThread.Resume();
                //If not started yet, start.
            else if (workingThread.ThreadState == ThreadState.Unstarted)
                workingThread.Start();
        }

        // Returns how many nodes are explored.
        public int GetExpandedNodes()
        { 
            // Return the count of the list that keeps the visited states.
            return visitedStates.Count + 1; 
        }

        // Not to be confused with CurrentPuzzleState!
        // This method returns the string indicating what the solution
        // method is doing currently.
        public string GetCurrentState()
        { return currentState; }

        // A method for checking if a state is visited,
        // i. e. in visited states list.
        protected bool IsVisited(PuzzleState state)
        {
            // Check if there is a match  for the state
            // in the visited states list.
            foreach (PuzzleState visitedState in visitedStates)
            {
                // Overridden equals function checks whether
                // all of the placements are same.
                if (visitedState.Equals(state))
                {
                    return true;
                }
            }
            return false;
        }

        // This method is for setting up the necessary variables 
        // when the solution is found.
        protected void Finish()
        {
            finished = true;
            currentState = "Finished";
        }

        // Returns a list that contains all the steps from start to goal.
        public LinkedList<PuzzleState> GetSolution()
        {
            // Return null if not finished yet.
            if (finished == false)
                return null;
            else
            {
                // Create a new list.
                LinkedList<PuzzleState> solution = new LinkedList<PuzzleState>();
                // Start with last state-
                solution.AddFirst(lastState);
                // Until you reach the start state-
                while (solution.First.Value.Parent != null)
                {
                    // Add the parent of the topmost item to the top of the list.
                    solution.AddFirst(solution.First.Value.Parent);
                }
                return solution;
            }
        }
    }
}
