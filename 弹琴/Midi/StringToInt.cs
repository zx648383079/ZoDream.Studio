using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Midi
{
    /// <summary>
    /// 文本转换为音阶
    /// </summary>
    public class StringToInt
    {
        private string _text;
        /// <summary>
        /// MIDI内容
        /// </summary>
        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="text">MIDI文件</param>
        public StringToInt(string text)
        {
            text=Regex.Replace(text, @"\s", "");
            this._text = text;
        }
        /// <summary>
        /// 转换成音阶：时间
        /// </summary>
        /// <returns></returns>
        public List<MidiTime> GetMidi()
        {
            List<MidiTime> midiTimes = new List<MidiTime>();
            if (!string.IsNullOrEmpty(_text))
            {
                MatchCollection ms= Regex.Matches(_text, @"(?<a>[#b]){0,1}(?<b>[0-7])(?<c>·){0,1}");
                foreach (Match item in ms)
                {
                    #region 获取真正的音阶
                    int num;
                    switch (int.Parse(item.Groups["b"].Value))
                    {
                        case 1:
                            num = 0;
                            break;
                        case 2:
                            num = 2;
                            break;
                        case 3:
                            num = 4;
                            break;
                        case 4:
                            num = 5;
                            break;
                        case 5:
                            num = 7;
                            break;
                        case 6:
                            num = 9;
                            break;
                        case 7:
                            num = 11;
                            break;
                        default:
                            num = -20;   //这里应该变无声
                            break;
                    }
                    #endregion


                    #region 处理升降阶
                    switch (item.Groups["a"].Value)
                    {
                        case "#":
                            num++;
                            break;
                        case "b":
                            num--;
                            break;
                        default:
                            break;
                    }
                    #endregion

                    #region 处理延长阶

                    double time = 1;

                    if ("·".Equals(item.Groups["c"].Value))
                    {
                        time = 0.5;
                    }
                    #endregion


                    midiTimes.Add(new MidiTime(num, time));
                }
            }

            return midiTimes;
        }

    }
}
