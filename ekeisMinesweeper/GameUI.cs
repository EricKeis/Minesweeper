using ekeisMinesweeper.CustomEventArgs;

namespace ekeisMinesweeper
{
    /// <summary>
    /// Minesweeper User Interface. This class handles ann UI logic for the minesweeper game.
    /// </summary>
    internal partial class GameUI : Form
    {
        Cell[,] cells;
        bool isTimerStarted = false;
        int timeElapsed = 0;
        int gamesPlayed = 0;
        int gamesWon = 0;
        int timePlayed = 0;

        internal event EventHandler ResetBoard;

        // Creates the minesweeper board and subscribes various events for UI elements.
        internal GameUI(int boardSize)
        {
            cells = new Cell[boardSize, boardSize];

            InitializeComponent();
            CreateGrid();

            this.Size = new Size(30 + boardSize * cells[0, 0].SizeOfCell, 90 + boardSize * cells[0, 0].SizeOfCell);

            gameTimer.Tick += _updateGameTimer;
            gameToolStripMenuItem.DropDownItemClicked += _menuBarClickedHandler;
            helpToolStripMenuItem.DropDownItemClicked += _menuBarClickedHandler;
        }

        internal Cell[,] Cells { get => cells; }

        // Creates the minesweeper board for the UI.
        internal void CreateGrid()
        {

            for (int row = 0; row < cells.GetLength(0); row++)
            {
                for (int col = 0; col < cells.GetLength(1); col++)
                {
                    cells[row, col] = new Cell(row, col);
                    cells[row, col].Location = new Point(5 + (col * cells[row, col].SizeOfCell), 25 + (row * cells[row, col].SizeOfCell));
                    this.Controls.Add(cells[row, col]);
                }
            }
        }

        // Enable or disable the buttons for all cells in the minesweeper board.
        private void _toggleAllCells(bool enabled)
        {
            for (int row = 0; row < cells.GetLength(0); row++)
            {
                for (int col = 0; col < cells.GetLength(1); col++)
                {
                    cells[row, col].Button.Enabled = false;
                }
            }
        }

        // Update the game timer label when the timer tick event fires.
        private void _updateGameTimer(object sender, EventArgs e)
        {
            timeElapsed++;

            gameTimerDisplay.Text = TimeSpan.FromSeconds(timeElapsed).ToString();
        }

        // Call appropriate methods when menu bar items are clicked.
        private void _menuBarClickedHandler(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.ToString().Equals("Exit"))
            {
                Application.Exit();
            }
            else if (e.ClickedItem.ToString().Equals("Restart"))
            {
                _restartGame();
            }
            else if (e.ClickedItem.ToString().Equals("Statistics"))
            {
                _showStatistics();
            }
            else if (e.ClickedItem.ToString().Equals("Instructions"))
            {
                _showInstructions();
            }
            else if (e.ClickedItem.ToString().Equals("About"))
            {
                _showAbout();
            }
        }

        // Show message box with game statistics.
        private void _showStatistics()
        {
            float winRatio = gamesWon > 0 ? (gamesWon / (float)gamesPlayed) * 100 : 0;
            int gameDuration = gamesPlayed > 0 ? timePlayed / gamesPlayed : 0;
            string message = $"Win Ratio: {winRatio}%\nAverage game duration: {TimeSpan.FromSeconds(gameDuration)}";

            MessageBox.Show(message, "Statistics", MessageBoxButtons.OK);
        }

        // Reset UI and gameboard.
        private void _restartGame()
        {
            this.ActiveControl = null;

            gameTimer.Stop();
            isTimerStarted = false;
            timeElapsed = 0;
            gameTimerDisplay.Text = "00:00:00";

            for (int row = 0; row < cells.GetLength(0); row++)
            {
                for (int col = 0; col < cells.GetLength(1); col++)
                {
                    cells[row, col].resetCell();
                }
            }

            OnResetBoard(new EventArgs());
        }

        // Show message box on how to play.
        private void _showInstructions()
        {
            string message = "Use the left click button on the mouse to select a space on the grid. " +
                "If you hit a bomb, you lose. The numbers on the board represent how many bombs are adjacent " +
                "to a square. For example, if a square has a \"3\" on it, then there are 3 bombs next to that square. " +
                "The bombs could be above, below, right left, or diagonal to the square. Avoid all the bombs and expose " +
                "all the empty spaces to win Minesweeper.";

            MessageBox.Show(message, "Instructions", MessageBoxButtons.OK);
        }

        // Show message box with information about the creator.
        private void _showAbout()
        {
            string message = "Coded by Eric Keis for CS3020";

            MessageBox.Show(message, "About", MessageBoxButtons.OK);
        }

        // Raise ResetBoard event if there are any subscribers.
        protected virtual void OnResetBoard(EventArgs e)
        {
            EventHandler resetBoard = ResetBoard;

            resetBoard?.Invoke(this, e);
        }

        // Update cell to show correct image based on gameboard data.
        internal void UpdateCellHandler(object sender, UpdateCellEventArgs e)
        {
            String fileName;

            if (e.IsMine)
            {
                fileName = "Mine";

                if (e.IsClicked)
                {
                    fileName += "_clicked";
                }
                else if (e.IsDefused)
                {
                    fileName += "_defused";
                }
            }
            else
            {
                fileName = "Minesweeper_" + e.CellValue;
            }
            cells[e.YPos, e.XPos].Button.Visible = false;

            Bitmap image = (Bitmap)Properties.Resources.ResourceManager.GetObject(fileName);
            Size size = new Size(cells[e.YPos, e.XPos].Size.Width, cells[e.YPos, e.XPos].Size.Height);
            cells[e.YPos, e.XPos].BackgroundImage = new Bitmap(image, size);

            if (!isTimerStarted)
            {
                isTimerStarted = true;
                gameTimer.Interval = 1000;
                gameTimer.Start();
            }
        }

        // Disable gameboard and update statistics when game is over.
        internal void GameOverHandler(object sender, GameOverEventArgs e)
        {
            _toggleAllCells(false);

            timePlayed += timeElapsed;
            gameTimer.Stop();
            isTimerStarted = false;
            timeElapsed = 0;
            winStatus.Text = e.IsWinner ? "You won!" : "You lost!";
            winStatus.Visible = true;

            gamesPlayed++;
            if (e.IsWinner)
            {
                gamesWon++;
            }
        }
    }
}