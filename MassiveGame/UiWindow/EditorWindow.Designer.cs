namespace MassiveGame.UI
{
    partial class EditorWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EditorWindow));
            this.GLControl = new OpenTK.GLControl();
            this.engineEntityListBoxHost = new System.Windows.Forms.Integration.ElementHost();
            this.engineEntityListBox = new WpfControlLibrary1.EngineEntityListBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // GLControl
            // 
            this.GLControl.BackColor = System.Drawing.Color.Black;
            this.GLControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GLControl.Location = new System.Drawing.Point(0, 0);
            this.GLControl.Margin = new System.Windows.Forms.Padding(4);
            this.GLControl.Name = "GLControl";
            this.GLControl.Size = new System.Drawing.Size(1444, 982);
            this.GLControl.TabIndex = 0;
            this.GLControl.VSync = false;
            this.GLControl.Paint += new System.Windows.Forms.PaintEventHandler(this.OnRender);
            this.GLControl.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnKeyDown);
            this.GLControl.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.OnKeyPress);
            this.GLControl.KeyUp += new System.Windows.Forms.KeyEventHandler(this.OnKeyUp);
            this.GLControl.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnMouseDown);
            this.GLControl.MouseMove += new System.Windows.Forms.MouseEventHandler(this.OnMouseMove);
            this.GLControl.MouseUp += new System.Windows.Forms.MouseEventHandler(this.GLControl_MouseUp);
            this.GLControl.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.OnMouseWheel);
            this.GLControl.Resize += new System.EventHandler(this.OnResize);
            // 
            // engineEntityListBoxHost
            // 
            this.engineEntityListBoxHost.Dock = System.Windows.Forms.DockStyle.Fill;
            this.engineEntityListBoxHost.Location = new System.Drawing.Point(0, 0);
            this.engineEntityListBoxHost.Name = "engineEntityListBoxHost";
            this.engineEntityListBoxHost.Size = new System.Drawing.Size(500, 439);
            this.engineEntityListBoxHost.TabIndex = 0;
            this.engineEntityListBoxHost.Text = "elementHost1";
            this.engineEntityListBoxHost.ChildChanged += new System.EventHandler<System.Windows.Forms.Integration.ChildChangedEventArgs>(this.elementHost1_ChildChanged);
            this.engineEntityListBoxHost.Child = this.engineEntityListBox;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.GLControl);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1444, 982);
            this.panel1.TabIndex = 1;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.panel3);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel2.Location = new System.Drawing.Point(1444, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(500, 982);
            this.panel2.TabIndex = 2;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.engineEntityListBoxHost);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(500, 439);
            this.panel3.TabIndex = 0;
            // 
            // EditorWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1944, 982);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel2);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "EditorWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Highlight Engine";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnClosing);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnMouseDown);
            this.Move += new System.EventHandler(this.OnMove);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private OpenTK.GLControl GLControl;
        private System.Windows.Forms.Integration.ElementHost engineEntityListBoxHost;
        private WpfControlLibrary1.EngineEntityListBox engineEntityListBox;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
    }
}