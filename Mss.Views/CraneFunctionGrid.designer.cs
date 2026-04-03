using DacQuest.DFX.Core.DataItems.Proxy;
namespace Mss.Views
{
    partial class CraneFunctionGrid
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
            if (_systemSettingsProxy != null)
            {
                _systemSettingsProxy.DataItemChanged -= _SystemSettings_DataItemChanged;
                _systemSettingsProxy.CollectionRefreshed -= _SystemSettings_CollectionRefreshed;
                XProxyCache.Release(_systemSettingsProxy);
                _systemSettingsProxy = null;
            }
            if (_storageProxy != null)
            {
                _storageProxy.DataItemChanged -= _Storage_DataItemChanged;
                _storageProxy.CollectionRefreshed -= _Storage_CollectionRefreshed;
                XProxyCache.Release(_storageProxy);
                _storageProxy = null;
            }
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
            this.contextMenuReprint = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuEditItem = new System.Windows.Forms.ToolStripMenuItem();
            //this.contextMenuInsertEmpty = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenu
            // 
            this.contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.contextMenuReprint,
            this.contextMenuEditItem});
            //this.contextMenuInsertEmpty});
            this.contextMenu.Name = "contextMenu";
            this.contextMenu.Size = new System.Drawing.Size(194, 70);
            this.contextMenu.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this._ContextMenu_ItemClicked);
            // 
            // contextMenuReprint
            // 
            this.contextMenuReprint.Name = "contextMenuReprint";
            this.contextMenuReprint.Size = new System.Drawing.Size(193, 22);
            this.contextMenuReprint.Text = "Reprint Shipping Label";
            // 
            // contextMenuEditItem
            // 
            this.contextMenuEditItem.Name = "contextMenuEditItem";
            this.contextMenuEditItem.Size = new System.Drawing.Size(193, 22);
            this.contextMenuEditItem.Text = "Edit Load Item";
            // 
            // contextMenuInsertEmpty
            // 
            //this.contextMenuInsertEmpty.Name = "contextMenuInsertEmpty";
            //this.contextMenuInsertEmpty.Size = new System.Drawing.Size(193, 22);
            //this.contextMenuInsertEmpty.Text = "Insert Empty Pallet";
            // 
            // CraneFunctionGrid
            // 
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this._CraneFunctionGrid_MouseClick);
            this.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this._CraneFunctionGrid_MouseDoubleClick);
            this.contextMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip contextMenu;
        private System.Windows.Forms.ToolStripMenuItem contextMenuReprint;
        private System.Windows.Forms.ToolStripMenuItem contextMenuEditItem;
        //private System.Windows.Forms.ToolStripMenuItem contextMenuInsertEmpty;
    }
}
