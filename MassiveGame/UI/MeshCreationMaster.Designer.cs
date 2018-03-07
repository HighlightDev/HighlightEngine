namespace MassiveGame
{
    partial class MeshCreationMaster
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MeshCreationMaster));
            this.TLP_Main = new System.Windows.Forms.TableLayoutPanel();
            this.TLP_Right = new System.Windows.Forms.TableLayoutPanel();
            this.glControl1 = new OpenTK.GLControl();
            this.TLP_Buttons = new System.Windows.Forms.TableLayoutPanel();
            this.TLP_Left = new System.Windows.Forms.TableLayoutPanel();
            //this.MB_ModelSelection = new UIControls.MinimizeBox();
            //this.MB_TextureSelection = new UIControls.MinimizeBox();
            //this.ET_TextureSelection = new UIControls.EntityTextures();
            this.TLP_Main.SuspendLayout();
            this.TLP_Right.SuspendLayout();
            this.TLP_Left.SuspendLayout();
            this.SuspendLayout();
            // 
            // TLP_Main
            // 
            this.TLP_Main.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.TLP_Main.ColumnCount = 2;
            this.TLP_Main.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 297F));
            this.TLP_Main.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.TLP_Main.Controls.Add(this.TLP_Right, 1, 0);
            this.TLP_Main.Controls.Add(this.TLP_Left, 0, 0);
            this.TLP_Main.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TLP_Main.Location = new System.Drawing.Point(0, 0);
            this.TLP_Main.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.TLP_Main.Name = "TLP_Main";
            this.TLP_Main.RowCount = 1;
            this.TLP_Main.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.TLP_Main.Size = new System.Drawing.Size(910, 700);
            this.TLP_Main.TabIndex = 5;
            // 
            // TLP_Right
            // 
            this.TLP_Right.ColumnCount = 1;
            this.TLP_Right.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.TLP_Right.Controls.Add(this.glControl1, 0, 0);
            this.TLP_Right.Controls.Add(this.TLP_Buttons, 0, 1);
            this.TLP_Right.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TLP_Right.Location = new System.Drawing.Point(302, 3);
            this.TLP_Right.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.TLP_Right.Name = "TLP_Right";
            this.TLP_Right.RowCount = 2;
            this.TLP_Right.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.TLP_Right.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.TLP_Right.Size = new System.Drawing.Size(604, 694);
            this.TLP_Right.TabIndex = 7;
            // 
            // glControl1
            // 
            this.glControl1.BackColor = System.Drawing.Color.Black;
            this.glControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.glControl1.Location = new System.Drawing.Point(3, 2);
            this.glControl1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.glControl1.Name = "glControl1";
            this.glControl1.Size = new System.Drawing.Size(598, 640);
            this.glControl1.TabIndex = 6;
            this.glControl1.VSync = false;
            // 
            // TLP_Buttons
            // 
            this.TLP_Buttons.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.TLP_Buttons.ColumnCount = 2;
            this.TLP_Buttons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TLP_Buttons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TLP_Buttons.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TLP_Buttons.Location = new System.Drawing.Point(3, 646);
            this.TLP_Buttons.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.TLP_Buttons.Name = "TLP_Buttons";
            this.TLP_Buttons.RowCount = 1;
            this.TLP_Buttons.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.TLP_Buttons.Size = new System.Drawing.Size(598, 46);
            this.TLP_Buttons.TabIndex = 0;
            // 
            // TLP_Left
            // 
            this.TLP_Left.ColumnCount = 1;
            this.TLP_Left.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            //this.TLP_Left.Controls.Add(this.MB_ModelSelection, 0, 0);
            //this.TLP_Left.Controls.Add(this.MB_TextureSelection, 0, 1);
            this.TLP_Left.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TLP_Left.Location = new System.Drawing.Point(4, 3);
            this.TLP_Left.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.TLP_Left.Name = "TLP_Left";
            this.TLP_Left.RowCount = 3;
            this.TLP_Left.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.TLP_Left.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.TLP_Left.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.TLP_Left.Size = new System.Drawing.Size(291, 694);
            this.TLP_Left.TabIndex = 8;
            // 
            // MB_ModelSelection
            // 
            //this.MB_ModelSelection.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            //this.MB_ModelSelection.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            //this.MB_ModelSelection.Dock = System.Windows.Forms.DockStyle.Fill;
            //this.MB_ModelSelection.HeaderActiveForeColor = System.Drawing.SystemColors.MenuHighlight;
            //this.MB_ModelSelection.HeaderInactiveForeColor = System.Drawing.SystemColors.ButtonFace;
            //this.MB_ModelSelection.HeaderText = "   Select Model";
            //this.MB_ModelSelection.isMinimized = false;
            //this.MB_ModelSelection.Location = new System.Drawing.Point(3, 2);
            //this.MB_ModelSelection.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            //this.MB_ModelSelection.Maximized = ((System.Drawing.Image)(resources.GetObject("MB_ModelSelection.Maximized")));
            //this.MB_ModelSelection.Minimized = ((System.Drawing.Image)(resources.GetObject("MB_ModelSelection.Minimized")));
            //this.MB_ModelSelection.MinimumSize = new System.Drawing.Size(0, 27);
            //this.MB_ModelSelection.Name = "MB_ModelSelection";
            //this.MB_ModelSelection.RightToLeftAlign = System.Windows.Forms.RightToLeft.No;
            //this.MB_ModelSelection.Size = new System.Drawing.Size(285, 318);
            //this.MB_ModelSelection.TabIndex = 0;
            // 
            // MB_TextureSelection
            // 
            //this.MB_TextureSelection.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            //this.MB_TextureSelection.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            //this.MB_TextureSelection.Dock = System.Windows.Forms.DockStyle.Fill;
            //this.MB_TextureSelection.HeaderActiveForeColor = System.Drawing.SystemColors.MenuHighlight;
            //this.MB_TextureSelection.HeaderInactiveForeColor = System.Drawing.SystemColors.ButtonFace;
            //this.MB_TextureSelection.HeaderText = "   Select Textures";
            //this.MB_TextureSelection.isMinimized = false;
            //this.MB_TextureSelection.Location = new System.Drawing.Point(3, 324);
            //this.MB_TextureSelection.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            //this.MB_TextureSelection.Maximized = ((System.Drawing.Image)(resources.GetObject("MB_TextureSelection.Maximized")));
            //this.MB_TextureSelection.Minimized = ((System.Drawing.Image)(resources.GetObject("MB_TextureSelection.Minimized")));
            //this.MB_TextureSelection.MinimizingPart = this.ET_TextureSelection;
            //this.MB_TextureSelection.MinimumSize = new System.Drawing.Size(6, 22);
            //this.MB_TextureSelection.Name = "MB_TextureSelection";
            //this.MB_TextureSelection.RightToLeftAlign = System.Windows.Forms.RightToLeft.No;
            //this.MB_TextureSelection.Size = new System.Drawing.Size(285, 318);
            //this.MB_TextureSelection.TabIndex = 1;
            // 
            // ET_TextureSelection
            // 
            //this.ET_TextureSelection.AccessibleDescription = "";
            //this.ET_TextureSelection.AccessibleName = "";
            //this.ET_TextureSelection.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            //this.ET_TextureSelection.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            //this.ET_TextureSelection.CheckBoxActive = ((System.Drawing.Image)(resources.GetObject("ET_TextureSelection.CheckBoxActive")));
            //this.ET_TextureSelection.CheckBoxChecked = ((System.Drawing.Image)(resources.GetObject("ET_TextureSelection.CheckBoxChecked")));
            //this.ET_TextureSelection.CheckBoxInactive = ((System.Drawing.Image)(resources.GetObject("ET_TextureSelection.CheckBoxInactive")));
            //this.ET_TextureSelection.ContextMenuForeColor = System.Drawing.SystemColors.ButtonShadow;
            //this.ET_TextureSelection.Dock = System.Windows.Forms.DockStyle.Fill;
            //this.ET_TextureSelection.ForeColor = System.Drawing.SystemColors.ButtonFace;
            //this.ET_TextureSelection.HeaderActiveForeColor = System.Drawing.SystemColors.MenuHighlight;
            //this.ET_TextureSelection.HeaderInactiveForeColor = System.Drawing.SystemColors.ButtonFace;
            //this.ET_TextureSelection.LargeIconsSize = new System.Drawing.Size(96, 96);
            //this.ET_TextureSelection.ListViewBackColor = System.Drawing.Color.DimGray;
            //this.ET_TextureSelection.ListViewForeColor = System.Drawing.SystemColors.ButtonFace;
            //this.ET_TextureSelection.Location = new System.Drawing.Point(4, 30);
            //this.ET_TextureSelection.Maximized = ((System.Drawing.Image)(resources.GetObject("ET_TextureSelection.Maximized")));
            //this.ET_TextureSelection.MinimizeCheckBoxHeight = 212;
            //this.ET_TextureSelection.Minimized = ((System.Drawing.Image)(resources.GetObject("ET_TextureSelection.Minimized")));
            //this.ET_TextureSelection.Name = "ET_TextureSelection";
            //this.ET_TextureSelection.RightToLeftHeaderAlign = System.Windows.Forms.RightToLeft.No;
            //this.ET_TextureSelection.Size = new System.Drawing.Size(277, 284);
            //this.ET_TextureSelection.SmallIconsSize = new System.Drawing.Size(32, 32);
            //this.ET_TextureSelection.TabIndex = 2;
            // 
            // MeshCreationMaster
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ClientSize = new System.Drawing.Size(910, 700);
            this.Controls.Add(this.TLP_Main);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MeshCreationMaster";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Preview";
            this.TLP_Main.ResumeLayout(false);
            this.TLP_Right.ResumeLayout(false);
            this.TLP_Left.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel TLP_Main;
        private System.Windows.Forms.TableLayoutPanel TLP_Right;
        private System.Windows.Forms.TableLayoutPanel TLP_Buttons;
        private System.Windows.Forms.TableLayoutPanel TLP_Left;
        //private UIControls.MinimizeBox MB_ModelSelection;
        //private UIControls.MinimizeBox MB_TextureSelection;
        private OpenTK.GLControl glControl1;
        //private UIControls.EntityTextures ET_TextureSelection;
    }
}