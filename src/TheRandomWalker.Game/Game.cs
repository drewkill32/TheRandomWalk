using System;
using System.Diagnostics;
using System.Drawing;

namespace TheRandomWalk
{

    public class Game
    {
        public event GameUpdatedEventHandler Updated;


        private readonly Random rand = new Random();

        private Walker walker;
        private LampPost lampPost;
        


        public GameStatus Status { get; private set; }
        public GameLocation? FinishLocation { get; private set; }

        public double MaxWidth => boundries.Width;

        public double MaxHight => boundries.Height;

        public LampPost LampPost => lampPost;

        public Walker Walker => walker;

        public int Moves { get; private set; }

        private readonly Rectangle boundries;
        private readonly Action<LampPost, Walker> renderAction;

        public double CellSize { get; set; }


        public Game(Rectangle boundries, double cellSize,Action<LampPost,Walker> renderAction)
        {

            this.boundries = boundries;
            this.renderAction = renderAction;
            this.CellSize = cellSize;
            this.boundries = boundries;
            Status = GameStatus.NotStarted;
        }


        public void Start(bool debug)
        {
            if (Status == GameStatus.Running || Status == GameStatus.Debug)
                throw new Exception("The Game has already started\r\nDelete the game and start again");
            Point midPoint = new Point(boundries.Width / 2, boundries.Height / 2);
            lampPost = new LampPost(this, midPoint)
            {
                Height = CellSize,
                Width = CellSize
            };

            //create the new walker at the midpoint of the grid and move it NSEW
            walker = new Walker(this, midPoint)
            {
                Height = CellSize,
                Width = CellSize
            };
            do
            {
                walker.Move(rand);
            } while (walker.LastDirection == Direction.None);
            renderAction(lampPost, walker);
            Status = debug ? GameStatus.Debug : GameStatus.Running;
        }

        public void Stop()
        {
            lampPost = null;
            walker = null;
            Status = GameStatus.NotStarted;
            Moves = 0;
            FinishLocation = null;
        }

        public void Start()
        {
            Start(false);
        }

        private Point Move()
        {
            var point = walker.Move(rand);
            //moveHistory.Add(point);
            return point;
        }

        public void Move(Direction direction)
        {
            if (Status != GameStatus.Debug)
                throw new Exception("Move with a direction can only be used in Debug mode");
            walker.Move(direction);
            Update();
        }

        public void EnableDebug()
        {
            Status = GameStatus.Debug;
        }

        public void DisableDebug()
        {
            Status = GameStatus.Running;
        }


        public void Update()
        {

            if (Status != GameStatus.Debug)
                Move();
            if (Status != GameStatus.Running && Status != GameStatus.Debug) return;
            Moves++;
            if (GameOver())
            {
                Status = GameStatus.Finished;
            }
            OnGameUpdated(new GameUpdatedEventArgs(walker, Status));
        }

        private bool GameOver()
        {
            bool finished = false;
            //waler returned to lampost

            if (Walker.Nearby(LampPost.Location,0))
            {
                
                FinishLocation = GameLocation.LampPost;
                finished= true;
            }

            //if walker is outside the bounds of the boundaries
            if (Walker.Location.X < 0)
            {
                FinishLocation = GameLocation.LeftEdge;
                finished = true;
            }
            if (Walker.Location.X >= boundries.Width)
            {
                FinishLocation = GameLocation.RightEdge;
                finished = true;
            }
            if (Walker.Location.Y < 0)
            {
                FinishLocation = GameLocation.TopEdge;
                finished = true;
            }
            if (Walker.Location.Y >= boundries.Height)
            {
                FinishLocation = GameLocation.BottomEdge;
                finished = true;
            }


            return finished;
        }

        protected virtual void OnGameUpdated(GameUpdatedEventArgs args)
        {
            Updated?.Invoke(this, args);
        }
    }

    public delegate void GameUpdatedEventHandler(object sender, GameUpdatedEventArgs args);
}
