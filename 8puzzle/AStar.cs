using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace _8puzzle
{
    class AStar : ISolution
    {   
        // Iterative deepening solution class
        // The method uses the functions of ISolution base class
        // for basic functions and only implements the solution.

        // Enumeration for Heuristic types
        public enum Heuristic
        { ManhattanDistance, MisplacedTiles } ;
        // Variable to keep track of which heuristic is selected
        private Heuristic selectedHeuristic;

        // A struct of priority queue members to keep
        // the state, the heuristic, and cost of getting there.
        private struct pqMember 
        {
            public int heuristic;
            public int costOfGettingToState;
            public PuzzleState state;
        };

        // Constructor function to set up appropriate variables.
        public AStar(PuzzleState state, Heuristic selectedHeuristic)
        {
            this.startState = state;
            this.selectedHeuristic = selectedHeuristic;
            workingThread = new Thread(new ThreadStart(Run));
            visitedStates = new LinkedList<PuzzleState>();
        }
        private int CalculateHeuristic(PuzzleState state, int costOfGettingHere)
        {
            // Choose the appropriate heuristic according to selected method
            int heuristic = (selectedHeuristic == Heuristic.ManhattanDistance) ?
                PuzzleState.HeuristicManhattan(state) : PuzzleState.HeuristicMisplaced(state);
            return heuristic + costOfGettingHere + 1;
        }
        
        // Method for A* solution
        // The method keeps a priority list and choses the minimum heuristic
        // state in the list and adds its successors with new heuristics.
        private void Run()
        {
            pqMember myInitialMemeber = new pqMember();
            LinkedList <pqMember> processingQueue = new LinkedList<pqMember>();
            // initial state with cost of getting there = 0 and with initial state
            myInitialMemeber.state = startState;
            myInitialMemeber.heuristic = CalculateHeuristic(startState, 0);
            myInitialMemeber.costOfGettingToState = 0;
            processingQueue.AddFirst(myInitialMemeber);
            while (true)
            {
                // Pick the minimum heuristic member in the list.
                pqMember minHeuristic = new pqMember();
                minHeuristic.heuristic = Int32.MaxValue;
                
                foreach (pqMember member in processingQueue)
                {
                    if (member.heuristic< minHeuristic.heuristic)
                        minHeuristic = member;
                }
                
                processingQueue.Remove(minHeuristic);
           
                currentPuzzleState = minHeuristic.state;

                // if moving single step command is prompted 
                // pause operation.
                if (singleStep == true)
                {
                    singleStep = false;
                    workingThread.Suspend();
                }

                // It the minimum-heuristic state is the goal, finish.
                if (PuzzleState.IsGoal(minHeuristic.state))
                {
                    this.lastState = minHeuristic.state;
                    Finish();
                    return;
                }
                // Add it to the list of visited states.
                if (IsVisited(minHeuristic.state) == false)
                    visitedStates.AddLast(minHeuristic.state);

                // Get its succeesors.
                LinkedList<PuzzleState> successors = PuzzleState.Successors(minHeuristic.state);
                foreach (PuzzleState successor in successors)
                {
                    // If they are not visited before
                    if (IsVisited(successor) == false)
                    {
                        // append them to the end of the priority queue
                        visitedStates.AddLast(successor);
                        pqMember temp = new pqMember();
                        temp.state = successor;
                        temp.heuristic=CalculateHeuristic(successor,minHeuristic.costOfGettingToState);
                        temp.costOfGettingToState = minHeuristic.costOfGettingToState + 1;
                        processingQueue.AddFirst(temp);
                    }
                }
            }

        }


    }
}
