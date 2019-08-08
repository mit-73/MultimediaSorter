using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shell;
using System.Windows.Threading;
using Microsoft.Win32;
using Ookii.Dialogs.Wpf;
using MultimediaSorter.Properties;
using MultimediaSorter.ViewModels.Base;
using MultimediaSorter.ViewModels.Commands;

namespace MultimediaSorter.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel()
        {
            _filePath = Settings.Default.FilePath;
            _savePath = Settings.Default.SavePath;
            _moveFiles = Settings.Default.MoveFiles;
            _extensionFilter = Settings.Default.ExtensionFilter;
            _dirMask = Settings.Default.DirMask;
            _dirMaskToolTip = Resources.DirMaskToolTip;

            _processedFiles = new ObservableCollection<string>();
            ProcessNotStarted = true;
        }

        #region Propertys

        private string _filePath;

        public string FilePath
        {
            get => _filePath;
            set
            {
                SetProperty(ref _filePath, value);
                OnPropertyChanged(nameof(IsValid));
                Settings.Default.FilePath = value;
                Settings.Default.Save();
            }
        }

        private string _savePath;

        public string SavePath
        {
            get => _savePath;
            set
            {
                SetProperty(ref _savePath, value);
                OnPropertyChanged(nameof(IsValid));
                Settings.Default.SavePath = value;
                Settings.Default.Save();
            }
        }

        private double _fileCount;

        public double FileCount
        {
            get => _fileCount;
            set => SetProperty(ref _fileCount, value);
        }

        private bool _moveFiles;

        public bool MoveFiles
        {
            get => _moveFiles;
            set
            {
                SetProperty(ref _moveFiles, value);
                Settings.Default.MoveFiles = value;
                Settings.Default.Save();
            }
        }

        private string _extensionFilter;

        public string ExtensionFilter
        {
            get => _extensionFilter;
            set
            {
                SetProperty(ref _extensionFilter, value);
                Settings.Default.ExtensionFilter = value;
                Settings.Default.Save();
            }
        }

        private bool _searchInSubFolder;

        public bool SearchInSubFolder
        {
            get => _searchInSubFolder;
            set
            {
                SetProperty(ref _searchInSubFolder, value);
                Settings.Default.SearchInSubFolder = value;
                Settings.Default.Save();
            }
        }

        private string _dirMask;

        public string DirMask
        {
            get => _dirMask;
            set
            {
                SetProperty(ref _dirMask, value);
                OnPropertyChanged(nameof(SampleDirName));
                OnPropertyChanged(nameof(IsValid));
                Settings.Default.DirMask = value;
                Settings.Default.Save();
            }
        }

        private string _dirMaskToolTip;

        public string DirMaskToolTip
        {
            get => _dirMaskToolTip;
            set => SetProperty(ref _dirMaskToolTip, value);
        }

        private double _progressValue;

        public double ProgressValue
        {
            get => _progressValue;
            set
            {
                SetProperty(ref _progressValue, value);
                OnPropertyChanged(nameof(ProgressPrecent));
            }
        }

        private bool _processStarted;

        public bool ProcessStarted
        {
            get => _processStarted;
            private set => SetProperty(ref _processStarted, value);
        }

        private bool _processNotStarted;

        public bool ProcessNotStarted
        {
            get => _processNotStarted;
            private set => SetProperty(ref _processNotStarted, value);
        }

        private TaskbarItemProgressState _progressState;

        public TaskbarItemProgressState ProgressState
        {
            get => _progressState;
            private set => SetProperty(ref _progressState, value);
        }

        private bool _needStop;

        public bool NeedStop
        {
            get => _needStop;
            set => SetProperty(ref _needStop, value);
        }

        private ObservableCollection<string> _processedFiles;

        public ObservableCollection<string> ProcessedFiles => _processedFiles;

        public double ProgressPrecent => ProgressValue.Equals(0) ? ProgressValue / 100 : 0;

        public string SampleDirName
        {
            get
            {
                try
                {
                    if (DirMask.ToCharArray().Any(ch => Path.GetInvalidPathChars().Contains(ch)))
                        throw new FormatException("Содержаться некорректные символы");
                    return $"Пример: {DateTime.Now.ToString(_dirMask)}";
                }
                catch
                {
                    return Resources.NotCorrectMask;
                }
            }
        }

        public bool IsValid
        {
            get
            {
                return Directory.Exists(FilePath) && Directory.Exists(SavePath) &&
                       !string.IsNullOrWhiteSpace(DirMask) &&
                       !DirMask.ToCharArray().Any(ch => Path.GetInvalidPathChars().Contains(ch)) &&
                       !string.IsNullOrWhiteSpace(ExtensionFilter);
            }
        }

        #endregion

        #region Commands

        public RelayCommand SelectFilePathCommand
        {
            get { return new RelayCommand(o => SelectFilePath(), o => !ProcessStarted); }
        }

        public RelayCommand SelectSavePathCommand
        {
            get { return new RelayCommand(o => SelectSavePath(), o => !ProcessStarted); }
        }

        public RelayCommand StartProcessingCommand
        {
            get { return new RelayCommand(o => StartProcessing(), o => !ProcessStarted && IsValid); }
        }

        public RelayCommand StopProcessingCommand
        {
            get { return new RelayCommand(o => StopProcessing(), o => ProcessStarted); }
        }

        #endregion

        #region Methods

        public void SelectFilePath()
        {
            var openFileDialog = new VistaFolderBrowserDialog();
            openFileDialog.Description = Resources.SelectFilePath;
            openFileDialog.SelectedPath = FilePath;
            openFileDialog.ShowNewFolderButton = true;
            var result = openFileDialog.ShowDialog();
            if (result == false) return;
            FilePath = openFileDialog.SelectedPath;
        }

        public void SelectSavePath()
        {
            var saveFileDialog = new VistaFolderBrowserDialog();
            saveFileDialog.Description = Resources.SelectSaveFilePath;
            saveFileDialog.SelectedPath = SavePath;
            saveFileDialog.ShowNewFolderButton = true;
            var result = saveFileDialog.ShowDialog();
            if (result == false) return;
            SavePath = saveFileDialog.SelectedPath;
        }

        public Task StartSortingAsync()
        {
            return Task.Factory.StartNew(RunSorting, TaskCreationOptions.LongRunning);
        }

        private void RunSorting()
        {
            NeedStop = false;
            var dirInfo = new DirectoryInfo(FilePath);
            var files = new List<FileInfo>();
            var extensions = ExtensionFilter.Split(new[]
            {
                ';'
            }, StringSplitOptions.RemoveEmptyEntries);
            if (!extensions.Any())
                return;
            Array.ForEach(extensions,
                ext => files.AddRange(dirInfo.GetFiles(ext,
                    SearchInSubFolder ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly)));
            FileCount = files.Count;
            for (var i = 0; i < files.Count; i++)
            {
                if (NeedStop)
                    break;
                try
                {
                    var path = Path.Combine(SavePath, GetFolderName(files[i].FullName));
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);
                    if (MoveFiles)
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

        public string GetFolderName(string filePath)
        {
            using (var fileStream = File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                try
                {
                    fileStream.Flush();
                    fileStream.Close();
                    var fileInfo = new FileInfo(filePath);

                    if (fileInfo.CreationTime >= fileInfo.LastWriteTime)
                    {
                        return fileInfo.LastWriteTime.ToString(DirMask);
                    }
                    else if (fileInfo.CreationTime <= fileInfo.LastWriteTime)
                    {
                        return fileInfo.CreationTime.ToString(DirMask);
                    }
                    else return fileInfo.CreationTime.ToString(DirMask);
                }
                catch
                {
                    fileStream.Flush();
                    fileStream.Close();
                    var fileInfo = new FileInfo(filePath);
                    var fileCreationTime = fileInfo.CreationTime.ToString(DirMask);
                    var fileWriteTime = fileInfo.LastWriteTime.ToString(DirMask);

                    if (fileCreationTime == fileWriteTime)
                    {
                        return fileCreationTime;
                    }
                    else return fileWriteTime;
                }
            }
        }

        private void StopProcessing()
        {
            NeedStop = true;
        }

        private async void StartProcessing()
        {
            ProcessedFiles.Clear();
            ProgressState = TaskbarItemProgressState.Normal;
            OnPropertyChanged(nameof(TaskbarItemProgressState));
            ProcessStarted = true;
            ProcessNotStarted = false;
            await StartSortingAsync();
            ProgressState = TaskbarItemProgressState.None;
            ProgressValue = 0;
            FileCount = 0;
            ProcessStarted = false;
            ProcessNotStarted = true;
            MessageBox.Show(Resources.SortComplite, Application.Current.MainWindow?.Title, MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        public void ReportProgress(int progressPrecent, string fileName)
        {
            ProgressValue = progressPrecent / FileCount * 100d;
            Application.Current.Dispatcher.Invoke(() => ProcessedFiles.Add(fileName),
                DispatcherPriority.DataBind);
        }

        #endregion
    }
}