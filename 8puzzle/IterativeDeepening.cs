using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace _8puzzle
{
    class IterativeDeepening : ISolution
    {
        // Iterative deepening solution class
        // The method uses the functions of ISolution base class
        // for basic functions and only implements the solution.

        // Constructor function to set up appropriate variables.
        public IterativeDeepening(PuzzleState startState)
        {
            this.startState = startState;
            workingThread = new Thread(new ThreadStart(Run));
            visitedStates = new LinkedList<PuzzleState>();
        }

        // Method for iterative deepening solution.
        // The method keeps two lists, one for current level and the other for next level
        // When a level explored the algorithm begins the search in the next level.
        private void Run()
        {
            // allocate two lists
            LinkedList<PuzzleState> thisLevel = new LinkedList<PuzzleState>();
            LinkedList<PuzzleState> nextLevel = new LinkedList<PuzzleState>();
            // start with the initial state
            thisLevel.AddFirst(startState);
            while (true)
            {
                // If the state is goal return
                if (PuzzleState.IsGoal(thisLevel.First.Value))
                {
                    this.lastState = thisLevel.First.Value;
                    Finish();
                    return;
                }
                currentPuzzleState = thisLevel.First.Value;
                // Add this state to the list of visited states.
                visitedStates.AddLast(currentPuzzleState);
                // if moving single step command is prompted 
                // pause operation.
                if (singleStep == true)
                {
                    singleStep = false;
                    workingThread.Suspend();
                }
                // Get the successors.
                LinkedList<PuzzleState> successors = PuzzleState.Successors( thisLevel.First.Value);
                foreach (PuzzleState successor in successors)
                {
                    // If they are visited ignore, else...
                    if(IsVisited(successor) != true )
                    {
                        // Add them to the list of items to be visited at the next level.
                        nextLevel.AddFirst(successor);
                    }
                }
                // Remove current item.
                thisLevel.RemoveFirst();
                // If all items are processed in the current list,
                // proceed to the next level.
                if (thisLevel.Count == 0)
                {
                    thisLevel = nextLevel;
                    GC.Collect();
                    nextLevel = new LinkedList<PuzzleState>();
                }
            }
        }
    }
}
