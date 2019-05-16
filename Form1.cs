using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MineSweeper
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        enum Enumbuttonstatus
        {
            Opened,      //被翻开
            Unopened,    //未翻开
            Marked,      //红旗标记
            Suspected    //问号标记
        }

        private bool Initialized = false;
        HashSet<int> MinesPosition = new HashSet<int>();
        private int UnopenedButtons = 100;
        Bitmap[] bitmaps = new Bitmap[]
        {
            new Bitmap("？.png"),
            new Bitmap("1.png"),
            new Bitmap("2.png"),
            new Bitmap("3.png"),
            new Bitmap("4.png"),
            new Bitmap("5.png"),
            new Bitmap("6.png"),
            new Bitmap("7.png"),
            new Bitmap("8.png"),
            new Bitmap("boom.png"),
            new Bitmap("flag.png"),
            new Bitmap("empty.png"),
            new Bitmap("opened.png")
        };

        private void ButtonDoubleClick(object sender, MouseEventArgs e)
        {
            Button button = sender as Button;
            MessageBox.Show(button.TabIndex.ToString());
        }

        private void MouseLeftButtonClick(object sender, MouseEventArgs e)
        {
            _Button CurrentButton = sender as _Button;
            if (CurrentButton.IsClicked) return;
            int Position = CurrentButton.TabIndex;
            if (!Initialized)
            {
                InitMinePanel(Position);
                Initialized = true;
            }
            if (buttons[Position].ContainsMine)
            {
                foreach (var i in MinesPosition)
                {
                    buttons[i].Image = bitmaps[9];
                }
                MessageBox.Show("游戏结束！");
                Application.Exit();
            }
            else
            {
                if (buttons[Position].AroundNumber != 0)
                {
                    buttons[Position].Image = bitmaps[buttons[Position].AroundNumber];
                    buttons[Position].IsClicked = true;
                    buttons[Position].Status = Enumbuttonstatus.Opened;
                    //buttons[Position].Image = bitmaps[12];
                    --UnopenedButtons;
                }
                else if (buttons[Position].AroundNumber == 0)
                {
                    buttons[Position].Enabled = false;
                    buttons[Position].IsClicked = true;
                    buttons[Position].Status = Enumbuttonstatus.Opened;
                    --UnopenedButtons;
                    BreadthFirstSearch(Position);
                }
            }
        }

        //广度遍历算法
        private void BreadthFirstSearch(int Position)
        {
            Queue<int> AuxiliaryQueue = new Queue<int>();
            AuxiliaryQueue.Enqueue(Position);
            while (!(AuxiliaryQueue.Count == 0))
            {
                int QueueFront = AuxiliaryQueue.First();
                AuxiliaryQueue.Dequeue();
                foreach (var Around in GetRoundbuttons(QueueFront))
                {
                    if (!buttons[Around].IsClicked)
                    {
                        if (buttons[Around].AroundNumber == 0)
                        {
                            buttons[Around].Enabled = false;
                            buttons[Around].IsClicked = true;
                            buttons[Around].Status = Enumbuttonstatus.Opened;
                            --UnopenedButtons;
                            AuxiliaryQueue.Enqueue(Around);
                        }
                        else
                        {
                            buttons[Around].Image = bitmaps[buttons[Around].AroundNumber];
                            buttons[Around].IsClicked = true;
                            buttons[Around].Status = Enumbuttonstatus.Opened;
                            //buttons[Around].Image = bitmaps[12];
                            --UnopenedButtons;
                        }
                    }
                }
            }
        }

        private void MouseRightButtonClick(object sender, MouseEventArgs e)
        {
            _Button button = sender as _Button;
            if (button.Status == Enumbuttonstatus.Unopened)
            {
                button.Status = Enumbuttonstatus.Marked;
                button.Image = bitmaps[10];
            }
            else if (button.Status == Enumbuttonstatus.Marked)
            {
                button.Status = Enumbuttonstatus.Suspected;
                button.Image = bitmaps[0];
            }
            else if (button.Status == Enumbuttonstatus.Suspected)
            {
                button.Status = Enumbuttonstatus.Unopened;
                button.Image = bitmaps[11];
            }
            else
            {

            }
        }

        private void ButtonSingleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) MouseLeftButtonClick(sender, e);
            if (e.Button == MouseButtons.Right) MouseRightButtonClick(sender, e);
            if (UnopenedButtons == 10)
            {
                MessageBox.Show("成功！");
                Application.Exit();
            }
        }

        private void InitMinePanel(int _Exception)
        {
            Random random = new Random();

            int i = 0;
            while(i < 10)
            {
                var x = random.Next(0, 100);
                var Around = GetRoundbuttons(x);
                if (!MinesPosition.Contains(x) && x != _Exception && !Around.Contains(_Exception)) 
                {
                    MinesPosition.Add(x);
                    buttons[x].ContainsMine = true;
                    foreach (var AroundBlock in GetRoundbuttons(x))
                    {
                        ++buttons[AroundBlock].AroundNumber;
                    }
                    ++i;
                }
            }
        }
        private IEnumerable<int> GetRoundbuttons(int i)
        {
            int x = i % 10;
            int y = i / 10;
            if (y >= 1) 
                yield return i - 10;
            if (y <= 8)
                yield return i + 10;
            if (x <= 8)
                yield return i + 1;
            if (x >= 1)
                yield return i - 1;
            if (x >= 1 && y >= 1)
                yield return i - 11;
            if (x >= 1 && y <= 8)
                yield return i + 9;
            if (x <= 8 && y <= 8)
                yield return i + 11;
            if (x <= 8 && y >= 1)
                yield return i - 9;
        }
    }
}
