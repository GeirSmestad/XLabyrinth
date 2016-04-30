using LabyrinthEngine.Entities;
using LabyrinthEngine.Helpers;
using LabyrinthEngine.Geometry;
using LabyrinthEngine.Moves;
using LabyrinthEngine.Playfield;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabyrinthEngine.GameLogic
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
        private Move mostRecentlyExecutedMove;

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
            mostRecentlyExecutedMove = move;
            var player = move.PerformedBy;
            var action = move.ActionType;

            descriptionOfCurrentMove = new StringBuilder();

            if (player.IsOutsideLabyrinth())
            {
                descriptionOfCurrentMove.Append("You are outside, enjoying the sunshine. ");
                return;
            }

            WallSection wallToCross = null;
            player.PositionBeforePreviousMovementAction = new Position(player.X, player.Y);

            switch (action)
            {
                case MoveType.MoveUp:
                    wallToCross = board.GetWallAbove(player);
                    if (wallToCross.IsPassable)
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
                    wallToCross = board.GetWallBelow(player);
                    if (wallToCross.IsPassable)
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
                    wallToCross = board.GetWallLeftOf(player);
                    if (wallToCross.IsPassable)
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
                    wallToCross = board.GetWallRightOf(player);
                    if (wallToCross.IsPassable)
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
                    if (board.GetPlayfieldSquareOf(player).Type == SquareType.Teleporter)
                    {
                        var nextHole = board.GetPlayfieldSquareOf(player).Hole.NextHole;
                        player.X = nextHole.X;
                        player.Y = nextHole.Y;
                        descriptionOfCurrentMove.Append("You jump into the hole. ");
                    }
                    else
                    {
                        descriptionOfCurrentMove.Append("Standing still. ");
                    }
                    break;
                default:
                    throw new InvalidOperationException("Fall-through in game logic: "
                        + "Action should have been executed as a followup action.");
            }

            if (wallToCross != null && wallToCross.IsExit && wallToCross.IsPassable)
            {
                playerFoundTheExit(player);
            }
        }

        public void ResolveFollowupAction(Move move)
        {
            mostRecentlyExecutedMove = move;
            var action = move.ActionType;
            var player = move.PerformedBy;

            descriptionOfCurrentMove = new StringBuilder();

            if (player.IsOutsideLabyrinth())
            {
                var outsidePhase = player.OutsideLabyrinthPhase;
                if (outsidePhase == OutsideLabyrinthPhase.ExitedDuringThisMovementPhase)
                {
                    player.OutsideLabyrinthPhase = OutsideLabyrinthPhase.SkippingThisTurn;
                    descriptionOfCurrentMove.Append("You are outside and have to skip the next turn. ");
                    return;
                }
                else if (outsidePhase == OutsideLabyrinthPhase.SkippingThisTurn)
                {
                    player.OutsideLabyrinthPhase = OutsideLabyrinthPhase.Inside;
                    player.X = player.PositionBeforePreviousMovementAction.X;
                    player.Y = player.PositionBeforePreviousMovementAction.Y;

                    descriptionOfCurrentMove.Append("You re-enter the labyrinth, " +
                        "ready for action. ");
                    return;
                }
            }

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
                descriptionOfCurrentMove.Append("Doing nothing. ");
            }
            else
            {
                throw new InvalidOperationException("Fall-through in game logic: "
                + "Unhandled followup action.");
            }
        }


        public void ResolvePostMovementEventsFor(Player player)
        {
            if (player.IsOutsideLabyrinth()) { return; }

            var squareAfterMoving = board.GetPlayfieldSquareOf(player);

            if (squareAfterMoving.NumTreasures > 0)
            {
                if (player.IsAlive && !player.CarriesTreasure)
                {
                    player.CarriesTreasure = true;
                    squareAfterMoving.NumTreasures--;
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

            if (squareAfterMoving.Type == SquareType.FitnessStudio)
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

            if (squareAfterMoving.Type == SquareType.AmmoStorage)
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

            if (squareAfterMoving.Type == SquareType.HamsterStorage)
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

            if (squareAfterMoving.Type == SquareType.CementStorage)
            {
                if (player.IsAlive)
                {
                    player.NumCement = Player.CementCapacity;
                    descriptionOfCurrentMove.Append("You are in the cement storage and " +
                        "replenish your supply. ");
                }
                else
                {
                    descriptionOfCurrentMove.Append("You are in the cement storage. ");
                }
            }

            if (squareAfterMoving.Type == SquareType.Teleporter)
            {
                if (mostRecentlyExecutedMove.ActionType == MoveType.DoNothing ||
                    mostRecentlyExecutedMove.ActionType == MoveType.FallThroughHole)
                {
                    // Special case: Do nothing. Post-movement teleportation only happens when 
                    // player deliberately moved onto a new square -- not when waiting or
                    // immediately after teleporting.
                }
                else
                {
                    var nextHole = squareAfterMoving.Hole.NextHole;
                    player.X = nextHole.X;
                    player.Y = nextHole.Y;
                    descriptionOfCurrentMove.Append("You fell into a hole. ");
                }
            }

            if (isCentaurAdjacentTo(player))
            {
                descriptionOfCurrentMove.Append("Clop clop... ");
            }
        }

        public void ResolveEndOfTurnEvents()
        {
            moveCentaur();

            foreach (Player player in players)
            {
                if (isCentaurAdjacentTo(player))
                {
                    descriptionOfCurrentMove.AppendFormat("Clop clop, {0}... ", player.Name);
                }
            }

            var centaurVictims = findAllLivingPlayersAt(board.centaur.X, board.centaur.Y);
            foreach (Player player in centaurVictims)
            {
                killPlayer(player);
                descriptionOfCurrentMove.AppendFormat("{0} is savagely trampled by the centaur and dies. ",
                    player.Name);
            }

        }

        private void playerFoundTheExit(Player player)
        {
            player.OutsideLabyrinthPhase = OutsideLabyrinthPhase.ExitedDuringThisMovementPhase;
            if (player.CarriesTreasure)
            {
                player.CarriesTreasure = false;
                player.Score++;
                descriptionOfCurrentMove.AppendFormat("You have found the exit! You stow away your " +
                    "hard-earned treasure. You have {0} points. ", player.Score);
            }
            else
            {
                descriptionOfCurrentMove.Append("You are outside! You decide to take a " +
                    "stroll in the nice weather. ");
            }
        }

        private void handleWallConstruction(Move move)
        {
            var player = move.PerformedBy;
            var action = move.ActionType;
            WallSection targetWall;

            if (!player.IsAlive)
            {
                descriptionOfCurrentMove.Append("Dead people cannot handle construction equipment, unfortunately. ");
                return;
            }

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

            if (!player.IsAlive)
            {
                descriptionOfCurrentMove.Append("You are dead, and ghosts can't use hamster spray. ");
                return;
            }

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
                descriptionOfCurrentMove.Append("You are out of hamster spray. ");
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

            if (!player.IsAlive)
            {
                descriptionOfCurrentMove.Append("You are dead, and therefore cannot pick up or otherwise " +
                    "interact with hamsters. ");
                return;
            }

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
                descriptionOfCurrentMove.Append("Your hamster happily scurries into a crack in the masonry. ");
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

            if (!player.IsAlive)
            {
                descriptionOfCurrentMove.Append("You are dead, and hence unable to operate heavy weaponry. ");
                return;
            }

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
            else if (targetWall.IsExterior && !targetWall.IsExit)
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

            if (!player.IsAlive)
            {
                descriptionOfCurrentMove.Append("You are dead, and are hence currently unable to operate weaponry. ");
                return;
            }

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
                            // TODO: Encountered crash bug here. Try reproducing with dead shooter and live victim.
                            // I think I need stricter logic on what actions a dead player can perform.
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

        /// <summary>
        /// This method summarizes the logic of centaur movement, if you want to know.
        /// </summary>
        private void moveCentaur()
        {
            var centaur = board.centaur;

            var nextCentaurPosition = centaur.NextPositionInPath();

            if (!HelperMethods.AreAdjacent(centaur.X, centaur.Y,
                nextCentaurPosition.X, nextCentaurPosition.Y))
            {
                centaur.MoveToNextPositionInPath();
                return;
            }

            if (HelperMethods.NextMoveOfCentaurIsBlockedByWall(centaur, board))
            {
                if (nextCentaurPosition.IgnoreWallsWhenSteppingHere)
                {
                    centaur.MoveToNextPositionInPath();
                    return;
                }

                centaur.ReverseDirection();
                nextCentaurPosition = centaur.NextPositionInPath();
                if (HelperMethods.NextMoveOfCentaurIsBlockedByWall(centaur, board) &&
                       !nextCentaurPosition.IgnoreWallsWhenSteppingHere)
                {
                    return;
                }
            }

            centaur.MoveToNextPositionInPath();
        }

        private bool isCentaurAdjacentTo(Player player)
        {
            var centaur = board.centaur;
            return HelperMethods.AreAdjacent(centaur.X, centaur.Y, player.X, player.Y);
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
