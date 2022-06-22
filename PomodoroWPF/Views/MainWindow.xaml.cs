using System.Windows;
using System.Windows.Input;

namespace PomodoroWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        #region Draggable window
        protected void MoveWindow(object sender, MouseButtonEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
                this.DragMove();
        }
        #endregion

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
