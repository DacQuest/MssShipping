namespace Mss.Views
{
    partial class LoadDirectorView
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
            this._btnAccept = new System.Windows.Forms.Button();
            this._navigator = new System.Windows.Forms.ToolStrip();
            this._navigatorBtnRefreshItem = new System.Windows.Forms.ToolStripButton();
            this._btnReject = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this._lblPalletID = new System.Windows.Forms.Label();
            this._lblSku = new System.Windows.Forms.Label();
            this._lblJobID = new System.Windows.Forms.Label();
            this._lblState = new System.Windows.Forms.Label();
            this._slugAGrid = new Mss.Views.SlugGrid();
            this._slugBGrid = new Mss.Views.SlugGrid();
            this._navigator.SuspendLayout();
            this.SuspendLayout();
            // 
            // _btnAccept
            // 
            this._btnAccept.BackColor = System.Drawing.SystemColors.Control;
            this._btnAccept.Enabled = false;
            this._btnAccept.Font = new System.Drawing.Font("Arial", 30F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._btnAccept.Location = new System.Drawing.Point(151, 638);
            this._btnAccept.Name = "_btnAccept";
            this._btnAccept.Size = new System.Drawing.Size(253, 107);
            this._btnAccept.TabIndex = 2;
            this._btnAccept.Text = "Accept";
            this._btnAccept.UseVisualStyleBackColor = false;
            this._btnAccept.Click += new System.EventHandler(this._BtnAccept_Click);
            // 
            // _navigator
            // 
            this._navigator.BackColor = System.Drawing.SystemColors.Control;
            this._navigator.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this._navigator.ImageScalingSize = new System.Drawing.Size(24, 24);
            this._navigator.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._navigatorBtnRefreshItem});
            this._navigator.Location = new System.Drawing.Point(0, 0);
            this._navigator.Name = "_navigator";
            this._navigator.Padding = new System.Windows.Forms.Padding(5, 2, 5, 2);
            this._navigator.Size = new System.Drawing.Size(1651, 35);
            this._navigator.TabIndex = 0;
            // 
            // _navigatorBtnRefreshItem
            // 
            this._navigatorBtnRefreshItem.Image = global::Mss.Views.Properties.Resources.GreenRefresh24;
            this._navigatorBtnRefreshItem.Name = "_navigatorBtnRefreshItem";
            this._navigatorBtnRefreshItem.Size = new System.Drawing.Size(74, 28);
            this._navigatorBtnRefreshItem.Text = "Refresh";
            this._navigatorBtnRefreshItem.ToolTipText = "Refresh";
            this._navigatorBtnRefreshItem.Click += new System.EventHandler(this._NavigatorBtnRefreshItem_Click);
            // 
            // _btnReject
            // 
            this._btnReject.BackColor = System.Drawing.SystemColors.Control;
            this._btnReject.Enabled = false;
            this._btnReject.Font = new System.Drawing.Font("Arial", 30F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._btnReject.Location = new System.Drawing.Point(1022, 638);
            this._btnReject.Name = "_btnReject";
            this._btnReject.Size = new System.Drawing.Size(253, 107);
            this._btnReject.TabIndex = 9;
            this._btnReject.Text = "Reject";
            this._btnReject.UseVisualStyleBackColor = false;
            this._btnReject.Click += new System.EventHandler(this._BtnReject_Click);
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(577, 575);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(114, 23);
            this.label1.TabIndex = 3;
            this.label1.Text = "Pallet ID";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(577, 686);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(114, 23);
            this.label2.TabIndex = 5;
            this.label2.Text = "SKU";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(577, 762);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(114, 23);
            this.label3.TabIndex = 7;
            this.label3.Text = "Job ID";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _lblPalletID
            // 
            this._lblPalletID.BackColor = System.Drawing.SystemColors.Window;
            this._lblPalletID.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._lblPalletID.Font = new System.Drawing.Font("Arial", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._lblPalletID.Location = new System.Drawing.Point(581, 598);
            this._lblPalletID.Name = "_lblPalletID";
            this._lblPalletID.Size = new System.Drawing.Size(253, 77);
            this._lblPalletID.TabIndex = 4;
            this._lblPalletID.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // _lblSku
            // 
            this._lblSku.BackColor = System.Drawing.SystemColors.Window;
            this._lblSku.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._lblSku.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._lblSku.Location = new System.Drawing.Point(581, 709);
            this._lblSku.Name = "_lblSku";
            this._lblSku.Size = new System.Drawing.Size(253, 43);
            this._lblSku.TabIndex = 6;
            this._lblSku.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // _lblJobID
            // 
            this._lblJobID.BackColor = System.Drawing.SystemColors.Window;
            this._lblJobID.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._lblJobID.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._lblJobID.Location = new System.Drawing.Point(581, 785);
            this._lblJobID.Name = "_lblJobID";
            this._lblJobID.Size = new System.Drawing.Size(253, 43);
            this._lblJobID.TabIndex = 8;
            this._lblJobID.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // _lblState
            // 
            this._lblState.BackColor = System.Drawing.SystemColors.Window;
            this._lblState.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._lblState.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._lblState.Location = new System.Drawing.Point(12, 39);
            this._lblState.Name = "_lblState";
            this._lblState.Size = new System.Drawing.Size(1412, 45);
            this._lblState.TabIndex = 1;
            this._lblState.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // _slugAGrid
            // 
            this._slugAGrid.EnableSort = true;
            this._slugAGrid.Location = new System.Drawing.Point(10, 131);
            this._slugAGrid.Name = "_slugAGrid";
            this._slugAGrid.OptimizeMode = SourceGrid.CellOptimizeMode.ForRows;
            this._slugAGrid.SelectionMode = SourceGrid.GridSelectionMode.Cell;
            this._slugAGrid.Size = new System.Drawing.Size(797, 406);
            this._slugAGrid.TabIndex = 10;
            this._slugAGrid.TabStop = true;
            this._slugAGrid.ToolTipText = "";
            // 
            // _slugBGrid
            // 
            this._slugBGrid.EnableSort = true;
            this._slugBGrid.Location = new System.Drawing.Point(822, 131);
            this._slugBGrid.Name = "_slugBGrid";
            this._slugBGrid.OptimizeMode = SourceGrid.CellOptimizeMode.ForRows;
            this._slugBGrid.SelectionMode = SourceGrid.GridSelectionMode.Cell;
            this._slugBGrid.Size = new System.Drawing.Size(797, 406);
            this._slugBGrid.TabIndex = 11;
            this._slugBGrid.TabStop = true;
            this._slugBGrid.ToolTipText = "";
            // 
            // LoadDirectorView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._slugBGrid);
            this.Controls.Add(this._slugAGrid);
            this.Controls.Add(this._lblState);
            this.Controls.Add(this._lblJobID);
            this.Controls.Add(this._lblSku);
            this.Controls.Add(this._lblPalletID);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this._btnReject);
            this.Controls.Add(this._navigator);
            this.Controls.Add(this._btnAccept);
            this.Name = "LoadDirectorView";
            this.Size = new System.Drawing.Size(1651, 912);
            this._navigator.ResumeLayout(false);
            this._navigator.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button _btnAccept;
        protected System.Windows.Forms.ToolStrip _navigator;
        protected System.Windows.Forms.ToolStripButton _navigatorBtnRefreshItem;
        private System.Windows.Forms.Button _btnReject;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label _lblPalletID;
        private System.Windows.Forms.Label _lblSku;
        private System.Windows.Forms.Label _lblJobID;
        private System.Windows.Forms.Label _lblState;
        private SlugGrid _slugAGrid;
        private SlugGrid _slugBGrid;
    }
}
