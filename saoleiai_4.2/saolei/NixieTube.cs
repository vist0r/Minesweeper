using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace saolei
{

    public partial class NixieTube : UserControl
    {
        int[,] num = new int[10,7] { 
            { 1, 0, 1, 1, 1, 1, 1 },
            { 0, 0, 0, 0, 1, 0, 1 },
            { 1, 1, 1, 0, 1, 1, 0 }, 
            { 1, 1, 1, 0, 1, 0, 1 }, 
            { 0, 1, 0, 1, 1, 0, 1 },
            { 1, 1, 1, 1, 0, 0, 1 },
            { 1, 1, 1, 1, 0, 1, 1 },
            { 1, 0, 0, 0, 1, 0, 1 },
            { 1, 1, 1, 1, 1, 1, 1 },
            { 1, 1, 1, 1, 1, 0, 1 }
        };
        PictureBox[] pictureBoxes=new PictureBox[7];
        public NixieTube()
        {
            InitializeComponent();
            pictureBoxes[0] = pictureBox1;
            pictureBoxes[1] = pictureBox2;
            pictureBoxes[2] = pictureBox3;
            pictureBoxes[3] = pictureBox4;
            pictureBoxes[4] = pictureBox5;
            pictureBoxes[5] = pictureBox6;
            pictureBoxes[6] = pictureBox7;
        }

        private void NixieTube_Load(object sender, EventArgs e)
        {
          
        }

        public void test()
        {
            for (int i = 0; i < 10; i++)
            {
                Set(i);
                Thread.Sleep(1000);
                Reset();
            }
        }

        public void Set(object data)
        {
            int digit = (int)data;
            for (int i = 0; i < 7; i++) 
            {
                if (num[digit, i] == 1) 
                {
                    switch (i)
                    {
                        case 0:
                            pictureBoxes[i].BackgroundImage= Properties.Resources.horizon_light;
                            break;
                        case 1:
                            pictureBoxes[i].BackgroundImage = Properties.Resources.horizon_light;
                            break;
                        case 2:
                            pictureBoxes[i].BackgroundImage = Properties.Resources.horizon_light;
                            break;
                        case 3:
                            pictureBoxes[i].BackgroundImage = Properties.Resources.vertical_light;
                            break;
                        case 4:
                            pictureBoxes[i].BackgroundImage = Properties.Resources.vertical_light;
                            break;
                        case 5:
                            pictureBoxes[i].BackgroundImage = Properties.Resources.vertical_light;
                            break;
                        case 6:
                            pictureBoxes[i].BackgroundImage = Properties.Resources.vertical_light;
                            break;
                    }
                }
            }
        }
        public void Reset()
        {
            for (int i = 0; i < 7; i++)
            {
                switch (i)
                {
                    case 0:
                        pictureBoxes[i].BackgroundImage = Properties.Resources.horizon_dark;
                        break;
                    case 1:
                        pictureBoxes[i].BackgroundImage = Properties.Resources.horizon_dark;
                        break;
                    case 2:
                        pictureBoxes[i].BackgroundImage = Properties.Resources.horizon_dark;
                        break;
                    case 3:
                        pictureBoxes[i].BackgroundImage = Properties.Resources.vertical_dark;
                        break;
                    case 4:
                        pictureBoxes[i].BackgroundImage = Properties.Resources.vertical_dark;
                        break;
                    case 5:
                        pictureBoxes[i].BackgroundImage = Properties.Resources.vertical_dark;
                        break;
                    case 6:
                        pictureBoxes[i].BackgroundImage = Properties.Resources.vertical_dark;
                        break;
                }
            }
        }
    }
}
