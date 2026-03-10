namespace Mss.ShippingService
{
    partial class ServiceHostForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ServiceHostForm));
            this._btnShutDown = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // _btnShutDown
            // 
            this._btnShutDown.BackColor = System.Drawing.SystemColors.Window;
            this._btnShutDown.Location = new System.Drawing.Point(208, 254);
            this._btnShutDown.Name = "_btnShutDown";
            this._btnShutDown.Size = new System.Drawing.Size(160, 32);
            this._btnShutDown.TabIndex = 2;
            this._btnShutDown.Text = "Shut Down Shipping Service";
            this._btnShutDown.UseVisualStyleBackColor = false;
            this._btnShutDown.Click += new System.EventHandler(this._BtnShutDown_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::Mss.ShippingService.Properties.Resources.MagnaSeatingLogo2;
            this.pictureBox1.Location = new System.Drawing.Point(12, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(551, 236);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // ServiceHostForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(577, 295);
            this.Controls.Add(this._btnShutDown);
            this.Controls.Add(this.pictureBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "ServiceHostForm";
            this.Text = "Mississauga Seating Shipping Service";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this._ServiceHostForm_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button _btnShutDown;
    }
}

