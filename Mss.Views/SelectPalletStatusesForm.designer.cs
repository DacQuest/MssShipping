namespace Mss.Views
{
    partial class SelectPalletStatusesForm
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
            this._chkOK = new System.Windows.Forms.CheckBox();
            this._chkHold = new System.Windows.Forms.CheckBox();
            this._chkQAPick = new System.Windows.Forms.CheckBox();
            this._chkUnknown = new System.Windows.Forms.CheckBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // _chkOK
            // 
            this._chkOK.AutoSize = true;
            this._chkOK.Location = new System.Drawing.Point(17, 12);
            this._chkOK.Name = "_chkOK";
            this._chkOK.Size = new System.Drawing.Size(41, 17);
            this._chkOK.TabIndex = 0;
            this._chkOK.Text = "OK";
            this._chkOK.UseVisualStyleBackColor = true;
            this._chkOK.CheckedChanged += new System.EventHandler(this._ChkOK_CheckedChanged);
            // 
            // _chkHold
            // 
            this._chkHold.AutoSize = true;
            this._chkHold.Location = new System.Drawing.Point(94, 12);
            this._chkHold.Name = "_chkHold";
            this._chkHold.Size = new System.Drawing.Size(48, 17);
            this._chkHold.TabIndex = 1;
            this._chkHold.Text = "Hold";
            this._chkHold.UseVisualStyleBackColor = true;
            this._chkHold.CheckedChanged += new System.EventHandler(this._ChkHold_CheckedChanged);
            // 
            // _chkQCSort
            // 
            //this._chkQAPick.AutoSize = true;
            //this._chkQAPick.Location = new System.Drawing.Point(17, 35);
            //this._chkQAPick.Name = "_chkQCSort";
            //this._chkQAPick.Size = new System.Drawing.Size(63, 17);
            //this._chkQAPick.TabIndex = 4;
            //this._chkQAPick.Text = "QC Sort";
            //this._chkQAPick.UseVisualStyleBackColor = true;
            //this._chkQAPick.CheckedChanged += new System.EventHandler(this._ChkQCSort_CheckedChanged);
            // 
            // _chkUnknown
            // 
            this._chkUnknown.AutoSize = true;
            this._chkUnknown.Location = new System.Drawing.Point(94, 35);
            this._chkUnknown.Name = "_chkUnknown";
            this._chkUnknown.Size = new System.Drawing.Size(72, 17);
            this._chkUnknown.TabIndex = 5;
            this._chkUnknown.Text = "Unknown";
            this._chkUnknown.UseVisualStyleBackColor = true;
            this._chkUnknown.CheckedChanged += new System.EventHandler(this._ChkUnknown_CheckedChanged);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(94, 65);
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
            this.btnOK.Location = new System.Drawing.Point(12, 65);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 6;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // SelectPalletStatusesForm
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(181, 100);
            this.ControlBox = false;
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this._chkUnknown);
            this.Controls.Add(this._chkQAPick);
            this.Controls.Add(this._chkHold);
            this.Controls.Add(this._chkOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "SelectPalletStatusesForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Select Pallet Statuses";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox _chkOK;
        private System.Windows.Forms.CheckBox _chkHold;
        private System.Windows.Forms.CheckBox _chkQAPick;
        //        private System.Windows.Forms.CheckBox chkMissingData;
        private System.Windows.Forms.CheckBox _chkUnknown;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
    }
}