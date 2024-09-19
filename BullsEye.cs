using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OSC_AutoClicker
{
    public partial class BullsEye : Form
    {
        // マウスポインタの位置を保存する
        private Point mousePoint;
        public BullsEye()
        {
            InitializeComponent();
        }
        public void SetIndex(int i)
        {
            //ターゲットマーカーの描画設定
            switch(i)
            {
                case 0:
                    pictureBox1.Image = Properties.Resources.kakashi_aiming1;
                    break;
                case 1:
                    pictureBox1.Image = Properties.Resources.kakashi_aiming2;
                    break;
                case 2:
                    pictureBox1.Image = Properties.Resources.kakashi_aiming3;
                    break;
                case 3:
                    pictureBox1.Image = Properties.Resources.kakashi_aiming4;
                    break;
                case 4:
                    pictureBox1.Image = Properties.Resources.kakashi_aiming5;
                    break;
                case 5:
                    pictureBox1.Image = Properties.Resources.kakashi_aiming6;
                    break;
                case 6:
                    pictureBox1.Image = Properties.Resources.kakashi_aiming7;
                    break;
                case 7:
                    pictureBox1.Image = Properties.Resources.kakashi_aiming8;
                    break;
                default:
                    break;
            }
        }
        public NativeMethod.POINT GetCenterPos()
        {
            //ターゲットマーカーの中心の座標を返す
            Point p = this.PointToScreen(new Point(Size.Width / 2, Size.Height / 2));
            return new NativeMethod.POINT(p.X,p.Y);
        }

        private void BullsEye_MouseDown(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
            {
                //クリック押下時の位置を記憶する
                mousePoint = new Point(e.X, e.Y);
            }
        }

        private void BullsEye_MouseMove(object sender, MouseEventArgs e)
        {
            //差分を取って移動する
            if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
            {
                this.Left += e.X - mousePoint.X;
                this.Top += e.Y - mousePoint.Y;
            }
        }
    }
}
