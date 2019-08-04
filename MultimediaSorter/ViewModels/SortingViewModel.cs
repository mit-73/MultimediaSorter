using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MultimediaSorter.ViewModels.Base;

namespace MultimediaSorter.ViewModels
{
    public class SortingViewModel : ViewModelBase
    {
        private SaveFilesViewModel SaveFilesViewModel { get; set; }
        private GetFilesViewModel GetFilesViewModel { get; set; }
        private FilterViewModel FilterViewModel { get; set; }
        private ProcessingViewModel ProcessingViewModel { get; set; }
        private  MoveFilesViewModel MoveFilesViewModel { get; set; }
        private SearchFilesViewModel SearchFilesViewModel { get; set; }
        private CountFilesViewModel CountFilesViewModel { get; set; }
        private MaskViewModel MaskViewModel { get; set; }
        
        public SortingViewModel()
        {
            SaveFilesViewModel = new SaveFilesViewModel();
            GetFilesViewModel = new GetFilesViewModel();
            FilterViewModel = new FilterViewModel();
            ProcessingViewModel = new ProcessingViewModel();
            MoveFilesViewModel = new MoveFilesViewModel();
            SearchFilesViewModel = new SearchFilesViewModel();
            CountFilesViewModel = new CountFilesViewModel();
            MaskViewModel = new MaskViewModel();
            
        }

        public Task StartSortingAsync()
        {
            return Task.Factory.StartNew(RunSorting, TaskCreationOptions.LongRunning);
        }

        private void RunSorting()
        {
            ProcessingViewModel.NeedStop = false;
            var dirInfo = new DirectoryInfo(GetFilesViewModel.FilePath);
            var files = new List<FileInfo>();
            var extensions = FilterViewModel.ExtensionFilter.Split(new[]
            {
                ';'
            }, StringSplitOptions.RemoveEmptyEntries);
            if (!extensions.Any())
                return;
            Array.ForEach(extensions,
                ext => files.AddRange(dirInfo.GetFiles(ext,
                    SearchFilesViewModel.SearchInSubFolder ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly)));
            CountFilesViewModel.FileCount = files.Count;
            for (var i = 0; i < files.Count; i++)
            {
                if (ProcessingViewModel.NeedStop)
                    break;
                try
                {
                    var path = Path.Combine(SaveFilesViewModel.SavePath, MaskViewModel.GetFolderName(files[i].FullName));
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);
                    if (MoveFilesViewModel.MoveFiles)
                        File.Move(files[i].FullName, Path.Combine(path, files[i].Name));
                    else
                        File.Copy(files[i].FullName, Path.Combine(path, files[i].Name), false);
                }
                catch (Exception ex)
                {
                    ProcessingViewModel.ReportProgress(i + 1, ex.Message);
                    continue;
                }

                ProcessingViewModel.ReportProgress(i + 1, files[i].Name);
            }
        }
    }
}