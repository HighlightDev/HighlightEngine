using System;

namespace UIControls
{
    partial class RadioButtonEx
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
            this.RB_Label = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // RB_Label
            // 
            this.RB_Label.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RB_Label.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.75F);
            this.RB_Label.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.RB_Label.Image = global::UIControls.Properties.Resources.radio_button_checked;
            this.RB_Label.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.RB_Label.Location = new System.Drawing.Point(0, 0);
            this.RB_Label.Name = "RB_Label";
            this.RB_Label.Size = new System.Drawing.Size(152, 28);
            this.RB_Label.TabIndex = 3;
            this.RB_Label.Text = "      Add text here";
            this.RB_Label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.RB_Label.Click += new System.EventHandler(this.RB_Label_Click);
            this.RB_Label.DoubleClick += new System.EventHandler(this.RB_Label_DoubleClick);
            this.RB_Label.MouseEnter += new System.EventHandler(this.RB_Label_MouseEnter);
            this.RB_Label.MouseLeave += new System.EventHandler(this.RB_Label_MouseLeave);
            // 
            // RadioButtonEx
            // 
            this.BackColor = System.Drawing.Color.FromArgb(((Int32)(((byte)(64)))), ((Int32)(((byte)(64)))), ((Int32)(((byte)(64)))));
            this.Checked = true;
            this.Controls.Add(this.RB_Label);
            this.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.Size = new System.Drawing.Size(152, 28);
            this.TabStop = true;
            this.UseVisualStyleBackColor = false;
            this.CheckedChanged += new System.EventHandler(this.RadioButtonEx_CheckedChanged);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label RB_Label;
    }
}
