using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidiPlayer.Midi
{
    public class File
    {

        #region 读取文件
        public static Score Read(Stream stream)
        {
            ushort trackNums;
            var score = ReadHeader(stream, out trackNums);
            ReadTracks(score, trackNums);
            return score;
        }

        /// <summary>
        /// 异步版本
        /// </summary>
        public static Task ReadAsync(Stream inStream)
        {
            Action act = () => Read(inStream);
            return Task.Run(act);
        }
        #endregion

        #region 写入文件
        public static void Write(Score score, Stream stream)
        {
            if (score.TrackList.Count == 0)
            {
                throw new InvalidOperationException("轨道列表不能为空");
            }
            if (score.FormatType == 0 && score.TrackList.Count > 1)
            {
                throw new InvalidOperationException("单音轨格式只允许存在一个轨道");
            }

            // 开始写数据
            byte[] buffer = null;
            // 写入头部标识
            buffer = Encoding.ASCII.GetBytes(Score.CHUNK_ID);
            outStream.Write(buffer, 0, buffer.Length);
            // 写入头部长度，固定值，六个字节
            buffer = Convert32BitnumToBEBytes(6);
            outStream.Write(buffer, 0, buffer.Length);
            // 写入轨道类型，两个字节
            buffer = Convert16BitnumToBEBytes((ushort)score.FormatType);
            outStream.Write(buffer, 0, buffer.Length);
            // 写入轨道总数
            buffer = Convert16BitnumToBEBytes((ushort)score.TrackList.Count);
            outStream.Write(buffer, 0, buffer.Length);
            // 写入每个四分音符的延时值
            buffer = Convert16BitnumToBEBytes(score.TicksPerBeat);
            outStream.Write(buffer, 0, buffer.Length);
            outStream.Flush();
            // 头部写完

            // 接着写轨道
            // 每个轨道数据块都是连在一起的，按顺序写入就好了
            foreach (Track track in score.TrackList)
            {
                // 写入轨道标识
                buffer = Encoding.ASCII.GetBytes(Track.CHUNK_ID);
                outStream.Write(buffer, 0, buffer.Length);
                // 事件长度暂时不能知道，所以先合成事件数据
                byte[] eventsData = CombinEventdatasForTrack(track);
                // 现在可以知道事件数据的长度了
                buffer = Convert32BitnumToBEBytes((uint)eventsData.Length);
                // 写入的时候要先写长度
                outStream.Write(buffer, 0, buffer.Length);
                // 再写入事件数据
                outStream.Write(eventsData, 0, eventsData.Length);
                outStream.Flush();
            }
            /* 至此，整个 Midi 文件数据写完了 */
        }

        /// <summary>
        /// 异步版本
        /// </summary>
        public static Task WriteAsync(Score score, Stream stream)
        {
            Action act = ()=> Write(stream);
            return Task.Run(act);
        }
        #endregion


        #region 辅助方法
                /// <summary>
        /// 生成可变字节
        /// </summary>
        /// <param name="val">输入值</param>
        /// <returns>基于 Big-Endian 的字节数组</returns>
        public static byte[] ConvertToVariableLength(uint val)
        {
            if (val == 0)
                return new byte[] { 0 };
            List<uint> tempvalues = new List<uint>();
            uint tempval = val;
            do
            {
                /*
                 * 从右往左，每7个二进制位组合
                 * 比如有两个字节
                 * 0001 1100    1111 1011
                 * 总共十六位，去掉左边两位，变成14位，再组合起来
                 * 0111001 1111011
                 * 0x7F = 0111 1111，去掉第7位以后的数字
                 */
                tempvalues.Add(tempval & 0x7F);
            }
            while ((tempval >>= 7) > 0);// 每次移动7位，正好形成每个字节的前7位（第8位是标志位）

            // 每7位形成一个字节后
            // 第一个字节的第8位为0，表示可变长度的结束位
            // 其它字节的第8位必须为1（0x80 = 1000 0000）
            // 表示可变长度的非结束位
            // 与 0x80 组合后正好可以将第8位变为1，而其他位不会丢失
            if (tempvalues.Count > 1)
            {
                for (int k = 1; k < tempvalues.Count; k++)
                {
                    tempvalues[k] |= 0x80;
                }
            }

            // 反转顺序，必须，因为存放到MIDI文件中的顺序是正好相反的
            tempvalues.Reverse();

            byte[] data = new byte[tempvalues.Count];
            for (int i = 0; i < tempvalues.Count; i++)
            {
                data[i] = Convert.ToByte(tempvalues[i]);
            }

            return data;
        }

        /// <summary>
        /// 从可变长度字节数组中读出实际长度
        /// </summary>
        public static uint ConvertFromVariableLength(byte[] buffer)
        {
            // 读出来的是 Big Endian Bytes
            // 如果当前CPU是以 Little Endian 顺序存储
            // 就必须反转字节顺序
            if (buffer.Length == 1 && buffer[0] == 0x00)
            {
                return 0;
            }

            Array.Reverse(buffer);//反转

            uint result = 0;
            // 有效位是前7位，即 0111 1111
            // 第8位是标志位，无需处理
            uint validates = 0x7F;
            /*
             假设有三个字节，分别是
             0111 1001
             0101 1111
             0000 0011
             各取前7位，组成结果
              11 101 1111 111 1001

            第一个字节向左移0位，第二个字节向左移7位，第三个字节向左移14位……
            即
              0000 0000  0000 0000  0000 0000  0111 1111
              0000 0000  0000 0000  0011 1111  1000 0000
              0000 0000  0001 1111  1100 0000  0000 0000
              0000 1111  1110 0000  0000 0000  0000 0000
            然后组合起来就是一个整数值了
             */
            for (int i = 0; i < buffer.Length; i++)
            {
                uint temp = buffer[i] & validates;
                temp <<= (7 * i);
                result |= temp;
            }
            return result;
        }

        /// <summary>
        /// 将16位整数转为 Big Endian 字节顺序
        /// </summary>
        public static byte[] Convert16BitnumToBEBytes(ushort val)
        {
            if (val == 0)
            {
                return new byte[] { 0, 0 };
            }
            uint temp = val;
            List<byte> parts = new List<byte>();
            do
            {
                byte b = (byte)(temp & 0xFF);
                parts.Add(b);
            }
            while ((temp >>= 8) > 0);

            byte[] srcArr = parts.ToArray();
            byte[] newArr = new byte[2];
            srcArr.CopyTo(newArr, 0);
            Array.Reverse(newArr);
            return newArr;
        }

        /// <summary>
        /// 从字节中读出16位整数
        /// </summary>
        public static ushort Convert16BitFromBEBytes(byte[] buffer)
        {
            if (buffer.Length != 2)
            {
                throw new ArgumentException("16位数值只允许两个字节");
            }
            ushort result = 0;
            // 反转
            Array.Reverse(buffer);
            for (int i = 0; i < buffer.Length; i++)
            {
                result |= (ushort)(buffer[i] << (8 * i));
            }
            return result;
        }

        /// <summary>
        /// 将 32 位整数转为 Big endian 顺序的字节
        /// </summary>
        public static byte[] Convert32BitnumToBEBytes(uint val)
        {
            if (val == 0)
            {
                return new byte[] { 0, 0, 0, 0 };
            }
            uint temp = val;
            List<byte> parts = new List<byte>();
            do
            {
                byte tb = Convert.ToByte(temp & 0xFF);
                parts.Add(tb);
            }
            while ((temp >>= 8) > 0);
            // 32位整数是4个字节，但是转化后可能不足4个字节
            byte[] src = parts.ToArray();
            byte[] newbuffer = new byte[4];
            src.CopyTo(newbuffer, 0);
            // 反转
            Array.Reverse(newbuffer);
            return newbuffer;
        }

        /// <summary>
        /// 读出32位整数
        /// </summary>
        public static uint Convert32BitFromBEBytes(byte[] buffer)
        {
            if (buffer.Length != 4)
            {
                throw new ArgumentException("32位整数必须是四个字节");
            }
            uint res = 0;
            // 反转
            Array.Reverse(buffer);
            for (int n = 0; n < buffer.Length; n++)
            {
                res |= (uint)(buffer[n] << (8 * n));
            }
            return res;
        }

        /// <summary>
        /// 将整数值转为 24 位 big endian 顺序的字节，这个用于设置节拍元数据
        /// </summary>
        public static byte[] Convert24BitnumToBEBytes(uint val)
        {
            /*
             * 0000 0000  0000 0000  1111 1111
             * 0000 0000  1111 1111  0000 0000
             * 1111 1111  0000 0000  0000 0000
             */
            List<byte> data = new List<byte>();
            for (int i = 0; i < 3; i++)
            {
                uint temp = val >> (i * 8);
                temp &= 0xFF;
                data.Add((byte)temp);
            }
            data.Reverse();
            return data.ToArray();
        }

        /// <summary>
        /// 从字节数组中读出24位数值
        /// </summary>
        public static uint Convert24BitFromBEBytes(byte[] buffer)
        {
            if (buffer.Length != 3)
            {
                throw new ArgumentException("24位整数必须为三个字节");
            }
            Array.Reverse(buffer); //反转
            uint res = 0;
            for (int i = 0; i < buffer.Length; i++)
            {
                uint t = buffer[i];
                res |= t << (i * 8);
            }
            return res;
        }

        /// <summary>
        /// 用单个字节存放有符号整数。主要用于转换调号
        /// </summary>
        public static byte ConvertSignedValToSinglebyte(short val)
        {
            // 负数在计算机中是以 取反 + 1（补码）的形式存储
            /*
             * 比如-7，+7的表示如下
             * 0000 0000  0000 0111
             * 要变成-7，首先要取反，变成
             * 1111 1111  1111 1000
             * 然后加1（补码），得到
             * 1111 1111  1111 1001
             * 可是这里头有两个字节，若能存一个字节，那只好把高字节去掉，只要跟0xFF做【与】运算就能去掉了。
             * 
             *  1111 1111  1111 1001
             * -------- And ----------
             *  0000 0000  1111 1111
             *  ------- 结果 ---------
             *  0000 0000  1111 1001
             */
            short r = (short)(val & 0xFF);
            return (byte)r;
        }
        /// <summary>
        /// 从单个字节中读出带符号的整数
        /// </summary>
        public static short ConvertSignedValFromSinglebyte(byte bt)
        {
            short result = 0;
            // 如果第8位为0，表示大于/等于0的数，如果为1则为负值
            if ((bt & 0x80) == 0x80)
            {
                // 负值 1111 1111  1xxx xxxx
                result = (short)(0xFF00 | bt);
            }
            else
            {
                result = bt;
            }
            return result;
        }
        #endregion



        #region 读取文件私有方法
        /// <summary>
        /// 读取文件头数据块
        /// </summary>
        private static Score ReadHeader(Stream stream, out ushort trackNums)
        {
            // 先读标识
            byte[] buffer = null;
            buffer = new byte[4];
            stream.Read(buffer, 0, 4);
            string hdid = Encoding.ASCII.GetString(buffer);
            if(hdid.Equals(Score.CHUNK_ID) == false)
            {
                throw new Exception("这不是 MIDI 文件");
            }
            // 接着读长度
            buffer = new byte[4];
            stream.Read(buffer, 0, 4);
            uint len = Convert32BitFromBEBytes(buffer);
            if (len != 6)
            {
                throw new Exception("头部长度不正确");
            }
            var score = new Score();
            // 读轨道类型
            buffer = new byte[2];
            stream.Read(buffer, 0, 2);
            score.FormatType = Convert16BitFromBEBytes(buffer);
            // 读轨道数
            buffer = new byte[2];
            stream.Read(buffer, 0, 2);
            trackNums = Convert16BitFromBEBytes(buffer);
            // 读每个四分音符的tick值
            buffer = new byte[2];
            stream.Read(buffer, 0, buffer.Length);
            score.TicksPerBeat = Convert16BitFromBEBytes(buffer);
            return score;
        }
        /// <summary>
        /// 读取轨道数据块
        /// </summary>
        private void ReadTracks(Stream stream, ushort trackNums)
        {
            for(int i = 0; i < trackNums; i++)
            {
                var track = new Track();
                // 先读音轨区标识
                byte[] buffer = null;
                buffer = new byte[4];
                stream.Read(buffer, 0, 4);
                string chunkID = Encoding.ASCII.GetString(buffer);
                if (!chunkID.Equals(Track.CHUNK_ID))
                {
                    throw new Exception("找不到轨道标识");
                }
                // 读事件列表长度
                stream.Read(buffer, 0, 4);
                uint chunkLen = Convert32BitFromBEBytes(buffer);
                // 把事件块加载到内存中处理
                using(MemoryStream ms = new MemoryStream())
                {
                    buffer = new byte[chunkLen];
                    stream.Read(buffer, 0, buffer.Length);
                    ms.Write(buffer, 0, buffer.Length);
                    ms.Position = 0L;// 定位于内存流的开始处
                    ReadEvents(track, ms);
                }
                Score.TrackList.Add(track);
            }
        }

        /// <summary>
        /// 读取可变长度值
        /// </summary>
        private uint ReadVariableLength(Stream stream)
        {
            List<byte> temps = new List<byte>();
            int n;
            do
            {
                n = stream.ReadByte();
                temps.Add((byte)n);
            }
            while ((n & 0x80) == 0x80);
            return ConvertFromVariableLength(temps.ToArray());
        }

        /// <summary>
        /// 读取事件列表
        /// </summary>
        private void ReadEvents(Track theTrack, Stream stream)
        {
            // 这是个死循环，直至遇到轨道结束标志 FF 2F 00
            while (true)
            {
                // 先读 Delta Time 这是动态长度，比较麻烦
                uint deltaTime = ReadVariableLength(stream);
                // 读取一个字节，用来判断事件类型
                int flag = stream.ReadByte();
                // 优先处理元数据
                if (flag == EventTypes.Meta)
                {
                    uint dataLen = 0;
                    byte[] buffer = null;
                    // 再读一个字节，可以确定元数据类型
                    int metatype = stream.ReadByte();
                    // 如果是轨道结束，则跳出循环
                    if (metatype == MetaEventTypes.TrackEnd)
                    {
                        // 虽长度是0，但也占了一个字节
                        // 不能不读，否则后面内容就对不齐了
                        dataLen = ReadVariableLength(stream);
                        if (dataLen != 0)
                        {
                            throw new Exception("轨道结束标记长度应为0");
                        }
                        TrackEndEvent end = new TrackEndEvent();
                        end.DeltaTime = deltaTime;
                        theTrack.EventList.Add(end);
                        break;
                    }
                    else //如果是其他元数据，继续处理
                    {
                        switch (metatype)
                        {
                            case MetaEventTypes.SetTempo:
                                // 读长度，此类型为三个字节
                                dataLen = ReadVariableLength(stream);
                                buffer = new byte[dataLen];
                                stream.Read(buffer, 0, buffer.Length);
                                SetTempoEvent tempo = new SetTempoEvent();
                                // 读出微秒数
                                uint microsecs = Convert24BitFromBEBytes(buffer);
                                tempo.MicrosecondsPerQuarterNote = microsecs;
                                // 不能忘了 delta time 值
                                tempo.DeltaTime = deltaTime;
                                // 把事件添加到列表中
                                theTrack.EventList.Add(tempo);
                                break;
                            case MetaEventTypes.TimeSignature:
                                // 读取长度，正常是四个字节
                                dataLen = ReadVariableLength(stream);
                                if(dataLen != 4)
                                {
                                    throw new Exception("Time Signature 元数据内容不正确");
                                }
                                buffer = new byte[dataLen];
                                stream.Read(buffer, 0, buffer.Length);
                                // 生成事件
                                TimeSignatureEvent timesgev = new TimeSignatureEvent();
                                // delta time
                                timesgev.DeltaTime = deltaTime;
                                // 四个字节依次赋值
                                timesgev.Numerator = buffer[0];
                                timesgev.Denominator = buffer[1];
                                timesgev.Metronome = buffer[2];
                                timesgev.The32nds = buffer[3];
                                // 把事件添加到列表中
                                theTrack.EventList.Add(timesgev);
                                break;
                            case MetaEventTypes.KeySignature:
                                // 读长度，正常是两个字节
                                dataLen = ReadVariableLength(stream);
                                if (dataLen != 2)
                                {
                                    throw new Exception("Key Signature 元数据内容不正确");
                                }
                                buffer = new byte[dataLen];
                                stream.Read(buffer, 0, buffer.Length);
                                KeySignatureEvent keysg = new KeySignatureEvent();
                                keysg.DeltaTime = deltaTime;
                                keysg.Key = ConvertSignedValFromSinglebyte(buffer[0]);
                                keysg.Scale = buffer[1];
                                // 添加到事件列表中
                                theTrack.EventList.Add(keysg);
                                break;
                        }
                    }
                }
                else
                {
                    // 然后处理通道事件
                    byte tempbyte;
                    var channelevType = flag & 0xF0;
                    // 通道号，标识字节的后4位
                    byte channel = (byte)(flag & 0x0F);
                    switch (channelevType)
                    {
                        case EventTypes.NoteOn:
                            NoteOnEvent noteon = new NoteOnEvent();
                            noteon.DeltaTime = deltaTime;
                            noteon.Channel = channel;
                            // 第二个字节是音符
                            tempbyte = (byte)stream.ReadByte();
                            noteon.Note = tempbyte;
                            // 第三个字节是音速
                            tempbyte = (byte)stream.ReadByte();
                            noteon.Velocity = tempbyte;
                            // 添加到事件列表中
                            theTrack.EventList.Add(noteon);
                            break;
                        case EventTypes.NoteOff:
                            NoteOffEvent noteoff = new NoteOffEvent();
                            noteoff.DeltaTime = deltaTime;
                            // 通道
                            noteoff.Channel = channel;
                            // 音符
                            tempbyte = (byte)stream.ReadByte();
                            noteoff.Note = tempbyte;
                            // 音速
                            tempbyte = (byte)stream.ReadByte();
                            noteoff.Velocity = tempbyte;
                            // 添加到事件列表中
                            theTrack.EventList.Add(noteoff);
                            break;
                        case EventTypes.NoteAftertouch:
                            NoteAftertouchEvent noteaftertouch = new NoteAftertouchEvent();
                            noteaftertouch.DeltaTime = deltaTime;
                            // 通道
                            noteaftertouch.Channel = channel;
                            // 音符
                            tempbyte = (byte)stream.ReadByte();
                            noteaftertouch.Note = tempbyte;
                            // 延音值
                            tempbyte = (byte)stream.ReadByte();
                            noteaftertouch.Amount = tempbyte;
                            // 添加到事件列表中
                            theTrack.EventList.Add(noteaftertouch);
                            break;
                        case EventTypes.Controller:
                            ControllerEvent controller = new ControllerEvent();
                            controller.DeltaTime = deltaTime;
                            // 通道编号
                            controller.Channel = channel;
                            // 控制器编号
                            tempbyte = (byte)stream.ReadByte();
                            controller.Controller = tempbyte;
                            // 控制值
                            tempbyte = (byte)stream.ReadByte();
                            controller.Value = tempbyte;
                            // 添加到事件列表中
                            theTrack.EventList.Add(controller);
                            break;
                        case EventTypes.ProgramChange:
                            ProgramChangeEvent programch = new ProgramChangeEvent();
                            programch.DeltaTime = deltaTime;
                            // 通道
                            programch.Channel = channel;
                            // 乐器编号
                            tempbyte = (byte)stream.ReadByte();
                            programch.Program = tempbyte;
                            // 添加到事件列表中
                            theTrack.EventList.Add(programch);
                            break;
                        case EventTypes.ChannelAftertouch:
                            ChannelAftertouchEvent cnlaftertouch = new ChannelAftertouchEvent();
                            cnlaftertouch.DeltaTime = deltaTime;
                            // 通道
                            cnlaftertouch.Channel = channel;
                            // 通道延音值
                            tempbyte = (byte)stream.ReadByte();
                            cnlaftertouch.Amount = tempbyte;
                            // 添加到事件列表中
                            theTrack.EventList.Add(cnlaftertouch);
                            break;
                        case EventTypes.PitchBend:
                            PitchBendEvent pitchbend = new PitchBendEvent();
                            pitchbend.DeltaTime = deltaTime;
                            // 通道
                            pitchbend.Channel = channel;
                            // LSB 
                            tempbyte = (byte)stream.ReadByte();
                            pitchbend.LSBValue = tempbyte;
                            // MSB
                            tempbyte = (byte)stream.ReadByte();
                            pitchbend.MSBValue = tempbyte;
                            // 加入到事件列表中
                            theTrack.EventList.Add(pitchbend);
                            break;
                    }
                }
            }
        }
        #endregion
    
        #region 写入文件私有方法
        /// <summary>
        /// 将一个轨道中所有事件数据拼接
        /// </summary>
        private byte[] CombinEventdatasForTrack(Track track)
        {
            byte[] data = null;
            using (MemoryStream tstream = new MemoryStream())
            {
                foreach (MidiEventBase mdevent in track.EventList)
                {
                    // 1 - Delta Time
                    byte[] deltatime = ConvertToVariableLength(mdevent.DeltaTime);
                    tstream.Write(deltatime, 0, deltatime.Length);
                    // 2 - 看看是不是元数据
                    if (mdevent.EventType == EventTypes.Meta)
                    {
                        MetaEvent metaev = (MetaEvent)mdevent;
                        // 对于元数据，第一个字节都是 0xFF
                        tstream.WriteByte(metaev.EventType);
                        // 还得看看是哪一种元数据
                        switch (metaev.MetaType)
                        {
                            case MetaEventTypes.SetTempo: //设置节奏
                                SetTempoEvent tempoevt = (SetTempoEvent)metaev;
                                // 第二个字节是元数据类型
                                tstream.WriteByte(tempoevt.MetaType);
                                // 第三个字节表示后面事件数据的长度，三个字节
                                byte[] varlen = ConvertToVariableLength(3);
                                tstream.Write(varlen, 0, varlen.Length);
                                // 最后写入节奏（微秒）
                                byte[] evdata = Convert24BitnumToBEBytes(tempoevt.MicrosecondsPerQuarterNote);
                                tstream.Write(evdata, 0, evdata.Length);
                                break;
                            case MetaEventTypes.TimeSignature: //设置时间标记
                                TimeSignatureEvent timeSingedEvt = (TimeSignatureEvent)metaev;
                                // 第二个字节，元数据类型
                                tstream.WriteByte(timeSingedEvt.MetaType);
                                // 接着是后面数据的长度，四个字节
                                varlen = ConvertToVariableLength(4);
                                tstream.Write(varlen, 0, varlen.Length);
                                // 写入各个值
                                tstream.WriteByte(timeSingedEvt.Numerator);
                                tstream.WriteByte(timeSingedEvt.Denominator);
                                tstream.WriteByte(timeSingedEvt.Metronome);
                                tstream.WriteByte(timeSingedEvt.The32nds);
                                break;
                            case MetaEventTypes.KeySignature: //设置调号
                                KeySignatureEvent keysgnevt = (KeySignatureEvent)metaev;
                                // 第二个字节，元数据类型
                                tstream.WriteByte(keysgnevt.MetaType);
                                // 接着是数据长度
                                varlen = ConvertToVariableLength(2);
                                tstream.Write(varlen, 0, varlen.Length);
                                // 最后是数据内容
                                byte key = ConvertSignedValToSinglebyte(keysgnevt.Key);
                                tstream.WriteByte(key);
                                tstream.WriteByte(keysgnevt.Scale);
                                break;
                            case MetaEventTypes.TrackEnd: // 轨道结束
                                TrackEndEvent endtrack = (TrackEndEvent)metaev;
                                // 第二个字，元数据类型
                                tstream.WriteByte((byte)endtrack.MetaType);
                                // 后面没有数据，长度为0
                                tstream.WriteByte(0);
                                break;
                        }
                    }
                    else
                    {
                        // 通道事件
                        ChannelEvent channelevt = (ChannelEvent)mdevent;
                        // 所有通道事件第一个字节都是事件标识与通道编号组成
                        byte tag = (byte)(channelevt.EventType | channelevt.Channel);
                        tstream.WriteByte(tag);
                        switch (channelevt.EventType)
                        {
                            case EventTypes.NoteOn:
                                NoteOnEvent noteon = (NoteOnEvent)channelevt;
                                // 第二个字节是音符
                                tstream.WriteByte(noteon.Note);
                                // 第三个字节是音速
                                tstream.WriteByte(noteon.Velocity);
                                break;
                            case EventTypes.NoteOff:
                                NoteOffEvent noteoff = (NoteOffEvent)channelevt;
                                // 第二个字节是音符
                                tstream.WriteByte(noteoff.Note);
                                // 第三个字节是音速
                                tstream.WriteByte(noteoff.Velocity);
                                break;
                            case EventTypes.NoteAftertouch:
                                NoteAftertouchEvent noteafter = (NoteAftertouchEvent)channelevt;
                                // 第二个字节是音符
                                tstream.WriteByte(noteafter.Note);
                                // 第三个字节是延音值
                                tstream.WriteByte(noteafter.Amount);
                                break;
                            case EventTypes.ProgramChange:
                                ProgramChangeEvent programchg = (ProgramChangeEvent)channelevt;
                                // 只需要一个字节即可，表示乐器类型
                                tstream.WriteByte(programchg.Program);
                                break;
                            case EventTypes.ChannelAftertouch:
                                ChannelAftertouchEvent cnlaft = (ChannelAftertouchEvent)channelevt;
                                // 只使用一个字节
                                tstream.WriteByte(cnlaft.Amount);
                                break;
                            case EventTypes.PitchBend:
                                PitchBendEvent pitchev = (PitchBendEvent)channelevt;
                                // 第二个字节，低数位
                                tstream.WriteByte(pitchev.LSBValue);
                                // 第三个字节，高数位
                                tstream.WriteByte(pitchev.MSBValue);
                                break;
                            case EventTypes.Controller:
                                ControllerEvent ctrlev = (ControllerEvent)channelevt;
                                // 第二个字节是控制器
                                tstream.WriteByte(ctrlev.Controller);
                                // 第三个字节是控制值
                                tstream.WriteByte(ctrlev.Value);
                                break;
                        }
                    }
                }

                data = tstream.ToArray();
            }

            return data;
        }
        #endregion
    }
}
