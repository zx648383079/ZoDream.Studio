using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace 弹琴
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Init();
        }

        MidiBase midiBase;
        Rectangle indexRect;     //当前发音位置

        public void Init()
        {
            midiBase = new MidiBase();
            //填充乐器数据
            foreach (string item in Enum.GetNames(typeof(MidiToneType)))
            {
                MidiTypeCb.Items.Add(item);
            }

            //画格子
            for (int i = 0; i < 56; i++)
            {
                ColumnDefinition col = new ColumnDefinition();

                KeyGrid.ColumnDefinitions.Add(col);
            }

            
                
                

            for (int i = 0; i < 48; i++)
            {
                Rectangle rect = new Rectangle();
                rect.SetValue(Grid.ColumnSpanProperty, 2);

                int tem = i % 12;         //计算余数，
                int tem1 = i / 12;
                int tem2 = tem > 4 ? 1 : 0;

                rect.SetValue(Grid.ColumnProperty, tem1 * 14 + tem + tem2);
                rect.MouseDown += Rect_MouseDown;
                rect.MouseUp += Rect_MouseUp;
                rect.MouseMove += Rect_MouseMove;
                //判断是黑键还是白键
                string name;
                if (tem ==1 || tem ==3 || tem ==6 || tem ==8 || tem ==10)
                {
                    name = "black" + i.ToString("00");               
                    rect.Fill = new SolidColorBrush(Colors.Black);
                    rect.SetValue(Grid.ZIndexProperty, 1);                //通过zindex改变黑白键的显示顺序
                }
                else
                {
                    name = "white" + i.ToString("00");
                    rect.Fill = new SolidColorBrush(Colors.White);
                    rect.SetValue(Grid.RowSpanProperty, 2);
                    rect.SetValue(Grid.ZIndexProperty, 0);
                }
                rect.Name = name;                               //做标记，包含黑白键和音阶信息

                KeyGrid.Children.Add(rect);

                KeyGrid.RegisterName(name, rect);               //必须这样注册才能用 findname找到
            }
        }

        private void Rect_MouseMove(object sender, MouseEventArgs e)
        {
            if (midiBase !=null)
            {
                //通过判断是否移动
                if (indexRect !=null && indexRect !=(Rectangle)sender)
                {
                    Up();
                    Down((Rectangle)sender);
                }
            }
        }

        private void Rect_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Up();
        }
        /// <summary>
        /// 取消发音
        /// </summary>
        private void Up()
        {
            if (midiBase != null && indexRect !=null)
            {
                //回归原色
                if (indexRect.Name[0] == 'b')
                {
                    indexRect.Fill = new SolidColorBrush(Colors.Black);
                }
                else
                {
                    indexRect.Fill = new SolidColorBrush(Colors.White);
                }


                string tem = indexRect.Name.Substring(5, 2);
                UInt32 tem1 = UInt32.Parse(tem) + 48;
                byte i = (byte)(tem1);
                midiBase.KeyUp(i);

                indexRect = null;
            }
        }

        private void Rect_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Down((Rectangle)sender);
        }
        /// <summary>
        /// 鼠标按下
        /// </summary>
        /// <param name="rect"></param>
        private void Down(Rectangle rect)
        {
            if (midiBase != null)
            {
                indexRect = rect;

                indexRect.Fill = new SolidColorBrush(Colors.OrangeRed);

                string tem = indexRect.Name.Substring(5, 2);     //获取音阶
                UInt32 tem1 = UInt32.Parse(tem) + 48;
                byte i = (byte)(tem1);
                midiBase.KeyDown(i);
            }
        }
        /// <summary>
        /// 更改乐器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MidiTypeCb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            midiBase.SetTimber((byte)MidiTypeCb.SelectedIndex);
        }
        /// <summary>
        /// 改变音量
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<Double> e)
        {
            if (midiBase !=null)
            {
                midiBase.Volume = (byte)((int)VolumeSlider.Value);
            }
            
        }
        /// <summary>
        /// 为防止鼠标移出控件后返回出现依旧能执行 move
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void KeyGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            if (indexRect !=null)
            {
                Up();
            }
        }
        /// <summary>
        /// 播放
        /// </summary>
        /// <param name="num"></param>
        public void Play(int num)
        {
            int tem = num % 12;
            string name;
            if (tem == 1 || tem == 3 || tem == 6 || tem == 8 || tem == 10)
            {
                name = "black" + num.ToString("00");
            }
            else
            {
                name = "white" + num.ToString("00");
            }
            Rectangle rect = KeyGrid.FindName(name) as Rectangle;
            Down(rect);
        }
        /// <summary>
        /// 弹出窗口，注册委托
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PlayBtn_Click(object sender, RoutedEventArgs e)
        {
            PlayWindow playWindow = new PlayWindow();
            playWindow.PlayEvent += new PlayWindow.PlayDelegate(Play);
            playWindow.StopEvent += new PlayWindow.StopDelegate(Up);
            playWindow.ShowDialog();
        }
    }
}
