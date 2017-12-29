using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using TheRandomWalk;
using Rectangle = System.Drawing.Rectangle;
using Point = System.Drawing.Point;

namespace TheRandomWalker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private enum MarkType
        {
            Row,
            Column
        }

        private Game game;
        private double cellSize = 25;
        private Ellipse lampPost;
        private Ellipse walker;

        private readonly DispatcherTimer timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(100) };
        private readonly SolidColorBrush markerBrush = new SolidColorBrush(Colors.Gray);


        public ICommand MoveCommand { get; }


        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            MoveCommand = new RelayCommand(o =>
            {
                int i;
                if (int.TryParse(o.ToString(), out i))
                    game.Move((Direction)i);
                else
                    throw new InvalidCastException("Unable to get int out of parameter");
            });
        }
        /// <summary>
        /// creates lines to mark out the grid
        /// </summary>
        /// <param name="maxMarkers"> the number of markes to draw (rows or columns)</param>
        /// <param name="type">Row Or Column</param>
        private void CreateGridMarkers(double maxMarkers, MarkType type)
        {
            for (int i = 1; i < maxMarkers; i++)
            {
                var line = new Line()
                {
                    Stroke = new SolidColorBrush(Colors.Black),
                    StrokeThickness = 1,
                };
                switch (type)
                {
                    case MarkType.Row:
                        line.X1 = 0;
                        line.X2 = DrawCanvas.ActualWidth;
                        Canvas.SetTop(line, i * cellSize);
                        break;
                    case MarkType.Column:
                        line.Y1 = 0;
                        line.Y2 = DrawCanvas.ActualHeight;
                        Canvas.SetLeft(line, i * cellSize);
                        break;
                }

                DrawCanvas.Children.Add(line);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            double rows = DrawCanvas.ActualHeight / cellSize;
            double columns = DrawCanvas.ActualWidth / cellSize;
            CreateGridMarkers(rows, MarkType.Row);
            CreateGridMarkers(columns, MarkType.Column);
            var boundaries = new Rectangle(0, 0, (int)DrawCanvas.ActualWidth, (int)DrawCanvas.ActualHeight);
            game = new Game(boundaries, cellSize, RenderObjects);
            game.Updated += Game_Updated;
            ;

        }

        private void RenderObjects(LampPost lp, Walker w)
        {
            lampPost = new Ellipse
            {
                Width = lp.Width,
                Height = lp.Height,
                Fill = new SolidColorBrush(Colors.Yellow),
                Visibility = Visibility.Collapsed,
            };
            walker = new Ellipse
            {
                Width = w.Width,
                Height = w.Height,
                Fill = new SolidColorBrush(Colors.RoyalBlue),
                Visibility = Visibility.Collapsed
            };
            DrawCanvas.Children.Add(lampPost);
            
            DrawCanvas.Children.Add(walker);
            //make the lamppost appear on top of every other object
            Canvas.SetZIndex(lampPost, 1000);
            //make the walker appear above everything but the lamppost
            Canvas.SetZIndex(walker, 900);
        }


        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            StartGame();

        }

        private void StartGame()
        {

            if (!debugCheckBox.IsChecked == true)
            {

                timer.Tick += Timer_Tick;
                timer.Start();
            }
            startButton.IsEnabled = false;
            stopButton.IsEnabled = true;
            game.Start(debugCheckBox.IsChecked ?? false);
            UpdateLocation(lampPost, game.LampPost.Location);
            UpdateLocation(walker, game.Walker.Location);

        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            game.Update();
        }

        private void Game_Updated(object sender, GameUpdatedEventArgs e)
        {

            //move the current walker
            UpdateLocation(walker, e.Mover.Location);
            DrawIfDoesntExist(e.Mover.PreviousLocation);
            if (!(sender is Game g)) return;
            if (g.Status == GameStatus.Finished)
            {
                debugTextBox.Text += $"{(string.IsNullOrWhiteSpace(debugTextBox.Text) ? "" : "\r\n")} {g.Moves} Moves and finished at {g.FinishLocation}";
                StopGame();
                StartGame();
            }
            if (debugCheckBox.IsChecked == true)
                debugTextBox.Text = $"Status:  \t{game.Status}\r\nMoves:    \t{game.Moves}\r\nLocation:\t{game.Walker.Location.X},{game.Walker.Location.Y}\r\nBoundaries:\t{game.MaxWidth},{game.MaxHight}";
        }



        private void DrawIfDoesntExist(Point point)
        {
            // draw a new circle at the previous location
            bool markerExists = false;
            foreach (UIElement child in DrawCanvas.Children)
            {
                if (!(child is Ellipse ellipse)) continue;
                var left = (int) Canvas.GetLeft(child);
                var top = (int)Canvas.GetTop(child);
                if ( left != point.X || top != point.Y) continue;
                if (Equals(ellipse.Fill, markerBrush))
                    markerExists = true;
            }
            if (!markerExists)
            {
                var history = new Ellipse()
                {
                    Width = cellSize,
                    Height = cellSize,
                    Fill = markerBrush
                };
                DrawCanvas.Children.Add(history);
                UpdateLocation(history, point);

            }


        }

        private void UpdateLocation(UIElement element, Point location)
        {

            element.Visibility = Visibility.Visible;
            Canvas.SetLeft(element, location.X);
            Canvas.SetTop(element, location.Y);
        }


        private void StopButton_OnClick(object sender, RoutedEventArgs e)
        {
            StopGame();
        }

        private void StopGame()
        {
            startButton.IsEnabled = true;
            stopButton.IsEnabled = false;
            timer?.Stop();
            timer.Tick -= Timer_Tick;
            for (int i = DrawCanvas.Children.Count - 1; i >= 0; i--)
            {
                var element = DrawCanvas.Children[i];
                if (element is Ellipse)
                {
                    DrawCanvas.Children.Remove(element);
                }
            }
            game.Stop();
            if (debugCheckBox.IsChecked == true)
                debugTextBox.Text = "";
        }

        private void DebugCheckBox_OnClick(object sender, RoutedEventArgs e)
        {
            if (debugCheckBox.IsChecked == true)
            {
                timer.Stop();
                game.EnableDebug();
            }
            else
            {
                timer.Start();
            }
        }
    }
}
