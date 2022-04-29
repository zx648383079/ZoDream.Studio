using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Shared.Converters;
using ZoDream.Shared.Models;
using ZoDream.Shared.Storage;

namespace ZoDream.Shared.Players
{
    public class AudioPlayer : IPlayer
    {
        private List<AudioSource> channelItems = new();
        public string[] ChannelItems => channelItems.Select(i => i.Name).ToArray();

        public byte Volume { get; set; }

        public bool IsReady { get; private set; } = false;

        public void Dispose()
        {
            
        }

        public void Play(PianoKey key)
        {
            
        }

        public void Play(byte channel, PianoKey key)
        {
            
        }

        public async Task PlayAsync(byte channel, PianoKey key, uint ms)
        {
            Play(channel, key);
            await Task.Delay((int)ms);
            Stop(channel, key);
        }



        public void Stop(PianoKey key)
        {
            
        }

        public void Stop(byte channel, PianoKey key)
        {
            
        }

        public async Task ReadyAsync()
        {
            await LoadFileAsync("");
        }


        private string FindKey(AudioSource source, PianoKey key, out uint begin, out uint end)
        {
            var k = key.ToKey127();
            foreach (var item in source.Items)
            {
                if (item.BeginKey is null)
                {
                    continue;
                }
                if (item.Endkey is null)
                {
                    if (key == item.BeginKey)
                    {
                        begin = item.Position;
                        end = item.End;
                        return source.Path + item.Name;
                    }
                    continue;
                }
                if (k >= item.BeginKey.ToKey127() && k <= item.Endkey.ToKey127())
                {
                    begin = item.Position;
                    end = item.End;
                    return source.Path + item.Name;
                }
            }
            if (source.BeginKey is null)
            {
                begin = 0;
                end = 0;
                return string.Empty;
            }
            var offset = k - source.BeginKey.ToKey127();
            if (source.StepKey > 1)
            {
                offset /= (int)source.StepKey;
            }
            begin = Convert.ToUInt32(source.Position + offset * (source.Duration + source.Gap));
            end = begin + source.Duration;
            return source.Path;
        }

        private async Task LoadFileAsync(string file)
        {
            if (string.IsNullOrWhiteSpace(file))
            {
                return;
            }
            var res = await LocationStorage.ReadAsync(file);
            if (string.IsNullOrWhiteSpace(res))
            {
                return;
            }
            var item = JsonConvert.DeserializeObject<AudioSource>(res, new PianoKeyConverter());
            if (item == null)
            {
                return;
            }
            channelItems.Add(item);
        }
    }
}
