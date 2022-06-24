using System.Windows.Forms;

namespace PomodoroWPF.Services
{
    public interface INotificationService
    {
        void Notify(string title, string message, int timeout, ToolTipIcon icon);
    }
}
