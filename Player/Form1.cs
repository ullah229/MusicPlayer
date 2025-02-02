using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WMPLib;
using System.IO;
using System.Drawing.Text;
using System.Threading;
using System.Runtime.CompilerServices;

namespace Player
{
    public partial class Form1 : Form
    {
        private WMPLib.WindowsMediaPlayer wplayer = null;    
        private string[] files;    
        private bool musplay = false;
        private Image MyImage;
        
        public Form1()
        {
            InitializeComponent();
            timer1.Interval = 850;
            timer2.Interval = 850;
            
        }
        private void button1_Click(object sender, EventArgs e)
        {
              OpenFileDialog ofd = new OpenFileDialog();
                ofd.Multiselect = true;
                ofd.Filter = "(mp3,wav,mp4,mov,wmv,mpg)|*.mp3;*.wav;*.mp4;*.mov;*.wmv;*.mpg|all files|*.*";
                if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    files = ofd.FileNames;
                    for (int i = 0; i < files.Length; i++)
                    {
                        listBox1.Items.Add(files[i]);
                    }
                }
        }
        private void PlayFile(object sender, EventArgs e)
        {
            try
            {
                if (wplayer == null)
                {
                    wplayer = new WMPLib.WindowsMediaPlayer();
                    wplayer.URL = listBox1.Text;
                    wplayer.PlayStateChange += new WMPLib._WMPOCXEvents_PlayStateChangeEventHandler(wplayer_PlayStateChange);
                }
                musplay = !musplay;
                if (musplay)
                {
                    button2.Text = "Playing";
                    wplayer.controls.play();
                    wplayer.settings.volume = 5;
                    label4.Text = wplayer.currentMedia.getItemInfo("Title");
                    label2.Text = wplayer.currentMedia.durationString;
                    timer1.Start();
                }
                else
                {
                    wplayer.controls.pause();
                    button2.Text = "Paused";
                    timer1.Stop();
                }              
            }
            catch
            {
                label4.Text = "Invalid action";
            }
        }

        void wplayer_PlayStateChange(int NewState)
        {
            try
            {
                if (wplayer.playState == WMPLib.WMPPlayState.wmppsMediaEnded)
                {
                    listBox1.SelectedIndex = listBox1.SelectedIndex + 1;
                    wplayer.URL = listBox1.Text;
                    label2.Text = wplayer.currentMedia.durationString;
                    label4.Text = wplayer.currentMedia.getItemInfo("Title");
                    timer2.Enabled = true;
                }
            }
            catch
            {
                if (wplayer.playState == WMPLib.WMPPlayState.wmppsMediaEnded)
                {
                    listBox1.SelectedIndex = -1;
                    listBox1.SelectedIndex = listBox1.SelectedIndex + 1;
                    timer2.Enabled = true;
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                wplayer.controls.stop();
                wplayer = null;
                label2.Text = "0";
                label3.Text = "0";
            }
            catch
            {
                label4.Text = "Invalid action";
            }
        }

        private void listBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (wplayer == null)
            {
                wplayer = new WMPLib.WindowsMediaPlayer();     
                wplayer.URL = listBox1.Text;
                label5.Text = wplayer.settings.volume.ToString();
                wplayer.PlayStateChange += new WMPLib._WMPOCXEvents_PlayStateChangeEventHandler(wplayer_PlayStateChange);
            }
            musplay = !musplay;
            if (musplay || listBox1.SelectedItem != listBox1.SelectedItem)
            {
                button2.Text = "Playing";
                wplayer.controls.play();
                wplayer.settings.volume = 5;
                label4.Text = wplayer.currentMedia.getItemInfo("Title");
                label2.Text = wplayer.currentMedia.durationString;
                timer1.Start();
            }
            else
            {
                button2.Text = "Paused";
                wplayer.controls.stop();
                wplayer = null;
                timer1.Stop();
            }       
        }       

        private void VolumeUp(object sender, EventArgs e)
        {
            try
            {
                if (wplayer.settings.volume < 100)
                {
                    wplayer.settings.volume = wplayer.settings.volume + 5;
                    label5.Text = wplayer.settings.volume.ToString();
                }
            }
            catch
            {
                label4.Text = "No song volume to adjust";
            }
        }

        private void VolumeDown(object sender, EventArgs e)
        {
            try
            {
                if (wplayer.settings.volume >= 0)
                {
                    wplayer.settings.volume = wplayer.settings.volume - 5;
                    label5.Text = wplayer.settings.volume.ToString();
                }
            }
            catch
            {
                label4.Text = "No song volume to adjust";
            }
        }

        private void timer1_Tick_1(object sender, EventArgs e)
        {
            try
            {
                label3.Text = wplayer.controls.currentPositionString;
                label2.Text = wplayer.currentMedia.durationString;
            }
            catch
            {
                label4.Text = "Error";
            }
        }

        private void skipbutton(object sender, EventArgs e)
        {
            try
            { 
                listBox1.SelectedIndex = listBox1.SelectedIndex + 1;
                wplayer.URL = listBox1.Text;
                label2.Text = wplayer.currentMedia.durationString;
                label4.Text = wplayer.currentMedia.getItemInfo("Title");
                wplayer.controls.play();
                
            }
            catch 
            {    
                listBox1.SelectedIndex = -1;
                listBox1.SelectedIndex = listBox1.SelectedIndex + 1;
                wplayer.URL = listBox1.Text;
                label2.Text = wplayer.currentMedia.durationString;
            }       
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            wplayer.controls.play();
            timer2.Enabled = false;
        }
    }
}
// Hello