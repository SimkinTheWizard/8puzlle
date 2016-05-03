using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace _8puzzle
{
    class BreadthFirst : ISolution
    {
        // Breadth first solution class
        // The method uses the functions of ISolution base class
        // for basic functions and only implements the solution.

        // Constructor function to set up appropriate variables.
        public BreadthFirst(PuzzleState startState)
        {
            this.startState = startState;
            workingThread = new Thread(new ThreadStart(Run));
            visitedStates = new LinkedList<PuzzleState>();
        }


        // Method for beadth first solution. The method employs a queue for 
        // first analyzing all the possible next moves and adds their next moves
        private void Run()
        {
            // Allocate queue.
            Queue<PuzzleState> processingQueue = new Queue<PuzzleState>();
            // Start with initial state.
            processingQueue.Enqueue(startState);
            while (true)
            {
                // Dequeue the first state.
                PuzzleState state = processingQueue.Dequeue();
                // if this state is goal, finish.
                if (PuzzleState.IsGoal(state))
                {
                    this.lastState = state;
                    Finish();
                    return ;
                }
                currentPuzzleState = state;
                // if moving single step command is prompted 
                // pause operation.
                if (singleStep == true)
                {
                    singleStep = false;
                    workingThread.Suspend();
                }
                // If this state is not visited before add it to the 
                // list of visited states.
                if (IsVisited(state) == false)
                    visitedStates.AddLast(state);
                // Get the successors of the current State.
                LinkedList<PuzzleState> successors = PuzzleState.Successors(state);
                foreach (PuzzleState successor in successors)
                {
                    // if it is not visited
                    if (IsVisited(successor) == false)
                    {
                        // Add to the last of the queue so that it will be processed 
                        // after all the breadth is examined (Breadth first ensured.)
                        visitedStates.AddLast(successor);
                        processingQueue.Enqueue(successor);
                    }
                }
            }

        }

        
    }
}
