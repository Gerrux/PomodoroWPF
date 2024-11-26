using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PomodoroWPF.DataModels
{
    public enum GifMode
    {
        Stay,   // режим паузы
        Rest,   // режим отдыха
        Run,    // режим работы (0-33% и 66-100%)
        FastRun // режим работы (33-66%)
    }
}
