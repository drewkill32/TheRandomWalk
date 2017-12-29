using System;

namespace TheRandomWalk
{
    public class GameUpdatedEventArgs:EventArgs
    {
        public Mover Mover { get; }
        public GameStatus Status { get; }


        public GameUpdatedEventArgs(Mover move,GameStatus status)
        {
            Mover = move;
            Status = status;
        }
    }
}
