using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using MultimediaSorter.ViewModel;

namespace MultimediaSorter.Helpers
{
    public class HandlesEvent
    {
        private MainViewModel _mainViewModel = new MainViewModel();
        FileManagement _getFolder = new FileManagement();

        public Task StartSortingAsync()
        {
            return Task.Factory.StartNew(RunSorting, TaskCreationOptions.LongRunning);
        }

        private void RunSorting()
        {
            _mainViewModel.NeedStop = false;
            var dirInfo = new DirectoryInfo(_mainViewModel.FilePath);
            var files = new List<FileInfo>();
            var extensions = _mainViewModel.ExtensionFilter.Split(new[]
            {
                ';'
            }, StringSplitOptions.RemoveEmptyEntries);
            if (!extensions.Any())
                return;
            Array.ForEach(extensions, ext => files.AddRange(dirInfo.GetFiles(ext, _mainViewModel.SearchInSubFolder ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly)));
            _mainViewModel.FileCount = files.Count;
            for (var i = 0; i < files.Count; i++)
            {
                if (_mainViewModel.NeedStop)
                    break;
                try
                {
                    var path = Path.Combine(_mainViewModel.SavePath, _getFolder.GetFolderName(files[i].FullName));
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);
                    if (_mainViewModel.MoveFiles)
                        File.Move(files[i].FullName, Path.Combine(path, files[i].Name));
                    else
                        File.Copy(files[i].FullName, Path.Combine(path, files[i].Name), false);
                }
                catch (Exception ex)
                {
                    ReportProgress(i + 1, ex.Message);
                    continue;
                }
                ReportProgress(i + 1, files[i].Name);
            }
        }


        private void ReportProgress(int progressPrecent, string fileName)
        {
            _mainViewModel.ProgressValue = progressPrecent / _mainViewModel.FileCount * 100d;
            Application.Current.Dispatcher.Invoke(() => _mainViewModel.ProcessedFiles.Add(fileName), DispatcherPriority.DataBind);
        }
    }
}
