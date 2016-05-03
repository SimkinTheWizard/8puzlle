using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace _8puzzle
{
    class DepthFirst : ISolution
    {
        // Depth first solution class
        // The method uses the functions of ISolution base class
        // for basic functions and only implements the solution.

        // Constructor function to set up appropriate variables.
        public DepthFirst(PuzzleState startState)
        {
            this.startState = startState;           
            workingThread = new Thread(new ThreadStart(Run));
            workingThread.Priority = ThreadPriority.AboveNormal;
            visitedStates = new LinkedList<PuzzleState>();
        }

        // Iterative approach.
        // The method that solves the problem. It uses a stack to keep track of 
        // which nodes to explore. It pushes the children to the top of the stack
        // so that exploring the depth first is ensured.
        private void Run()
        {
            Stack<PuzzleState> processingQueue = new Stack<PuzzleState>();
            // Push the first state.
            processingQueue.Push(startState);
            while (true)
            {
                // If the topmost state is the goal state finish.
                PuzzleState state = processingQueue.Pop();
                if (PuzzleState.IsGoal(state))
                {
                    this.lastState = state;
                    Finish();
                    return;
                }
                currentPuzzleState = state;
                // if moving single step command is prompted 
                // pause operation.
                if (singleStep == true)
                {
                    singleStep = false;
                    workingThread.Suspend();
                }
                // If this state is not visited before

                    // Add it to the list of visited state.
                    if (IsVisited(state) == false)
                    visitedStates.AddLast(state);
                    // Query the successors of this state
                    LinkedList<PuzzleState> successors = PuzzleState.Successors(state);
                    foreach (PuzzleState successor in successors)
                    {
                        // If it is not visited before
                        if (IsVisited(successor) == false)
                        {
                            visitedStates.AddLast(successor);
                            // Push it to the top of the stack to be explored next.
                            processingQueue.Push(successor);
                        }
                    }
                
            }

        }

        // Recursive method
        // Doesn't work because it results in stack overflows
        // when too many nodes (about 5,000-10,000 nodes) are explored
        /*private void Run()
        {
            // Start with initial state
            DepthFirstSolution(startState);
        }

        private bool DepthFirstSolution(PuzzleState state)
        {
            
            // If this is goal state finish.
            if (PuzzleState.IsGoal(state))
            {
                this.lastState = state;
                Finish();
                return true;
            }
           // If it is visited before return false.
            if (IsVisited(state))
                return false;
            // Otherwise add to the list of visited states.
            else
                visitedStates.AddLast(state);
            
            LinkedList<PuzzleState> successors = PuzzleState.Successors( state );
            foreach (PuzzleState successor in successors)
            {
                // Call the functions for the successors. (Depth first ensured)
                if (DepthFirstSolution(successor) == true)
                    return true;
                //else
                  //  GC.Collect();
            }
            return false;
        }*/


    }
}
