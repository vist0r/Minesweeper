using System.Windows.Forms;

namespace saolei
{
    public class Pane : Label
    {
        public Pane()
        {
            this.BackgroundImageLayout = ImageLayout.Stretch;
        }
        /// <summary>
        /// 当前方格是否有地雷
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int _x { get; set; }
        public int _y { get; set; }
        public bool _Has_mine { get; set; }
        public int _Around { get; set; }
        public int _Stat { get;set; }
        public void _Open()
        {
            this._Stat = 1;
            if (this._Has_mine)
            {
                this.BackgroundImage = Properties.Resources.bang;
                this.Enabled = false;
            }
            else
            {
               // Console.WriteLine(this._Around + "\n");
                switch (this._Around) {
                    case 0:
                        this.BackgroundImage = Properties.Resources.Image2;
                        //this.Enabled = false;
                        break;
                    case 1:
                        
                        this.BackgroundImage = Properties.Resources.Image3;
                        //this.Enabled = false;
                        break;
                    case 2:
                        
                        this.BackgroundImage = Properties.Resources.Image4;
                       // this.Enabled = false;
                        break;
                    case 3:
                        
                        this.BackgroundImage = Properties.Resources.Image5;
                        //this.Enabled = false;
                        break;
                    case 4:
                        
                        this.BackgroundImage = Properties.Resources.Image6;
                        //this.Enabled = false;
                        break;
                    case 5:
                        
                        this.BackgroundImage = Properties.Resources.Image7;
                       // this.Enabled = false;
                        break;
                    case 6:
                        
                        this.BackgroundImage = Properties.Resources.Image8;
                       // this.Enabled = false;
                        break;
                    case 7:
                        
                        this.BackgroundImage = Properties.Resources.Image9;
                        //this.Enabled = false;
                        break;
                    case 8:
                        
                        this.BackgroundImage = Properties.Resources.Image10;
                        //this.Enabled = false;
                        break;

                }

                
            }
            //throw new NotImplementedException();
        }
        public void _Mark()
        {
            // throw new NotImplementedException();
            this.BackgroundImage = Properties.Resources.Image1;
            this._Stat = 2;
        }
        public void _Reset()
        {
            //throw new NotImplementedException();
            this.BackgroundImage = Properties.Resources.grid;
            this._Stat = 0;
        }
        public void _aiper()
        {
            this.BackgroundImage = Properties.Resources.Image1;
            this._Stat = 2;
        }
    }
}
