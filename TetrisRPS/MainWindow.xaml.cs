﻿using System;
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
using System.Windows.Threading;

namespace TetrisRPS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Array containg the tile images
        // Order goes first with the empty tile then corresponds to each blocks ID
        private readonly ImageSource[] tileImages = new ImageSource[]
        {
            new BitmapImage(new Uri("assets/TileEmpty.png", UriKind.Relative)),
            new BitmapImage(new Uri("assets/TileCyan.png", UriKind.Relative)),
            new BitmapImage(new Uri("assets/TileBlue.png", UriKind.Relative)),
            new BitmapImage(new Uri("assets/TileOrange.png", UriKind.Relative)),
            new BitmapImage(new Uri("assets/TileYellow.png", UriKind.Relative)),
            new BitmapImage(new Uri("assets/TileGreen.png", UriKind.Relative)),
            new BitmapImage(new Uri("assets/TilePurple.png", UriKind.Relative)),
            new BitmapImage(new Uri("assets/TileRed.png", UriKind.Relative))
        };

        // Array contains full picture of the blocks
        // Used in holding and upcoming block 
        // The order matches each blocks ID
        private readonly ImageSource[] blockImages = new ImageSource[]
        {
            new BitmapImage(new Uri("assets/Block-Empty.png", UriKind.Relative)),
            new BitmapImage(new Uri("assets/Block-I.png", UriKind.Relative)),
            new BitmapImage(new Uri("assets/Block-J.png", UriKind.Relative)),
            new BitmapImage(new Uri("assets/Block-L.png", UriKind.Relative)),
            new BitmapImage(new Uri("assets/Block-O.png", UriKind.Relative)),
            new BitmapImage(new Uri("assets/Block-S.png", UriKind.Relative)),
            new BitmapImage(new Uri("assets/Block-T.png", UriKind.Relative)),
            new BitmapImage(new Uri("assets/Block-Z.png", UriKind.Relative))
        };

        // For each of the game grid cells there is and image control
        private readonly Image[,] firstImageControls;
        private readonly Image[,] secondImageControls;

        private GameState firstGameState = new GameState();
        private GameState secondGameState = new GameState();

        DispatcherTimer timer = new DispatcherTimer();

        public MainWindow()
        {
            InitializeComponent();
            firstImageControls = SetupGameCanvas(firstGameState.GameGrid, firstCanvas);
            secondImageControls = SetupGameCanvas(secondGameState.GameGrid, secondCanvas);
            timer.Tick += Game_Tick;
            timer.Interval = TimeSpan.FromMilliseconds(500);
        }

        private void Game_Tick(object? sender, EventArgs e)
        {
            if (!firstGameState.IsGameOver && !secondGameState.IsGameOver)
            {
                firstGameState.MoveBlockDown();
                Draw(firstGameState, firstImageControls);
            }
            else
            {
                gameOverScreen.Visibility = Visibility.Visible;
                playerWinText.Text = "Game Over";
                timer.Stop();
            }
        }

        private Image[,] SetupGameCanvas(GameGrid grid, Canvas canvas)
        {
            Image[,] ImageControls = new Image[grid.Rows, grid.Columns];
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
                    Canvas.SetTop(imageControl, (r - 2) * cellSize);
                    Canvas.SetLeft(imageControl, c * cellSize);
                    canvas.Children.Add(imageControl);
                    ImageControls[r, c] = imageControl;
                }
            }
            return ImageControls;
        }

        private void DrawGrid(GameGrid grid, Image[,] control)
        {
            for (int r = 0; r < grid.Rows; r++)
            {
                for (int c = 0; c < grid.Columns; c++)
                {
                    int id = grid[r, c];
                    control[r, c].Source = tileImages[id];
                }
            }
        }

        private void DrawBlock(Block block, Image[,] control)
        {
            foreach (Position p in block.TilePositions())
            {
                control[p.Row, p.Column].Source = tileImages[block.Id];
            }
        }

        private void DrawHeldBlock(Block heldBlock)
        {
            if (heldBlock == null)
            {
                holdImage.Source = blockImages[0];
            }
            else
            {
                holdImage.Source = blockImages[heldBlock.Id];
            }
        }

        private void Draw(GameState gameState, Image[,] control)
        {
            DrawGrid(gameState.GameGrid, control);
            DrawBlock(gameState.currentBlock, control);
            DrawHeldBlock(gameState.HeldBlock);
        }

        // Detecting player input
        // Function is called inside the Window
        private void WindowKeyDown(object sender, KeyEventArgs e)
        {
            if (firstGameState.IsGameOver)
            {
                return;
            }
            firstGameState.MoveBlock((int)e.Key);
            Draw(firstGameState, firstImageControls);
        }

        private async void End_Click(object sender, RoutedEventArgs e)
        {
            gameOverScreen.Visibility = Visibility.Hidden;
            firstGameState = new GameState();
            timer.Start();
        }

        private async void Star_Clik(object sender, RoutedEventArgs e)
        {
            IPInput.IsEnabled = false;
            StartButton.IsEnabled = false;
            gameOverScreen.Visibility = Visibility.Hidden;
            firstGameState = new GameState();
            timer.Start();

        }

        // Play again button
        // The button appears in the overlay once the game is over
        // Honestly not sure how this one is going to work with two players having to press it at the same time


    }
}
