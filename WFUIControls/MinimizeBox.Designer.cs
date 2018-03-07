namespace UIControls
{
    partial class MinimizeBox
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
            this.TLP = new System.Windows.Forms.TableLayoutPanel();
            this.L_Header = new System.Windows.Forms.Label();
            this.TLP.SuspendLayout();
            this.SuspendLayout();
            // 
            // TLP
            // 
            this.TLP.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.TLP.ColumnCount = 1;
            this.TLP.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.TLP.Controls.Add(this.L_Header, 0, 0);
            this.TLP.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TLP.Location = new System.Drawing.Point(0, 0);
            this.TLP.Name = "TLP";
            this.TLP.RowCount = 2;
            this.TLP.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.TLP.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.TLP.Size = new System.Drawing.Size(230, 314);
            this.TLP.TabIndex = 0;
            // 
            // L_Header
            // 
            this.L_Header.AutoSize = true;
            this.L_Header.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.L_Header.Cursor = System.Windows.Forms.Cursors.Hand;
            this.L_Header.Dock = System.Windows.Forms.DockStyle.Fill;
            this.L_Header.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.L_Header.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.L_Header.Image = global::UIControls.Properties.Resources.minimized;
            this.L_Header.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.L_Header.Location = new System.Drawing.Point(4, 1);
            this.L_Header.Name = "L_Header";
            this.L_Header.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.L_Header.Size = new System.Drawing.Size(222, 25);
            this.L_Header.TabIndex = 0;
            this.L_Header.Text = "   Add text here";
            this.L_Header.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.L_Header.Click += new System.EventHandler(this.l_header_Click);
            this.L_Header.DoubleClick += new System.EventHandler(this.L_Header_DoubleClick);
            this.L_Header.MouseEnter += new System.EventHandler(this.l_header_MouseEnter);
            this.L_Header.MouseLeave += new System.EventHandler(this.l_header_MouseLeave);
            // 
            // MinimizeBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.Controls.Add(this.TLP);
            this.Name = "MinimizeBox";
            this.Size = new System.Drawing.Size(230, 314);
            this.Load += new System.EventHandler(this.OnLoad);
            this.TLP.ResumeLayout(false);
            this.TLP.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel TLP;
        private System.Windows.Forms.Label L_Header;
    }
}
