namespace Mss.Views
{
    partial class PlcTagWriteForm
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
            this._txtNewTagValue = new System.Windows.Forms.TextBox();
            this._btnWriteTag = new System.Windows.Forms.Button();
            this._btnCancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // _txtNewTagValue
            // 
            this._txtNewTagValue.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._txtNewTagValue.Location = new System.Drawing.Point(12, 29);
            this._txtNewTagValue.Name = "_txtNewTagValue";
            this._txtNewTagValue.Size = new System.Drawing.Size(156, 26);
            this._txtNewTagValue.TabIndex = 0;
            this._txtNewTagValue.TextChanged += new System.EventHandler(this._TxtNewTagValue_TextChanged);
            // 
            // _btnWriteTag
            // 
            this._btnWriteTag.Location = new System.Drawing.Point(12, 61);
            this._btnWriteTag.Name = "_btnWriteTag";
            this._btnWriteTag.Size = new System.Drawing.Size(75, 23);
            this._btnWriteTag.TabIndex = 1;
            this._btnWriteTag.Text = "Write Tag";
            this._btnWriteTag.UseVisualStyleBackColor = true;
            this._btnWriteTag.Click += new System.EventHandler(this._BtnWriteTag_Click);
            // 
            // _btnCancel
            // 
            this._btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._btnCancel.Location = new System.Drawing.Point(93, 61);
            this._btnCancel.Name = "_btnCancel";
            this._btnCancel.Size = new System.Drawing.Size(75, 23);
            this._btnCancel.TabIndex = 2;
            this._btnCancel.Text = "Cancel";
            this._btnCancel.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(81, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "New Tag Value";
            // 
            // PlcTagWriteForm
            // 
            this.AcceptButton = this._btnWriteTag;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this._btnCancel;
            this.ClientSize = new System.Drawing.Size(182, 94);
            this.ControlBox = false;
            this.Controls.Add(this.label1);
            this.Controls.Add(this._btnCancel);
            this.Controls.Add(this._btnWriteTag);
            this.Controls.Add(this._txtNewTagValue);
            this.Name = "PlcTagWriteForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Write Tag";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox _txtNewTagValue;
        private System.Windows.Forms.Button _btnWriteTag;
        private System.Windows.Forms.Button _btnCancel;
        private System.Windows.Forms.Label label1;
    }
}