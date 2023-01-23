using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ekeisMinesweeper.CustomEventArgs
{
    /// <summary>
    /// Data to pass on update cell event.
    /// </summary>
    internal class UpdateCellEventArgs : EventArgs
    {
        int xPos;
        int yPos;
        char cellValue;
        bool isMine = false;
        bool isDefused = false;
        bool isClicked = false;

        internal UpdateCellEventArgs(int row, int col, char cellValue)
        {
            this.xPos = col;
            this.yPos = row;
            this.cellValue = cellValue;
        }

        internal UpdateCellEventArgs(int row, int col, char cellValue, bool isMine, bool isDefused) : this(row, col, cellValue)
        {
            this.isMine = isMine;
            this.isDefused = isDefused;
        }

        public bool IsMine { get => isMine; set => isMine = value; }
        public bool IsDefused { get => isDefused; set => isDefused = value; }
        public bool IsClicked { get => isClicked; set => isClicked = value; }
        internal int XPos { get => xPos; }
        internal int YPos { get => yPos; }
        internal char CellValue { get => cellValue; }
    }
}
