namespace Mss.Views
{
    partial class SystemSettingsView
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
            this._cmbMaxAuditAttempts = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this._lblAutoAcceptLoads = new System.Windows.Forms.Label();
            this._cmbFifoMode = new System.Windows.Forms.ComboBox();
            this._btnAutoAcceptLoads = new System.Windows.Forms.Button();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this._lblFifoMode = new System.Windows.Forms.Label();
            this._lblAuditAttempts = new System.Windows.Forms.Label();
            this.navigator = new System.Windows.Forms.ToolStrip();
            this.navigatorLblTitle = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.navigatorBtnRefreshItem = new System.Windows.Forms.ToolStripButton();
            this._btnLoadBEnabled = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this._lblLoadBEnabled = new System.Windows.Forms.Label();
            this._btnLoadAEnabled = new System.Windows.Forms.Button();
            this._lblLoadAEnabled = new System.Windows.Forms.Label();
            this._lblPreferredSlug = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this._cmbPreferredSlug = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this._lblSlugPickPriority = new System.Windows.Forms.Label();
            this._cmbSlugPickPriority = new System.Windows.Forms.ComboBox();
            this._craneFunctionGrid = new Mss.Views.CraneFunctionGrid();
            this.navigator.SuspendLayout();
            this.SuspendLayout();
            // 
            // _cmbMaxAuditAttempts
            // 
            this._cmbMaxAuditAttempts.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cmbMaxAuditAttempts.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._cmbMaxAuditAttempts.FormattingEnabled = true;
            this._cmbMaxAuditAttempts.Location = new System.Drawing.Point(125, 387);
            this._cmbMaxAuditAttempts.Name = "_cmbMaxAuditAttempts";
            this._cmbMaxAuditAttempts.Size = new System.Drawing.Size(99, 23);
            this._cmbMaxAuditAttempts.TabIndex = 25;
            this._cmbMaxAuditAttempts.Visible = false;
            this._cmbMaxAuditAttempts.SelectedIndexChanged += new System.EventHandler(this._CmbMaxAuditAttempts_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 152);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(98, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Auto Accept Loads";
            // 
            // _lblAutoAcceptLoads
            // 
            this._lblAutoAcceptLoads.BackColor = System.Drawing.Color.Gray;
            this._lblAutoAcceptLoads.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._lblAutoAcceptLoads.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._lblAutoAcceptLoads.ForeColor = System.Drawing.Color.White;
            this._lblAutoAcceptLoads.Location = new System.Drawing.Point(143, 146);
            this._lblAutoAcceptLoads.Name = "_lblAutoAcceptLoads";
            this._lblAutoAcceptLoads.Size = new System.Drawing.Size(117, 23);
            this._lblAutoAcceptLoads.TabIndex = 29;
            this._lblAutoAcceptLoads.Text = "Disabled";
            this._lblAutoAcceptLoads.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // _cmbFifoMode
            // 
            this._cmbFifoMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cmbFifoMode.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._cmbFifoMode.FormattingEnabled = true;
            this._cmbFifoMode.Location = new System.Drawing.Point(143, 177);
            this._cmbFifoMode.Name = "_cmbFifoMode";
            this._cmbFifoMode.Size = new System.Drawing.Size(117, 23);
            this._cmbFifoMode.TabIndex = 23;
            this._cmbFifoMode.SelectedIndexChanged += new System.EventHandler(this._CmbFifoMode_SelectedIndexChanged);
            // 
            // _btnAutoAcceptLoads
            // 
            this._btnAutoAcceptLoads.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._btnAutoAcceptLoads.Location = new System.Drawing.Point(143, 146);
            this._btnAutoAcceptLoads.Name = "_btnAutoAcceptLoads";
            this._btnAutoAcceptLoads.Size = new System.Drawing.Size(117, 23);
            this._btnAutoAcceptLoads.TabIndex = 9;
            this._btnAutoAcceptLoads.Text = "Enable";
            this._btnAutoAcceptLoads.UseVisualStyleBackColor = true;
            this._btnAutoAcceptLoads.Click += new System.EventHandler(this._BtnAutoAcceptLoads_Click);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(21, 393);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(98, 13);
            this.label13.TabIndex = 24;
            this.label13.Text = "Max Audit Attempts";
            this.label13.Visible = false;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(10, 183);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(60, 13);
            this.label14.TabIndex = 22;
            this.label14.Text = "FIFO Mode";
            // 
            // _lblFifoMode
            // 
            this._lblFifoMode.BackColor = System.Drawing.SystemColors.Window;
            this._lblFifoMode.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._lblFifoMode.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._lblFifoMode.Location = new System.Drawing.Point(143, 177);
            this._lblFifoMode.Name = "_lblFifoMode";
            this._lblFifoMode.Size = new System.Drawing.Size(117, 23);
            this._lblFifoMode.TabIndex = 32;
            this._lblFifoMode.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // _lblAuditAttempts
            // 
            this._lblAuditAttempts.BackColor = System.Drawing.SystemColors.Window;
            this._lblAuditAttempts.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._lblAuditAttempts.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._lblAuditAttempts.Location = new System.Drawing.Point(195, 387);
            this._lblAuditAttempts.Name = "_lblAuditAttempts";
            this._lblAuditAttempts.Size = new System.Drawing.Size(29, 23);
            this._lblAuditAttempts.TabIndex = 33;
            this._lblAuditAttempts.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this._lblAuditAttempts.Visible = false;
            // 
            // navigator
            // 
            this.navigator.BackColor = System.Drawing.SystemColors.Control;
            this.navigator.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.navigator.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.navigator.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.navigatorLblTitle,
            this.toolStripSeparator1,
            this.navigatorBtnRefreshItem});
            this.navigator.Location = new System.Drawing.Point(0, 0);
            this.navigator.Name = "navigator";
            this.navigator.Padding = new System.Windows.Forms.Padding(5, 2, 5, 2);
            this.navigator.Size = new System.Drawing.Size(746, 35);
            this.navigator.TabIndex = 34;
            // 
            // navigatorLblTitle
            // 
            this.navigatorLblTitle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.navigatorLblTitle.Name = "navigatorLblTitle";
            this.navigatorLblTitle.Size = new System.Drawing.Size(97, 28);
            this.navigatorLblTitle.Text = "System Settings";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 31);
            // 
            // navigatorBtnRefreshItem
            // 
            this.navigatorBtnRefreshItem.Image = global::Mss.Views.Properties.Resources.GreenRefresh24;
            this.navigatorBtnRefreshItem.Name = "navigatorBtnRefreshItem";
            this.navigatorBtnRefreshItem.Size = new System.Drawing.Size(74, 28);
            this.navigatorBtnRefreshItem.Text = "Refresh";
            this.navigatorBtnRefreshItem.ToolTipText = "Refresh";
            this.navigatorBtnRefreshItem.Click += new System.EventHandler(this._NavigatorBtnRefreshItem_Click);
            // 
            // _btnLoadBEnabled
            // 
            this._btnLoadBEnabled.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._btnLoadBEnabled.Location = new System.Drawing.Point(143, 103);
            this._btnLoadBEnabled.Name = "_btnLoadBEnabled";
            this._btnLoadBEnabled.Size = new System.Drawing.Size(117, 23);
            this._btnLoadBEnabled.TabIndex = 39;
            this._btnLoadBEnabled.Text = "Enable";
            this._btnLoadBEnabled.UseVisualStyleBackColor = true;
            this._btnLoadBEnabled.Click += new System.EventHandler(this._BtnLoadBEnabled_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(10, 80);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(83, 13);
            this.label4.TabIndex = 36;
            this.label4.Text = "Load A Enabled";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(10, 109);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(83, 13);
            this.label5.TabIndex = 38;
            this.label5.Text = "Load B Enabled";
            // 
            // _lblLoadBEnabled
            // 
            this._lblLoadBEnabled.BackColor = System.Drawing.Color.Gray;
            this._lblLoadBEnabled.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._lblLoadBEnabled.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._lblLoadBEnabled.ForeColor = System.Drawing.Color.White;
            this._lblLoadBEnabled.Location = new System.Drawing.Point(143, 103);
            this._lblLoadBEnabled.Name = "_lblLoadBEnabled";
            this._lblLoadBEnabled.Size = new System.Drawing.Size(117, 23);
            this._lblLoadBEnabled.TabIndex = 42;
            this._lblLoadBEnabled.Text = "Disabled";
            this._lblLoadBEnabled.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // _btnLoadAEnabled
            // 
            this._btnLoadAEnabled.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._btnLoadAEnabled.Location = new System.Drawing.Point(143, 74);
            this._btnLoadAEnabled.Name = "_btnLoadAEnabled";
            this._btnLoadAEnabled.Size = new System.Drawing.Size(118, 23);
            this._btnLoadAEnabled.TabIndex = 37;
            this._btnLoadAEnabled.Text = "Enable";
            this._btnLoadAEnabled.UseVisualStyleBackColor = true;
            this._btnLoadAEnabled.Click += new System.EventHandler(this._BtnLoadAEnabled_Click);
            // 
            // _lblLoadAEnabled
            // 
            this._lblLoadAEnabled.BackColor = System.Drawing.Color.Gray;
            this._lblLoadAEnabled.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._lblLoadAEnabled.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._lblLoadAEnabled.ForeColor = System.Drawing.Color.White;
            this._lblLoadAEnabled.Location = new System.Drawing.Point(143, 74);
            this._lblLoadAEnabled.Name = "_lblLoadAEnabled";
            this._lblLoadAEnabled.Size = new System.Drawing.Size(117, 23);
            this._lblLoadAEnabled.TabIndex = 44;
            this._lblLoadAEnabled.Text = "Disabled";
            this._lblLoadAEnabled.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // _lblPreferredLoad
            // 
            this._lblPreferredSlug.BackColor = System.Drawing.SystemColors.Window;
            this._lblPreferredSlug.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._lblPreferredSlug.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._lblPreferredSlug.Location = new System.Drawing.Point(161, 45);
            this._lblPreferredSlug.Name = "_lblPreferredLoad";
            this._lblPreferredSlug.Size = new System.Drawing.Size(99, 23);
            this._lblPreferredSlug.TabIndex = 32;
            this._lblPreferredSlug.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(10, 51);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(77, 13);
            this.label10.TabIndex = 22;
            this.label10.Text = "Preferred Load";
            // 
            // _cmbPreferredLoad
            // 
            this._cmbPreferredSlug.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cmbPreferredSlug.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._cmbPreferredSlug.FormattingEnabled = true;
            this._cmbPreferredSlug.Location = new System.Drawing.Point(143, 45);
            this._cmbPreferredSlug.Name = "_cmbPreferredLoad";
            this._cmbPreferredSlug.Size = new System.Drawing.Size(117, 23);
            this._cmbPreferredSlug.TabIndex = 23;
            this._cmbPreferredSlug.SelectedIndexChanged += new System.EventHandler(this._CmbPreferredLoad_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 214);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(89, 13);
            this.label3.TabIndex = 45;
            this.label3.Text = "Load Pick Priority";
            // 
            // _lblLoadPickPriority
            // 
            this._lblSlugPickPriority.BackColor = System.Drawing.SystemColors.Window;
            this._lblSlugPickPriority.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._lblSlugPickPriority.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._lblSlugPickPriority.Location = new System.Drawing.Point(143, 209);
            this._lblSlugPickPriority.Name = "_lblLoadPickPriority";
            this._lblSlugPickPriority.Size = new System.Drawing.Size(117, 23);
            this._lblSlugPickPriority.TabIndex = 32;
            this._lblSlugPickPriority.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // _cmbLoadPickPriority
            // 
            this._cmbSlugPickPriority.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cmbSlugPickPriority.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._cmbSlugPickPriority.FormattingEnabled = true;
            this._cmbSlugPickPriority.Location = new System.Drawing.Point(143, 209);
            this._cmbSlugPickPriority.Name = "_cmbLoadPickPriority";
            this._cmbSlugPickPriority.Size = new System.Drawing.Size(117, 23);
            this._cmbSlugPickPriority.TabIndex = 23;
            this._cmbSlugPickPriority.SelectedIndexChanged += new System.EventHandler(this._CmbLoadPickPriority_SelectedIndexChanged);
            // 
            // _craneFunctionGrid
            // 
            this._craneFunctionGrid.EnableSort = true;
            this._craneFunctionGrid.Location = new System.Drawing.Point(282, 45);
            this._craneFunctionGrid.Name = "_craneFunctionGrid";
            this._craneFunctionGrid.OptimizeMode = SourceGrid.CellOptimizeMode.ForRows;
            this._craneFunctionGrid.SelectionMode = SourceGrid.GridSelectionMode.Cell;
            this._craneFunctionGrid.Size = new System.Drawing.Size(450, 427);
            this._craneFunctionGrid.TabIndex = 27;
            this._craneFunctionGrid.TabStop = true;
            this._craneFunctionGrid.ToolTipText = "";
            // 
            // SystemSettingsView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label3);
            this.Controls.Add(this._btnLoadBEnabled);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label5);
            this.Controls.Add(this._lblLoadBEnabled);
            this.Controls.Add(this._btnLoadAEnabled);
            this.Controls.Add(this._lblLoadAEnabled);
            this.Controls.Add(this.navigator);
            this.Controls.Add(this._cmbMaxAuditAttempts);
            this.Controls.Add(this._cmbPreferredSlug);
            this.Controls.Add(this._cmbSlugPickPriority);
            this.Controls.Add(this._cmbFifoMode);
            this.Controls.Add(this._btnAutoAcceptLoads);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.label2);
            this.Controls.Add(this._craneFunctionGrid);
            this.Controls.Add(this._lblAutoAcceptLoads);
            this.Controls.Add(this._lblSlugPickPriority);
            this.Controls.Add(this._lblPreferredSlug);
            this.Controls.Add(this._lblFifoMode);
            this.Controls.Add(this._lblAuditAttempts);
            this.Name = "SystemSettingsView";
            this.Size = new System.Drawing.Size(746, 485);
            this.Load += new System.EventHandler(this.SystemSettingsView_Load);
            this.navigator.ResumeLayout(false);
            this.navigator.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ComboBox _cmbMaxAuditAttempts;
        private CraneFunctionGrid _craneFunctionGrid;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label _lblAutoAcceptLoads;
        private System.Windows.Forms.ComboBox _cmbFifoMode;
        private System.Windows.Forms.Button _btnAutoAcceptLoads;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label _lblFifoMode;
        private System.Windows.Forms.Label _lblAuditAttempts;
        protected System.Windows.Forms.ToolStrip navigator;
        private System.Windows.Forms.ToolStripLabel navigatorLblTitle;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        protected System.Windows.Forms.ToolStripButton navigatorBtnRefreshItem;
        private System.Windows.Forms.Button _btnLoadBEnabled;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label _lblLoadBEnabled;
        private System.Windows.Forms.Button _btnLoadAEnabled;
        private System.Windows.Forms.Label _lblLoadAEnabled;
//         private System.Windows.Forms.Button _btnAutoReleaseLoadsEnabled;
//         private System.Windows.Forms.Label _lblAutoReleasLoadsEnabled;
        private System.Windows.Forms.Label _lblPreferredSlug;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.ComboBox _cmbPreferredSlug;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label _lblSlugPickPriority;
        private System.Windows.Forms.ComboBox _cmbSlugPickPriority;
    }
}
