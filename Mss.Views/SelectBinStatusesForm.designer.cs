namespace Mss.Views
{
    partial class SelectBinStatusesForm
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
            this.chkEmpty = new System.Windows.Forms.CheckBox();
            this.chkPickable = new System.Windows.Forms.CheckBox();
            this.chkPutAllocated = new System.Windows.Forms.CheckBox();
            this.chkGetAllocated = new System.Windows.Forms.CheckBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.chkOffline = new System.Windows.Forms.CheckBox();
            this._chkOfflineDuplicate = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // chkEmpty
            // 
            this.chkEmpty.AutoSize = true;
            this.chkEmpty.Location = new System.Drawing.Point(12, 12);
            this.chkEmpty.Name = "chkEmpty";
            this.chkEmpty.Size = new System.Drawing.Size(55, 17);
            this.chkEmpty.TabIndex = 0;
            this.chkEmpty.Text = "Empty";
            this.chkEmpty.UseVisualStyleBackColor = true;
            this.chkEmpty.CheckedChanged += new System.EventHandler(this._ChkEmpty_CheckedChanged);
            // 
            // chkPickable
            // 
            this.chkPickable.AutoSize = true;
            this.chkPickable.Location = new System.Drawing.Point(123, 12);
            this.chkPickable.Name = "chkPickable";
            this.chkPickable.Size = new System.Drawing.Size(67, 17);
            this.chkPickable.TabIndex = 1;
            this.chkPickable.Text = "Pickable";
            this.chkPickable.UseVisualStyleBackColor = true;
            this.chkPickable.CheckedChanged += new System.EventHandler(this._ChkPickable_CheckedChanged);
            // 
            // chkPutAllocated
            // 
            this.chkPutAllocated.AutoSize = true;
            this.chkPutAllocated.Location = new System.Drawing.Point(123, 35);
            this.chkPutAllocated.Name = "chkPutAllocated";
            this.chkPutAllocated.Size = new System.Drawing.Size(89, 17);
            this.chkPutAllocated.TabIndex = 4;
            this.chkPutAllocated.Text = "Put Allocated";
            this.chkPutAllocated.UseVisualStyleBackColor = true;
            this.chkPutAllocated.CheckedChanged += new System.EventHandler(this._ChkPutAllocated_CheckedChanged);
            // 
            // chkGetAllocated
            // 
            this.chkGetAllocated.AutoSize = true;
            this.chkGetAllocated.Location = new System.Drawing.Point(12, 35);
            this.chkGetAllocated.Name = "chkGetAllocated";
            this.chkGetAllocated.Size = new System.Drawing.Size(90, 17);
            this.chkGetAllocated.TabIndex = 3;
            this.chkGetAllocated.Text = "Get Allocated";
            this.chkGetAllocated.UseVisualStyleBackColor = true;
            this.chkGetAllocated.CheckedChanged += new System.EventHandler(this._ChkGetAllocated_CheckedChanged);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(127, 90);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Enabled = false;
            this.btnOK.Location = new System.Drawing.Point(29, 90);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 6;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // chkOffline
            // 
            this.chkOffline.AutoSize = true;
            this.chkOffline.Location = new System.Drawing.Point(12, 58);
            this.chkOffline.Name = "chkOffline";
            this.chkOffline.Size = new System.Drawing.Size(56, 17);
            this.chkOffline.TabIndex = 3;
            this.chkOffline.Text = "Offline";
            this.chkOffline.UseVisualStyleBackColor = true;
            this.chkOffline.CheckedChanged += new System.EventHandler(this._ChkOffline_CheckedChanged);
            // 
            // _chkOfflineDuplicate
            // 
            this._chkOfflineDuplicate.AutoSize = true;
            this._chkOfflineDuplicate.Location = new System.Drawing.Point(123, 58);
            this._chkOfflineDuplicate.Name = "_chkOfflineDuplicate";
            this._chkOfflineDuplicate.Size = new System.Drawing.Size(104, 17);
            this._chkOfflineDuplicate.TabIndex = 3;
            this._chkOfflineDuplicate.Text = "Offline Duplicate";
            this._chkOfflineDuplicate.UseVisualStyleBackColor = true;
            this._chkOfflineDuplicate.CheckedChanged += new System.EventHandler(this._ChkOfflineDuplicate_CheckedChanged);
            // 
            // SelectBinStatusesForm
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(231, 125);
            this.ControlBox = false;
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this._chkOfflineDuplicate);
            this.Controls.Add(this.chkOffline);
            this.Controls.Add(this.chkGetAllocated);
            this.Controls.Add(this.chkPutAllocated);
            this.Controls.Add(this.chkPickable);
            this.Controls.Add(this.chkEmpty);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "SelectBinStatusesForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Select Bin Statuses";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox chkEmpty;
        private System.Windows.Forms.CheckBox chkPickable;
        private System.Windows.Forms.CheckBox chkPutAllocated;
        private System.Windows.Forms.CheckBox chkGetAllocated;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.CheckBox chkOffline;
        private System.Windows.Forms.CheckBox _chkOfflineDuplicate;
    }
}