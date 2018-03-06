using System;
using System.Drawing;
using System.Text;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Net.Sockets;

namespace saolei
{
    public partial class Form1 : System.Windows.Forms.Form
    {
        public static int status = 0;
        //0:结束运行
        //1:单机运行
        //20:AI对战 21:踩雷输 22:赢 23:输
        //30:PVP 31:踩雷输 32:赢 33:输

        public static int beginTime;
        public static int duration;
        public static Form1 _instance;
        public static int mode = 0;
        public static int row = 15;
        public static int col = 20;
        public static int bomb = 30;
        public Form1()
        {
            _instance = this;
            InitializeComponent();
        }
        public void resize(int mode, int row, int col)
        {
            button1.Visible = false;
            button2.Visible = false;
            if (Form1.mode == mode && mode != 0)   
            {
                return;
            }
            Form1.mode = mode;
            if (mode == 0)
            {
                Form1.mode = 1;
            }
            const int unitHeight = 25;
            const int unitWidth = 25;
            int width, height;
            switch (mode)
            {
                case 0: //自定义
                    this.mineFild1.leftDown = false;
                    this.mineFild1.rightDown = false;
                    this.mineFild1.bothDown = false;
                    if (Form1._instance.mineFild1.gameStat == 0)
                    {
                        Form1._instance.led2.timer.Close();
                        Form1._instance.led2.t.Abort();
                    }
                    led1.Reset();
                    led2.Reset();
                    mode = 1;
                    width = 8 + unitWidth + col * 20 + unitWidth + 8;
                    height = 31 + unitHeight + unitHeight * 3 + unitHeight + row * 20 + unitHeight + 8;
                    Form1._instance.Size = new Size(width, height);
                    Form1._instance.mineFild1.Size = new Size(row * 20, col * 20);
                    Form1._instance.mineFild1.Location = new Point(unitWidth, unitHeight * 5);
                    Form1._instance.led2.Location = new Point(25 + col * 20 - 82, 50);
                    this.mineFild1._Delet();
                    this.mineFild1.gameStat = 0;
                    this.mineFild1.Init(bomb, row, col);
                    this.mineFild1.Enabled = true;
                    break;
                case 1: //单人
                    width = 8 + unitWidth + col * 20 + unitWidth + 8;
                    height = 31 + unitHeight + unitHeight*3 + unitHeight + row * 20 + unitHeight + 8;
                    Form1._instance.Size = new Size(width, height);
                    Form1._instance.mineFild1.Size = new Size(row * 20, col * 20);
                    Form1._instance.mineFild1.Location = new Point(unitWidth, unitHeight * 5);
                    Form1._instance.led2.Location = new Point(25 + col * 20 - 82, 50);
                    break;
                case 2: //AI
                    width = 8 + unitWidth + col * 20 + unitWidth + col * 20 +unitWidth + 8;
                    height = 31 + unitHeight + unitHeight*3 + unitHeight + row * 20 + unitHeight + 8;
                    Form1._instance.Size = new Size(width, height);
                    Form1._instance.mineFild1.Size = new Size(row * 20, col * 20);
                    Form1._instance.mineFild1.Location = new Point(unitWidth, unitHeight * 5);
                    Form1._instance.mineFild2.Size = new Size(row * 20, col * 20);
                    Form1._instance.mineFild2.Location = new Point(unitWidth * 2 + col * 20, unitHeight * 5);
                    Form1._instance.led2.Location = new Point(25 + col * 20 - 82, 50);
                    break;
                case 3: //联机
                    width = 8 + unitWidth + col * 20 + unitWidth + unitWidth + col * 10 + unitWidth + 8;
                    height = 31 + unitHeight + unitHeight*3 + unitHeight + row * 20 + unitHeight + 8;
                    Form1._instance.Size = new Size(width, height);
                    Form1._instance.mineFild1.Size = new Size(col * 20, row * 20);
                    Form1._instance.mineFild1.Location = new Point(unitWidth, unitHeight * 5);
                    Form1._instance.pictureBox1.Size = new Size(col * 10, row * 10);
                    Form1._instance.pictureBox1.Location = new Point(unitWidth + col * 20 + unitWidth + unitWidth, unitHeight * 5);
                    Form1._instance.led2.Location = new Point(25 + col * 20 - 82, 50);
                    break;
                default: //无效
                    return;
            }
        }
        public class point
        {
            public int x;
            public int y;
            public int around;
            public int rate;
            public void set_val(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void toolStripContainer1_ContentPanel_Load(object sender, EventArgs e)
        {

        }

        private void InitializeComponent()
        {
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.gameGToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.normalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aIToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.EasyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.UsualToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.HardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.CrazyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hellToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.netToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.button1 = new System.Windows.Forms.Button();
            this.bToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.button2 = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.customToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.led2 = new saolei.LED();
            this.led1 = new saolei.LED();
            this.mineFild2 = new saolei.MineFild();
            this.mineFild1 = new saolei.MineFild();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.gameGToolStripMenuItem,
            this.normalToolStripMenuItem,
            this.aIToolStripMenuItem,
            this.netToolStripMenuItem,
            this.customToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1058, 25);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // gameGToolStripMenuItem
            // 
            this.gameGToolStripMenuItem.Name = "gameGToolStripMenuItem";
            this.gameGToolStripMenuItem.Size = new System.Drawing.Size(84, 21);
            this.gameGToolStripMenuItem.Text = "New Game";
            this.gameGToolStripMenuItem.Click += new System.EventHandler(this.gameGToolStripMenuItem_Click);
            // 
            // normalToolStripMenuItem
            // 
            this.normalToolStripMenuItem.Name = "normalToolStripMenuItem";
            this.normalToolStripMenuItem.Size = new System.Drawing.Size(64, 21);
            this.normalToolStripMenuItem.Text = "Normal";
            this.normalToolStripMenuItem.Click += new System.EventHandler(this.normalToolStripMenuItem_Click);
            // 
            // aIToolStripMenuItem
            // 
            this.aIToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.EasyToolStripMenuItem,
            this.UsualToolStripMenuItem,
            this.HardToolStripMenuItem,
            this.CrazyToolStripMenuItem,
            this.hellToolStripMenuItem});
            this.aIToolStripMenuItem.Name = "aIToolStripMenuItem";
            this.aIToolStripMenuItem.Size = new System.Drawing.Size(32, 21);
            this.aIToolStripMenuItem.Text = "AI";
            // 
            // EasyToolStripMenuItem
            // 
            this.EasyToolStripMenuItem.Name = "EasyToolStripMenuItem";
            this.EasyToolStripMenuItem.Size = new System.Drawing.Size(108, 22);
            this.EasyToolStripMenuItem.Text = "Easy";
            this.EasyToolStripMenuItem.Click += new System.EventHandler(this.EasyToolStripMenuItem_Click);
            // 
            // UsualToolStripMenuItem
            // 
            this.UsualToolStripMenuItem.Name = "UsualToolStripMenuItem";
            this.UsualToolStripMenuItem.Size = new System.Drawing.Size(108, 22);
            this.UsualToolStripMenuItem.Text = "Usual";
            this.UsualToolStripMenuItem.Click += new System.EventHandler(this.UsualToolStripMenuItem_Click);
            // 
            // HardToolStripMenuItem
            // 
            this.HardToolStripMenuItem.Name = "HardToolStripMenuItem";
            this.HardToolStripMenuItem.Size = new System.Drawing.Size(108, 22);
            this.HardToolStripMenuItem.Text = "Hard";
            this.HardToolStripMenuItem.Click += new System.EventHandler(this.HardToolStripMenuItem_Click);
            // 
            // CrazyToolStripMenuItem
            // 
            this.CrazyToolStripMenuItem.Name = "CrazyToolStripMenuItem";
            this.CrazyToolStripMenuItem.Size = new System.Drawing.Size(108, 22);
            this.CrazyToolStripMenuItem.Text = "Crazy";
            this.CrazyToolStripMenuItem.Click += new System.EventHandler(this.CrazyToolStripMenuItem_Click);
            // 
            // hellToolStripMenuItem
            // 
            this.hellToolStripMenuItem.Name = "hellToolStripMenuItem";
            this.hellToolStripMenuItem.Size = new System.Drawing.Size(108, 22);
            this.hellToolStripMenuItem.Text = "Hell";
            this.hellToolStripMenuItem.Click += new System.EventHandler(this.hellToolStripMenuItem_Click);
            // 
            // netToolStripMenuItem
            // 
            this.netToolStripMenuItem.Name = "netToolStripMenuItem";
            this.netToolStripMenuItem.Size = new System.Drawing.Size(41, 21);
            this.netToolStripMenuItem.Text = "Net";
            this.netToolStripMenuItem.Click += new System.EventHandler(this.netToolStripMenuItem_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(25, 28);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // bToolStripMenuItem
            // 
            this.bToolStripMenuItem.Name = "bToolStripMenuItem";
            this.bToolStripMenuItem.Size = new System.Drawing.Size(32, 19);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(106, 28);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 3;
            this.button2.Text = "button2";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pictureBox1.Location = new System.Drawing.Point(1039, 368);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(10, 10);
            this.pictureBox1.TabIndex = 4;
            this.pictureBox1.TabStop = false;
            // 
            // customToolStripMenuItem
            // 
            this.customToolStripMenuItem.Name = "customToolStripMenuItem";
            this.customToolStripMenuItem.Size = new System.Drawing.Size(64, 21);
            this.customToolStripMenuItem.Text = "Custom";
            this.customToolStripMenuItem.Click += new System.EventHandler(this.customToolStripMenuItem_Click);
            // 
            // led2
            // 
            this.led2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.led2.Location = new System.Drawing.Point(243, 52);
            this.led2.Name = "led2";
            this.led2.Size = new System.Drawing.Size(82, 48);
            this.led2.TabIndex = 7;
            // 
            // led1
            // 
            this.led1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.led1.Location = new System.Drawing.Point(25, 50);
            this.led1.Name = "led1";
            this.led1.Size = new System.Drawing.Size(82, 48);
            this.led1.TabIndex = 6;
            // 
            // mineFild2
            // 
            this.mineFild2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mineFild2.Location = new System.Drawing.Point(1039, 355);
            this.mineFild2.Margin = new System.Windows.Forms.Padding(0);
            this.mineFild2.Name = "mineFild2";
            this.mineFild2.Size = new System.Drawing.Size(19, 39);
            this.mineFild2.TabIndex = 5;
            // 
            // mineFild1
            // 
            this.mineFild1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mineFild1.Location = new System.Drawing.Point(51, 73);
            this.mineFild1.Margin = new System.Windows.Forms.Padding(0);
            this.mineFild1.Name = "mineFild1";
            this.mineFild1.Size = new System.Drawing.Size(421, 352);
            this.mineFild1.TabIndex = 1;
            this.mineFild1.Load += new System.EventHandler(this.mineFild1_Load);
            // 
            // Form1
            // 
            this.ClientSize = new System.Drawing.Size(1058, 486);
            this.Controls.Add(this.led2);
            this.Controls.Add(this.led1);
            this.Controls.Add(this.mineFild2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.mineFild1);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load_1);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void Form1_Load_1(object sender, EventArgs e)
        {
            resize(1, row, col);
            this.mineFild1.Init(bomb, row, col);
            this.mineFild1.Enabled = true;
        }

        private void mineFild1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            // this.mineFild1._ShowAll();
            // this.nixieTube1.test();
            Thread t = new Thread(led1.test);
            t.Start();
            t = new Thread(led2.test);
            t.Start();
        }

        private void newGameToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }


        private void button2_Click(object sender, EventArgs e)
        {
            this.mineFild1.AI_Mode(row / 2, col / 2);

        }

        private void netToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.mineFild1.leftDown = false;
            this.mineFild1.rightDown = false;
            this.mineFild1.bothDown = false;
            if (Form1._instance.mineFild1.gameStat == 0)
            {
                Form1._instance.led2.timer.Close();
                Form1._instance.led2.t.Abort();
            }
            led1.Reset();
            led2.Reset();
            if (status == 30 || status == 2) 
            {
                return;
            }
            resize(3, row, col);
            this.mineFild1._Delet();
            this.mineFild1.Init(bomb, row, col);
            this.mineFild1.Enabled = true;
            status = 30;

            IntPtr handle = Process.GetCurrentProcess().MainWindowHandle;
            Socket socketClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress IP = IPAddress.Parse("127.0.0.1");
            IPEndPoint IPE = new IPEndPoint(IP, 8080);
            socketClient.Connect(IPE);
            try
            {
                var localEndPoint = socketClient.LocalEndPoint;
                var buffer = Encoding.UTF8.GetBytes("Battle request.\n");
                var count = socketClient.Send(buffer);
                buffer = new byte[1024];
                count = socketClient.Receive(buffer);
                var message = Encoding.UTF8.GetString(buffer, 0, count);
                socketClient.Close();
                Console.WriteLine(message);
                var IPEndPoints = message.Split('|');
                var serverIP = IPEndPoints[0].Split(':')[0];
                var serverPort = IPEndPoints[0].Split(':')[1];
                var clientIP = IPEndPoints[1].Split(':')[0];
                var clientPort = IPEndPoints[1].Split(':')[1];
                beginTime = System.Environment.TickCount;
                if (localEndPoint.ToString() == IPEndPoints[0])
                {
                    Net.RoomInfo ri = new Net.RoomInfo(handle, serverIP, serverPort);
                    Thread t = new Thread(Net.SetUpRoom);
                    t.Start(ri);
                }
                else
                {
                    Net.RoomInfo ri = new Net.RoomInfo(handle, serverIP, serverPort);
                    Thread t = new Thread(Net.EnterRoom);
                    t.Start(ri);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return;
            }
        }

        private void aiMode(object sender, EventArgs e)
        {
            status = 20;
            resize(2, row, col);
            this.mineFild1._Delet();
            this.mineFild1.gameStat = 0;
            this.mineFild1.Init(bomb, row, col);
            this.mineFild2._Delet();
            this.mineFild2.gameStat = 0;
            this.mineFild2.Init(bomb, row, col);
            this.mineFild2.Enabled = false;
            this.mineFild1.Enabled = true;
        }

        private void normalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.mineFild1.leftDown = false;
            this.mineFild1.rightDown = false;
            this.mineFild1.bothDown = false;
            if (Form1._instance.mineFild1.gameStat == 0)
            {
                Form1._instance.led2.timer.Close();
                Form1._instance.led2.t.Abort();
            }
            led1.Reset();
            led2.Reset();
            if (mode == 1) return;
            resize(1, row, col);
            this.mineFild1._Delet();
            this.mineFild1.gameStat = 0;
            this.mineFild1.Init(bomb, row, col);
            this.mineFild1.Enabled = true;
            status = 1;
        }

        private void EasyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.mineFild1.leftDown = false;
            this.mineFild1.rightDown = false;
            this.mineFild1.bothDown = false;
            if (Form1._instance.mineFild1.gameStat == 0)
            {
                Form1._instance.led2.timer.Close();
                Form1._instance.led2.t.Abort();
            }
            led1.Reset();
            led2.Reset();
            this.mineFild2.Delay = 500;
            if (mode == 2) return;
            aiMode(sender, e);
        }

        private void UsualToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.mineFild1.leftDown = false;
            this.mineFild1.rightDown = false;
            this.mineFild1.bothDown = false;
            if (Form1._instance.mineFild1.gameStat == 0)
            {
                Form1._instance.led2.timer.Close();
                Form1._instance.led2.t.Abort();
            }
            led1.Reset();
            led2.Reset();
            this.mineFild2.Delay = 300;
            if (mode == 2) return;
            aiMode(sender, e);
        }

        private void HardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.mineFild1.leftDown = false;
            this.mineFild1.rightDown = false;
            this.mineFild1.bothDown = false;
            if (Form1._instance.mineFild1.gameStat == 0)
            {
                Form1._instance.led2.timer.Close();
                Form1._instance.led2.t.Abort();
            }
            led1.Reset();
            led2.Reset();
            this.mineFild2.Delay = 100;
            if (mode == 2) return;
            aiMode(sender, e);
        }

        private void CrazyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.mineFild1.leftDown = false;
            this.mineFild1.rightDown = false;
            this.mineFild1.bothDown = false;
            if (Form1._instance.mineFild1.gameStat == 0)
            {
                Form1._instance.led2.timer.Close();
                Form1._instance.led2.t.Abort();
            }
            led1.Reset();
            led2.Reset();
            this.mineFild2.Delay = 50;
            if (mode == 2) return;
            aiMode(sender, e);
        }

        private void hellToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.mineFild1.leftDown = false;
            this.mineFild1.rightDown = false;
            this.mineFild1.bothDown = false;
            if (Form1._instance.mineFild1.gameStat == 0)
            {
                Form1._instance.led2.timer.Close();
                Form1._instance.led2.t.Abort();
            }
            led1.Reset();
            led2.Reset();
            this.mineFild2.Delay = 0;
            if (mode == 2) return;
            aiMode(sender, e);
        }

        private void nixieTube1_Load(object sender, EventArgs e)
        {

        }

        private void gameGToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.mineFild1.leftDown = false;
            this.mineFild1.rightDown = false;
            this.mineFild1.bothDown = false;
            if (Form1._instance.mineFild1.gameStat == 0)
            {
                Form1._instance.led2.timer.Close();
                Form1._instance.led2.t.Abort();
            }
            led1.Reset();
            led2.Reset();
            if (mode == 3) return;
            this.mineFild1._Delet();
            this.mineFild1.gameStat = 0;
            this.mineFild1.Init(bomb, row, col);
            this.mineFild1.Enabled = true;
            if (mode == 2)
            {
                status = 20;
                this.mineFild2._Delet();
                this.mineFild2.gameStat = 0;
                this.mineFild2.Init(bomb, row, col);
            }
        }

        private void customToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Custom c = new Custom();
            c.Show();
        }
    }
}
