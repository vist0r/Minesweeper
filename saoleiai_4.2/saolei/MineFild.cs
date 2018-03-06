using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;

namespace saolei
{
    
    public partial class MineFild : UserControl
    {
          public  class point   //////AI NEED 
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
        public bool leftDown = false;
        public bool rightDown = false;
        public bool bothDown = false;
        Thread t;
        int [,] Map = new int [1000, 1000];
        public int gameStat = 0; 
        point [] mp = new point[10000 + 100];
        int[] nx = new int[8] { 0, -1, 0, 1, -1, 1, -1, 1 };
        int[] ny = new int[8] { 1, 0, -1, 0, -1, 1, 1, -1 };
        int[] dir = new int[8] { 0, 1, 2, 3, 4, 5, 6, 7 };
        int[,] vis = new int[1000, 1000]; 
        int[,] _is_mine = new int[1000, 1000];
        double[,] _is_mine_rate = new double[1000, 1000];
        int[,] _vis = new int[1000, 1000];
        int[,] dvis= new int[1000, 1000];
        int[,] allans = new int[260, 10000];
        point[] ans = new point[10000];
        int pnum;
        int row;
        int col;
        int Cnt;
        int FirstClick;
        public int marks;
        public int Delay = 0;
        public MineFild()
        {
            InitializeComponent();
        }
     
        public void _Delet()
        {
            for (int i = this.Controls.Count - 1; i >= 0; i--) 
            {
                Control c = this.Controls[i];
                c.Dispose();
            }
        }
        private void MineFild_SizeChanged(object sender, EventArgs e)
        {
            this._LayoutPane();
        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="pane_num">雷数</param>
        /// <param name="row">正方形边长</param>
        public void Init(int pane_num, int row, int col)
        {
            this.FirstClick = 0;
            this.row = row;
            this.col = col;
            this.pnum = pane_num;
            for (int i = 0; i < 1000; i++)
                for (int j = 0; j < 1000; j++) vis[i, j] = 0;
            for (int i = 0; i < row; i++)
            { 
                for (int j = 0; j < col; j++)
                {
                    Pane pane = new Pane();
                    pane.BackgroundImage = Properties.Resources.grid;
                    pane.MouseUp += new MouseEventHandler(pane_MouseUp);
                    pane.MouseDown += new MouseEventHandler(pane_MouseDown);
                    pane._x = i;
                    pane._y = j;
                    pane._Stat = 0;
                    this.Controls.Add(pane);
                    Map[i, j] = this.Controls.Count - 1;
                }
           }
           this._LayoutPane();
        }
        private void AroundSelected(int x, int y)
        {
            for (int i = 0; i < 8; i++)
            {
                int xx = nx[i] + x;
                int yy = ny[i] + y;
                if (Canset(xx, yy))
                {
                    Pane pane = (Pane)this.Controls[Map[xx, yy]];
                    if (pane._Stat == 0) pane.BackgroundImage = Properties.Resources.Image2;
                }
            }
        }
        private void AroundReset(int x, int y)
        {
            for (int i = 0; i < 8; i++)
            {
                int xx = nx[i] + x;
                int yy = ny[i] + y;
                if (Canset(xx, yy))
                {
                    Pane pane = (Pane)this.Controls[Map[xx, yy]];
                    if (pane._Stat == 0) pane.BackgroundImage = Properties.Resources.grid;
                }
            }
        }

        void showaround_pane(point o)
        {

            Pane pane = (Pane)this.Controls[Map[o.x, o.y]];
            if (pane._Stat == 0) return;
            if (pane._Stat == 2) return;
            if (pane._Stat == 1)
            {
                int tot = 0;
                for (int i = 0; i < 8; i++)
                {
                    int xx = o.x + nx[i];
                    int yy = o.y + ny[i];
                    if (Canset(xx, yy))
                    {
                        Pane pane2 = (Pane)this.Controls[Map[xx, yy]];
                        if (pane2._Stat == 2) tot++;
                    }
                }
                if (tot != pane._Around) return;
                for (int i = 0; i < 8; i++)
                {
                    int xx = nx[i] + o.x;
                    int yy = ny[i] + o.y;
                    if (Canset(xx, yy))
                    {
                        Pane pane2 = (Pane)this.Controls[Map[xx, yy]];
                        if (pane2._Has_mine && pane2._Stat != 2)
                        {
                            gameStat = 1;
                            Form1._instance.led2.timer.Close();
                            Form1._instance.led2.t.Abort();
                            Form1._instance.mineFild1.Enabled = false;
                            this._ShowAll();
                            if (Form1.status == 30)
                            {
                                Form1.duration = System.Environment.TickCount - Form1.beginTime;
                                Form1.status = 31;
                                return;
                            }
                            if (Form1.status == 20)
                            {
                                Form1.status = 21;
                                MessageBox.Show("Defeat. You hit a bomb just now.");
                                t.Abort();
                                return;
                            }
                            MessageBox.Show("Lose.");
                            return;
                        }
                    }
                }
                for (int i = 0; i < 8; i++)
                {
                    int xx = nx[i] + o.x;
                    int yy = ny[i] + o.y;
                    if (Canset(xx, yy))
                    {
                        Pane pane2 = (Pane)this.Controls[Map[xx, yy]];
                        if (pane2._Stat == 0)
                        {
                            _Displayround(pane2);
                        }
                    }
                }
            }
        }


        private void pane_MouseDown(object sender, MouseEventArgs e)
        {
            Pane pane = (Pane)sender;
            switch (e.Button)
            {
                case MouseButtons.Left:
                    leftDown = true;
                    break;
                case MouseButtons.Right:
                    rightDown = true;
                    break;
            }
            if (leftDown&&rightDown)
            {
                bothDown = true;
                if (pane._Stat == 0) pane.BackgroundImage = Properties.Resources.grid;
                AroundSelected(pane._x, pane._y);
                return;
            }
            if (pane._Stat == 0) pane.BackgroundImage = Properties.Resources.Image2;
        }
        private void pane_MouseUp(object sender,MouseEventArgs e)
        {
            Pane pane = (Pane)sender;
            switch (e.Button)
            {
                case MouseButtons.Left:
                    leftDown = false;
                    break;
                case MouseButtons.Right:
                    rightDown = false;
                    break;
            }
            if (bothDown && (leftDown || rightDown))
            {
                point p = new point();
                p.x = pane._x;
                p.y = pane._y;
                showaround_pane(p);
                AroundReset(pane._x, pane._y);
                return;
            }
            if (bothDown && !(leftDown || rightDown)) 
            {
                bothDown = false;
                return;
            }
            if(!pane.ClientRectangle.Contains(e.Location))
            {
                if (pane._Stat == 0) pane.BackgroundImage = Properties.Resources.grid;
                return;
            }
            if (this.FirstClick == 0)
            {
                marks = 0;
                this.LayMines(pane);
                this.FirstClick = 1;
                for (int i = 0; i < row; i++)
                {
                    for (int j = 0; j < col; j++)
                    {
                        Pane pane2 = (Pane)this.Controls[Map[i, j]];
                        pane2._Around = this._Count_Round(i, j);
                    }
                }
                if (Form1.mode == 2)
                {
                    point p = new point();
                    p.x = Form1.col / 2;
                    p.y = Form1.row / 2;
                    t = new Thread(Form1._instance.mineFild2.AI_Start);
                    t.Start(p);
                }
                Form1._instance.led1.Set(pnum);
                Form1._instance.led2.t = new Thread(Form1._instance.led2.Timekeeping);
                Form1._instance.led2.t.Start();
            }
            if (e.Button == MouseButtons.Right)
            {
                if (pane._Stat == 2)
                {
                    pane.BackgroundImage = Properties.Resources.grid;
                    pane._Stat = 0;
                    marks--;
                    Form1._instance.led1.Reset();
                    Form1._instance.led1.Set(pnum - marks);
                }
                else
                {
                    if (pane._Stat == 0)
                    {
                        pane._Stat = 2;
                        pane.BackgroundImage = Properties.Resources.Image1;
                        marks++;
                        Form1._instance.led1.Reset();
                        Form1._instance.led1.Set(pnum - marks);
                    }
                }
            }
            else
            {
                this._Displayround(pane);
            }
        }
        /// <summary>
        /// 随机撒雷
        /// </summary>
        /// <param name="num">雷数</param>
        private void LayMines(Pane now)
        {
            Random ran = new Random();
            for (int i = 0; i < this.pnum; i++)
            {
                int cnt = ran.Next(0, this.Controls.Count);
                point pos = new point();
                pos.x = now._x;
                pos.y = now._y;
                if (getpos(pos) == cnt)
                {
                    i--;
                    continue;
                }
                Pane pane = (Pane)this.Controls[cnt];
                if (vis[pane._x ,pane._y] == 1)
                {
                    i--;
                    continue;
                }
                vis[pane._x, pane._y] = 1;
                pane._Has_mine = true;
            }          

        }
        /// <summary>
        /// 显示所有雷 
        /// </summary>
        public void _ShowAll()
        {
            foreach (Pane pane in this.Controls)
            {      
                    pane._Open();
            }
        }
        /// <summary>
        /// 重新布局
        /// </summary>
        private void _LayoutPane()
        {
            if (this.Controls.Count == 0) return;
            int panelen = (int)Math.Sqrt(this.Controls.Count);
            int paneWidth = 20;
            int paneHight = 20;
            this.Width = 20 * col;
            this.Height = 20 * row;

            int cnt = 0;
            int paneTop = 0;
            int paneLeft = 0;
            for (int i = 0;i < row; i++)
            {
                paneTop = i * paneHight;
                for (int j = 0; j < col; j++)
                {
                    paneLeft = j * paneWidth;
                    Pane pane = (Pane) this.Controls[cnt];
                    pane.Size = new Size(paneWidth, paneHight);
                    pane.Location = new Point(paneLeft, paneTop);
                    cnt++;
                }
            }
        }
        private void MineFild_Load(object sender, EventArgs e)
        {

        }
        public int _Count_Round(int x,int y)
        {
            int res = 0;
            for (int i = 0; i < 8; i++)
            {
                int xx = nx[i] + x;
                int yy = ny[i] + y;
                if (xx >= 0 && xx < this.row && yy >= 0 && yy < this.col)
                {
                     Pane pane = (Pane)this.Controls[(Map[xx, yy])];
                    if (pane._Has_mine) res++;
                }
            }
            return res;

        }
        public void  _Displayround(Pane now)
        {
            if (gameStat == 1)
            {
                return;
            }
            Queue<Pane> q = new Queue<Pane>();
            q.Clear();
            if (now._Has_mine)
            {
                Form1._instance.led2.timer.Close();
                Form1._instance.led2.t.Abort();
                Form1._instance.mineFild1.Enabled = false;
                this._ShowAll();
                if (Form1.status == 30)
                {
                    Form1.duration = System.Environment.TickCount - Form1.beginTime;
                    Form1.status = 31;
                    return;
                }
                if (Form1.status == 20) 
                {
                    Form1.status = 21;
                    MessageBox.Show("Defeat. You hit a bomb just now.");
                    t.Abort();
                    return;
                }
                MessageBox.Show("Lose.");
                return;
            }
            q.Enqueue(now);
            vis[now._x, now._y] = 1;
            int flag = 1;
            for (int i = 0; i < this.row; i++)
            {
                for (int j = 0; j < this.col; j++)
                {
                    if (vis[i, j] == 0)
                    {
                        flag = 0;
                    }
                }
            }
            if (flag == 1)
            {
                Form1._instance.led2.timer.Close();
                Form1._instance.led2.t.Abort();
                Form1._instance.mineFild1.Enabled = false;
                now._Open();
                if (Form1.status == 30)
                {
                    Form1.duration = System.Environment.TickCount - Form1.beginTime;
                    Form1.status = 32;
                    return;
                }
                if (Form1.status == 20) 
                {
                    t.Abort();
                }
                MessageBox.Show("Win.");
                return;
            }
            while (q.Count != 0)
            {
                Pane fr = q.Dequeue();
                if (fr._Stat==2)
                {
                    continue;
                }
                if (this._Count_Round(fr._x, fr._y) != 0)
                {
                    fr._Open();
                    continue;
                }
                fr._Open();
                for (int  i = 0; i < 8; i++)
                {
                    int xx = nx[i] + fr._x;
                    int yy = ny[i] + fr._y;
                    if (Canset(xx,yy) && vis[xx, yy] == 0)
                    {
                        Pane t = (Pane) this.Controls[Map[xx, yy]];
                        vis[xx, yy] = 1;
                        q.Enqueue(t);
                    }
                }
            }
            int unopen = 0;
            for (int i = 0; i < this.row; i++)
            {
                for (int j = 0; j < this.col; j++)
                {
                    Pane pane = (Pane)this.Controls[Map[i, j]];
                    if (pane._Stat == 0) unopen++;

                }
            }
            if (pnum == unopen)
            {
                Form1._instance.led2.timer.Close();
                Form1._instance.led2.t.Abort();
                Form1._instance.mineFild1.Enabled = false;
                gameStat = 1;
                if (Form1.status == 30)
                {
                    Form1.duration = System.Environment.TickCount - Form1.beginTime;
                    Form1.status = 32;
                    return;
                }
                if (Form1.status == 20)
                {
                    t.Abort();
                }
                MessageBox.Show("Win.");
            }
        }
        /*
         *  
         *  AI 板块 
         * 
         * 
         */
         public void AI_Start(object o)
        {
            var data = o as point;
            AI_Mode(data.x, data.y);
        }
         /// <summary>
         /// 判断当前坐标能否安放在矩阵
         /// </summary>
         /// <param name="x"></param>
         /// <param name="y"></param>
         /// <returns></returns>
        public bool Canset(int x,int y)
        {
            return x >= 0 && x < this.row && y >= 0 && y < this.col;
        }/// <summary>
        /// x,y最大值
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public double max(double x,double y)
        {
            return x < y ? y : x;
        }
        /// <summary>
        /// 统计当前点附近标记雷数
        /// </summary>
        /// <param name="now"></param>
        /// <returns></returns>
        public int _Possible_mine(Pane now)
        {
            int res = 0;
            for (int i = 0;i < 8; i++)
            {
                int xx = nx[i] + now._x;
                int yy = ny[i] + now._y;
                if (Canset(xx,yy))
                {
                    Pane pane = (Pane)this.Controls[Map[xx, yy]];
                    if ((pane._Stat == 0||pane._Stat ==2) && _is_mine[xx, yy] == 1) res++;
                }
            }
            return res;
        }
        /// <summary>
        /// 统计当前点附近未打开开雷数，标记的点算已经打开
        /// </summary>
        /// <param name="now"></param>
        /// <returns></returns>
        public int _Unopen(point now)
        {
            int res = 0;
            for (int i = 0; i < 8; i++)
            {
                int xx = nx[i] + now.x;
                int yy = ny[i] + now.y;

                if (xx >= 0 && xx < this.row && yy >= 0 && yy < this.col)
                {

                    Pane pane = (Pane)this.Controls[Map[xx, yy]];
                    if (pane._Stat == 0) res++;
                }
            }
            return res;
        }
        /// <summary>
        /// AI 初始化
        /// </summary>
        public void AI_Init()
        {
            FirstClick = 0;
            for (int i = 0; i < 1000; i++)
            {
                for (int j = 0; j < 1000; j++)
                {
                    vis[i, j] = 0;
                    _is_mine[i, j] = 0;
                }
            }
            for (int i = 0; i < 1000; i++)
            {
                for (int j = 0; j < 1000; j++)
                {
                    _vis[i, j] = 0;
                    _is_mine_rate[i, j] = -1;
                }
            }
            for (int i = 0; i < 260; i++)
            {
                for (int j = 0; j < 10000; j++)
                {
                    allans[i, j] = 0;
                }
            }

        }
        /// <summary>
        /// AI 接口
        /// </summary>
        /// <param name="x">电脑一开始点击的位置的横坐标</param>
        /// <param name="y">电脑一开始点击的位置的纵坐标</param>
        public void AI_Mode(int x, int y)
        {
            AI_Init();
            if (this.FirstClick == 0)
            {
                this.FirstClick = 1;
                Pane pane = (Pane)this.Controls[Map[x, y]];
                this.LayMines(pane);

                for (int i = 0; i < row; i++)
                {
                    for (int j = 0; j < col; j++)
                    {
                        Pane pane2 = (Pane)this.Controls[Map[i, j]];
                        pane2._Around = this._Count_Round(i, j);
                    }
                }
            }
            point t = new point();
            t.x = x; t.y = y;
            AI_think(t);
            int u;
            while (gameStat != 1)
            {
                u = _DeepThink_Init();
                int stat = AI_DeepThink(u);
                Console.WriteLine("stat = " + stat);
                int cnt = 0;
                for (int i = 0; i < this.row; i++)
                {
                    for (int j = 0; j < this.col; j++)
                    {
                        Pane pane = (Pane)this.Controls[Map[i, j]];
                        if ((pane._Stat == 1) && (pane._Around != 0))
                        {
                            point pos = new point();
                            pos.x = i;
                            pos.y = j;
                            mp[cnt++] = pos;
                        }
                    }
                }
                if (stat == 1)
                {
                    _Think(cnt);
                }
                else
                {
                    int tar = 0;
                    point pos = new point();
                    pos.x = -1;
                    int flg = 0;
                    Pane[] ran = new Pane[4];
                    ran[0] = (Pane)this.Controls[Map[0, 0]];
                    ran[1] = (Pane)this.Controls[Map[0, this.col - 1]];
                    ran[2] = (Pane)this.Controls[Map[this.row - 1, 0]];
                    ran[3] = (Pane)this.Controls[Map[this.row - 1, this.col - 1]];
                    for (int i = 0; i < 4; i++)
                    {

                        if (ran[i]._Stat == 0)
                        {
                            pos.x = ran[i]._x;
                            pos.y = ran[i]._y;
                            flg = 1;
                            break;
                        }
                    }
                    if (flg == 0)
                    {


                        for (int i = 0; i < cnt; i++)
                        {
                            for (int k = 0; k < 8; k++)
                            {
                                int xx = nx[k] + mp[i].x;
                                int yy = ny[k] + mp[i].y;
                                if (Canset(xx, yy))
                                {
                                    Pane pane = (Pane)this.Controls[Map[xx, yy]];
                                    if (pane._Stat == 0)
                                    {
                                        pos.x = xx;
                                        pos.y = yy;
                                        tar = 1;
                                        break;
                                    }
                                }
                            }
                            if (tar == 1)
                            {
                                break;
                            }
                        }
                    }
                    if (pos.x == -1)
                    {
                        for (int i = 0; i < this.row; i++)
                        {
                            for (int j = 0; j < this.col; j++)
                            {
                                Pane pane = (Pane)this.Controls[Map[i, j]];
                                if (pane._Stat == 0)
                                {
                                    pos.x = i;
                                    pos.y = j;
                                }
                            }
                        }
                    }
                    AI_think(pos);
                }
            }

        }

        /// <summary>
        /// AI 走决定点下当前点之后的影响
        /// </summary>
        /// <param name="u"></param>
        public void AI_think(point u)
        {
            if (u.x == -1) return;
            if (gameStat == 1) return;
            Pane now = (Pane)this.Controls[Map[u.x, u.y]];
            Queue<Pane> q = new Queue<Pane>();
            q.Clear();
            if (now._Has_mine)
            {
                //this._ShowAll();
                gameStat = 1;
                Form1._instance.led2.timer.Close();
                Form1._instance.led2.t.Abort();
                now._Open();
                switch (Form1.status)
                {
                    case 20:
                        MessageBox.Show("You win, AI hit a bomb just now.");
                        Form1.status = 22;
                        break;
                    case 21:
                        MessageBox.Show("Draw.");
                        Form1.status = 23;
                        break;
                    case 22:
                        MessageBox.Show("You win.");
                        break;
                }
                //MessageBox.Show("Lose");
                return;
            }
            q.Enqueue(now);
            vis[now._x, now._y] = 1;
            int flag = 1;
            for (int i = 0; i < this.row; i++)
            {
                for (int j = 0; j < this.col; j++)
                {
                    if (vis[i, j] == 0)
                    {

                        flag = 0;
                    }
                }
            }
            if (flag == 1)
            {
                gameStat = 1;
                now._Open();
                Form1._instance.led2.timer.Close();
                Form1._instance.led2.t.Abort();
                switch (Form1.status)
                {
                    case 20:
                        MessageBox.Show("Defeat.");
                        Form1.status = 23;
                        break;
                    case 21:
                        MessageBox.Show("Defeat.");
                        Form1.status = 23;
                        break;
                    case 22:
                        MessageBox.Show("You win.");
                        break;
                }
                //MessageBox.Show("win");
                return;
            }
            int cnt = 0;
            while (q.Count != 0)
            {
               
                Pane fr = q.Dequeue();
                if (fr._Around != 0)
                {
                    fr._Open();
                    point pos = new point();
                    pos.set_val(fr._x, fr._y);
                    continue;
                }
                fr._Open();
                for (int i = 0; i < 8; i++)
                {
                    int xx = nx[i] + fr._x;
                    int yy = ny[i] + fr._y;
                    if (Canset(xx, yy) && vis[xx, yy] == 0)
                    {
                        Pane t = (Pane)this.Controls[Map[xx, yy]];
                        vis[xx, yy] = 1;
                        q.Enqueue(t);
                    }
                }

            }
            for (int i = 0; i < this.row; i++)
            {
                for (int j = 0; j < this.col; j++)
                {
                    Pane pane = (Pane)this.Controls[Map[i, j]];
                    if ((pane._Stat == 1)&& (pane._Around != 0))
                    {
                        point pos = new point();
                        pos.x = i;
                        pos.y = j;
                        mp[cnt++] = pos; 
                    }
                }
            }
            flag = 1;
            for (int i = 0; i < this.row; i++)
            {
                for (int j = 0; j < this.col; j++)
                {
                    if (vis[i, j] == 0)
                    {

                        flag = 0;
                    }
                }
            }
            if (flag == 1)
            {
                gameStat = 1;
                now._Open();
                Form1._instance.led2.timer.Close();
                Form1._instance.led2.t.Abort();
                switch (Form1.status)
                {
                    case 20:
                        MessageBox.Show("Defeat.");
                        Form1.status = 23;
                        break;
                    case 21:
                        MessageBox.Show("Defeat.");
                        Form1.status = 23;
                        break;
                    case 22:
                        MessageBox.Show("You win.");
                        break;
                }
                //MessageBox.Show("win");
                return;
            }
            _Think(cnt);
        }
        /// <summary>
        /// AI浅层思考 不需要推理能确定是雷的点
        /// </summary>
        /// <param name="cnt"></param>
        public  void _Think(int cnt) { 
            for (int i = 0; i < cnt; i++)
            {
                mp[i].around = _Unopen(mp[i]);
            }
            for (int i = 0; i < cnt; i++)
            {
                for (int j = 0; j < cnt; j++)
                {
                    Pane pane1 = (Pane)this.Controls[Map[mp[i].x, mp[i].y]];
                    Pane pane2 = (Pane)this.Controls[Map[mp[j].x, mp[j].y]];
                    int ui = mp[i].around;
                    int uj = mp[j].around; 
                    if (ui == uj)
                    {
                        if (pane1._Around < pane2._Around)
                        {
                            point t = mp[i]; mp[i] = mp[j]; mp[j] = t;
                        }
                    }
                    if (ui < uj)
                    {
                        point t = mp[i]; mp[i] = mp[j]; mp[j] = t;
                    }
                }
            }
            for (int i = 0; i < this.row; i++)
            {
                for (int j = 0; j < this.col; j++)
                {
                    if (_is_mine_rate[i, j] == 0 || _is_mine_rate[i, j] == 1) continue;
                    _is_mine_rate[i, j] = -1;
                }
            }
            Queue<point> rec = new Queue<point>();
            rec.Clear();
            for (int i = 0; i < cnt; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    int xx = nx[j] + mp[i].x;
                    int yy = ny[j] + mp[i].y;
                    point pos = new point();
                    pos.x = xx;
                    pos.y = yy;
                    if (Canset(xx, yy))
                    {

                        Pane pane = (Pane)this.Controls[Map[xx, yy]];
                        Pane pane2 = (Pane)this.Controls[Map[mp[i].x, mp[i].y]];
                        if (pane._Stat == 0)
                        {
                            
                            double a = (double)((double)pane2._Around - _Possible_mine(pane2)) / ((double)_Unopen(mp[i]));
                            _is_mine_rate[xx, yy] = (a==0)?0:this.max(a, (double)(_is_mine_rate[xx, yy]));
                            if (_is_mine_rate[xx, yy] == 1)
                            {
                                _is_mine[xx, yy] = 1;

                                pane._Mark();

                            }
                            else
                            {
                                if (_is_mine_rate[xx, yy] == 0&&_vis[xx,yy]==0)
                                {
                               
                                    point tmp = new point();
                                    tmp.x = xx;
                                    tmp.y = yy;
                                    rec.Enqueue(tmp);
                                }
                            }

                        }
                        if (pane._Stat == 2)
                        {
                            _is_mine_rate[pane._x, pane._y] = 1;
                            _is_mine[xx, yy] = 1;
                        }
                    }
                }
            }
            for (int i = 0; i < this.row; i++)
            {
                for (int j = 0; j < this.col; j++)
                {

                    if (_is_mine_rate[i, j] == 0)
                    {
                        point tmp = new point();
                        tmp.x = i;
                        tmp.y = j;
                        if (_vis[i, j] == 0)
                        {
                            _vis[i, j] = 1;
                   
                        }
                    }
                   // Console.Write(Math.Round(_is_mine_rate[i, j], 3) + "  ");
                }
               // Console.WriteLine();

            }
            //Console.WriteLine();

            if (rec.Count == 0)
            {

                int flag = 1;
                for (int i = 0; i < this.row; i++)
                {
                    for (int j = 0; j < this.col; j++)
                    {
                        Pane pane = (Pane)this.Controls[Map[i, j]];
                        if (vis[i, j] == 0&&pane._Stat!=2)
                        {

                            flag = 0;
                        }
                    }
                }
                if (flag == 1)
                {
                    Form1._instance.led2.timer.Close();
                    Form1._instance.led2.t.Abort();
                    switch (Form1.status)
                    {
                        case 20:
                            MessageBox.Show("Defeat.");
                            Form1.status = 23;
                            break;
                        case 21:
                            MessageBox.Show("Defeat.");
                            Form1.status = 23;
                            break;
                        case 22:
                            MessageBox.Show("You win.");
                            break;
                    }
                    //MessageBox.Show("win");
                    gameStat = 1;
                    return;
                }
                 Console.WriteLine("Can't Solve Now!");
                return;
            }
            while (rec.Count != 0)
            {
                Thread.Sleep(Delay);
                point fr = rec.Dequeue();
                AI_think(fr);
            }
        }
        /// <summary>
        /// AI 深层思考初始化
        /// </summary>
        /// <returns></returns>
        public int _DeepThink_Init()
        {
            Cnt = 0;
            point pot = new point();
            pot.x = -1;
            pot.y = -1;

            for (int i = 0; i < this.row;i++)
            {
                for (int j = 0; j < this.col; j++)
                    dvis[i, j] = 0;
            }
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < col; j++)
                {
                    Pane pane = (Pane)this.Controls[Map[i, j]];
                    if (pane._Stat == 0)
                    {
                        if (dvis[i, j] == 0)
                        {
                            dvis[i, j] = 1;
                            
                            for (int k = 0; k < 8; k++)
                            {
                                int xx = pane._x + nx[k];
                                int yy = pane._y + ny[k];
                                if (Canset(xx,yy))
                                {
                                    Pane tmp = (Pane)this.Controls[Map[xx, yy]];
                                    if (tmp._Around != 0 && tmp._Stat == 1&&dvis[xx,yy]==0)
                                    {
                                        dvis[xx, yy] = 1;
                                        point pos = new point();
                                        pos.x = xx;
                                        pos.y = yy;
                                        ans[Cnt++] = pos;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return Cnt;
        }
        /// <summary>
        /// 将矩阵压缩为一维
        /// </summary>
        /// <param name="now"></param>
        /// <returns></returns>
        int getpos(point now)
        {
            return now.x * this.col + now.y;
        }
        int get_mine(point now)
        {
            int res = 0;
            for (int i = 0; i < 8; i++)
            {
                int xx = nx[i] + now.x;
                int yy = ny[i] + now.y;
                if (Canset(xx,yy))
                {
                    if (_is_mine[xx, yy] == 1) res++;
                }
            }
            return res;
        }
        /// <summary>
        /// 检查答案是否合法
        /// </summary>
        /// <param name="now"></param>
        /// <param name="pre"></param>
        /// <returns></returns>
        int _Checkans(point now,int pre)
        {
            Console.WriteLine("Check " + now.x + " " + now.y + " begin");
            for (int i = 0;i < 8; i++)
            {
                int xx = nx[i] + now.x;
                int yy = ny[i] + now.y;
                if (Canset(xx, yy))
                {
                    Pane pane = (Pane)this.Controls[Map[xx, yy]];
                    point pos = new point();
                    pos.x = xx;
                    pos.y = yy;
                    if (pane._Stat == 1&& pane._Around != 0)
                    {
                        if (get_mine(pos) > pane._Around)
                        {
                            Console.WriteLine(pos.x + " " + pos.y + " Not Okey Because mine_pos  > pane_Around");
                            return 0;
                        }
                        else if (get_mine(pos) < pane._Around)
                        {
                            Console.WriteLine("pos = " + "( " + pos.x + ", " + pos.y + ")");
                            Console.WriteLine("Cal = " + (pane._Around - get_mine(pos)));
                            if (AI_Find_Ans(8, pane._Around - get_mine(pos), pos, 1 << 8, pre + 1) != 1)
                            {
                               
                                Console.WriteLine(now.x + " " + now.y + "Not Okey Because Can't find This soultion");
                                Console.WriteLine("Check " + pos.x + " " + pos.y + " end");
                                return 0;
                            }
                            Console.WriteLine(pos.x + " " + pos.y + " is Okey");
                            Console.WriteLine("Check " + pos.x + " " + pos.y + " end");
                        }
                    }
                }
            }
            Console.WriteLine(now.x + " " + now.y + " is Okey");
            Console.WriteLine("Check " + now.x + " " + now.y + " end");

            return 1;
        }
        int ansnum = 0;
        /// <summary>
        /// 当前走法是否存在答案
        /// </summary>
        /// <param name="n"></param>
        /// <param name="k"></param>
        /// <param name="u"></param>
        /// <param name="M"></param>
        /// <param name="pre"></param>
        /// <returns></returns>
        int AI_Find_Ans(int n,int k,point u,int M,int pre)
        {
            if (n < k) return 0;
            int flag = 1;
            if (n == 0 || k ==0)
            {
                M = M ^ (1<<8);
                for (int i = 0; i < 8; i++)
                {
                    if ((M & (1 << i)) != 0)
                    {                     
                        int xx = u.x + nx[dir[i]];
                        int yy = u.y + ny[dir[i]];
                        if (Canset(xx, yy))
                        {
                            Pane pane = (Pane)this.Controls[Map[xx, yy]];
                            if (_is_mine[xx, yy] == 1 || pane._Stat != 0)
                            {
                                flag = 0;
                                break;
                            }

                        }else
                        {
                            flag = 0;
                            break;
                        }
                    }
                   
                }
                if (flag == 1)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        if ((M & (1 << i)) != 0)
                        {
                           
                            int xx = u.x + nx[dir[i]];
                            int yy = u.y + ny[dir[i]];
                           
                            if (Canset(xx, yy))
                            {
                                Console.Write("posible ans = " + "( " + xx + ", " + yy + ")"+ " ");
                                _is_mine[xx, yy] = 1;
                            }
                        }

                    }
                    Console.WriteLine();
                }else
                {
                    return 0;
                }
                int flag2 = 0;
                if (_Checkans(u, pre) == 1)
                {
                      flag2 = 1;
                      for (int i = 0; i < 8; i++)
                      {
                            if ((M & (1 << i)) != 0)
                            {

                                int xx = u.x + nx[dir[i]];
                                int yy = u.y + ny[dir[i]];

                                if (Canset(xx, yy))
                                {
                                    point pos = new point();
                                    pos.x = xx;
                                    pos.y = yy;
                                    Console.WriteLine(pre + " ans = " + "( " + xx + ", " + yy + ")");
                                    if (pre == 0) allans[ansnum, getpos(pos)] = 1;
                                }
                            }

                      }
                      if (pre == 0) ansnum++;

                }
                for (int i = 0; i < 8; i++)
                {
                      if ((M & (1 << i)) != 0)
                      {

                          int xx = u.x + nx[dir[i]];
                          int yy = u.y + ny[dir[i]];
                          if (Canset(xx, yy))
                          {
                             _is_mine[xx, yy] = 0;
                          }
                      }
                }
                if (pre == 0) Console.WriteLine("------------------------------------------------------");
                if (flag2 == 0) return 0;
                return 1;
             }
            M = M | (1 << (n - 1));
            int k1 = AI_Find_Ans(n - 1, k - 1,u,M,pre);
            M = M ^ (1 << (n - 1));
            int k2 = AI_Find_Ans(n - 1, k,u,M,pre);
            return k1 | k2;
            
            
        }
        /// <summary>
        /// 将一维矩阵变换为二维的点
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        point get_point(int x)
        {
            point pos = new point();
            pos.x = x / this.col;
            pos.y = x % this.col;
            return pos;
        }
        /// <summary>
        /// AI 长考，判断是否有确定是地雷的位置
        /// </summary>
        /// <param name="now"></param>
        /// <returns></returns>
        public int AI_DeepThink(int now)
        {
            if (now == 0) return 0;
            int flag = 0;
            for (int k = 0; k < now; k++)
            {
                Console.WriteLine("Start = " + ans[k].x + " " + ans[k].y);

                Pane pane = (Pane)this.Controls[Map[ans[k].x, ans[k].y]];
                Console.WriteLine(pane._Around + " " + get_mine(ans[k]));

                AI_Find_Ans(8, pane._Around - get_mine(ans[k]), ans[k], 1 << 8, 0);
                for (int i = 0; i < this.row * this.col; i++)
                {
                    int tmp = 1;
                    for (int j = 0; j < ansnum; j++)
                    {
                        tmp = tmp & allans[j, i];
                    }
                    if (tmp == 1)
                    {
                        point pt = new point();
                        pt = get_point(i);
                        _is_mine[pt.x, pt.y] = 1;
                        pane = (Pane)this.Controls[Map[pt.x, pt.y]];
                        pane._aiper();
                        flag = 1;
                    }
                }
                for (int i = 0; i < ansnum; i++)
                {
                    for (int j = 0; j < 10000; j++)
                    {
                        allans[i, j] = 0;
                    }
                }
                ansnum = 0;
            }
            return flag;

        }

        /*
         * 
         * 
         *  AI 板块结束
         * 
         * 
         */
    }
}
