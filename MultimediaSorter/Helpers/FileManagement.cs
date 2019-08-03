using System.IO;
using Microsoft.Win32;
using MultimediaSorter.Properties;
using MultimediaSorter.ViewModel;

namespace MultimediaSorter.Helpers
{
    public class FileManagement
    {
        private MainViewModel _mainViewModel;

        public string GetFolderName(string filePath)
        {
            using (var fileStream = File.Open(filePath, FileMode.Open, FileAccess.Read))
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
                    fileStream.Flush();
                    fileStream.Close();
                    var fileInfo = new FileInfo(filePath);

                    if (fileInfo.CreationTime >= fileInfo.LastWriteTime)
                    {
                        return fileInfo.LastWriteTime.ToString(_mainViewModel.DirMask);
                    }
                    else if (fileInfo.CreationTime <= fileInfo.LastWriteTime)
                    {
                        return fileInfo.CreationTime.ToString(_mainViewModel.DirMask);
                    }
                    else return fileInfo.CreationTime.ToString(_mainViewModel.DirMask);
                }
                catch
                {
                    fileStream.Flush();
                    fileStream.Close();
                    var fileInfo = new FileInfo(filePath);
                    var fileCreationTime = fileInfo.CreationTime.ToString(_mainViewModel.DirMask);
                    var fileWriteTime = fileInfo.LastWriteTime.ToString(_mainViewModel.DirMask);

                    if (fileCreationTime == fileWriteTime)
                    {
                        return fileCreationTime;
                    }
                    else return fileWriteTime;
                }
            }
        }

        public void SelectFilePath()
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Title = Resources.SelectFilePath;
            openFileDialog.FileName = _mainViewModel.FilePath;
            var result = openFileDialog.ShowDialog();
            if (result == false) return;
            _mainViewModel.FilePath = openFileDialog.FileName;
        }
        
        public void SelectSavePath()
        {
            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = Resources.SelectSaveFilePath;
            saveFileDialog.FileName = _mainViewModel.SavePath;
            var result = saveFileDialog.ShowDialog();
            if (result == false) return;
            _mainViewModel.SavePath = saveFileDialog.FileName;
        }
    }
}
