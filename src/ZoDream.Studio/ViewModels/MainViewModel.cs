using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ZoDream.Shared.Models;
using ZoDream.Shared.Storage;
using ZoDream.Shared.ViewModel;
using ZoDream.Studio.Routes;

namespace ZoDream.Studio.ViewModels
{
    public class MainViewModel: BindableBase
    {
        public MainViewModel()
        {
            _ = LoadAsync();
        }

        public AppOption Option { get; private set; } = new();


        public async Task LoadAsync()
        {
            var option = await AppData.LoadAsync<AppOption>();
            if (option != null)
            {
                Option = option;
            }
            ShellManager.GoToAsync("startup");
        }

        public async Task LoadProjectAsync(string fileName)
        {
            var data = await AppData.LoadAsync<ProjectItem>(fileName);
            if (data is null)
            {
                MessageBox.Show("文件不存在，或文件错误");
                return;
            }
            ShellManager.GoToAsync("workspace", new Dictionary<string, object>
            {
                {"file", fileName },
                {"project", data }
            });
        }
    }
}
