using System;
using System.IO;
using System.Linq;
using MultimediaSorter.Properties;
using MultimediaSorter.ViewModels.Base;

namespace MultimediaSorter.ViewModels
{
    public class MaskViewModel : ViewModelBase
    {
        private ValidationFilesViewModel ValidationFilesViewModel { get; set; }
        
        public MaskViewModel()
        {
            ValidationFilesViewModel = new ValidationFilesViewModel();
            
            _dirMask = Settings.Default.DirMask;
            _dirMaskToolTip = Resources.DirMaskToolTip;
        }

        private string _dirMask;

        public string DirMask
        {
            get => _dirMask;
            set
            {
                SetProperty(ref _dirMask, value);
                OnPropertyChanged(nameof(SampleDirName));
                OnPropertyChanged(nameof(ValidationFilesViewModel.IsValid));
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
    }
}