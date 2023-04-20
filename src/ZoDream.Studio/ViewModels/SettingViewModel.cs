using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ZoDream.Shared.Models;
using ZoDream.Shared.ViewModel;
using ZoDream.Studio.Routes;

namespace ZoDream.Studio.ViewModels
{
    public class SettingViewModel: BindableBase
    {

        public SettingViewModel()
        {
            BackCommand = new RelayCommand(TapBack);
            ConfirmCommand = new RelayCommand(TapConfirm);
            StepCommand = new RelayCommand(TapStep);
        }

        private int step;

        public int Step {
            get => step;
            set => Set(ref step, value);
        }

        private ObservableCollection<DataItem> breadCrumbItems = new();

        public ObservableCollection<DataItem> BreadCrumbItems {
            get => breadCrumbItems;
            set => Set(ref breadCrumbItems, value);
        }

        public ICommand BackCommand { get; private set; }
        public ICommand ConfirmCommand { get; private set; }
        public ICommand StepCommand { get; private set; }


        private void TapBack(object? _)
        {
            if (Step > 0)
            {
                Step = 0;
                AddStep(Step);
                return;
            }
            ShellManager.GoBackAsync();
        }

        private void TapConfirm(object? _)
        {

        }

        private void TapStep(object? arg)
        {
            Step = ConvertToInt(arg);
            AddStep(Step);
        }

        private int ConvertToInt(object? arg)
        {
            if (arg is DataItem item)
            {
                arg = item.Value;
            }
            if (arg is int o)
            {
                return o;
            }
            else if (int.TryParse(arg?.ToString(), out var r))
            {
                return r;
            }
            else
            {
                return 0;
            }
        }

        private void AddStep(int val)
        {
            if (val < 1)
            {
                BreadCrumbItems.Clear();
                return;
            }
            for (var i = 0; i < BreadCrumbItems.Count; i++)
            {
                if (val == ConvertToInt(BreadCrumbItems[i].Value))
                {
                    RemoveEnd(i + 1);
                    return;
                }
            }
            BreadCrumbItems.Add(new DataItem(val switch
            {
                1 => "项目设置",
                2 => "音源管理",
                3 => "插件管理",
                4 => "系统设置",
                9 => "关于",
                _ => string.Empty,
            }, val));
        }

        private void RemoveEnd(int start)
        {
            for (int i = BreadCrumbItems.Count - 1; i >= start; i--)
            {
                BreadCrumbItems.RemoveAt(i);
            }
        }
    }
}
