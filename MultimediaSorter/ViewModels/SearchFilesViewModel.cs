using MultimediaSorter.Properties;
using MultimediaSorter.ViewModels.Base;

namespace MultimediaSorter.ViewModels
{
    public class SearchFilesViewModel : ViewModelBase
    {
        public SearchFilesViewModel()
        {
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
    }
}