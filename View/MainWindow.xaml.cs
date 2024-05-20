using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using autoclicker.Model;

namespace autoclicker.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        
        
        private static readonly Regex Regex = MyRegex();
        
        public MainWindow()
        {
            InitializeComponent();
        }

        private void PreviewNumericInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = IsTextAllowed(e.Text);
        }
        
        private static bool IsTextAllowed(string text)
        {
            return Regex.IsMatch(text);
        }

        private void PastingNumericInput(object sender, DataObjectPastingEventArgs e)
        {
            if (!e.DataObject.GetDataPresent(typeof(string)))
                e.CancelCommand();
            else
            {
                var text = (string)e.DataObject.GetData(typeof(string));
                if (IsTextAllowed(text)) e.CancelCommand();
            }
        }

        [GeneratedRegex("[^0-9]+")]
        private static partial Regex MyRegex();
        
        private void Start_Click(object sender, RoutedEventArgs e)
        {
            AutoclickerViewModel.Instance.StartClicker();
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            AutoclickerViewModel.Instance.StopClicker();
        }

        private void MainWindow_OnClosing(object? sender, CancelEventArgs e)
        {
            AutoclickerViewModel.Instance.StopClicker();
        }
    }
}