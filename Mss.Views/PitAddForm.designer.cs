namespace Mss.Views
{
    partial class PitAddForm
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
            this._btnCancel = new System.Windows.Forms.Button();
            this._btnAdd = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this._cmbNewPalletStatus = new System.Windows.Forms.ComboBox();
            this._chkOverridePalletStatus = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this._txtPalletID = new System.Windows.Forms.TextBox();
            this._cmbPitCode = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this._cmbHoldCode = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // _btnCancel
            // 
            this._btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._btnCancel.Location = new System.Drawing.Point(233, 154);
            this._btnCancel.Name = "_btnCancel";
            this._btnCancel.Size = new System.Drawing.Size(75, 23);
            this._btnCancel.TabIndex = 12;
            this._btnCancel.Text = "Cancel";
            this._btnCancel.UseVisualStyleBackColor = true;
            this._btnCancel.Click += new System.EventHandler(this._BtnNo_Click);
            // 
            // _btnAdd
            // 
            this._btnAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._btnAdd.Enabled = false;
            this._btnAdd.Location = new System.Drawing.Point(151, 154);
            this._btnAdd.Name = "_btnAdd";
            this._btnAdd.Size = new System.Drawing.Size(75, 23);
            this._btnAdd.TabIndex = 6;
            this._btnAdd.Text = "Add";
            this._btnAdd.UseVisualStyleBackColor = true;
            this._btnAdd.Click += new System.EventHandler(this._BtnAdd_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.ErrorImage = null;
            this.pictureBox1.Image = global::Mss.Views.Properties.Resources.GreenPlus48;
            this.pictureBox1.Location = new System.Drawing.Point(12, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(48, 48);
            this.pictureBox1.TabIndex = 13;
            this.pictureBox1.TabStop = false;
            // 
            // _cmbNewPalletStatus
            // 
            this._cmbNewPalletStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cmbNewPalletStatus.Enabled = false;
            this._cmbNewPalletStatus.FormattingEnabled = true;
            this._cmbNewPalletStatus.Location = new System.Drawing.Point(141, 88);
            this._cmbNewPalletStatus.Name = "_cmbNewPalletStatus";
            this._cmbNewPalletStatus.Size = new System.Drawing.Size(167, 21);
            this._cmbNewPalletStatus.TabIndex = 4;
            this._cmbNewPalletStatus.SelectedIndexChanged += new System.EventHandler(this._CmbNewPalletStatus_SelectedIndexChanged);
            // 
            // _chkOverridePalletStatus
            // 
            this._chkOverridePalletStatus.AutoSize = true;
            this._chkOverridePalletStatus.Location = new System.Drawing.Point(75, 71);
            this._chkOverridePalletStatus.Name = "_chkOverridePalletStatus";
            this._chkOverridePalletStatus.Size = new System.Drawing.Size(128, 17);
            this._chkOverridePalletStatus.TabIndex = 3;
            this._chkOverridePalletStatus.Text = "Override Pallet Status";
            this._chkOverridePalletStatus.UseVisualStyleBackColor = true;
            this._chkOverridePalletStatus.CheckedChanged += new System.EventHandler(this._ChkOverridePalletStatus_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(72, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 13);
            this.label1.TabIndex = 18;
            this.label1.Text = "Pallet ID";
            // 
            // _txtPalletID
            // 
            this._txtPalletID.Location = new System.Drawing.Point(141, 12);
            this._txtPalletID.Name = "_txtPalletID";
            this._txtPalletID.Size = new System.Drawing.Size(167, 20);
            this._txtPalletID.TabIndex = 1;
            this._txtPalletID.TextChanged += new System.EventHandler(this._TxtPalletID_TextChanged);
            // 
            // _cmbPitCode
            // 
            this._cmbPitCode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cmbPitCode.FormattingEnabled = true;
            this._cmbPitCode.Location = new System.Drawing.Point(141, 38);
            this._cmbPitCode.Name = "_cmbPitCode";
            this._cmbPitCode.Size = new System.Drawing.Size(167, 21);
            this._cmbPitCode.TabIndex = 2;
            this._cmbPitCode.SelectedIndexChanged += new System.EventHandler(this._cmbPitCode_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(72, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(52, 13);
            this.label2.TabIndex = 22;
            this.label2.Text = "PIT Code";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(72, 91);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(62, 13);
            this.label3.TabIndex = 23;
            this.label3.Text = "New Status";
            // 
            // _cmbHoldCode
            // 
            this._cmbHoldCode.DisplayMember = "Description";
            this._cmbHoldCode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cmbHoldCode.DropDownWidth = 400;
            this._cmbHoldCode.Enabled = false;
            this._cmbHoldCode.FormattingEnabled = true;
            this._cmbHoldCode.Location = new System.Drawing.Point(75, 115);
            this._cmbHoldCode.Name = "_cmbHoldCode";
            this._cmbHoldCode.Size = new System.Drawing.Size(233, 21);
            this._cmbHoldCode.TabIndex = 5;
            this._cmbHoldCode.ValueMember = "HoldCode";
            this._cmbHoldCode.SelectedIndexChanged += new System.EventHandler(this._CmbHoldCode_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 118);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(57, 13);
            this.label4.TabIndex = 23;
            this.label4.Text = "Hold Code";
            // 
            // PitAddForm
            // 
            this.AcceptButton = this._btnAdd;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this._btnCancel;
            this.ClientSize = new System.Drawing.Size(320, 189);
            this.ControlBox = false;
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this._cmbPitCode);
            this.Controls.Add(this._txtPalletID);
            this.Controls.Add(this.label1);
            this.Controls.Add(this._chkOverridePalletStatus);
            this.Controls.Add(this._cmbHoldCode);
            this.Controls.Add(this._cmbNewPalletStatus);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this._btnCancel);
            this.Controls.Add(this._btnAdd);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "PitAddForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Add PIT Pallet";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button _btnCancel;
        private System.Windows.Forms.Button _btnAdd;
        private System.Windows.Forms.ComboBox _cmbNewPalletStatus;
        private System.Windows.Forms.CheckBox _chkOverridePalletStatus;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox _txtPalletID;
        private System.Windows.Forms.ComboBox _cmbPitCode;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox _cmbHoldCode;
        private System.Windows.Forms.Label label4;
    }
}