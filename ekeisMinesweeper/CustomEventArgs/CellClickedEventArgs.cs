using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ekeisMinesweeper.CustomEventArgs
{
    /// <summary>
    /// Data to pass on cell clicked event.
    /// </summary>
    public class CellClickedEventArgs : EventArgs
    {
        int col;
        int row;
        String _clickType;

        internal CellClickedEventArgs(int row, int col)
        {
            this.col = col;
            this.row = row;
        }

        internal int Col { get => col; }
        internal int Row { get => row; }
        internal string ClickType { get => _clickType; set => _clickType = value; }
    }
}
