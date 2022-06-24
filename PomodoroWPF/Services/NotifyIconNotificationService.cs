using System.Windows.Forms;

namespace PomodoroWPF.Services
{
    public class NotifyIconNotificationService : INotificationService
    {
        private readonly NotifyIcon _notifyIcon;
        public NotifyIconNotificationService(NotifyIcon notifyIcon)
        {
            _notifyIcon = notifyIcon;
        }
        public void Notify(string title, string message, int timeout, ToolTipIcon icon)
        {
            _notifyIcon.ShowBalloonTip(timeout, title, message, icon);
        }
    }
}
