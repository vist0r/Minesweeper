using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace saolei
{
    public partial class Custom : Form
    {
        public Custom()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var text1 = textBox1.Text;
            var text2 = textBox2.Text;
            var text3 = textBox3.Text;
            int row, col, bomb;
            bool isInt1 = int.TryParse(text1, out row);
            bool isInt2 = int.TryParse(text2, out col);
            bool isInt3 = int.TryParse(text3, out bomb);
            if (!isInt1)
            {
                MessageBox.Show("行数输入错误。");
                return;
            }
            if (!isInt2)
            {
                MessageBox.Show("列数输入错误。");
                return;
            }
            if (!isInt3)
            {
                MessageBox.Show("地雷数输入错误。");
                return;
            }
            if (row < 10 || row > 30)
            {
                MessageBox.Show("行数不在规定范围内。");
                return;
            }
            if (col < 10 || col > 30)
            {
                MessageBox.Show("列数不在规定范围内。");
                return;
            }
            if (bomb < 10 || bomb > row * col)
            {
                MessageBox.Show("地雷数不在规定范围内。");
                return;
            }
            Form1.row = row;
            Form1.col = col;
            Form1.bomb = bomb;
            Form1._instance.resize(0, row, col);
            this.Close();
        }
    }
}
