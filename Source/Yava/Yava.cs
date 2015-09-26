
// Yava.
// A simple, portable game/emulator launcher.


using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using Yava.Controls;
using Yava.FoldersFile;


namespace Yava
{
    [System.ComponentModel.DesignerCategory("")]
    internal class Yava : Form
    {
        // gui components:
        private readonly DoubleBufferedListView foldersListView;
        private readonly DoubleBufferedListView filesListView;

        // settings:
        private readonly String settingsFilepath;
        private readonly YavaSettings settings;

        // folders:
        private readonly String foldersFilepath;
        private readonly FoldersFileReader foldersFileReader;

        // remembering selected folders and files:
        private String lastSelectedFolderName;
        private Dictionary<String, String> folderNameToLastSelectedFilePath;

        /// <summary>
        /// Yava implementation.
        /// </summary>
        /// <param name="settingsFilepath">
        /// Path to the settings file to use.
        /// </param>
        /// <param name="foldersFilepath">
        /// Path to the folders file to use.
        /// </param>
        public Yava(String settingsFilepath, String foldersFilepath)
        {
            // this form:
            this.DoubleBuffered = true;
            this.Icon = Util.ResourceAsIcon("Yava.Resources.gnome-joystick.ico");
            this.MinimumSize = new Size(640, 480);
            this.Text = "Yava Launcher";

            // gui components:
            foldersListView = new DoubleBufferedListView();
            foldersListView.Columns.Add("Folders");
            foldersListView.Dock = DockStyle.Left;
            foldersListView.Font = new Font("Verdana", 9);
            foldersListView.HideSelection = false;
            foldersListView.MultiSelect = true;
            foldersListView.FullRowSelect = true;
            foldersListView.ShowItemToolTips = true;
            foldersListView.View = View.Details;
            foldersListView.Width = 200;

            filesListView = new DoubleBufferedListView();
            filesListView.Columns.Add("Files");
            filesListView.Dock = DockStyle.Fill;
            filesListView.Font = new Font("Verdana", 9);
            filesListView.HideSelection = false;
            filesListView.FullRowSelect = true;
            filesListView.MultiSelect = false;
            filesListView.ShowItemToolTips = true;
            filesListView.View = View.Details;
            filesListView.Width = 400;

            Splitter splitter = new Splitter();
            splitter.Dock = DockStyle.Left;

            Controls.Add(filesListView);
            Controls.Add(splitter);
            Controls.Add(foldersListView);

            // settings:
            this.settingsFilepath = settingsFilepath;
            this.settings = SettingsLoad();

            // folders:
            this.foldersFilepath = foldersFilepath;
            this.foldersFileReader = new FoldersFileReader();

            // remembering selected folders and files:
            this.lastSelectedFolderName = null;
            this.folderNameToLastSelectedFilePath = new Dictionary<String, String>();

            // apply settings before wiring events:
            this.Width = settings.LastYavaFormWidth;
            this.Height = settings.LastYavaFormHeight;
            this.foldersListView.Width = settings.LastFoldersListViewWidth;
            this.filesListView.Width = settings.LastFilesListViewWidth;
            this.lastSelectedFolderName = settings.LastSelectedFolderName;
            this.folderNameToLastSelectedFilePath = settings.FolderNameToLastSelectedFilePath;

            // wire events - splitter:
            splitter.SplitterMoved += OnSplitterMoved;

            // wire events - listviews:
            foldersListView.ItemSelectionChanged += OnFoldersListViewItemSelectionChanged;
            filesListView.ItemSelectionChanged += OnFilesListViewItemSelectionChanged;

            // wire events - keyboard:
            foldersListView.KeyDown += OnFoldersListViewKeyDown;
            filesListView.KeyDown += OnFilesListViewKeyDown;

            // wire events - mouse:
            filesListView.MouseDoubleClick += OnFilesListViewMouseDoubleClick;

            // wire events - form:
            this.FormClosing += OnFormClosing;
            this.ResizeEnd += OnResizeEnd;

            // load content:
            LoadContent();
        }

        ///
        /// Settings file
        ///

        /// <summary>
        /// Load the settings from our settings filepath if it exists.
        /// Return default settings otherwise.
        /// </summary>
        private YavaSettings SettingsLoad()
        {
            try
            {
                if (File.Exists(settingsFilepath))
                {
                    return (YavaSettings) Util.Deserialize(settingsFilepath);
                }
            }
            catch (Exception exception)
            {
                String text = String.Format(
                    "Unable to load settings: \n" +
                    "{0} \n\n" +
                    "This usually means that the file is corrupt, empty \n" +
                    "or incompatible with the current Yava version. \n\n" +
                    "Exception message: \n" +
                    "{1} \n",
                    settingsFilepath,
                    exception.Message
                );

                String caption = "Error loading settings file";
                MessageBox.Show(text, caption);
            }

            // unable to load or doesn't exist, use defaults:
            return new YavaSettings();
        }

        /// <summary>
        /// Save the current settings.
        /// </summary>
        private void SettingsSave()
        {
            try
            {
                Util.Serialize(settings, settingsFilepath);
            }
            catch (Exception exception)
            {
                String text = String.Format(
                    "Unable to save settings: \n" +
                    "{0} \n\n" +
                    "Exception message: \n" +
                    "{1} \n",
                    settingsFilepath,
                    exception.Message
                );

                String caption = "Error saving settings file";
                MessageBox.Show(text, caption);
            }
        }

        ///
        /// Updating listviews
        ///

        /// <summary>
        /// Resize the folder ListView first column according to the content.
        /// </summary>
        private void ListViewFoldersResize()
        {
            foldersListView.BeginUpdate();
            foldersListView.Columns[0].AutoResize(ColumnHeaderAutoResizeStyle.None);
            foldersListView.Columns[0].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
            foldersListView.Columns[0].Width = Math.Max(foldersListView.Columns[0].Width, 150);
            foldersListView.EndUpdate();
        }

        /// <summary>
        /// Resize the files ListView first column according to the content.
        /// </summary>
        private void ListViewFilesResize()
        {
            filesListView.BeginUpdate();
            filesListView.Columns[0].AutoResize(ColumnHeaderAutoResizeStyle.None);
            filesListView.Columns[0].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
            filesListView.Columns[0].Width = Math.Max(filesListView.Columns[0].Width, 150);
            filesListView.EndUpdate();
        }

        ///
        /// Remembering selected items
        ///

        /// <summary>
        /// Remember the currently selected folder name.
        /// Used in combination with ListViewFoldersSelectLastSelectedFolder()
        /// to set the selected folder after reloading the listview.
        /// </summary>
        private void ListViewFoldersRememberSelectedFolder()
        {
            // we remember the last selected folder
            // but not for multiple selections:
            if (foldersListView.SelectedItems.Count == 1)
            {
                Folder folder = foldersListView.SelectedItems[0].Tag as Folder;
                String foldername = folder.Name;

                lastSelectedFolderName = foldername;
            }
            // no selection or multiple selections:
            else
            {
                lastSelectedFolderName = null;
            }
        }

        /// <summary>
        /// Try to select the folder that was last selected on the folders listview.
        /// </summary>
        private void ListViewFoldersSelectLastSelectedFolder()
        {
            if (lastSelectedFolderName != null)
            {
                // find the item:
                foreach (ListViewItem item in foldersListView.Items)
                {
                    Folder test = item.Tag as Folder;
                    if (test.Name.Equals(lastSelectedFolderName))
                    {
                        // do not trigger the selection event:
                        foldersListView.ItemSelectionChanged -= OnFoldersListViewItemSelectionChanged;
                        {
                            foldersListView.EnsureVisible(item.Index);
                            item.Focused = true;
                            item.Selected = true;
                        }
                        foldersListView.ItemSelectionChanged += OnFoldersListViewItemSelectionChanged;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Remember the current folder selected file path.
        /// Used in combination with ListViewFilesSelectLastSelectedFile()
        /// to set the selected file after reloading the listview.
        /// </summary>
        private void ListViewFilesRememberSelectedFile()
        {
            // we remember the last selected file for each single folder
            // but not for multiple selections:
            if (foldersListView.SelectedItems.Count == 1)
            {
                Folder folder = foldersListView.SelectedItems[0].Tag as Folder;
                String foldername = folder.Name;

                if (filesListView.SelectedItems.Count == 1)
                {
                    FolderFile file = filesListView.SelectedItems[0].Tag as FolderFile;
                    String filepath = file.Path;

                    folderNameToLastSelectedFilePath[foldername] = filepath;
                }
                else
                {
                    // note: Dictionary.Remove(key) does nothing when the key is not found:
                    folderNameToLastSelectedFilePath.Remove(foldername);
                }
            }
        }

        /// <summary>
        /// Try to select the file that was last selected on the files listview.
        /// </summary>
        private void ListViewFilesSelectLastSelectedFile()
        {
            // we remember the last selected file for each single folder
            // but not for multiple selections:
            if (foldersListView.SelectedItems.Count == 1)
            {
                Folder folder = foldersListView.SelectedItems[0].Tag as Folder;
                String foldername = folder.Name;

                if (folderNameToLastSelectedFilePath.ContainsKey(foldername))
                {
                    String filepath = folderNameToLastSelectedFilePath[foldername];

                    // find the item:
                    foreach (ListViewItem item in filesListView.Items)
                    {
                        FolderFile test = item.Tag as FolderFile;
                        if (test.Path.Equals(filepath))
                        {
                            // do not trigger the selection event:
                            filesListView.ItemSelectionChanged -= OnFilesListViewItemSelectionChanged;
                            {
                                filesListView.EnsureVisible(item.Index);
                                item.Focused = true;
                                item.Selected = true;
                            }
                            filesListView.ItemSelectionChanged += OnFilesListViewItemSelectionChanged;
                            break;
                        }
                    }
                }
            }
        }

        ///
        /// Executing files
        ///

        /// <summary>
        /// Open the folders file with the default program
        /// associated to the extension.
        /// </summary>
        private void FoldersFileOpen()
        {
            try
            {
                Process.Start(foldersFilepath);
            }
            catch (Exception exception)
            {
                String text = exception.Message;
                String caption = "Error openning folders file";
                MessageBox.Show(text, caption);
            }
        }

        /// <summary>
        /// Execute a file from the files listview.
        /// </summary>
        /// <param name="file">File to execute.</param>
        private void ListViewFilesExecuteFile(FolderFile file)
        {
            Folder folder = file.Folder;

            // step 1: fill the startup information:
            ProcessStartInfo psi = null;

            // initial values as specified in the folder options:
            String executable = folder.Executable;
            String parameters = folder.Parameters;
            String workingdirectory = folder.WorkingDirectory;

            try
            {
                // variables to expand as absolute paths:
                String FILEPATH = Path.GetFullPath(file.Path);
                String FOLDERPATH = Path.GetFullPath(folder.Path);

                // executable:
                executable = executable.Replace("%FILEPATH%", FILEPATH);
                executable = executable.Replace("%FOLDERPATH%", FOLDERPATH);
                executable = Path.GetFullPath(executable);

                // parameters:
                // when none specified, use the file path:
                parameters = parameters ?? '"' + "%FILEPATH%" + '"';
                parameters = parameters.Replace("%FILEPATH%", FILEPATH);
                parameters = parameters.Replace("%FOLDERPATH%", FOLDERPATH);

                // working directory:
                // when none specified, use the executable folder:
                workingdirectory = workingdirectory ?? Path.GetDirectoryName(executable);
                workingdirectory = workingdirectory.Replace("%FILEPATH%", FILEPATH);
                workingdirectory = workingdirectory.Replace("%FOLDERPATH%", FOLDERPATH);
                workingdirectory = Path.GetFullPath(workingdirectory);

                // everything ok:
                psi = new ProcessStartInfo();
                psi.FileName = executable;
                psi.Arguments = parameters;
                psi.WorkingDirectory = workingdirectory;
            }

            // path error (from Path.GetFullPath)
            // show the exception message:
            catch (Exception exception)
            {
                String text = String.Format(
                    "Executable: \n" +
                    "{0} \n\n" +
                    "Parameters: \n" +
                    "{1} \n\n" +
                    "Working Directory: \n" +
                    "{2} \n\n" +
                    "Exception message: \n" +
                    "{3}",
                    executable,
                    parameters ?? "<unspecified> (use file path)",
                    workingdirectory ?? "<unspecified> (use executable folder)",
                    exception.Message
                );

                String caption = "Error setting up process";
                MessageBox.Show(text, caption, MessageBoxButtons.OK);
            }

            // unable to setup, bail out:
            if (psi == null)
            {
                return;
            }

            // step 2: run the process:
            try
            {
                Process process = Process.Start(psi);

                // this check is needed because Process.Start(...) returns null
                // when reusing a process:
                if (process != null)
                {
                    process.PriorityBoostEnabled = true;
                    process.PriorityClass = ProcessPriorityClass.AboveNormal;
                }
            }

            // process error (from Process.Start)
            // show the exception message:
            catch (Exception exception)
            {
                String text = String.Format(
                    "Executable: \n" +
                    "{0} \n\n" +
                    "Parameters: \n" +
                    "{1} \n\n" +
                    "Working Directory: \n" +
                    "{2} \n\n" +
                    "Exception message: \n" +
                    "{3}",
                    psi.FileName,
                    psi.Arguments,
                    psi.WorkingDirectory,
                    exception.Message
                );

                String caption = "Error executing file";
                MessageBox.Show(text, caption);
            }
        }

        /// <summary>
        /// Run the currently selected file on the files listview.
        /// </summary>
        private void ListViewFilesExecuteSelectedFile()
        {
            if (filesListView.SelectedItems.Count == 1)
            {
                FolderFile file = filesListView.SelectedItems[0].Tag as FolderFile;
                ListViewFilesExecuteFile(file);
            }
        }

        ///
        /// Loading folders and files into the listviews
        ///

        /// <summary>
        /// Open the folders file and parse the content
        /// adding each folder to the folders listview.
        /// On errors, show a MessageBox with details.
        /// </summary>
        private void LoadFolders()
        {
            List<Folder> folders = new List<Folder>();
            Boolean success = false;

            try
            {
                foldersFileReader.Read(foldersFilepath, folders);
                success = true;
            }

            // syntax or parsing error
            // show details and ask the user to edit:
            catch (FoldersFileReadError exception)
            {
                String text = String.Format(
                    "{0} \n" +
                    "Error at line {1}: {2} \n\n" +
                    "{3} \n\n" +
                    "Edit the file and press F5 to refresh. \n" +
                    "Do you want to open the folders file now?",
                    exception.FilePath,
                    exception.LineNumber,
                    exception.Message,
                    exception.Line
                );

                String caption = "Error reading folders file";
                if (Util.MessageBoxYesNo(text, caption))
                {
                    FoldersFileOpen();
                }
            }

            // io error
            // show the exception message:
            catch (Exception exception)
            {
                String caption = "Error opening folders file";
                String text = exception.Message;

                MessageBox.Show(text, caption, MessageBoxButtons.OK);
            }

            // clear previous content:
            foldersListView.BeginUpdate();
            foldersListView.Items.Clear();

            // add the folders when needed:
            if (success)
            {
                ListViewItem[] items = new ListViewItem[folders.Count];

                Int32 index = 0;
                foreach (Folder folder in folders)
                {
                    ListViewItem item = new ListViewItem();

                    item.Tag = folder;
                    item.Text = folder.Name;
                    item.ToolTipText = folder.Path;

                    items[index] = item;
                    index++;
                }

                foldersListView.Items.AddRange(items);
            }

            // done:
            foldersListView.EndUpdate();
            ListViewFoldersResize();
        }

        /// <summary>
        /// Populate the files listview using the selected folders files.
        /// On errors, show a MessageBox with details.
        /// </summary>
        private void LoadFiles()
        {
            List<ListViewItem> items = new List<ListViewItem>();
            Boolean success = false;

            try
            {
                foreach (ListViewItem selectedItem in foldersListView.SelectedItems)
                {
                    Folder folder = selectedItem.Tag as Folder;
                    foreach (FolderFile file in folder.EnumerateFiles())
                    {
                        ListViewItem item = new ListViewItem();

                        item.Tag = file;
                        item.Text = Path.GetFileName(file.Path);
                        item.ToolTipText = file.Path;

                        items.Add(item);
                    }
                }

                success = true;
            }

            // io error
            // show the exception message:
            catch (Exception exception)
            {
                String caption = "Error loading folder files";
                String text = exception.Message;

                MessageBox.Show(text, caption, MessageBoxButtons.OK);
            }

            // clear previous content:
            filesListView.BeginUpdate();
            filesListView.Items.Clear();

            // add the files when needed:
            if (success)
            {
                filesListView.Items.AddRange(items.ToArray());
            }

            // done:
            filesListView.EndUpdate();
            ListViewFilesResize();
        }

        /// <summary>
        /// Loads both folders and files, selecting the last
        /// selected ones when possible.
        /// </summary>
        private void LoadContent()
        {
            LoadFolders();
            ListViewFoldersSelectLastSelectedFolder();

            LoadFiles();
            ListViewFilesSelectLastSelectedFile();
        }

        ///
        /// Events: splitter
        ///

        /// <summary>
        /// When the spliiter is moved, auto-resize the listviews.
        /// </summary>
        private void OnSplitterMoved(Object sender, EventArgs e)
        {
            ListViewFoldersResize();
            ListViewFilesResize();
        }

        ///
        /// Events: listviews
        ///

        /// <summary>
        /// When the folder selection changes, load the appropriate files list
        /// and try to select the last known selected file.
        /// </summary>
        private void OnFoldersListViewItemSelectionChanged(Object sender, ListViewItemSelectionChangedEventArgs e)
        {
            LoadFiles();
            ListViewFilesSelectLastSelectedFile();
            ListViewFoldersRememberSelectedFolder();
        }

        /// <summary>
        /// When the file selection changes
        /// remember the new one as the last selected file.
        /// </summary>
        private void OnFilesListViewItemSelectionChanged(Object sender, ListViewItemSelectionChangedEventArgs e)
        {
            ListViewFilesRememberSelectedFile();
        }

        ///
        /// Events: keyboard
        ///

        /// <summary>
        /// Folders listview keyboard shortcuts.
        /// </summary>
        private void OnFoldersListViewKeyDown(Object sender, KeyEventArgs e)
        {
            switch (e.KeyData)
            {
                case Keys.F5:
                    LoadContent();
                    foldersListView.Focus();
                    e.Handled = true;
                    break;
            }
        }

        /// <summary>
        /// Files listview keyboard shortcuts.
        /// </summary>
        private void OnFilesListViewKeyDown(Object sender, KeyEventArgs e)
        {
            switch (e.KeyData)
            {
                case Keys.F5:
                    LoadContent();
                    filesListView.Focus();
                    e.Handled = true;
                    break;
                case Keys.Return:
                    ListViewFilesExecuteSelectedFile();
                    break;
            }
        }

        ///
        /// Events: mouse
        ///

        /// <summary>
        /// On double click, run the currently selected file.
        /// </summary>
        private void OnFilesListViewMouseDoubleClick(Object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ListViewFilesExecuteSelectedFile();
            }
        }

        ///
        /// Events: form
        ///

        /// <summary>
        /// When the form is closed, save settings.
        /// </summary>
        private void OnFormClosing(Object sender, FormClosingEventArgs e)
        {
            settings.LastYavaFormWidth = this.Width;
            settings.LastYavaFormHeight = this.Height;
            settings.LastFoldersListViewWidth = foldersListView.Width;
            settings.LastFilesListViewWidth = filesListView.Width;
            settings.LastSelectedFolderName = this.lastSelectedFolderName;
            settings.FolderNameToLastSelectedFilePath = this.folderNameToLastSelectedFilePath;

            SettingsSave();
        }

        /// <summary>
        /// When the form is resized, auto-resize the listviews.
        /// </summary>
        private void OnResizeEnd(Object sender, EventArgs e)
        {
            ListViewFoldersResize();
            ListViewFilesResize();
        }
    }
}

