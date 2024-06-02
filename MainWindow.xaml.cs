using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GameOfLife;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private bool[,] _gamePanelStates = new bool[100, 100];
    public MainWindow()
    {
        InitializeComponent();
        PrepGameBoard();
    }

    private void PrepGameBoard()
    {
        for (var i = 0; i < GamePanel.Rows; i++)
        {
            for (var j = 0; j < GamePanel.Columns; j++)
            {
                GamePanel.Children.Add(GetButton(i, j));
                _gamePanelStates[i, j] = false;
            }
        }
    }

    private void ButtonClick(object sender, RoutedEventArgs e, int row, int column)
    {
        var button = (Button)sender;

        if (_gamePanelStates[row, column] == false)
        {
            button.Background = new SolidColorBrush(Colors.Black);
            _gamePanelStates[row, column] = true;
        }
        else
        {
            button.Background = new SolidColorBrush(Colors.Transparent);
            _gamePanelStates[row, column] = false;
        }
            
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
}