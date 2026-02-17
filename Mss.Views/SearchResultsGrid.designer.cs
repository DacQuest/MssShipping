using DacQuest.DFX.Core.DataItems.Proxy;

namespace Mss.Views
{
    partial class SearchResultsGrid
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            //            if (_holdCodesProxy != null)
            //            {
            //                XProxyCache.Release(_holdCodesProxy);
            //                _holdCodesProxy = null;
            //            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.contextMenuGoToLane = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenu
            // 
            this.contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.contextMenuGoToLane});
            this.contextMenu.Name = "contextMenu";
            this.contextMenu.Size = new System.Drawing.Size(107, 26);
            // 
            // contextMenuGoToLane
            // 
            this.contextMenuGoToLane.Name = "contextMenuGoToLane";
            this.contextMenuGoToLane.Size = new System.Drawing.Size(106, 22);
            this.contextMenuGoToLane.Text = "Go To";
            // 
            // SearchResultsGrid
            // 
            //             this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.SearchResultsGrid_MouseClick);
            this.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this._SearchResultsGrid_MouseDoubleClick);
            this.contextMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip contextMenu;
        private System.Windows.Forms.ToolStripMenuItem contextMenuGoToLane;
    }
}
