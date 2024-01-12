using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TetrisProject.Blocks;
using Block = TetrisProject.Blocks.Block;

/**
 * Tetris Project
 *
 * Jake Licmo
 * Date created: December 28, 2023
 * Last updated: January 6, 2024
 * 
 * Developed and referenced from OttoBotCode on YouTube: https://youtu.be/jcUctrLC-7M?si=r5RB-D2UsTaWpsXV
 * Tetris assets retrieved from OttoBotCode: https://ottobotcode.com/tetris/
 **/

namespace TetrisProject
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Loads tile images for blocks.
        /// </summary>
        private readonly ImageSource[] tileImages = new ImageSource[]
        {
            new BitmapImage(new Uri("Assets/TileEmpty.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileCyan.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileBlue.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileOrange.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileYellow.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileGreen.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TilePurple.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileRed.png", UriKind.Relative))
        };

        /// <summary>
        /// Loads block images.
        /// </summary>
        private readonly ImageSource[] blockImages = new ImageSource[]
        {
            new BitmapImage(new Uri("Assets/Block-Empty.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-I.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-J.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-L.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-O.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-S.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-T.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-Z.png", UriKind.Relative)),
        };

        private readonly Image[,] imageControls;
        private readonly int maxDelay = 1000;
        private readonly int minDelay = 75;
        private readonly int delayDecrease = 25;

        private GameState gameState = new GameState();

        public MainWindow()
        {
            InitializeComponent();
            imageControls = SetupGameCanvas(gameState.GameGrid);
        }

        /// <summary>
        /// Sets up game canvas on main window.
        /// </summary>
        private Image[,] SetupGameCanvas(GameGrid grid)
        {
            Image[,] imageControls = new Image[grid.Rows, grid.Columns];
            int cellSize = 25;

            for (int r = 0; r < grid.Rows; r++)
            {
                for (int c = 0; c < grid.Columns; c++)
                {
                    Image imageControl = new Image
                    {
                        Width = cellSize,
                        Height = cellSize
                    };

                    Canvas.SetTop(imageControl, (r - 2) * cellSize + 10);
                    Canvas.SetLeft(imageControl, c * cellSize);
                    GameCanvas.Children.Add(imageControl);
                    imageControls[r, c] = imageControl;
                }
            }
            return imageControls;
        }

        /// <summary>
        /// Calls in game grid.
        /// </summary>
        private void DrawGrid(GameGrid grid)
        {
            for (int r = 0; r < grid.Rows; r++)
            {
                for(int c=0; c < grid.Columns; c++)
                {
                    int id = grid[r, c];
                    imageControls[r, c].Opacity = 1;
                    imageControls[r,c].Source = tileImages[id];
                }
            }
        }

        /// <summary>
        /// Draws the block from the block queue.
        /// </summary>
        private void DrawBlock(Block block)
        {
            foreach(Position p in block.TilePositions())
            {
                imageControls[p.Row, p.Column].Opacity = 1;
                imageControls[p.Row, p.Column].Source = tileImages[block.Id];
            }
        }

        /// <summary>
        /// Draws the next preview block from the queue.
        /// </summary>
        private void DrawNextBlock(BlockQueue blockQueue)
        {
            Block next = blockQueue.NextBlock;
            NextImage.Source = blockImages[next.Id];
        }

        /// <summary>
        /// Draws the previous block from the block queue. Used in the 'Hold' function.
        /// </summary>
        private void DrawPreviousBlock(Block heldBlock) 
        {
            if(heldBlock == null)
            {
                HoldImage.Source = blockImages[0];
            }
            else
            {
                HoldImage.Source = blockImages[heldBlock.Id];
            }
        }

        /// <summary>
        /// Draws the 'ghost' block where the current block is projected to land.
        /// </summary>
        private void DrawGhostBlock(Block ghostBlock)
        {
            int dropDistance = gameState.BlockDropDistance();
            
            foreach(Position p in ghostBlock.TilePositions())
            {
                imageControls[p.Row + dropDistance, p.Column].Opacity = 0.25;
                imageControls[p.Row + dropDistance, p.Column].Source = tileImages[ghostBlock.Id];
            }
        }

        /// <summary>
        /// Calls in all the draw methods of the game.
        /// </summary>
        private void Draw(GameState gameState)
        {
            DrawGrid(gameState.GameGrid);
            DrawGhostBlock(gameState.CurrentBlock);
            DrawBlock(gameState.CurrentBlock);
            DrawNextBlock(gameState.BlockQueue);
            DrawPreviousBlock(gameState.HeldBlock);
            ScoreText.Text = $"Score: {gameState.Score}";
        }

        /// <summary>
        /// Executes all the movement functions of the game. Periodically moves the current block in play down unless the game is over.
        /// </summary>
        private async Task GameLoop()
        {
            Draw(gameState);

            while(!gameState.GameOver) 
            {
                int delay = Math.Max(minDelay, maxDelay - (gameState.Score * delayDecrease));
                await Task.Delay(delay);
                gameState.MoveBlockDown();
                Draw(gameState);
            }

            GameOverMenu.Visibility = Visibility.Visible;

            FinalScoreText.Text = $"Score: {gameState.Score}";
        }

        /// <summary>
        /// Keyboard event for the game controls.
        /// </summary>
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if(gameState.GameOver)
            {
                return;
            }

            switch(e.Key)
            {
                case Key.J:
                    gameState.MoveBlockLeft();
                    break;
                case Key.L:
                    gameState.MoveBlockRight();
                    break;
                case Key.K:
                    gameState.MoveBlockDown();
                    break;
                case Key.I:
                    gameState.RotateBlockCW();
                    break;
                case Key.U:
                    gameState.RotateBlockCCW();
                    break;
                case Key.C:
                    gameState.HoldBlock();
                    break;
                case Key.Space:
                    gameState.DropBlock();
                    break;
                default:
                    return;
            }

            Draw(gameState);
        }

        private async void GameCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            await GameLoop();
        }

        private async void PlayAgain_Click(object sender, RoutedEventArgs e)
        {
            gameState = new GameState();
            GameOverMenu.Visibility= Visibility.Hidden;
            await GameLoop();
        }

    }
}
