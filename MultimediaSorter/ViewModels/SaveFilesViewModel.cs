using Microsoft.Win32;
using MultimediaSorter.Properties;
using MultimediaSorter.ViewModels.Base;
using MultimediaSorter.ViewModels.Commands;

namespace MultimediaSorter.ViewModels
{
    public class SaveFilesViewModel : ViewModelBase
    {
        private ValidationFilesViewModel ValidationFilesViewModel { get; set; }
        private ProcessingViewModel ProcessingViewModel { get; set; }
        
        public SaveFilesViewModel()
        {
            ValidationFilesViewModel = new ValidationFilesViewModel();
            ProcessingViewModel = new ProcessingViewModel();
            
            _savePath = Settings.Default.SavePath;
        }

        public RelayCommand SelectSavePathCommand
        {
            get { return new RelayCommand(o => SelectSavePath(), o => !ProcessingViewModel.ProcessStarted); }
        }

        private string _savePath;

        public string SavePath
        {
            get => _savePath;
            set
            {
                SetProperty(ref _savePath, value);
                OnPropertyChanged(nameof(ValidationFilesViewModel.IsValid));
                Settings.Default.SavePath = value;
                Settings.Default.Save();
            }
        }

        public void SelectSavePath()
        {
            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = Resources.SelectSaveFilePath;
            saveFileDialog.FileName = SavePath;
            var result = saveFileDialog.ShowDialog();
            if (result == false) return;
            SavePath = saveFileDialog.FileName;
        }
    }
}