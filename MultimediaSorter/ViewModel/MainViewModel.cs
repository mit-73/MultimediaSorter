using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Shell;
using MultimediaSorter.Commands;
using MultimediaSorter.Helpers;
using MultimediaSorter.Properties;

namespace MultimediaSorter.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        //TODO Refactoring
        
        private HandlesEvent _handlesEvent;
        private FileManagement _fileManagement;

        public MainViewModel()
        {
            ProcessedFiles = new ObservableCollection<string>();
            
            _filePath = Settings.Default.FilePath;
            _dirMask = Settings.Default.DirMask;
            _savePath = Settings.Default.SavePath;
            _moveFiles = Settings.Default.MoveFiles;
            _extensionFilter = Settings.Default.ExtensionFilter;
            
            _dirMaskToolTip = Resources.DirMaskToolTip;

            ProcessNotStarted = true;
        }

        public RelayCommand SelectFilePathCommand
        {
            get { return new RelayCommand(o => _fileManagement.SelectFilePath(), o => !ProcessStarted); }
        }

        public RelayCommand SelectSavePathCommand
        {
            get { return new RelayCommand(o => _fileManagement.SelectSavePath(), o => !ProcessStarted); }
        }

        public RelayCommand StartProcessingCommand
        {
            get { return new RelayCommand(o => StartProcessing(), o => !ProcessStarted && IsValid); }
        }

        public RelayCommand StopProcessingCommand
        {
            get { return new RelayCommand(o => StopProcessing(), o => ProcessStarted); }
        }

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
            set
            {
                SetProperty(ref _dirMaskToolTip, value);
            }
        }
        
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
                return Directory.Exists(FilePath) &&
                       Directory.Exists(SavePath) &&
                       !string.IsNullOrWhiteSpace(DirMask) &&
                       !DirMask.ToCharArray().Any(ch => Path.GetInvalidPathChars().Contains(ch)) &&
                       !string.IsNullOrWhiteSpace(ExtensionFilter);
            }
        }

        public ObservableCollection<string> ProcessedFiles { get; }

        public double ProgressPrecent => ProgressValue != 0 ? ProgressValue / 100 : 0;

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

        private double _fileCount;

        public double FileCount
        {
            get => _fileCount;
            set => SetProperty(ref _fileCount, value);
        }

        private async void StartProcessing()
        {
            ProcessedFiles.Clear();
            ProgressState = TaskbarItemProgressState.Normal;
            OnPropertyChanged(nameof(TaskbarItemProgressState));
            ProcessStarted = true;
            ProcessNotStarted = false;
            await _handlesEvent.StartSortingAsync();
            ProgressState = TaskbarItemProgressState.None;
            ProgressValue = 0;
            FileCount = 0;
            ProcessStarted = false;
            ProcessNotStarted = true;
            MessageBox.Show(Resources.SortComplite, Application.Current.MainWindow?.Title, MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private bool _needStop;

        public bool NeedStop
        {
            get => _needStop;
            set => SetProperty(ref _needStop, value);
        }

        private void StopProcessing()
        {
            NeedStop = true;
        }
    }
}
