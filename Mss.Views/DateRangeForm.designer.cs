namespace Mss.Views
{
    partial class DateRangeForm
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
            this.dtStart = new VisualHint.SmartFieldPackEditor.DateTimePack.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.dtEnd = new VisualHint.SmartFieldPackEditor.DateTimePack.DateTimePicker();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dtStart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtEnd)).BeginInit();
            this.SuspendLayout();
            // 
            // dtStart
            // 
            this.dtStart.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dtStart.CalendarFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dtStart.CustomFormat = "dd MMM yyy  HH:mm:ss ";
            this.dtStart.FixedWidth = false;
            this.dtStart.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtStart.Location = new System.Drawing.Point(101, 9);
            this.dtStart.MaxDate = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
            this.dtStart.MinDate = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
            this.dtStart.Name = "dtStart";
            this.dtStart.Offset = new System.Drawing.Size(3, 0);
            this.dtStart.ShowUpDown = true;
            this.dtStart.Size = new System.Drawing.Size(184, 20);
            this.dtStart.TabIndex = 1;
            this.dtStart.TabKeyNavigationMode = VisualHint.SmartFieldPackEditor.FieldPackEditor.TabKeyNavigationModes.EachField;
            this.dtStart.Value = new System.DateTime(2011, 12, 11, 20, 50, 58, 0);
            this.dtStart.ValueChanged += new System.EventHandler(this.dtStart_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Start Date/Time";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 39);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(80, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "End Date/Time";
            // 
            // dtEnd
            // 
            this.dtEnd.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dtEnd.CalendarFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dtEnd.CustomFormat = "dd MMM yyy  HH:mm:ss";
            this.dtEnd.FixedWidth = false;
            this.dtEnd.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtEnd.Location = new System.Drawing.Point(101, 35);
            this.dtEnd.MaxDate = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
            this.dtEnd.MinDate = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
            this.dtEnd.Name = "dtEnd";
            this.dtEnd.Offset = new System.Drawing.Size(3, 0);
            this.dtEnd.ShowUpDown = true;
            this.dtEnd.Size = new System.Drawing.Size(184, 20);
            this.dtEnd.TabIndex = 3;
            this.dtEnd.TabKeyNavigationMode = VisualHint.SmartFieldPackEditor.FieldPackEditor.TabKeyNavigationModes.EachField;
            this.dtEnd.Value = new System.DateTime(2011, 12, 11, 20, 50, 58, 0);
            this.dtEnd.ValueChanged += new System.EventHandler(this.dtEnd_ValueChanged);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Enabled = false;
            this.btnOK.Location = new System.Drawing.Point(129, 65);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 4;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(210, 65);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // DateRangeForm
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(297, 100);
            this.ControlBox = false;
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dtEnd);
            this.Controls.Add(this.dtStart);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "DateRangeForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Select Date Range";
            ((System.ComponentModel.ISupportInitialize)(this.dtStart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtEnd)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private VisualHint.SmartFieldPackEditor.DateTimePack.DateTimePicker dtStart;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private VisualHint.SmartFieldPackEditor.DateTimePack.DateTimePicker dtEnd;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
    }
}