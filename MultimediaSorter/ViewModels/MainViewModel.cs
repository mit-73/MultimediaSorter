using System.Collections.ObjectModel;
using MultimediaSorter.ViewModels.Base;

namespace MultimediaSorter.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel()
        {
            _childrens = new ObservableCollection<object>();
            
            _childrens.Add(new CountFilesViewModel()); //0
            _childrens.Add(new FilterViewModel()); //1
            _childrens.Add(new GetFilesViewModel()); //2
            _childrens.Add(new MaskViewModel()); //3
            _childrens.Add(new MoveFilesViewModel()); //4
            _childrens.Add(new ProcessingViewModel()); //5
            _childrens.Add(new SaveFilesViewModel()); //6
            _childrens.Add(new SearchFilesViewModel()); //7
            _childrens.Add(new SortingViewModel()); //8
            _childrens.Add(new ValidationFilesViewModel()); //9
        }
        
        private ObservableCollection<object> _childrens;
        public ObservableCollection<object> Childrens => _childrens;
    }
}
