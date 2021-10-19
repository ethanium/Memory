using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Memory
{
    /// <summary>
    /// TODO: Show only 2 cards
    /// </summary>
    public partial class MainForm : Form
    {
        private const int ORIGINAL_WIDTH = 1277;
        private const int ORIGINAL_HEIGHT = 784;

        public string Card1;
        public string Card2;

        public DateTime GameStarted;
        public DateTime GameEnded;

        public MainForm()
        {
            InitializeComponent();

            MainTableLayoutPanel.Dock = DockStyle.Fill;
            foreach (object control in MainTableLayoutPanel.Controls)
            {
                Button button = (Button)control;
                button.BackgroundImageLayout = ImageLayout.Stretch;
                button.Dock = DockStyle.Fill;
                button.Text = "";
                button.Tag = "";
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            StartGame();
        }

        private void StartGame()
        {
            List<char> availableCards = new List<char>()
            {
                'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n'
            };
            availableCards.AddRange(availableCards);
            availableCards = availableCards.OrderBy(a => Guid.NewGuid()).ToList();

            for (int i = 0; i < MainTableLayoutPanel.Controls.Count; i++)
            {
                Button button = (Button)MainTableLayoutPanel.Controls[i];
                button.Tag = $"{availableCards[i]}";
                button.BackgroundImage = null;
                button.Enabled = true;
            }

            CloseCardTimer.Enabled = false;

            GameStarted = DateTime.Now;
        }

        private void newGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StartGame();
        }

        private void Button_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Card2))
            {
                Button button = sender as Button;

                string filename = Path.Combine(Application.StartupPath, "Images", "SquidGame", $"{button.Tag}.jpg");
                button.BackgroundImage = Image.FromFile(filename);

                if (string.IsNullOrWhiteSpace(Card1))
                {
                    Card1 = button.Name;
                }
                else
                {
                    if (Card1 != button.Name)
                    {
                        Card2 = button.Name;

                        Button firstButton = MainTableLayoutPanel.Controls.OfType<Button>().FirstOrDefault(x => x.Name == Card1);

                        if (firstButton.Tag.ToString() == button.Tag.ToString())
                        {
                            firstButton.Enabled = false;
                            button.Enabled = false;

                            if (!MainTableLayoutPanel.Controls.OfType<Button>().Any(x => x.Enabled))
                            {
                                GameEnded = DateTime.Now;
                                int minutes = (int)(GameEnded - GameStarted).TotalMinutes;
                                int seconds = (GameEnded - GameStarted).Seconds;
                                MessageBox.Show($"{minutes:n0} minute{(minutes > 1 ? "s" : "")}" + (seconds > 0 ? $" and {seconds:n0} second{(seconds > 1 ? "s" : "")}" : ""));

                                StartGame();
                            }
                        }

                        CloseCardTimer.Start();
                    }
                }
            }
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Normal)
            {
                Size = new Size() { Width = ORIGINAL_WIDTH, Height = ORIGINAL_HEIGHT };
            }
        }

        private void MainForm_ResizeBegin(object sender, EventArgs e)
        {

        }

        private void MainForm_ResizeEnd(object sender, EventArgs e)
        {

        }

        private void CloseCardTimer_Tick(object sender, EventArgs e)
        {
            CloseCardTimer.Stop();

            MainTableLayoutPanel.Controls.OfType<Button>().Where(x => x.Enabled).ToList().ForEach(x => x.BackgroundImage = null);

            Card1 = null;
            Card2 = null;
        }
    }
}
