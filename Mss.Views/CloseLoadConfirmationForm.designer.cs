namespace Mss.Views
{
    partial class CloseLoadConfirmationForm
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
            this._btnYes = new System.Windows.Forms.Button();
            this._btnNo = new System.Windows.Forms.Button();
            this._lblMessage = new System.Windows.Forms.Label();
            this._pbAction = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this._pbAction)).BeginInit();
            this.SuspendLayout();
            // 
            // _btnYes
            // 
            this._btnYes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._btnYes.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._btnYes.Location = new System.Drawing.Point(176, 69);
            this._btnYes.Name = "_btnYes";
            this._btnYes.Size = new System.Drawing.Size(75, 23);
            this._btnYes.TabIndex = 2;
            this._btnYes.Text = "Yes";
            this._btnYes.UseVisualStyleBackColor = true;
            this._btnYes.Click += new System.EventHandler(this._BtnYes_Click);
            // 
            // _btnNo
            // 
            this._btnNo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._btnNo.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._btnNo.Font = new System.Drawing.Font("Segoe UI", 9F);
            this._btnNo.Location = new System.Drawing.Point(257, 69);
            this._btnNo.Name = "_btnNo";
            this._btnNo.Size = new System.Drawing.Size(75, 23);
            this._btnNo.TabIndex = 3;
            this._btnNo.Text = "No";
            this._btnNo.UseVisualStyleBackColor = true;
            this._btnNo.Click += new System.EventHandler(this._BtnNo_Click);
            // 
            // _lblMessage
            // 
            this._lblMessage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._lblMessage.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._lblMessage.Location = new System.Drawing.Point(77, 11);
            this._lblMessage.Name = "_lblMessage";
            this._lblMessage.Size = new System.Drawing.Size(255, 48);
            this._lblMessage.TabIndex = 0;
            this._lblMessage.Text = "Are you certain that you want to CLOSE the Load on Slug X?";
            this._lblMessage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // _pbAction
            // 
            this._pbAction.Image = global::Mss.Views.Properties.Resources.RedMinus48;
            this._pbAction.Location = new System.Drawing.Point(13, 11);
            this._pbAction.Name = "_pbAction";
            this._pbAction.Size = new System.Drawing.Size(48, 48);
            this._pbAction.TabIndex = 4;
            this._pbAction.TabStop = false;
            // 
            // CloseLoadConfirmationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this._btnNo;
            this.ClientSize = new System.Drawing.Size(344, 104);
            this.Controls.Add(this._pbAction);
            this.Controls.Add(this._lblMessage);
            this.Controls.Add(this._btnNo);
            this.Controls.Add(this._btnYes);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CloseLoadConfirmationForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Confirm Close Load";
            ((System.ComponentModel.ISupportInitialize)(this._pbAction)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button _btnYes;
        private System.Windows.Forms.Button _btnNo;
        private System.Windows.Forms.Label _lblMessage;
        private System.Windows.Forms.PictureBox _pbAction;
    }
}