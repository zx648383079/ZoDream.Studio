using System;
using System.Collections.Generic;
using System.Windows.Input;
using ZoDream.Shared.Readers;
using ZoDream.Shared.ViewModel;
using ZoDream.Studio.Routes;

namespace ZoDream.Studio.ViewModels
{
    public class AddTrackViewModel: BindableBase
    {
        public AddTrackViewModel()
        {
            StepCommand = new RelayCommand(TapStep);
            TextCommand = new RelayCommand(TapText);
            VideoCommand = new RelayCommand(TapVideo);
            ImageCommand = new RelayCommand(TapImage);
            AudioCommand = new RelayCommand(TapAudio);
            AudioRecordCommand = new RelayCommand(TapAudioRecord);
            SpeakCommand = new RelayCommand(TapSpeak);
            AiSpeakCommand = new RelayCommand(TapAiSpeak);
            ScreenRecordCommand = new RelayCommand(TapScreenRecord);
            PianoCommand = new RelayCommand(TapPiano);
        }


        public Action<bool>? FinishFn { get; set; }

        private int step;

        public int Step {
            get => step;
            set => Set(ref step, value);
        }

        public ICommand StepCommand { get; private set; }
        public ICommand TextCommand { get; private set; }
        public ICommand VideoCommand { get; private set; }
        public ICommand ImageCommand { get; private set; }
        public ICommand PianoCommand { get; private set; }
        public ICommand SpeakCommand { get; private set; }
        public ICommand AiSpeakCommand { get; private set; }
        public ICommand AudioRecordCommand { get; private set; }
        public ICommand ScreenRecordCommand { get; private set; }
        public ICommand AudioCommand { get; private set; }

        private void TapSpeak(object? _)
        {
            ShellManager.GoToAsync("speak");
            FinishFn?.Invoke(true);
        }

        private void TapAiSpeak(object? _)
        {
            ShellManager.GoToAsync("ai/speak");
            FinishFn?.Invoke(true);
        }

        private void TapPiano(object? _)
        {
            ShellManager.GoToAsync("piano");
            FinishFn?.Invoke(true);
        }

        private void TapText(object? _)
        {
            ShellManager.GoToAsync("text");
            FinishFn?.Invoke(true);
        }

        private void TapVideo(object? _)
        {
            var picker = new Microsoft.Win32.OpenFileDialog
            {
                Title = "选择视频文件",
                RestoreDirectory = true,
                Filter = $"视频文件|{FileExtension.FormatFilter(FileExtension.VideoFile)}|所有文件|*.*",
            };
            if (picker.ShowDialog() != true)
            {
                return;
            }
            ShellManager.GoToAsync("video", new Dictionary<string, object>
            {
                {"file", picker.FileName}
            });
            FinishFn?.Invoke(true);
        }

        private void TapAudio(object? _)
        {
            var picker = new Microsoft.Win32.OpenFileDialog
            {
                Title = "选择音频文件",
                RestoreDirectory = true,
                Filter = $"音频文件|{FileExtension.FormatFilter(FileExtension.AudioFile)}|所有文件|*.*",
            };
            if (picker.ShowDialog() != true)
            {
                return;
            }
            ShellManager.GoToAsync("audio", new Dictionary<string, object>
            {
                {"file", picker.FileName}
            });
            FinishFn?.Invoke(true);
        }

        private void TapImage(object? _)
        {
            var picker = new Microsoft.Win32.OpenFileDialog
            {
                Title = "选择图片文件",
                RestoreDirectory = true,
                Filter = $"图片文件|{FileExtension.FormatFilter(FileExtension.ImageFile)}|所有文件|*.*",
            };
            if (picker.ShowDialog() != true)
            {
                return;
            }
            ShellManager.GoToAsync("image", new Dictionary<string, object>
            {
                {"file", picker.FileName}
            });
            FinishFn?.Invoke(true);
        }

        private void TapAudioRecord(object? _)
        {
            ShellManager.GoToAsync("audio/record");
            FinishFn?.Invoke(true);
        }

        private void TapScreenRecord(object? _)
        {
            ShellManager.GoToAsync("screen/record");
            FinishFn?.Invoke(true);
        }

        private void TapStep(object? arg)
        {
            if (arg is int o)
            {
                Step = o;
            } else if (int.TryParse(arg?.ToString(), out var r))
            {
                Step = r;
            } else
            {
                Step = 0;
            }
        }
    }
}
