using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MusicPlayer
{
    public static class MusicPlayerCommands
    {
        public static readonly RoutedUICommand OpenFile = new RoutedUICommand(
                "Open a media file for execution",
                nameof(OpenFile),
                typeof(MusicPlayerCommands),
                new InputGestureCollection { new KeyGesture(Key.A, ModifierKeys.Control, "Ctrl+A") }
                );

        public static readonly RoutedUICommand OpenFolder = new RoutedUICommand(
            "Open a folder to be the root of the list",
            nameof(OpenFolder),
            typeof(MusicPlayerCommands),
            new InputGestureCollection { new KeyGesture(Key.O, ModifierKeys.Control, "Ctrl+O") }
            );
    }
}
