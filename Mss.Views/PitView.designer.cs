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
            this.navigator = new System.Windows.Forms.ToolStrip();
            this.navigatorBtnRefreshItem = new System.Windows.Forms.ToolStripButton();
            this.navigatorBtnAddItem = new System.Windows.Forms.ToolStripButton();
            this.navigatorBtnQuickAdd = new System.Windows.Forms.ToolStripButton();
            this.navigatorBtnAddSingleEmpty = new System.Windows.Forms.ToolStripButton();
            this.navigatorBtnDelete = new System.Windows.Forms.ToolStripButton();
            this._lblCollectionName = new System.Windows.Forms.Label();
            this._dgvPit = new System.Windows.Forms.DataGridView();
            this.navigator.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._dgvPit)).BeginInit();
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
            this.navigator.Size = new System.Drawing.Size(671, 35);
            this.navigator.TabIndex = 0;
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
            this.navigatorBtnQuickAdd.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.navigatorBtnQuickAdd.Name = "navigatorBtnQuickAdd";
            this.navigatorBtnQuickAdd.Size = new System.Drawing.Size(65, 28);
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
            // _lblCollectionName
            // 
            this._lblCollectionName.AutoSize = true;
            this._lblCollectionName.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold);
            this._lblCollectionName.Location = new System.Drawing.Point(3, 46);
            this._lblCollectionName.Name = "_lblCollectionName";
            this._lblCollectionName.Size = new System.Drawing.Size(134, 19);
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
            this._dgvPit.RowHeadersVisible = false;
            this._dgvPit.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this._dgvPit.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._dgvPit.Size = new System.Drawing.Size(668, 241);
            this._dgvPit.TabIndex = 3;
            this._dgvPit.SelectionChanged += new System.EventHandler(this._DgvPit_SelectionChanged);
            // 
            // PitView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._dgvPit);
            this.Controls.Add(this._lblCollectionName);
            this.Controls.Add(this.navigator);
            this.Name = "PitView";
            this.Size = new System.Drawing.Size(671, 312);
            this.navigator.ResumeLayout(false);
            this.navigator.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._dgvPit)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        protected System.Windows.Forms.ToolStrip navigator;
        protected System.Windows.Forms.ToolStripButton navigatorBtnRefreshItem;
        private System.Windows.Forms.ToolStripButton navigatorBtnAddItem;
        private System.Windows.Forms.ToolStripButton navigatorBtnDelete;
        private System.Windows.Forms.Label _lblCollectionName;
        private System.Windows.Forms.ToolStripButton navigatorBtnAddSingleEmpty;
        private System.Windows.Forms.ToolStripButton navigatorBtnQuickAdd;
        private System.Windows.Forms.DataGridView _dgvPit;
    }
}
