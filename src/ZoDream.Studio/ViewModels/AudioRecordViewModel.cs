using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ZoDream.Shared.ViewModel;
using ZoDream.Studio.Routes;

namespace ZoDream.Studio.ViewModels
{
    public class AudioRecordViewModel: BindableBase
    {

        public AudioRecordViewModel()
        {
            PlayCommand = new RelayCommand(TapPlay);
            StopCommand = new RelayCommand(TapStop);
            BackCommand = new RelayCommand(TapBack);
            ConfirmCommand = new RelayCommand(TapConfirm);
            RecordCommand = new RelayCommand(TapRecord);
        }

        /// <summary>
        /// 录制状态，0 停止 1 录制中 2 播放中
        /// </summary>
        private int recordStatus;

        public int RecordStatus {
            get => recordStatus;
            set => Set(ref recordStatus, value);
        }


        public ICommand PlayCommand { get; private set; }
        public ICommand RecordCommand { get; private set; }
        public ICommand StopCommand { get; private set; }
        public ICommand BackCommand { get; private set; }
        public ICommand ConfirmCommand { get; private set; }

        private void TapBack(object? _)
        {
            ShellManager.GoBackAsync();
        }

        private void TapConfirm(object? _)
        {

        }

        private void TapPlay(object? _)
        {

        }

        private void TapRecord(object? _)
        {

        }
        private void TapStop(object? _)
        {

        }
    }
}
