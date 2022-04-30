using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Shared.Models;

namespace ZoDream.Shared.Players
{
    public class SpeekPlayer : IPlayer<string>
    {

        private SpeechSynthesizer? Driver;

        public byte Volume { get; set; }

        private string[] channelItems = Array.Empty<string>();

        public string[] ChannelItems => channelItems;

        public bool IsReady { get; private set; } = false;

        public void Play(string key)
        {
            Play((byte)0, key);
        }

        public void Play(byte channel, string key)
        {
            if (Driver == null)
            {
                return;
            }
            Driver.SelectVoice(channelItems[channel]);
            Driver.Speak(key);
        }


        public async Task PlayAsync(byte channel, string key, uint ms)
        {
            Play(channel, key);
            await Task.Delay((int)ms);
            Stop(channel, key);
        }

        public void Stop(string key)
        {
            Driver?.Pause();
        }

        public void Stop(byte channel, string key)
        {
            Driver?.Pause();
        }

        public void Dispose()
        {
            Driver?.Dispose();
        }

        public Task ReadyAsync()
        {
            return Task.Factory.StartNew(() =>
            {
                Driver = new SpeechSynthesizer();
                channelItems = Driver!.GetInstalledVoices()
                .Select(i => i.VoiceInfo.Name).ToArray();
                IsReady = true;
            });
        }
    }
}
