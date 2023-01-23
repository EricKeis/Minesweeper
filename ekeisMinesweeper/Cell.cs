using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ekeisMinesweeper.CustomEventArgs;

namespace ekeisMinesweeper
{
    /// <summary>
    /// Custom Cell user control for creating minesweeper board.
    /// </summary>
    internal partial class Cell : UserControl
    {
        Button button = new Button();
        int sizeOfCell = Screen.PrimaryScreen.Bounds.Width / 90;
        int col;
        int row;

        internal event EventHandler<CellClickedEventArgs> CellClicked;

        /// <summary>
        /// Creates a custom cell control.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        internal Cell(int row, int col)
        {
            this.row = row;
            this.col = col;
            InitializeComponent();
            this.Size = new Size(SizeOfCell, SizeOfCell);
            this.BackgroundImage = _loadImage("Minesweeper_0");
            this.BackColor = Color.Red;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.Size = new Size(SizeOfCell, sizeOfCell);
            button.MouseDown += _buttonClickHandler;
            button.Location = new Point(0, 0);
            button.BackColor = Color.LightGray;
            button.BackgroundImage = _loadImage("Minesweeper_unopened_square");
            button.Tag = "Minesweeper_unopened_square";
            this.Controls.Add(button);
        }

        // Reset a cell to its beginning state.
        internal void resetCell()
        {
            this.BackgroundImage = _loadImage("Minesweeper_0");
            button.BackgroundImage = _loadImage("Minesweeper_unopened_square");
            button.Tag = "Minesweeper_unopened_square";
            button.Visible = true;
            button.Enabled = true;
        }

        // Load an image from resources and resize it to the size of the control.
        private Bitmap _loadImage(String imgName)
        {
            Size size = new Size(Size.Width, Size.Height);
            Bitmap image = (Bitmap)Properties.Resources.ResourceManager.GetObject(imgName);
            return new Bitmap(image, size);
        }

        // Calls the OnCellClicked with the correct event args.
        private void _buttonClickHandler(Object sender, MouseEventArgs e)
        {
            CellClickedEventArgs args = new CellClickedEventArgs(row, col);

            if (e.Button == MouseButtons.Left)
            {
                args.ClickType = "Left";
                OnCellClicked(args);
            }
            else if(button.Tag.Equals("Minesweeper_unopened_square"))
            {
                button.BackgroundImage = _loadImage("Minesweeper_flag");
                button.Tag = "Minesweeper_flag";

                args.ClickType = "Right";
                OnCellClicked(args);
            }
            else
            {
                button.BackgroundImage = _loadImage("Minesweeper_unopened_square");
                button.Tag = "Minesweeper_unopened_square";

                args.ClickType = "Right";
                OnCellClicked(args);
            }
        }

        // Raises Cell clicked event if something is listening.
        protected virtual void OnCellClicked(CellClickedEventArgs e)
        {
            EventHandler<CellClickedEventArgs> cellClicked = CellClicked;

            if (cellClicked != null)
            {
                CellClicked(this, e);
            }
        }

        internal int SizeOfCell { get => sizeOfCell; }
        internal Button Button { get => button; }
    }
}
