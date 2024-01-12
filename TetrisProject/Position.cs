using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TetrisProject
{
    /// <summary>
    /// Describes the position or cell in the GameGrid
    /// </summary>
    public class Position
    {
        public int Row { get; set; }
        public int Column { get; set; }

        public Position (int row, int column)
        {
            Row = row;
            Column = column;
        }   


    }
}
