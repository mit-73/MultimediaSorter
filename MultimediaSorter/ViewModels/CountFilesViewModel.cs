using MultimediaSorter.ViewModels.Base;

namespace MultimediaSorter.ViewModels
{
    public class CountFilesViewModel : ViewModelBase
    {
        public CountFilesViewModel()
        {
        }
        
        private double _fileCount;

        public double FileCount
        {
            get => _fileCount;
            set => SetProperty(ref _fileCount, value);
        }
    }
}