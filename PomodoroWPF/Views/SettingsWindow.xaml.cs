using PomodoroWPF.ViewModels; // Add reference to System.Windows.Forms
using System.Windows;
using System.Windows.Input;

namespace PomodoroWPF.Views
{
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
            this.DataContext = new SettingsWindowViewModel();
        }

        private void InputDigit_PTI(object sender, TextCompositionEventArgs e)
        {
            CheckIsNumeric(e);
        }

        private void CheckIsNumeric(TextCompositionEventArgs e)
        {
            if (!int.TryParse(e.Text, out _))
            {
                e.Handled = true;
            }
        }
    }
}
