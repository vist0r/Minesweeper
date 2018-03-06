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
    public partial class LED : UserControl
    {
        public LED()
        {
            InitializeComponent();
            t = new Thread(Timekeeping);
            timer = new System.Timers.Timer(10000);
        }
    }
}
