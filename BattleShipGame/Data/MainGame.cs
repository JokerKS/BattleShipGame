using BattleShipGame.Date;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading;
using System.Windows.Forms;


namespace BattleShipGame.Data
{
    class MainGame
    {
        #region Всі потрібні дані і конструктори
        //всі елементи, які будуть створені і відображені динамічно
        MainForm form;
        private TableLayoutPanel tableLayoutPanel1, tableLayoutPanel2, tableLayoutPanel3;
        private Panel panel1;
        private Button btn1, btn2, btn3, btn4, btn5, btn6, btn7;
        private PictureBox pictureBox1, pictureBox2;
        private NumericUpDown numUpDown;
        private Label label, label2, label3;
        private TextBox txt1, txt2;
        private FlowLayoutPanel flowpanel;
        ToolTip t;

        private MenuStrip menuStrip1;
        private ToolStripMenuItem mainToolStripMenuItem;
        private ToolStripMenuItem powrótDoMenuToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem CloseToolStripMenuItem;
        private ToolStripMenuItem OProgramieToolStripMenuItem;
        private ToolStripMenuItem SoundToolStripMenuItem;
        private ToolStripMenuItem SoundOffToolStripMenuItem;
        private ToolStripMenuItem SoundOnToolStripMenuItem;

        private byte page;

        //для звуків
        System.Media.SoundPlayer player;
        bool musicFlag = true;

        //для малювання
        Bitmap btm1, btm2;
        Graphics formGraphics1, formGraphics2;

        //статус програми
        private enum GameStatus : byte { Menu = 0, Preparation = 1, Game = 2, Finish = 3 };
        private GameStatus status_game;

        //розмір клітинки
        private int size_cell;
        //кількість клітинок для гри
        private byte size_board;

        //масив, в якому записані розміри і кількість кораблів
        byte[,] NumberANDsize_ship_szablon;

        //для визначення чий зараз хід
        bool who_goes;

        //діючі особи
        Gamer act1, act3;
        Computer act2;

        //кораблі для процесу розстановки
        Ship[] ex_ship;
        //тип активного корабля
        bool type_active_ship = true;
        //тимчасові координати клітинки
        int tmpx = -1, tmpy = -1;

        //кількість ходів
        int number_of_moves;

        //Режими:
        //0 - режим ще не вибраний
        //1 - гра проти комп'ютера
        //2 - гра один проти одного
        byte mode;
        
        
        //Конструктор, який приймає форму, встановлює її розмір
        //встановлює хто ходить, змінює статус і викликає функцію для створення меню
        public MainGame(MainForm f)
        {
            form = f;
            form.MinimumSize = new Size(600, 450);
            form.Size = form.MinimumSize;

            

            who_goes = true;

            status_game = GameStatus.Menu;
            CreateTopMenu();
            CreateView();
        }
        #endregion

        private void CreateTopMenu()
        {
            menuStrip1 = new MenuStrip();
            mainToolStripMenuItem = new ToolStripMenuItem();
            powrótDoMenuToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator1 = new ToolStripSeparator();
            CloseToolStripMenuItem = new ToolStripMenuItem();
            OProgramieToolStripMenuItem = new ToolStripMenuItem();
            SoundToolStripMenuItem = new ToolStripMenuItem();
            SoundOffToolStripMenuItem = new ToolStripMenuItem();
            SoundOnToolStripMenuItem = new ToolStripMenuItem();
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] {
                mainToolStripMenuItem, SoundToolStripMenuItem, OProgramieToolStripMenuItem});
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(484, 24);
            menuStrip1.TabIndex = 0;
            menuStrip1.Text = "menuStrip1";
            // 
            // mainToolStripMenuItem
            // 
            mainToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] {
            powrótDoMenuToolStripMenuItem, toolStripSeparator1, CloseToolStripMenuItem});
            mainToolStripMenuItem.Size = new Size(46, 20);
            mainToolStripMenuItem.Text = "Main";
            // 
            // powrótDoMenuToolStripMenuItem
            // 
            powrótDoMenuToolStripMenuItem.Size = new Size(163, 22);
            powrótDoMenuToolStripMenuItem.Text = "Powrót do menu";
            powrótDoMenuToolStripMenuItem.Click += OpenMainMenu_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Size = new Size(160, 6);
            // 
            // CloseToolStripMenuItem
            // 
            CloseToolStripMenuItem.Size = new Size(163, 22);
            CloseToolStripMenuItem.Click += Close_Click;
            CloseToolStripMenuItem.Text = "Zamknij";
            // 
            // OProgramieToolStripMenuItem
            //
            OProgramieToolStripMenuItem.Size = new Size(46, 20);
            OProgramieToolStripMenuItem.Click += OProgramie_Click;
            OProgramieToolStripMenuItem.Text = "O programie";
            // 
            // SoundToolStripMenuItem
            //
            SoundToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] {
                SoundOffToolStripMenuItem, SoundOnToolStripMenuItem});
            SoundToolStripMenuItem.Size = new Size(46, 20);
            SoundToolStripMenuItem.Text = "Dźwięk";
            // 
            // SoundOffToolStripMenuItem
            //
            SoundOffToolStripMenuItem.Size = new Size(46, 20);
            SoundOffToolStripMenuItem.Text = "Tak";
            SoundOffToolStripMenuItem.Click += Music_Click;
            // 
            // SoundOnToolStripMenuItem
            //
            SoundOnToolStripMenuItem.Size = new Size(46, 20);
            SoundOnToolStripMenuItem.Text = "Nie";
            SoundOnToolStripMenuItem.Click += Music_Click;

            form.Controls.Add(menuStrip1);
        }

        private void OpenMainMenu_Click(object sender, EventArgs e)
        {
            ClearAllComponents(form);
            status_game = (byte)GameStatus.Menu;
            act1 = null; act2 = null; act3 = null;
            who_goes = true;
            number_of_moves = 0;
            mode = 0;

            CreateView();
        }

        #region Основна функція програми, яка в залежності від статусу виконує задані їй дії
        private void CreateView()
        {
            #region Створення меню
            if (status_game == GameStatus.Menu)
            {
                CreateMenu();
            }
            #endregion
            #region Підготовка і розстановка кораблів
            else if (status_game == GameStatus.Preparation)
            {
                if (mode == 1)
                {
                    if (!form.Controls.Contains(tableLayoutPanel1))
                    {
                        form.SizeChanged += new EventHandler(MainForm_SizeChanged);
                        // 
                        // tableLayoutPanel1
                        // 
                        tableLayoutPanel1 = new TableLayoutPanel();
                        tableLayoutPanel1.Dock = DockStyle.Fill;
                        tableLayoutPanel1.ColumnCount = 5;
                        tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
                        tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
                        tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
                        tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
                        tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
                        tableLayoutPanel1.RowCount = 3;
                        tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
                        tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));
                        tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
                        tableLayoutPanel1.Paint += new PaintEventHandler(tableLayoutPanel1_Paint);
                        // 
                        // pictureBox1
                        // 
                        pictureBox1 = new PictureBox();
                        pictureBox1.Name = "pic1";
                        pictureBox1.Dock = DockStyle.Fill;
                        pictureBox1.TabStop = false;
                        pictureBox1.MouseDown += new MouseEventHandler(picBox_MouseDown);
                        pictureBox1.MouseClick += new MouseEventHandler(pictureBox2_MouseClick);
                        pictureBox1.MouseMove += new MouseEventHandler(picBox_MouseMove);
                        tableLayoutPanel1.Controls.Add(pictureBox1, 1, 1);

                        // 
                        // pictureBox2
                        // 
                        pictureBox2 = new PictureBox();
                        pictureBox2.Name = "pic2";
                        pictureBox2.Dock = DockStyle.Fill;
                        pictureBox2.TabStop = false;
                        pictureBox2.MouseClick += new MouseEventHandler(pictureBox2_MouseClick);
                        tableLayoutPanel1.Controls.Add(pictureBox2, 3, 1);

                        //
                        // numUpDown
                        //
                        numUpDown = new NumericUpDown();
                        numUpDown.Maximum = 20;
                        numUpDown.Minimum = 8;
                        numUpDown.Value = 10;
                        numUpDown.ReadOnly = true;
                        numUpDown.Anchor = AnchorStyles.Left;
                        tableLayoutPanel1.Controls.Add(numUpDown, 3, 0);

                        size_board = (byte)numUpDown.Value;
                        NumberANDsize_ship_szablon = new byte[,] { { 4, 1 }, { 3, 2 }, { 2, 3 }, { 1, 4 } };

                        act1 = new Gamer(size_board, NumberANDsize_ship_szablon);
                        act2 = new Computer(size_board, NumberANDsize_ship_szablon);

                        numUpDown.ValueChanged += new EventHandler(numUpDown_ValueChanged);
                        //
                        // Label
                        //
                        label = new Label();
                        label.Text = "Wybierz rozmiar pola: ";
                        label.Dock = DockStyle.Fill;
                        label.TextAlign = ContentAlignment.MiddleRight;
                        label.BackColor = Color.Transparent;
                        label.AutoSize = true;
                        label.Font = new Font("Century Gothic", 14F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(204)));
                        tableLayoutPanel1.Controls.Add(label,1,0);

                        flowpanel = new FlowLayoutPanel();
                        flowpanel.Dock = DockStyle.Fill;
                        flowpanel.BackColor = Color.Transparent;
                        flowpanel.WrapContents = false;
                        tableLayoutPanel1.Controls.Add(flowpanel, 1, 2);
                        tableLayoutPanel1.SetColumnSpan(flowpanel, 3);

                        btn1 = new Button();
                        btn1.Text = "Wygenerować";
                        btn1.BringToFront();
                        btn1.Click += new EventHandler(Random_Click);
                        btn1.Cursor = Cursors.Hand;
                        btn1.Size = new Size(165, 45);
                        flowpanel.Controls.Add(btn1);

                        btn2 = new Button();
                        btn2.Text = "Wyczyścić";
                        btn2.BringToFront();
                        btn2.Click += new EventHandler(Clear_Click);
                        btn2.Cursor = Cursors.Hand;
                        btn2.Size = new Size(140, 45);
                        flowpanel.Controls.Add(btn2);

                        btn3 = new Button();
                        btn3.Text = "Start";
                        btn3.BringToFront();
                        btn3.Click += new EventHandler(Start_Click);
                        btn3.Visible = false;
                        btn3.Cursor = Cursors.Hand;
                        btn3.Size = new Size(100, 45);
                        flowpanel.Controls.Add(btn3);

                        btn1.Font = btn2.Font = btn3.Font = btn4.Font = new Font("Century Gothic", 14F, FontStyle.Bold, GraphicsUnit.Point, 204);

                        form.Controls.Add(tableLayoutPanel1);
                    }
                    tableLayoutPanel1.RowStyles[1].Height = pictureBox1.Width;

                    size_cell = pictureBox1.Width / (size_board + 3);

                    label.Font = numUpDown.Font = new Font("Century Gothic", tableLayoutPanel1.RowStyles[0].Height/2, FontStyle.Bold, GraphicsUnit.Point, ((byte)(204)));

                    btm1 = new Bitmap(pictureBox1.Width, pictureBox1.Height);
                    formGraphics1 = Graphics.FromImage(btm1);
                    formGraphics1.Clear(Color.White);
                    btm2 = new Bitmap(pictureBox1.Width, pictureBox1.Height);
                    formGraphics2 = Graphics.FromImage(btm2);

                    DrawBoard(formGraphics1);

                    act1.DrawAllShip(formGraphics1, size_cell);

                    if (ex_ship == null)
                    {
                        ex_ship = new Ship[NumberANDsize_ship_szablon.GetLength(0)];
                        for (int i = 0, tmp = 2; i < ex_ship.Length; i++, tmp += 2)
                            ex_ship[i] = new Ship(1, NumberANDsize_ship_szablon[i, 0], tmp);
                    }

                    CreateFleet();

                    pictureBox1.Image = btm1;
                }

                else if (mode == 2)
                {
                    if (!form.Controls.Contains(tableLayoutPanel1))
                    {
                        tableLayoutPanel1 = new TableLayoutPanel();
                        tableLayoutPanel1.Dock = DockStyle.Fill;
                        tableLayoutPanel1.ColumnCount = 3;
                        tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 35F));
                        tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
                        tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 35F));
                        tableLayoutPanel1.RowCount = 3;
                        tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 35F));
                        tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
                        tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 35F));
                        tableLayoutPanel1.Paint += new PaintEventHandler(tableLayoutPanel1_Paint);
                        form.Controls.Add(tableLayoutPanel1);

                        panel1 = new Panel();
                        panel1.Dock = DockStyle.Fill;
                        panel1.Margin = new Padding(0, 0, 0, 0);
                        tableLayoutPanel1.Controls.Add(panel1, 1, 1);

                        label = new Label();
                        label.Text = "Wybierz rozmiar pola: ";
                        label.AutoSize = true;
                        panel1.Controls.Add(label);

                        label2 = new Label();
                        label2.Text = "Imię 1 gracza: ";
                        label2.AutoSize = true;
                        panel1.Controls.Add(label2);

                        label3 = new Label();
                        label3.Text = "Imię 2 gracza: ";
                        label3.AutoSize = true;
                        panel1.Controls.Add(label3);

                        txt1 = new TextBox();
                        panel1.Controls.Add(txt1);
                        txt2 = new TextBox();
                        panel1.Controls.Add(txt2);

                        numUpDown = new NumericUpDown();
                        numUpDown.Maximum = 20;
                        numUpDown.Minimum = 8;
                        numUpDown.Value = 10;
                        numUpDown.ReadOnly = true;
                        numUpDown.AutoSize = true;
                        numUpDown.ValueChanged += new EventHandler(numUpDown_ValueChanged);
                        panel1.Controls.Add(numUpDown);

                        btn1 = new Button();
                        btn1.FlatStyle = FlatStyle.Popup;
                        btn1.Text = "Start";
                        btn1.AutoSize = true;
                        btn1.TabIndex = 0;
                        btn1.Click += new EventHandler(TwoGamersGame_Click);
                        btn1.Enter += new EventHandler(btn_MouseEnter);
                        btn1.MouseEnter += new EventHandler(btn_MouseEnter);
                        panel1.Controls.Add(btn1);

                        btn1.Font = label.Font = label2.Font = label3.Font = txt1.Font = txt2.Font = new Font("Century Gothic", 14, FontStyle.Bold, GraphicsUnit.Point, ((byte)(204)));
                        txt1.Width = txt2.Width = 200;
                        numUpDown.Font = new Font("Century Gothic", 12, FontStyle.Bold, GraphicsUnit.Point, ((byte)(204)));
                    }
                    label.Location = new Point(20, 10);
                    numUpDown.Location = new Point(label.Location.X + label.Width + 20, label.Location.Y);
                    label2.Location = new Point(20, label.Location.Y + label.Height + 25);
                    txt1.Location = new Point(label2.Location.X + label2.Width + 20, label2.Location.Y);
                    label3.Location = new Point(20, label2.Location.Y + label2.Height + 25);
                    txt2.Location = new Point(label3.Location.X + label3.Width + 20, label3.Location.Y);
                    btn1.Location = new Point(180,label3.Location.Y + label3.Height + 25);
                }

            }
            #endregion
            #region Процес гри
            else if (status_game == GameStatus.Game)
            {
                if (!form.Controls.Contains(tableLayoutPanel1))
                {
                    form.SizeChanged += new EventHandler(MainForm_SizeChanged);
                    // 
                    // tableLayoutPanel1
                    // 
                    tableLayoutPanel1 = new TableLayoutPanel();
                    tableLayoutPanel1.Dock = DockStyle.Fill;
                    tableLayoutPanel1.ColumnCount = 3;
                    tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
                    tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
                    tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
                    tableLayoutPanel1.RowCount = 2;
                    tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
                    tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 45F));
                    tableLayoutPanel1.Paint += new PaintEventHandler(tableLayoutPanel1_Paint);

                    tableLayoutPanel3 = new TableLayoutPanel();
                    tableLayoutPanel3.Dock = DockStyle.Fill;
                    tableLayoutPanel3.BackColor = Color.White;
                    tableLayoutPanel3.ColumnCount = 3;
                    tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
                    tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
                    tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
                    tableLayoutPanel3.RowCount = 2;
                    tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
                    tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Absolute, 80F));
                    tableLayoutPanel1.Controls.Add(tableLayoutPanel3, 1, 0);

                    tableLayoutPanel2 = new TableLayoutPanel();
                    tableLayoutPanel2.Dock = DockStyle.Fill;
                    tableLayoutPanel3.BackColor = Color.White;
                    tableLayoutPanel2.ColumnCount = 3;
                    tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
                    tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
                    tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
                    tableLayoutPanel1.Controls.Add(tableLayoutPanel2, 1, 1);
                    // 
                    // pictureBox1
                    // 
                    pictureBox1 = new PictureBox();
                    pictureBox1.Dock = DockStyle.Fill;
                    pictureBox1.TabStop = false;
                    if(mode == 2) pictureBox1.MouseClick += new MouseEventHandler(Gamer1_MouseClick);
                    tableLayoutPanel3.Controls.Add(pictureBox1, 0, 1);
                    // 
                    // pictureBox2
                    // 
                    pictureBox2 = new PictureBox();
                    pictureBox2.Dock = DockStyle.Fill;
                    pictureBox2.TabStop = false;
                    if (mode == 1)
                        pictureBox2.MouseClick += new MouseEventHandler(pictureBox2_MouseClick);
                    else if(mode == 2)
                        pictureBox2.MouseClick += new MouseEventHandler(Gamer2_MouseClick);
                    tableLayoutPanel3.Controls.Add(pictureBox2, 2, 1);
                    //
                    // label
                    //
                    label = new Label();
                    label.Text = "Ilość ruchów: " + number_of_moves;
                    label.Dock = DockStyle.Fill;
                    label.TextAlign = ContentAlignment.MiddleCenter;
                    label.AutoSize = true;
                    label.BackColor = Color.White;
                    label.Font = new Font("Century Gothic", 16F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(204)));
                    tableLayoutPanel2.Controls.Add(label, 2, 0);

                    //
                    // label2
                    //
                    label2 = new Label();
                    label2.Dock = DockStyle.Fill;
                    label2.TextAlign = ContentAlignment.MiddleCenter;
                    label2.AutoSize = true;
                    label2.BackColor = Color.Transparent;
                    label2.Font = new Font("Century Gothic", 16, FontStyle.Bold, GraphicsUnit.Point, ((byte)(204)));
                    label2.Text = (mode==1)?"Gracz chodzi": act1.Name+" chodzi";
                    tableLayoutPanel3.Controls.Add(label2, 2, 0);
                    //
                    // label3
                    //
                    label3 = new Label();
                    label3.Dock = DockStyle.Fill;
                    label3.TextAlign = ContentAlignment.MiddleCenter;
                    label3.AutoSize = true;
                    label3.BackColor = Color.Transparent;
                    label3.Font = new Font("Century Gothic", 16, FontStyle.Bold, GraphicsUnit.Point, ((byte)(204)));
                    label3.Text = (mode == 1) ? "Komputer chodzi" : act3.Name + " chodzi";
                    tableLayoutPanel3.Controls.Add(label3, 0, 0);

                    form.Controls.Add(tableLayoutPanel1);
                }
                tableLayoutPanel3.RowStyles[1].Height = pictureBox1.Width;
                tableLayoutPanel1.RowStyles[0].Height = tableLayoutPanel3.Height;

                size_cell = pictureBox1.Width / (size_board + 3);

                btm1 = new Bitmap(pictureBox1.Width, pictureBox1.Height);
                formGraphics1 = Graphics.FromImage(btm1);
                formGraphics1.Clear(Color.White);
                btm2 = new Bitmap(pictureBox1.Width, pictureBox1.Height);
                formGraphics2 = Graphics.FromImage(btm2);
                formGraphics2.Clear(Color.White);

                DrawBoard(formGraphics1);
                DrawBoard(formGraphics2);

                if(mode == 1)
                {
                    act1.DrawAllShip(formGraphics1, size_cell);
                    act2.DrawAllHiddenShip(formGraphics2, size_cell);

                    act1.DrawAllMiss(formGraphics2, size_cell);
                    act2.DrawAllMiss(formGraphics1, size_cell);

                    if (!who_goes)
                    {
                        label3.Visible = true;
                        label2.Visible = false;
                        ComputerShot();
                    }
                    else
                    {
                        label2.Visible = true;
                        label3.Visible = false;
                    }
                }
                else if(mode == 2)
                {
                    act1.DrawAllHiddenShip(formGraphics1, size_cell);
                    act3.DrawAllHiddenShip(formGraphics2, size_cell);

                    act1.DrawAllMiss(formGraphics2, size_cell);
                    act3.DrawAllMiss(formGraphics1, size_cell);

                    if (!who_goes)
                    {
                        label3.Visible = true;
                        label2.Visible = false;
                    }
                    else
                    {
                        label2.Visible = true;
                        label3.Visible = false;
                    }
                }

                pictureBox1.Image = btm1;
                pictureBox2.Image = btm2;
            }
            #endregion
            #region Завершення гри
            else if (status_game == GameStatus.Finish)
            {
                if (mode == 1)
                {
                    MessageBox.Show((who_goes ? "Gracz" : "Komputer") + " wygrał!",
                        "Mamy zwycięzcę! Gratulacje!", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }
                else if (mode == 2)
                {
                    MessageBox.Show((who_goes ? act1.Name : act3.Name) + " wygrał!",
                        "Mamy zwycięzcę! Gratulacje!", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }
                status_game = (byte)GameStatus.Menu;
                act1 = null; act2 = null; act3 = null;
                who_goes = true;
                number_of_moves = 0;
                mode = 0;

                if (form.Controls.Contains(tableLayoutPanel1))
                {
                    form.Controls.Remove(tableLayoutPanel1);
                    tableLayoutPanel1.Dispose();
                    form.SizeChanged -= new EventHandler(MainForm_SizeChanged);
                }
                CreateView();
            }
            #endregion
        }
        #endregion

        private void TwoGamersGame_Click(object sender, EventArgs e)
        {
            if (txt1.Text!="" || txt2.Text!="")
            {
                status_game = GameStatus.Game;
                act1.AutoGenerateShip();
                System.Threading.Thread.Sleep(200);
                act3.AutoGenerateShip();
                act1.Name = txt1.Text;
                act3.Name = txt2.Text;

                if (form.Controls.Contains(tableLayoutPanel1))
                {
                    btn1.Click -= new EventHandler(TwoGamersGame_Click);
                    btn1.MouseEnter -= new EventHandler(btn_MouseEnter);
                    btn1.Enter -= new EventHandler(btn_MouseEnter);
                    panel1.Controls.Remove(btn1);
                    panel1.Controls.Remove(label);
                    panel1.Controls.Remove(label2);
                    panel1.Controls.Remove(label3);
                    panel1.Controls.Remove(txt1);
                    panel1.Controls.Remove(txt2);
                    panel1.Controls.Remove(numUpDown);
                    btn1.Dispose(); label.Dispose(); label2.Dispose(); label3.Dispose();
                    txt1.Dispose(); txt2.Dispose(); numUpDown.Dispose();
                    tableLayoutPanel1.Controls.Remove(panel1);
                    panel1.Dispose();
                    tableLayoutPanel1.Paint -= new PaintEventHandler(tableLayoutPanel1_Paint);
                    form.Controls.Remove(tableLayoutPanel1);
                    tableLayoutPanel1.Dispose();
                }
                CreateView();
            }
            else MessageBox.Show("Wprowadź imiona graczy",
                "Nie wszystkie dane są wypełnione!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        #region Всі функції, що зв'язані зі створенням меню
        private void CreateMenu()
        {
            if (!form.Controls.Contains(tableLayoutPanel1))
            {
                player = new System.Media.SoundPlayer(Properties.Resources.Tick);
                // 
                // tableLayoutPanel1
                // 
                tableLayoutPanel1 = new TableLayoutPanel();
                tableLayoutPanel1.ColumnCount = 3;
                tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
                tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 330F));
                tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
                tableLayoutPanel1.Dock = DockStyle.Fill;
                tableLayoutPanel1.RowCount = 3;
                tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 40F));
                tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 360F));
                tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 60F));
                tableLayoutPanel1.Paint += new PaintEventHandler(tableLayoutPanel1_Paint);
                // 
                // panel1
                // 
                panel1 = new Panel();
                panel1.Dock = DockStyle.Fill;
                panel1.Margin = new Padding(0, 0, 0, 0);
                // 
                // pictureBox1
                // 
                pictureBox1 = new PictureBox();
                pictureBox1.Dock = DockStyle.Fill;
                pictureBox1.Image = Properties.Resources.Menu;
                pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                pictureBox1.TabStop = false;
                panel1.Controls.Add(pictureBox1);
                //
                // btn1
                //
                btn1 = new Button();
                btn1.Text = "Przeciwko komputerowi";
                btn1.Location = new Point(30, 90);
                btn1.TabIndex = 0;
                btn1.Click += new EventHandler(NewGame_Click);
                btn1.Enter += new EventHandler(btn_MouseEnter);
                btn1.MouseEnter += new EventHandler(btn_MouseEnter);
                panel1.Controls.Add(btn1);
                btn1.BringToFront();
                //
                // btn2
                //
                btn2 = new Button();
                btn2.Text = "Gra dla dwóch osób";
                btn2.Location = new Point(30, 140);
                btn2.TabIndex = 1;
                btn2.Click += new EventHandler(CompatibleGame_Click);
                btn2.Enter += new EventHandler(btn_MouseEnter);
                btn2.MouseEnter += new EventHandler(btn_MouseEnter);
                panel1.Controls.Add(btn2);
                btn2.BringToFront();
                //
                // btn3
                //
                btn3 = new Button();
                btn3.Text = "Zasady gry";
                btn3.Location = new Point(30, 190);
                btn3.TabIndex = 2;
                btn3.Click += new EventHandler(Zasady_Click);
                btn3.Enter += new EventHandler(btn_MouseEnter);
                btn3.MouseEnter += new EventHandler(btn_MouseEnter);
                panel1.Controls.Add(btn3);
                btn3.BringToFront();
                //
                // btn4
                //
                btn4 = new Button();
                btn4.Text = "Zamknij";
                btn4.Font = new Font("Century Gothic", 12F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(204)));
                btn4.Location = new Point(30, 290);
                btn4.TabIndex = 6;
                btn4.Click += new EventHandler(Close_Click);
                btn4.Enter += new EventHandler(btn_MouseEnter);
                btn4.MouseEnter += new EventHandler(btn_MouseEnter);
                panel1.Controls.Add(btn4);
                btn4.BringToFront();
                //
                // btn5
                //
                btn5 = new Button();
                btn5.Location = new Point(30, 240);
                btn5.TabIndex = 3;
                btn5.BackgroundImage = Properties.Resources.settings;
                btn5.Enter += new EventHandler(btn_MouseEnter);
                btn5.MouseEnter += new EventHandler(btn_MouseEnter);
                panel1.Controls.Add(btn5);
                btn5.BringToFront();
                //
                // btn6
                //
                btn6 = new Button();
                btn6.Text = "O programie";
                btn6.Location = new Point(80, 240);
                btn6.Size = new Size(170,45);
                btn6.TabIndex = 4;
                btn6.Click += new EventHandler(OProgramie_Click);
                btn6.Enter += new EventHandler(btn_MouseEnter);
                btn6.MouseEnter += new EventHandler(btn_MouseEnter);
                panel1.Controls.Add(btn6);
                btn6.BringToFront();
                //
                // btn7
                //
                btn7 = new Button();
                btn7.Location = new Point(255, 240);
                btn7.TabIndex = 5;
                btn7.BackgroundImage = Properties.Resources.volume_max;
                btn7.Click += new EventHandler(Music_Click);
                btn7.Enter += new EventHandler(btn_MouseEnter);
                btn7.MouseEnter += new EventHandler(btn_MouseEnter);
                panel1.Controls.Add(btn7);
                btn7.BringToFront();
                t = new ToolTip();
                t.SetToolTip(btn7, "Wyłączyć dźwięk");
                //
                // Спільне для всіх кнопок
                //
                btn1.Size = btn2.Size = btn3.Size = btn4.Size = new Size(270, 45);
                btn5.Size = btn7.Size = new Size(45, 45);
                btn5.BackgroundImageLayout = btn7.BackgroundImageLayout = ImageLayout.Stretch;
                btn5.UseVisualStyleBackColor = btn7.UseVisualStyleBackColor = true;
                btn1.Cursor = btn2.Cursor = btn3.Cursor = btn4.Cursor = btn5.Cursor = btn6.Cursor = btn7.Cursor = Cursors.Hand;
                btn1.FlatStyle = btn2.FlatStyle = btn3.FlatStyle = btn4.FlatStyle = btn5.FlatStyle = btn6.FlatStyle = btn7.FlatStyle = FlatStyle.Popup;
                btn1.Font = btn2.Font = btn3.Font = btn6.Font = new Font("Century Gothic", 14F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(204)));
                btn1.ForeColor = btn2.ForeColor = btn3.ForeColor = btn6.ForeColor = Color.MediumBlue;
                btn1.BackColor = btn2.BackColor = btn3.BackColor = btn4.BackColor = btn5.BackColor = btn6.BackColor = btn7.BackColor = SystemColors.Menu;

                tableLayoutPanel1.Controls.Add(panel1, 1, 1);

                form.Controls.Add(tableLayoutPanel1);
            }
            SoundClickReaction();
        }

        private void OProgramie_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Developers:\n\tJoker_KS(Viktor Kozenko)\n\tSofiiicandy(Sofiia Peretiatko)",
                        "Versja programy 1.27!", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
        }

        private void Zasady_Click(object sender, EventArgs e)
        {
            page = 0;
            if (form.Controls.Contains(tableLayoutPanel1))
            {
                btn1.Click -= new EventHandler(NewGame_Click);
                btn2.Click -= new EventHandler(CompatibleGame_Click);
                btn3.Click -= new EventHandler(Zasady_Click);
                btn4.Click -= new EventHandler(Close_Click);
                btn6.Click -= new EventHandler(OProgramie_Click);
                btn7.Click -= new EventHandler(Music_Click);
                btn1.MouseEnter -= new EventHandler(btn_MouseEnter);
                btn2.MouseEnter -= new EventHandler(btn_MouseEnter);
                btn3.MouseEnter -= new EventHandler(btn_MouseEnter);
                btn4.MouseEnter -= new EventHandler(btn_MouseEnter);
                btn5.MouseEnter -= new EventHandler(btn_MouseEnter);
                btn6.MouseEnter -= new EventHandler(btn_MouseEnter);
                btn7.MouseEnter -= new EventHandler(btn_MouseEnter);
                btn1.Enter -= new EventHandler(btn_MouseEnter);
                btn2.Enter -= new EventHandler(btn_MouseEnter);
                btn3.Enter -= new EventHandler(btn_MouseEnter);
                btn4.Enter -= new EventHandler(btn_MouseEnter);
                btn5.Enter -= new EventHandler(btn_MouseEnter);
                btn6.Enter -= new EventHandler(btn_MouseEnter);
                btn7.Enter -= new EventHandler(btn_MouseEnter);
                tableLayoutPanel1.Paint -= new PaintEventHandler(tableLayoutPanel1_Paint);

                ClearAllComponents(form);
            }

            tableLayoutPanel1 = new TableLayoutPanel();
            tableLayoutPanel1.Dock = DockStyle.Fill;

            tableLayoutPanel1.RowCount = 2;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 70F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 70F));

            tableLayoutPanel2 = new TableLayoutPanel();
            tableLayoutPanel2.Margin = new Padding(5, 5, 5, 5);
            tableLayoutPanel2.Dock = DockStyle.Fill;
            tableLayoutPanel2.ColumnCount = 3;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 32F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 32F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 32F));
            tableLayoutPanel1.Controls.Add(tableLayoutPanel2, 0, 1);
            //baton_nazad
            btn1 = new Button();
            if (page == 0) btn1.Enabled = false;
            btn1.BackgroundImage = Properties.Resources.settings;
            btn1.MaximumSize = new Size(500,40);
            btn1.Anchor = AnchorStyles.Right;
            btn1.Size = new Size(100, 100);
            btn1.AutoSize = true;
            btn1.TabIndex = 0;
            btn1.Click += new EventHandler(Nazad_Click);
            tableLayoutPanel2.Controls.Add(btn1, 0, 0);

            btn1.BringToFront();
            //
            //baton_menu
            //
            btn2 = new Button();
            btn2.BackgroundImage = Properties.Resources.settings;
            btn2.MaximumSize = new Size(500, 40);
            btn2.Anchor = AnchorStyles.None;

            btn2.Size = new Size(btn1.Width, 40);
            btn2.TabIndex = 1;
            btn2.Click += new EventHandler(Back2_Click);
            tableLayoutPanel2.Controls.Add(btn2, 1, 0);
            btn2.BringToFront();
            //
            //baton_wpered 
            //
            btn3 = new Button();
            btn3.BackgroundImage = Properties.Resources.settings;
            btn3.MaximumSize = new Size(500, 40);
            btn3.Anchor = AnchorStyles.Left;
            btn3.Size = new Size(btn1.Width, 40);
            btn3.TabIndex = 2;
            btn3.Click += new EventHandler(Vpered_Click);
            tableLayoutPanel2.Controls.Add(btn3, 2, 0);
            btn3.BringToFront();
            btn3.BackgroundImageLayout = btn1.BackgroundImageLayout = btn2.BackgroundImageLayout = ImageLayout.Stretch;

            panel1 = new Panel();
            panel1.Dock = DockStyle.Fill;

            panel1.AutoScroll = true;
            tableLayoutPanel1.Controls.Add(panel1, 0, 0);

            pictureBox1 = new PictureBox();
            pictureBox1.Dock = DockStyle.None;

            pictureBox1.Image = Properties.Resources.page1;
            pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;

            pictureBox1.TabStop = false;
            panel1.Controls.Add(pictureBox1);
            form.Controls.Add(tableLayoutPanel1);
        }

        private void Back2_Click(object sender, EventArgs e)
        {
            if (form.Controls.Contains(tableLayoutPanel1))
            {
                btn1.Click -= new EventHandler(Nazad_Click);
                btn2.Click -= new EventHandler(Back2_Click);
                btn3.Click -= new EventHandler(Vpered_Click);
                ClearAllComponents(form);
            }
            CreateView();
        }

        private void Vpered_Click(object sender, EventArgs e)
        {
            if (page != 2)
            {
                page++;
            }

            if (page == 1)
            {
                pictureBox1.Image = Properties.Resources.page2;
            }
            else if (page == 2)
            {
                pictureBox1.Image = Properties.Resources.page3;
                btn3.Enabled = false;
            }
            if (page != 0) btn1.Enabled = true;
        }

        private void Nazad_Click(object sender, EventArgs e)
        {
            if (page != 0)
            {
                page--;
            }

            if (page == 0)
            {
                pictureBox1.Image = Properties.Resources.page1;
                btn1.Enabled = false;
            }
            else  if (page == 1)
            {
                pictureBox1.Image = Properties.Resources.page2;
            }

            if(page!=2) btn3.Enabled = true;
        }

        private void Next_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void btn_MouseEnter(object sender, EventArgs e)
        {
            if (musicFlag)
                player.Play();
        }
        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {
            if (e.ClipRectangle!=new Rectangle(0,0,0,0))
            using (LinearGradientBrush brush = new LinearGradientBrush
                (e.ClipRectangle, Color.White, Color.Blue, 120F))
            {
                e.Graphics.FillRectangle(brush, e.ClipRectangle);
            }
        }
        private void Music_Click(object sender, EventArgs e)
        {
            musicFlag = !musicFlag;
            SoundClickReaction();
        }

        private void SoundClickReaction()
        {
            if (musicFlag)
            {
                btn7.BackgroundImage = Properties.Resources.volume_max;
                SoundOffToolStripMenuItem.Checked = true;
                SoundOnToolStripMenuItem.Checked = false;
                t.SetToolTip(btn7, "Wyłączyć dźwięk");
            }
            else
            {
                btn7.BackgroundImage = Properties.Resources.volume_min;
                SoundOffToolStripMenuItem.Checked = false;
                SoundOnToolStripMenuItem.Checked = true;
                t.SetToolTip(btn7, "Włączyć dźwięk");
            }
        }
        private void NewGame_Click(object sender, EventArgs e)
        {
            status_game = GameStatus.Preparation;
            size_board = 10;
            act1 = new Gamer(size_board);
            mode = 1;

            BtnClick();
        }
        private void CompatibleGame_Click(object sender, EventArgs e)
        {
            status_game = GameStatus.Preparation;
            size_board = 10;
            mode = 2;
            NumberANDsize_ship_szablon = new byte[,] { { 4, 1 }, { 3, 2 }, { 2, 3 }, { 1, 4 } };
            act1 = new Gamer(size_board, NumberANDsize_ship_szablon);
            act3 = new Gamer(size_board, NumberANDsize_ship_szablon);

            BtnClick();
        }
        private void Close_Click(object sender, EventArgs e)
        {
            form.Close();
        }
        private void BtnClick()
        {
            if (form.Controls.Contains(tableLayoutPanel1))
            {
                btn1.Click -= new EventHandler(NewGame_Click);
                btn2.Click -= new EventHandler(CompatibleGame_Click);
                btn3.Click -= new EventHandler(Zasady_Click);
                btn4.Click -= new EventHandler(Close_Click);
                btn6.Click -= new EventHandler(OProgramie_Click);
                btn7.Click -= new EventHandler(Music_Click);
                btn1.MouseEnter -= new EventHandler(btn_MouseEnter);
                btn2.MouseEnter -= new EventHandler(btn_MouseEnter);
                btn3.MouseEnter -= new EventHandler(btn_MouseEnter);
                btn4.MouseEnter -= new EventHandler(btn_MouseEnter);
                btn5.MouseEnter -= new EventHandler(btn_MouseEnter);
                btn6.MouseEnter -= new EventHandler(btn_MouseEnter);
                btn7.MouseEnter -= new EventHandler(btn_MouseEnter);
                btn1.Enter -= new EventHandler(btn_MouseEnter);
                btn2.Enter -= new EventHandler(btn_MouseEnter);
                btn3.Enter -= new EventHandler(btn_MouseEnter);
                btn4.Enter -= new EventHandler(btn_MouseEnter);
                btn5.Enter -= new EventHandler(btn_MouseEnter);
                btn6.Enter -= new EventHandler(btn_MouseEnter);
                btn7.Enter -= new EventHandler(btn_MouseEnter);
                tableLayoutPanel1.Paint -= new PaintEventHandler(tableLayoutPanel1_Paint);

                ClearAllComponents(form);
            }
            CreateView();
        }
        #endregion

        #region Процес розстановки кораблів гравцем
        private void CreateFleet()
        {
            DrawingExampleShip();

            if (AllShip())
            {
                btn3.Visible = true;
            }
            else btn3.Visible = false;
        }

        private void DrawingExampleShip()
        {
            formGraphics2.Clear(Color.White);

            int max = 0;
            foreach (var item in ex_ship)
            {
                item.DrawingShip(formGraphics2, size_cell);
                if (max < item.Size) max = item.Size;
            }

            Font f = new Font("Times New Roman", size_cell / 4 * 3);
            int coorY = size_cell * 4 - 4;
            byte[,] tmp_array = act1.GetNumberANDsize_ship();
            for (int i = 0; i < tmp_array.GetLength(0); i++, coorY += size_cell * 2)
                if (tmp_array[i, 1] == 0)
                    formGraphics2.DrawString(" - " + tmp_array[i, 1].ToString(), f, Brushes.Red, size_cell * 2 + (max + 1) * size_cell, coorY);
                else formGraphics2.DrawString(" - " + tmp_array[i, 1].ToString(), f, Brushes.Black, size_cell * 2 + (max + 1) * size_cell, coorY);

            pictureBox2.Image = btm2;
        }

        private bool AllShip()
        {
            byte[,] tmp_array = act1.GetNumberANDsize_ship();
            for (int i = 0; i < tmp_array.GetLength(0); i++)
            {
                if (tmp_array[i, 1] != 0)
                    return false;
            }
            return true;
        }

        private void Random_Click(object sender, EventArgs e)
        {
            Ship.DropActive(ex_ship);
            act1.AutoGenerateShip();
            CreateView();
        }
        private void Clear_Click(object sender, EventArgs e)
        {
            Ship.DropActive(ex_ship);
            ex_ship = null;
            act1.ClearShip();
            CreateView();
        }
        private void Start_Click(object sender, EventArgs e)
        {
            status_game = GameStatus.Game;
            if (form.Controls.Contains(tableLayoutPanel1))
            {
                pictureBox1.MouseDown -= new MouseEventHandler(picBox_MouseDown);
                btn1.Click -= new EventHandler(Random_Click);
                btn1.Click -= new EventHandler(Random_Click);
                btn2.Click -= new EventHandler(Clear_Click);
                btn3.Click -= new EventHandler(Start_Click);
                ClearAllComponents(form);
            }
            act2.AutoGenerateShip();
            CreateView();
        }


        private void picBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                int index_active = Ship.FindActiveShip(ex_ship);
                if (index_active != -1)
                {
                    type_active_ship = !type_active_ship;
                    tmpx = -1; tmpy = -1;
                }
            }
        }

        private void picBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (status_game == GameStatus.Preparation)
            {
                if (e.X > size_cell * 2 && e.Y > size_cell * 2 &&
                e.X < (size_cell * 2 + size_cell * size_board) &&
                e.Y < (size_cell * 2 + size_cell * size_board))
                {
                    int x, y;
                    if (Ship.FindActiveShip(ex_ship) != -1)
                    {
                        ConvertCoordinateToCells(e.X, e.Y, out x, out y);
                        if (x != tmpx || y != tmpy)
                        {
                            tmpx = x;
                            tmpy = y;
                            RedrawBattlefield(x, y);
                        }
                    }
                }
            }
        }

        private void RedrawBattlefield(int x, int y, bool click = false)
        {

            Ship tmpShip = FindOptimalCoor(x, y, click);
            if (tmpShip != null)
            {
                formGraphics1.Clear(Color.White);
                formGraphics2.Clear(Color.White);

                DrawBoard(formGraphics1);
                act1.DrawAllShip(formGraphics1, size_cell);
                CreateFleet();
                tmpShip.DrawingShip(formGraphics1, size_cell);

                pictureBox1.Image = btm1;
                pictureBox2.Image = btm2;
            }
        }

        //Функція для створення корабля під час розстановки
        //а також перевірки його розташування
        private Ship FindOptimalCoor(int x, int y, bool click = false)
        {
            int index_active = Ship.FindActiveShip(ex_ship);
            if (index_active != -1)
            {
                byte[,] tmp_array = act1.GetNumberANDsize_ship();
                int size_active = ex_ship[index_active].Size;
                int tmp;
                if (tmp_array[index_active, 1] > 0)
                {
                    if (type_active_ship)
                    {
                        tmp = y + size_active - 1;
                        while (tmp >= size_board)
                        {
                            tmp--;
                            y--;
                        }
                        if (!click)
                        {
                            return new Ship(y, tmp, x);
                        }
                        else
                        {
                            if (act1.Check(y, tmp, x, type_active_ship))
                            {
                                tmp_array[index_active, 1]--;
                                if (tmp_array[index_active, 1] == 0)
                                    Ship.DropActive(ex_ship);
                            }
                            return new Ship(y, tmp, x);
                        }
                    }
                    else
                    {
                        tmp = x + size_active - 1;
                        while (tmp >= size_board)
                        {
                            tmp--;
                            x--;
                        }
                        if (!click)
                        {
                            return new Ship(x, tmp, y, false);
                        }
                        else
                        {
                            if (act1.Check(x, tmp, y, type_active_ship))
                            {
                                tmp_array[index_active, 1]--;
                                if (tmp_array[index_active, 1] == 0)
                                    Ship.DropActive(ex_ship);
                            }
                            return new Ship(x, tmp, y, false);
                        }
                    }
                }
            }
            return null;
        }

        private void numUpDown_ValueChanged(object sender, EventArgs e)
        {
            size_board = (byte)numUpDown.Value;
            if (size_board == 8 || size_board == 9)
            {
                NumberANDsize_ship_szablon = new byte[,] { { 3, 1 }, { 2, 3 }, { 1, 3 } };
            }
            else if (size_board == 10 || size_board == 11)
            {
                NumberANDsize_ship_szablon = new byte[,] { { 4, 1 }, { 3, 2 }, { 2, 3 }, { 1, 4 } };
            }
            else if (size_board == 12 || size_board == 13)
            {
                NumberANDsize_ship_szablon = new byte[,] { { 4, 2 }, { 3, 3 }, { 2, 3 }, { 1, 5 } };
            }
            else if (size_board == 14 || size_board == 15)
            {
                NumberANDsize_ship_szablon = new byte[,] { { 5, 1 }, { 4, 2 }, { 3, 3 }, { 2, 4 }, { 1, 7 } };
            }
            else if (size_board == 16 || size_board == 17)
            {
                NumberANDsize_ship_szablon = new byte[,] { { 5, 2 }, { 4, 3 }, { 3, 4 }, { 2, 5 }, { 1, 8 } };
            }
            else if (size_board > 17)
            {
                NumberANDsize_ship_szablon = new byte[,] { { 5, 3 }, { 4, 4 }, { 3, 5 }, { 2, 6 }, { 1, 12 } };
            }
            if (mode == 1)
            {
                act1 = new Gamer(size_board, NumberANDsize_ship_szablon);
                act2 = new Computer(size_board, NumberANDsize_ship_szablon);
            }
            else if(mode == 2)
            {
                act1 = new Gamer(size_board, NumberANDsize_ship_szablon);
                act3 = new Gamer(size_board, NumberANDsize_ship_szablon);
            }

            Ship.DropActive(ex_ship);
            act1.ClearShip();
            ex_ship = null;
            CreateView();
        }

        #endregion

        #region Ходи гравця і комп'ютера
        private void pictureBox2_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                int x, y;
                ConvertCoordinateToCells(e.X, e.Y, out x, out y);
                if (x >= 0 && y >= 0)
                {
                    if (status_game == GameStatus.Game && who_goes)
                    {
                        int course = act2.Course(x, y);
                        if (course == 2)
                        {
                            who_goes = !who_goes;
                            if (!who_goes)
                            {
                                label3.Visible = true;
                                label2.Visible = false;
                            }
                            else
                            {
                                label2.Visible = true;
                                label3.Visible = false;
                            }
                            act1.Result_OF_Shot(x, y, course);
                            DrawMiss(formGraphics2, x, y, true);
                            pictureBox2.Image = btm2;

                            number_of_moves++;
                            label.Text = "Ilość ruchów: " + number_of_moves;

                            ComputerShot();
                        }
                        else if (course == -1 || course == -2)
                        {
                            act1.Result_OF_Shot(x, y, course);
                            CreateView();
                        }
                        if (course == -2 && act2.AllKilled())
                        {
                            number_of_moves++;
                            label.Text = "Ilość ruchów: " + number_of_moves;
                            status_game = GameStatus.Finish;
                            CreateView();
                        }
                    }
                    else if (status_game == GameStatus.Preparation)
                    {
                        byte pole;
                        if ((sender as PictureBox).Name == "pic1")
                            pole = 1;
                        else pole = 2;

                        int tmp_rez = act1.SetActiveShip(ex_ship, (byte)x, (byte)y, pole, ref type_active_ship);

                        if (pole == 1)
                        {
                            if (tmp_rez == -1)
                            {
                                int index_active = Ship.FindActiveShip(ex_ship);
                                if (index_active != -1)
                                {
                                    RedrawBattlefield(x, y, true);
                                }
                            }
                        }
                        else DrawingExampleShip();
                    }
                }
            }
        }
        private void ComputerShot()
        {
            int coorX, coorY;
            act2.ComputerLogic(out coorX, out coorY);
            int status = act1.Course(coorX, coorY);
            if (status == 2)
            {
                who_goes = !who_goes;
                if (!who_goes)
                {
                    label3.Visible = true;
                    label2.Visible = false;
                }
                else
                {
                    label2.Visible = true;
                    label3.Visible = false;
                }
                act2.Result_OF_Shot(coorX, coorY, status);
                DrawMiss(formGraphics1, coorX, coorY);
                pictureBox1.Image = btm1;
            }
            else if (status == -1 || status == -2)
            {
                if (act1.AllKilled())
                {
                    status_game = GameStatus.Finish;
                    act2.Result_OF_Shot(coorX, coorY, status, true);
                }
                else
                {
                    act2.Result_OF_Shot(coorX, coorY, status);
                }
                CreateView();
            }
        }
        #endregion

        #region Ходи двох гравців
        private void Gamer1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                int x, y;
                ConvertCoordinateToCells(e.X, e.Y, out x, out y);
                if (x >= 0 && y >= 0)
                {
                    if (status_game == GameStatus.Game && !who_goes)
                    {
                        int course = act1.Course(x, y);
                        if (course == 2)
                        {
                            who_goes = !who_goes;
                            if (!who_goes)
                            {
                                label3.Visible = true;
                                label2.Visible = false;
                            }
                            else
                            {
                                label2.Visible = true;
                                label3.Visible = false;
                            }
                            act3.Result_OF_Shot(x, y, course);
                            DrawMiss(formGraphics1, x, y, true);
                            pictureBox1.Image = btm1;
                        }
                        else if (course == -1 || course == -2)
                        {
                            act3.Result_OF_Shot(x, y, course);
                            CreateView();
                        }
                        if (course == -2 && act1.AllKilled())
                        {
                            status_game = GameStatus.Finish;
                            CreateView();
                        }
                    }
                }
            }
        }
        private void Gamer2_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                int x, y;
                ConvertCoordinateToCells(e.X, e.Y, out x, out y);
                if (x >= 0 && y >= 0)
                {
                    if (status_game == GameStatus.Game && who_goes)
                    {
                        int course = act3.Course(x, y);
                        if (course == 2)
                        {
                            who_goes = !who_goes;
                            if (!who_goes)
                            {
                                label3.Visible = true;
                                label2.Visible = false;
                            }
                            else
                            {
                                label2.Visible = true;
                                label3.Visible = false;
                            }
                            act1.Result_OF_Shot(x, y, course);
                            DrawMiss(formGraphics2, x, y, true);
                            pictureBox2.Image = btm2;
                            number_of_moves++;
                            label.Text = "Ilość ruchów: " + number_of_moves;
                        }
                        else if (course == -1 || course == -2)
                        {
                            act1.Result_OF_Shot(x, y, course);
                            CreateView();
                        }
                        if (course == -2 && act3.AllKilled())
                        {
                            number_of_moves++;
                            label.Text = "Ilość ruchów: " + number_of_moves;
                            status_game = GameStatus.Finish;
                            CreateView();
                        }
                    }
                }
            }
        }
        #endregion



        #region Малювання 1 частини поля для гри і малювання промахів
        private void DrawBoard(Graphics f)
        {
            Pen pen = new Pen(Color.Blue, 1F);
            int side_square = size_cell * size_board;

            f.FillRectangle(Brushes.Aquamarine, size_cell * 2, size_cell * 2, side_square, side_square);

            for (int i = 0, x1 = size_cell * 3, y1 = size_cell * 3;
                i < size_board - 1; i++, x1 += size_cell, y1 += size_cell)
            {
                f.DrawLine(pen, x1, size_cell * 2, x1, side_square + size_cell * 2);
                f.DrawLine(pen, size_cell * 2, y1, size_cell * 2 + side_square, y1);
            }
            pen.Width = 3.5F;
            f.DrawRectangle(pen, size_cell * 2, size_cell * 2, side_square, side_square);

            char elem = 'A';
            for (int i = 0; i < size_board; i++, elem++)
            {
                f.DrawString((i + 1).ToString(),
                    new Font("Century Gothic", size_cell / 4 * 3, FontStyle.Bold, GraphicsUnit.Point, 204),
                    Brushes.Black, (i + 1 < 10 ? size_cell : size_cell / 5 * 4), size_cell * 2 + i * size_cell);
                f.DrawString(elem.ToString(),
                    new Font("Century Gothic", size_cell / 4 * 3, FontStyle.Bold, GraphicsUnit.Point, 204),
                    Brushes.Black, size_cell * 2 + i * size_cell, size_cell - size_cell / 4);
            }
        }
        private void DrawMiss(Graphics f, int x, int y, bool music = false)
        {
            int coorX = y * size_cell + size_cell * 2 + 4;
            int coorY = x * size_cell + size_cell * 2 + 4;
            f.FillEllipse(new SolidBrush(Color.Yellow), coorX, coorY, size_cell - 8, size_cell - 8);
            if (musicFlag && music)
            {
                Random rnd = new Random();
                int tmp = rnd.Next(2);
                if (tmp == 0)
                    player = new System.Media.SoundPlayer(Properties.Resources.miss1);
                else player = new System.Media.SoundPlayer(Properties.Resources.miss2);
                player.Play();
            }
        }
        private void MainForm_SizeChanged(object sender, EventArgs e)
        {
            if(status_game==GameStatus.Game)
            {
                double form_size = pictureBox2.Width + 25 
                    + tableLayoutPanel3.RowStyles[0].Height + tableLayoutPanel1.RowStyles[1].Height;
                if (form.Size.Height < form_size)
                    form.Size = new Size(form.Width, (int)form_size);
            }
            CreateView();
        }
        #endregion

        #region Конвертування координат до клітинок
        private void ConvertCoordinateToCells(int x, int y, out int cell_x, out int cell_y)
        {
            cell_x = -1;
            cell_y = -1;
            if (y > size_cell * 2 && y < size_board * size_cell + size_cell * 2)
            {
                if (x > size_cell * 2 && x < size_board * size_cell + size_cell * 2)
                {
                    cell_y = (x - size_cell * 2) / size_cell;
                    cell_x = (y - size_cell * 2) / size_cell;
                }
            }
        }
        #endregion

        #region Функція для очистки всіх компонентів з форми
        private void ClearAllComponents(Control c)
        {
            for (int i = c.Controls.Count-1; i > 0; i--)
            {
                if (c.Controls[i] is TableLayoutPanel || c.Controls[i] is FlowLayoutPanel || c.Controls[i] is Panel)
                {
                    ClearAllComponents(c.Controls[i]);
                }
                else if (!(c.Controls[i] is MenuStrip)) 
                {
                    Control tmp = c.Controls[i];
                    c.Controls.Remove(c.Controls[i]);
                    tmp.Dispose();
                    Thread.Sleep(1000);
                }
            }
            if (!(c is Form))
            {
                c.Parent.Controls.Remove(c);
                c.Dispose();
            }
        }
        #endregion
    }
}