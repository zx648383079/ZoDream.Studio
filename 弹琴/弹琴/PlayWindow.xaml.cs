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
using System.Windows.Shapes;
using System.Windows.Threading;
using Midi;

namespace 弹琴
{
    /// <summary>
    /// PlayWindow.xaml 的交互逻辑
    /// </summary>
    public partial class PlayWindow : Window
    {
        /// <summary>
        /// 初始化
        /// </summary>
        public PlayWindow()
        {
            InitializeComponent();
        }

        DispatcherTimer timer;
        List<Midi.MidiTime> _midiTimes;
        int index;
        /// <summary>
        /// 播放的委托
        /// </summary>
        /// <param name="num"></param>
        public delegate void PlayDelegate(int num);
        /// <summary>
        /// 播放的事件
        /// </summary>
        public event PlayDelegate PlayEvent;
        /// <summary>
        /// 停止的委托
        /// </summary>
        public delegate void StopDelegate();
        /// <summary>
        /// 停止的事件
        /// </summary>
        public event StopDelegate StopEvent;

        private void PlayBtn_Click(object sender, RoutedEventArgs e)
        {
            StringToInt midi = new StringToInt(ScoreTb.Text);

            _midiTimes = midi.GetMidi();

            index = 0;
            if (_midiTimes.Count>0)
            {
                timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromMilliseconds(500);
                timer.Tick += Timer_Tick;
                timer.Start();
            }
            
            //PlayEvent(int.Parse(ScoreTb.Text)+12);
            
        }
        /// <summary>
        /// 设置时间
        /// </summary>
        private void SetTime()
        {
            double time = _midiTimes[index].Time*TimeSlider.Value;
            timer.Interval = TimeSpan.FromSeconds(time);
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            StopEvent();
            if (index<_midiTimes.Count)
            {
                //解决0
                if (_midiTimes[index].Midi<-12)
                {

                }
                else
                {
                      PlayEvent(_midiTimes[index].Midi+12);
                }

                
                SetTime();
            }
            else
            {
                timer.Stop();
            }
            index++;
        }

        private void TimeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<Double> e)
        {
            if (timer !=null)
            {
                
            }
        }
    }
}
