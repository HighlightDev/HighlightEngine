namespace MassiveGame
{
    partial class AddModelForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddModelForm));
            this.TB_Translation_X = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.TB_Translation_Y = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.TB_Translation_Z = new System.Windows.Forms.TextBox();
            this.GB_Transformation = new System.Windows.Forms.GroupBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.CB_Transformation_Toggle = new System.Windows.Forms.CheckBox();
            this.B_Right_S_X = new System.Windows.Forms.Button();
            this.B_Right_R_X = new System.Windows.Forms.Button();
            this.B_Left_S_X = new System.Windows.Forms.Button();
            this.B_Right_T_X = new System.Windows.Forms.Button();
            this.B_Right_S_Y = new System.Windows.Forms.Button();
            this.CB_BiasModifier = new System.Windows.Forms.CheckBox();
            this.B_Left_R_X = new System.Windows.Forms.Button();
            this.B_Left_S_Y = new System.Windows.Forms.Button();
            this.B_Left_T_X = new System.Windows.Forms.Button();
            this.B_Right_S_Z = new System.Windows.Forms.Button();
            this.TB_Rotation_X = new System.Windows.Forms.TextBox();
            this.B_Left_S_Z = new System.Windows.Forms.Button();
            this.B_Right_R_Y = new System.Windows.Forms.Button();
            this.TB_Scale_X = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.TB_Scale_Z = new System.Windows.Forms.TextBox();
            this.B_Right_T_Y = new System.Windows.Forms.Button();
            this.TB_Scale_Y = new System.Windows.Forms.TextBox();
            this.B_Left_R_Y = new System.Windows.Forms.Button();
            this.label10 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.B_Left_T_Y = new System.Windows.Forms.Button();
            this.B_Right_R_Z = new System.Windows.Forms.Button();
            this.TB_Rotation_Z = new System.Windows.Forms.TextBox();
            this.B_Right_T_Z = new System.Windows.Forms.Button();
            this.B_Left_R_Z = new System.Windows.Forms.Button();
            this.TB_Rotation_Y = new System.Windows.Forms.TextBox();
            this.B_Left_T_Z = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.GB_Mesh = new System.Windows.Forms.GroupBox();
            this.TV_MeshSelection = new System.Windows.Forms.TreeView();
            this.CB_MeshSelection_Toggle = new System.Windows.Forms.CheckBox();
            this.B_Add = new System.Windows.Forms.Button();
            this.AddMeshMenu = new System.Windows.Forms.MenuStrip();
            this.TSMI_Close = new System.Windows.Forms.ToolStripMenuItem();
            this.TSMI_Pin = new System.Windows.Forms.ToolStripMenuItem();
            this.GB_Texturing = new System.Windows.Forms.GroupBox();
            this.TV_SM_Selection = new System.Windows.Forms.TreeView();
            this.TV_NM_Selection = new System.Windows.Forms.TreeView();
            this.TV_TA_Selection = new System.Windows.Forms.TreeView();
            this.CB_Texturing_Toggle = new System.Windows.Forms.CheckBox();
            this.CB_SM_Toggle = new System.Windows.Forms.CheckBox();
            this.CB_EnableSM = new System.Windows.Forms.CheckBox();
            this.CB_NM_Toggle = new System.Windows.Forms.CheckBox();
            this.CB_EnableNM = new System.Windows.Forms.CheckBox();
            this.CB_TA_Toggle = new System.Windows.Forms.CheckBox();
            this.CB_EnableTA = new System.Windows.Forms.CheckBox();
            this.Add_Button_Splitter = new System.Windows.Forms.Splitter();
            this.Resize_Label = new System.Windows.Forms.Label();
            this.B_Preview = new System.Windows.Forms.Button();
            this.GB_Transformation.SuspendLayout();
            this.GB_Mesh.SuspendLayout();
            this.AddMeshMenu.SuspendLayout();
            this.GB_Texturing.SuspendLayout();
            this.SuspendLayout();
            // 
            // TB_Translation_X
            // 
            this.TB_Translation_X.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TB_Translation_X.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.TB_Translation_X.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.TB_Translation_X.ForeColor = System.Drawing.Color.Black;
            this.TB_Translation_X.Location = new System.Drawing.Point(60, 41);
            this.TB_Translation_X.MaxLength = 8;
            this.TB_Translation_X.Name = "TB_Translation_X";
            this.TB_Translation_X.Size = new System.Drawing.Size(90, 22);
            this.TB_Translation_X.TabIndex = 1;
            this.TB_Translation_X.Text = "0";
            this.TB_Translation_X.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.TB_Translation_X.TextChanged += new System.EventHandler(this.TB_Translation_X_TextChange);
            this.TB_Translation_X.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TB_Translation_X_KeyPress);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label3.Location = new System.Drawing.Point(7, 44);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(19, 16);
            this.label3.TabIndex = 4;
            this.label3.Text = "X:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label4.Location = new System.Drawing.Point(7, 72);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(20, 16);
            this.label4.TabIndex = 6;
            this.label4.Text = "Y:";
            // 
            // TB_Translation_Y
            // 
            this.TB_Translation_Y.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TB_Translation_Y.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.TB_Translation_Y.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.TB_Translation_Y.ForeColor = System.Drawing.Color.Black;
            this.TB_Translation_Y.Location = new System.Drawing.Point(60, 69);
            this.TB_Translation_Y.MaxLength = 8;
            this.TB_Translation_Y.Name = "TB_Translation_Y";
            this.TB_Translation_Y.Size = new System.Drawing.Size(90, 22);
            this.TB_Translation_Y.TabIndex = 5;
            this.TB_Translation_Y.Text = "0";
            this.TB_Translation_Y.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.TB_Translation_Y.TextChanged += new System.EventHandler(this.TB_Translation_Y_TextChange);
            this.TB_Translation_Y.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TB_Translation_Y_KeyPress);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label5.Location = new System.Drawing.Point(7, 100);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(19, 16);
            this.label5.TabIndex = 8;
            this.label5.Text = "Z:";
            // 
            // TB_Translation_Z
            // 
            this.TB_Translation_Z.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TB_Translation_Z.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.TB_Translation_Z.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.TB_Translation_Z.ForeColor = System.Drawing.Color.Black;
            this.TB_Translation_Z.Location = new System.Drawing.Point(60, 97);
            this.TB_Translation_Z.MaxLength = 8;
            this.TB_Translation_Z.Name = "TB_Translation_Z";
            this.TB_Translation_Z.Size = new System.Drawing.Size(90, 22);
            this.TB_Translation_Z.TabIndex = 7;
            this.TB_Translation_Z.Text = "0";
            this.TB_Translation_Z.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.TB_Translation_Z.TextChanged += new System.EventHandler(this.TB_Translation_Z_TextChange);
            this.TB_Translation_Z.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TB_Translation_Z_KeyPress);
            // 
            // GB_Transformation
            // 
            this.GB_Transformation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.GB_Transformation.Controls.Add(this.label12);
            this.GB_Transformation.Controls.Add(this.label11);
            this.GB_Transformation.Controls.Add(this.label1);
            this.GB_Transformation.Controls.Add(this.CB_Transformation_Toggle);
            this.GB_Transformation.Controls.Add(this.B_Right_S_X);
            this.GB_Transformation.Controls.Add(this.B_Right_R_X);
            this.GB_Transformation.Controls.Add(this.B_Left_S_X);
            this.GB_Transformation.Controls.Add(this.B_Right_T_X);
            this.GB_Transformation.Controls.Add(this.B_Right_S_Y);
            this.GB_Transformation.Controls.Add(this.CB_BiasModifier);
            this.GB_Transformation.Controls.Add(this.B_Left_R_X);
            this.GB_Transformation.Controls.Add(this.B_Left_S_Y);
            this.GB_Transformation.Controls.Add(this.B_Left_T_X);
            this.GB_Transformation.Controls.Add(this.B_Right_S_Z);
            this.GB_Transformation.Controls.Add(this.TB_Rotation_X);
            this.GB_Transformation.Controls.Add(this.B_Left_S_Z);
            this.GB_Transformation.Controls.Add(this.B_Right_R_Y);
            this.GB_Transformation.Controls.Add(this.TB_Scale_X);
            this.GB_Transformation.Controls.Add(this.label8);
            this.GB_Transformation.Controls.Add(this.TB_Translation_X);
            this.GB_Transformation.Controls.Add(this.label9);
            this.GB_Transformation.Controls.Add(this.label2);
            this.GB_Transformation.Controls.Add(this.TB_Scale_Z);
            this.GB_Transformation.Controls.Add(this.B_Right_T_Y);
            this.GB_Transformation.Controls.Add(this.TB_Scale_Y);
            this.GB_Transformation.Controls.Add(this.B_Left_R_Y);
            this.GB_Transformation.Controls.Add(this.label10);
            this.GB_Transformation.Controls.Add(this.label5);
            this.GB_Transformation.Controls.Add(this.label6);
            this.GB_Transformation.Controls.Add(this.B_Left_T_Y);
            this.GB_Transformation.Controls.Add(this.B_Right_R_Z);
            this.GB_Transformation.Controls.Add(this.label3);
            this.GB_Transformation.Controls.Add(this.TB_Rotation_Z);
            this.GB_Transformation.Controls.Add(this.B_Right_T_Z);
            this.GB_Transformation.Controls.Add(this.B_Left_R_Z);
            this.GB_Transformation.Controls.Add(this.TB_Translation_Z);
            this.GB_Transformation.Controls.Add(this.TB_Rotation_Y);
            this.GB_Transformation.Controls.Add(this.B_Left_T_Z);
            this.GB_Transformation.Controls.Add(this.label7);
            this.GB_Transformation.Controls.Add(this.TB_Translation_Y);
            this.GB_Transformation.Controls.Add(this.label4);
            this.GB_Transformation.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.GB_Transformation.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.GB_Transformation.Location = new System.Drawing.Point(12, 348);
            this.GB_Transformation.Name = "GB_Transformation";
            this.GB_Transformation.Size = new System.Drawing.Size(181, 380);
            this.GB_Transformation.TabIndex = 9;
            this.GB_Transformation.TabStop = false;
            this.GB_Transformation.Text = "Mesh Transformation:";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(10, 243);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(46, 16);
            this.label12.TabIndex = 31;
            this.label12.Text = "Scale:";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(10, 132);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(61, 16);
            this.label11.TabIndex = 30;
            this.label11.Text = "Rotation:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(78, 16);
            this.label1.TabIndex = 29;
            this.label1.Text = "Translation:";
            // 
            // CB_Transformation_Toggle
            // 
            this.CB_Transformation_Toggle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.CB_Transformation_Toggle.AutoSize = true;
            this.CB_Transformation_Toggle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.CB_Transformation_Toggle.Cursor = System.Windows.Forms.Cursors.Hand;
            this.CB_Transformation_Toggle.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.CB_Transformation_Toggle.Location = new System.Drawing.Point(170, 7);
            this.CB_Transformation_Toggle.Name = "CB_Transformation_Toggle";
            this.CB_Transformation_Toggle.Size = new System.Drawing.Size(12, 11);
            this.CB_Transformation_Toggle.TabIndex = 28;
            this.CB_Transformation_Toggle.UseVisualStyleBackColor = false;
            this.CB_Transformation_Toggle.CheckedChanged += new System.EventHandler(this.CB_Transformation_Toggle_CheckedChanged);
            // 
            // B_Right_S_X
            // 
            this.B_Right_S_X.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.B_Right_S_X.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("B_Right_S_X.BackgroundImage")));
            this.B_Right_S_X.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.B_Right_S_X.Cursor = System.Windows.Forms.Cursors.Hand;
            this.B_Right_S_X.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.B_Right_S_X.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.B_Right_S_X.Location = new System.Drawing.Point(156, 267);
            this.B_Right_S_X.Name = "B_Right_S_X";
            this.B_Right_S_X.Size = new System.Drawing.Size(20, 20);
            this.B_Right_S_X.TabIndex = 21;
            this.B_Right_S_X.UseVisualStyleBackColor = true;
            this.B_Right_S_X.Click += new System.EventHandler(this.B_Right_S_X_Click);
            // 
            // B_Right_R_X
            // 
            this.B_Right_R_X.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.B_Right_R_X.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("B_Right_R_X.BackgroundImage")));
            this.B_Right_R_X.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.B_Right_R_X.Cursor = System.Windows.Forms.Cursors.Hand;
            this.B_Right_R_X.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.B_Right_R_X.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.B_Right_R_X.Location = new System.Drawing.Point(156, 154);
            this.B_Right_R_X.Name = "B_Right_R_X";
            this.B_Right_R_X.Size = new System.Drawing.Size(20, 20);
            this.B_Right_R_X.TabIndex = 27;
            this.B_Right_R_X.UseVisualStyleBackColor = true;
            this.B_Right_R_X.Click += new System.EventHandler(this.B_Right_R_X_Click);
            // 
            // B_Left_S_X
            // 
            this.B_Left_S_X.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("B_Left_S_X.BackgroundImage")));
            this.B_Left_S_X.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.B_Left_S_X.Cursor = System.Windows.Forms.Cursors.Hand;
            this.B_Left_S_X.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.B_Left_S_X.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.B_Left_S_X.Location = new System.Drawing.Point(34, 267);
            this.B_Left_S_X.Name = "B_Left_S_X";
            this.B_Left_S_X.Size = new System.Drawing.Size(20, 20);
            this.B_Left_S_X.TabIndex = 20;
            this.B_Left_S_X.UseVisualStyleBackColor = true;
            this.B_Left_S_X.Click += new System.EventHandler(this.B_Left_S_X_Click);
            // 
            // B_Right_T_X
            // 
            this.B_Right_T_X.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.B_Right_T_X.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("B_Right_T_X.BackgroundImage")));
            this.B_Right_T_X.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.B_Right_T_X.Cursor = System.Windows.Forms.Cursors.Hand;
            this.B_Right_T_X.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.B_Right_T_X.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.B_Right_T_X.Location = new System.Drawing.Point(156, 42);
            this.B_Right_T_X.Name = "B_Right_T_X";
            this.B_Right_T_X.Size = new System.Drawing.Size(20, 20);
            this.B_Right_T_X.TabIndex = 27;
            this.B_Right_T_X.UseVisualStyleBackColor = true;
            this.B_Right_T_X.Click += new System.EventHandler(this.B_Right_T_X_Click);
            // 
            // B_Right_S_Y
            // 
            this.B_Right_S_Y.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.B_Right_S_Y.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("B_Right_S_Y.BackgroundImage")));
            this.B_Right_S_Y.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.B_Right_S_Y.Cursor = System.Windows.Forms.Cursors.Hand;
            this.B_Right_S_Y.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.B_Right_S_Y.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.B_Right_S_Y.Location = new System.Drawing.Point(156, 295);
            this.B_Right_S_Y.Name = "B_Right_S_Y";
            this.B_Right_S_Y.Size = new System.Drawing.Size(20, 20);
            this.B_Right_S_Y.TabIndex = 19;
            this.B_Right_S_Y.UseVisualStyleBackColor = true;
            this.B_Right_S_Y.Click += new System.EventHandler(this.B_Right_S_Y_Click);
            // 
            // CB_BiasModifier
            // 
            this.CB_BiasModifier.AutoSize = true;
            this.CB_BiasModifier.Cursor = System.Windows.Forms.Cursors.Hand;
            this.CB_BiasModifier.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.CB_BiasModifier.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.CB_BiasModifier.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.CB_BiasModifier.Location = new System.Drawing.Point(7, 354);
            this.CB_BiasModifier.Name = "CB_BiasModifier";
            this.CB_BiasModifier.Size = new System.Drawing.Size(155, 20);
            this.CB_BiasModifier.TabIndex = 16;
            this.CB_BiasModifier.Text = "Make bias equal to 10";
            this.CB_BiasModifier.UseVisualStyleBackColor = true;
            this.CB_BiasModifier.CheckedChanged += new System.EventHandler(this.CB_BiasModifier_CheckedChanged);
            // 
            // B_Left_R_X
            // 
            this.B_Left_R_X.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("B_Left_R_X.BackgroundImage")));
            this.B_Left_R_X.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.B_Left_R_X.Cursor = System.Windows.Forms.Cursors.Hand;
            this.B_Left_R_X.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.B_Left_R_X.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.B_Left_R_X.Location = new System.Drawing.Point(34, 154);
            this.B_Left_R_X.Name = "B_Left_R_X";
            this.B_Left_R_X.Size = new System.Drawing.Size(20, 20);
            this.B_Left_R_X.TabIndex = 26;
            this.B_Left_R_X.UseVisualStyleBackColor = true;
            this.B_Left_R_X.Click += new System.EventHandler(this.B_Left_R_X_Click);
            // 
            // B_Left_S_Y
            // 
            this.B_Left_S_Y.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("B_Left_S_Y.BackgroundImage")));
            this.B_Left_S_Y.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.B_Left_S_Y.Cursor = System.Windows.Forms.Cursors.Hand;
            this.B_Left_S_Y.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.B_Left_S_Y.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.B_Left_S_Y.Location = new System.Drawing.Point(34, 295);
            this.B_Left_S_Y.Name = "B_Left_S_Y";
            this.B_Left_S_Y.Size = new System.Drawing.Size(20, 20);
            this.B_Left_S_Y.TabIndex = 18;
            this.B_Left_S_Y.UseVisualStyleBackColor = true;
            this.B_Left_S_Y.Click += new System.EventHandler(this.B_Left_S_Y_Click);
            // 
            // B_Left_T_X
            // 
            this.B_Left_T_X.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("B_Left_T_X.BackgroundImage")));
            this.B_Left_T_X.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.B_Left_T_X.Cursor = System.Windows.Forms.Cursors.Hand;
            this.B_Left_T_X.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.B_Left_T_X.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.B_Left_T_X.Location = new System.Drawing.Point(34, 42);
            this.B_Left_T_X.Name = "B_Left_T_X";
            this.B_Left_T_X.Size = new System.Drawing.Size(20, 20);
            this.B_Left_T_X.TabIndex = 26;
            this.B_Left_T_X.UseVisualStyleBackColor = true;
            this.B_Left_T_X.Click += new System.EventHandler(this.B_Left_T_X_Click);
            // 
            // B_Right_S_Z
            // 
            this.B_Right_S_Z.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.B_Right_S_Z.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("B_Right_S_Z.BackgroundImage")));
            this.B_Right_S_Z.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.B_Right_S_Z.Cursor = System.Windows.Forms.Cursors.Hand;
            this.B_Right_S_Z.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.B_Right_S_Z.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.B_Right_S_Z.Location = new System.Drawing.Point(156, 323);
            this.B_Right_S_Z.Name = "B_Right_S_Z";
            this.B_Right_S_Z.Size = new System.Drawing.Size(20, 20);
            this.B_Right_S_Z.TabIndex = 17;
            this.B_Right_S_Z.UseVisualStyleBackColor = true;
            this.B_Right_S_Z.Click += new System.EventHandler(this.B_Right_S_Z_Click);
            // 
            // TB_Rotation_X
            // 
            this.TB_Rotation_X.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TB_Rotation_X.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.TB_Rotation_X.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.TB_Rotation_X.ForeColor = System.Drawing.Color.Black;
            this.TB_Rotation_X.Location = new System.Drawing.Point(60, 153);
            this.TB_Rotation_X.MaxLength = 8;
            this.TB_Rotation_X.Name = "TB_Rotation_X";
            this.TB_Rotation_X.Size = new System.Drawing.Size(90, 22);
            this.TB_Rotation_X.TabIndex = 1;
            this.TB_Rotation_X.Text = "0";
            this.TB_Rotation_X.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.TB_Rotation_X.TextChanged += new System.EventHandler(this.TB_Rotation_X_TextChange);
            this.TB_Rotation_X.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TB_Rotation_X_KeyPress);
            // 
            // B_Left_S_Z
            // 
            this.B_Left_S_Z.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("B_Left_S_Z.BackgroundImage")));
            this.B_Left_S_Z.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.B_Left_S_Z.Cursor = System.Windows.Forms.Cursors.Hand;
            this.B_Left_S_Z.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.B_Left_S_Z.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.B_Left_S_Z.Location = new System.Drawing.Point(34, 323);
            this.B_Left_S_Z.Name = "B_Left_S_Z";
            this.B_Left_S_Z.Size = new System.Drawing.Size(20, 20);
            this.B_Left_S_Z.TabIndex = 16;
            this.B_Left_S_Z.UseVisualStyleBackColor = true;
            this.B_Left_S_Z.Click += new System.EventHandler(this.B_Left_S_Z_Click);
            // 
            // B_Right_R_Y
            // 
            this.B_Right_R_Y.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.B_Right_R_Y.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("B_Right_R_Y.BackgroundImage")));
            this.B_Right_R_Y.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.B_Right_R_Y.Cursor = System.Windows.Forms.Cursors.Hand;
            this.B_Right_R_Y.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.B_Right_R_Y.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.B_Right_R_Y.Location = new System.Drawing.Point(156, 182);
            this.B_Right_R_Y.Name = "B_Right_R_Y";
            this.B_Right_R_Y.Size = new System.Drawing.Size(20, 20);
            this.B_Right_R_Y.TabIndex = 25;
            this.B_Right_R_Y.UseVisualStyleBackColor = true;
            this.B_Right_R_Y.Click += new System.EventHandler(this.B_Right_R_Y_Click);
            // 
            // TB_Scale_X
            // 
            this.TB_Scale_X.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TB_Scale_X.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.TB_Scale_X.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.TB_Scale_X.ForeColor = System.Drawing.Color.Black;
            this.TB_Scale_X.Location = new System.Drawing.Point(60, 266);
            this.TB_Scale_X.MaxLength = 8;
            this.TB_Scale_X.Name = "TB_Scale_X";
            this.TB_Scale_X.Size = new System.Drawing.Size(90, 22);
            this.TB_Scale_X.TabIndex = 1;
            this.TB_Scale_X.Text = "0";
            this.TB_Scale_X.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.TB_Scale_X.TextChanged += new System.EventHandler(this.TB_Scale_X_TextChange);
            this.TB_Scale_X.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TB_Scale_X_KeyPress);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label8.Location = new System.Drawing.Point(7, 325);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(19, 16);
            this.label8.TabIndex = 8;
            this.label8.Text = "Z:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label9.Location = new System.Drawing.Point(7, 269);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(19, 16);
            this.label9.TabIndex = 4;
            this.label9.Text = "X:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(7, 212);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(19, 16);
            this.label2.TabIndex = 8;
            this.label2.Text = "Z:";
            // 
            // TB_Scale_Z
            // 
            this.TB_Scale_Z.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TB_Scale_Z.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.TB_Scale_Z.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.TB_Scale_Z.ForeColor = System.Drawing.Color.Black;
            this.TB_Scale_Z.Location = new System.Drawing.Point(60, 322);
            this.TB_Scale_Z.MaxLength = 8;
            this.TB_Scale_Z.Name = "TB_Scale_Z";
            this.TB_Scale_Z.Size = new System.Drawing.Size(90, 22);
            this.TB_Scale_Z.TabIndex = 7;
            this.TB_Scale_Z.Text = "0";
            this.TB_Scale_Z.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.TB_Scale_Z.TextChanged += new System.EventHandler(this.TB_Scale_Z_TextChange);
            this.TB_Scale_Z.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TB_Scale_Z_KeyPress);
            // 
            // B_Right_T_Y
            // 
            this.B_Right_T_Y.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.B_Right_T_Y.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("B_Right_T_Y.BackgroundImage")));
            this.B_Right_T_Y.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.B_Right_T_Y.Cursor = System.Windows.Forms.Cursors.Hand;
            this.B_Right_T_Y.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.B_Right_T_Y.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.B_Right_T_Y.Location = new System.Drawing.Point(156, 70);
            this.B_Right_T_Y.Name = "B_Right_T_Y";
            this.B_Right_T_Y.Size = new System.Drawing.Size(20, 20);
            this.B_Right_T_Y.TabIndex = 25;
            this.B_Right_T_Y.UseVisualStyleBackColor = true;
            this.B_Right_T_Y.Click += new System.EventHandler(this.B_Right_T_Y_Click);
            // 
            // TB_Scale_Y
            // 
            this.TB_Scale_Y.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TB_Scale_Y.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.TB_Scale_Y.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.TB_Scale_Y.ForeColor = System.Drawing.Color.Black;
            this.TB_Scale_Y.Location = new System.Drawing.Point(60, 294);
            this.TB_Scale_Y.MaxLength = 8;
            this.TB_Scale_Y.Name = "TB_Scale_Y";
            this.TB_Scale_Y.Size = new System.Drawing.Size(90, 22);
            this.TB_Scale_Y.TabIndex = 5;
            this.TB_Scale_Y.Text = "0";
            this.TB_Scale_Y.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.TB_Scale_Y.TextChanged += new System.EventHandler(this.TB_Scale_Y_TextChange);
            this.TB_Scale_Y.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TB_Scale_Y_KeyPress);
            // 
            // B_Left_R_Y
            // 
            this.B_Left_R_Y.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("B_Left_R_Y.BackgroundImage")));
            this.B_Left_R_Y.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.B_Left_R_Y.Cursor = System.Windows.Forms.Cursors.Hand;
            this.B_Left_R_Y.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.B_Left_R_Y.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.B_Left_R_Y.Location = new System.Drawing.Point(34, 182);
            this.B_Left_R_Y.Name = "B_Left_R_Y";
            this.B_Left_R_Y.Size = new System.Drawing.Size(20, 20);
            this.B_Left_R_Y.TabIndex = 24;
            this.B_Left_R_Y.UseVisualStyleBackColor = true;
            this.B_Left_R_Y.Click += new System.EventHandler(this.B_Left_R_Y_Click);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label10.Location = new System.Drawing.Point(7, 297);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(20, 16);
            this.label10.TabIndex = 6;
            this.label10.Text = "Y:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label6.Location = new System.Drawing.Point(7, 156);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(19, 16);
            this.label6.TabIndex = 4;
            this.label6.Text = "X:";
            // 
            // B_Left_T_Y
            // 
            this.B_Left_T_Y.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("B_Left_T_Y.BackgroundImage")));
            this.B_Left_T_Y.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.B_Left_T_Y.Cursor = System.Windows.Forms.Cursors.Hand;
            this.B_Left_T_Y.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.B_Left_T_Y.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.B_Left_T_Y.Location = new System.Drawing.Point(34, 70);
            this.B_Left_T_Y.Name = "B_Left_T_Y";
            this.B_Left_T_Y.Size = new System.Drawing.Size(20, 20);
            this.B_Left_T_Y.TabIndex = 24;
            this.B_Left_T_Y.UseVisualStyleBackColor = true;
            this.B_Left_T_Y.Click += new System.EventHandler(this.B_Left_T_Y_Click);
            // 
            // B_Right_R_Z
            // 
            this.B_Right_R_Z.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.B_Right_R_Z.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("B_Right_R_Z.BackgroundImage")));
            this.B_Right_R_Z.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.B_Right_R_Z.Cursor = System.Windows.Forms.Cursors.Hand;
            this.B_Right_R_Z.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.B_Right_R_Z.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.B_Right_R_Z.Location = new System.Drawing.Point(156, 210);
            this.B_Right_R_Z.Name = "B_Right_R_Z";
            this.B_Right_R_Z.Size = new System.Drawing.Size(20, 20);
            this.B_Right_R_Z.TabIndex = 23;
            this.B_Right_R_Z.UseVisualStyleBackColor = true;
            this.B_Right_R_Z.Click += new System.EventHandler(this.B_Right_R_Z_Click);
            // 
            // TB_Rotation_Z
            // 
            this.TB_Rotation_Z.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TB_Rotation_Z.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.TB_Rotation_Z.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.TB_Rotation_Z.ForeColor = System.Drawing.Color.Black;
            this.TB_Rotation_Z.Location = new System.Drawing.Point(60, 209);
            this.TB_Rotation_Z.MaxLength = 8;
            this.TB_Rotation_Z.Name = "TB_Rotation_Z";
            this.TB_Rotation_Z.Size = new System.Drawing.Size(90, 22);
            this.TB_Rotation_Z.TabIndex = 7;
            this.TB_Rotation_Z.Text = "0";
            this.TB_Rotation_Z.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.TB_Rotation_Z.TextChanged += new System.EventHandler(this.TB_Rotation_Z_TextChange);
            this.TB_Rotation_Z.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TB_Rotation_Z_KeyPress);
            // 
            // B_Right_T_Z
            // 
            this.B_Right_T_Z.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.B_Right_T_Z.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("B_Right_T_Z.BackgroundImage")));
            this.B_Right_T_Z.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.B_Right_T_Z.Cursor = System.Windows.Forms.Cursors.Hand;
            this.B_Right_T_Z.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.B_Right_T_Z.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.B_Right_T_Z.Location = new System.Drawing.Point(156, 98);
            this.B_Right_T_Z.Name = "B_Right_T_Z";
            this.B_Right_T_Z.Size = new System.Drawing.Size(20, 20);
            this.B_Right_T_Z.TabIndex = 23;
            this.B_Right_T_Z.UseVisualStyleBackColor = true;
            this.B_Right_T_Z.Click += new System.EventHandler(this.B_Right_T_Z_Click);
            // 
            // B_Left_R_Z
            // 
            this.B_Left_R_Z.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("B_Left_R_Z.BackgroundImage")));
            this.B_Left_R_Z.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.B_Left_R_Z.Cursor = System.Windows.Forms.Cursors.Hand;
            this.B_Left_R_Z.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.B_Left_R_Z.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.B_Left_R_Z.Location = new System.Drawing.Point(34, 210);
            this.B_Left_R_Z.Name = "B_Left_R_Z";
            this.B_Left_R_Z.Size = new System.Drawing.Size(20, 20);
            this.B_Left_R_Z.TabIndex = 22;
            this.B_Left_R_Z.UseVisualStyleBackColor = true;
            this.B_Left_R_Z.Click += new System.EventHandler(this.B_Left_R_Z_Click);
            // 
            // TB_Rotation_Y
            // 
            this.TB_Rotation_Y.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TB_Rotation_Y.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.TB_Rotation_Y.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.TB_Rotation_Y.ForeColor = System.Drawing.Color.Black;
            this.TB_Rotation_Y.Location = new System.Drawing.Point(60, 181);
            this.TB_Rotation_Y.MaxLength = 8;
            this.TB_Rotation_Y.Name = "TB_Rotation_Y";
            this.TB_Rotation_Y.Size = new System.Drawing.Size(90, 22);
            this.TB_Rotation_Y.TabIndex = 5;
            this.TB_Rotation_Y.Text = "0";
            this.TB_Rotation_Y.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.TB_Rotation_Y.TextChanged += new System.EventHandler(this.TB_Rotation_Y_TextChange);
            this.TB_Rotation_Y.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TB_Rotation_Y_KeyPress);
            // 
            // B_Left_T_Z
            // 
            this.B_Left_T_Z.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("B_Left_T_Z.BackgroundImage")));
            this.B_Left_T_Z.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.B_Left_T_Z.Cursor = System.Windows.Forms.Cursors.Hand;
            this.B_Left_T_Z.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.B_Left_T_Z.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.B_Left_T_Z.Location = new System.Drawing.Point(34, 98);
            this.B_Left_T_Z.Name = "B_Left_T_Z";
            this.B_Left_T_Z.Size = new System.Drawing.Size(20, 20);
            this.B_Left_T_Z.TabIndex = 22;
            this.B_Left_T_Z.UseVisualStyleBackColor = true;
            this.B_Left_T_Z.Click += new System.EventHandler(this.B_Left_T_Z_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label7.Location = new System.Drawing.Point(7, 184);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(20, 16);
            this.label7.TabIndex = 6;
            this.label7.Text = "Y:";
            // 
            // GB_Mesh
            // 
            this.GB_Mesh.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.GB_Mesh.Controls.Add(this.TV_MeshSelection);
            this.GB_Mesh.Controls.Add(this.CB_MeshSelection_Toggle);
            this.GB_Mesh.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.GB_Mesh.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.GB_Mesh.Location = new System.Drawing.Point(12, 32);
            this.GB_Mesh.Name = "GB_Mesh";
            this.GB_Mesh.Size = new System.Drawing.Size(181, 300);
            this.GB_Mesh.TabIndex = 12;
            this.GB_Mesh.TabStop = false;
            this.GB_Mesh.Text = "Mesh Selection:";
            // 
            // TV_MeshSelection
            // 
            this.TV_MeshSelection.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.TV_MeshSelection.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TV_MeshSelection.FullRowSelect = true;
            this.TV_MeshSelection.Location = new System.Drawing.Point(3, 18);
            this.TV_MeshSelection.Name = "TV_MeshSelection";
            this.TV_MeshSelection.Size = new System.Drawing.Size(175, 279);
            this.TV_MeshSelection.TabIndex = 30;
            this.TV_MeshSelection.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.TV_MeshSelection_AfterSelect);
            // 
            // CB_MeshSelection_Toggle
            // 
            this.CB_MeshSelection_Toggle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.CB_MeshSelection_Toggle.AutoSize = true;
            this.CB_MeshSelection_Toggle.Cursor = System.Windows.Forms.Cursors.Hand;
            this.CB_MeshSelection_Toggle.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.CB_MeshSelection_Toggle.Location = new System.Drawing.Point(170, 7);
            this.CB_MeshSelection_Toggle.Name = "CB_MeshSelection_Toggle";
            this.CB_MeshSelection_Toggle.Size = new System.Drawing.Size(12, 11);
            this.CB_MeshSelection_Toggle.TabIndex = 29;
            this.CB_MeshSelection_Toggle.UseVisualStyleBackColor = true;
            this.CB_MeshSelection_Toggle.CheckedChanged += new System.EventHandler(this.CB_MeshSelection_Toggle_CheckedChanged);
            // 
            // B_Add
            // 
            this.B_Add.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.B_Add.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.B_Add.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.B_Add.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.B_Add.Location = new System.Drawing.Point(12, 1014);
            this.B_Add.Name = "B_Add";
            this.B_Add.Size = new System.Drawing.Size(181, 25);
            this.B_Add.TabIndex = 13;
            this.B_Add.Text = "Add mesh";
            this.B_Add.UseVisualStyleBackColor = true;
            this.B_Add.Click += new System.EventHandler(this.B_Add_Click);
            // 
            // AddMeshMenu
            // 
            this.AddMeshMenu.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.AddMeshMenu.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.AddMeshMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TSMI_Close,
            this.TSMI_Pin});
            this.AddMeshMenu.Location = new System.Drawing.Point(0, 0);
            this.AddMeshMenu.Name = "AddMeshMenu";
            this.AddMeshMenu.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.AddMeshMenu.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.AddMeshMenu.ShowItemToolTips = true;
            this.AddMeshMenu.Size = new System.Drawing.Size(205, 24);
            this.AddMeshMenu.TabIndex = 15;
            // 
            // TSMI_Close
            // 
            this.TSMI_Close.AutoToolTip = true;
            this.TSMI_Close.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.TSMI_Close.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.TSMI_Close.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.TSMI_Close.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.TSMI_Close.Image = ((System.Drawing.Image)(resources.GetObject("TSMI_Close.Image")));
            this.TSMI_Close.Name = "TSMI_Close";
            this.TSMI_Close.ShowShortcutKeys = false;
            this.TSMI_Close.Size = new System.Drawing.Size(28, 20);
            this.TSMI_Close.ToolTipText = "Hide";
            this.TSMI_Close.Click += new System.EventHandler(this.Close_Click);
            // 
            // TSMI_Pin
            // 
            this.TSMI_Pin.AutoToolTip = true;
            this.TSMI_Pin.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.TSMI_Pin.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.TSMI_Pin.Image = ((System.Drawing.Image)(resources.GetObject("TSMI_Pin.Image")));
            this.TSMI_Pin.Name = "TSMI_Pin";
            this.TSMI_Pin.Size = new System.Drawing.Size(28, 20);
            this.TSMI_Pin.ToolTipText = "Detach";
            this.TSMI_Pin.Click += new System.EventHandler(this.Pin_Click);
            // 
            // GB_Texturing
            // 
            this.GB_Texturing.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.GB_Texturing.Controls.Add(this.TV_SM_Selection);
            this.GB_Texturing.Controls.Add(this.TV_NM_Selection);
            this.GB_Texturing.Controls.Add(this.TV_TA_Selection);
            this.GB_Texturing.Controls.Add(this.CB_Texturing_Toggle);
            this.GB_Texturing.Controls.Add(this.CB_SM_Toggle);
            this.GB_Texturing.Controls.Add(this.CB_EnableSM);
            this.GB_Texturing.Controls.Add(this.CB_NM_Toggle);
            this.GB_Texturing.Controls.Add(this.CB_EnableNM);
            this.GB_Texturing.Controls.Add(this.CB_TA_Toggle);
            this.GB_Texturing.Controls.Add(this.CB_EnableTA);
            this.GB_Texturing.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.GB_Texturing.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.GB_Texturing.Location = new System.Drawing.Point(12, 744);
            this.GB_Texturing.Name = "GB_Texturing";
            this.GB_Texturing.Size = new System.Drawing.Size(181, 196);
            this.GB_Texturing.TabIndex = 17;
            this.GB_Texturing.TabStop = false;
            this.GB_Texturing.Text = "Texturing:";
            this.GB_Texturing.Enter += new System.EventHandler(this.GB_Texturing_Enter);
            // 
            // TV_SM_Selection
            // 
            this.TV_SM_Selection.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TV_SM_Selection.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.TV_SM_Selection.Location = new System.Drawing.Point(9, 179);
            this.TV_SM_Selection.Name = "TV_SM_Selection";
            this.TV_SM_Selection.Size = new System.Drawing.Size(166, 1);
            this.TV_SM_Selection.TabIndex = 34;
            this.TV_SM_Selection.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.TV_SM_AfterSelect);
            // 
            // TV_NM_Selection
            // 
            this.TV_NM_Selection.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TV_NM_Selection.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.TV_NM_Selection.Location = new System.Drawing.Point(9, 120);
            this.TV_NM_Selection.Name = "TV_NM_Selection";
            this.TV_NM_Selection.Size = new System.Drawing.Size(166, 1);
            this.TV_NM_Selection.TabIndex = 33;
            this.TV_NM_Selection.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.TV_NM_AfterSelect);
            // 
            // TV_TA_Selection
            // 
            this.TV_TA_Selection.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TV_TA_Selection.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.TV_TA_Selection.Location = new System.Drawing.Point(9, 61);
            this.TV_TA_Selection.Name = "TV_TA_Selection";
            this.TV_TA_Selection.Size = new System.Drawing.Size(166, 1);
            this.TV_TA_Selection.TabIndex = 32;
            this.TV_TA_Selection.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.TV_TA_AfterSelect);
            // 
            // CB_Texturing_Toggle
            // 
            this.CB_Texturing_Toggle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.CB_Texturing_Toggle.AutoSize = true;
            this.CB_Texturing_Toggle.Cursor = System.Windows.Forms.Cursors.Hand;
            this.CB_Texturing_Toggle.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.CB_Texturing_Toggle.Location = new System.Drawing.Point(170, 7);
            this.CB_Texturing_Toggle.Name = "CB_Texturing_Toggle";
            this.CB_Texturing_Toggle.Size = new System.Drawing.Size(12, 11);
            this.CB_Texturing_Toggle.TabIndex = 31;
            this.CB_Texturing_Toggle.UseVisualStyleBackColor = true;
            this.CB_Texturing_Toggle.CheckedChanged += new System.EventHandler(this.CB_Texturing_Toggle_CheckedChanged);
            // 
            // CB_SM_Toggle
            // 
            this.CB_SM_Toggle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.CB_SM_Toggle.AutoSize = true;
            this.CB_SM_Toggle.Checked = true;
            this.CB_SM_Toggle.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CB_SM_Toggle.Cursor = System.Windows.Forms.Cursors.Hand;
            this.CB_SM_Toggle.Enabled = false;
            this.CB_SM_Toggle.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.CB_SM_Toggle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.CB_SM_Toggle.Location = new System.Drawing.Point(108, 160);
            this.CB_SM_Toggle.Name = "CB_SM_Toggle";
            this.CB_SM_Toggle.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.CB_SM_Toggle.Size = new System.Drawing.Size(65, 17);
            this.CB_SM_Toggle.TabIndex = 8;
            this.CB_SM_Toggle.Text = "maximize";
            this.CB_SM_Toggle.UseVisualStyleBackColor = true;
            this.CB_SM_Toggle.CheckedChanged += new System.EventHandler(this.CB_SM_Toggle_CheckedChanged);
            // 
            // CB_EnableSM
            // 
            this.CB_EnableSM.AutoSize = true;
            this.CB_EnableSM.Cursor = System.Windows.Forms.Cursors.Hand;
            this.CB_EnableSM.Enabled = false;
            this.CB_EnableSM.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.CB_EnableSM.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.CB_EnableSM.Location = new System.Drawing.Point(9, 139);
            this.CB_EnableSM.Name = "CB_EnableSM";
            this.CB_EnableSM.Size = new System.Drawing.Size(140, 19);
            this.CB_EnableSM.TabIndex = 6;
            this.CB_EnableSM.Text = "Enable specular map";
            this.CB_EnableSM.UseVisualStyleBackColor = true;
            this.CB_EnableSM.CheckedChanged += new System.EventHandler(this.CB_EnableSM_CheckedChanged);
            // 
            // CB_NM_Toggle
            // 
            this.CB_NM_Toggle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.CB_NM_Toggle.AutoSize = true;
            this.CB_NM_Toggle.Checked = true;
            this.CB_NM_Toggle.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CB_NM_Toggle.Cursor = System.Windows.Forms.Cursors.Hand;
            this.CB_NM_Toggle.Enabled = false;
            this.CB_NM_Toggle.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.CB_NM_Toggle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.CB_NM_Toggle.Location = new System.Drawing.Point(108, 101);
            this.CB_NM_Toggle.Name = "CB_NM_Toggle";
            this.CB_NM_Toggle.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.CB_NM_Toggle.Size = new System.Drawing.Size(65, 17);
            this.CB_NM_Toggle.TabIndex = 5;
            this.CB_NM_Toggle.Text = "maximize";
            this.CB_NM_Toggle.UseVisualStyleBackColor = true;
            this.CB_NM_Toggle.CheckedChanged += new System.EventHandler(this.CB_NM_Toggle_CheckedChanged);
            // 
            // CB_EnableNM
            // 
            this.CB_EnableNM.AutoSize = true;
            this.CB_EnableNM.Cursor = System.Windows.Forms.Cursors.Hand;
            this.CB_EnableNM.Enabled = false;
            this.CB_EnableNM.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.CB_EnableNM.Location = new System.Drawing.Point(9, 80);
            this.CB_EnableNM.Name = "CB_EnableNM";
            this.CB_EnableNM.Size = new System.Drawing.Size(141, 20);
            this.CB_EnableNM.TabIndex = 3;
            this.CB_EnableNM.Text = "Enable normal map";
            this.CB_EnableNM.UseVisualStyleBackColor = true;
            this.CB_EnableNM.CheckedChanged += new System.EventHandler(this.CB_EnableNM_CheckedChanged);
            // 
            // CB_TA_Toggle
            // 
            this.CB_TA_Toggle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.CB_TA_Toggle.AutoSize = true;
            this.CB_TA_Toggle.Checked = true;
            this.CB_TA_Toggle.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CB_TA_Toggle.Cursor = System.Windows.Forms.Cursors.Hand;
            this.CB_TA_Toggle.Enabled = false;
            this.CB_TA_Toggle.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.CB_TA_Toggle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.CB_TA_Toggle.Location = new System.Drawing.Point(110, 42);
            this.CB_TA_Toggle.Name = "CB_TA_Toggle";
            this.CB_TA_Toggle.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.CB_TA_Toggle.Size = new System.Drawing.Size(65, 17);
            this.CB_TA_Toggle.TabIndex = 2;
            this.CB_TA_Toggle.Text = "maximize";
            this.CB_TA_Toggle.UseVisualStyleBackColor = true;
            this.CB_TA_Toggle.CheckedChanged += new System.EventHandler(this.CB_TA_Toggle_CheckedChanged);
            // 
            // CB_EnableTA
            // 
            this.CB_EnableTA.AutoSize = true;
            this.CB_EnableTA.Cursor = System.Windows.Forms.Cursors.Hand;
            this.CB_EnableTA.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.CB_EnableTA.Location = new System.Drawing.Point(9, 21);
            this.CB_EnableTA.Name = "CB_EnableTA";
            this.CB_EnableTA.Size = new System.Drawing.Size(141, 20);
            this.CB_EnableTA.TabIndex = 0;
            this.CB_EnableTA.Text = "Enable texture atlas";
            this.CB_EnableTA.UseVisualStyleBackColor = true;
            this.CB_EnableTA.CheckedChanged += new System.EventHandler(this.CB_EnableTA_CheckedChanged);
            // 
            // Add_Button_Splitter
            // 
            this.Add_Button_Splitter.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Add_Button_Splitter.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.Add_Button_Splitter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.Add_Button_Splitter.Location = new System.Drawing.Point(0, 968);
            this.Add_Button_Splitter.Name = "Add_Button_Splitter";
            this.Add_Button_Splitter.Size = new System.Drawing.Size(205, 83);
            this.Add_Button_Splitter.TabIndex = 18;
            this.Add_Button_Splitter.TabStop = false;
            // 
            // Resize_Label
            // 
            this.Resize_Label.Cursor = System.Windows.Forms.Cursors.SizeWE;
            this.Resize_Label.Dock = System.Windows.Forms.DockStyle.Right;
            this.Resize_Label.Location = new System.Drawing.Point(195, 24);
            this.Resize_Label.Name = "Resize_Label";
            this.Resize_Label.Size = new System.Drawing.Size(10, 944);
            this.Resize_Label.TabIndex = 19;
            this.Resize_Label.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Resize_Label_OnMouseDown);
            this.Resize_Label.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Resize_Label_OnMouseUp);
            // 
            // B_Preview
            // 
            this.B_Preview.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.B_Preview.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.B_Preview.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.B_Preview.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.B_Preview.Location = new System.Drawing.Point(12, 983);
            this.B_Preview.Name = "B_Preview";
            this.B_Preview.Size = new System.Drawing.Size(181, 25);
            this.B_Preview.TabIndex = 21;
            this.B_Preview.Text = "Preview";
            this.B_Preview.UseVisualStyleBackColor = true;
            this.B_Preview.Click += new System.EventHandler(this.B_Preview_Click);
            // 
            // AddModelForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ClientSize = new System.Drawing.Size(205, 1051);
            this.Controls.Add(this.B_Preview);
            this.Controls.Add(this.Resize_Label);
            this.Controls.Add(this.AddMeshMenu);
            this.Controls.Add(this.B_Add);
            this.Controls.Add(this.Add_Button_Splitter);
            this.Controls.Add(this.GB_Texturing);
            this.Controls.Add(this.GB_Mesh);
            this.Controls.Add(this.GB_Transformation);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MainMenuStrip = this.AddMeshMenu;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(300, 1080);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(205, 0);
            this.Name = "AddModelForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "AddModelForm";
            this.TopMost = true;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.OnClose);
            this.Load += new System.EventHandler(this.OnLoad);
            this.MouseEnter += new System.EventHandler(this.OnMouseEnter);
            this.MouseLeave += new System.EventHandler(this.OnMouseLeave);
            this.GB_Transformation.ResumeLayout(false);
            this.GB_Transformation.PerformLayout();
            this.GB_Mesh.ResumeLayout(false);
            this.GB_Mesh.PerformLayout();
            this.AddMeshMenu.ResumeLayout(false);
            this.AddMeshMenu.PerformLayout();
            this.GB_Texturing.ResumeLayout(false);
            this.GB_Texturing.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox TB_Translation_X;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox TB_Translation_Y;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox TB_Translation_Z;
        private System.Windows.Forms.GroupBox GB_Transformation;
        private System.Windows.Forms.TextBox TB_Rotation_X;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox TB_Rotation_Z;
        private System.Windows.Forms.TextBox TB_Rotation_Y;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox TB_Scale_X;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox TB_Scale_Z;
        private System.Windows.Forms.TextBox TB_Scale_Y;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.GroupBox GB_Mesh;
        private System.Windows.Forms.Button B_Add;
        private System.Windows.Forms.MenuStrip AddMeshMenu;
        private System.Windows.Forms.ToolStripMenuItem TSMI_Close;
        private System.Windows.Forms.Button B_Left_S_Z;
        private System.Windows.Forms.Button B_Right_S_Z;
        private System.Windows.Forms.Button B_Right_T_X;
        private System.Windows.Forms.Button B_Left_T_X;
        private System.Windows.Forms.Button B_Right_T_Y;
        private System.Windows.Forms.Button B_Left_T_Y;
        private System.Windows.Forms.Button B_Right_T_Z;
        private System.Windows.Forms.Button B_Left_T_Z;
        private System.Windows.Forms.Button B_Right_R_X;
        private System.Windows.Forms.Button B_Left_R_X;
        private System.Windows.Forms.Button B_Right_R_Y;
        private System.Windows.Forms.Button B_Left_R_Y;
        private System.Windows.Forms.Button B_Right_R_Z;
        private System.Windows.Forms.Button B_Left_R_Z;
        private System.Windows.Forms.Button B_Right_S_X;
        private System.Windows.Forms.Button B_Left_S_X;
        private System.Windows.Forms.Button B_Right_S_Y;
        private System.Windows.Forms.Button B_Left_S_Y;
        private System.Windows.Forms.CheckBox CB_BiasModifier;
        private System.Windows.Forms.CheckBox CB_Transformation_Toggle;
        private System.Windows.Forms.CheckBox CB_MeshSelection_Toggle;
        private System.Windows.Forms.GroupBox GB_Texturing;
        private System.Windows.Forms.CheckBox CB_EnableTA;
        private System.Windows.Forms.CheckBox CB_Texturing_Toggle;
        private System.Windows.Forms.CheckBox CB_SM_Toggle;
        private System.Windows.Forms.CheckBox CB_EnableSM;
        private System.Windows.Forms.CheckBox CB_NM_Toggle;
        private System.Windows.Forms.CheckBox CB_EnableNM;
        private System.Windows.Forms.CheckBox CB_TA_Toggle;
        private System.Windows.Forms.Splitter Add_Button_Splitter;
        private System.Windows.Forms.ToolStripMenuItem TSMI_Pin;
        private System.Windows.Forms.Label Resize_Label;
        private System.Windows.Forms.Button B_Preview;
        private System.Windows.Forms.TreeView TV_MeshSelection;
        private System.Windows.Forms.TreeView TV_SM_Selection;
        private System.Windows.Forms.TreeView TV_NM_Selection;
        private System.Windows.Forms.TreeView TV_TA_Selection;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label1;
    }
}