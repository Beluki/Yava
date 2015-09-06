
// Yava.
// A simple, portable game/rom launcher.


using System;
using System.Drawing;
using System.Windows.Forms;

using Yava.Controls;


namespace Yava
{
    [System.ComponentModel.DesignerCategory("")]
    internal class Yava : Form
    {
        // gui components:
        private readonly DoubleBufferedListView foldersListView;
        private readonly DoubleBufferedListView filesListView;

        /// <summary>
        /// Yava implementation.
        /// </summary>
        public Yava()
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
            foldersListView.Font = new Font("Verdana", 10);
            foldersListView.FullRowSelect = true;
            foldersListView.View = View.Details;
            foldersListView.Width = 200;

            filesListView = new DoubleBufferedListView();
            filesListView.Columns.Add("Files");
            filesListView.Dock = DockStyle.Fill;
            filesListView.Font = new Font("Verdana", 10);
            filesListView.FullRowSelect = true;
            filesListView.View = View.Details;
            filesListView.Width = 400;

            Splitter splitter = new Splitter();
            splitter.Dock = DockStyle.Left;
            splitter.SplitterMoved += OnSplitterMoved;

            Controls.Add(filesListView);
            Controls.Add(splitter);
            Controls.Add(foldersListView);

            ResizeListViewColumns();
        }

        ///
        /// Helper functions to use in events:
        ///

        /// <summary>
        /// Resize the files and folder ListView first column according
        /// to their content.
        /// </summary>
        private void ResizeListViewColumns()
        {
            foldersListView.BeginUpdate();
            filesListView.BeginUpdate();

            // resize:
            foldersListView.Columns[0].AutoResize(ColumnHeaderAutoResizeStyle.None);
            foldersListView.Columns[0].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);

            filesListView.Columns[0].AutoResize(ColumnHeaderAutoResizeStyle.None);
            filesListView.Columns[0].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);

            // ensure that the column headers are visible:
            foldersListView.Columns[0].Width = Math.Max(foldersListView.Columns[0].Width, 150);
            filesListView.Columns[0].Width = Math.Max(filesListView.Columns[0].Width, 150);

            foldersListView.EndUpdate();
            filesListView.EndUpdate();
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

