namespace Mss.Views
{
    partial class BulkSystemSettingsEditForm
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
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this._lblMessage = new System.Windows.Forms.Label();
            this._btnCancel = new System.Windows.Forms.Button();
            this._btnEnableAll = new System.Windows.Forms.Button();
            this._btnDisableAll = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::Mss.Views.Properties.Resources.WarningSystemEvent_48x48;
            this.pictureBox1.Location = new System.Drawing.Point(12, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(48, 48);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 13;
            this.pictureBox1.TabStop = false;
            // 
            // _lblMessage
            // 
            this._lblMessage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._lblMessage.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._lblMessage.Location = new System.Drawing.Point(66, 12);
            this._lblMessage.Name = "_lblMessage";
            this._lblMessage.Size = new System.Drawing.Size(222, 48);
            this._lblMessage.TabIndex = 10;
            this._lblMessage.Text = "What would you like to do for all the Master settings?";
            this._lblMessage.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // _btnCancel
            // 
            this._btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._btnCancel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._btnCancel.Location = new System.Drawing.Point(202, 73);
            this._btnCancel.Name = "_btnCancel";
            this._btnCancel.Size = new System.Drawing.Size(75, 23);
            this._btnCancel.TabIndex = 12;
            this._btnCancel.Text = "Cancel";
            this._btnCancel.UseVisualStyleBackColor = true;
            // 
            // _btnEnableAll
            // 
            this._btnEnableAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._btnEnableAll.DialogResult = System.Windows.Forms.DialogResult.Yes;
            this._btnEnableAll.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._btnEnableAll.Location = new System.Drawing.Point(20, 73);
            this._btnEnableAll.Name = "_btnEnableAll";
            this._btnEnableAll.Size = new System.Drawing.Size(75, 23);
            this._btnEnableAll.TabIndex = 11;
            this._btnEnableAll.Text = "Enable All";
            this._btnEnableAll.UseVisualStyleBackColor = true;
            // 
            // _btnDisableAll
            // 
            this._btnDisableAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._btnDisableAll.DialogResult = System.Windows.Forms.DialogResult.No;
            this._btnDisableAll.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._btnDisableAll.Location = new System.Drawing.Point(111, 73);
            this._btnDisableAll.Name = "_btnDisableAll";
            this._btnDisableAll.Size = new System.Drawing.Size(75, 23);
            this._btnDisableAll.TabIndex = 14;
            this._btnDisableAll.Text = "Disable All";
            this._btnDisableAll.UseVisualStyleBackColor = true;
            // 
            // BulkSystemSettingsEditForm
            // 
            this.AcceptButton = this._btnEnableAll;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this._btnCancel;
            this.ClientSize = new System.Drawing.Size(300, 112);
            this.ControlBox = false;
            this.Controls.Add(this._btnDisableAll);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this._lblMessage);
            this.Controls.Add(this._btnCancel);
            this.Controls.Add(this._btnEnableAll);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "BulkSystemSettingsEditForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Confirm Bulk System Settings Change";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label _lblMessage;
        private System.Windows.Forms.Button _btnCancel;
        private System.Windows.Forms.Button _btnEnableAll;
        private System.Windows.Forms.Button _btnDisableAll;
    }
}