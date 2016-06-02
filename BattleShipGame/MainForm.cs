using BattleShipGame.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BattleShipGame
{
    public partial class MainForm : Form
    {
        MainGame gra;
        public MainForm()
        {
            InitializeComponent();
            gra = new MainGame(this);
        }
    }
}
