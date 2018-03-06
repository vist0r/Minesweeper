using System;
using System.Threading;

namespace saolei
{
    partial class LED
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.nixieTube1 = new saolei.NixieTube();
            this.nixieTube2 = new saolei.NixieTube();
            this.nixieTube3 = new saolei.NixieTube();
            this.SuspendLayout();
            // 
            // nixieTube1
            // 
            this.nixieTube1.Location = new System.Drawing.Point(56, 4);
            this.nixieTube1.Name = "nixieTube1";
            this.nixieTube1.Size = new System.Drawing.Size(22, 40);
            this.nixieTube1.TabIndex = 2;
            // 
            // nixieTube2
            // 
            this.nixieTube2.Location = new System.Drawing.Point(30, 4);
            this.nixieTube2.Name = "nixieTube2";
            this.nixieTube2.Size = new System.Drawing.Size(22, 40);
            this.nixieTube2.TabIndex = 1;
            // 
            // nixieTube3
            // 
            this.nixieTube3.Location = new System.Drawing.Point(4, 4);
            this.nixieTube3.Name = "nixieTube3";
            this.nixieTube3.Size = new System.Drawing.Size(22, 40);
            this.nixieTube3.TabIndex = 0;
            // 
            // LED
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.Controls.Add(this.nixieTube1);
            this.Controls.Add(this.nixieTube2);
            this.Controls.Add(this.nixieTube3);
            this.Name = "LED";
            this.Size = new System.Drawing.Size(82, 48);
            this.ResumeLayout(false);

        }

        #endregion

        public NixieTube nixieTube3;
        public NixieTube nixieTube2;
        public NixieTube nixieTube1;

        public Thread t;
        int count;
        public System.Timers.Timer timer;

        public void test()
        {
            for (int i = 0; i < 10; i++)
            {
                Set(i);
                Thread.Sleep(500);
                Reset();
            }
        }

        public void Set(object data)
        {
            var integer = (int)data;
            if (integer > 999)
            {
                return;
            }
            if (integer > 99)
            {
                var digit1 = integer % 10;
                var digit2 = integer / 10 % 10;
                var digit3 = integer / 100;
                nixieTube1.Set(digit1);
                nixieTube2.Set(digit2);
                nixieTube3.Set(digit3);
                return;
            }
            if (integer > 9)
            {
                var digit1 = integer % 10;
                var digit2 = integer / 10;
                nixieTube1.Set(digit1);
                nixieTube2.Set(digit2);
                nixieTube3.Set(0);
                return;
            }
            if (integer >= 0)
            {
                var digit = integer;
                nixieTube1.Set(digit);
                nixieTube2.Set(0);
                nixieTube3.Set(0);
                return;
            }
            return;
        }
        public void Reset()
        {
            nixieTube1.Reset();
            nixieTube2.Reset();
            nixieTube3.Reset();
        }

        public void Timekeeping()
        {
            count = 0;
            Set(count);
            timer = new System.Timers.Timer(1000);
            timer.Elapsed += new System.Timers.ElapsedEventHandler(Handler);
            timer.AutoReset = true;
            timer.Enabled = true;
        }
        public void Handler(object source, System.Timers.ElapsedEventArgs e)
        {
            Reset();
            count++;
            Set(count);
        }
    }
}
