using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BattleShipGame
{
    public partial class AboutProgramForm : Form
    {
        private bool click = false;
        public AboutProgramForm()
        {
            InitializeComponent();
            Icon = Properties.Resources.JKSicon;
            button1.AutoSize = true;
        }

        private void label5_Click(object sender, EventArgs e)
        {
            label5.Text += " (Joker KS)";
            label5.Click -= label5_Click;
            pictureBox1.Image = Properties.Resources.programist;
            SecretClick();
        }

        private void label6_Click(object sender, EventArgs e)
        {
            label6.Text += " (Sofiiicandy)";
            label6.Click -= label6_Click;
            pictureBox1.Image = Properties.Resources.designer;
            SecretClick();
        }

        private void SecretClick()
        {
            if (!click)
            {
                this.Opacity = 95;
                this.Width = 600;
                click = !click;
                button1.Text = "Ja nic nie widzia-łem/lam";
                button1.Location = new Point(this.Width / 2 - button1.Width / 2, button1.Location.Y);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
