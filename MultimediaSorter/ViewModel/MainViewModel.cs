using System;
using System.IO;
using System.Linq;
using MultimediaSorter.Properties;

namespace MultimediaSorter.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private string _filePath;
        public string FilePath
        {
            get => _filePath;
            private set
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
            private set
            {
                SetProperty(ref _savePath, value);
                OnPropertyChanged(nameof(IsValid));
                Settings.Default.SavePath = value;
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
        
        private string _extensionFilter;
        public string ExtensionFilter
        {
            get { return _extensionFilter; }
            set
            {
                _extensionFilter = value;
                OnPropertyChanged();
                Settings.Default.ExtensionFilter = value;
                Settings.Default.Save();
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
    }
}
