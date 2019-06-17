using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using MusicPlayer.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace MusicPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static readonly DependencyProperty RootProperty = DependencyProperty.Register(
            nameof(Root),
            typeof(FileSystemInfo),
            typeof(MainWindow),
            new FrameworkPropertyMetadata(
                null,
                new PropertyChangedCallback(OnRootChanged))
            );

        private static void OnRootChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is FileInfo info && info.Exists)
                (d as MainWindow).element.Source = new Uri(info.FullName);

            CommandManager.InvalidateRequerySuggested();
        }

        DispatcherTimer timer;

        public FileSystemInfo Root
        {
            get => (FileSystemInfo)GetValue(RootProperty);
            set => SetValue(RootProperty, value);
        }

        public MainWindow()
        {
            InitializeComponent();
            timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(100)
            };
            timer.Tick += TimerOnTick;
        }

        private void TimerOnTick(object sender, EventArgs e)
        {
            trackBar.Value = element.Position.Ticks;
            UpdateTimeLabel();
        }

        private void TrackBarOnMouseDown(object sender, MouseButtonEventArgs e)
        {
            (sender as ProgressBar).CaptureMouse();
            if (!playButton.IsChecked.GetValueOrDefault())
                return;

            element.Pause();
            timer.Stop();
        }

        private void TrackBarOnMouseUp(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(null);
            if (!playButton.IsChecked.GetValueOrDefault())
                return;

            element.Position = TimeSpan.FromTicks((long)trackBar.Value);
            element.Play();
            timer.Start();
        }

        private void BarOnMouseMove(object sender, MouseEventArgs e)
        {
            var bar = sender as ProgressBar;

            if (Mouse.Captured == bar)
            {
                var pos = e.GetPosition(bar);
                var range = bar.Maximum - bar.Minimum;
                bar.Value = Math.Max(bar.Minimum, Math.Min(bar.Maximum, pos.X / bar.ActualWidth * range));

                if (bar == trackBar)
                    UpdateTimeLabel();
            }
        }

        private void VolumeOnMouseDown(object sender, MouseButtonEventArgs e)
        {
            (sender as ProgressBar).CaptureMouse();
            tooltip.IsOpen = true;
        }

        private void VolumeOnMouseUp(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(null);
            tooltip.IsOpen = false;
        }

        private void PlayOnChecked(object sender, RoutedEventArgs e) => MediaCommands.Play.Execute(null, sender as IInputElement);

        private void PlayOnUnchecked(object sender, RoutedEventArgs e) => MediaCommands.Pause.Execute(null, sender as IInputElement);

        private void ElementOnMediaOpened(object sender, RoutedEventArgs e) => trackBar.Maximum = element.NaturalDuration.TimeSpan.Ticks;

        private void UpdateTimeLabel() => time.Text = $"{TimeSpan.FromTicks((long)trackBar.Value):m\\:ss} - {new TimeSpan((long)trackBar.Maximum):mm\\:ss}";

        private void ToggleOnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (playButton.IsChecked.GetValueOrDefault())
                MediaCommands.Play.Execute(e.Parameter, sender as IInputElement);
            else
                MediaCommands.Pause.Execute(e.Parameter, sender as IInputElement);
        }

        private void PlayOnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            element.Play();
            timer.Start();
        }

        private void PauseOnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            element.Pause();
            timer.Stop();
        }

        private void PreviousTrackOnExecuted(object sender, ExecutedRoutedEventArgs e)
        {

        }

        private void NextTrackOnExecuted(object sender, ExecutedRoutedEventArgs e)
        {

        }

        private void PreviousTrackOnCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;
        }

        private void NextTrackOnCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;
        }

        private void ToggleOnCanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = (Root?.Exists).GetValueOrDefault();

        private void IncreaseVolumeOnExecuted(object sender, ExecutedRoutedEventArgs e) => volumeBar.Value += volumeBar.SmallChange;

        private void DecreaseVolumeOnExecuted(object sender, ExecutedRoutedEventArgs e) => volumeBar.Value -= volumeBar.SmallChange;

        private void CloseOnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            AppSettings.Volume = volumeBar.Value;
            AppSettings.MediaSource = element.Source?.AbsolutePath;
            AppSettings.Save();

            Close();
        }

        private void OpenFileOnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var dlg = new OpenFileDialog
            {
                InitialDirectory = AppSettings.FileDirectory,
                Multiselect = true
            };

            if (!dlg.ShowDialog(this).GetValueOrDefault())
                return;

            AppSettings.FileDirectory = System.IO.Path.GetDirectoryName(dlg.FileName);

            if (dlg.FileNames.Length > 1)
                return;
            else
                Root = new FileInfo(dlg.FileName);
        }

        private void OpenFolderOnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var dlg = new CommonOpenFileDialog
            {
                IsFolderPicker = true,
                InitialDirectory = AppSettings.RootDirectory,
                Title = "Select Folder",
                EnsurePathExists = true
            };

            if (dlg.ShowDialog(this) != CommonFileDialogResult.Ok)
                return;

            AppSettings.RootDirectory = dlg.FileName;
            Root = new DirectoryInfo(dlg.FileName);
        }
    }
}
