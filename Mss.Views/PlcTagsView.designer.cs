namespace Mss.Views
{
    partial class PlcTagsView
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
            this._dgvTags = new System.Windows.Forms.DataGridView();
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.toolStripTitleLabel = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripTagSetLabel = new System.Windows.Forms.ToolStripLabel();
            this.toolStripDevicesComboBox = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripRefreshButton = new System.Windows.Forms.ToolStripButton();
            ((System.ComponentModel.ISupportInitialize)(this._dgvTags)).BeginInit();
            this.toolStrip2.SuspendLayout();
            this.SuspendLayout();
            // 
            // _dgvTags
            // 
            this._dgvTags.AllowUserToAddRows = false;
            this._dgvTags.AllowUserToDeleteRows = false;
            this._dgvTags.AllowUserToOrderColumns = true;
            this._dgvTags.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this._dgvTags.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this._dgvTags.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._dgvTags.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this._dgvTags.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._dgvTags.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._dgvTags.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this._dgvTags.GridColor = System.Drawing.SystemColors.Control;
            this._dgvTags.Location = new System.Drawing.Point(0, 28);
            this._dgvTags.MultiSelect = false;
            this._dgvTags.Name = "_dgvTags";
            this._dgvTags.ReadOnly = true;
            this._dgvTags.RowHeadersVisible = false;
            this._dgvTags.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._dgvTags.Size = new System.Drawing.Size(720, 368);
            this._dgvTags.TabIndex = 1;
            this._dgvTags.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this._DataGrid_CellDoubleClick);
            // 
            // toolStrip2
            // 
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripTitleLabel,
            this.toolStripSeparator1,
            this.toolStripRefreshButton,
            this.toolStripSeparator2,
            this.toolStripTagSetLabel,
            this.toolStripDevicesComboBox});
            this.toolStrip2.Location = new System.Drawing.Point(0, 0);
            this.toolStrip2.Name = "toolStrip2";
            this.toolStrip2.Size = new System.Drawing.Size(718, 25);
            this.toolStrip2.TabIndex = 4;
            this.toolStrip2.Text = "toolStrip2";
            // 
            // toolStripTitleLabel
            // 
            this.toolStripTitleLabel.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripTitleLabel.Name = "toolStripTitleLabel";
            this.toolStripTitleLabel.Size = new System.Drawing.Size(92, 22);
            this.toolStripTitleLabel.Text = "PLC Monitor";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripTagSetLabel
            // 
            this.toolStripTagSetLabel.Name = "toolStripTagSetLabel";
            this.toolStripTagSetLabel.Size = new System.Drawing.Size(68, 22);
            this.toolStripTagSetLabel.Text = "Tag Group: ";
            // 
            // toolStripDevicesComboBox
            // 
            this.toolStripDevicesComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.toolStripDevicesComboBox.Name = "toolStripDevicesComboBox";
            this.toolStripDevicesComboBox.Size = new System.Drawing.Size(200, 25);
            this.toolStripDevicesComboBox.SelectedIndexChanged += new System.EventHandler(this._ToolStripDevicesComboBox_SelectedIndexChanged);
            // 
            // toolStripRefreshButton
            // 
            this.toolStripRefreshButton.Image = global::Mss.Views.Properties.Resources.GreenRefresh24;
            this.toolStripRefreshButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripRefreshButton.Name = "toolStripRefreshButton";
            this.toolStripRefreshButton.Size = new System.Drawing.Size(66, 22);
            this.toolStripRefreshButton.Text = "Refresh";
            this.toolStripRefreshButton.Click += new System.EventHandler(this._ToolStripRefreshButton_Click);
            // 
            // PlcTagsView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.toolStrip2);
            this.Controls.Add(this._dgvTags);
            this.Name = "PlcTagsView";
            this.Size = new System.Drawing.Size(718, 396);
            ((System.ComponentModel.ISupportInitialize)(this._dgvTags)).EndInit();
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView _dgvTags;
        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.ToolStripLabel toolStripTitleLabel;
        private System.Windows.Forms.ToolStripButton toolStripRefreshButton;
        private System.Windows.Forms.ToolStripLabel toolStripTagSetLabel;
        private System.Windows.Forms.ToolStripComboBox toolStripDevicesComboBox;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
    }
}
