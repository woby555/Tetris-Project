using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TetrisProject.Blocks;

namespace TetrisProject
{
    /// <summary>
    /// GameState class. Describes the interaction between the GameGrid, Blocks, and Positions for when a game is initiated.
    /// </summary>
    public class GameState
    {
        private Block currentBlock;

        public Block CurrentBlock
        {
            get => currentBlock;   
            private set
            {
                currentBlock = value;
                currentBlock.Reset();

                for(int i = 0; i < 2; i++)
                {
                    currentBlock.Move(1, 0);
                    if(!BlockFits())
                    {
                        currentBlock.Move(-1, 0);
                    }
                }
            }
        }

        public GameGrid GameGrid { get; }
        public BlockQueue BlockQueue { get; }
        public bool GameOver { get; private set; }

        public int Score { get; private set; }

        public Block HeldBlock { get; private set; }

        public bool CanHold { get; private set; }  

        /// <summary>
        /// Initializes a game.
        /// </summary>
        public GameState()
        {
            GameGrid = new GameGrid(22, 10);
            BlockQueue = new BlockQueue();
            CurrentBlock = BlockQueue.GetAndUpdate();
            CanHold = true;
        }

        /// <summary>
        /// Checks if a block is in a legal position.
        /// </summary>
        /// <returns>True if the block can fit.</returns>
        private bool BlockFits()
        {
            foreach (Position p in CurrentBlock.TilePositions())
            {
                if (!GameGrid.IsEmpty(p.Row, p.Column))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Describes the 'Hold' functionality in Tetris. Holds the current block in play and selects the next block in queue.
        /// </summary>
        public void HoldBlock()
        {
            if(!CanHold)
            {
                return;
            }

            if(HeldBlock == null)
            {
                HeldBlock = CurrentBlock;
                CurrentBlock = BlockQueue.GetAndUpdate();
            }
            else
            {
                Block tmp = CurrentBlock;
                CurrentBlock = HeldBlock;
                HeldBlock = tmp;
            }

            CanHold = false;
        }

        /// <summary>
        /// Rotates the piece clockwise.
        /// </summary>
        public void RotateBlockCW()
        {
            CurrentBlock.RotateCW();

            if(!BlockFits()) 
            {
                CurrentBlock.RotateCCW();
            }
        }

        /// <summary>
        /// Rotates the piece counter-clockwise.
        /// </summary>
        public void RotateBlockCCW()
        {
            CurrentBlock.RotateCCW();
            if(!BlockFits())
            {
                CurrentBlock.RotateCW();
            }
        }

        /// <summary>
        /// Moves the block left.
        /// </summary>
        public void MoveBlockLeft()
        {
            CurrentBlock.Move(0, -1);
            if(!BlockFits())
            {
                CurrentBlock.Move(0, 1);
            }
        }

        /// <summary>
        /// Moves the block right.
        /// </summary>
        public void MoveBlockRight()
        {
            CurrentBlock.Move(0, 1);
            if (!BlockFits())
            {
                CurrentBlock.Move(0, -1);
            }
        }

        /// <summary>
        /// Determines if a game is over.
        /// </summary>
        /// <returns>True if the block reaches the top of the GameGrid</returns>
        private bool IsGameOver()
        {
            return !(GameGrid.IsRowEmpty(0) && GameGrid.IsRowEmpty(1));
        }

        /// <summary>
        /// Places the block and determines the game state.
        /// </summary>
        private void PlaceBlock()
        {
            foreach (Position p in CurrentBlock.TilePositions())
            {
                GameGrid[p.Row, p.Column] = CurrentBlock.Id;
            }

            Score += GameGrid.ClearFullRows();
            if (IsGameOver()) 
            {
                GameOver = true; 
            }
            else
            {
                CurrentBlock = BlockQueue.GetAndUpdate();
                CanHold = true;
            }
        }

        /// <summary>
        /// Moves the block down.
        /// </summary>
        public void MoveBlockDown()
        {
            CurrentBlock.Move(1,0);
            if (!BlockFits())
            {
                CurrentBlock.Move(-1, 0);
                PlaceBlock();
            }
        }

        /// <summary>
        /// Determines the tile drop distance from the top, to its bottom-most position.
        /// </summary>
        /// <param name="p">The tile's current position.</param>
        /// <returns>The distance from the tile, to its bottom-most position.</returns>
        private int TileDropDistance(Position p)
        {
            int drop = 0;

            while (GameGrid.IsEmpty(p.Row + drop + 1, p.Column))
            {
                drop++;
            }

            return drop;
        }

        /// <summary>
        /// Returns the block's drop distance, to the bottom.
        /// </summary>
        /// <returns></returns>
        public int BlockDropDistance()
        {
            int drop = GameGrid.Rows;

            foreach(Position p in CurrentBlock.TilePositions())
            {
                drop = System.Math.Min(drop, TileDropDistance(p));
            }

            return drop;    
        }

        /// <summary>
        /// Instantly drops the block down.
        /// </summary>
        public void DropBlock()
        {
            CurrentBlock.Move(BlockDropDistance(), 0);
            PlaceBlock();
        }
    }
}
