using LabyrinthEngine.Entities;
using LabyrinthEngine.Helpers;
using LabyrinthEngine.Moves;
using LabyrinthEngine.Playfield;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabyrinthEngine
{
    /// <summary>
    /// Performs internal actions to update the game state during the course of the game.
    /// Most of the game rules are described in this class. The class belongs to and is 
    /// controlled by an instance of the GameState class.
    /// </summary>
    public class TurnController
    {
        private BoardState board;
        private List<Player> players;
        private Random randomNumberGenerator;

        private StringBuilder descriptionOfCurrentMove;

        public TurnController(BoardState board, List<Player> players, Random randomNumberGenerator)
        {
            this.board = board;
            this.players = players;
            this.randomNumberGenerator = randomNumberGenerator;
        }

        public string DescriptionOfExecutedMove()
        {
            return descriptionOfCurrentMove.ToString();
        }

        public void ResolveMovementAction(Move move)
        {
            var player = move.PerformedBy;
            var action = move.ActionType;
            descriptionOfCurrentMove = new StringBuilder();

            switch (action)
            {
                case MoveType.MoveUp:
                    if (board.GetWallAbove(player).IsPassable)
                    {
                        player.Y--;
                        descriptionOfCurrentMove.Append("Walked north. ");
                    }
                    else
                    {
                        descriptionOfCurrentMove.Append("Hit wall. ");
                    }
                    
                    break;
                case MoveType.MoveDown:
                    if (board.GetWallBelow(player).IsPassable)
                    {
                        player.Y++;
                        descriptionOfCurrentMove.Append("Walked south. ");
                    }
                    else
                    {
                        descriptionOfCurrentMove.Append("Hit wall. ");
                    }
                    break;
                case MoveType.MoveLeft:
                    if (board.GetWallLeftOf(player).IsPassable)
                    {
                        player.X--;
                        descriptionOfCurrentMove.Append("Walked west. ");
                    }
                    else
                    {
                        descriptionOfCurrentMove.Append("Hit wall. ");
                    }
                    break;
                case MoveType.MoveRight:
                    if (board.GetWallRightOf(player).IsPassable)
                    {
                        player.X++;
                        descriptionOfCurrentMove.Append("Walked east. ");
                    }
                    else
                    {
                        descriptionOfCurrentMove.Append("Hit wall. ");
                    }
                    break;
                case MoveType.DoNothing:
                    descriptionOfCurrentMove.Append("Standing still. ");
                    break;
                case MoveType.FallThroughHole:
                    descriptionOfCurrentMove.Append("Standing still. ");
                    // TODO: I think the correct solution is to handle this in ResolvePostMovementEvents.
                    break;
                default:
                    throw new InvalidOperationException("Fall-through in game logic: "
                        + "Action should have been executed as a followup action.");
            }
        }

        public void ResolveFollowupAction(Move move)
        {
            var action = move.ActionType;
            descriptionOfCurrentMove = new StringBuilder();

            if (action == MoveType.FireUp ||
                action == MoveType.FireRight ||
                action == MoveType.FireDown ||
                action == MoveType.FireLeft ||
                action == MoveType.FireAtSameSquare)
            {
                handleShooting(move);
            }
            else if (action == MoveType.ThrowGrenadeUp ||
                     action == MoveType.ThrowGrenadeRight ||
                     action == MoveType.ThrowGrenadeDown ||
                     action == MoveType.ThrowGrenadeLeft)
            {
                handleGrenadeThrow(move);
            }
            else if (action == MoveType.PlaceHamsterUp ||
                     action == MoveType.PlaceHamsterRight ||
                     action == MoveType.PlaceHamsterDown ||
                     action == MoveType.PlaceHamsterLeft)
            {
                handleHamsterPlacement(move);
            }
            else if (action == MoveType.HamsterSprayUp ||
                     action == MoveType.HamsterSprayRight ||
                     action == MoveType.HamsterSprayDown ||
                     action == MoveType.HamsterSprayLeft)
            {
                handleHamsterSpraying(move);
            }
            else if (action == MoveType.BuildWallUp ||
                     action == MoveType.BuildWallRight ||
                     action == MoveType.BuildWallDown ||
                     action == MoveType.BuildWallLeft)
            {
                handleWallConstruction(move);
            }
            else if (action == MoveType.DoNothing || 
                     action == MoveType.FallThroughHole ||
                     action == MoveType.MoveUp ||
                     action == MoveType.MoveRight ||
                     action == MoveType.MoveDown ||
                     action == MoveType.MoveLeft)
            {
                return; // Skip invalid followup actions.
            }
            else
            {
                throw new InvalidOperationException("Fall-through in game logic: "
                + "Unhandled followup action.");
            }
        }

        private void handleWallConstruction(Move move)
        {
            var player = move.PerformedBy;
            var action = move.ActionType;
            WallSection targetWall;

            switch (action)
            {
                case MoveType.BuildWallUp:
                    targetWall = board.GetWallAbove(player);
                    break;
                case MoveType.BuildWallRight:
                    targetWall = board.GetWallRightOf(player);
                    break;
                case MoveType.BuildWallDown:
                    targetWall = board.GetWallBelow(player);
                    break;
                case MoveType.BuildWallLeft:
                    targetWall = board.GetWallLeftOf(player);
                    break;
                default:
                    throw new InvalidOperationException("handleWallConstruction called with illegal move");
            }

            if (player.NumCement <= 0)
            {
                descriptionOfCurrentMove.Append("You are out of cement. ");
            }
            else if (targetWall.IsPassable)
            {
                player.NumCement--;
                targetWall.IsPassable = false;
                descriptionOfCurrentMove.Append("You construct a new wall. ");
            }
            else
            {
                descriptionOfCurrentMove.Append("There is already a wall there. ");
            }
        }

        private void handleHamsterSpraying(Move move)
        {
            var player = move.PerformedBy;
            var action = move.ActionType;
            WallSection targetWall;

            switch (action)
            {
                case MoveType.HamsterSprayUp:
                    targetWall = board.GetWallAbove(player);
                    break;
                case MoveType.HamsterSprayRight:
                    targetWall = board.GetWallRightOf(player);
                    break;
                case MoveType.HamsterSprayDown:
                    targetWall = board.GetWallBelow(player);
                    break;
                case MoveType.HamsterSprayLeft:
                    targetWall = board.GetWallLeftOf(player);
                    break;
                default:
                    throw new InvalidOperationException("handleHamsterSpraying called with illegal move");
            }

            if (player.NumHamsterSprays <= 0)
            {
                descriptionOfCurrentMove.Append("You are out of cement. ");
            }
            else if (targetWall.HasHamster)
            {
                player.NumHamsterSprays--;
                targetWall.HasHamster = false;
                descriptionOfCurrentMove.Append("Several squeaking hamsters tumble out from the wall. ");
            }
            else
            {
                player.NumHamsterSprays--;
                descriptionOfCurrentMove.Append("You spray the wall. ");
            }
        }

        private void handleHamsterPlacement(Move move)
        {
            var player = move.PerformedBy;
            var action = move.ActionType;
            WallSection targetWall;

            switch (action)
            {
                case MoveType.PlaceHamsterUp:
                    targetWall = board.GetWallAbove(player);
                    break;
                case MoveType.PlaceHamsterRight:
                    targetWall = board.GetWallRightOf(player);
                    break;
                case MoveType.PlaceHamsterDown:
                    targetWall = board.GetWallBelow(player);
                    break;
                case MoveType.PlaceHamsterLeft:
                    targetWall = board.GetWallLeftOf(player);
                    break;
                default:
                    throw new InvalidOperationException("handleHamsterPlacement called with illegal move");
            }

            if (player.NumHamsters <= 0)
            {
                descriptionOfCurrentMove.Append("You are out of hamsters. ");
            }
            else if (!targetWall.HasHamster)
            {
                player.NumHamsters--;
                targetWall.HasHamster = true;
                descriptionOfCurrentMove.Append("Several squeaking hamsters tumble out from the wall. ");
            }
            else
            {
                descriptionOfCurrentMove.Append("A family of hamsters angrily repels your rodent. ");
            }
        }

        private void handleGrenadeThrow(Move move)
        {
            var player = move.PerformedBy;
            var action = move.ActionType;
            WallSection targetWall;

            switch (action)
            {
                case MoveType.ThrowGrenadeUp:
                    targetWall = board.GetWallAbove(player);
                    break;
                case MoveType.ThrowGrenadeRight:
                    targetWall = board.GetWallRightOf(player);
                    break;
                case MoveType.ThrowGrenadeDown:
                    targetWall = board.GetWallBelow(player);
                    break;
                case MoveType.ThrowGrenadeLeft:
                    targetWall = board.GetWallLeftOf(player);
                    break;
                default:
                    throw new InvalidOperationException("handleGrenadeThrow called with illegal move");
            }

            if (player.NumGrenades <= 0)
            {
                descriptionOfCurrentMove.Append("You are out of grenades. ");
            }
            else if (targetWall.IsExterior)
            {
                player.NumGrenades--;
                descriptionOfCurrentMove.Append("The grenade explodes against an exterior wall. ");
            }
            else if (targetWall.HasHamster)
            {
                descriptionOfCurrentMove.Append("A hamster returns your grenade with the pin inserted. ");
            }
            else if (targetWall.IsPassable)
            {
                player.NumGrenades--;
                descriptionOfCurrentMove.Append("The grenade explodes in the air. ");
            }
            else
            {
                player.NumGrenades--;
                targetWall.IsPassable = true;
                descriptionOfCurrentMove.Append("You blow up the wall. ");
            }
        }

        private void handleShooting(Move move)
        {
            var player = move.PerformedBy;
            var action = move.ActionType;
            Player victim = null;
            bool centaurWasHit = false;

            bool arrowWasBlockedByWall = false;
            int arrowX = player.X;
            int arrowY = player.Y;

            while (victim == null && centaurWasHit == false)
            {
                switch (action)
                {
                    case MoveType.FireUp:
                        arrowWasBlockedByWall =
                            board.GetWallAbove(arrowX, arrowY).IsExterior ||
                            !board.GetWallAbove(arrowX, arrowY).IsPassable;
                        arrowY--;
                        break;
                    case MoveType.FireRight:
                        arrowWasBlockedByWall =
                            board.GetWallRightOf(arrowX, arrowY).IsExterior ||
                            !board.GetWallRightOf(arrowX, arrowY).IsPassable;
                        arrowX++;
                        break;
                    case MoveType.FireDown:
                        arrowWasBlockedByWall =
                            board.GetWallBelow(arrowX, arrowY).IsExterior ||
                            !board.GetWallBelow(arrowX, arrowY).IsPassable;
                        arrowY++;
                        break;
                    case MoveType.FireLeft:
                        arrowWasBlockedByWall =
                            board.GetWallLeftOf(arrowX, arrowY).IsExterior ||
                            !board.GetWallLeftOf(arrowX, arrowY).IsPassable;
                        arrowX--;
                        break;
                    case MoveType.FireAtSameSquare:
                        var victimCandidates = findAllLivingPlayersAt(player.X, player.Y);
                        bool removedShooterFromCandidates = victimCandidates.Remove(player);

                        if (!removedShooterFromCandidates)
                        {
                            throw new LabyrinthInvalidStateException("Could not find shooter among players " +
                                "on shooter's square. Likely logic error in equality operator of Player.");
                        }

                        if (victimCandidates.Count > 0)
                        {
                            var victimIndex = randomNumberGenerator.Next(0, victimCandidates.Count - 1);
                            victim = victimCandidates[victimIndex];
                        }
                        arrowWasBlockedByWall = true;
                        break;
                }

                if (arrowWasBlockedByWall)
                {
                    // Stop evaluating arrow path when we (eventually) encounter a wall.
                    // This always happens if there is no other obstacle.
                    break;
                }
                
                if (board.centaur.X == arrowX && board.centaur.Y == arrowY)
                {
                    centaurWasHit = true;
                }
                else if (findAllLivingPlayersAt(arrowX, arrowY).Count > 0)
                {
                    victim = pickRandomLivingPlayerAt(arrowX, arrowY);
                }
            }

            if (player.NumArrows <= 0)
            {
                descriptionOfCurrentMove.Append("You are out of ammo. Dang. ");
            }
            else
            {
                player.NumArrows--;

                if (centaurWasHit)
                {
                    descriptionOfCurrentMove.Append("The centaur snatches the arrow out " 
                        + "of the air and stares ominously at you. ");
                }
                else if (victim == null)
                {
                    descriptionOfCurrentMove.Append("You missed. ");
                }
                else
                {
                    killPlayer(victim);
                    descriptionOfCurrentMove.AppendFormat("You killed {0}! ", victim.Name);
                }
            }
        }

        public void ResolvePostMovementEventsFor(Player player)
        {
            var newSquare = board.GetPlayfieldSquareOf(player);

            if (newSquare.NumTreasures > 0)
            {
                if (player.IsAlive && !player.CarriesTreasure)
                {
                    player.CarriesTreasure = true;
                    newSquare.NumTreasures--;
                    descriptionOfCurrentMove.Append("You have found treasure! ");
                }
                else if (player.IsAlive && player.CarriesTreasure)
                {
                    descriptionOfCurrentMove.Append("There is treasure here, " +
                        "but your hands are full. ");
                }
                else if (!player.IsAlive)
                {
                    descriptionOfCurrentMove.Append("There is treasure here, " +
                        "but your ghost hands can't carry it. ");
                }
            }
            
            if (newSquare.Type == SquareType.FitnessStudio)
            {
                if (!player.IsAlive)
                {
                    player.IsAlive = true;
                    descriptionOfCurrentMove.Append("You enter the fitness studio " +
                        "and return to life! ");
                }
            }

            if (board.centaur.X == player.X && board.centaur.Y == player.Y)
            {
                if (player.IsAlive)
                {
                    killPlayer(player);
                    descriptionOfCurrentMove.Append("You are trampled to death by the centaur. ");
                }
                else
                {
                    descriptionOfCurrentMove.Append("The centaur ignores your ghostly form. ");
                }
            }
            
            if (newSquare.Type == SquareType.AmmoStorage)
            {
                if (player.IsAlive)
                {
                    player.NumArrows = Player.ArrowCapacity;
                    player.NumGrenades = Player.GrenadeCapacity;
                    descriptionOfCurrentMove.Append("You are in the ammo storage and load up " +
                        "on arrows and grenades. ");
                }
                else
                {
                    descriptionOfCurrentMove.Append("You are in the ammo storage. ");
                }
            }

            if (newSquare.Type == SquareType.HamsterStorage)
            {
                if (player.IsAlive)
                {
                    player.NumHamsters = Player.HamsterCapacity;
                    player.NumHamsterSprays = Player.HamsterSprayCapacity;
                    descriptionOfCurrentMove.Append("You are in the hamster storage and stock " +
                        "up on hamsters and hamster spray. ");
                }
                else
                {
                    descriptionOfCurrentMove.Append("You are in the hamster storage. ");
                }
            }

            if (newSquare.Type == SquareType.CementStorage)
            {
                if (player.IsAlive)
                {
                    player.NumCement = Player.CementCapacity;
                    descriptionOfCurrentMove.Append("You are in the cement storage and "+ 
                        "replenish your supply. ");
                }
                else
                {
                    descriptionOfCurrentMove.Append("You are in the cement storage. ");
                }
            }

            if (newSquare.Type == SquareType.Teleporter)
            {
                // TODO: Implement teleportation
            }

            if (isCentaurAdjacentTo(player))
            {
                descriptionOfCurrentMove.Append("Clop clop... ");
            }
        }

        private void ResolveEndOfTurn()
        {
            // Move centaur if end of turn
            // Print clopclop messages for the relevant players

        }

        private void moveCentaur()
        {
            // TODO: Implement centaur movement
        }

        private bool isCentaurAdjacentTo(Player player)
        {
            var centaur = board.centaur;

            if (centaur.X == player.X)
            {
                if (player.Y == centaur.Y-1 || player.Y == centaur.Y+1)
                {
                    return true;
                }
            }
            else if (centaur.Y == player.Y)
            {
                if (player.X == centaur.X - 1 || player.X == centaur.X + 1)
                {
                    return true;
                }
            }

            return false;
        }

        private void killPlayer(Player victim)
        {
            if (!victim.IsAlive)
            {
                throw new InvalidOperationException("Attempted to kill a dead player.");
            }

            victim.IsAlive = false;
            victim.NumArrows = 0;
            victim.NumGrenades = 0;
            victim.NumHamsters = 0;
            victim.NumHamsterSprays = 0;
            victim.NumCement = 0;

            if (victim.CarriesTreasure)
            {
                victim.CarriesTreasure = false;
                board.GetPlayfieldSquareOf(victim).NumTreasures++;
            }
        }

        private List<Player> findAllLivingPlayersAt(int x, int y)
        {
            return players.FindAll(player => player.X == x && player.Y == y && player.IsAlive);
        }

        private Player pickRandomLivingPlayerAt(int x, int y)
        {
            var candidates = findAllLivingPlayersAt(x, y);

            if (candidates.Count == 0)
            {
                throw new InvalidOperationException("pickRandomLivingPlayerAt called " +
                    "on coordinates with no players.");
            }

            var indexOfLuckyPlayer = randomNumberGenerator.Next(0, candidates.Count - 1);
            return candidates[indexOfLuckyPlayer];
        }

        public static bool IsMovementAction(MoveType action)
        {
            return action == MoveType.DoNothing ||
                action == MoveType.FallThroughHole ||
                action == MoveType.MoveDown ||
                action == MoveType.MoveLeft ||
                action == MoveType.MoveRight ||
                action == MoveType.MoveUp;
        }
    }
}
