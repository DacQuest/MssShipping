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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BroadcastView));
            this.dataGrid = new System.Windows.Forms.DataGridView();
            this.navigator = new System.Windows.Forms.ToolStrip();
            this.navigatorLblCount = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.navigatorLblInstruction = new System.Windows.Forms.ToolStripLabel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.navigatorBtnRefreshItem = new System.Windows.Forms.ToolStripButton();
            this.navigatorBtnExport = new System.Windows.Forms.ToolStripButton();
            this.navigatorBtnRelease = new System.Windows.Forms.ToolStripButton();
            this.navigatorBtnEdit = new System.Windows.Forms.ToolStripButton();
            this.navigatorBtnRecover = new System.Windows.Forms.ToolStripButton();
            ((System.ComponentModel.ISupportInitialize)(this.dataGrid)).BeginInit();
            this.navigator.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGrid
            // 
            this.dataGrid.AllowUserToAddRows = false;
            this.dataGrid.AllowUserToDeleteRows = false;
            this.dataGrid.AllowUserToOrderColumns = true;
            this.dataGrid.AllowUserToResizeRows = false;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.dataGrid.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle2;
            this.dataGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGrid.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGrid.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dataGrid.GridColor = System.Drawing.SystemColors.Control;
            this.dataGrid.Location = new System.Drawing.Point(0, 68);
            this.dataGrid.MultiSelect = false;
            this.dataGrid.Name = "dataGrid";
            this.dataGrid.ReadOnly = true;
            this.dataGrid.RowHeadersVisible = false;
            this.dataGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGrid.Size = new System.Drawing.Size(718, 345);
            this.dataGrid.TabIndex = 1;
            // 
            // navigator
            // 
            this.navigator.BackColor = System.Drawing.SystemColors.Control;
            this.navigator.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.navigator.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.navigator.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.navigatorLblCount,
            this.toolStripSeparator1,
            this.navigatorBtnRefreshItem,
            this.navigatorBtnExport,
            this.navigatorBtnRelease,
            this.navigatorBtnEdit,
            this.navigatorBtnRecover,
            this.navigatorLblInstruction});
            this.navigator.Location = new System.Drawing.Point(0, 0);
            this.navigator.Name = "navigator";
            this.navigator.Padding = new System.Windows.Forms.Padding(5, 2, 5, 2);
            this.navigator.Size = new System.Drawing.Size(718, 35);
            this.navigator.TabIndex = 0;
            // 
            // navigatorLblCount
            // 
            this.navigatorLblCount.AutoSize = false;
            this.navigatorLblCount.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.navigatorLblCount.Name = "navigatorLblCount";
            this.navigatorLblCount.Size = new System.Drawing.Size(70, 28);
            this.navigatorLblCount.Text = "Count:  000";
            this.navigatorLblCount.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 31);
            // 
            // navigatorLblInstruction
            // 
            this.navigatorLblInstruction.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.navigatorLblInstruction.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.navigatorLblInstruction.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.navigatorLblInstruction.Name = "navigatorLblInstruction";
            this.navigatorLblInstruction.Size = new System.Drawing.Size(205, 15);
            this.navigatorLblInstruction.Text = "Click [Refresh] to update shortages.";
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.Location = new System.Drawing.Point(3, 46);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(88, 19);
            this.lblTitle.TabIndex = 2;
            this.lblTitle.Text = "Broadcast";
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.DefaultExt = "csv";
            this.saveFileDialog1.FileName = "Broadcast.csv";
            this.saveFileDialog1.Filter = "CSV Files|*.csv";
            // 
            // navigatorBtnRefreshItem
            // 
            this.navigatorBtnRefreshItem.Image = global::Mss.Views.Properties.Resources.GreenRefresh24;
            this.navigatorBtnRefreshItem.Name = "navigatorBtnRefreshItem";
            this.navigatorBtnRefreshItem.Size = new System.Drawing.Size(74, 28);
            this.navigatorBtnRefreshItem.Text = "Refresh";
            this.navigatorBtnRefreshItem.ToolTipText = "Refresh";
            this.navigatorBtnRefreshItem.Click += new System.EventHandler(this.navigatorBtnRefreshItem_Click);
            // 
            // navigatorBtnExport
            // 
//             this.navigatorBtnExport.Image = global::Mss.Views.Properties.Resources.BlueExport24;
            this.navigatorBtnExport.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.navigatorBtnExport.Name = "navigatorBtnExport";
            this.navigatorBtnExport.Size = new System.Drawing.Size(68, 28);
            this.navigatorBtnExport.Text = "Export";
            this.navigatorBtnExport.Click += new System.EventHandler(this._NavigatorBtnExport_Click);
            // 
            // navigatorBtnRelease
            // 
            this.navigatorBtnRelease.Enabled = false;
            this.navigatorBtnRelease.Image = global::Mss.Views.Properties.Resources.PurpleRelease24;
            this.navigatorBtnRelease.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.navigatorBtnRelease.Name = "navigatorBtnRelease";
            this.navigatorBtnRelease.Size = new System.Drawing.Size(74, 28);
            this.navigatorBtnRelease.Text = "Release";
            this.navigatorBtnRelease.Visible = false;
            this.navigatorBtnRelease.Click += new System.EventHandler(this.navigatorBtnRelease_Click);
            // 
            // navigatorBtnEdit
            // 
            this.navigatorBtnEdit.Enabled = false;
            this.navigatorBtnEdit.Image = ((System.Drawing.Image)(resources.GetObject("navigatorBtnEdit.Image")));
            this.navigatorBtnEdit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.navigatorBtnEdit.Name = "navigatorBtnEdit";
            this.navigatorBtnEdit.Size = new System.Drawing.Size(55, 28);
            this.navigatorBtnEdit.Text = "Edit";
            this.navigatorBtnEdit.Visible = false;
            this.navigatorBtnEdit.Click += new System.EventHandler(this.navigatorBtnEdit_Click);
            // 
            // navigatorBtnRecover
            // 
            this.navigatorBtnRecover.Enabled = false;
            this.navigatorBtnRecover.Image = global::Mss.Views.Properties.Resources.GreenRecycle24;
            this.navigatorBtnRecover.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.navigatorBtnRecover.Name = "navigatorBtnRecover";
            this.navigatorBtnRecover.Size = new System.Drawing.Size(77, 28);
            this.navigatorBtnRecover.Text = "Recover";
            this.navigatorBtnRecover.Visible = false;
            this.navigatorBtnRecover.Click += new System.EventHandler(this.navigatorBtnRecover_Click);
            // 
            // BroadcastView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.dataGrid);
            this.Controls.Add(this.navigator);
            this.Name = "BroadcastView";
            this.Size = new System.Drawing.Size(718, 413);
            ((System.ComponentModel.ISupportInitialize)(this.dataGrid)).EndInit();
            this.navigator.ResumeLayout(false);
            this.navigator.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        protected System.Windows.Forms.ToolStrip navigator;
        protected System.Windows.Forms.ToolStripButton navigatorBtnRefreshItem;
        private System.Windows.Forms.DataGridView dataGrid;
        private System.Windows.Forms.ToolStripLabel navigatorLblCount;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton navigatorBtnEdit;
        private System.Windows.Forms.ToolStripButton navigatorBtnRelease;
        private System.Windows.Forms.ToolStripButton navigatorBtnRecover;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.ToolStripLabel navigatorLblInstruction;
        private System.Windows.Forms.ToolStripButton navigatorBtnExport;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
    }
}
