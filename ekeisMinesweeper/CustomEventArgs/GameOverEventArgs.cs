using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ekeisMinesweeper.CustomEventArgs
{
    /// <summary>
    /// Data to pass on game over event.
    /// </summary>
    internal class GameOverEventArgs : EventArgs
    {
        bool isWinner;

        public GameOverEventArgs(bool isWinner)
        {
            this.isWinner = isWinner;
        }

        public bool IsWinner { get => isWinner; }
    }
}
