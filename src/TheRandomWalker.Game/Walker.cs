using System;
using System.Drawing;
using System.Linq;

namespace TheRandomWalk
{
    public class Walker : Mover
    {

        public Walker(Game game, Point location) : base(game, location)
        {
        }

        public Point Move(Random rand)
        {
            //get a random direction either North or South or 3
            int min = Enum.GetValues(typeof(Direction)).Cast<int>().Min();
            int max = Enum.GetValues(typeof(Direction)).Cast<int>().Max();
            var randNumber = rand.Next(min, max + 1);
            var direction = (Direction)randNumber;
            var location = Move(direction);
            return location;
        }
    }
}
