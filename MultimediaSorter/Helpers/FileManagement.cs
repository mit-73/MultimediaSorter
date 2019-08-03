using System.IO;
using System.Windows.Forms;
using System.Windows.Interop;
using MultimediaSorter.Properties;
using MultimediaSorter.ViewModel;
using PhotoSorter;
using Application = System.Windows.Application;

namespace MultimediaSorter.Helpers
{
    public class FileManagement
    {
        private MainViewModel _mainViewModel = new MainViewModel();
        
        public string GetFolderName(string filePath)
        {
            using (var photo = File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                try
                {
                    /*
                    var decoder = BitmapDecoder.Create(photo, BitmapCreateOptions.IgnoreColorProfile, BitmapCacheOption.Default);
                    var bitmapMetadata = (BitmapMetadata)decoder.Frames[0].Metadata;
                    if (bitmapMetadata != null)
                    {
                        var dt = Convert.ToDateTime(bitmapMetadata.DateTaken);
                        photo.Flush();
                        photo.Close();
                        return dt.ToString(_dirMask);
                    }
                    */
                    photo.Flush();
                    photo.Close();
                    var fi = new FileInfo(filePath);

                    if (fi.CreationTime >= fi.LastWriteTime)
                    {
                        return fi.LastWriteTime.ToString(_mainViewModel.DirMask);
                    }
                    else if (fi.CreationTime <= fi.LastWriteTime)
                    {
                        return fi.CreationTime.ToString(_mainViewModel.DirMask);
                    }
                    else return fi.CreationTime.ToString(_mainViewModel.DirMask);
                }
                catch
                {
                    photo.Flush();
                    photo.Close();
                    var fi = new FileInfo(filePath);
                    var fi1 = fi.CreationTime.ToString(_mainViewModel.DirMask);
                    var fi2 = fi.LastWriteTime.ToString(_mainViewModel.DirMask);

                    if (fi1 == fi2)
                    {
                        return fi1;
                    }
                    else return fi2;
                }
            }
        }

        private void SelectFilePath()
        {
            using (var dlg = new FolderBrowserDialog())
            {
                dlg.Description = Resources.SelectFilePath;
                dlg.ShowNewFolderButton = true;
                dlg.SelectedPath = _mainViewModel.FilePath;
                var oldWindow = new OldWindow(new WindowInteropHelper(Application.Current.MainWindow).Handle);
                if (dlg.ShowDialog(oldWindow) != DialogResult.OK)
                    return;
                _mainViewModel.FilePath = dlg.SelectedPath;
            }
        }

        private void SelectSavePath()
        {
            using (var dlg = new FolderBrowserDialog())
            {
                dlg.Description = Resources.SelectSaveFilePath;
                dlg.ShowNewFolderButton = true;
                dlg.SelectedPath = _mainViewModel.SavePath;
                var oldWindow = new OldWindow(new WindowInteropHelper(Application.Current.MainWindow).Handle);
                if (dlg.ShowDialog(oldWindow) != DialogResult.OK)
                    return;
                _mainViewModel.SavePath = dlg.SelectedPath;
            }
        }
    }
}
