using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Net;
using System.Threading;
using System.Net.Sockets;
using System.IO;

namespace saolei
{
    public class Net
    {
        public static Socket socket;
        public static int TransferDelay = 2000;
        public static double compressRate = 0.5;
        public class RoomInfo
        {
            public IntPtr HWND;
            public String IP;
            public String Port;
            public RoomInfo(IntPtr a, String b, String c)
            {
                HWND = a;
                IP = b;
                Port = c;
            }
        }
        public static byte[] getEightBitsByteArray(int num)
        {
            String res = "";
            var numStr = num.ToString();
            switch (numStr.Length)
            {
                case 3:
                    res = "00000" + numStr;
                    break;
                case 4:
                    res = "0000" + numStr;
                    break;
                case 5:
                    res = "000" + numStr;
                    break;
                case 6:
                    res = "00" + numStr;
                    break;
                case 7:
                    res = "0" + numStr;
                    break;
                case 8:
                    res = numStr;
                    break;
            }
            return Encoding.UTF8.GetBytes(res);
        }
        public static void SetUpRoom(object data)
        {
            var ri = data as RoomInfo;
            Console.WriteLine("Host.");
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress IP = IPAddress.Parse(ri.IP);
            IPEndPoint IPE = new IPEndPoint(IP, Int32.Parse(ri.Port));
            socket.Bind(IPE);
            socket.Listen(1);
            while (true)
            {
                Socket client = socket.Accept();
                var pic = GetWindowCapture(ri.HWND);
                pic = CutPic(pic, 
                    Form1._instance.mineFild1.Location.X+8,
                    Form1._instance.mineFild1.Location.Y+31,
                    Form1._instance.mineFild1.Width, 
                    Form1._instance.mineFild1.Height
                );
                pic = CompressPic(pic, compressRate);
                var byteList = new List<byte>();
                var bytePic = BitmapToBytes(pic);
                Console.WriteLine(bytePic.Length);
                switch (Form1.status)
                {
                    case 30:
                        byteList.Add(0);
                        byteList.AddRange(getEightBitsByteArray(bytePic.Length));
                        byteList.AddRange(bytePic);
                        break;
                    case 31:
                        byteList.Add(1);
                        byteList.AddRange(BitConverter.GetBytes(Form1.duration));
                        break;
                    case 32:
                        byteList.Add(2);
                        byteList.AddRange(BitConverter.GetBytes(Form1.duration));
                        break;
                }
                if (Form1.status==31|| Form1.status == 32)
                {
                    foreach(var i in byteList)
                    {
                        Console.Write(i + "  ");
                    }
                }
                var receiver = new byte[1024 * 1024];
                Int64 duration;
                client.Receive(receiver);
                client.Send(byteList.ToArray());
                switch (receiver[0])
                {
                    case 0:
                        if (Form1.status == 31)
                        {
                            Form1._instance.led2.timer.Close();
                            Form1._instance.led2.t.Abort();
                            Form1._instance.mineFild1.Enabled = false;
                            Form1._instance.pictureBox1.BackgroundImage = null;
                            MessageBox.Show("Defeat.");
                            socket.Close();
                            return;
                        }
                        if (Form1.status == 32)
                        {
                            Form1._instance.led2.timer.Close();
                            Form1._instance.led2.t.Abort();
                            Form1._instance.mineFild1.Enabled = false;
                            Form1._instance.pictureBox1.BackgroundImage = null;
                            MessageBox.Show("Win.");
                            socket.Close();
                            return;
                        }
                        var buffer = new byte[8];
                        Array.Copy(receiver, 1, buffer, 0, 8);
                        var len = int.Parse(Encoding.UTF8.GetString(buffer));
                        buffer = new byte[len];
                        Array.Copy(receiver, 9, buffer, 0, len);
                        pic = BytesToBitmap(buffer);
                        Form1._instance.pictureBox1.BackgroundImage = pic as Image;
                        break;
                    case 1:
                        Form1._instance.pictureBox1.BackgroundImage = null;
                        duration = BitConverter.ToInt64(receiver, 1);
                        if (Form1.status == 31)
                        {
                            if (Form1.duration == duration)
                            {
                                MessageBox.Show("Draw.");
                            }
                            if (Form1.duration < duration)
                            {
                                MessageBox.Show("Defeat. You: " + Form1.duration + "ms; Opponent: " + duration + "ms.");
                            }
                            if (Form1.duration > duration)
                            {
                                MessageBox.Show("Win. You: " + Form1.duration + "ms; Opponent: " + duration + "ms.");
                            }
                        }
                        else
                        {
                            if (Form1.status == 30)
                            {
                                Form1._instance.led2.timer.Close();
                                Form1._instance.led2.t.Abort();
                                Form1._instance.mineFild1.Enabled = false;
                            }
                            MessageBox.Show("You win, the opponent has hit the bomb.");
                            Form1.status = 32;
                        }
                        socket.Close();
                        return;
                    case 2:
                        Form1._instance.pictureBox1.BackgroundImage = null;
                        duration = BitConverter.ToInt64(receiver, 1);
                        if (Form1.status == 32) 
                        {
                            if (Form1.duration == duration)
                            {
                                MessageBox.Show("Draw.");
                            }
                            if (Form1.duration > duration)
                            {
                                MessageBox.Show("Defeat. You: " + Form1.duration + "ms; Opponent: " + duration + "ms.");
                            }
                            if (Form1.duration < duration)
                            {
                                MessageBox.Show("Win. You: " + Form1.duration + "ms; Opponent: " + duration + "ms.");
                            }
                        }
                        if (Form1.status == 30 || Form1.status == 31)
                        {
                            if (Form1.status == 30)
                            {
                                Form1._instance.led2.timer.Close();
                                Form1._instance.led2.t.Abort();
                                Form1._instance.mineFild1.Enabled = false;
                            }
                            MessageBox.Show("Defeat.");
                            Form1.status = 0;
                        }
                        socket.Close();
                        return;
                }
            }
        }
        public static void EnterRoom(object data)
        {
            Thread.Sleep(500);
            var ri = data as RoomInfo;
            Console.WriteLine("Guest.");
            while (true)
            {
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPAddress IP = IPAddress.Parse(ri.IP);
                IPEndPoint IPE = new IPEndPoint(IP, Int32.Parse(ri.Port));
                socket.Connect(IPE);
                var pic = GetWindowCapture(ri.HWND);
                pic = CutPic(pic,
                    Form1._instance.mineFild1.Location.X+8,
                    Form1._instance.mineFild1.Location.Y+31,
                    Form1._instance.mineFild1.Width,
                    Form1._instance.mineFild1.Height
                );
                pic = CompressPic(pic, compressRate);
                var bytePic = BitmapToBytes(pic);
                var byteList = new List<byte>();
                switch (Form1.status)
                {
                    case 30:
                        byteList.Add(0);
                        byteList.AddRange(getEightBitsByteArray(bytePic.Length));
                        byteList.AddRange(bytePic);
                        break;
                    case 31:
                        byteList.Add(1);
                        byteList.AddRange(BitConverter.GetBytes(Form1.duration));
                        break;
                    case 32:
                        byteList.Add(2);
                        byteList.AddRange(BitConverter.GetBytes(Form1.duration));
                        break;
                }
                var receiver = new byte[1024 * 1024];
                Int64 duration;
                socket.Send(byteList.ToArray());
                socket.Receive(receiver);
                Console.WriteLine(receiver[0]);
                switch (receiver[0])
                {
                    case 0:
                        if (Form1.status == 31)
                        {
                            Form1._instance.led2.timer.Close();
                            Form1._instance.led2.t.Abort();
                            Form1._instance.mineFild1.Enabled = false;
                            Form1._instance.pictureBox1.BackgroundImage = null;
                            MessageBox.Show("Lose.");
                            socket.Close();
                            return;
                        }
                        if (Form1.status == 32)
                        {
                            Form1._instance.led2.timer.Close();
                            Form1._instance.led2.t.Abort();
                            Form1._instance.mineFild1.Enabled = false;
                            Form1._instance.pictureBox1.BackgroundImage = null;
                            MessageBox.Show("Win.");
                            socket.Close();
                            return;
                        }
                        var buffer = new byte[8];
                        Array.Copy(receiver, 1, buffer, 0, 8);
                        var len = int.Parse(Encoding.UTF8.GetString(buffer));
                        buffer = new byte[len];
                        Array.Copy(receiver, 9, buffer, 0, len);
                        pic = BytesToBitmap(buffer);
                        Form1._instance.pictureBox1.BackgroundImage = pic as Image;
                        break;
                    case 1:
                        Form1._instance.pictureBox1.BackgroundImage = null;
                        duration = BitConverter.ToInt64(receiver, 1);
                        if (Form1.status==31)
                        {
                            if (Form1.duration == duration)
                            {
                                MessageBox.Show("Draw.");
                            }
                            if (Form1.duration < duration)
                            {
                                MessageBox.Show("Defeat. You: " + Form1.duration + "ms; Opponent: " + duration + "ms.");
                            }
                            if (Form1.duration > duration)
                            {
                                MessageBox.Show("Win. You: " + Form1.duration + "ms; Opponent: " + duration + "ms.");
                            }
                        }
                        else
                        {
                            if (Form1.status == 30)
                            {
                                Form1._instance.led2.timer.Close();
                                Form1._instance.led2.t.Abort();
                                Form1._instance.mineFild1.Enabled = false;
                            }
                            MessageBox.Show("You win, the opponent has hit the bomb.");
                            Form1.status = 32;
                        }
                        socket.Close();
                        return;
                    case 2:
                        Form1._instance.pictureBox1.BackgroundImage = null;
                        duration = BitConverter.ToInt64(receiver, 1);
                        if (Form1.status == 32)
                        {
                            if (Form1.duration == duration)
                            {
                                MessageBox.Show("Draw.");
                            }
                            if (Form1.duration > duration)
                            {
                                MessageBox.Show("Defeat. You: " + Form1.duration + "ms; Opponent: " + duration + "ms.");
                            }
                            if (Form1.duration < duration)
                            {
                                MessageBox.Show("Win. You: " + Form1.duration + "ms; Opponent: " + duration + "ms.");
                            }
                        }
                        if (Form1.status == 30 || Form1.status == 31)
                        {
                            if (Form1.status == 30)
                            {
                                Form1._instance.led2.timer.Close();
                                Form1._instance.led2.t.Abort();
                                Form1._instance.mineFild1.Enabled = false;
                            }
                            MessageBox.Show("Defeat.");
                            Form1.status = 0;
                        }
                        socket.Close();
                        return;
                }
                socket.Close();
                Thread.Sleep(2000);
            }
        }
        public static byte[] BitmapToBytes(Bitmap bitmap)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                bitmap.Save(ms, ImageFormat.Jpeg);
                byte[] data = new byte[ms.Length];
                ms.Seek(0, SeekOrigin.Begin);
                ms.Read(data, 0, Convert.ToInt32(ms.Length));
                return data;
            }
        }
        public static Bitmap BytesToBitmap(byte[] Bytes)
        {
            MemoryStream ms = null;
            try
            {
                ms = new MemoryStream(Bytes);
                return new Bitmap((Image)new Bitmap(ms));
            }
            catch (ArgumentNullException ex)
            {
                Console.WriteLine(ex);
                throw ex;
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex);
                throw ex;
            }
            finally
            {
                ms.Close();
            }
        }
        //后台抓取句柄截图
        public static Bitmap GetWindowCapture(IntPtr hwnd)
        {
            IntPtr hscrdc = GetWindowDC(hwnd);
            RECT rect = new RECT();
            GetWindowRect(hwnd, ref rect);
            int width = rect.Right - rect.Left;
            int height = rect.Bottom - rect.Top;
            IntPtr hbitmap = CreateCompatibleBitmap(hscrdc, width, height);
            IntPtr hmemdc = CreateCompatibleDC(hscrdc);
            SelectObject(hmemdc, hbitmap);
            PrintWindow(hwnd, hmemdc, 0);
            Bitmap bmp = Bitmap.FromHbitmap(hbitmap);
            DeleteDC(hscrdc);
            DeleteDC(hmemdc);
            return bmp;
        }
        //裁剪图片
        public static Bitmap CutPic(Bitmap b, int startX, int startY, int width, int height)
        {
            if (b == null)
            {
                return null;
            }
            int w = b.Width;
            int h = b.Height;
            if (startX >= w || startY >= h)
            {
                return null;
            }
            if (startX + width > w)
            {
                width = w - startX;
            }
            if (startY + height > h)
            {
                height = h - startY;
            }
            try
            {
                Bitmap res = new Bitmap(width, height, PixelFormat.Format24bppRgb);
                Graphics g = Graphics.FromImage(res);
                g.DrawImage(b, new Rectangle(0, 0, width, height), new Rectangle(startX, startY, width, height), GraphicsUnit.Pixel);
                g.Dispose();
                return res;
            }
            catch
            {
                return null;
            }
        }
        //压缩图片
        public static Bitmap CompressPic(Image srcImage, double percent)
        {
            int newH = int.Parse(Math.Round(srcImage.Height * percent).ToString());
            int newW = int.Parse(Math.Round(srcImage.Width * percent).ToString());
            try
            {
                Bitmap b = new Bitmap(newW, newH);
                Graphics g = Graphics.FromImage(b);
                g.InterpolationMode = InterpolationMode.Default;
                g.DrawImage(srcImage, new Rectangle(0, 0, newW, newH), new Rectangle(0, 0, srcImage.Width, srcImage.Height), GraphicsUnit.Pixel);
                g.Dispose();
                return b;
            }
            catch (Exception)
            {
                return null;
            }
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            internal int Left;
            internal int Top;
            internal int Right;
            internal int Bottom;
        }
        [DllImport("user32.dll")]
        /*
        Windows API
        该函数返回指定窗口的边框矩形的尺寸。该尺寸以相对于屏幕坐标左上角的屏幕坐标给出。
        */
        public static extern IntPtr GetWindowRect(IntPtr hWnd, ref RECT rect);
        [DllImport("gdi32.dll")]
        /*
        Windows API
        为一个设备创建设备上下文环境。
        */
        public static extern IntPtr CreateDC(string lpszDriver, string lpszDevice, string lpszOutput, IntPtr lpInitData);
        [DllImport("gdi32.dll")]
        /*
        Windows API
        对指定的源设备环境区域中的像素进行位块（bit_block）转换，以传送到目标设备环境。
        */
        public static extern int BitBlt(IntPtr hdcDest, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, UInt32 dwRop);
        [DllImport("gdi32.dll")]
        /*
        Windows API
        创建一个与指定设备兼容的内存设备上下文环境。
        */
        public static extern IntPtr CreateCompatibleDC(IntPtr hdc);
        [DllImport("gdi32.dll")]
        /*
        Windows API
        创建与指定的设备环境相关的设备兼容的位图。
        */
        public static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int nWidth, int nHeight);
        [DllImport("gdi32.dll")]
        /*
        Windows API
        选择一对象到指定的设备上下文环境中，该新对象替换先前的相同类型的对象。
        */
        public static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);
        [DllImport("gdi32.dll")]
        /*
        Windows API
        该函数删除指定的设备上下文环境。
        */
        public static extern int DeleteDC(IntPtr hdc);
        [DllImport("user32.dll")]
        /*
        Windows API
        后台抓取句柄截图
        */
        public static extern bool PrintWindow(IntPtr hwnd, IntPtr hdcBlt, UInt32 nFlags);
        [DllImport("user32.dll")]
        /*
        Windows API
        返回hwnd参数所指定的窗口的设备环境。
        */
        public static extern IntPtr GetWindowDC(IntPtr hwnd);
        [DllImport("kernel32")]
        /*
        Windows API
        返回从操作系统启动所经过的毫秒数
        */
        static extern uint GetTickCount();
    }
}

