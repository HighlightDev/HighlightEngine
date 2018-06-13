using System;

namespace UIControls
{
    partial class CheckBoxEx
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
            this.CB_Label = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // CB_Label
            // 
            this.CB_Label.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CB_Label.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.75F);
            this.CB_Label.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.CB_Label.Image = global::UIControls.Properties.Resources.check_box_checked;
            this.CB_Label.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.CB_Label.Location = new System.Drawing.Point(0, 0);
            this.CB_Label.Name = "CB_Label";
            this.CB_Label.Size = new System.Drawing.Size(150, 25);
            this.CB_Label.TabIndex = 2;
            this.CB_Label.Text = "      Add text here";
            this.CB_Label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.CB_Label.Click += new System.EventHandler(this.CB_Label_Click);
            this.CB_Label.DoubleClick += new System.EventHandler(this.CB_Label_DoubleClick);
            this.CB_Label.MouseEnter += new System.EventHandler(this.CB_Label_MouseEnter);
            this.CB_Label.MouseLeave += new System.EventHandler(this.CB_Label_MouseLeave);
            // 
            // CheckBoxEx
            // 
            this.BackColor = System.Drawing.Color.FromArgb(((Int32)(((byte)(64)))), ((Int32)(((byte)(64)))), ((Int32)(((byte)(64)))));
            this.Controls.Add(this.CB_Label);
            this.Size = new System.Drawing.Size(150, 25);
            this.UseVisualStyleBackColor = false;
            this.CheckedChanged += new System.EventHandler(this.CheckBoxEx_CheckedChanged);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label CB_Label;
    }
}
