using System;

namespace UIControls
{
    partial class ButtonEx
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ButtonEx));
            this.L_Button = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // L_Button
            // 
            this.L_Button.BackColor = System.Drawing.Color.FromArgb(((Int32)(((byte)(64)))), ((Int32)(((byte)(64)))), ((Int32)(((byte)(64)))));
            this.L_Button.Dock = System.Windows.Forms.DockStyle.Fill;
            this.L_Button.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.L_Button.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.L_Button.Image = ((System.Drawing.Image)(resources.GetObject("L_Button.Image")));
            this.L_Button.Location = new System.Drawing.Point(0, 0);
            this.L_Button.MinimumSize = new System.Drawing.Size(0, 32);
            this.L_Button.Name = "L_Button";
            this.L_Button.Padding = new System.Windows.Forms.Padding(0, 0, 0, 5);
            this.L_Button.Size = new System.Drawing.Size(122, 32);
            this.L_Button.TabIndex = 0;
            this.L_Button.Text = "Add text here";
            this.L_Button.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.L_Button.MouseEnter += new System.EventHandler(this.L_Button_MouseEnter);
            this.L_Button.MouseLeave += new System.EventHandler(this.L_Button_MouseLeave);
            // 
            // ButtonEx
            // 
            this.BackColor = System.Drawing.Color.FromArgb(((Int32)(((byte)(64)))), ((Int32)(((byte)(64)))), ((Int32)(((byte)(64)))));
            this.Controls.Add(this.L_Button);
            this.MinimumSize = new System.Drawing.Size(0, 32);
            this.Size = new System.Drawing.Size(122, 32);
            this.UseVisualStyleBackColor = false;
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.Label L_Button;
    }
}
