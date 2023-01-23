using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ekeisMinesweeper.CustomEventArgs;

namespace ekeisMinesweeper
{
    /// <summary>
    /// Minesweeper class to handle all game data and logic.
    /// </summary>
    internal class GameBoard
    {
        char[,] board;
        bool _isBoardGenerated;
        int numMines;
        int flaggedMines;

        List<(int row, int col)> mines;

        internal event EventHandler<UpdateCellEventArgs> UpdateCell;
        internal event EventHandler<GameOverEventArgs> GameOver;

        // Initialize gameboard and default values.
        internal GameBoard(int boardSize, int numMines)
        {
            board = new char[boardSize, boardSize];
            this.numMines = numMines;
            flaggedMines = 0;
            _isBoardGenerated = false;

            mines = new List<(int row, int col)>();
        }

        // Generate mines on game board with valid locations.
        private void _generateMines(int numMines, CellClickedEventArgs e)
        {
            int minesGenerated = 0;
            Random random = new Random((int)DateTimeOffset.Now.ToUnixTimeMilliseconds());

            while (minesGenerated < numMines)
            {
                int nextRow = random.Next(board.GetLength(1));
                int nextCol = random.Next(board.GetLength(0));

                if (board[nextRow, nextCol] != 'M' && nextCol != e.Col && nextRow != e.Row)
                {
                    board[nextRow, nextCol] = 'M';
                    mines.Add((nextRow, nextCol));
                    minesGenerated++;
                }
            }
        }

        // Reveals cells using DFS until there are no more cells to search.
        private void _revealCells(int row, int col)
        {
            int[] _rowOffset = { -1, -1, -1, 0, 1, 1, 1, 0 };
            int[] _colOffset = { -1, 0, 1, 1, 1, 0, -1, -1 };

            if (_isMine(row, col))
            {
                _revealMines(row, col);
                OnGameOver(new GameOverEventArgs(false));
            }
            else
            {
                int mines = _calculateAdjacentMines(row, col, _rowOffset, _colOffset);

                board[row, col] = mines.ToString()[0];
                OnUpdateCell(new UpdateCellEventArgs(row, col, board[row, col]));

                if (mines == 0)
                {
                    _searchAdjacentCells(row, col, _rowOffset, _colOffset);
                }
            }
        }

        // Reveal all mine locations and raise event for UI to handle.
        private void _revealMines(int row, int col)
        {
            foreach (var _mine in mines)
            {
                bool isDefused = board[_mine.row, _mine.col] == 'D';
                UpdateCellEventArgs args = new UpdateCellEventArgs(_mine.row, _mine.col, board[_mine.row, _mine.col], true, isDefused);

                if (_mine.row == row && _mine.col == col)
                {
                    args.IsClicked = true;
                }

                OnUpdateCell(args);
            }
        }

        // Checks if adjacent cells are valid locations, then calls revealCells method to continue DFS.
        private void _searchAdjacentCells(int row, int col, int[] _rowOffset, int[] _colOffset)
        {
            for (int i = 0; i < 8; i++)
            {
                int nextCol = col + _colOffset[i];
                int nextRow = row + _rowOffset[i];

                if (_isValid(nextRow, nextCol) && board[nextRow, nextCol] == '\0')
                {
                    _revealCells(nextRow, nextCol);
                }
            }
        }

        // Sums up mines adjacent to the incoming cell and returns total.
        private int _calculateAdjacentMines(int row, int col, int[] _rowOffset, int[] _colOffset)
        {
            int mines = 0;
            for (int i = 0; i < 8; i++)
            {
                int nextCol = col + _colOffset[i];
                int nextRow = row + _rowOffset[i];

                if (_isValid(nextRow, nextCol) && _isMine(nextRow, nextCol))
                {
                    mines++;
                }
            }
            return mines;
        }

        // Checks if cell is a mine.
        private bool _isMine(int row, int col)
        {
            return board[row, col] == 'M' || board[row, col] == 'D';
        }

        // Check is cell is not out of bounds.
        private bool _isValid(int row, int col)
        {
            return col >= 0 && row >= 0 && col < board.GetLength(0) && row < board.GetLength(1);
        }

        // Updates the cell on the board to respresent being flagged.
        private void _toggleFlagOnCell(int row, int col)
        {
            switch (board[row, col])
            {
                case 'M':
                    board[row, col] = 'D';
                    flaggedMines++;
                    if (_hasFoundAllMines())
                    {
                        OnGameOver(new GameOverEventArgs(true));
                    }
                    break;
                case 'F':
                    board[row, col] = '\0';
                    break;
                case 'D':
                    board[row, col] = 'M';
                    flaggedMines--;
                    break;
                case '\0':
                    board[row, col] = 'F';
                    break;
            }
        }

        // Checks if all mines have been located.
        private bool _hasFoundAllMines()
        {
            return flaggedMines == numMines;
        }

        // Reset all cells back to their default value.
        private void _clearCells()
        {
            for (int row = 0; row < board.GetLength(0); row++)
            {
                for (int col = 0; col < board.GetLength(1); col++)
                {
                    board[row, col] = '\0';
                }
            }
        }

        // Display board to console if it is enabled.
        internal void DisplayBoard()
        {
            for (int row = 0; row < board.GetLength(0); row++)
            {
                for (int col = 0; col < board.GetLength(1); col++)
                {
                    Console.Write($"[{(board[row, col] == '\0'  ? ' ' : board[row, col])}]");
                }
                Console.WriteLine();
            }
        }

        // Searches adjacent cells or flags a cell based on incoming click value.
        internal void HandleCellClick(object sender, CellClickedEventArgs e)
        {
            if (!_isBoardGenerated)
            {
                _generateMines(numMines, e);
                _isBoardGenerated = true;
            }

            if (e.ClickType.Equals("Left"))
            {
                _revealCells(e.Row, e.Col);
            }
            else if(e.ClickType.Equals("Right"))
            {
                _toggleFlagOnCell(e.Row, e.Col);
            }
            //DisplayBoard();
        }

        // Resets all board values back to their original values.
        internal void ResetBoard(object sender, EventArgs e)
        {
            _isBoardGenerated = false;
            flaggedMines = 0;
            mines = new List<(int row, int col)>();
            _clearCells();
        }

        // Raises UpdateCell Event.
        protected virtual void OnUpdateCell(UpdateCellEventArgs e)
        {
            EventHandler<UpdateCellEventArgs> updateCell = UpdateCell;

            updateCell?.Invoke(this, e);
        }

        // Raises GameOver Event.
        protected virtual void OnGameOver(GameOverEventArgs e)
        {
            EventHandler<GameOverEventArgs> gameOver = GameOver;

            gameOver?.Invoke(this, e);
        }
    }
}
