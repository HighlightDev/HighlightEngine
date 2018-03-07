namespace RenderWindow
{
    partial class RenderForm
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
            this.components = new System.ComponentModel.Container();
            this.OpenGlWindow = new Tao.Platform.Windows.SimpleOpenGlControl();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.loadASEModelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadOBJModelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadSimpleAnimationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadAnimExToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.assimpMLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.withMTLFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.withTXRFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileDialogForModel = new System.Windows.Forms.OpenFileDialog();
            this.backgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.RenderTimer = new System.Windows.Forms.Timer(this.components);
            this.openFileDialogForImg = new System.Windows.Forms.OpenFileDialog();
            this.trackBarZoom = new System.Windows.Forms.TrackBar();
            this.lZoom = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.trackBarAngle = new System.Windows.Forms.TrackBar();
            this.lAngle = new System.Windows.Forms.Label();
            this.ExitBtn = new System.Windows.Forms.Button();
            this.trackBarAxisY = new System.Windows.Forms.TrackBar();
            this.trackBarAxisZ = new System.Windows.Forms.TrackBar();
            this.trackBarAxisX = new System.Windows.Forms.TrackBar();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.lAxisZ = new System.Windows.Forms.Label();
            this.lAxisY = new System.Windows.Forms.Label();
            this.lAxisX = new System.Windows.Forms.Label();
            this.folderBrowserDialogForAnimation = new System.Windows.Forms.FolderBrowserDialog();
            this.testToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarZoom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarAngle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarAxisY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarAxisZ)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarAxisX)).BeginInit();
            this.SuspendLayout();
            // 
            // OpenGlWindow
            // 
            this.OpenGlWindow.AccumBits = ((byte)(0));
            this.OpenGlWindow.AutoCheckErrors = false;
            this.OpenGlWindow.AutoFinish = false;
            this.OpenGlWindow.AutoMakeCurrent = true;
            this.OpenGlWindow.AutoSwapBuffers = true;
            this.OpenGlWindow.BackColor = System.Drawing.Color.Black;
            this.OpenGlWindow.ColorBits = ((byte)(32));
            this.OpenGlWindow.DepthBits = ((byte)(16));
            this.OpenGlWindow.Location = new System.Drawing.Point(13, 27);
            this.OpenGlWindow.Name = "OpenGlWindow";
            this.OpenGlWindow.Size = new System.Drawing.Size(713, 577);
            this.OpenGlWindow.StencilBits = ((byte)(0));
            this.OpenGlWindow.TabIndex = 0;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadASEModelToolStripMenuItem,
            this.loadOBJModelToolStripMenuItem,
            this.loadSimpleAnimationToolStripMenuItem,
            this.loadAnimExToolStripMenuItem,
            this.assimpMLToolStripMenuItem,
            this.testToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1019, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // loadASEModelToolStripMenuItem
            // 
            this.loadASEModelToolStripMenuItem.Name = "loadASEModelToolStripMenuItem";
            this.loadASEModelToolStripMenuItem.Size = new System.Drawing.Size(105, 20);
            this.loadASEModelToolStripMenuItem.Text = "Load ASE model";
            this.loadASEModelToolStripMenuItem.Click += new System.EventHandler(this.loadASEModelToolStripMenuItem_Click);
            // 
            // loadOBJModelToolStripMenuItem
            // 
            this.loadOBJModelToolStripMenuItem.Name = "loadOBJModelToolStripMenuItem";
            this.loadOBJModelToolStripMenuItem.Size = new System.Drawing.Size(105, 20);
            this.loadOBJModelToolStripMenuItem.Text = "Load OBJ model";
            this.loadOBJModelToolStripMenuItem.Click += new System.EventHandler(this.loadOBJModelToolStripMenuItem_Click);
            // 
            // loadSimpleAnimationToolStripMenuItem
            // 
            this.loadSimpleAnimationToolStripMenuItem.Name = "loadSimpleAnimationToolStripMenuItem";
            this.loadSimpleAnimationToolStripMenuItem.Size = new System.Drawing.Size(140, 20);
            this.loadSimpleAnimationToolStripMenuItem.Text = "Load simple animation";
            this.loadSimpleAnimationToolStripMenuItem.Click += new System.EventHandler(this.loadSimpleAnimationToolStripMenuItem_Click);
            // 
            // loadAnimExToolStripMenuItem
            // 
            this.loadAnimExToolStripMenuItem.Name = "loadAnimExToolStripMenuItem";
            this.loadAnimExToolStripMenuItem.Size = new System.Drawing.Size(86, 20);
            this.loadAnimExToolStripMenuItem.Text = "Load animEx";
            this.loadAnimExToolStripMenuItem.Click += new System.EventHandler(this.loadAnimExToolStripMenuItem_Click);
            // 
            // assimpMLToolStripMenuItem
            // 
            this.assimpMLToolStripMenuItem.Name = "assimpMLToolStripMenuItem";
            this.assimpMLToolStripMenuItem.Size = new System.Drawing.Size(78, 20);
            this.assimpMLToolStripMenuItem.Text = "Assimp ML";
            this.assimpMLToolStripMenuItem.Click += new System.EventHandler(this.assimpMLToolStripMenuItem_Click);
            // 
            // withMTLFileToolStripMenuItem
            // 
            this.withMTLFileToolStripMenuItem.Name = "withMTLFileToolStripMenuItem";
            this.withMTLFileToolStripMenuItem.Size = new System.Drawing.Size(32, 19);
            // 
            // withTXRFileToolStripMenuItem
            // 
            this.withTXRFileToolStripMenuItem.Name = "withTXRFileToolStripMenuItem";
            this.withTXRFileToolStripMenuItem.Size = new System.Drawing.Size(32, 19);
            // 
            // openFileDialogForModel
            // 
            this.openFileDialogForModel.FileName = "openFileDialog1";
            // 
            // RenderTimer
            // 
            this.RenderTimer.Interval = 25;
            this.RenderTimer.Tick += new System.EventHandler(this.RenderTimerTick);
            // 
            // openFileDialogForImg
            // 
            this.openFileDialogForImg.FileName = "openFileDialogForImg";
            // 
            // trackBarZoom
            // 
            this.trackBarZoom.Location = new System.Drawing.Point(962, 113);
            this.trackBarZoom.Maximum = 5000;
            this.trackBarZoom.Minimum = -5000;
            this.trackBarZoom.Name = "trackBarZoom";
            this.trackBarZoom.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.trackBarZoom.Size = new System.Drawing.Size(45, 400);
            this.trackBarZoom.TabIndex = 2;
            this.trackBarZoom.TickFrequency = 100;
            this.trackBarZoom.Value = 1000;
            this.trackBarZoom.Scroll += new System.EventHandler(this.trackBarZoomScroll);
            // 
            // lZoom
            // 
            this.lZoom.AutoSize = true;
            this.lZoom.Location = new System.Drawing.Point(962, 531);
            this.lZoom.Name = "lZoom";
            this.lZoom.Size = new System.Drawing.Size(36, 13);
            this.lZoom.TabIndex = 3;
            this.lZoom.Text = "lZoom";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(962, 97);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(34, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Zoom";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(907, 97);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Angle V";
            // 
            // trackBarAngle
            // 
            this.trackBarAngle.Location = new System.Drawing.Point(907, 113);
            this.trackBarAngle.Maximum = 360;
            this.trackBarAngle.Minimum = -360;
            this.trackBarAngle.Name = "trackBarAngle";
            this.trackBarAngle.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.trackBarAngle.Size = new System.Drawing.Size(45, 400);
            this.trackBarAngle.TabIndex = 6;
            this.trackBarAngle.TickFrequency = 10;
            this.trackBarAngle.Scroll += new System.EventHandler(this.trackBarAngleScroll);
            // 
            // lAngle
            // 
            this.lAngle.AutoSize = true;
            this.lAngle.Location = new System.Drawing.Point(907, 531);
            this.lAngle.Name = "lAngle";
            this.lAngle.Size = new System.Drawing.Size(36, 13);
            this.lAngle.TabIndex = 7;
            this.lAngle.Text = "lAngle";
            // 
            // ExitBtn
            // 
            this.ExitBtn.Location = new System.Drawing.Point(830, 581);
            this.ExitBtn.Name = "ExitBtn";
            this.ExitBtn.Size = new System.Drawing.Size(86, 23);
            this.ExitBtn.TabIndex = 8;
            this.ExitBtn.Text = "Exit";
            this.ExitBtn.UseVisualStyleBackColor = true;
            this.ExitBtn.Click += new System.EventHandler(this.OnBtnExit);
            // 
            // trackBarAxisY
            // 
            this.trackBarAxisY.Location = new System.Drawing.Point(805, 113);
            this.trackBarAxisY.Maximum = 10000;
            this.trackBarAxisY.Minimum = -10000;
            this.trackBarAxisY.Name = "trackBarAxisY";
            this.trackBarAxisY.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.trackBarAxisY.Size = new System.Drawing.Size(45, 400);
            this.trackBarAxisY.SmallChange = 10;
            this.trackBarAxisY.TabIndex = 9;
            this.trackBarAxisY.TickFrequency = 100;
            this.trackBarAxisY.Scroll += new System.EventHandler(this.trackBarAxisYScroll);
            // 
            // trackBarAxisZ
            // 
            this.trackBarAxisZ.Location = new System.Drawing.Point(856, 113);
            this.trackBarAxisZ.Maximum = 10000;
            this.trackBarAxisZ.Minimum = -10000;
            this.trackBarAxisZ.Name = "trackBarAxisZ";
            this.trackBarAxisZ.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.trackBarAxisZ.Size = new System.Drawing.Size(45, 400);
            this.trackBarAxisZ.SmallChange = 10;
            this.trackBarAxisZ.TabIndex = 10;
            this.trackBarAxisZ.TickFrequency = 100;
            this.trackBarAxisZ.Value = -5000;
            this.trackBarAxisZ.Scroll += new System.EventHandler(this.trackBarAxisZScroll);
            // 
            // trackBarAxisX
            // 
            this.trackBarAxisX.Location = new System.Drawing.Point(754, 113);
            this.trackBarAxisX.Maximum = 10000;
            this.trackBarAxisX.Minimum = -10000;
            this.trackBarAxisX.Name = "trackBarAxisX";
            this.trackBarAxisX.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.trackBarAxisX.Size = new System.Drawing.Size(45, 400);
            this.trackBarAxisX.SmallChange = 10;
            this.trackBarAxisX.TabIndex = 11;
            this.trackBarAxisX.TickFrequency = 100;
            this.trackBarAxisX.Scroll += new System.EventHandler(this.trackBarAxisXScroll);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(754, 97);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 13);
            this.label3.TabIndex = 12;
            this.label3.Text = "axis X";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(805, 97);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(35, 13);
            this.label4.TabIndex = 13;
            this.label4.Text = "axis Y";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(856, 97);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(35, 13);
            this.label5.TabIndex = 14;
            this.label5.Text = "axis Z";
            // 
            // lAxisZ
            // 
            this.lAxisZ.AutoSize = true;
            this.lAxisZ.Location = new System.Drawing.Point(856, 531);
            this.lAxisZ.Name = "lAxisZ";
            this.lAxisZ.Size = new System.Drawing.Size(35, 13);
            this.lAxisZ.TabIndex = 17;
            this.lAxisZ.Text = "lAxisZ";
            // 
            // lAxisY
            // 
            this.lAxisY.AutoSize = true;
            this.lAxisY.Location = new System.Drawing.Point(805, 531);
            this.lAxisY.Name = "lAxisY";
            this.lAxisY.Size = new System.Drawing.Size(35, 13);
            this.lAxisY.TabIndex = 16;
            this.lAxisY.Text = "lAxisY";
            // 
            // lAxisX
            // 
            this.lAxisX.AutoSize = true;
            this.lAxisX.Location = new System.Drawing.Point(754, 531);
            this.lAxisX.Name = "lAxisX";
            this.lAxisX.Size = new System.Drawing.Size(35, 13);
            this.lAxisX.TabIndex = 15;
            this.lAxisX.Text = "lAxisX";
            // 
            // testToolStripMenuItem
            // 
            this.testToolStripMenuItem.Name = "testToolStripMenuItem";
            this.testToolStripMenuItem.Size = new System.Drawing.Size(40, 20);
            this.testToolStripMenuItem.Text = "Test";
            this.testToolStripMenuItem.Click += new System.EventHandler(this.testToolStripMenuItem_Click);
            // 
            // RenderForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1019, 616);
            this.Controls.Add(this.lAxisZ);
            this.Controls.Add(this.lAxisY);
            this.Controls.Add(this.lAxisX);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.trackBarAxisX);
            this.Controls.Add(this.trackBarAxisZ);
            this.Controls.Add(this.trackBarAxisY);
            this.Controls.Add(this.ExitBtn);
            this.Controls.Add(this.lAngle);
            this.Controls.Add(this.trackBarAngle);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lZoom);
            this.Controls.Add(this.trackBarZoom);
            this.Controls.Add(this.OpenGlWindow);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "RenderForm";
            this.Text = "Render of 3D objects";
            this.Load += new System.EventHandler(this.FormLoad);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarZoom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarAngle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarAxisY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarAxisZ)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarAxisX)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Tao.Platform.Windows.SimpleOpenGlControl OpenGlWindow;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.OpenFileDialog openFileDialogForModel;
        private System.Windows.Forms.OpenFileDialog openFileDialogForImg;
        private System.ComponentModel.BackgroundWorker backgroundWorker;
        private System.Windows.Forms.Timer RenderTimer;
        private System.Windows.Forms.TrackBar trackBarZoom;
        private System.Windows.Forms.Label lZoom;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TrackBar trackBarAngle;
        private System.Windows.Forms.Label lAngle;
        private System.Windows.Forms.Button ExitBtn;
        private System.Windows.Forms.TrackBar trackBarAxisY;
        private System.Windows.Forms.TrackBar trackBarAxisZ;
        private System.Windows.Forms.TrackBar trackBarAxisX;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lAxisZ;
        private System.Windows.Forms.Label lAxisY;
        private System.Windows.Forms.Label lAxisX;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialogForAnimation;
        private System.Windows.Forms.ToolStripMenuItem withMTLFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem withTXRFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadAnimExToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadASEModelToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadOBJModelToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadSimpleAnimationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem assimpMLToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem testToolStripMenuItem;
    }
}