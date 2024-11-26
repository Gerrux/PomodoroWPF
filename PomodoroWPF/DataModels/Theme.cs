using System.Collections.Generic;

namespace PomodoroWPF.DataModels
{
    public class Theme
    {
        public string Name { get; set; }
        public string TextColor { get; set; } // HEX color code
        public string BackgroundImagePath { get; set; } // Path to background image
    }

    public static class Themes
    {
        public static List<Theme> AvailableThemes = new List<Theme>
        {
            new Theme { Name = "Green", TextColor = "#00FF38", BackgroundImagePath = "/Assets/Backgrounds/classic/bg_green.png" },
            new Theme { Name = "Red", TextColor = "#FF0004", BackgroundImagePath = "/Assets/Backgrounds/classic/bg_red.png" },
            new Theme { Name = "Blue", TextColor = "#ADD8E6", BackgroundImagePath = "/Assets/Backgrounds/classic/bg_blue.png" },
            new Theme { Name = "Purple", TextColor = "#C5B4E3", BackgroundImagePath = "/Assets/Backgrounds/classic/bg_purple.png" },
            new Theme { Name = "Orange", TextColor = "#F87C00", BackgroundImagePath = "/Assets/Backgrounds/classic/bg_orange.png" },
            new Theme { Name = "White", TextColor = "#FFFFFF", BackgroundImagePath = "/Assets/Backgrounds/classic/bg_white.png" },
            // Custom theme will use white background
            new Theme { Name = "Custom", TextColor = "#FFFFFF", BackgroundImagePath = "/Assets/Backgrounds/classic/bg_white.png" }
        };
    }
}
