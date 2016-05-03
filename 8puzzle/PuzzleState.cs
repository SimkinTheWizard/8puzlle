using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace _8puzzle
{
   
    // This is the class for representation of the current puzzle state.
    class PuzzleState
    {
        // The 1-dimensional array for keeping the placement of the tiles.
        private int[] placement;

        // This is the size of the state 3,4, or 5. (for 3 by 3, 4 by 4, and 5 by 5 respectively)
        private int size = 3;

        // Movable element is represented as the highest vale e.g.:9 for 3 by 3.
        private int movableElement = 3 * 3;

        // Indicates whether this is a start state or not.
        // Also parent of the start state will be null.
        private bool isStartState = false;

        // Whose successor this state is.
        private PuzzleState parent;

        // Constructor for for creating successor state classes
        // this method is private and can be only called from another puzzle state.
        private PuzzleState(int[] placement, int size, PuzzleState parent)
        {
            this.placement = placement;
            this.size = size;
            movableElement = size * size;
            this.parent = parent;
            if (parent == null)
                isStartState = true;
        }
        // This is default constructor for creating a new game.
        public PuzzleState(int[] placement, int size, bool isStartState)
        {
            this.placement = placement;
            this.size = size;
            movableElement = size * size;
            this.isStartState = isStartState;
        }

        // Property for accessing the placement: read-only.
        public int[] Placement
        {
            get { return placement; }
        }
        // Property for accessing the size: read-only. 
        //Only can be modified from the constructor.
        public int Size
        {
            get { return size; }
        }
        // Property for returning the parent state: read-only.
        public PuzzleState Parent
        {
            get { return parent; }
        }

        // This is a sub-routine for Successors function. Creates arrays
        // by swapping given two elements.
        private static int[] SwapElements(PuzzleState state, int index1, int index2) 
        {
            // Allocate an array with appropriate size. 
            int size = state.Size;
            int[] newArray = new int[size * size];
            // Copy the placement of the previous state.
            Array.Copy(state.Placement, newArray, size * size);
            // Replace the contents of the array.
            newArray[index1] = state.Placement[index2];
            newArray[index2] = state.Placement[index1];
            return newArray;
        }
        public static LinkedList<PuzzleState> Successors(PuzzleState state)
        {
            int size = state.Size;
            int movableElement = size * size; 
            // Create a list of legal moves.
            LinkedList<PuzzleState> legalMoves = new LinkedList<PuzzleState>();
            // locate the movable element
            int x = 0;
            int y = 0;
            for (int xpos = 0; xpos<size; xpos++)
                for (int ypos = 0; ypos < size; ypos++)
                {
                    if (state.Placement[ypos * size + xpos] == movableElement)
                    {
                        x = xpos;
                        y = ypos;
                    }
                }
            // If movable element isn't in the firt row create a new parent where it
            // moves up one row.
            if (x > 0)
            { legalMoves.AddFirst(new PuzzleState(SwapElements(state, y * size + x, y * size + x - 1), size, state)); }
            // If movable element isn't in the last row create a new parent where it
            // moves down one row.
            if (x<size-1)
            { legalMoves.AddFirst(new PuzzleState(SwapElements(state, y * size + x, y * size + x + 1), size, state)); }
            // Apply same for the columns.
            if (y > 0)
            { legalMoves.AddFirst(new PuzzleState(SwapElements(state, y * size + x, (y - 1) * size + x), size, state)); }
            if (y < size - 1)
            { legalMoves.AddFirst(new PuzzleState(SwapElements(state, y * size + x, (y + 1) * size + x), size, state)); }
            return legalMoves;
        }

        // Overriden Equals function for checking whether both states are equal.
        // returns true if their placements are the same.
        public override bool Equals(object obj)
        {
            PuzzleState toBeCompared = (PuzzleState)obj;
            bool equals = true;
            for (int i = 0; i < size * size - 1; i++)
            {
                // if any element is different
                // this is not the same state
                if (placement[i] != toBeCompared.Placement[i])
                {
                    equals = false;
                    break;
                }
            }
            return equals;

        }
        // Manhattan difference heuristic
        public static int HeuristicManhattan(PuzzleState state)
        {
            int manhattan = 0;
            for (int i = 0; i < state.Size * state.Size; i++)
            {
                // X-difference from its final position.
                manhattan += Math.Abs(((state.Placement[i] - 1) % state.Size - i % state.Size));
                // Y-difference
                manhattan += Math.Abs(((state.Placement[i] - 1) - (state.Placement[i] - 1) % state.Size) / state.Size -
                               ((i - i % state.Size) / state.Size));
            }
            return manhattan;
        }
        // Number of misplaced tiles heuristic.
        public static int HeuristicMisplaced(PuzzleState state)
        {
            int misplaced = 0;
            for (int i = 0; i < state.Size * state.Size - 1; i++)
            {
                // if any element is out of its place
                if (state.Placement[i] != i + 1)
                {
                    misplaced ++;
                }
            }
            return misplaced;
        }

        // Method for checking whether a state is the goal state
        public static bool IsGoal(PuzzleState state)
        {
            for (int i = 0; i < state.Size * state.Size - 1; i++)
            {
                // if any element is out of its place
                // this is not the goal state
                if (state.Placement[i] != i + 1)
                {
                    return false;
                }
            }
            return true;
        }
        // Public method for creating goal state
        // This is used for generating a random puzzle.
        public static PuzzleState GoalState(int size)
        {
            int[] placement = new int[size * size];
            for (int i = 0; i < size * size ; i++)
            {
                placement[i] = i + 1;
            }
            return new PuzzleState(placement, size, false);
        }
        // Public method for creating a random state.
        public static PuzzleState RandomState(int size)
        {
            Random randomGenerator = new Random();
            int[] stateArray = new int[size * size];
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    // Generate a random number
                    int currentNumber = 1;
                    // Check whether this value is previously used
                    // and change if necessary
                    bool existsInArray = true;
                    while (existsInArray == true)
                    {
                        currentNumber = randomGenerator.Next(1, size * size + 1);
                        existsInArray = false;
                        for (int k = 0; k < i * size + j; k++)
                        {
                            if (stateArray[k] == currentNumber)
                            {
                                existsInArray = true;
                                break;
                            }

                        }
                    } // End while
                    stateArray[i * size + j] = currentNumber;
                }// End for
            }// End for
            return new PuzzleState(stateArray, size, true);
        }
        // This method creates a random problem with a specific distance 
        // from the goal state.
        public static PuzzleState RandomStepsFromGoalState(int size, int steps)
        {
            // Create the goal state.
            PuzzleState goal = GoalState(size);
            // Keep a list of moves so that same state is not generated twice.
            LinkedList<PuzzleState> previousSteps = new LinkedList<PuzzleState>();
            // Allocate a random generator.
            Random stepGeneratorRandom = new Random();
            // Add goal to the list.
            previousSteps.AddLast(goal);
            for (int i = 0; i < steps; i++)
            {
                PuzzleState current = previousSteps.Last();
                // Get the list of possible moves
                LinkedList<PuzzleState> successors = PuzzleState.Successors(current);
                PuzzleState[] successorsArray = successors.ToArray<PuzzleState>();
                PuzzleState temp = null;
                bool exists = true;
                while (exists == true)
                {
                    // select one randomly
                    int selectedIndex = stepGeneratorRandom.Next(successors.Count);
                    temp = successorsArray[selectedIndex];
                    // then check whether this state is generated before.
                    exists = false;
                    foreach (PuzzleState successor in previousSteps)
                    {
                        if (successor.Equals(temp))
                            exists = true;
                    }
                    
                }
                previousSteps.AddLast(temp);
            }
            return new PuzzleState( previousSteps.Last().Placement,size,true);
        }



    }

    
}
