using LabyrinthEngine;
using LabyrinthEngine.Entities;
using LabyrinthEngine.Helpers;
using LabyrinthEngine.Moves;
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
    /// This is a simple front-end for Hamster Labyrinth, written as a WinForms application.
    /// </summary>
    public partial class MainForm : Form
    {
        BoardState board;
        GameState game;

        const int fontSize = 10;
        const int startX = 75;
        const int startY = 30;
        const int squareHeight = 70;
        const int squareWidth = 70;

        public MainForm()
        {
            InitializeComponent();

            string boardXmlContent = System.IO.File.ReadAllText(@"..\..\Levels\TestBoard.xml");

            var loader = new BoardLoader(boardXmlContent);
            board = loader.Board;

            var player1 = new Player() { Name = "Geir", X = 3, Y = 3 };
            var player2 = new Player() { Name = "Nemesis", X = 1, Y = 0 };
            var players = new List<Player>();
            players.Add(player1);
            players.Add(player2);

            game = new GameState(board, players);

            canvas.Invalidate();
            printPlayerState();
            textBoxMessages.Text += "Game started." + Environment.NewLine;
        }

        private void executeAction(MoveType move)
        {
            var player = game.CurrentPlayer();
            var moveNumber = game.MoveCounter;
            var message = game.PerformMove(move);
            
            canvas.Invalidate();
            textBoxMessages.Text += string.Format("{0}/{1}: {2}{3}",
                player.Name, moveNumber, message, Environment.NewLine);
            textBoxMessages.SelectionStart = textBoxMessages.Text.Length;
            textBoxMessages.ScrollToCaret();
            printPlayerState();
        }

        private void printPlayerState()
        {
            StringBuilder result = new StringBuilder();
            var player = game.CurrentPlayer();

            result.AppendFormat("Current player: {0}", player.Name);
            result.Append(Environment.NewLine);
            result.Append(Environment.NewLine);
            result.AppendFormat("{0}", player.IsAlive ? "Alive." : "Dead.");
            result.Append(Environment.NewLine);
            result.AppendFormat("{0}", player.CarriesTreasure ? "Carries treasure." : "No treasure.");
            result.Append(Environment.NewLine);
            result.AppendFormat("Score: {0}", player.Score);
            result.Append(Environment.NewLine);

            result.AppendFormat("Arrows: {0}", player.NumArrows);
            result.Append(Environment.NewLine);
            result.AppendFormat("Grenades: {0}", player.NumGrenades);
            result.Append(Environment.NewLine);
            result.AppendFormat("Hamsters: {0}", player.NumHamsters);
            result.Append(Environment.NewLine);
            result.AppendFormat("Hamster spray: {0}", player.NumHamsterSprays);
            result.Append(Environment.NewLine);
            result.AppendFormat("Cement: {0}", player.NumCement);

            textBoxPlayerStatus.Text = result.ToString();
        }

        private void canvas_Paint(object sender, PaintEventArgs e)
        {
            if (board == null || game == null)
            {
                return;
            }

            // Horizontal walls
            for (int x = 0; x < board.HorizontalWalls.GetLength(0); x++)
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
                        pen = new Pen(Color.Green, 4);
                    }
                    else
                    {
                        pen = new Pen(Color.Black, 4);
                    }

                    if (board.HorizontalWalls[x, w_y].IsExit &&
                        board.HorizontalWalls[x, w_y].HasHamster)
                    {
                        pen = new Pen(Color.LightGreen);
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

                    if (board.VerticalWalls[y, w_x].IsExit &&
                        board.VerticalWalls[y, w_x].HasHamster)
                    {
                        pen = new Pen(Color.LightGreen);
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
            for (int x = 0; x < board.PlayfieldGrid.GetLength(0); x++)
            {
                for (int y = 0; y < board.PlayfieldGrid.GetLength(1); y++)
                {
                    if (board.PlayfieldGrid[x, y].NumTreasures > 0) // Treasure
                    {
                        e.Graphics.FillRectangle(Brushes.Gold,
                            startX + squareWidth * x + squareWidth / 4,
                            startY + squareHeight * y + squareHeight / 2,
                            squareHeight / 3, squareHeight / 3);
                    }

                    var squareType = board.PlayfieldGrid[x, y].Type;
                    var drawX = startX + x * squareWidth + squareWidth / 2 - fontSize / 2;
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
                            // TODO: Teleporter numbering
                            e.Graphics.DrawString("T", font, Brushes.Black, drawX, drawY);
                            break;
                    }

                    font.Dispose();
                }
            }

            var centaur = game.Board.centaur;

            e.Graphics.FillRectangle(Brushes.Firebrick,
                 startX + squareWidth * centaur.X + squareWidth / 2,
                 startY + squareHeight * centaur.Y + squareHeight / 4,
                 squareHeight / 4, squareHeight / 4);

            Brush[] playerBrushes = { Brushes.Blue, Brushes.Red, Brushes.Yellow, Brushes.Green };

            for (int playerIndex = 0; playerIndex < game.Players.Count; playerIndex++)
            {
                var player = game.Players[playerIndex];
                e.Graphics.FillEllipse(playerBrushes[playerIndex],
                    startX + squareWidth * player.X + squareWidth / 2,
                    startY + squareHeight * player.Y + squareHeight / 2,
                    squareHeight / 4, squareHeight / 4);
            }
        }

        private void buttonMoveUp_Click(object sender, EventArgs e)
        {
            executeAction(MoveType.MoveUp);
        }

        private void buttonMoveDown_Click(object sender, EventArgs e)
        {
            executeAction(MoveType.MoveDown);
        }

        private void buttonMoveLeft_Click(object sender, EventArgs e)
        {
            executeAction(MoveType.MoveLeft);
        }

        private void buttonMoveRight_Click(object sender, EventArgs e)
        {
            executeAction(MoveType.MoveRight);
        }

        private void buttonDoNothing_Click(object sender, EventArgs e)
        {
            executeAction(MoveType.DoNothing);
        }

        private void bActionUp_Click(object sender, EventArgs e)
        {
            if (rbShoot.Checked)
            {
                executeAction(MoveType.FireUp);
            }
            else if (rbGrenade.Checked)
            {
                executeAction(MoveType.ThrowGrenadeUp);
            }
            else if (rbHamster.Checked)
            {
                executeAction(MoveType.PlaceHamsterUp);
            }
            else if (rbHamsterSpray.Checked)
            {
                executeAction(MoveType.HamsterSprayUp);
            }
            else if (rbCement.Checked)
            {
                executeAction(MoveType.BuildWallUp);
            }
        }

        private void bActionDown_Click(object sender, EventArgs e)
        {
            if (rbShoot.Checked)
            {
                executeAction(MoveType.FireDown);
            }
            else if (rbGrenade.Checked)
            {
                executeAction(MoveType.ThrowGrenadeDown);
            }
            else if (rbHamster.Checked)
            {
                executeAction(MoveType.PlaceHamsterDown);
            }
            else if (rbHamsterSpray.Checked)
            {
                executeAction(MoveType.HamsterSprayDown);
            }
            else if (rbCement.Checked)
            {
                executeAction(MoveType.BuildWallDown);
            }
        }

        private void bActionRight_Click(object sender, EventArgs e)
        {
            if (rbShoot.Checked)
            {
                executeAction(MoveType.FireRight);
            }
            else if (rbGrenade.Checked)
            {
                executeAction(MoveType.ThrowGrenadeRight);
            }
            else if (rbHamster.Checked)
            {
                executeAction(MoveType.PlaceHamsterRight);
            }
            else if (rbHamsterSpray.Checked)
            {
                executeAction(MoveType.HamsterSprayRight);
            }
            else if (rbCement.Checked)
            {
                executeAction(MoveType.BuildWallRight);
            }
        }

        private void bActionLeft_Click(object sender, EventArgs e)
        {
            if (rbShoot.Checked)
            {
                executeAction(MoveType.FireLeft);
            }
            else if (rbGrenade.Checked)
            {
                executeAction(MoveType.ThrowGrenadeLeft);
            }
            else if (rbHamster.Checked)
            {
                executeAction(MoveType.PlaceHamsterLeft);
            }
            else if (rbHamsterSpray.Checked)
            {
                executeAction(MoveType.HamsterSprayLeft);
            }
            else if (rbCement.Checked)
            {
                executeAction(MoveType.BuildWallLeft);
            }
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
