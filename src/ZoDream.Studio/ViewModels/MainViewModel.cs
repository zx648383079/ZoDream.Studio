﻿using FFMpegCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ZoDream.Shared.Models;
using ZoDream.Shared.Readers;
using ZoDream.Shared.Storage;
using ZoDream.Shared.ViewModel;
using ZoDream.Studio.Routes;

namespace ZoDream.Studio.ViewModels
{
    public class MainViewModel: BindableBase, IDisposable
    {
        public MainViewModel()
        {
            _ = LoadAsync();
        }

        public const string PickerFilter = "项目文件|*.json|所有文件|*.*";

        public ProjectReader Reader { get; private set; } = new();
        public AppOption Option { get; private set; } = new();

        public string ProjectFileName { get; set; } = string.Empty;

        public ProjectItem? Project { get; set; }

        private PreviewProjectViewModel? previewView;

        public PreviewProjectViewModel PreviewView {
            get {
                previewView ??= new PreviewProjectViewModel();
                return previewView;
            }
            set { previewView = value; }
        }



        public async Task LoadAsync()
        {
            var option = await AppData.LoadAsync<AppOption>();
            if (option != null)
            {
                Option = option;
            }
            ResetOption();
            ShellManager.GoToAsync("startup");
        }


        public string TempFolder => string.IsNullOrWhiteSpace(Option.TempFolder) ?
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "temp")
                : Option.TempFolder;

        public void ResetOption()
        {
            GlobalFFOptions.Configure(arg => {
                if (!string.IsNullOrWhiteSpace(Option.BinFolder))
                {
                    arg.BinaryFolder = Option.BinFolder;
                }
                arg.TemporaryFilesFolder = TempFolder;
                if (Directory.Exists(arg.TemporaryFilesFolder))
                {
                    Directory.CreateDirectory(arg.TemporaryFilesFolder);
                }
            });
        }

        public async Task SaveAsync()
        {
            ResetOption();
            await AppData.SaveAsync(Option);
        }

        public async Task LoadProjectAsync(string fileName)
        {
            var data = await Reader.ReadAsync(fileName);
            if (data is null)
            {
                MessageBox.Show("文件不存在，或文件错误");
                return;
            }
            Enter(fileName, data);
            ShellManager.GoToAsync("workspace", new Dictionary<string, object>
            {
                {"file", fileName },
                {"project", data }
            });
        }

        public async Task SaveProjectAsync()
        {
            if (string.IsNullOrWhiteSpace(ProjectFileName))
            {
                var picker = new Microsoft.Win32.SaveFileDialog
                {
                    RestoreDirectory = true,
                    Filter = PickerFilter
                };
                if (picker.ShowDialog() != true)
                {
                    return;
                }
                ProjectFileName = picker.FileName;
            }
            await Reader.WriteAsync(ProjectFileName, Project!);
        }

        public void Enter(ProjectItem project)
        {
            Enter(string.Empty, project);
        }

        public void Enter(string fileName)
        {
            _ = LoadProjectAsync(fileName);
        }

        public void Enter(string fileName, ProjectItem project)
        {
            ProjectFileName = fileName;
            Project = project;
        }

        public void Dispose()
        {
            PreviewView.Dispose();
        }
    }
}
