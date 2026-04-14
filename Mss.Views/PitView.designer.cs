namespace Mss.Views
{
    partial class PitView
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
            this.navigator = new System.Windows.Forms.ToolStrip();
            this.lblCollectionName = new System.Windows.Forms.Label();
            this._pitGrid = new Mss.Views.PitGrid();
            this.navigatorBtnRefreshItem = new System.Windows.Forms.ToolStripButton();
            this.navigatorBtnAddItem = new System.Windows.Forms.ToolStripButton();
            this.navigatorBtnQuickAdd = new System.Windows.Forms.ToolStripButton();
            this.navigatorBtnAddSingleEmpty = new System.Windows.Forms.ToolStripButton();
            this.navigatorBtnDelete = new System.Windows.Forms.ToolStripButton();
            this.navigator.SuspendLayout();
            this.SuspendLayout();
            // 
            // navigator
            // 
            this.navigator.BackColor = System.Drawing.SystemColors.Control;
            this.navigator.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.navigator.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.navigator.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.navigatorBtnRefreshItem,
            this.navigatorBtnAddItem,
            this.navigatorBtnQuickAdd,
            this.navigatorBtnAddSingleEmpty,
            this.navigatorBtnDelete});
            this.navigator.Location = new System.Drawing.Point(0, 0);
            this.navigator.Name = "navigator";
            this.navigator.Padding = new System.Windows.Forms.Padding(5, 2, 5, 2);
            this.navigator.Size = new System.Drawing.Size(861, 35);
            this.navigator.TabIndex = 0;
            // 
            // lblCollectionName
            // 
            this.lblCollectionName.AutoSize = true;
            this.lblCollectionName.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold);
            this.lblCollectionName.Location = new System.Drawing.Point(16, 46);
            this.lblCollectionName.Name = "lblCollectionName";
            this.lblCollectionName.Size = new System.Drawing.Size(134, 19);
            this.lblCollectionName.TabIndex = 1;
            this.lblCollectionName.Text = "Collection Name";
            // 
            // _pitGrid
            // 
            this._pitGrid.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this._pitGrid.EnableSort = false;
            this._pitGrid.Location = new System.Drawing.Point(19, 65);
            this._pitGrid.Name = "_pitGrid";
            this._pitGrid.OptimizeMode = SourceGrid.CellOptimizeMode.ForRows;
            this._pitGrid.SelectionMode = SourceGrid.GridSelectionMode.Row;
            this._pitGrid.Size = new System.Drawing.Size(826, 372);
            this._pitGrid.TabIndex = 2;
            this._pitGrid.TabStop = true;
            this._pitGrid.Text = "PalletGrid";
            this._pitGrid.ToolTipText = "";
            // 
            // navigatorBtnRefreshItem
            // 
            this.navigatorBtnRefreshItem.Image = global::Mss.Views.Properties.Resources.GreenRefresh24;
            this.navigatorBtnRefreshItem.Name = "navigatorBtnRefreshItem";
            this.navigatorBtnRefreshItem.Size = new System.Drawing.Size(74, 28);
            this.navigatorBtnRefreshItem.Text = "Refresh";
            this.navigatorBtnRefreshItem.ToolTipText = "Refresh";
            this.navigatorBtnRefreshItem.Click += new System.EventHandler(this._NavigatorBtnRefreshItem_Click);
            // 
            // navigatorBtnAddItem
            // 
            this.navigatorBtnAddItem.Image = global::Mss.Views.Properties.Resources.GreenPlus24;
            this.navigatorBtnAddItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.navigatorBtnAddItem.Name = "navigatorBtnAddItem";
            this.navigatorBtnAddItem.Size = new System.Drawing.Size(117, 28);
            this.navigatorBtnAddItem.Text = "Slow Add Pallet";
            this.navigatorBtnAddItem.Visible = false;
            this.navigatorBtnAddItem.Click += new System.EventHandler(this._NavigatorBtnAddItem_Click);
            // 
            // navigatorBtnQuickAdd
            // 
//             this.navigatorBtnQuickAdd.Image = global::Mss.Views.Properties.Resources.QuickGreenPlus24;
            this.navigatorBtnQuickAdd.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.navigatorBtnQuickAdd.Name = "navigatorBtnQuickAdd";
            this.navigatorBtnQuickAdd.Size = new System.Drawing.Size(89, 28);
            this.navigatorBtnQuickAdd.Text = "Add Pallet";
            this.navigatorBtnQuickAdd.Visible = false;
            this.navigatorBtnQuickAdd.Click += new System.EventHandler(this._NavigatorBtnQuickAdd_Click);
            // 
            // navigatorBtnAddSingleEmpty
            // 
            this.navigatorBtnAddSingleEmpty.Image = global::Mss.Views.Properties.Resources.GreenPlus24;
            this.navigatorBtnAddSingleEmpty.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.navigatorBtnAddSingleEmpty.Name = "navigatorBtnAddSingleEmpty";
            this.navigatorBtnAddSingleEmpty.Size = new System.Drawing.Size(129, 28);
            this.navigatorBtnAddSingleEmpty.Text = "Add Single Empty";
            this.navigatorBtnAddSingleEmpty.Visible = false;
            this.navigatorBtnAddSingleEmpty.Click += new System.EventHandler(this._NavigatorBtnAddSingleEmpty_Click);
            // 
            // navigatorBtnDelete
            // 
            this.navigatorBtnDelete.Image = global::Mss.Views.Properties.Resources.RedMinus24;
            this.navigatorBtnDelete.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.navigatorBtnDelete.Name = "navigatorBtnDelete";
            this.navigatorBtnDelete.Size = new System.Drawing.Size(100, 28);
            this.navigatorBtnDelete.Text = "Delete Pallet";
            this.navigatorBtnDelete.Visible = false;
            this.navigatorBtnDelete.Click += new System.EventHandler(this._NavigatorBtnDelete_Click);
            // 
            // PitView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblCollectionName);
            this.Controls.Add(this.navigator);
            this.Controls.Add(this._pitGrid);
            this.Name = "PitView";
            this.Size = new System.Drawing.Size(861, 455);
            this.navigator.ResumeLayout(false);
            this.navigator.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private PitGrid _pitGrid;
        protected System.Windows.Forms.ToolStrip navigator;
        protected System.Windows.Forms.ToolStripButton navigatorBtnRefreshItem;
        private System.Windows.Forms.ToolStripButton navigatorBtnAddItem;
        private System.Windows.Forms.ToolStripButton navigatorBtnDelete;
        private System.Windows.Forms.Label lblCollectionName;
        private System.Windows.Forms.ToolStripButton navigatorBtnAddSingleEmpty;
        private System.Windows.Forms.ToolStripButton navigatorBtnQuickAdd;
    }
}
