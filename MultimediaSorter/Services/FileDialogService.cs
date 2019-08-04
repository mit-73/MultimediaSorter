using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using MultimediaSorter.Services;
using Microsoft.Win32;

namespace MultimediaSorter.Services
{
    public class FileDialogService : IFileDialogService
    {
        public string FilePath { get; set; }
        
        public string Open(string filename)
        {
            string text;
            using (FileStream fs = new FileStream(filename, FileMode.OpenOrCreate))
            {
                text = File.ReadAllText(filename);
            }
 
            return text;
        }
        
        public void Save(string filename, string saveList)
        {
            using (FileStream fs = new FileStream(filename, FileMode.Create))
            {
                File.WriteAllText(filename, saveList);
            }
        }

        public bool OpenFileDialog()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                FilePath = openFileDialog.FileName;
                return true;
            }

            return false;
        }

        public bool SaveFileDialog()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            if (saveFileDialog.ShowDialog() == true)
            {
                FilePath = saveFileDialog.FileName;
                return true;
            }

            return false;
        }

        public void ShowMessage(string message, string caption)
        {
            MessageBox.Show(message, caption);
        }
    }
}
