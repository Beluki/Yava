
// Yava.
// A simple, portable game/rom launcher.


using System;
using System.Collections.Generic;
using System.Drawing;
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
            Text = "Yava launcher";

            // gui components:
            foldersListView = new DoubleBufferedListView();
            foldersListView.Columns.Add("Folders");
            foldersListView.Dock = DockStyle.Left;
            foldersListView.Font = new Font("Verdana", 9);
            foldersListView.FullRowSelect = true;
            foldersListView.ShowItemToolTips = true;
            foldersListView.View = View.Details;
            foldersListView.Width = 200;

            filesListView = new DoubleBufferedListView();
            filesListView.Columns.Add("Files");
            filesListView.Dock = DockStyle.Fill;
            filesListView.Font = new Font("Verdana", 9);
            filesListView.FullRowSelect = true;
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

            LoadFolders();
            ResizeListViewColumns();
        }

        ///
        /// Helper listview functions:
        ///

        /// <summary>
        /// Resize the files and folder ListView first column according
        /// to their content.
        /// </summary>
        private void ResizeListViewColumns()
        {
            foldersListView.Columns[0].AutoResize(ColumnHeaderAutoResizeStyle.None);
            foldersListView.Columns[0].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);

            filesListView.Columns[0].AutoResize(ColumnHeaderAutoResizeStyle.None);
            filesListView.Columns[0].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);

            // ensure that the column headers are visible:
            foldersListView.Columns[0].Width = Math.Max(foldersListView.Columns[0].Width, 150);
            filesListView.Columns[0].Width = Math.Max(filesListView.Columns[0].Width, 150);
        }

        /// <summary>
        /// Clear and disable both the folders and the files listviews.
        /// </summary>
        private void DisableListViews()
        {
            foldersListView.Items.Clear();
            filesListView.Items.Clear();

            foldersListView.Enabled = false;
            filesListView.Enabled = false;
        }

        ///
        /// Loading content:
        ///

        /// <summary>
        /// Open the folders file and parse the content
        /// adding each folder to the folders listview.
        /// </summary>
        private Boolean LoadFolders()
        {
            Boolean success = false;
            List<Folder> folders = new List<Folder>();

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

                }
            }
            // io error:
            catch (Exception exception)
            {
                String caption = "Error opening folders file";
                String text = exception.Message;

                MessageBox.Show(text, caption, MessageBoxButtons.OK);
            }

            // clear previous content and update the listview:
            foldersListView.BeginUpdate();
            foldersListView.Items.Clear();

            // ok? add all the folders:
            if (success)
            {
                foreach (Folder folder in folders)
                {
                    ListViewItem item = new ListViewItem();

                    item.Tag = folder;
                    item.Text = folder.Name;
                    item.ToolTipText = folder.Path;

                    foldersListView.Items.Add(item);
                }
            }

            foldersListView.EndUpdate();
            return success;
        }

        ///
        /// Events: resizing
        ///

        /// <summary>
        /// When the form is resized, auto-resize the ListViews column.
        /// </summary>
        private void OnResizeEnd(Object sender, EventArgs e)
        {
            ResizeListViewColumns();
        }

        /// <summary>
        /// When the spliiter is moved, auto-resize the ListViews column.
        /// </summary>
        private void OnSplitterMoved(Object sender, EventArgs e)
        {
            ResizeListViewColumns();
        }
    }
}

