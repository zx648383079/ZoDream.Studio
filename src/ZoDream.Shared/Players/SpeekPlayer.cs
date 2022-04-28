using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Shared.Models;

namespace ZoDream.Shared.Players
{
    public class SpeekPlayer : IPlayer
    {

        private SpeechSynthesizer? Driver;

        public byte Volume { get; set; }

        private string[] channelItems = Array.Empty<string>();

        public string[] ChannelItems => channelItems;

        public bool IsReady { get; private set; } = false;

        public void Play(PianoKey key)
        {
            Play((byte)0, key);
        }

        public void Play(byte channel, PianoKey key)
        {
            if (Driver == null)
            {
                return;
            }
            Driver.SelectVoice(channelItems[channel]);
            Driver.Speak(key.Code.ToString());
        }

        public void Play(MidiChannel channel, PianoKey key)
        {
            Play((byte)channel, key);
        }

        public async Task PlayAsync(byte channel, PianoKey key, uint ms)
        {
            Play(channel, key);
            await Task.Delay((int)ms);
            Stop(channel, key);
        }

        public void Stop(PianoKey key)
        {
            Driver?.Pause();
        }

        public void Stop(byte channel, PianoKey key)
        {
            Driver?.Pause();
        }

        public void Stop(MidiChannel channel, PianoKey key)
        {
            Stop((byte)channel, key);
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
