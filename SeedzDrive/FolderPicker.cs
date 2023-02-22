//using System;
//using System.IO;
//using System.Threading.Tasks;
//using System.Windows.Forms;

////using WindowsFolderPicker = Windows.Storage.Pickers.FolderPicker;

//namespace SeedzDrive;

//public class FolderPicker : IFolderPicker
//{
//    public async Task<string> PickFolder()
//    {
//        var dialog = new FolderBrowserDialog
//        {
//            Description = "Time to select a folder",
//            UseDescriptionForTitle = true,
//            SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory)
//                           + Path.DirectorySeparatorChar,
//            ShowNewFolderButton = true
//        };

//        return dialog.ShowDialog() == DialogResult.OK ? dialog.SelectedPath : Preferences.Default.Folder;
//    }
//}