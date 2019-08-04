namespace MultimediaSorter.Services
{
    public interface IFileDialogService
    {
        void ShowMessage(string message, string caption);
        string FilePath { get; set; }
        bool OpenFileDialog();
        bool SaveFileDialog();
        string Open(string filename);
        void Save(string filename, string saveList);
    }
}
