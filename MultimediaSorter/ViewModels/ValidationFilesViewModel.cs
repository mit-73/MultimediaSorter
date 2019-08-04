using System.IO;
using System.Linq;
using MultimediaSorter.ViewModels.Base;

namespace MultimediaSorter.ViewModels
{
    public class ValidationFilesViewModel : ViewModelBase
    {
        private SaveFilesViewModel SaveFilesViewModel { get; set; }
        private GetFilesViewModel GetFilesViewModel { get; set; }
        private MaskViewModel MaskViewModel { get; set; }
        private FilterViewModel FilterViewModel { get; set; }
        
        
        public ValidationFilesViewModel()
        {
            SaveFilesViewModel = new SaveFilesViewModel();
            GetFilesViewModel = new GetFilesViewModel();
            MaskViewModel = new MaskViewModel();
            FilterViewModel = new FilterViewModel();
        }

        public bool IsValid
        {
            get
            {
                return Directory.Exists(GetFilesViewModel.FilePath) && Directory.Exists(SaveFilesViewModel.SavePath) &&
                       !string.IsNullOrWhiteSpace(MaskViewModel.DirMask) &&
                       !MaskViewModel.DirMask.ToCharArray().Any(ch => Path.GetInvalidPathChars().Contains(ch)) &&
                       !string.IsNullOrWhiteSpace(FilterViewModel.ExtensionFilter);
            }
        }
    }
}