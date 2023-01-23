using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ekeisMinesweeper
{
    /// <summary>
    /// Game Controller class handles creating the instances of gameboard and UI as well as subscribing events.
    /// </summary>
    internal class GameController
    {
        int boardSize = 50;
        int numMines = 1;

        GameBoard board;
        GameUI gameUI;

        // Creates gameboard and UI and subscribes events.
        internal GameController()
        {
            board = new GameBoard(boardSize, numMines);
            gameUI = new GameUI(boardSize);

            board.UpdateCell += gameUI.UpdateCellHandler;
            board.GameOver += gameUI.GameOverHandler;
            gameUI.ResetBoard += board.ResetBoard;
            _addCellClickedHandler();

            //board.DisplayBoard();

            Application.Run(gameUI);
        }

        // Subscribes handler to the cells in the UI.
        private void _addCellClickedHandler()
        {
            for (int row = 0; row < gameUI.Cells.GetLength(0); row++)
            {
                for (int col = 0; col < gameUI.Cells.GetLength(1); col++)
                {
                    gameUI.Cells[row, col].CellClicked += board.HandleCellClick;
                }
            }
        }
    }
}
