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
    private List<(int, int)> _livingCellsPositions = [];
    private Button[,] _buttons = new Button[100,100];
    private readonly Timer _timer = new Timer(1000);
    

    public MainWindow()
    {
        InitializeComponent();
        PrepGameBoard();
        _timer.Elapsed += OnTimedEvent;

    }

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

    private void ButtonClick(object sender, RoutedEventArgs e, int row, int column)
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

    private void OnTimedEvent(object? source, ElapsedEventArgs e)
    {
        Dispatcher.Invoke(UpdatingLifeCycle);
    }

    private void UpdatingLifeCycle()
    {
        var tempLivingCellsPositions = new List<(int, int)>(_livingCellsPositions);
        foreach (var position in _livingCellsPositions)
        {
            foreach (var neighbour in GetNeighbours(position.Item1, position.Item2))
            {
                if (CheckRules(neighbour.Item1, neighbour.Item2))
                {
                    if (!_livingCellsPositions.Contains(neighbour))
                    {
                        tempLivingCellsPositions.Add(neighbour);
                    }

                    _buttons[neighbour.Item1, neighbour.Item2].Background = new SolidColorBrush(Colors.Black);
                }
                else
                {
                    if (_livingCellsPositions.Contains(neighbour))
                    {
                        tempLivingCellsPositions.Remove(neighbour);
                    }

                    _buttons[neighbour.Item1, neighbour.Item2].Background = new SolidColorBrush(Colors.Transparent);
                }
            }
            if (CheckRules(position.Item1, position.Item2))
            {
                if (!_livingCellsPositions.Contains(position))
                {
                    tempLivingCellsPositions.Add(position);
                }

                _buttons[position.Item1, position.Item2].Background = new SolidColorBrush(Colors.Black);
            }
            else
            {
                if (_livingCellsPositions.Contains(position))
                {
                    Console.WriteLine(tempLivingCellsPositions.Contains(position));
                    tempLivingCellsPositions.Remove(position);
                    Console.WriteLine(tempLivingCellsPositions.Contains(position));
                }

                _buttons[position.Item1, position.Item2].Background = new SolidColorBrush(Colors.Transparent);
            }
        }

        _livingCellsPositions = tempLivingCellsPositions;
        tempLivingCellsPositions.Clear();
        //TODO Check why the list is not properly cleaned after each epoch
    }

    private bool CheckRules(int y, int x)
    {
        var startX = x - 1 < 0 ? x : x - 1;
        var endX = x + 1 > GamePanel.Columns-1 ? x : x + 1;
        var startY = y - 1 < 0 ? y : y - 1;
        var endY = y + 1 > GamePanel.Rows-1 ? y : y + 1;
        

        var livingNeighbours = 0;
        

        
        for (var i = startY; i <= endY; i++)
        {
            for (var j = startX; j <= endX; j++)
            {
                if(i == y && j == x)
                    continue;
                
                if(_livingCellsPositions.Contains((i, j)))
                {
                    livingNeighbours++;
                }
            }
        }

        return livingNeighbours switch
        {
            2 => _livingCellsPositions.Contains((y, x)),
            3 => true,
            _ => false
        };
    }

    private List<(int, int)> GetNeighbours(int y, int x)
    {
        var neighbours = new List<(int, int)>();
        
        var startX = x - 1 < 0 ? x : x - 1;
        var endX = x + 1 > GamePanel.Columns-1 ? x : x + 1;
        var startY = y - 1 < 0 ? y : y - 1;
        var endY = y + 1 > GamePanel.Rows-1 ? y : y + 1;

        
        for (var i = startY; i <= endY; i++)
        {
            for (var j = startX; j <= endX; j++)
            {
                if(i == y && j == x)
                    continue;
                neighbours.Add((i, j));
                
            }
        }

        return neighbours;
    }

    private Button GetButton(int row, int column)
    {

        var button = new Button
        {
            Background = new SolidColorBrush(Colors.Transparent),
            Name = $"Button_{row}_{column}",
            BorderThickness = new Thickness(0.2)
        };
        button.Click += (sender, e) => { ButtonClick(sender, e, row, column);};
        return button;
    }

    private void PlayPause(object sender, RoutedEventArgs e)
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
}