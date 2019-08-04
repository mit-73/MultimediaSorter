using MultimediaSorter.Properties;
using MultimediaSorter.ViewModels.Base;

namespace MultimediaSorter.ViewModels
{
    public class MoveFilesViewModel : ViewModelBase
    {
        public MoveFilesViewModel()
        {
            _moveFiles = Settings.Default.MoveFiles;
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
    }
}