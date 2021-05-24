using System;
using System.Windows;
using System.IO;
using Microsoft.Win32;
using System.Windows.Threading;
using System.Windows.Controls;


namespace Playerwpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public WMPLib.WindowsMediaPlayer wplayer = null;
        public string[] files;
        public bool musplay = false;
        DispatcherTimer Songtimer = new DispatcherTimer();
        DispatcherTimer Songtrigger = new DispatcherTimer();

        public MainWindow()
        {
            InitializeComponent();
            Skipbtn.IsEnabled = false;
            Playbtn.IsEnabled = false;
            Songtimer.Tick += ZeitmesserTick;
            Songtimer.Interval = new TimeSpan(0, 0, (int)0.85);
            Songtrigger.Tick += TriggerTick;
            Songtrigger.Interval = new TimeSpan(0, 0, (int)0.85);
            Volumebar.Value = 50;
        }
        private void Addbuttonclick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = true;
            ofd.Filter = "(mp3,wav, ogg)|*.mp3;*.wav;*.ogg;.*";
            if (ofd.ShowDialog() == true)
            {
                files = ofd.FileNames;
                for (int i = 0; i < files.Length; i++)
                {
                    Songlist.Items.Add(files[i]);
                }
                Playbtn.IsEnabled = true;
            }
        }
        private void PlayFile(object sender, RoutedEventArgs e)
        {
            try
            {
                if (wplayer == null)
                {
                    wplayercrt();
                }
                musplay = !musplay;
                if (musplay)
                {
                    BSkipbtn.IsEnabled = true;
                    Skipbtn.IsEnabled = true;
                    Play();
                }
                else
                {
                    Pause();
                }
            }
            catch
            {
                Titlelbl.Content = "Invalid action";
            }
        }

        private void StopButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                BSkipbtn.IsEnabled = false;
                Skipbtn.IsEnabled = false;
                wplayer.controls.stop();
                Playbtn.Content = FindResource("Play");
                Durationlbl.Content = "00:00";
                CurrentPosLbl.Content = "00:00";
                Songtimer.IsEnabled = false;              
            }
            catch
            {
                Titlelbl.Content = "Invalid action";
            }
        }
        private void ZeitmesserTick(object sender, EventArgs e)
        {
            try
            {
                CurrentPosLbl.Content = wplayer.controls.currentPositionString;
                Durationlbl.Content = wplayer.currentMedia.durationString;
                SongPosBar.Value = (int)wplayer.controls.currentPosition;
                SongPosBar.Maximum = (int)wplayer.currentMedia.duration;
            }
            catch
            {
                Titlelbl.Content = "Error";
            }
        }
        private void TriggerTick(object sender, EventArgs e)
        {
            wplayer.controls.play();
            Songtrigger.IsEnabled = false;
        }
        private void skipbutton(object sender, RoutedEventArgs e)
        {
            try
            {
                SkipForward();
                wplayer.URL = Songlist.SelectedItem.ToString();
                Songlist.ScrollIntoView(Songlist.SelectedItem);
                Durationlbl.Content = wplayer.currentMedia.durationString;
                Titlelbl.Content = wplayer.currentMedia.getItemInfo("Title");
                wplayer.controls.play();
                if (Songlist.SelectedIndex > Songlist.Items.Count - 1)
                {
                    Songlist.SelectedIndex = -1;
                }
            }
            catch
            {
                Titlelbl.Content = "no";
            }
        }
        private void skipbackbutton(object sender, RoutedEventArgs e)
        {
            if (wplayer != null)
            {
                if (Songlist.SelectedIndex > 0)
                {
                    Songlist.ScrollIntoView(Songlist.SelectedItem);
                    Songlist.SelectedIndex = Songlist.SelectedIndex - 1;
                    wplayer.URL = Songlist.SelectedItem.ToString();                  
                    Durationlbl.Content = wplayer.currentMedia.durationString;
                    Titlelbl.Content = wplayer.currentMedia.getItemInfo("Title");
                    wplayer.controls.play();
                }
            }
        }

            private void VolumeDown(object sender, RoutedEventArgs e)
        {
            if (wplayer != null)
            {
                wplayer.settings.volume = wplayer.settings.volume - 5;
                volumetxt();
            }
        }

        private void VolumeUp(object sender, RoutedEventArgs e)
        {
            if (wplayer != null)
            {
                wplayer.settings.volume = wplayer.settings.volume + 5;
                volumetxt();
            }
        }
        public void wplayer_PlayStateChange(int NewState)
        {

            if (wplayer.playState == WMPLib.WMPPlayState.wmppsMediaEnded)
            {
                SkipForward();
                wplayer.URL = Songlist.SelectedItem.ToString();
                Songlist.ScrollIntoView(Songlist.SelectedItem);
                Durationlbl.Content = wplayer.currentMedia.durationString;
                Volumebar.Value = wplayer.settings.volume;
                Titlelbl.Content = wplayer.currentMedia.getItemInfo("Title");
                Songtrigger.IsEnabled = true;
            }
        }
        private void Songlist_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (Songlist.Items.Count > 0)
            {
                if (wplayer == null)
                {
                    wplayercrt();
                }
                musplay = !musplay;
                if (musplay || Songlist.SelectedItem != Songlist.SelectedItem)
                {
                    Play();
                    BSkipbtn.IsEnabled = true;
                    Skipbtn.IsEnabled = true;
                    volumetxt();
                }
                else
                {
                    Pause();
                    wplayer = null;
                }
            }
        }

        private void clearbtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Songlist.Items.Count > 0)
                {
                    Songlist.Items.Clear();
                    wplayer.controls.stop();
                    Application.Current.Shutdown();
                    System.Windows.Forms.Application.Restart();                 
                }
            }
            catch
            {
                Titlelbl.Content = "Error!";
            }
        }
        private void Voluembar_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
        {
            try
            {
                if (Volumebar.Value < 100)
                {
                    volumetxt();
                }
                if (Volumebar.Value < 0)
                {
                    volumetxt();
                }
            }
            catch
            {
                Titlelbl.Content = "No song to adjust";
            }
        }

        private void SkipForward()
        {
            if (Songlist.SelectedIndex < Songlist.Items.Count - 1)
            {
                Songlist.SelectedIndex = Songlist.SelectedIndex + 1;
            }
            else
            {
                Songlist.SelectedIndex = 0;
            }
        }
        public void Pause()
        {
            wplayer.controls.pause();
            Playbtn.Content = FindResource("Play");
            Songtimer.Stop();
        }
        public void Play()
        {
            wplayer.controls.play();
            Playbtn.Content = FindResource("Pause");
            Titlelbl.Content = wplayer.currentMedia.getItemInfo("Title");
            SongPosBar.Value = (int)wplayer.controls.currentPosition;
            Durationlbl.Content = wplayer.currentMedia.durationString;
            Songtimer.Start();
        }
        public void wplayercrt()
        {
            wplayer = new WMPLib.WindowsMediaPlayer();
            wplayer.URL = Songlist.SelectedItem.ToString();
            Skipbtn.IsEnabled = true;
            Playbtn.IsEnabled = true;
            Stopbtn.IsEnabled = true;
            wplayer.PlayStateChange += new WMPLib._WMPOCXEvents_PlayStateChangeEventHandler(wplayer_PlayStateChange);
        }
        public void volumetxt()
        {
            if (wplayer != null)
            {
                wplayer.settings.volume = (int)Volumebar.Value;
                Volumelbl.Content = wplayer.settings.volume.ToString();
            }
        }

        private void ScrollBar_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
        {
            if (wplayer != null)
            {
                wplayer.controls.currentPosition = SongPosBar.Value;
                SongPosBar.Maximum = (int)wplayer.currentMedia.duration;
            }
        }

        private void Volumebar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (wplayer != null)
            {
                wplayer.settings.volume = (int)Volumebar.Value;
                Volumelbl.Content = wplayer.settings.volume.ToString();
            }
        }

        private void Powerbtnclick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Minimizebtnclick(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if(e.ChangedButton == System.Windows.Input.MouseButton.Left)
            {
                this.DragMove();
            }
        }
    }
}
