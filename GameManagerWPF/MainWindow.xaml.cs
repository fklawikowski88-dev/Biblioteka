using System.Windows;
using GameManagerWPF.ViewModels;

namespace GameManagerWPF;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainViewModel();
    }
}
