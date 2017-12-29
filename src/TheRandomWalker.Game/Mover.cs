using System;
using System.Drawing;

namespace TheRandomWalk
{
    public abstract class Mover
    {
        private readonly Game game;
        private Point location;

        public Point Location
        {
            get { return location; }
            protected set
            {
                if (Equals(location, value)) return;
                PreviousLocation = location;
                location = value;

            }
        }

        public Point PreviousLocation { get; private set; }

        public Direction? LastDirection { get; private set; }

        protected Mover(Game game, Point location)
        {
            this.game = game;
            Location = location;
        }


        public bool Nearby(Point locationToCheck, int distance)
        {
            return Math.Abs(location.X - locationToCheck.X) <= distance &&
                   Math.Abs(location.Y - locationToCheck.Y) <= distance;
        }



        internal Point Move(Direction direction)
        {
            Point newLocation = Location;
            switch (direction)
            {
                case Direction.North:
                        newLocation.Y -= (int)game.CellSize;
                    break;
                case Direction.South:
                        newLocation.Y += (int)game.CellSize;
                    break;
                case Direction.East:
                        newLocation.X += (int)game.CellSize;
                    break;
                case Direction.West:
                        newLocation.X -= (int)game.CellSize;
                    break;
                case Direction.None:
                    break;
                case Direction.NorthWest:
                    newLocation.Y -= (int)game.CellSize;
                    newLocation.X -= (int)game.CellSize;
                    break;
                case Direction.NorthEast:
                    newLocation.Y -= (int)game.CellSize;
                    newLocation.X += (int)game.CellSize;
                    break;
                case Direction.SouthWest:
                    newLocation.Y += (int)game.CellSize;
                    newLocation.X -= (int)game.CellSize;
                    break;
                case Direction.SouthEast:
                    newLocation.Y += (int)game.CellSize;
                    newLocation.X += (int)game.CellSize;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }
            LastDirection = direction;
            Location = newLocation;
            return newLocation;
        }
    }
}
