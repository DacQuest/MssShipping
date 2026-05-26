namespace Mss.Views
{
    partial class SlugsView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SlugsView));
            this._lblLoadBNumber = new System.Windows.Forms.Label();
            this._lblLoadANumber = new System.Windows.Forms.Label();
            this._slugBGridLower = new Mss.Views.SlugGrid();
            this.label1 = new System.Windows.Forms.Label();
            this._btnAbortLoadB = new System.Windows.Forms.Button();
            this._btnAbortLoadA = new System.Windows.Forms.Button();
            this._btnAcceptLoadB = new System.Windows.Forms.Button();
            this._btnAcceptLoadA = new System.Windows.Forms.Button();
            this._lblSlugBName = new System.Windows.Forms.Label();
            this._lblSlugAName = new System.Windows.Forms.Label();
            this._slugAGridUpper = new Mss.Views.SlugGrid();
            this.navigator = new System.Windows.Forms.ToolStrip();
            this.navigatorLegend = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this._navigatorBtnRefreshItem = new System.Windows.Forms.ToolStripButton();
            this._navigatorSepReleaseNewLoad = new System.Windows.Forms.ToolStripSeparator();
            this._navigatorBtnReleaseBroadcast = new System.Windows.Forms.ToolStripButton();
            this._slugAGridLower = new Mss.Views.SlugGrid();
            this._slugBGridUpper = new Mss.Views.SlugGrid();
            this.label3 = new System.Windows.Forms.Label();
            this._btnCloseReopenLoadA = new System.Windows.Forms.Button();
            this._btnCloseReopenLoadB = new System.Windows.Forms.Button();
            this.navigator.SuspendLayout();
            this.SuspendLayout();
            // 
            // _lblLoadBNumber
            // 
            this._lblLoadBNumber.AutoSize = true;
            this._lblLoadBNumber.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this._lblLoadBNumber.Location = new System.Drawing.Point(1112, 50);
            this._lblLoadBNumber.Name = "_lblLoadBNumber";
            this._lblLoadBNumber.Size = new System.Drawing.Size(26, 30);
            this._lblLoadBNumber.TabIndex = 10;
            this._lblLoadBNumber.Text = "0";
            // 
            // _lblLoadANumber
            // 
            this._lblLoadANumber.AutoSize = true;
            this._lblLoadANumber.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this._lblLoadANumber.Location = new System.Drawing.Point(300, 50);
            this._lblLoadANumber.Name = "_lblLoadANumber";
            this._lblLoadANumber.Size = new System.Drawing.Size(26, 30);
            this._lblLoadANumber.TabIndex = 8;
            this._lblLoadANumber.Text = "0";
            // 
            // _slugBGridLower
            // 
            this._slugBGridLower.EnableSort = true;
            this._slugBGridLower.Location = new System.Drawing.Point(822, 495);
            this._slugBGridLower.Name = "_slugBGridLower";
            this._slugBGridLower.OptimizeMode = SourceGrid.CellOptimizeMode.ForRows;
            this._slugBGridLower.SelectionMode = SourceGrid.GridSelectionMode.Cell;
            this._slugBGridLower.Size = new System.Drawing.Size(797, 406);
            this._slugBGridLower.TabIndex = 2;
            this._slugBGridLower.TabStop = true;
            this._slugBGridLower.ToolTipText = "";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.label1.Location = new System.Drawing.Point(957, 50);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(161, 30);
            this.label1.TabIndex = 7;
            this.label1.Text = "Load Number:";
            // 
            // _btnAbortLoadB
            // 
            this._btnAbortLoadB.Enabled = false;
            this._btnAbortLoadB.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this._btnAbortLoadB.Image = global::Mss.Views.Properties.Resources.RedWhiteProhibited24;
            this._btnAbortLoadB.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this._btnAbortLoadB.Location = new System.Drawing.Point(1247, 47);
            this._btnAbortLoadB.Name = "_btnAbortLoadB";
            this._btnAbortLoadB.Size = new System.Drawing.Size(120, 30);
            this._btnAbortLoadB.TabIndex = 6;
            this._btnAbortLoadB.Text = "Abort Load B ";
            this._btnAbortLoadB.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this._btnAbortLoadB.UseVisualStyleBackColor = true;
            this._btnAbortLoadB.Visible = false;
            this._btnAbortLoadB.Click += new System.EventHandler(this._BtnAbortLoadB_Click);
            // 
            // _btnAbortLoadA
            // 
            this._btnAbortLoadA.Enabled = false;
            this._btnAbortLoadA.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._btnAbortLoadA.Image = global::Mss.Views.Properties.Resources.RedWhiteProhibited24;
            this._btnAbortLoadA.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this._btnAbortLoadA.Location = new System.Drawing.Point(435, 47);
            this._btnAbortLoadA.Name = "_btnAbortLoadA";
            this._btnAbortLoadA.Size = new System.Drawing.Size(120, 30);
            this._btnAbortLoadA.TabIndex = 6;
            this._btnAbortLoadA.Text = "Abort Load A ";
            this._btnAbortLoadA.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this._btnAbortLoadA.UseVisualStyleBackColor = true;
            this._btnAbortLoadA.Visible = false;
            this._btnAbortLoadA.Click += new System.EventHandler(this._BtnAbortLoadA_Click);
            // 
            // _btnAcceptLoadB
            // 
            this._btnAcceptLoadB.Enabled = false;
            this._btnAcceptLoadB.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this._btnAcceptLoadB.Image = global::Mss.Views.Properties.Resources.GreenCheckFancy24;
            this._btnAcceptLoadB.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this._btnAcceptLoadB.Location = new System.Drawing.Point(1499, 47);
            this._btnAcceptLoadB.Name = "_btnAcceptLoadB";
            this._btnAcceptLoadB.Size = new System.Drawing.Size(120, 30);
            this._btnAcceptLoadB.TabIndex = 5;
            this._btnAcceptLoadB.Text = "Accept Load B";
            this._btnAcceptLoadB.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this._btnAcceptLoadB.UseVisualStyleBackColor = true;
            this._btnAcceptLoadB.Visible = false;
            this._btnAcceptLoadB.Click += new System.EventHandler(this._BtnAcceptLoadB_Click);
            // 
            // _btnAcceptLoadA
            // 
            this._btnAcceptLoadA.Enabled = false;
            this._btnAcceptLoadA.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._btnAcceptLoadA.Image = global::Mss.Views.Properties.Resources.GreenCheckFancy24;
            this._btnAcceptLoadA.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this._btnAcceptLoadA.Location = new System.Drawing.Point(687, 47);
            this._btnAcceptLoadA.Name = "_btnAcceptLoadA";
            this._btnAcceptLoadA.Size = new System.Drawing.Size(120, 30);
            this._btnAcceptLoadA.TabIndex = 5;
            this._btnAcceptLoadA.Text = "Accept Load A";
            this._btnAcceptLoadA.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this._btnAcceptLoadA.UseVisualStyleBackColor = true;
            this._btnAcceptLoadA.Visible = false;
            this._btnAcceptLoadA.Click += new System.EventHandler(this._BtnAcceptLoadA_Click);
            // 
            // _lblSlugBName
            // 
            this._lblSlugBName.AutoSize = true;
            this._lblSlugBName.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this._lblSlugBName.Location = new System.Drawing.Point(817, 50);
            this._lblSlugBName.Name = "_lblSlugBName";
            this._lblSlugBName.Size = new System.Drawing.Size(78, 30);
            this._lblSlugBName.TabIndex = 4;
            this._lblSlugBName.Text = "Slug B";
            // 
            // _lblSlugAName
            // 
            this._lblSlugAName.AutoSize = true;
            this._lblSlugAName.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this._lblSlugAName.Location = new System.Drawing.Point(5, 50);
            this._lblSlugAName.Name = "_lblSlugAName";
            this._lblSlugAName.Size = new System.Drawing.Size(79, 30);
            this._lblSlugAName.TabIndex = 3;
            this._lblSlugAName.Text = "Slug A";
            // 
            // _slugAGridUpper
            // 
            this._slugAGridUpper.EnableSort = true;
            this._slugAGridUpper.Location = new System.Drawing.Point(10, 83);
            this._slugAGridUpper.Name = "_slugAGridUpper";
            this._slugAGridUpper.OptimizeMode = SourceGrid.CellOptimizeMode.ForRows;
            this._slugAGridUpper.SelectionMode = SourceGrid.GridSelectionMode.Cell;
            this._slugAGridUpper.Size = new System.Drawing.Size(797, 406);
            this._slugAGridUpper.TabIndex = 1;
            this._slugAGridUpper.TabStop = true;
            this._slugAGridUpper.ToolTipText = "";
            // 
            // navigator
            // 
            this.navigator.BackColor = System.Drawing.SystemColors.Control;
            this.navigator.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.navigator.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.navigator.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.navigatorLegend,
            this.toolStripSeparator1,
            this._navigatorBtnRefreshItem,
            this._navigatorSepReleaseNewLoad,
            this._navigatorBtnReleaseBroadcast});
            this.navigator.Location = new System.Drawing.Point(0, 0);
            this.navigator.Name = "navigator";
            this.navigator.Padding = new System.Windows.Forms.Padding(5, 2, 5, 2);
            this.navigator.Size = new System.Drawing.Size(1651, 35);
            this.navigator.TabIndex = 0;
            // 
            // navigatorLegend
            // 
            this.navigatorLegend.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.navigatorLegend.DropDownWidth = 160;
            this.navigatorLegend.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.navigatorLegend.IntegralHeight = false;
            this.navigatorLegend.Items.AddRange(new object[] {
            "Status Legend",
            "Invalid",
            "Waiting",
            "Pending",
            "Pickable",
            "Picking",
            "Picked",
            "Presequenced",
            "Sequenced",
            "Transferring",
            "Done",
            "Loadable"});
            this.navigatorLegend.MaxDropDownItems = 16;
            this.navigatorLegend.Name = "navigatorLegend";
            this.navigatorLegend.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
            this.navigatorLegend.Size = new System.Drawing.Size(160, 31);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 31);
            // 
            // _navigatorBtnRefreshItem
            // 
            this._navigatorBtnRefreshItem.Image = ((System.Drawing.Image)(resources.GetObject("_navigatorBtnRefreshItem.Image")));
            this._navigatorBtnRefreshItem.Name = "_navigatorBtnRefreshItem";
            this._navigatorBtnRefreshItem.Size = new System.Drawing.Size(74, 28);
            this._navigatorBtnRefreshItem.Text = "Refresh";
            this._navigatorBtnRefreshItem.ToolTipText = "Refresh";
            this._navigatorBtnRefreshItem.Click += new System.EventHandler(this._NavigatorBtnRefreshItem_Click);
            // 
            // _navigatorSepReleaseNewLoad
            // 
            this._navigatorSepReleaseNewLoad.Name = "_navigatorSepReleaseNewLoad";
            this._navigatorSepReleaseNewLoad.Size = new System.Drawing.Size(6, 31);
            this._navigatorSepReleaseNewLoad.Visible = false;
            // 
            // _navigatorBtnReleaseBroadcast
            // 
            this._navigatorBtnReleaseBroadcast.Font = new System.Drawing.Font("Segoe UI", 9F);
            this._navigatorBtnReleaseBroadcast.Image = global::Mss.Views.Properties.Resources.PurpleRelease24;
            this._navigatorBtnReleaseBroadcast.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._navigatorBtnReleaseBroadcast.Name = "_navigatorBtnReleaseBroadcast";
            this._navigatorBtnReleaseBroadcast.Size = new System.Drawing.Size(129, 28);
            this._navigatorBtnReleaseBroadcast.Text = "Release Broadcast";
            this._navigatorBtnReleaseBroadcast.Visible = false;
            this._navigatorBtnReleaseBroadcast.Click += new System.EventHandler(this._NavigatorBtnReleaseBroadcast_Click);
            // 
            // _slugAGridLower
            // 
            this._slugAGridLower.EnableSort = true;
            this._slugAGridLower.Location = new System.Drawing.Point(10, 495);
            this._slugAGridLower.Name = "_slugAGridLower";
            this._slugAGridLower.OptimizeMode = SourceGrid.CellOptimizeMode.ForRows;
            this._slugAGridLower.SelectionMode = SourceGrid.GridSelectionMode.Cell;
            this._slugAGridLower.Size = new System.Drawing.Size(797, 406);
            this._slugAGridLower.TabIndex = 2;
            this._slugAGridLower.TabStop = true;
            this._slugAGridLower.ToolTipText = "";
            // 
            // _slugBGridUpper
            // 
            this._slugBGridUpper.EnableSort = true;
            this._slugBGridUpper.Location = new System.Drawing.Point(822, 83);
            this._slugBGridUpper.Name = "_slugBGridUpper";
            this._slugBGridUpper.OptimizeMode = SourceGrid.CellOptimizeMode.ForRows;
            this._slugBGridUpper.SelectionMode = SourceGrid.GridSelectionMode.Cell;
            this._slugBGridUpper.Size = new System.Drawing.Size(797, 406);
            this._slugBGridUpper.TabIndex = 1;
            this._slugBGridUpper.TabStop = true;
            this._slugBGridUpper.ToolTipText = "";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.label3.Location = new System.Drawing.Point(145, 50);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(161, 30);
            this.label3.TabIndex = 9;
            this.label3.Text = "Load Number:";
            // 
            // _btnCloseReopenLoadA
            // 
            this._btnCloseReopenLoadA.Enabled = false;
            this._btnCloseReopenLoadA.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._btnCloseReopenLoadA.Image = global::Mss.Views.Properties.Resources.RedMinus24; 
            this._btnCloseReopenLoadA.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this._btnCloseReopenLoadA.Location = new System.Drawing.Point(561, 47);
            this._btnCloseReopenLoadA.Name = "_btnCloseReopenLoadA";
            this._btnCloseReopenLoadA.Size = new System.Drawing.Size(120, 30);
            this._btnCloseReopenLoadA.TabIndex = 5;
            this._btnCloseReopenLoadA.Text = "Close Load A  ";
            this._btnCloseReopenLoadA.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this._btnCloseReopenLoadA.UseVisualStyleBackColor = true;
            this._btnCloseReopenLoadA.Visible = false;
            this._btnCloseReopenLoadA.Click += new System.EventHandler(this._BtnCloseReopenLoadA_Click);
            // 
            // _btnCloseReopenLoadB
            // 
            this._btnCloseReopenLoadB.Enabled = false;
            this._btnCloseReopenLoadB.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._btnCloseReopenLoadB.Image = global::Mss.Views.Properties.Resources.RedMinus24;
            this._btnCloseReopenLoadB.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this._btnCloseReopenLoadB.Location = new System.Drawing.Point(1373, 47);
            this._btnCloseReopenLoadB.Name = "_btnCloseReopenLoadB";
            this._btnCloseReopenLoadB.Size = new System.Drawing.Size(120, 30);
            this._btnCloseReopenLoadB.TabIndex = 5;
            this._btnCloseReopenLoadB.Text = "Close Load B  ";
            this._btnCloseReopenLoadB.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this._btnCloseReopenLoadB.UseVisualStyleBackColor = true;
            this._btnCloseReopenLoadB.Visible = false;
            this._btnCloseReopenLoadB.Click += new System.EventHandler(this._BtnCloseReopenLoadB_Click);
            // 
            // SlugsView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._lblLoadBNumber);
            this.Controls.Add(this._lblLoadANumber);
            this.Controls.Add(this._slugBGridLower);
            this.Controls.Add(this.label1);
            this.Controls.Add(this._btnAbortLoadB);
            this.Controls.Add(this._btnAbortLoadA);
            this.Controls.Add(this._btnAcceptLoadB);
            this.Controls.Add(this._btnCloseReopenLoadB);
            this.Controls.Add(this._btnCloseReopenLoadA);
            this.Controls.Add(this._btnAcceptLoadA);
            this.Controls.Add(this._lblSlugBName);
            this.Controls.Add(this._lblSlugAName);
            this.Controls.Add(this._slugAGridUpper);
            this.Controls.Add(this.navigator);
            this.Controls.Add(this._slugAGridLower);
            this.Controls.Add(this._slugBGridUpper);
            this.Controls.Add(this.label3);
            this.Name = "SlugsView";
            this.Size = new System.Drawing.Size(1651, 912);
            this.navigator.ResumeLayout(false);
            this.navigator.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private SlugGrid _slugAGridUpper;
        private SlugGrid _slugAGridLower;
        private SlugGrid _slugBGridUpper;
        private SlugGrid _slugBGridLower;
        protected System.Windows.Forms.ToolStrip navigator;
        protected System.Windows.Forms.ToolStripButton _navigatorBtnRefreshItem;
        private System.Windows.Forms.ToolStripSeparator _navigatorSepReleaseNewLoad;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripComboBox navigatorLegend;
        private System.Windows.Forms.Label _lblSlugBName;
        private System.Windows.Forms.Label _lblSlugAName;
        private System.Windows.Forms.Button _btnAcceptLoadA;
        private System.Windows.Forms.Button _btnAbortLoadA;
        private System.Windows.Forms.Button _btnAcceptLoadB;
        private System.Windows.Forms.Button _btnAbortLoadB;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label _lblLoadANumber;
        private System.Windows.Forms.Label _lblLoadBNumber;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ToolStripButton _navigatorBtnReleaseBroadcast;
        private System.Windows.Forms.Button _btnCloseReopenLoadA;
        private System.Windows.Forms.Button _btnCloseReopenLoadB;
    }
}
