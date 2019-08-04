using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Shell;
using System.Windows.Threading;
using MultimediaSorter.Properties;
using MultimediaSorter.ViewModels.Base;
using MultimediaSorter.ViewModels.Commands;

namespace MultimediaSorter.ViewModels
{
    public class ProcessingViewModel : ViewModelBase
    {
        private ValidationFilesViewModel ValidationFilesViewModel { get; set; }
        private SortingViewModel SortingViewModel { get; set; }
        private  CountFilesViewModel CountFilesViewModel { get; set; }
        
        public ProcessingViewModel()
        {
            ValidationFilesViewModel = new ValidationFilesViewModel();
            SortingViewModel = new SortingViewModel();
            CountFilesViewModel = new CountFilesViewModel();
            
            ProcessedFiles = new ObservableCollection<string>();
            ProcessNotStarted = true;
        }

        public RelayCommand StartProcessingCommand
        {
            get { return new RelayCommand(o => StartProcessing(), o => !ProcessStarted && ValidationFilesViewModel.IsValid); }
        }

        public RelayCommand StopProcessingCommand
        {
            get { return new RelayCommand(o => StopProcessing(), o => ProcessStarted); }
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

        private async void StartProcessing()
        {
            ProcessedFiles.Clear();
            ProgressState = TaskbarItemProgressState.Normal;
            OnPropertyChanged(nameof(TaskbarItemProgressState));
            ProcessStarted = true;
            ProcessNotStarted = false;
            await SortingViewModel.StartSortingAsync();
            ProgressState = TaskbarItemProgressState.None;
            ProgressValue = 0;
            CountFilesViewModel.FileCount = 0;
            ProcessStarted = false;
            ProcessNotStarted = true;
            MessageBox.Show(Resources.SortComplite, Application.Current.MainWindow?.Title, MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        public void ReportProgress(int progressPrecent, string fileName)
        {
            ProgressValue = progressPrecent / CountFilesViewModel.FileCount * 100d;
            Application.Current.Dispatcher.Invoke(() => ProcessedFiles.Add(fileName),
                DispatcherPriority.DataBind);
        }
    }
}