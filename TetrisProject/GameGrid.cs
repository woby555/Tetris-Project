using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace TetrisProject
{
    /// <summary>
    /// Describes the dimensions of the game grid where all Tetris pieces will be dropped.
    /// </summary>
    public class GameGrid
    {
        private readonly int[,] grid;
        public int Rows { get; }
        public int Columns { get; }

        /// <summary>
        /// Index accessor. 
        /// </summary>
        /// <param name="r"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public int this[int r, int c] 
        {
            get => grid[r, c];
            set => grid[r, c] = value;
        }

        /// <summary>
        /// Constructs a game grid for the game.
        /// </summary>
        /// <param name="rows">The number of rows for the grid.</param>
        /// <param name="columns">The number of columns.</param>
        public GameGrid (int rows, int columns)
        {
            this.Rows = rows;
            this.Columns = columns;
            grid = new int[ rows, columns ];
        }

        /// <summary>
        /// Checks if a given row or column is within the GameGrid. 
        /// </summary>
        public bool IsInside (int r, int c)
        {
            return r >= 0 && r < Rows && c >= 0 && c < Columns;
        }

        /// <summary>
        /// Checks if a given cell is empty or not.
        /// </summary>
        public bool IsEmpty (int r , int c)
        {
            return IsInside(r,c) && grid [r, c] == 0;
        }

        /// <summary>
        /// Checks if a row of cells in the GameGrid is full.
        /// </summary>
        public bool IsRowFull(int r)
        {
            for (int c = 0; c < Columns; c++) 
            {
                if (grid[r, c] == 0)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Checks if a row of cells in the GameGrid is empty.
        /// </summary>
        public bool IsRowEmpty(int r)
        {
            for (int c = 0; c < Columns; c++)
            {
                if (grid[r, c] != 0)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Clears a row of cells.
        /// </summary>
        public void ClearRow (int r)
        {
            for( int c = 0;c < Columns;c++)
            {
                grid[r, c] = 0;
            }
        }

        /// <summary>
        /// Moves a row of cells down.
        /// </summary>
        public void MoveRowDown (int r, int numRows)
        {
            for (int c = 0; c < Columns; c++)
            {
                grid[r + numRows, c] = grid[r, c];
                grid[r,c] = 0;
            }
        }

        /// <summary>
        /// Clears all the full rows in the GameGrid and moves down any rows not full.
        /// </summary>
        public int ClearFullRows()
        {
            int cleared = 0;
            for (int r = Rows-1; r >= 0; r--)
            {
                if (IsRowFull(r))
                {
                    ClearRow(r);
                    cleared++;
                }
                else if (cleared > 0)
                {
                    MoveRowDown(r, cleared);
                }
            }

            return cleared;
        }
    }
}
