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
            this._btnPrintShipping = new System.Windows.Forms.Button();
            this._btnPrintLear = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
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
            // _btnPrintShipping
            // 
            this._btnPrintShipping.Enabled = false;
            this._btnPrintShipping.Location = new System.Drawing.Point(16, 151);
            this._btnPrintShipping.Name = "_btnPrintShipping";
            this._btnPrintShipping.Size = new System.Drawing.Size(156, 23);
            this._btnPrintShipping.TabIndex = 3;
            this._btnPrintShipping.Text = "Shipping Label";
            this._btnPrintShipping.UseVisualStyleBackColor = true;
            this._btnPrintShipping.Click += new System.EventHandler(this._BtnPrintShipping_Click);
            // 
            // _btnPrintLear
            // 
            this._btnPrintLear.Enabled = false;
            this._btnPrintLear.Location = new System.Drawing.Point(16, 180);
            this._btnPrintLear.Name = "_btnPrintLear";
            this._btnPrintLear.Size = new System.Drawing.Size(156, 23);
            this._btnPrintLear.TabIndex = 3;
            this._btnPrintLear.Text = "Lear Label";
            this._btnPrintLear.UseVisualStyleBackColor = true;
            this._btnPrintLear.Click += new System.EventHandler(this._BtnPrintLear_Click);
            // 
            // button1
            // 
            this.button1.Enabled = false;
            this.button1.Location = new System.Drawing.Point(93, 5);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 5;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Visible = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // LabelPrinterTesterView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.button1);
            this.Controls.Add(this._btnPrintLear);
            this.Controls.Add(this._btnPrintShipping);
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
        private System.Windows.Forms.Button _btnPrintShipping;
        private System.Windows.Forms.Button _btnPrintLear;
        private System.Windows.Forms.Button button1;
    }
}
