using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TetrisProject.Blocks
{
    /// <summary>
    /// Describes the various Blocks in Tetris and their movements within the game.
    /// </summary>
    public abstract class Block
    {
        //Starting decision of the piece.
        protected abstract Position[][] Tiles { get; }

        //Decides the starting offset position of the piece.
        protected abstract Position StartOffset { get; }

        ///Identifier for the 7 Tetris pieces.
        public abstract int Id { get; }

        private int rotationState;
        private Position offset;

        public Block()
        {
            offset = new Position(StartOffset.Row, StartOffset.Column);
        }

        /// <summary>
        /// Identifies the position and rotation of the piece in play.
        /// </summary>
        public IEnumerable<Position> TilePositions()
        {
            foreach (Position p in Tiles[rotationState])
            {
                yield return new Position(p.Row + offset.Row, p.Column + offset.Column);
            }
        }

        /// <summary>
        /// Rotates a piece clockwise.
        /// </summary>
        public void RotateCW()
        {
            rotationState = (rotationState + 1) % Tiles.Length;
        }

        /// <summary>
        /// Rotates a piece counter-clockwise.
        /// </summary>
        public void RotateCCW()
        {
            if (rotationState == 0)
            {
                rotationState = Tiles.Length - 1;
            }
            else
            {
                rotationState--;
            }
        }

        /// <summary>
        /// Moves a piece.
        /// </summary>
        public void Move(int rows, int columns)
        {
            offset.Row += rows;
            offset.Column += columns;
        }

        /// <summary>
        /// Resets the starting rotation and position for the next piece.
        /// </summary>
        public void Reset()
        {
            rotationState = 0;
            offset.Row = StartOffset.Row;
            offset.Column = StartOffset.Column;
        }
    }
}
