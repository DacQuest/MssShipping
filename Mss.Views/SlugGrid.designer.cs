using DacQuest.DFX.Core.DataItems.Proxy;
namespace Mss.Views
{
    partial class SlugGrid
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
//             if (_slugProxy != null)
//             {
//                 _slugProxy.DataItemChanged -= _Slug_DataItemChanged;
//                 _slugProxy.CollectionRefreshed -= _Slug_ColletionRefreshed;
//                 XProxyCache.Release(_slugProxy);
//                 _slugProxy = null;
//             }
//             if (_storageProxy != null)
//             {
//                 _storageProxy.DataItemChanged -= _Storage_DataItemChanged;
//                 _storageProxy.CollectionRefreshed -= _Storage_CollectionRefreshed;
//                 XProxyCache.Release(_storageProxy);
//                 _storageProxy = null;
//             }
            //             if (_systemSettingsProxy != null)
            //             {
            // //                _systemSettingsProxy.DataItemChanged -= _OnSystemSettingsChanged;
            // //                _systemSettingsProxy.CollectionRefreshed -= _OnSystemSettingsRefreshed;
            //                 XProxyCache.Release(_systemSettingsProxy);
            //                 _systemSettingsProxy = null;
            //             }
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
            this.contextMenuEditItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuReprintShippingLabel = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuRollback = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuInsertEmptyPallet = new System.Windows.Forms.ToolStripMenuItem();
            this._flashTimer = new System.Windows.Forms.Timer(this.components);
            this.contextMenuReprintLoadLabel = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenu
            // 
            this.contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.contextMenuEditItem,
            this.contextMenuReprintShippingLabel,
            this.contextMenuReprintLoadLabel,
            this.contextMenuRollback,
            this.contextMenuInsertEmptyPallet});
            this.contextMenu.Name = "contextMenu";
            this.contextMenu.Size = new System.Drawing.Size(194, 92);
            this.contextMenu.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this._ContextMenu_ItemClicked);
            // 
            // contextMenuEditItem
            // 
            this.contextMenuEditItem.Name = "contextMenuEditItem";
            this.contextMenuEditItem.Size = new System.Drawing.Size(193, 22);
            this.contextMenuEditItem.Text = "Edit Load Item";
            this.contextMenuEditItem.Visible = false;
            // 
            // contextMenuReprintShippingLabel
            // 
            this.contextMenuReprintShippingLabel.Name = "contextMenuReprintShippingLabel";
            this.contextMenuReprintShippingLabel.Size = new System.Drawing.Size(193, 22);
            this.contextMenuReprintShippingLabel.Text = "Reprint Shipping Label";
            this.contextMenuReprintShippingLabel.Visible = false;
            // 
            // contextMenuRollback
            // 
            this.contextMenuRollback.Name = "contextMenuRollback";
            this.contextMenuRollback.Size = new System.Drawing.Size(193, 22);
            this.contextMenuRollback.Text = "Rollback Pick";
            this.contextMenuRollback.Visible = false;
            // 
            // contextMenuInsertEmptyPallet
            // 
            this.contextMenuInsertEmptyPallet.Name = "contextMenuInsertEmptyPallet";
            this.contextMenuInsertEmptyPallet.Size = new System.Drawing.Size(193, 22);
            this.contextMenuInsertEmptyPallet.Text = "Insert Empty Pallet";
            this.contextMenuInsertEmptyPallet.Visible = false;
            // 
            // _flashTimer
            // 
            this._flashTimer.Interval = 500;
            this._flashTimer.Tick += new System.EventHandler(this._flashTimer_Tick);
            // 
            // contextMenuReprintLoadLabel
            // 
            this.contextMenuReprintLoadLabel.Name = "contextMenuReprintLoadLabel";
            this.contextMenuReprintLoadLabel.Size = new System.Drawing.Size(193, 22);
            this.contextMenuReprintLoadLabel.Text = "Reprint Load Label";
            this.contextMenuReprintLoadLabel.Visible = false;
            // 
            // SlugGrid
            // 
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this._LoadGrid_MouseClick);
            this.contextMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip contextMenu;
        private System.Windows.Forms.ToolStripMenuItem contextMenuReprintShippingLabel;
        private System.Windows.Forms.ToolStripMenuItem contextMenuRollback;
        private System.Windows.Forms.ToolStripMenuItem contextMenuEditItem;
        private System.Windows.Forms.ToolStripMenuItem contextMenuInsertEmptyPallet;
        private System.Windows.Forms.Timer _flashTimer;
        private System.Windows.Forms.ToolStripMenuItem contextMenuReprintLoadLabel;
    }
}
