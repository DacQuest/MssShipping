namespace Mss.Views
{
    partial class BroadcastView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BroadcastView));
            this._broadcastGrid = new System.Windows.Forms.DataGridView();
            this.navigator = new System.Windows.Forms.ToolStrip();
            this._navigatorLblCount = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this._navigatorBtnRefreshItem = new System.Windows.Forms.ToolStripButton();
            this._navigatorBtnEdit = new System.Windows.Forms.ToolStripButton();
            this._navigatorBtnRecover = new System.Windows.Forms.ToolStripButton();
            this._navigatorBtnRelease = new System.Windows.Forms.ToolStripButton();
            this._navigatorBtnExport = new System.Windows.Forms.ToolStripButton();
            this._lblTitle = new System.Windows.Forms.Label();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            ((System.ComponentModel.ISupportInitialize)(this._broadcastGrid)).BeginInit();
            this.navigator.SuspendLayout();
            this.SuspendLayout();
            // 
            // _broadcastGrid
            // 
            this._broadcastGrid.AllowUserToAddRows = false;
            this._broadcastGrid.AllowUserToDeleteRows = false;
            this._broadcastGrid.AllowUserToResizeColumns = false;
            this._broadcastGrid.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this._broadcastGrid.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this._broadcastGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._broadcastGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this._broadcastGrid.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._broadcastGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._broadcastGrid.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this._broadcastGrid.GridColor = System.Drawing.SystemColors.Control;
            this._broadcastGrid.Location = new System.Drawing.Point(3, 68);
            this._broadcastGrid.MultiSelect = false;
            this._broadcastGrid.Name = "_broadcastGrid";
            this._broadcastGrid.ReadOnly = true;
            this._broadcastGrid.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this._broadcastGrid.RowHeadersVisible = false;
            this._broadcastGrid.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this._broadcastGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._broadcastGrid.Size = new System.Drawing.Size(638, 345);
            this._broadcastGrid.TabIndex = 1;
            // 
            // navigator
            // 
            this.navigator.BackColor = System.Drawing.SystemColors.Control;
            this.navigator.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.navigator.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.navigator.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._navigatorLblCount,
            this.toolStripSeparator1,
            this._navigatorBtnRefreshItem,
            this._navigatorBtnEdit,
            this._navigatorBtnRecover,
            this._navigatorBtnRelease,
            this._navigatorBtnExport});
            this.navigator.Location = new System.Drawing.Point(0, 0);
            this.navigator.Name = "navigator";
            this.navigator.Padding = new System.Windows.Forms.Padding(5, 2, 5, 2);
            this.navigator.Size = new System.Drawing.Size(644, 35);
            this.navigator.TabIndex = 0;
            // 
            // _navigatorLblCount
            // 
            this._navigatorLblCount.AutoSize = false;
            this._navigatorLblCount.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this._navigatorLblCount.Name = "_navigatorLblCount";
            this._navigatorLblCount.Size = new System.Drawing.Size(70, 28);
            this._navigatorLblCount.Text = "Count:  000";
            this._navigatorLblCount.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 31);
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
            // _navigatorBtnEdit
            // 
            this._navigatorBtnEdit.Enabled = false;
            this._navigatorBtnEdit.Image = ((System.Drawing.Image)(resources.GetObject("_navigatorBtnEdit.Image")));
            this._navigatorBtnEdit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._navigatorBtnEdit.Name = "_navigatorBtnEdit";
            this._navigatorBtnEdit.Size = new System.Drawing.Size(55, 28);
            this._navigatorBtnEdit.Text = "Edit";
            this._navigatorBtnEdit.Visible = false;
            this._navigatorBtnEdit.Click += new System.EventHandler(this._NavigatorBtnEdit_Click);
            // 
            // _navigatorBtnRecover
            // 
            this._navigatorBtnRecover.Enabled = false;
            this._navigatorBtnRecover.Image = global::Mss.Views.Properties.Resources.GreenRecycle24;
            this._navigatorBtnRecover.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._navigatorBtnRecover.Name = "_navigatorBtnRecover";
            this._navigatorBtnRecover.Size = new System.Drawing.Size(77, 28);
            this._navigatorBtnRecover.Text = "Recover";
            this._navigatorBtnRecover.Visible = false;
            this._navigatorBtnRecover.Click += new System.EventHandler(this._NavigatorBtnRecover_Click);
            // 
            // _navigatorBtnRelease
            // 
            this._navigatorBtnRelease.Enabled = false;
            this._navigatorBtnRelease.Image = global::Mss.Views.Properties.Resources.PurpleRelease24;
            this._navigatorBtnRelease.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._navigatorBtnRelease.Name = "_navigatorBtnRelease";
            this._navigatorBtnRelease.Size = new System.Drawing.Size(74, 28);
            this._navigatorBtnRelease.Text = "Release";
            this._navigatorBtnRelease.Visible = false;
            this._navigatorBtnRelease.Click += new System.EventHandler(this._NavigatorBtnRelease_Click);
            // 
            // _navigatorBtnExport
            // 
            this._navigatorBtnExport.Enabled = false;
            this._navigatorBtnExport.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._navigatorBtnExport.Name = "_navigatorBtnExport";
            this._navigatorBtnExport.Size = new System.Drawing.Size(44, 28);
            this._navigatorBtnExport.Text = "Export";
            this._navigatorBtnExport.Visible = false;
            this._navigatorBtnExport.Click += new System.EventHandler(this._NavigatorBtnExport_Click);
            // 
            // _lblTitle
            // 
            this._lblTitle.AutoSize = true;
            this._lblTitle.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._lblTitle.Location = new System.Drawing.Point(3, 46);
            this._lblTitle.Name = "_lblTitle";
            this._lblTitle.Size = new System.Drawing.Size(88, 19);
            this._lblTitle.TabIndex = 2;
            this._lblTitle.Text = "Broadcast";
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.DefaultExt = "csv";
            this.saveFileDialog1.FileName = "Broadcast.csv";
            this.saveFileDialog1.Filter = "CSV Files|*.csv";
            // 
            // BroadcastView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._lblTitle);
            this.Controls.Add(this._broadcastGrid);
            this.Controls.Add(this.navigator);
            this.Name = "BroadcastView";
            this.Size = new System.Drawing.Size(644, 413);
            ((System.ComponentModel.ISupportInitialize)(this._broadcastGrid)).EndInit();
            this.navigator.ResumeLayout(false);
            this.navigator.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        protected System.Windows.Forms.ToolStrip navigator;
        protected System.Windows.Forms.ToolStripButton _navigatorBtnRefreshItem;
        private System.Windows.Forms.DataGridView _broadcastGrid;
        private System.Windows.Forms.ToolStripLabel _navigatorLblCount;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton _navigatorBtnEdit;
        private System.Windows.Forms.ToolStripButton _navigatorBtnRelease;
        private System.Windows.Forms.ToolStripButton _navigatorBtnRecover;
        private System.Windows.Forms.Label _lblTitle;
        private System.Windows.Forms.ToolStripButton _navigatorBtnExport;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
    }
}
