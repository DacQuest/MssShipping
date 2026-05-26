namespace Mss.Views
{
    partial class InventorySummaryView
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
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InventorySummaryView));
            this._dgvInventory = new System.Windows.Forms.DataGridView();
            this.navigator = new System.Windows.Forms.ToolStrip();
            this._navigatorBtnRefreshItem = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this._reportsDropDownButton = new System.Windows.Forms.ToolStripDropDownButton();
            this._inventorySummaryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._fullInventoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this._dgvInventory)).BeginInit();
            this.navigator.SuspendLayout();
            this.SuspendLayout();
            // 
            // _dgvInventory
            // 
            this._dgvInventory.AllowUserToAddRows = false;
            this._dgvInventory.AllowUserToDeleteRows = false;
            this._dgvInventory.AllowUserToResizeColumns = false;
            this._dgvInventory.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this._dgvInventory.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this._dgvInventory.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._dgvInventory.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this._dgvInventory.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._dgvInventory.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._dgvInventory.Location = new System.Drawing.Point(10, 38);
            this._dgvInventory.Name = "_dgvInventory";
            this._dgvInventory.ReadOnly = true;
            this._dgvInventory.RowHeadersVisible = false;
            this._dgvInventory.Size = new System.Drawing.Size(915, 197);
            this._dgvInventory.TabIndex = 0;
            // 
            // navigator
            // 
            this.navigator.BackColor = System.Drawing.SystemColors.Control;
            this.navigator.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.navigator.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.navigator.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._navigatorBtnRefreshItem,
            this.toolStripSeparator1,
            this._reportsDropDownButton});
            this.navigator.Location = new System.Drawing.Point(0, 0);
            this.navigator.Name = "navigator";
            this.navigator.Padding = new System.Windows.Forms.Padding(5, 2, 5, 2);
            this.navigator.Size = new System.Drawing.Size(936, 35);
            this.navigator.TabIndex = 1;
            // 
            // _navigatorBtnRefreshItem
            // 
            this._navigatorBtnRefreshItem.Image = ((System.Drawing.Image)(resources.GetObject("_navigatorBtnRefreshItem.Image")));
            this._navigatorBtnRefreshItem.Name = "_navigatorBtnRefreshItem";
            this._navigatorBtnRefreshItem.Size = new System.Drawing.Size(74, 28);
            this._navigatorBtnRefreshItem.Text = "Refresh";
            this._navigatorBtnRefreshItem.ToolTipText = "Refresh";
            this._navigatorBtnRefreshItem.Click += new System.EventHandler(this._NavigatorBtnRefreshItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 31);
            // 
            // _reportsDropDownButton
            // 
            this._reportsDropDownButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._inventorySummaryToolStripMenuItem,
            this._fullInventoryToolStripMenuItem});
            this._reportsDropDownButton.Image = ((System.Drawing.Image)(resources.GetObject("_reportsDropDownButton.Image")));
            this._reportsDropDownButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._reportsDropDownButton.Name = "_reportsDropDownButton";
            this._reportsDropDownButton.Size = new System.Drawing.Size(84, 28);
            this._reportsDropDownButton.Text = "Reports";
            // 
            // _inventorySummaryToolStripMenuItem
            // 
            this._inventorySummaryToolStripMenuItem.Name = "_inventorySummaryToolStripMenuItem";
            this._inventorySummaryToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            this._inventorySummaryToolStripMenuItem.Text = "Inventory Summary";
            this._inventorySummaryToolStripMenuItem.Click += new System.EventHandler(this._InventorySummaryToolStripMenuItem_Click);
            // 
            // _fullInventoryToolStripMenuItem
            // 
            this._fullInventoryToolStripMenuItem.Name = "_fullInventoryToolStripMenuItem";
            this._fullInventoryToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            this._fullInventoryToolStripMenuItem.Text = "Full Inventory";
            this._fullInventoryToolStripMenuItem.Click += new System.EventHandler(this._FullInventoryToolStripMenuItem_Click);
            // 
            // InventorySummaryView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.navigator);
            this.Controls.Add(this._dgvInventory);
            this.Name = "InventorySummaryView";
            this.Size = new System.Drawing.Size(936, 248);
            ((System.ComponentModel.ISupportInitialize)(this._dgvInventory)).EndInit();
            this.navigator.ResumeLayout(false);
            this.navigator.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView _dgvInventory;
        protected System.Windows.Forms.ToolStrip navigator;
        protected System.Windows.Forms.ToolStripButton _navigatorBtnRefreshItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripDropDownButton _reportsDropDownButton;
        private System.Windows.Forms.ToolStripMenuItem _inventorySummaryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _fullInventoryToolStripMenuItem;
    }
}
