namespace Mss.Views
{
    partial class LabelPrinterTesterView
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
            this.lstLabelPrinters = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnConnect = new System.Windows.Forms.Button();
            this.btnDisconnect = new System.Windows.Forms.Button();
            this._btnPrintShippingLabel = new System.Windows.Forms.Button();
            this._btnPrintTrailerLabel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lstLabelPrinters
            // 
            this.lstLabelPrinters.FormattingEnabled = true;
            this.lstLabelPrinters.Location = new System.Drawing.Point(16, 34);
            this.lstLabelPrinters.Name = "lstLabelPrinters";
            this.lstLabelPrinters.Size = new System.Drawing.Size(156, 82);
            this.lstLabelPrinters.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Label Printers";
            // 
            // btnConnect
            // 
            this.btnConnect.Enabled = false;
            this.btnConnect.Location = new System.Drawing.Point(16, 122);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(75, 23);
            this.btnConnect.TabIndex = 2;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this._BtnConnect_Click);
            // 
            // btnDisconnect
            // 
            this.btnDisconnect.Enabled = false;
            this.btnDisconnect.Location = new System.Drawing.Point(97, 122);
            this.btnDisconnect.Name = "btnDisconnect";
            this.btnDisconnect.Size = new System.Drawing.Size(75, 23);
            this.btnDisconnect.TabIndex = 4;
            this.btnDisconnect.Text = "Disconnect";
            this.btnDisconnect.UseVisualStyleBackColor = true;
            this.btnDisconnect.Click += new System.EventHandler(this._BtnDisconnect_Click);
            // 
            // _btnPrintShippingLabel
            // 
            this._btnPrintShippingLabel.Enabled = false;
            this._btnPrintShippingLabel.Location = new System.Drawing.Point(16, 151);
            this._btnPrintShippingLabel.Name = "_btnPrintShippingLabel";
            this._btnPrintShippingLabel.Size = new System.Drawing.Size(156, 23);
            this._btnPrintShippingLabel.TabIndex = 3;
            this._btnPrintShippingLabel.Text = "Shipping Label";
            this._btnPrintShippingLabel.UseVisualStyleBackColor = true;
            this._btnPrintShippingLabel.Click += new System.EventHandler(this._BtnPrintShippingLabel_Click);
            // 
            // _btnPrintTrailerLabel
            // 
            this._btnPrintTrailerLabel.Enabled = false;
            this._btnPrintTrailerLabel.Location = new System.Drawing.Point(16, 180);
            this._btnPrintTrailerLabel.Name = "_btnPrintTrailerLabel";
            this._btnPrintTrailerLabel.Size = new System.Drawing.Size(156, 23);
            this._btnPrintTrailerLabel.TabIndex = 3;
            this._btnPrintTrailerLabel.Text = "Trailer Label";
            this._btnPrintTrailerLabel.UseVisualStyleBackColor = true;
            this._btnPrintTrailerLabel.Click += new System.EventHandler(this._BtnPrintTrailerLabel_Click);
            // 
            // LabelPrinterTesterView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._btnPrintTrailerLabel);
            this.Controls.Add(this._btnPrintShippingLabel);
            this.Controls.Add(this.btnDisconnect);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lstLabelPrinters);
            this.Name = "LabelPrinterTesterView";
            this.Size = new System.Drawing.Size(186, 216);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox lstLabelPrinters;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Button btnDisconnect;
        private System.Windows.Forms.Button _btnPrintShippingLabel;
        private System.Windows.Forms.Button _btnPrintTrailerLabel;
    }
}
