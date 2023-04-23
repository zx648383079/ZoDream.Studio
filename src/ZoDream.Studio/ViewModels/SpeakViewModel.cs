using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using ZoDream.Shared.Models;
using ZoDream.Shared.ViewModel;
using ZoDream.Studio.Pages;
using ZoDream.Studio.Routes;

namespace ZoDream.Studio.ViewModels
{
    public class SpeakViewModel: BindableBase
    {
        public SpeakViewModel()
        {
            PlayCommand = new RelayCommand(TapPlay);
            StopCommand = new RelayCommand(TapStop);
            BackCommand = new RelayCommand(TapBack);
            ConfirmCommand = new RelayCommand(TapConfirm);
            AddCommand = new RelayCommand(TapAdd);
        }

        private bool paused = true;

        public bool Paused {
            get => paused;
            set => Set(ref paused, value);
        }

        private ObservableCollection<TextPromptItem> promptItems = new();

        public ObservableCollection<TextPromptItem> PromptItems {
            get => promptItems;
            set => Set(ref promptItems, value);
        }



        public ICommand PlayCommand { get; private set; }
        public ICommand StopCommand { get; private set; }
        public ICommand BackCommand { get; private set; }
        public ICommand ConfirmCommand { get; private set; }
        public ICommand AddCommand { get; private set; }

        private void TapBack(object? _)
        {
            ShellManager.GoBackAsync();
        }

        private void TapAdd(object? arg)
        {
            var offset = 0;
            var page = new AddTextDialog();
            if (arg is int o)
            {
                page.ViewModel.Volume = o;
            } else if (arg is Point p) {
                page.ViewModel.Volume = (int)p.Y;
                offset = (int)p.X;
            }
            if (page.ShowDialog() != true)
            {
                return;
            }
            PromptItems.Add(new TextPromptItem()
            {
                Content = page.ViewModel.Content,
                Volume = page.ViewModel.Volume,
                Duration = page.ViewModel.Duration,
                Offset = offset
            });
        }

        private void TapConfirm(object? _)
        {

        }

        private void TapPlay(object? _)
        {
            using var synth = new SpeechSynthesizer();

            // Configure the audio output.   
            synth.SetOutputToDefaultAudioDevice();

            // Create a PromptBuilder object and add content.  
            var style = new PromptBuilder();
            style.AppendText("Your order for");
            style.StartStyle(new PromptStyle(PromptRate.Slow));
            style.AppendText("one kitchen sink and one faucet");
            style.EndStyle();
            style.AppendText("has been confirmed.");

            // Speak the contents of the SSML prompt.  
            synth.Speak(style);
        }
        private void TapStop(object? _)
        {

        }
    }
}
