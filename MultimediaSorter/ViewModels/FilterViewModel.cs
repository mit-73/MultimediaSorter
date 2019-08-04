using MultimediaSorter.Properties;
using MultimediaSorter.ViewModels.Base;

namespace MultimediaSorter.ViewModels
{
    public class FilterViewModel : ViewModelBase
    {
        public FilterViewModel()
        {
            _extensionFilter = Settings.Default.ExtensionFilter;
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
    }
}