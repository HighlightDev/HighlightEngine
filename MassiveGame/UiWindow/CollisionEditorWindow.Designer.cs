namespace MassiveGame.UI
{
    partial class CollisionEditorWindow
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
            this.GLControl = new OpenTK.GLControl();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.trackBarTranslationX = new System.Windows.Forms.TrackBar();
            this.label3 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.trackBarTranslationY = new System.Windows.Forms.TrackBar();
            this.label2 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.trackBarTranslationZ = new System.Windows.Forms.TrackBar();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.trackBarRotationX = new System.Windows.Forms.TrackBar();
            this.label4 = new System.Windows.Forms.Label();
            this.panel5 = new System.Windows.Forms.Panel();
            this.trackBarRotationY = new System.Windows.Forms.TrackBar();
            this.label5 = new System.Windows.Forms.Label();
            this.panel6 = new System.Windows.Forms.Panel();
            this.trackBarRotationZ = new System.Windows.Forms.TrackBar();
            this.label6 = new System.Windows.Forms.Label();
            this.panel7 = new System.Windows.Forms.Panel();
            this.trackBarScaleX = new System.Windows.Forms.TrackBar();
            this.label7 = new System.Windows.Forms.Label();
            this.panel8 = new System.Windows.Forms.Panel();
            this.trackBarScaleY = new System.Windows.Forms.TrackBar();
            this.label8 = new System.Windows.Forms.Label();
            this.panel9 = new System.Windows.Forms.Panel();
            this.trackBarScaleZ = new System.Windows.Forms.TrackBar();
            this.label9 = new System.Windows.Forms.Label();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.check_multipleAxistScale = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.panel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarTranslationX)).BeginInit();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarTranslationY)).BeginInit();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarTranslationZ)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarRotationX)).BeginInit();
            this.panel5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarRotationY)).BeginInit();
            this.panel6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarRotationZ)).BeginInit();
            this.panel7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarScaleX)).BeginInit();
            this.panel8.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarScaleY)).BeginInit();
            this.panel9.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarScaleZ)).BeginInit();
            this.flowLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // GLControl
            // 
            this.GLControl.BackColor = System.Drawing.Color.Black;
            this.GLControl.Dock = System.Windows.Forms.DockStyle.Left;
            this.GLControl.Location = new System.Drawing.Point(0, 0);
            this.GLControl.Name = "GLControl";
            this.GLControl.Size = new System.Drawing.Size(936, 753);
            this.GLControl.TabIndex = 0;
            this.GLControl.VSync = false;
            this.GLControl.Paint += new System.Windows.Forms.PaintEventHandler(this.RenderTick);
            this.GLControl.KeyDown += new System.Windows.Forms.KeyEventHandler(this.GLControl_KeyDown);
            this.GLControl.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.GLControl_KeyPress);
            this.GLControl.KeyUp += new System.Windows.Forms.KeyEventHandler(this.GLControl_KeyUp);
            this.GLControl.MouseDown += new System.Windows.Forms.MouseEventHandler(this.GLControl_MouseDown);
            this.GLControl.MouseMove += new System.Windows.Forms.MouseEventHandler(this.GLControl_MouseMove);
            this.GLControl.Resize += new System.EventHandler(this.ResizeCallback);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel2, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(936, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(567, 753);
            this.tableLayoutPanel1.TabIndex = 3;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.panel4);
            this.flowLayoutPanel1.Controls.Add(this.panel3);
            this.flowLayoutPanel1.Controls.Add(this.panel2);
            this.flowLayoutPanel1.Controls.Add(this.panel1);
            this.flowLayoutPanel1.Controls.Add(this.panel5);
            this.flowLayoutPanel1.Controls.Add(this.panel6);
            this.flowLayoutPanel1.Controls.Add(this.panel7);
            this.flowLayoutPanel1.Controls.Add(this.panel8);
            this.flowLayoutPanel1.Controls.Add(this.panel9);
            this.flowLayoutPanel1.Controls.Add(this.check_multipleAxistScale);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 379);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(561, 371);
            this.flowLayoutPanel1.TabIndex = 3;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.trackBarTranslationX);
            this.panel4.Controls.Add(this.label3);
            this.panel4.Location = new System.Drawing.Point(3, 3);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(181, 76);
            this.panel4.TabIndex = 9;
            // 
            // trackBarTranslationX
            // 
            this.trackBarTranslationX.Location = new System.Drawing.Point(3, 36);
            this.trackBarTranslationX.Maximum = 100;
            this.trackBarTranslationX.Name = "trackBarTranslationX";
            this.trackBarTranslationX.Size = new System.Drawing.Size(178, 45);
            this.trackBarTranslationX.TabIndex = 2;
            this.trackBarTranslationX.Value = 50;
            this.trackBarTranslationX.Scroll += new System.EventHandler(this.TrackBarValueChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 7);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(69, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Translation X";
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.trackBarTranslationY);
            this.panel3.Controls.Add(this.label2);
            this.panel3.Location = new System.Drawing.Point(190, 3);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(181, 76);
            this.panel3.TabIndex = 8;
            // 
            // trackBarTranslationY
            // 
            this.trackBarTranslationY.Location = new System.Drawing.Point(3, 36);
            this.trackBarTranslationY.Maximum = 100;
            this.trackBarTranslationY.Name = "trackBarTranslationY";
            this.trackBarTranslationY.Size = new System.Drawing.Size(178, 45);
            this.trackBarTranslationY.TabIndex = 2;
            this.trackBarTranslationY.Value = 50;
            this.trackBarTranslationY.Scroll += new System.EventHandler(this.TrackBarValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 7);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(69, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Translation Y";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.trackBarTranslationZ);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Location = new System.Drawing.Point(377, 3);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(181, 76);
            this.panel2.TabIndex = 7;
            // 
            // trackBarTranslationZ
            // 
            this.trackBarTranslationZ.Location = new System.Drawing.Point(3, 36);
            this.trackBarTranslationZ.Maximum = 100;
            this.trackBarTranslationZ.Name = "trackBarTranslationZ";
            this.trackBarTranslationZ.Size = new System.Drawing.Size(181, 45);
            this.trackBarTranslationZ.TabIndex = 2;
            this.trackBarTranslationZ.Value = 50;
            this.trackBarTranslationZ.Scroll += new System.EventHandler(this.TrackBarValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(69, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Translation Z";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.trackBarRotationX);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Location = new System.Drawing.Point(3, 85);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(181, 76);
            this.panel1.TabIndex = 12;
            // 
            // trackBarRotationX
            // 
            this.trackBarRotationX.Location = new System.Drawing.Point(3, 36);
            this.trackBarRotationX.Maximum = 100;
            this.trackBarRotationX.Name = "trackBarRotationX";
            this.trackBarRotationX.Size = new System.Drawing.Size(178, 45);
            this.trackBarRotationX.TabIndex = 2;
            this.trackBarRotationX.Value = 50;
            this.trackBarRotationX.Scroll += new System.EventHandler(this.TrackBarValueChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 7);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(57, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "Rotation X";
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.trackBarRotationY);
            this.panel5.Controls.Add(this.label5);
            this.panel5.Location = new System.Drawing.Point(190, 85);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(181, 76);
            this.panel5.TabIndex = 11;
            // 
            // trackBarRotationY
            // 
            this.trackBarRotationY.Location = new System.Drawing.Point(6, 36);
            this.trackBarRotationY.Maximum = 100;
            this.trackBarRotationY.Name = "trackBarRotationY";
            this.trackBarRotationY.Size = new System.Drawing.Size(178, 45);
            this.trackBarRotationY.TabIndex = 2;
            this.trackBarRotationY.Value = 50;
            this.trackBarRotationY.Scroll += new System.EventHandler(this.TrackBarValueChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 7);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(57, 13);
            this.label5.TabIndex = 3;
            this.label5.Text = "Rotation Y";
            // 
            // panel6
            // 
            this.panel6.Controls.Add(this.trackBarRotationZ);
            this.panel6.Controls.Add(this.label6);
            this.panel6.Location = new System.Drawing.Point(377, 85);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(181, 76);
            this.panel6.TabIndex = 10;
            // 
            // trackBarRotationZ
            // 
            this.trackBarRotationZ.Location = new System.Drawing.Point(3, 36);
            this.trackBarRotationZ.Maximum = 100;
            this.trackBarRotationZ.Name = "trackBarRotationZ";
            this.trackBarRotationZ.Size = new System.Drawing.Size(178, 45);
            this.trackBarRotationZ.TabIndex = 2;
            this.trackBarRotationZ.Value = 50;
            this.trackBarRotationZ.Scroll += new System.EventHandler(this.TrackBarValueChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 7);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(57, 13);
            this.label6.TabIndex = 3;
            this.label6.Text = "Rotation Z";
            // 
            // panel7
            // 
            this.panel7.Controls.Add(this.trackBarScaleX);
            this.panel7.Controls.Add(this.label7);
            this.panel7.Location = new System.Drawing.Point(3, 167);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(181, 76);
            this.panel7.TabIndex = 15;
            // 
            // trackBarScaleX
            // 
            this.trackBarScaleX.Location = new System.Drawing.Point(3, 36);
            this.trackBarScaleX.Maximum = 100;
            this.trackBarScaleX.Name = "trackBarScaleX";
            this.trackBarScaleX.Size = new System.Drawing.Size(178, 45);
            this.trackBarScaleX.TabIndex = 2;
            this.trackBarScaleX.Value = 50;
            this.trackBarScaleX.Scroll += new System.EventHandler(this.TrackBarValueChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(12, 7);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(44, 13);
            this.label7.TabIndex = 3;
            this.label7.Text = "Scale X";
            // 
            // panel8
            // 
            this.panel8.Controls.Add(this.trackBarScaleY);
            this.panel8.Controls.Add(this.label8);
            this.panel8.Location = new System.Drawing.Point(190, 167);
            this.panel8.Name = "panel8";
            this.panel8.Size = new System.Drawing.Size(181, 76);
            this.panel8.TabIndex = 14;
            // 
            // trackBarScaleY
            // 
            this.trackBarScaleY.Location = new System.Drawing.Point(3, 36);
            this.trackBarScaleY.Maximum = 100;
            this.trackBarScaleY.Name = "trackBarScaleY";
            this.trackBarScaleY.Size = new System.Drawing.Size(178, 45);
            this.trackBarScaleY.TabIndex = 2;
            this.trackBarScaleY.Value = 50;
            this.trackBarScaleY.Scroll += new System.EventHandler(this.TrackBarValueChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(12, 7);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(44, 13);
            this.label8.TabIndex = 3;
            this.label8.Text = "Scale Y";
            // 
            // panel9
            // 
            this.panel9.Controls.Add(this.trackBarScaleZ);
            this.panel9.Controls.Add(this.label9);
            this.panel9.Location = new System.Drawing.Point(377, 167);
            this.panel9.Name = "panel9";
            this.panel9.Size = new System.Drawing.Size(181, 76);
            this.panel9.TabIndex = 13;
            // 
            // trackBarScaleZ
            // 
            this.trackBarScaleZ.Location = new System.Drawing.Point(3, 36);
            this.trackBarScaleZ.Maximum = 100;
            this.trackBarScaleZ.Name = "trackBarScaleZ";
            this.trackBarScaleZ.Size = new System.Drawing.Size(178, 45);
            this.trackBarScaleZ.TabIndex = 2;
            this.trackBarScaleZ.Value = 50;
            this.trackBarScaleZ.Scroll += new System.EventHandler(this.TrackBarValueChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(12, 7);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(44, 13);
            this.label9.TabIndex = 3;
            this.label9.Text = "Scale Z";
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.Controls.Add(this.button1);
            this.flowLayoutPanel2.Controls.Add(this.button2);
            this.flowLayoutPanel2.Controls.Add(this.button3);
            this.flowLayoutPanel2.Controls.Add(this.button4);
            this.flowLayoutPanel2.Controls.Add(this.button5);
            this.flowLayoutPanel2.Controls.Add(this.treeView1);
            this.flowLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel2.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel2.Location = new System.Drawing.Point(3, 3);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(561, 370);
            this.flowLayoutPanel2.TabIndex = 4;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(3, 3);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(113, 23);
            this.button1.TabIndex = 3;
            this.button1.Text = "Create Mesh";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.createMesh_B_click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(3, 32);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(113, 23);
            this.button2.TabIndex = 4;
            this.button2.Text = "Set Texture";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.setMeshTexture_B_click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(3, 61);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(113, 42);
            this.button3.TabIndex = 6;
            this.button3.Text = "Create CollisionComponent";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.createCollisionComponent_B_click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(3, 109);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(113, 41);
            this.button4.TabIndex = 7;
            this.button4.Text = "Serialize Collision Components";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.serialize_B_click);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(3, 156);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(113, 52);
            this.button5.TabIndex = 8;
            this.button5.Text = "Deserialize Collision Components";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.deserialize_B_click);
            // 
            // treeView1
            // 
            this.treeView1.Location = new System.Drawing.Point(122, 3);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(474, 358);
            this.treeView1.TabIndex = 5;
            // 
            // check_multipleAxistScale
            // 
            this.check_multipleAxistScale.AutoSize = true;
            this.check_multipleAxistScale.Location = new System.Drawing.Point(3, 249);
            this.check_multipleAxistScale.Name = "check_multipleAxistScale";
            this.check_multipleAxistScale.Size = new System.Drawing.Size(119, 17);
            this.check_multipleAxistScale.TabIndex = 16;
            this.check_multipleAxistScale.Text = "Multiple axis scaling";
            this.check_multipleAxistScale.UseVisualStyleBackColor = true;
            this.check_multipleAxistScale.CheckedChanged += new System.EventHandler(this.check_multipleAxistScale_CheckedChanged);
            // 
            // CollisionEditorWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1503, 753);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.GLControl);
            this.Name = "CollisionEditorWindow";
            this.Text = "Form1";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarTranslationX)).EndInit();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarTranslationY)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarTranslationZ)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarRotationX)).EndInit();
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarRotationY)).EndInit();
            this.panel6.ResumeLayout(false);
            this.panel6.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarRotationZ)).EndInit();
            this.panel7.ResumeLayout(false);
            this.panel7.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarScaleX)).EndInit();
            this.panel8.ResumeLayout(false);
            this.panel8.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarScaleY)).EndInit();
            this.panel9.ResumeLayout(false);
            this.panel9.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarScaleZ)).EndInit();
            this.flowLayoutPanel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private OpenTK.GLControl GLControl;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.TrackBar trackBarTranslationX;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.TrackBar trackBarTranslationY;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TrackBar trackBarTranslationZ;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TrackBar trackBarRotationX;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.TrackBar trackBarRotationY;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.TrackBar trackBarRotationZ;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Panel panel7;
        private System.Windows.Forms.TrackBar trackBarScaleX;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Panel panel8;
        private System.Windows.Forms.TrackBar trackBarScaleY;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Panel panel9;
        private System.Windows.Forms.TrackBar trackBarScaleZ;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.CheckBox check_multipleAxistScale;
    }
}

