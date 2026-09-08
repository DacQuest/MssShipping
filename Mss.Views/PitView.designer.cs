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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this._navigator = new System.Windows.Forms.ToolStrip();
            this._navigatorBtnRefreshItem = new System.Windows.Forms.ToolStripButton();
            this._navigatorBtnAddItem = new System.Windows.Forms.ToolStripButton();
            this._navigatorBtnDelete = new System.Windows.Forms.ToolStripButton();
            this._navigatorBtnAddSingleEmpty = new System.Windows.Forms.ToolStripButton();
            this._navigatorBtnQuickAdd = new System.Windows.Forms.ToolStripButton();
            this._lblCollectionName = new System.Windows.Forms.Label();
            this._dgvPit = new System.Windows.Forms.DataGridView();
            this._navigator.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._dgvPit)).BeginInit();
            this.SuspendLayout();
            // 
            // _navigator
            // 
            this._navigator.BackColor = System.Drawing.SystemColors.Control;
            this._navigator.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this._navigator.ImageScalingSize = new System.Drawing.Size(24, 24);
            this._navigator.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._navigatorBtnRefreshItem,
            this._navigatorBtnAddItem,
            this._navigatorBtnDelete,
            this._navigatorBtnAddSingleEmpty,
            this._navigatorBtnQuickAdd});
            this._navigator.Location = new System.Drawing.Point(0, 0);
            this._navigator.Name = "_navigator";
            this._navigator.Padding = new System.Windows.Forms.Padding(5, 2, 5, 2);
            this._navigator.Size = new System.Drawing.Size(671, 35);
            this._navigator.TabIndex = 0;
            // 
            // _navigatorBtnRefreshItem
            // 
            this._navigatorBtnRefreshItem.Image = global::Mss.Views.Properties.Resources.GreenRefresh24;
            this._navigatorBtnRefreshItem.Name = "_navigatorBtnRefreshItem";
            this._navigatorBtnRefreshItem.Size = new System.Drawing.Size(74, 28);
            this._navigatorBtnRefreshItem.Text = "Refresh";
            this._navigatorBtnRefreshItem.ToolTipText = "Refresh";
            this._navigatorBtnRefreshItem.Click += new System.EventHandler(this._NavigatorBtnRefreshItem_Click);
            // 
            // _navigatorBtnAddItem
            // 
            this._navigatorBtnAddItem.Image = global::Mss.Views.Properties.Resources.GreenPlus24;
            this._navigatorBtnAddItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._navigatorBtnAddItem.Name = "_navigatorBtnAddItem";
            this._navigatorBtnAddItem.Size = new System.Drawing.Size(89, 28);
            this._navigatorBtnAddItem.Text = "Add Pallet";
            this._navigatorBtnAddItem.Visible = false;
            this._navigatorBtnAddItem.Click += new System.EventHandler(this._NavigatorBtnAddItem_Click);
            // 
            // _navigatorBtnDelete
            // 
            this._navigatorBtnDelete.Image = global::Mss.Views.Properties.Resources.RedMinus24;
            this._navigatorBtnDelete.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._navigatorBtnDelete.Name = "_navigatorBtnDelete";
            this._navigatorBtnDelete.Size = new System.Drawing.Size(100, 28);
            this._navigatorBtnDelete.Text = "Delete Pallet";
            this._navigatorBtnDelete.Visible = false;
            this._navigatorBtnDelete.Click += new System.EventHandler(this._NavigatorBtnDelete_Click);
            // 
            // _navigatorBtnAddSingleEmpty
            // 
            this._navigatorBtnAddSingleEmpty.Image = global::Mss.Views.Properties.Resources.GreenPlus24;
            this._navigatorBtnAddSingleEmpty.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._navigatorBtnAddSingleEmpty.Name = "_navigatorBtnAddSingleEmpty";
            this._navigatorBtnAddSingleEmpty.Size = new System.Drawing.Size(129, 28);
            this._navigatorBtnAddSingleEmpty.Text = "Add Single Empty";
            this._navigatorBtnAddSingleEmpty.Visible = false;
            this._navigatorBtnAddSingleEmpty.Click += new System.EventHandler(this._NavigatorBtnAddSingleEmpty_Click);
            // 
            // _navigatorBtnQuickAdd
            // 
            this._navigatorBtnQuickAdd.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._navigatorBtnQuickAdd.Name = "_navigatorBtnQuickAdd";
            this._navigatorBtnQuickAdd.Size = new System.Drawing.Size(99, 28);
            this._navigatorBtnQuickAdd.Text = "Quick Add Pallet";
            this._navigatorBtnQuickAdd.Visible = false;
            this._navigatorBtnQuickAdd.Click += new System.EventHandler(this._NavigatorBtnQuickAdd_Click);
            // 
            // _lblCollectionName
            // 
            this._lblCollectionName.AutoSize = true;
            this._lblCollectionName.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this._lblCollectionName.Location = new System.Drawing.Point(3, 35);
            this._lblCollectionName.Name = "_lblCollectionName";
            this._lblCollectionName.Size = new System.Drawing.Size(183, 30);
            this._lblCollectionName.TabIndex = 1;
            this._lblCollectionName.Text = "Collection Name";
            // 
            // _dgvPit
            // 
            this._dgvPit.AllowUserToAddRows = false;
            this._dgvPit.AllowUserToDeleteRows = false;
            this._dgvPit.AllowUserToResizeColumns = false;
            this._dgvPit.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this._dgvPit.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this._dgvPit.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._dgvPit.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this._dgvPit.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._dgvPit.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._dgvPit.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this._dgvPit.GridColor = System.Drawing.SystemColors.Control;
            this._dgvPit.Location = new System.Drawing.Point(3, 68);
            this._dgvPit.Name = "_dgvPit";
            this._dgvPit.ReadOnly = true;
            this._dgvPit.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders;
            this._dgvPit.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this._dgvPit.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._dgvPit.Size = new System.Drawing.Size(668, 241);
            this._dgvPit.TabIndex = 3;
            this._dgvPit.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this._DgvPit_CellMouseClick);
            this._dgvPit.RowPostPaint += new System.Windows.Forms.DataGridViewRowPostPaintEventHandler(this._DgvPit_RowPostPaint);
            this._dgvPit.SelectionChanged += new System.EventHandler(this._DgvPit_SelectionChanged);
            // 
            // PitView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._dgvPit);
            this.Controls.Add(this._lblCollectionName);
            this.Controls.Add(this._navigator);
            this.Name = "PitView";
            this.Size = new System.Drawing.Size(671, 312);
            this._navigator.ResumeLayout(false);
            this._navigator.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._dgvPit)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        protected System.Windows.Forms.ToolStrip _navigator;
        protected System.Windows.Forms.ToolStripButton _navigatorBtnRefreshItem;
        private System.Windows.Forms.ToolStripButton _navigatorBtnAddItem;
        private System.Windows.Forms.ToolStripButton _navigatorBtnDelete;
        private System.Windows.Forms.Label _lblCollectionName;
        private System.Windows.Forms.ToolStripButton _navigatorBtnAddSingleEmpty;
        private System.Windows.Forms.ToolStripButton _navigatorBtnQuickAdd;
        private System.Windows.Forms.DataGridView _dgvPit;
    }
}
