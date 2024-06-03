using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Timer = System.Timers.Timer;

namespace GameOfLife;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    /// <summary>
    /// List of all living cells
    /// </summary>
    private List<(int, int)> _livingCellsPositions = [];
    
    /// <summary>
    /// Array of all buttons
    /// To change number of cells on screen change size of this array and rows and columns of UniformGrid in XAML file
    /// </summary>
    private readonly Button[,] _buttons = new Button[80,80];
    
    /// <summary>
    /// Timer which calls updating life cycle
    /// </summary>
    private readonly Timer _timer = new Timer(100);
    
    /// <summary>
    /// Main function of WPF
    /// </summary>
    public MainWindow()
    {
        InitializeComponent();
        PrepGameBoard();
        _timer.Elapsed += OnTimedEventEventFunction;

    }
    /// <summary>
    /// Function for generating buttons that will be used as cells
    /// </summary>
    private void PrepGameBoard()
    {
        for (var i = 0; i < GamePanel.Rows; i++)
        {
            for (var j = 0; j < GamePanel.Columns; j++)
            {
                var button = GetButton(i, j);
                GamePanel.Children.Add(button);
                _buttons[i, j] = button;
            }
        }
    }
   
    /// <summary>
    /// Main function that updates life cycle
    /// </summary>
    private void UpdateLifeCycle()
    {
        var tempLivingCellsPositions = new List<(int, int)>(_livingCellsPositions);
        foreach (var position in _livingCellsPositions)
        {
            foreach (var neighbour in GetNeighbours(position.Item1, position.Item2))
            {
                if (CheckRules(neighbour.Item1, neighbour.Item2))
                {
                    if (!tempLivingCellsPositions.Contains(neighbour))
                    {
                        tempLivingCellsPositions.Add(neighbour);
                    }

                    _buttons[neighbour.Item1, neighbour.Item2].Background = new SolidColorBrush(Colors.Black);
                }
                else
                {
                    if (tempLivingCellsPositions.Contains(neighbour))
                    {
                        tempLivingCellsPositions.Remove(neighbour);
                    }

                    _buttons[neighbour.Item1, neighbour.Item2].Background = new SolidColorBrush(Colors.Transparent);
                }
            }
            if (CheckRules(position.Item1, position.Item2))
            {
                if (!tempLivingCellsPositions.Contains(position))
                {
                    tempLivingCellsPositions.Add(position);
                }

                _buttons[position.Item1, position.Item2].Background = new SolidColorBrush(Colors.Black);
            }
            else
            {
                if (tempLivingCellsPositions.Contains(position))
                {
                    tempLivingCellsPositions.Remove(position);
                }

                _buttons[position.Item1, position.Item2].Background = new SolidColorBrush(Colors.Transparent);
            }
        }

        _livingCellsPositions = tempLivingCellsPositions;
    }

    /// <summary>
    /// Function for checking rules of Conway's Game of Life
    /// 1. If cell has less than 2 neighbours it dies from isolation
    /// 2. If cell has 2 or 3 neighbours and is alive it lives to the next generation
    /// 3. If cell has more than 3 neighbours it dies from overpopulation
    /// 4. If cell is dead and has 3 living neighbours it becomes living cell
    /// </summary>
    /// <param name="row">the row position of cell that is being checked</param>
    /// <param name="column">the column position of cell that is being checked</param>
    /// <returns></returns>
    private bool CheckRules(int row, int column)
    {
        var livingNeighbours = GetNeighbours(row, column)
            .Count(neighbour => _livingCellsPositions.Contains(neighbour));

        return livingNeighbours switch
        {
            2 => _livingCellsPositions.Contains((row, column)),
            3 => true,
            _ => false
        };
    }

    /// <summary>
    /// Function to get all neighbours of a cell
    /// </summary>
    /// <param name="row">the row position of cell that is being checked</param>
    /// <param name="column">the column position of cell that is being checked</param>
    /// <returns></returns>
    private List<(int, int)> GetNeighbours(int row, int column)
    {
        var neighbours = new List<(int, int)>();
        
        var startX = column - 1 < 0 ? column : column - 1;
        var endX = column + 1 > GamePanel.Columns-1 ? column : column + 1;
        var startY = row - 1 < 0 ? row : row - 1;
        var endY = row + 1 > GamePanel.Rows-1 ? row : row + 1;

        
        for (var i = startY; i <= endY; i++)
        {
            for (var j = startX; j <= endX; j++)
            {
                if(i == row && j == column)
                    continue;
                neighbours.Add((i, j));
                
            }
        }

        return neighbours;
    }

    /// <summary>
    /// Function for getting button that represents cell from position
    /// </summary>
    /// <param name="row">the row position of cell whose button is needed</param>
    /// <param name="column">the column position of cell whose button is needed</param>
    /// <returns></returns>
    private Button GetButton(int row, int column)
    {

        var button = new Button
        {
            Background = new SolidColorBrush(Colors.Transparent),
            Name = $"Button_{row}_{column}",
            BorderThickness = new Thickness(0.2)
        };
        button.Click += (sender, e) => { ButtonClickEventFunction(sender, e, row, column);};
        button.MouseEnter += (sender, e) => { UpdateCordsEventFunction(sender, e, row, column);};
        return button;
    }

    /// <summary>
    /// Function that is used to play or pause life cycles
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void PlayPauseEventFunction(object sender, RoutedEventArgs e)
    {
        if (_timer.Enabled)
        {
            PlayButton.Content = "Play";
            _timer.Enabled = false;
        }
        else
        {
            PlayButton.Content = "Pause";
            _timer.Enabled = true;
        }
    }
    
    /// <summary>
    /// Function that is used to change life cycles interval 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void SpeedChangeEventFunction(object sender, TextChangedEventArgs e)
    {
        if (Double.TryParse(Speed.Text, out var interval))
        {
            
            _timer.Interval = interval >= 50 ? interval : 50;
        }
    }
    
    /// <summary>
    /// Function that is used to make cell alive or dead depending on button clicked
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <param name="row">the row position of button which was clicked</param>
    /// <param name="column">the column position of button which was clicked</param>
    private void ButtonClickEventFunction(object sender, RoutedEventArgs e, int row, int column)
    {
        var button = (Button)sender;

        if (!_livingCellsPositions.Contains((row, column)))
        {
            button.Background = new SolidColorBrush(Colors.Black);
            _livingCellsPositions.Add((row, column));
        }
        else
        {
            button.Background = new SolidColorBrush(Colors.Transparent);
            _livingCellsPositions.Remove((row, column));
        }
    }

    /// <summary>
    /// Function that is called by a timer each interval, it calls function to update life cycle
    /// Uses Dispatcher.Invoke to call 
    /// </summary>
    /// <param name="source"></param>
    /// <param name="e"></param>
    private void OnTimedEventEventFunction(object? source, ElapsedEventArgs e)
    {
        Dispatcher.Invoke(UpdateLifeCycle);
    }
    
    /// <summary>
    /// Function to update text which shows on which cords user's mouse is
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <param name="row">row position of a button over which is mouse cursor</param>
    /// <param name="column">column position of a button over which is mouse cursor</param>
    private void UpdateCordsEventFunction(object sender, EventArgs e, int row, int column)
    {
        Cords.Text = $"{row}; {column}";
    }
}