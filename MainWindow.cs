using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Threading.Tasks;
using System.Windows.Forms;
using Rug.Osc;

namespace OSC_AutoClicker
{
    public partial class OSC_AutoClicker : Form
    {
        //ターゲットマーカー最大数
        const int NUM_BULLSEYES = 8;

        //OSCレシーバー
        private OscReceiver oscReceiver;

        // OSC受信待ちをするタスク
        private Task oscReceiveTask = null;

        // ターゲットマーカーのウィンドウ
        BullsEye[] bullsEye = new BullsEye[NUM_BULLSEYES];

        public OSC_AutoClicker()
        {
            InitializeComponent();
            //的を表示
            checkedListBox1.SetItemChecked(0, true);

            //受信先のIPアドレスを設定
            System.Net.IPAddress address = System.Net.IPAddress.Parse("127.0.0.1");
            //受信先のポートを設定
            int receivePort = 9001;

            oscReceiver = new OscReceiver(address, receivePort);
            oscReceiver.Connect();

            //OSC受信用のタスクを生成
            oscReceiveTask = new Task(() => OscListenProcess());

            //受信タスクをスタート
            oscReceiveTask.Start();
        }

        //一定周期ごとに
        private void timer1_Tick(object sender, EventArgs e)
        {
            for (int i = 0; i < NUM_BULLSEYES; i++)
            {
                //全てのターゲットマーカーを再度表示
                if (bullsEye[i] != null && !bullsEye[i].IsDisposed && !bullsEye[i].Visible)
                {
                    bullsEye[i].Visible = true;
                }
            }
        }

        //カーソルを動かしてクリックをする処理
        //int index　クリック先のターゲットマーカー
        private void AutoClick(int index)
        {
            //存在しないターゲットマーカーを操作しようとした場合、無視する
            if (index < 0 || index >= NUM_BULLSEYES) { return; }
            //ターゲットマーカーが非表示の時も何もしない
            if (bullsEye[index] == null || bullsEye[index].IsDisposed || !bullsEye[index].Visible) return; 

            //ターゲットマーカーを一時的に非表示
            for (int i = 0; i < NUM_BULLSEYES; i++)
            {
                if (bullsEye[i] == null || bullsEye[i].IsDisposed){ continue; }
                bullsEye[i].Visible = false;
            }
            //位置を取得してクリックを実行
            var pos = bullsEye[index].GetCenterPos();
            NativeMethod.SetCursor(pos.X, pos.Y);
            NativeMethod.MouseClick();

            //効果音の再生
            using (SoundPlayer player = new SoundPlayer(@"Click.wav"))
            {
                try
                {
                    player.Load();
                    player.Play();
                }
                //再生に失敗したらエラー出力
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
        private void OSC_AutoClicker_FormClosing(object sender, FormClosingEventArgs e)
        {
            // アプリケーション終了時にOSC接続を閉じる
            oscReceiver.Close();

            for (int i = 0; i < NUM_BULLSEYES; i++)
            {
                //的を削除
                if (bullsEye[i] != null && !bullsEye[i].IsDisposed)
                {
                    bullsEye[i].Close();
                }
            }
        }


        // OSC受信をListenする処理
        // Taskで実行されており、繰り返し受信を確認し
        // 接続が終了したら処理を終わらせる
        private void OscListenProcess()
        {
            try
            {
                // OSCレシーバーが終了されるまで繰り返し処理する
                while (oscReceiver.State != OscSocketState.Closed)
                {
                    // 受信待ち(メッセージを受信したら処理が帰ってくる)
                    OscPacket packet = oscReceiver.Receive();

                    for(int i = 1; i <= NUM_BULLSEYES; i++)
                    {
                        // 受信したメッセージが条件に合うならクリックを実行
                        if (packet.ToString() == String.Concat("/avatar/parameters/click", i.ToString(), ", True"))
                        {
                            this.Invoke(new Action<int>(AutoClick), i - 1);
                            Console.WriteLine(packet.ToString());
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                // 例外処理　発生時はコンソールに出力
                // 　ただし
                //    m_OscReceiver.Receive()で受信待ち状態の時に終了処理(m_OscReceiver.close())をすると
                //    正しい処理でもExceptionnとなるため、接続中かで正しい処理か例外かを判断する
                if (oscReceiver.State == OscSocketState.Connected)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        //チェック状態を変更したらターゲットマーカーも表示非表示を切り替える
        private void checkedListBox1_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            Boolean isChecked = (e.NewValue == CheckState.Checked);
            //チェック状態をもとにターゲットマーカーを表示/非表示

            if (isChecked)
            {
                //ターゲットマーカーを表示
                bullsEye[e.Index] = new BullsEye();
                bullsEye[e.Index].Show(this);
                bullsEye[e.Index].SetIndex(e.Index);


                //ターゲットマーカー位置を初期化

                bullsEye[e.Index].Left = this.Left;
                bullsEye[e.Index].Top = this.Top;
            }
            else
            {
                //ターゲットマーカーを閉じる
                bullsEye[e.Index].Close();
            }
        }

        //ボタンを押したらターゲットマーカー1を押した扱いにする
        private void button1_Click(object sender, EventArgs e)
        {
            AutoClick(0);
        }

    }
}
