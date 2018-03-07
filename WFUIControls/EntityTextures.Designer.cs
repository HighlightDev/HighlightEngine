namespace UIControls
{
    partial class EntityTextures
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EntityTextures));
            this.TLP = new System.Windows.Forms.TableLayoutPanel();
            this.CMS_LV = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.TSMI_SmallIcons = new System.Windows.Forms.ToolStripMenuItem();
            this.TSMI_LargeIcons = new System.Windows.Forms.ToolStripMenuItem();
            this.TS_Separator = new System.Windows.Forms.ToolStripSeparator();
            this.TSMI_Refresh = new System.Windows.Forms.ToolStripMenuItem();
            this.MCB_TA = new UIControls.MinimizeCheckBox();
            this.LV_TA = new System.Windows.Forms.ListView();
            this.MCB_NM = new UIControls.MinimizeCheckBox();
            this.LV_NM = new System.Windows.Forms.ListView();
            this.MCB_SM = new UIControls.MinimizeCheckBox();
            this.LV_SM = new System.Windows.Forms.ListView();
            this.TLP.SuspendLayout();
            this.CMS_LV.SuspendLayout();
            this.SuspendLayout();
            // 
            // TLP
            // 
            this.TLP.ColumnCount = 1;
            this.TLP.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.TLP.Controls.Add(this.MCB_TA, 0, 0);
            this.TLP.Controls.Add(this.MCB_NM, 0, 1);
            this.TLP.Controls.Add(this.MCB_SM, 0, 2);
            this.TLP.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TLP.ForeColor = System.Drawing.SystemColors.AppWorkspace;
            this.TLP.Location = new System.Drawing.Point(0, 0);
            this.TLP.Name = "TLP";
            this.TLP.RowCount = 4;
            this.TLP.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.TLP.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.TLP.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.TLP.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.TLP.Size = new System.Drawing.Size(264, 407);
            this.TLP.TabIndex = 0;
            // 
            // CMS_LV
            // 
            this.CMS_LV.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.CMS_LV.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.CMS_LV.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TSMI_SmallIcons,
            this.TSMI_LargeIcons,
            this.TS_Separator,
            this.TSMI_Refresh});
            this.CMS_LV.Name = "CMS_LV";
            this.CMS_LV.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.CMS_LV.Size = new System.Drawing.Size(205, 82);
            // 
            // TSMI_SmallIcons
            // 
            this.TSMI_SmallIcons.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.TSMI_SmallIcons.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.TSMI_SmallIcons.ForeColor = System.Drawing.SystemColors.ButtonShadow;
            this.TSMI_SmallIcons.Name = "TSMI_SmallIcons";
            this.TSMI_SmallIcons.ShortcutKeyDisplayString = "Num+";
            this.TSMI_SmallIcons.Size = new System.Drawing.Size(204, 24);
            this.TSMI_SmallIcons.Text = "Small Icons";
            this.TSMI_SmallIcons.TextDirection = System.Windows.Forms.ToolStripTextDirection.Horizontal;
            this.TSMI_SmallIcons.Click += new System.EventHandler(this.TSMI_SmallIcons_Click);
            // 
            // TSMI_LargeIcons
            // 
            this.TSMI_LargeIcons.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.TSMI_LargeIcons.Checked = true;
            this.TSMI_LargeIcons.CheckState = System.Windows.Forms.CheckState.Checked;
            this.TSMI_LargeIcons.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.TSMI_LargeIcons.ForeColor = System.Drawing.SystemColors.ButtonShadow;
            this.TSMI_LargeIcons.Name = "TSMI_LargeIcons";
            this.TSMI_LargeIcons.ShortcutKeyDisplayString = "Num-";
            this.TSMI_LargeIcons.Size = new System.Drawing.Size(204, 24);
            this.TSMI_LargeIcons.Text = "Large Icons";
            this.TSMI_LargeIcons.TextDirection = System.Windows.Forms.ToolStripTextDirection.Horizontal;
            this.TSMI_LargeIcons.Click += new System.EventHandler(this.TSMI_LargeIcons_Click);
            // 
            // TS_Separator
            // 
            this.TS_Separator.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.TS_Separator.ForeColor = System.Drawing.SystemColors.ButtonShadow;
            this.TS_Separator.Name = "TS_Separator";
            this.TS_Separator.Size = new System.Drawing.Size(201, 6);
            // 
            // TSMI_Refresh
            // 
            this.TSMI_Refresh.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.TSMI_Refresh.ForeColor = System.Drawing.SystemColors.ButtonShadow;
            this.TSMI_Refresh.Name = "TSMI_Refresh";
            this.TSMI_Refresh.ShortcutKeyDisplayString = "F5";
            this.TSMI_Refresh.Size = new System.Drawing.Size(204, 24);
            this.TSMI_Refresh.Text = "Refresh";
            this.TSMI_Refresh.Click += new System.EventHandler(this.TSMI_Refresh_Click);
            // 
            // MCB_TA
            // 
            this.MCB_TA.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.MCB_TA.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.MCB_TA.CheckBoxActive = ((System.Drawing.Image)(resources.GetObject("MCB_TA.CheckBoxActive")));
            this.MCB_TA.CheckBoxChecked = ((System.Drawing.Image)(resources.GetObject("MCB_TA.CheckBoxChecked")));
            this.MCB_TA.CheckBoxInactive = ((System.Drawing.Image)(resources.GetObject("MCB_TA.CheckBoxInactive")));
            this.MCB_TA.CheckBoxText = "      Enable";
            this.MCB_TA.Checked = true;
            this.MCB_TA.Dock = System.Windows.Forms.DockStyle.Top;
            this.MCB_TA.HeaderActiveForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.MCB_TA.HeaderInactiveForeColor = System.Drawing.SystemColors.ButtonFace;
            this.MCB_TA.HeaderText = "   Texture Atlas";
            this.MCB_TA.IsMinimized = false;
            this.MCB_TA.Location = new System.Drawing.Point(3, 3);
            this.MCB_TA.MaxHeight = 0;
            this.MCB_TA.Maximized = ((System.Drawing.Image)(resources.GetObject("MCB_TA.Maximized")));
            this.MCB_TA.Minimized = ((System.Drawing.Image)(resources.GetObject("MCB_TA.Minimized")));
            this.MCB_TA.MinimizingPart = this.LV_TA;
            this.MCB_TA.Name = "MCB_TA";
            this.MCB_TA.RightToLeftHeaderAlign = System.Windows.Forms.RightToLeft.Yes;
            this.MCB_TA.Size = new System.Drawing.Size(258, 107);
            this.MCB_TA.TabIndex = 0;
            this.MCB_TA.Resize += new System.EventHandler(this.MCB_TA_Resize);
            // 
            // LV_TA
            // 
            this.LV_TA.AccessibleDescription = "";
            this.LV_TA.AccessibleName = "";
            this.LV_TA.BackColor = System.Drawing.Color.DimGray;
            this.LV_TA.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.LV_TA.ContextMenuStrip = this.CMS_LV;
            this.LV_TA.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LV_TA.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.LV_TA.ForeColor = System.Drawing.SystemColors.ButtonShadow;
            this.LV_TA.GridLines = true;
            this.LV_TA.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.LV_TA.HideSelection = false;
            this.LV_TA.Location = new System.Drawing.Point(4, 56);
            this.LV_TA.MultiSelect = false;
            this.LV_TA.Name = "LV_TA";
            this.LV_TA.Size = new System.Drawing.Size(250, 47);
            this.LV_TA.TabIndex = 3;
            this.LV_TA.UseCompatibleStateImageBehavior = false;
            this.LV_TA.KeyDown += new System.Windows.Forms.KeyEventHandler(this.LV_TA_KeyDown);
            // 
            // MCB_NM
            // 
            this.MCB_NM.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.MCB_NM.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.MCB_NM.CheckBoxActive = ((System.Drawing.Image)(resources.GetObject("MCB_NM.CheckBoxActive")));
            this.MCB_NM.CheckBoxChecked = ((System.Drawing.Image)(resources.GetObject("MCB_NM.CheckBoxChecked")));
            this.MCB_NM.CheckBoxInactive = ((System.Drawing.Image)(resources.GetObject("MCB_NM.CheckBoxInactive")));
            this.MCB_NM.CheckBoxText = "      Enable";
            this.MCB_NM.Checked = true;
            this.MCB_NM.Dock = System.Windows.Forms.DockStyle.Top;
            this.MCB_NM.HeaderActiveForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.MCB_NM.HeaderInactiveForeColor = System.Drawing.SystemColors.ButtonFace;
            this.MCB_NM.HeaderText = "   Normal Map";
            this.MCB_NM.IsMinimized = false;
            this.MCB_NM.Location = new System.Drawing.Point(3, 116);
            this.MCB_NM.MaxHeight = 0;
            this.MCB_NM.Maximized = ((System.Drawing.Image)(resources.GetObject("MCB_NM.Maximized")));
            this.MCB_NM.Minimized = ((System.Drawing.Image)(resources.GetObject("MCB_NM.Minimized")));
            this.MCB_NM.MinimizingPart = this.LV_NM;
            this.MCB_NM.Name = "MCB_NM";
            this.MCB_NM.RightToLeftHeaderAlign = System.Windows.Forms.RightToLeft.Yes;
            this.MCB_NM.Size = new System.Drawing.Size(258, 108);
            this.MCB_NM.TabIndex = 1;
            this.MCB_NM.Resize += new System.EventHandler(this.MCB_NM_Resize);
            // 
            // LV_NM
            // 
            this.LV_NM.BackColor = System.Drawing.Color.DimGray;
            this.LV_NM.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.LV_NM.ContextMenuStrip = this.CMS_LV;
            this.LV_NM.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LV_NM.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.LV_NM.ForeColor = System.Drawing.SystemColors.ButtonShadow;
            this.LV_NM.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.LV_NM.Location = new System.Drawing.Point(4, 56);
            this.LV_NM.MultiSelect = false;
            this.LV_NM.Name = "LV_NM";
            this.LV_NM.Size = new System.Drawing.Size(250, 48);
            this.LV_NM.TabIndex = 3;
            this.LV_NM.UseCompatibleStateImageBehavior = false;
            this.LV_NM.KeyDown += new System.Windows.Forms.KeyEventHandler(this.LV_NM_KeyDown);
            // 
            // MCB_SM
            // 
            this.MCB_SM.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.MCB_SM.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.MCB_SM.CheckBoxActive = ((System.Drawing.Image)(resources.GetObject("MCB_SM.CheckBoxActive")));
            this.MCB_SM.CheckBoxChecked = ((System.Drawing.Image)(resources.GetObject("MCB_SM.CheckBoxChecked")));
            this.MCB_SM.CheckBoxInactive = ((System.Drawing.Image)(resources.GetObject("MCB_SM.CheckBoxInactive")));
            this.MCB_SM.CheckBoxText = "      Enable";
            this.MCB_SM.Checked = true;
            this.MCB_SM.Dock = System.Windows.Forms.DockStyle.Top;
            this.MCB_SM.HeaderActiveForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.MCB_SM.HeaderInactiveForeColor = System.Drawing.SystemColors.ButtonFace;
            this.MCB_SM.HeaderText = "   Specular Map";
            this.MCB_SM.IsMinimized = false;
            this.MCB_SM.Location = new System.Drawing.Point(3, 230);
            this.MCB_SM.MaxHeight = 0;
            this.MCB_SM.Maximized = ((System.Drawing.Image)(resources.GetObject("MCB_SM.Maximized")));
            this.MCB_SM.Minimized = ((System.Drawing.Image)(resources.GetObject("MCB_SM.Minimized")));
            this.MCB_SM.MinimizingPart = this.LV_SM;
            this.MCB_SM.Name = "MCB_SM";
            this.MCB_SM.RightToLeftHeaderAlign = System.Windows.Forms.RightToLeft.Yes;
            this.MCB_SM.Size = new System.Drawing.Size(258, 107);
            this.MCB_SM.TabIndex = 2;
            this.MCB_SM.Resize += new System.EventHandler(this.MCB_SM_Resize);
            // 
            // LV_SM
            // 
            this.LV_SM.BackColor = System.Drawing.Color.DimGray;
            this.LV_SM.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.LV_SM.ContextMenuStrip = this.CMS_LV;
            this.LV_SM.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LV_SM.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.LV_SM.ForeColor = System.Drawing.SystemColors.ButtonShadow;
            this.LV_SM.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.LV_SM.Location = new System.Drawing.Point(4, 56);
            this.LV_SM.MultiSelect = false;
            this.LV_SM.Name = "LV_SM";
            this.LV_SM.Size = new System.Drawing.Size(250, 47);
            this.LV_SM.TabIndex = 3;
            this.LV_SM.UseCompatibleStateImageBehavior = false;
            this.LV_SM.KeyDown += new System.Windows.Forms.KeyEventHandler(this.LV_SM_KeyDown);
            // 
            // EntityTextures
            // 
            this.AccessibleDescription = "";
            this.AccessibleName = "";
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.Controls.Add(this.TLP);
            this.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.Name = "EntityTextures";
            this.Size = new System.Drawing.Size(264, 407);
            this.Load += new System.EventHandler(this.EntityTextures_Load);
            this.Resize += new System.EventHandler(this.EntityTextures_Resize);
            this.TLP.ResumeLayout(false);
            this.CMS_LV.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel TLP;
        private MinimizeCheckBox MCB_TA;
        private MinimizeCheckBox MCB_NM;
        private MinimizeCheckBox MCB_SM;
        public System.Windows.Forms.ListView LV_TA;
        public System.Windows.Forms.ListView LV_NM;
        public System.Windows.Forms.ListView LV_SM;
        private System.Windows.Forms.ContextMenuStrip CMS_LV;
        private System.Windows.Forms.ToolStripMenuItem TSMI_SmallIcons;
        private System.Windows.Forms.ToolStripMenuItem TSMI_LargeIcons;
        private System.Windows.Forms.ToolStripSeparator TS_Separator;
        private System.Windows.Forms.ToolStripMenuItem TSMI_Refresh;
    }
}
