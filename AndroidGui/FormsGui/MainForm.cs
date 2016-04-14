using LabyrinthEngine;
using LabyrinthEngine.Entities;
using LabyrinthEngine.Helpers;
using LabyrinthEngine.Playfield;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FormsGui
{
    /// <summary>
    /// This is a simple front-end for Hamster Labyrinth, written as a pure command-line interface.
    /// </summary>
    public partial class MainForm : Form
    {
        BoardState board;
        GameState game;

        const int fontSize = 10;

        public MainForm()
        {
            InitializeComponent();

            string boardXmlContent = System.IO.File.ReadAllText(@"..\..\Levels\TestBoard.xml");

            var loader = new BoardLoader(boardXmlContent);
            board = loader.Board;

            var player1 = new Player() { Name = "Geir", X=3, Y=3 };
            var player2 = new Player() { Name = "Nemesis", X = 1, Y = 0 };
            var players = new List<Player>();
            players.Add(player1);
            players.Add(player2);

            game = new GameState(board, players);

            paintBoardState();

            // TODO: Implement buttons in the GUI, for actions
            // TODO: Implement text boxes in the GUI, for status updates (players&board)
        }





        private void paintBoardState()
        {
            int squareHeight = 20;
            int squareWidth = 20;
            using (var context = canvas.CreateGraphics())
            {
                context.DrawLine(Pens.Beige, 0, 0, 50, 50);

            }
            canvas.Invalidate();
        }

        private void printPlayersState(GameState game)
        {

        }

        private void canvas_Paint(object sender, PaintEventArgs e)
        {
            var startX = 30;
            var startY = 30;

            var squareHeight = 50;
            var squareWidth = 50;

            if (board == null || game == null)
            {
                return;
            }

            // Horizontal walls
            for (int x = 0;  x < board.HorizontalWalls.GetLength(0); x++)
            {
                for (int w_y = 0; w_y < board.HorizontalWalls.GetLength(1); w_y++)
                {
                    Pen pen;
                    if (board.HorizontalWalls[x, w_y].IsExit)
                    {
                        continue;
                    }
                    else if (board.HorizontalWalls[x, w_y].IsPassable)
                    {
                        pen = new Pen(Color.Black, 1);
                    }
                    else if (board.HorizontalWalls[x, w_y].HasHamster)
                    {
                        pen = new Pen(Color.LightGreen, 4);
                    }
                    else
                    {
                        pen = new Pen(Color.Black, 4);
                    }
                    e.Graphics.DrawLine(pen,
                        startX + squareWidth * x,
                        startY + squareHeight * w_y,
                        startX + squareWidth * x + squareWidth,
                        startY + squareHeight * w_y);
                    pen.Dispose();
                }
            }

            // Vertical walls
            for (int y = 0; y < board.VerticalWalls.GetLength(0); y++)
            {
                for (int w_x = 0; w_x < board.HorizontalWalls.GetLength(1); w_x++)
                {
                    Pen pen;
                    if (board.VerticalWalls[y, w_x].IsExit)
                    {
                        continue;
                    }
                    else if (board.VerticalWalls[y, w_x].IsPassable)
                    {
                        pen = new Pen(Color.Black, 1);
                    }
                    else if (board.VerticalWalls[y, w_x].HasHamster)
                    {
                        pen = new Pen(Color.LightGreen, 4);
                    }
                    else
                    {
                        pen = new Pen(Color.Black, 4);
                    }
                    e.Graphics.DrawLine(pen,
                        startX + squareWidth * w_x,
                        startY + squareHeight * y,
                        startX + squareWidth * w_x,
                        startY + squareHeight * y + squareHeight);
                    pen.Dispose();
                }
            }

            // Playfield grid
            for (int x = 0; x < board.PlayfieldGrid.GetLength(0) ; x++)
            {
                for (int y = 0; y < board.PlayfieldGrid.GetLength(1); y++)
                {
                    var squareType = board.PlayfieldGrid[x, y].Type;
                    var drawX = startX + x * squareWidth + squareWidth / 2 - fontSize/2;
                    var drawY = startY + y * squareHeight + squareHeight / 2 - fontSize / 2;

                    var font = new Font(FontFamily.GenericMonospace, fontSize);


                    switch (squareType)
                    {
                        case SquareType.Empty:
                            break;
                        case SquareType.AmmoStorage:
                            e.Graphics.DrawString("A", font, Brushes.Black, drawX, drawY);
                            break;
                        case SquareType.HamsterStorage:
                            e.Graphics.DrawString("H", font, Brushes.Black, drawX, drawY);
                            break;
                        case SquareType.FitnessStudio:
                            e.Graphics.DrawString("F", font, Brushes.Black, drawX, drawY);
                            break;
                        case SquareType.CementStorage:
                            e.Graphics.DrawString("C", font, Brushes.Black, drawX, drawY);
                            break;
                        case SquareType.Teleporter:
                            e.Graphics.DrawString("T", font, Brushes.Black, drawX, drawY);
                            break;
                    }

                    font.Dispose();
                }
            }

            var centaur = game.Board.centaur;

            e.Graphics.FillRectangle(Brushes.Firebrick,
                 startX + squareWidth * centaur.X + squareWidth / 2,
                 startY + squareHeight * centaur.Y + squareHeight / 2,
                 squareHeight / 4, squareHeight / 4);

            Brush[] playerBrushes = { Brushes.Blue, Brushes.Red, Brushes.Yellow, Brushes.Green };

            for (int playerIndex = 0; playerIndex < game.Players.Count; playerIndex++)
            {
                var player = game.Players[playerIndex];
                e.Graphics.FillEllipse(playerBrushes[playerIndex], 
                    startX + squareWidth * player.X + squareWidth/2,
                    startY + squareHeight * player.Y + squareHeight / 2,
                    squareHeight /4, squareHeight / 4);
            }
        }
    }
}
