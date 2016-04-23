using LabyrinthEngine;
using LabyrinthEngine.Entities;
using LabyrinthEngine.GameLogic;
using LabyrinthEngine.Moves;
using LabyrinthEngine.Playfield;
using LabyrinthEngine.LevelConstruction;
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
        //private string boardFilename = @"..\..\Levels\TestBoard.xml";
        private string boardFilename = @"..\..\Levels\BeginnersLament.xml";

        private List<Player> InitialPlayerList;
        private BoardState InitialBoardState;
        private GameState game;

        private BoardState Board { get { return game.Board; } }

        const int fontSize = 9;
        const int startX = 75;
        const int startY = 30;
        const int squareHeight = 70;
        const int squareWidth = 70;

        public MainForm()
        {
            InitializeComponent();
            openFileDialog.InitialDirectory = Application.StartupPath;

            // Load default game state, we can change it in the UI later
            var player1 = new Player() { Name = "Geir", X = 3, Y = 3 };
            var player2 = new Player() { Name = "Nemesis", X = 1, Y = 0 };
            InitialPlayerList = new List<Player>();
            InitialPlayerList.Add(player1);
            InitialPlayerList.Add(player2);

            startNewGameWithSelectedPlayers();
        }

        private void askForPlayersWithNames(int howManyPlayers)
        {
            InitialPlayerList = new List<Player>();
            for (int i = 0; i < howManyPlayers; i++)
            {
                InitialPlayerList.Add(getPlayerFromUserInput(playerNumber:i+1));
            }
        }

        private void startNewGameWithSelectedPlayers()
        {
            InitialBoardState = loadBoardFromSpecifiedFilename();
            game = new GameState(InitialBoardState, InitialPlayerList);

            canvas.Invalidate();
            printPlayerState();
            textBoxMessages.Text = "Game started." + Environment.NewLine;
            printGameState();
            buttonUndo.Enabled = game.CanUndo();
            buttonRedo.Enabled = game.CanRedo();
        }

        private BoardState loadBoardFromSpecifiedFilename()
        {
            string boardXmlContent;
            try
            {
                boardXmlContent = System.IO.File.ReadAllText(boardFilename);
                var loader = new BoardLoader(boardXmlContent);
                return loader.Board;
            }
            catch (Exception)
            {
                boardXmlContent = System.IO.File.ReadAllText(@"TestBoard.xml");
                var loader = new BoardLoader(boardXmlContent);
                return loader.Board;
            }
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
            printGameState();

            buttonUndo.Enabled = game.CanUndo();
            buttonRedo.Enabled = game.CanRedo();
        }

        private void printGameState()
        {
            if (game.CurrentTurnPhase == TurnPhase.SelectMainAction)
            {
                labelStatus.Text = 
                    string.Format("Move {0}. Action phase: Movement. {1}'s turn.", 
                    game.MoveCounter, game.CurrentPlayer().Name);
            }
            else
            {
                labelStatus.Text = 
                    string.Format("Move {0}. Action phase: Followup action.  {1}'s turn.", 
                    game.MoveCounter, game.CurrentPlayer().Name);
            }
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
            if (InitialBoardState == null || game == null || Board == null)
            {
                return;
            }

            if (checkBoxHideBoard.Checked)
            {
                Brush backgroundColorBrush = new SolidBrush(canvas.BackColor);
                e.Graphics.FillRectangle(backgroundColorBrush, 0, 0, canvas.Width, canvas.Height);
                backgroundColorBrush.Dispose();
                return;
            }

            // Horizontal walls
            for (int x = 0; x < Board.HorizontalWalls.GetLength(0); x++)
            {
                for (int w_y = 0; w_y < Board.HorizontalWalls.GetLength(1); w_y++)
                {
                    Pen pen;
                    if (Board.HorizontalWalls[x, w_y].IsExterior &&
                        Board.HorizontalWalls[x, w_y].HasHamster)
                    {
                        pen = new Pen(Color.LightGreen, 4);
                    }
                    else if (Board.HorizontalWalls[x, w_y].IsExit &&
                        !Board.HorizontalWalls[x, w_y].IsPassable)
                    {
                        pen = new Pen(Color.Yellow, 4);
                    }
                    
                    else if (Board.HorizontalWalls[x, w_y].IsExit)
                    {
                        // Standard exit, open
                        continue;
                    }
                    else if (Board.HorizontalWalls[x, w_y].IsPassable)
                    {
                        pen = new Pen(Color.Black, 1);
                    }
                    else if (Board.HorizontalWalls[x, w_y].HasHamster)
                    {
                        pen = new Pen(Color.Green, 4);
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
            for (int y = 0; y < Board.VerticalWalls.GetLength(0); y++)
            {
                for (int w_x = 0; w_x < Board.HorizontalWalls.GetLength(1); w_x++)
                {
                    Pen pen;
                    if (Board.VerticalWalls[y, w_x].IsExterior &&
                        Board.VerticalWalls[y, w_x].HasHamster)
                    {
                        pen = new Pen(Color.LightGreen, 4);
                    }
                    else if (Board.VerticalWalls[y, w_x].IsExit &&
                        !Board.VerticalWalls[y, w_x].IsPassable)
                    {
                        pen = new Pen(Color.Yellow, 4);
                    }
                    
                    else if (Board.VerticalWalls[y, w_x].IsExit)
                    {
                        // Standard exit, open
                        continue;
                    }
                    else if (Board.VerticalWalls[y, w_x].IsPassable)
                    {
                        pen = new Pen(Color.Black, 1);
                    }
                    else if (Board.VerticalWalls[y, w_x].HasHamster)
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
            for (int x = 0; x < Board.PlayfieldGrid.GetLength(0); x++)
            {
                for (int y = 0; y < Board.PlayfieldGrid.GetLength(1); y++)
                {
                    if (Board.PlayfieldGrid[x, y].NumTreasures > 0) // Treasure
                    {
                        e.Graphics.FillRectangle(Brushes.Gold,
                            startX + squareWidth * x + squareWidth / 4,
                            startY + squareHeight * y + squareHeight / 2,
                            squareHeight / 3, squareHeight / 3);
                    }

                    var square = Board.PlayfieldGrid[x, y];
                    var drawX = startX + x * squareWidth + squareWidth / 2 - fontSize*6 / 2;
                    var drawY = startY + y * squareHeight + squareHeight / 2 - fontSize*6 / 2;

                    var font = new Font(FontFamily.GenericMonospace, fontSize);


                    switch (square.Type)
                    {
                        case SquareType.Empty:
                            break;
                        case SquareType.AmmoStorage:
                            e.Graphics.DrawString("Ammo   ", font, Brushes.Black, drawX, drawY);
                            break;
                        case SquareType.HamsterStorage:
                            e.Graphics.DrawString("Hamster", font, Brushes.Black, drawX, drawY);
                            break;
                        case SquareType.FitnessStudio:
                            e.Graphics.DrawString("Fitness", font, Brushes.Black, drawX, drawY);
                            break;
                        case SquareType.CementStorage:
                            e.Graphics.DrawString("Cement ", font, Brushes.Black, drawX, drawY);
                            break;
                        case SquareType.Teleporter:
                            e.Graphics.DrawString(
                                string.Format("Hole {0} ", square.Hole.TeleporterIndex), 
                                font, Brushes.Black, drawX, drawY);
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

            Brush[] playerBrushes = { Brushes.Blue, Brushes.Green, Brushes.Yellow, Brushes.DarkViolet, Brushes.HotPink, Brushes.DarkCyan };
            Brush[] deadPlayerBrushes = { Brushes.LightBlue, Brushes.LightGreen, Brushes.LightYellow, Brushes.MediumOrchid, Brushes.Pink, Brushes.Cyan};

            for (int playerIndex = 0; playerIndex < game.Players.Count; playerIndex++)
            {
                var player = game.Players[playerIndex];

                Brush brush;
                if (player.IsAlive)
                {
                    brush = playerBrushes[playerIndex];
                }
                else
                {
                    brush = deadPlayerBrushes[playerIndex];
                }

                e.Graphics.FillEllipse(brush,
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

        private void bShootHere_Click(object sender, EventArgs e)
        {
            if (rbShoot.Checked)
            {
                executeAction(MoveType.FireAtSameSquare);
            }
        }

        private void bFallThrough_Click(object sender, EventArgs e)
        {
            executeAction(MoveType.FallThroughHole);
        }

        private void newGameWith1Player_Click(object sender, EventArgs e)
        {
            askForPlayersWithNames(1);
            startNewGameWithSelectedPlayers();
        }

        private void newGameWith2Players_Click(object sender, EventArgs e)
        {
            askForPlayersWithNames(2);
            startNewGameWithSelectedPlayers();
        }

        private void newGameWith3Players_Click(object sender, EventArgs e)
        {
            askForPlayersWithNames(3);
            startNewGameWithSelectedPlayers();
        }

        private void newGameWith4Players_Click(object sender, EventArgs e)
        {
            askForPlayersWithNames(4);
            startNewGameWithSelectedPlayers();
        }

        private void newGameWith5Players_Click(object sender, EventArgs e)
        {
            askForPlayersWithNames(5);
            startNewGameWithSelectedPlayers();
        }

        private void newGameWith6Players_Click(object sender, EventArgs e)
        {
            askForPlayersWithNames(6);
            startNewGameWithSelectedPlayers();
        }

        private Player getPlayerFromUserInput(int playerNumber)
        {
            var name = NewPlayerPrompt.ShowDialog(
                string.Format("Player {0} name:", playerNumber.ToString()),
                "New game");
            if (!string.IsNullOrEmpty(name))
            {
                return new Player() { Name = name };
            }
            else
            {
                return new Player() { Name = "The Unsung" };
            }
        }

        private void checkBoxHideBoard_CheckedChanged(object sender, EventArgs e)
        {
            canvas.Invalidate();
        }

        private void buttonUndo_Click(object sender, EventArgs e)
        {
            game.UndoPreviousMove();
            printGameState();
            printPlayerState();
            canvas.Invalidate();
            buttonUndo.Enabled = game.CanUndo();
            buttonRedo.Enabled = game.CanRedo();
        }

        private void buttonRedo_Click(object sender, EventArgs e)
        {
            game.RedoNextMove();
            printGameState();
            printPlayerState();
            canvas.Invalidate();
            buttonUndo.Enabled = game.CanUndo();
            buttonRedo.Enabled = game.CanRedo();
        }

        private void loadLevelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var dialogResult = openFileDialog.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                boardFilename = openFileDialog.FileName;
                loadBoardFromSpecifiedFilename();
                startNewGameWithSelectedPlayers();
            }
            
        }
    }
}
