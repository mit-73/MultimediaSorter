using Microsoft.Win32;
using MultimediaSorter.Properties;
using MultimediaSorter.ViewModels.Base;
using MultimediaSorter.ViewModels.Commands;

namespace MultimediaSorter.ViewModels
{
    public class GetFilesViewModel : ViewModelBase
    {
        private ValidationFilesViewModel ValidationFilesViewModel { get; set; }
        private ProcessingViewModel ProcessingViewModel { get; set; }
        
        public GetFilesViewModel()
        {
            ValidationFilesViewModel = new ValidationFilesViewModel();
            ProcessingViewModel = new ProcessingViewModel();
            
            _filePath = Settings.Default.FilePath;
        }

        public RelayCommand SelectFilePathCommand
        {
            get { return new RelayCommand(o => SelectFilePath(), o => !ProcessingViewModel.ProcessStarted); }
        }

        private string _filePath;

        public string FilePath
        {
            get => _filePath;
            set
            {
                SetProperty(ref _filePath, value);
                OnPropertyChanged(nameof(ValidationFilesViewModel.IsValid));
                Settings.Default.FilePath = value;
                Settings.Default.Save();
            }
        }

        public void SelectFilePath()
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Title = Resources.SelectFilePath;
            openFileDialog.FileName = FilePath;
            var result = openFileDialog.ShowDialog();
            if (result == false) return;
            FilePath = openFileDialog.FileName;
        }
    }
}