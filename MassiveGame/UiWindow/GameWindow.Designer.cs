namespace MassiveGame.UI
{
    partial class GameWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GameWindow));
            this.GLControl = new OpenTK.GLControl(new OpenTK.Graphics.GraphicsMode(32, 24, 8));
            this.SuspendLayout();
            // 
            // GLControl
            // 
            this.GLControl.BackColor = System.Drawing.Color.Black;
            this.GLControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GLControl.Location = new System.Drawing.Point(0, 0);
            this.GLControl.Name = "GLControl";
            this.GLControl.Size = new System.Drawing.Size(1350, 729);
            this.GLControl.TabIndex = 0;
            this.GLControl.VSync = false;
            this.GLControl.Load += new System.EventHandler(this.OnLoad);
            this.GLControl.Paint += new System.Windows.Forms.PaintEventHandler(this.OnRender);
            this.GLControl.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnKeyDown);
            this.GLControl.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnMouseDown);
            this.GLControl.MouseMove += new System.Windows.Forms.MouseEventHandler(this.OnMouseMove);
            this.GLControl.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.OnMouseWheel);
            this.GLControl.Resize += new System.EventHandler(this.OnResize);
            this.GLControl.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.OnKeyPress);
            this.GLControl.KeyUp += new System.Windows.Forms.KeyEventHandler(this.OnKeyUp);
            // 
            // MainUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1350, 729);
            this.Controls.Add(this.GLControl);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainUI";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Highlight Engine";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnClosing);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnMouseDown);
            this.Move += new System.EventHandler(this.OnMove);
            this.ResumeLayout(false);

        }

        #endregion

        private OpenTK.GLControl GLControl;
    }
}