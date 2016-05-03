using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace _8puzzle
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        const int NUM_TEXTBOXES = 5;
        private int currentSize = 3;
        private int[] stateArray;
        private PuzzleState currentPuzzle;
        LinkedList<PuzzleState> solvedPuzzle;
        ISolution currentSolution;
        bool paused = false;
        bool finished = false;
        int numberOfRuns = 0; 
        double totalNumberOfExploredNodes = 0;
        DateTime startTime;
        // Method for keeping text boxes in a matrix for procedural access.
        // Inspired from http://stackoverflow.com/questions/1762721/resolved-puting-textboxes-in-to-a-textbox-array
        TextBox[,] displayMatrix = new TextBox[NUM_TEXTBOXES, NUM_TEXTBOXES];
        private void TextBoxesToDisplayMatrix()
        {
            for (int i = 0; i < NUM_TEXTBOXES; i++)
            {
                for (int j = 0; j < NUM_TEXTBOXES; j++)
                {
                    displayMatrix[i, j] = (TextBox)this.gameStateGroupBox.Controls["textBox" + i.ToString() + j.ToString()];
                    //MessageBox.Show(displayMatrix[i, j].ToString());
                }
            }
        }

        //On loading create a matrix of text boxes
        private void Form1_Load(object sender, EventArgs e)
        {
            TextBoxesToDisplayMatrix();
        }

        // Enable and disable text boxes depending on the selected size.
        private void GameSizeRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            // Set size w.r.t. selected radiobutton
            if (sender == gameSize3RadioButton)
            { currentSize = 3; }
            else if (sender == gameSize4RadioButton)
            { currentSize = 4; }
            else if (sender == gameSize5RadioButton)
            { currentSize = 5; }
            else if (sender == gameSize7RadioButton)
            { currentSize = 7; }
            // Enable or disable textboxes
            for (int i = 0; i < NUM_TEXTBOXES; i++)
            {
                for (int j = 0; j < NUM_TEXTBOXES; j++)
                {
                    if (i < currentSize && j < currentSize)
                        displayMatrix[i, j].Enabled = true;
                    else
                        displayMatrix[i, j].Enabled = false;

                }
            }
            
        }
       
        // Method for displaying the contents of an array in the matrix of text boxes
        private void DisplayContents(int[] arrayToBeDisplayed)
        {
            for (int i = 0; i < currentSize; i++)
            {
                for (int j = 0; j < currentSize; j++)
                {
                    if (arrayToBeDisplayed[j * currentSize + i] == currentSize * currentSize)
                        displayMatrix[i, j].Text = String.Empty;
                    else
                        displayMatrix[i, j].Text = arrayToBeDisplayed[j * currentSize + i].ToString() ;
                }
            }
        }

        // Method for converting the values in the matrix of text boxes to an array
        // which can be solved by the program.
        private int[] DisplayToArray()
        {
            try
            {
                int[] tempArray = new int[currentSize * currentSize];
                for (int i = 0; i < currentSize; i++)
                {
                    for (int j = 0; j < currentSize; j++)
                    {
                        // if movable element
                        if (displayMatrix[i, j].Text == String.Empty)
                            tempArray[j * currentSize + i] = currentSize * currentSize;
                        else
                        // Parse numbers
                            tempArray[j * currentSize + i] = Int32.Parse(displayMatrix[i, j].Text);
                        // if value entered is out of range
                        if (tempArray[j * currentSize + i] > currentSize * currentSize)
                        {
                            MessageBox.Show("Error!, Please check the numbers you entered");
                        }

                    }
                }
                return tempArray;

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return null;
            }
 
        }

        // Method for creating a new game.
        private void NewGameButton_Click(object sender, EventArgs e)
        {
            if (sender == randomButton)
            {   
                // If random button is pressed create a random game
                currentPuzzle = PuzzleState.RandomState(currentSize);
                DisplayContents(currentPuzzle.Placement);
            }
            else if (sender == manualButton)
            {
                // If manual button is pressed convert the values in the display
                // to a puzzle state and create a puzzle state.
                currentPuzzle = new PuzzleState(DisplayToArray(),currentSize,true) ;
                DisplayContents(currentPuzzle.Placement);
            }
            else if (sender == stepsFromSolutionButton)
            {
                // If steps from soulution button is pressed create a game 
                // with indicated steps from the solution
                currentPuzzle = PuzzleState.RandomStepsFromGoalState(currentSize, 
                    Decimal.ToInt32(stepsFromSolutionNumericUpDown.Value));
                if (currentSize<=5)
                    DisplayContents(currentPuzzle.Placement);
            }
            // Remove any possible remnants from the previous game.
            GC.Collect();
            
            solutionGroupBox.Enabled = true;

        }

        // Method for controlling the current solution 
        private void ControlButton_Click(object sender, EventArgs e)
        {
            if (sender == startButton)
            {
                // Select the  solution method.
                // TODO : needs to be converted to a switch- case structure
                if (depthFirstRadioButton.Checked == true)
                {
                    currentSolution = new DepthFirst(currentPuzzle);
                }
                else if (breadthFirstRadioButton.Checked == true)
                {
                    currentSolution = new BreadthFirst(currentPuzzle);
                }
                else if (iterativeDeepeningRadioButton.Checked == true)
                {
                    currentSolution = new IterativeDeepening(currentPuzzle);
                }
                else if (aStarManhattanRadioButton.Checked == true)
                {
                    currentSolution = new AStar(currentPuzzle, AStar.Heuristic.ManhattanDistance);
                }
                else if (aStarMisplacedRadioButton.Checked == true)
                {
                    currentSolution = new AStar(currentPuzzle, AStar.Heuristic.MisplacedTiles);
                }
                // Destroy any remaining structures from previous puzzle(s)
                GC.Collect();
                // If start button is pressed start solution.
                if (currentPuzzle == null)
                { NewGameButton_Click(manualButton,e); }
                
                if (currentSolution != null)
                {
                    
                    currentSolution.Start();
                    paused = false;
                    pauseButton.Text = "Pause";
                    timer1.Enabled = true;
                    startTime = DateTime.Now;
                }
                else
                {
                    MessageBox.Show("Could start is a puzzle selected?");
                }
            }
            else if (sender == stopButton)
            {
                // If stop button is pressed stop the solution.
                if (currentSolution != null)
                {
                    currentSolution.Stop();
                    paused = false;
                    pauseButton.Text = "Pause";
                }
            }
            else if (sender == pauseButton)
            {
                // If pause/continue button is pressed pause or continue
                // depending on the situation of the solution.
                if (currentSolution != null)
                {
                    if (paused == false)
                    {
                        currentSolution.Pause();
                        paused = true;
                        pauseButton.Text = "Continue";
                    }
                    else
                    {
                        currentSolution.Continue();
                        paused = false;
                        pauseButton.Text = "Pause";
                    }
                }
            }
            else if (sender == singleStepButton)
            {
                // Prompt execution of a single step
                if (currentSolution != null)
                {
                    currentSolution.SingleStep();
                    timer1.Enabled = true;
                }
            }
        }

        // timer1 allways checks the state of the solution and updates displays
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (currentSolution != null)
            {
                // Update displays
                currentStatusTextBox.Text = currentSolution.GetCurrentState();
                expandedNodesTextBox.Text = currentSolution.GetExpandedNodes().ToString();
                if (displayStatesCheckBox.Checked == true)
                {
                    DisplayContents(currentSolution.CurrentPuzzleState.Placement);
                }
                // if solution has been found 
                if (currentSolution.GetCurrentState() == "Finished")
                {
                    timePassedTextBox.Text = (DateTime.Now - startTime).TotalMilliseconds.ToString();
                    // disable timer1
                    timer1.Enabled = false;
                    // get the solution
                    solvedPuzzle = currentSolution.GetSolution();
                    // uptade the controls related to displaying the solution.
                    numberOfStepsTextBox.Text = solvedPuzzle.Count.ToString();
                    nextStepButton.Enabled = true;
                    playButton.Enabled = true;
                    solutionGroupBox.Enabled = false;
                    finished = true;
                }
 
            }
        }

        // when one of the steps from solution text boxes is changed equilize to the other.
        private void stepsFromSolutionNumericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            if (sender == stepsFromSolutionNumericUpDown)
                stepsFromSolutionNumericUpDown2.Value = stepsFromSolutionNumericUpDown.Value;
            else
                stepsFromSolutionNumericUpDown.Value = stepsFromSolutionNumericUpDown2.Value;
        }

        // for displaying solution
        private void DisplaySolutionButton_Click(object sender, EventArgs e)
        {
            if (sender == nextStepButton)
            {
                // if next step button is pressed move to the next state
                // in the solution.
                solvedPuzzle.AddLast(solvedPuzzle.First());
                solvedPuzzle.RemoveFirst();
                DisplayContents(solvedPuzzle.First.Value.Placement);

            }
            else if (sender == previousStepButton)
            {
                // if previous step button is pressed move to the next previous
                // in the solution.
                solvedPuzzle.AddFirst(solvedPuzzle.Last());
                solvedPuzzle.RemoveLast();
                DisplayContents(solvedPuzzle.First.Value.Placement);

            }
            else if (sender == playButton)
            {
                // if play button is pressed enable solutionPlay timer that
                // displays the steps sequentially.
                solutionPlayTimer.Enabled = true;
            }

            if (PuzzleState.IsGoal(solvedPuzzle.First()))
            {
                nextStepButton.Enabled = false;
            }
            else
            {
                nextStepButton.Enabled = true;
            }
            if (PuzzleState.IsGoal(solvedPuzzle.Last()))
            {
                previousStepButton.Enabled = false;
            }
            else 
            {
                previousStepButton.Enabled = true;
            }

        }

        // solutionPlayTimer plays the contents of the solution when activated
        // and disables when reached to the end (goal) state.
        private void solutionPlayTimer_Tick(object sender, EventArgs e)
        {
            if (nextStepButton.Enabled == true)
            {
                DisplaySolutionButton_Click(nextStepButton, e);
            }
            else
            {
                solutionPlayTimer.Enabled = false;
            }
        }

        private void runsNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            solutionGroupBox.Enabled = true;
        }

        private void monteCarloButton_Click(object sender, EventArgs e)
        {
            // Set variables up for monte carlo simulation
            numberOfRuns = 0; 
            totalNumberOfExploredNodes = 0;
            monteCarloResultTextBox.Text = String.Empty;
            finished = true;
            // Enable the timer that will control the simulation
            monteCarloTimer.Enabled = true;

            
        }

        private void monteCarloTimer_Tick(object sender, EventArgs e)
        {
            if (finished == true)
            {
                // if optimal solution is closer to goal then desired step number  ignore the solution
                if (solvedPuzzle != null && solvedPuzzle.Count >= stepsFromSolutionNumericUpDown.Value + 1)
                {

                    if (numberOfRuns++ == Decimal.ToInt32(runsNumericUpDown.Value))
                    {
                        monteCarloResultTextBox.Text = "Average: " + totalNumberOfExploredNodes / numberOfRuns;
                        monteCarloTimer.Enabled = false;
                        return;
                    }

                    if (numberOfRuns == 1)
                        totalNumberOfExploredNodes = 0;
                    else
                        totalNumberOfExploredNodes += currentSolution.GetExpandedNodes();
                }
                // Create a new game with n steps from solution
                NewGameButton_Click(stepsFromSolutionButton, e);
                // Start solution
                ControlButton_Click(startButton, e);
                finished = false;
            }
            else
            {
                monteCarloResultTextBox.Text = "Iteration: " +  numberOfRuns;
            }

        }



    }
}
