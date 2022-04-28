using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Shared.Models;

namespace ZoDream.Shared.Players
{
    public class AudioPlayer : IPlayer
    {
        public string[] ChannelItems => throw new NotImplementedException();

        public byte Volume { get; set; }

        public bool IsReady { get; private set; } = false;

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void Play(PianoKey key)
        {
            throw new NotImplementedException();
        }

        public void Play(byte channel, PianoKey key)
        {
            throw new NotImplementedException();
        }

        public async Task PlayAsync(byte channel, PianoKey key, uint ms)
        {
            Play(channel, key);
            await Task.Delay((int)ms);
            Stop(channel, key);
        }



        public void Stop(PianoKey key)
        {
            throw new NotImplementedException();
        }

        public void Stop(byte channel, PianoKey key)
        {
            throw new NotImplementedException();
        }

        public Task ReadyAsync()
        {
            throw new NotImplementedException();
        }
    }
}
