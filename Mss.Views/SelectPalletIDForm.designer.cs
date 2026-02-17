namespace Mss.Views
{
    partial class SelectPalletIDForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this._btnOK = new System.Windows.Forms.Button();
            this._btnCancel = new System.Windows.Forms.Button();
            this._cmbPalletID = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // _btnOK
            // 
            this._btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this._btnOK.Location = new System.Drawing.Point(12, 42);
            this._btnOK.Name = "_btnOK";
            this._btnOK.Size = new System.Drawing.Size(75, 23);
            this._btnOK.TabIndex = 1;
            this._btnOK.Text = "OK";
            this._btnOK.UseVisualStyleBackColor = true;
            this._btnOK.Click += new System.EventHandler(this._BtnOK_Click);
            // 
            // _btnCancel
            // 
            this._btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._btnCancel.Location = new System.Drawing.Point(93, 42);
            this._btnCancel.Name = "_btnCancel";
            this._btnCancel.Size = new System.Drawing.Size(75, 23);
            this._btnCancel.TabIndex = 2;
            this._btnCancel.Text = "Cancel";
            this._btnCancel.UseVisualStyleBackColor = true;
            // 
            // _cmbPalletID
            // 
            this._cmbPalletID.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this._cmbPalletID.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this._cmbPalletID.FormattingEnabled = true;
            this._cmbPalletID.Location = new System.Drawing.Point(12, 12);
            this._cmbPalletID.MaxDropDownItems = 16;
            this._cmbPalletID.Name = "_cmbPalletID";
            this._cmbPalletID.Size = new System.Drawing.Size(157, 21);
            this._cmbPalletID.TabIndex = 0;
            this._cmbPalletID.SelectedIndexChanged += new System.EventHandler(this._CmbPalletID_SelectedIndexChanged);
            this._cmbPalletID.KeyDown += new System.Windows.Forms.KeyEventHandler(this._CmbPalletID_KeyDown);
            this._cmbPalletID.Leave += new System.EventHandler(this._CmbPalletID_Leave);
            // 
            // SelectPalletIDForm
            // 
            this.AcceptButton = this._btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this._btnCancel;
            this.ClientSize = new System.Drawing.Size(181, 77);
            this.ControlBox = false;
            this.Controls.Add(this._cmbPalletID);
            this.Controls.Add(this._btnCancel);
            this.Controls.Add(this._btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "SelectPalletIDForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Select Pallet ID";
            this.Load += new System.EventHandler(this._SelectPalletIDForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button _btnOK;
        private System.Windows.Forms.Button _btnCancel;
        private System.Windows.Forms.ComboBox _cmbPalletID;
    }
}