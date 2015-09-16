
// Yava.
// A simple, portable game/emulator launcher.


using System;
using System.Windows.Forms;


namespace Yava.Controls
{
    [System.ComponentModel.DesignerCategory("")]
    internal class DoubleBufferedListView : ListView
    {
        public DoubleBufferedListView()
        {
            DoubleBuffered = true;
        }
    }
}

