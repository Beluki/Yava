
// Yava.
// A simple, portable game/rom launcher.


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

        // folders:
        private readonly String foldersFilepath;
        private readonly FoldersFileReader foldersFileReader;

        // remembering selected folders and files:
        private String lastSelectedFolderName;
        private Dictionary<String, FolderFile> folderNameToLastSelectedFile;

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
            DoubleBuffered = true;
            MinimumSize = new Size(640, 480);
            ResizeEnd += OnResizeEnd;
            Text = "Yava Launcher";

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
            splitter.SplitterMoved += OnSplitterMoved;

            Controls.Add(filesListView);
            Controls.Add(splitter);
            Controls.Add(foldersListView);

            // settings:
            this.settingsFilepath = settingsFilepath;

            // folders:
            this.foldersFilepath = foldersFilepath;
            this.foldersFileReader = new FoldersFileReader();

            // remembering selected folders and files:
            this.lastSelectedFolderName = null;
            this.folderNameToLastSelectedFile = new Dictionary<String, FolderFile>();

            // wire events - listviews:
            foldersListView.ItemSelectionChanged += OnFoldersListViewItemSelectionChanged;
            filesListView.ItemSelectionChanged += OnFilesListViewItemSelectionChanged;

            // wire events - keyboard:
            foldersListView.KeyDown += OnFoldersListViewKeyDown;
            filesListView.KeyDown += OnFilesListViewKeyDown;

            // load content:
            LoadContent();
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
        /// Remember the current selected folder.
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
        /// Remember the current folder selected file.
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
                    folderNameToLastSelectedFile[foldername] = file;
                }
                else
                {
                    // note: Dictionary.Remove(key) does nothing when the key is not found:
                    folderNameToLastSelectedFile.Remove(foldername);
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

                if (folderNameToLastSelectedFile.ContainsKey(foldername))
                {
                    FolderFile file = folderNameToLastSelectedFile[foldername];
                    String filepath = file.Path;

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

        ///
        /// Loading folders and files into the listviews
        ///

        /// <summary>
        /// Open the folders file and parse the content
        /// adding each folder to the folders listview.
        /// On errors, show a messagebox with details.
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
        /// On errors, show a messagebox with details.
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
                    foreach (FolderFile file in folder.GetFiles())
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
        public void LoadContent()
        {
            LoadFolders();
            ListViewFoldersSelectLastSelectedFolder();
            
            LoadFiles();
            ListViewFilesSelectLastSelectedFile();
        }

        ///
        /// Events: resizing
        ///

        /// <summary>
        /// When the form is resized, auto-resize the listviews.
        /// </summary>
        private void OnResizeEnd(Object sender, EventArgs e)
        {
            ListViewFoldersResize();
            ListViewFilesResize();
        }

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
            }
        }
    }
}

