namespace Mss.Views
{
    partial class PalletTrackerView
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
            this._btnSearch = new System.Windows.Forms.Button();
            this._btnClear = new System.Windows.Forms.Button();
            this._txtPalletID = new System.Windows.Forms.TextBox();
            this._dgvRecords = new System.Windows.Forms.DataGridView();
            this.label1 = new System.Windows.Forms.Label();
            this._chkDescendingOrder = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this._dgvRecords)).BeginInit();
            this.SuspendLayout();
            // 
            // _btnSearch
            // 
            this._btnSearch.Enabled = false;
            this._btnSearch.Location = new System.Drawing.Point(167, 17);
            this._btnSearch.Name = "_btnSearch";
            this._btnSearch.Size = new System.Drawing.Size(75, 23);
            this._btnSearch.TabIndex = 0;
            this._btnSearch.Text = "Search";
            this._btnSearch.UseVisualStyleBackColor = true;
            this._btnSearch.Click += new System.EventHandler(this._BtnSearch_Click);
            // 
            // _btnClear
            // 
            this._btnClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._btnClear.Location = new System.Drawing.Point(474, 17);
            this._btnClear.Name = "_btnClear";
            this._btnClear.Size = new System.Drawing.Size(75, 23);
            this._btnClear.TabIndex = 1;
            this._btnClear.Text = "Clear";
            this._btnClear.UseVisualStyleBackColor = true;
            this._btnClear.Click += new System.EventHandler(this._BtnClear_Click);
            // 
            // _txtPalletID
            // 
            this._txtPalletID.Location = new System.Drawing.Point(59, 19);
            this._txtPalletID.Name = "_txtPalletID";
            this._txtPalletID.Size = new System.Drawing.Size(100, 20);
            this._txtPalletID.TabIndex = 2;
            this._txtPalletID.TextChanged += new System.EventHandler(this._TxtPalletID_TextChanged);
            this._txtPalletID.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this._TxtPalletID_KeyPress);
            // 
            // _dgvRecords
            // 
            this._dgvRecords.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._dgvRecords.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._dgvRecords.Location = new System.Drawing.Point(13, 46);
            this._dgvRecords.Name = "_dgvRecords";
            this._dgvRecords.Size = new System.Drawing.Size(536, 249);
            this._dgvRecords.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Pallet ID";
            // 
            // _chkDescendingOrder
            // 
            this._chkDescendingOrder.AutoSize = true;
            this._chkDescendingOrder.Location = new System.Drawing.Point(249, 23);
            this._chkDescendingOrder.Name = "_chkDescendingOrder";
            this._chkDescendingOrder.Size = new System.Drawing.Size(112, 17);
            this._chkDescendingOrder.TabIndex = 5;
            this._chkDescendingOrder.Text = "Descending Order";
            this._chkDescendingOrder.UseVisualStyleBackColor = true;
            // 
            // PalletTrackerView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._chkDescendingOrder);
            this.Controls.Add(this.label1);
            this.Controls.Add(this._dgvRecords);
            this.Controls.Add(this._txtPalletID);
            this.Controls.Add(this._btnClear);
            this.Controls.Add(this._btnSearch);
            this.Name = "PalletTrackerView";
            this.Size = new System.Drawing.Size(563, 308);
            ((System.ComponentModel.ISupportInitialize)(this._dgvRecords)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button _btnSearch;
        private System.Windows.Forms.Button _btnClear;
        private System.Windows.Forms.TextBox _txtPalletID;
        private System.Windows.Forms.DataGridView _dgvRecords;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox _chkDescendingOrder;
    }
}
