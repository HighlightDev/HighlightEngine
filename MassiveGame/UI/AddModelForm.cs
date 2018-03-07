using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace MassiveGame
{
    public partial class AddModelForm : Form
    {
        public AddModelForm()
        {
            InitializeComponent();
            InitializeVariables();
        }

        #region Form actions

        private void OnLoad(object sender, EventArgs e)
        {
            GetListOfModelFiles();
            GetListOfTextureFiles(ProjectFolders.TextureFolders.TextureAtlasPath, TV_TA_Selection);
            GetListOfTextureFiles(ProjectFolders.TextureFolders.NormalMapsPath, TV_NM_Selection);
            GetListOfTextureFiles(ProjectFolders.TextureFolders.SpecularMapsPath, TV_SM_Selection);
            
            gb_mesh_height = GB_Mesh.Height;
            gb_transformation_height = GB_Transformation.Height;

            CB_Transformation_Toggle.Checked = true;
            CB_Texturing_Toggle.Checked = true;
        }

        private void OnClose(object sender, FormClosedEventArgs e)
        {
            CleanUp();
        }

        private void OnMouseEnter(object sender, EventArgs e)
        {
            Opacity = 1;
        }

        private void OnMouseLeave(object sender, EventArgs e)
        {
            if ((Cursor.Position.X >= Left) && (Cursor.Position.X <= Right))
            {
                if ((Cursor.Position.Y >= Top) && (Cursor.Position.Y <= Bottom))
                {
                    Opacity = 1;
                }
                else
                {
                    if (windowBorderTrigger) // if not borderless
                        Opacity = 1;
                    else
                        Opacity = 0.5;
                }
            }
            else
            {
                if (windowBorderTrigger) // if not borderless
                    Opacity = 1;
                else
                    Opacity = 0.5;
            }
        }

        #endregion


        #region Variables

        private ToolTip tip;
        private string selectedMesh,
            selectedTA,
            selectedNM,
            selectedSM;
        private string tranlation_x,
            tranlation_y,
            tranlation_z;
        private string rotation_x,
            rotation_y,
            rotation_z;
        private string scale_x,
            scale_y,
            scale_z;
        private int gb_mesh_height,
            gb_transformation_height,
            gb_texturing_height;
        private int minWidth,
            maxWidth;
        private bool _biasModifier;
        public bool windowBorderTrigger; // if false window is borderless

        #endregion


        #region Common functions

        private void InitializeVariables()
        {
            tip = new ToolTip();
            tip.SetToolTip(CB_MeshSelection_Toggle, "Maximize");
            tip.SetToolTip(CB_Transformation_Toggle, "Minimize");
            tip.SetToolTip(CB_Texturing_Toggle, "Maximize");

            tip.SetToolTip(Resize_Label, "Resize");

            SetArrowButtonToolTips("1");

            selectedMesh = String.Empty;
            selectedTA = String.Empty;
            selectedNM = String.Empty;
            selectedSM = String.Empty;

            tranlation_x = "0";
            tranlation_y = "0";
            tranlation_z = "0";
            rotation_x = "0";
            rotation_y = "0";
            rotation_z = "0";
            scale_x = "0";
            scale_y = "0";
            scale_z = "0";

            gb_mesh_height = 0;
            gb_transformation_height = 0;
            gb_texturing_height = 295;

            minWidth = 0;
            maxWidth = 0;

            _biasModifier = false;
            windowBorderTrigger = false;
        }

        private void SetArrowButtonToolTips(string tips)
        {
            tip.SetToolTip(B_Left_T_X, "-" + tips);
            tip.SetToolTip(B_Left_T_Y, "-" + tips);
            tip.SetToolTip(B_Left_T_Z, "-" + tips);
            tip.SetToolTip(B_Left_R_X, "-" + tips);
            tip.SetToolTip(B_Left_R_Y, "-" + tips);
            tip.SetToolTip(B_Left_R_Z, "-" + tips);
            tip.SetToolTip(B_Left_S_X, "-" + tips);
            tip.SetToolTip(B_Left_S_Y, "-" + tips);
            tip.SetToolTip(B_Left_S_Z, "-" + tips);

            tip.SetToolTip(B_Right_T_X, "+" + tips);
            tip.SetToolTip(B_Right_T_Y, "+" + tips);
            tip.SetToolTip(B_Right_T_Z, "+" + tips);
            tip.SetToolTip(B_Right_R_X, "+" + tips);
            tip.SetToolTip(B_Right_R_Y, "+" + tips);
            tip.SetToolTip(B_Right_R_Z, "+" + tips);
            tip.SetToolTip(B_Right_S_X, "+" + tips);
            tip.SetToolTip(B_Right_S_Y, "+" + tips);
            tip.SetToolTip(B_Right_S_Z, "+" + tips);
        }

        private int Bias(bool biasModifier)
        {
            return (biasModifier) ? 10 : 1;
        }

        private string GetTexturePaths()
        {
            if (CB_EnableNM.Checked)
                selectedNM = (selectedNM != String.Empty) ? GetNormalMapDirectory() : String.Empty;
            else
                selectedNM = String.Empty;

            if (CB_EnableSM.Checked)
                selectedSM = (selectedSM != String.Empty) ? GetSpecularMapDirectory() : String.Empty;
            else
                selectedSM = String.Empty;

            if ((selectedTA != String.Empty) && (selectedNM == String.Empty) && (selectedSM == String.Empty))
                return GetTextureAtlasDirectory();
            else if ((selectedTA != String.Empty) && (selectedNM != String.Empty) && (selectedSM == String.Empty))
                return GetTextureAtlasDirectory() + "|" + selectedNM;
            else if ((selectedTA != String.Empty) && (selectedNM != String.Empty) && (selectedSM != String.Empty))
                return GetTextureAtlasDirectory() + "|" + selectedNM + "|" + selectedSM;

            return String.Empty;
        }

        private void WriteMeshData()
        {
            StreamWriter sw_meshData = new StreamWriter("NewModel.msh");

            sw_meshData.WriteLine(GetModelDirectory());

            sw_meshData.WriteLine(GetTexturePaths());

            sw_meshData.WriteLine(tranlation_x);
            sw_meshData.WriteLine(tranlation_y);
            sw_meshData.WriteLine(tranlation_z);
            sw_meshData.WriteLine(rotation_x);
            sw_meshData.WriteLine(rotation_y);
            sw_meshData.WriteLine(rotation_z);
            sw_meshData.WriteLine(scale_x);
            sw_meshData.WriteLine(scale_y);
            sw_meshData.WriteLine(scale_z);

            sw_meshData.Close();
        }

        private void CleanUp()
        {
            selectedMesh = String.Empty;
            selectedTA = String.Empty;
            selectedNM = String.Empty;
            selectedSM = String.Empty;

            tranlation_x = String.Empty;
            tranlation_y = String.Empty;
            tranlation_z = String.Empty;
            rotation_x = String.Empty;
            rotation_y = String.Empty;
            rotation_z = String.Empty;
            scale_x = String.Empty;
            scale_y = String.Empty;
            scale_z = String.Empty;

            gb_mesh_height = 0;
            gb_transformation_height = 0;
            gb_texturing_height = 0;

            minWidth = 0;
            maxWidth = 0;

            tip.Dispose();
        }

        #endregion

        #region SubDirectory getters

        private string GetModelDirectory()
        {
            if (File.Exists(ProjectFolders.ModelsPath + selectedMesh))
            {
                return ProjectFolders.ModelsPath + selectedMesh;
            }
            else
            {
                string[] subdirectories = Directory.GetDirectories(ProjectFolders.ModelsPath);

                foreach (string subdirectory in subdirectories)
                {
                    if (File.Exists(subdirectory + "/" + selectedMesh))
                        return subdirectory + "/" + selectedMesh;
                }

                return selectedMesh;
            }
        }

        private string GetTextureAtlasDirectory()
        {
            if (File.Exists(ProjectFolders.TextureFolders.TextureAtlasPath + selectedTA))
            {
                return ProjectFolders.TextureFolders.TextureAtlasPath + selectedTA;
            }
            else
            {
                string[] subdirectories = Directory.GetDirectories(ProjectFolders.TextureFolders.TextureAtlasPath);

                foreach (string subdirectory in subdirectories)
                {
                    if (File.Exists(subdirectory + "/" + selectedTA))
                        return subdirectory + "/" + selectedTA;
                }

                return selectedTA;
            }
        }

        private string GetNormalMapDirectory()
        {
            if (File.Exists(ProjectFolders.TextureFolders.NormalMapsPath + selectedNM))
            {
                return ProjectFolders.TextureFolders.NormalMapsPath + selectedNM;
            }
            else
            {
                string[] subdirectories = Directory.GetDirectories(ProjectFolders.TextureFolders.NormalMapsPath);

                foreach (string subdirectory in subdirectories)
                {
                    if (File.Exists(subdirectory + "/" + selectedNM))
                        return subdirectory + "/" + selectedNM;
                }

                return selectedNM;
            }
        }

        private string GetSpecularMapDirectory()
        {
            if (File.Exists(ProjectFolders.TextureFolders.SpecularMapsPath + selectedSM))
            {
                return ProjectFolders.TextureFolders.SpecularMapsPath + selectedSM;
            }
            else
            {
                string[] subdirectories = Directory.GetDirectories(ProjectFolders.TextureFolders.SpecularMapsPath);

                foreach (string subdirectory in subdirectories)
                {
                    if (File.Exists(subdirectory + "/" + selectedSM))
                        return subdirectory + "/" + selectedSM;
                }

                return selectedSM;
            }
        }

        #endregion

        #region Getters of data file

        private void GetListOfModelFiles()
        {
            string[] subDirectories = Directory.GetDirectories(ProjectFolders.ModelsPath);

            for (int i = 0; i < subDirectories.Length; ++i)
            {
                GetModelInDerictory(subDirectories[i], i);
            }

            GetModelInDerictory(ProjectFolders.ModelsPath, 0, false);
        }

        private void GetModelInDerictory(string path, int iterator = 0, bool subDirectory = true)
        {
            if (subDirectory)
            {
                string nodeName = path.Remove(0, path.LastIndexOf("\\") + 1);
                nodeName = nodeName.Remove(0, nodeName.LastIndexOf("/") + 1);

                TV_MeshSelection.Nodes.Add(nodeName);

                nodeName = String.Empty;
            }

            string[] files = Directory.GetFiles(path, "*.obj");

            for (int j = 0; j < files.Length; ++j)
            {
                files[j] = files[j].Remove(0, files[j].LastIndexOf("\\") + 1);
                files[j] = files[j].Remove(0, files[j].LastIndexOf("/") + 1);

                if (subDirectory)
                    TV_MeshSelection.Nodes[iterator].Nodes.Add(files[j]);
                else
                    TV_MeshSelection.Nodes.Add(files[j]);
            }

            files = null;
            files = Directory.GetFiles(path, "*.dae");

            for (int j = 0; j < files.Length; ++j)
            {
                files[j] = files[j].Remove(0, files[j].LastIndexOf("\\") + 1);
                files[j] = files[j].Remove(0, files[j].LastIndexOf("/") + 1);

                if (subDirectory)
                    TV_MeshSelection.Nodes[iterator].Nodes.Add(files[j]);
                else
                    TV_MeshSelection.Nodes.Add(files[j]);
            }

            files = null;
            files = Directory.GetFiles(path, "*.ase");

            for (int j = 0; j < files.Length; ++j)
            {
                files[j] = files[j].Remove(0, files[j].LastIndexOf("\\") + 1);
                files[j] = files[j].Remove(0, files[j].LastIndexOf("/") + 1);

                if (subDirectory)
                    TV_MeshSelection.Nodes[iterator].Nodes.Add(files[j]);
                else
                    TV_MeshSelection.Nodes.Add(files[j]);
            }

            files = null;
        }

        private void GetListOfTextureFiles(string textureFilesPath, TreeView treeView)
        {
            string[] subDirectories = Directory.GetDirectories(textureFilesPath);

            for (int i = 0; i < subDirectories.Length; ++i)
            {
                GetTexturesInDerictory(subDirectories[i], treeView, i);
            }

            GetTexturesInDerictory(textureFilesPath, treeView, 0, false);
        }

        private void GetTexturesInDerictory(string path, TreeView treeView, int iterator = 0, bool subDirectory = true)
        {
            if (subDirectory)
            {
                string nodeName = path.Remove(0, path.LastIndexOf("\\") + 1);
                nodeName = nodeName.Remove(0, nodeName.LastIndexOf("/") + 1);

                treeView.Nodes.Add(nodeName);

                nodeName = String.Empty;
            }

            string[] files = Directory.GetFiles(path, "*.jpg");

            for (int j = 0; j < files.Length; ++j)
            {
                files[j] = files[j].Remove(0, files[j].LastIndexOf("\\") + 1);
                files[j] = files[j].Remove(0, files[j].LastIndexOf("/") + 1);

                if (subDirectory)
                    treeView.Nodes[iterator].Nodes.Add(files[j]);
                else
                    treeView.Nodes.Add(files[j]);
            }

            files = null;
            files = Directory.GetFiles(path, "*.png");

            for (int j = 0; j < files.Length; ++j)
            {
                files[j] = files[j].Remove(0, files[j].LastIndexOf("\\") + 1);
                files[j] = files[j].Remove(0, files[j].LastIndexOf("/") + 1);

                if (subDirectory)
                    treeView.Nodes[iterator].Nodes.Add(files[j]);
                else
                    treeView.Nodes.Add(files[j]);
            }

            files = null;
            files = Directory.GetFiles(path, "*.bmp");

            for (int j = 0; j < files.Length; ++j)
            {
                files[j] = files[j].Remove(0, files[j].LastIndexOf("\\") + 1);
                files[j] = files[j].Remove(0, files[j].LastIndexOf("/") + 1);

                if (subDirectory)
                    treeView.Nodes[iterator].Nodes.Add(files[j]);
                else
                    treeView.Nodes.Add(files[j]);
            }

            files = null;
        }

        #endregion

        #region Text box text change

        private void TB_Translation_X_TextChange(object sender, EventArgs e)
        {
            tranlation_x = TB_Translation_X.Text;
        }

        private void TB_Translation_Y_TextChange(object sender, EventArgs e)
        {
            tranlation_y = TB_Translation_Y.Text;
        }

        private void TB_Translation_Z_TextChange(object sender, EventArgs e)
        {
            tranlation_z = TB_Translation_Z.Text;
        }

        private void TB_Rotation_X_TextChange(object sender, EventArgs e)
        {
            rotation_x = TB_Rotation_X.Text;
        }

        private void TB_Rotation_Y_TextChange(object sender, EventArgs e)
        {
            rotation_y = TB_Rotation_Y.Text;
        }

        private void TB_Rotation_Z_TextChange(object sender, EventArgs e)
        {
            rotation_z = TB_Rotation_Z.Text;
        }

        private void TB_Scale_X_TextChange(object sender, EventArgs e)
        {
            scale_x = TB_Scale_X.Text;
        }

        private void TB_Scale_Y_TextChange(object sender, EventArgs e)
        {
            scale_y = TB_Scale_Y.Text;
        }

        private void TB_Scale_Z_TextChange(object sender, EventArgs e)
        {
            scale_z = TB_Scale_Z.Text;
        }

        #endregion

        #region Text box key press

        private void TB_Translation_X_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void TB_Translation_Y_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void TB_Translation_Z_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void TB_Rotation_X_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void TB_Rotation_Y_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void TB_Rotation_Z_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void TB_Scale_X_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void TB_Scale_Y_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void TB_Scale_Z_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        #endregion

        #region Decrement buttons

        private void B_Left_T_X_Click(object sender, EventArgs e)
        {
            TB_Translation_X.Text = (Convert.ToInt32(TB_Translation_X.Text) - Bias(_biasModifier)).ToString();
        }

        private void B_Left_T_Y_Click(object sender, EventArgs e)
        {
            TB_Translation_Z.Text = (Convert.ToInt32(TB_Translation_Y.Text) - Bias(_biasModifier)).ToString();
        }

        private void B_Left_T_Z_Click(object sender, EventArgs e)
        {
            TB_Translation_Z.Text = (Convert.ToInt32(TB_Translation_Z.Text) - Bias(_biasModifier)).ToString();
        }

        private void B_Left_R_X_Click(object sender, EventArgs e)
        {
            TB_Rotation_X.Text = (Convert.ToInt32(TB_Rotation_X.Text) - Bias(_biasModifier)).ToString();
        }

        private void B_Left_R_Y_Click(object sender, EventArgs e)
        {
            TB_Rotation_Y.Text = (Convert.ToInt32(TB_Rotation_Y.Text) - Bias(_biasModifier)).ToString();
        }

        private void B_Left_R_Z_Click(object sender, EventArgs e)
        {
            TB_Rotation_Z.Text = (Convert.ToInt32(TB_Rotation_Z.Text) - Bias(_biasModifier)).ToString();
        }

        private void B_Left_S_X_Click(object sender, EventArgs e)
        {
            TB_Scale_X.Text = (Convert.ToInt32(TB_Scale_X.Text) - Bias(_biasModifier)).ToString();
        }

        private void B_Left_S_Y_Click(object sender, EventArgs e)
        {
            TB_Scale_Y.Text = (Convert.ToInt32(TB_Scale_Y.Text) - Bias(_biasModifier)).ToString();
        }

        private void B_Left_S_Z_Click(object sender, EventArgs e)
        {
            TB_Scale_Z.Text = (Convert.ToInt32(TB_Scale_Z.Text) - Bias(_biasModifier)).ToString();
        }

        #endregion

        #region Increment buttons

        private void B_Right_T_X_Click(object sender, EventArgs e)
        {
            TB_Translation_X.Text = (Convert.ToInt32(TB_Translation_X.Text) + Bias(_biasModifier)).ToString();
        }

        private void B_Right_T_Y_Click(object sender, EventArgs e)
        {
            TB_Translation_Y.Text = (Convert.ToInt32(TB_Translation_Y.Text) + Bias(_biasModifier)).ToString();
        }

        private void B_Right_T_Z_Click(object sender, EventArgs e)
        {
            TB_Translation_Z.Text = (Convert.ToInt32(TB_Translation_Z.Text) + Bias(_biasModifier)).ToString();
        }

        private void B_Right_R_X_Click(object sender, EventArgs e)
        {
            TB_Rotation_X.Text = (Convert.ToInt32(TB_Rotation_X.Text) + Bias(_biasModifier)).ToString();
        }

        private void B_Right_R_Y_Click(object sender, EventArgs e)
        {
            TB_Rotation_Y.Text = (Convert.ToInt32(TB_Rotation_Y.Text) + Bias(_biasModifier)).ToString();
        }

        private void B_Right_R_Z_Click(object sender, EventArgs e)
        {
            TB_Rotation_Z.Text = (Convert.ToInt32(TB_Rotation_Z.Text) + Bias(_biasModifier)).ToString();
        }

        private void B_Right_S_X_Click(object sender, EventArgs e)
        {
            TB_Scale_X.Text = (Convert.ToInt32(TB_Scale_X.Text) + Bias(_biasModifier)).ToString();
        }

        private void B_Right_S_Y_Click(object sender, EventArgs e)
        {
            TB_Scale_Y.Text = (Convert.ToInt32(TB_Scale_Y.Text) + Bias(_biasModifier)).ToString();
        }

        private void B_Right_S_Z_Click(object sender, EventArgs e)
        {
            TB_Scale_Z.Text = (Convert.ToInt32(TB_Scale_Z.Text) + Bias(_biasModifier)).ToString();
        }

        #endregion

        #region Check boxes

        #region Mesh selection

        private void CB_MeshSelection_Toggle_CheckedChanged(object sender, EventArgs e)
        {
            if (CB_MeshSelection_Toggle.Checked)
            {
                GB_Mesh.Height = 19;
                //TV_MeshSelection.Height = 36;

                // need to move up Transformation Group box
                GB_Transformation.Top = GB_Transformation.Top - gb_mesh_height + 15;
                // need to move up Texturing Group box
                GB_Texturing.Top = GB_Texturing.Top - gb_mesh_height + 15;

                tip.SetToolTip(CB_MeshSelection_Toggle, "Maximize");
            }
            else
            {
                GB_Mesh.Height = gb_mesh_height;
                //TV_MeshSelection.Height = 144;

                // need to move down Transformation Group box
                GB_Transformation.Top = GB_Transformation.Top + gb_mesh_height - 15;
                // need to move down Texturing Group box
                GB_Texturing.Top = GB_Texturing.Top + gb_mesh_height - 15;

                // need to minimize other boxes
                CB_Transformation_Toggle.Checked = true;
                CB_Texturing_Toggle.Checked = true;

                tip.SetToolTip(CB_MeshSelection_Toggle, "Minimize");
            }
        }

        #endregion

        #region Transormation

        private void CB_Transformation_Toggle_CheckedChanged(object sender, EventArgs e)
        {
            if (CB_Transformation_Toggle.Checked)
            {
                GB_Transformation.Height = 19;

                // need to move up Texturing Group box
                GB_Texturing.Top = GB_Texturing.Top - gb_transformation_height + 15;

                tip.SetToolTip(CB_Transformation_Toggle, "Maximize");
            }
            else
            {
                GB_Transformation.Height = gb_transformation_height;

                // need to move down Texturing Group box
                GB_Texturing.Top = GB_Texturing.Top + gb_transformation_height - 15;

                // need to minimize other boxes
                CB_MeshSelection_Toggle.Checked = true;
                CB_Texturing_Toggle.Checked = true;

                tip.SetToolTip(CB_Transformation_Toggle, "Minimize");
            }
        }

        private void CB_BiasModifier_CheckedChanged(object sender, EventArgs e)
        {
            _biasModifier = !_biasModifier;

            if (_biasModifier)
                SetArrowButtonToolTips("10");
            else
                SetArrowButtonToolTips("1");
        }

        #endregion

        #region Texturing

        private void CB_Texturing_Toggle_CheckedChanged(object sender, EventArgs e)
        {
            if (CB_Texturing_Toggle.Checked)
            {
                gb_texturing_height = GB_Texturing.Height;
                GB_Texturing.Height = 19;

                tip.SetToolTip(CB_Texturing_Toggle, "Maximize");
            }
            else
            {
                GB_Texturing.Height = gb_texturing_height;

                // need to minimize other boxes
                CB_MeshSelection_Toggle.Checked = true;
                CB_Transformation_Toggle.Checked = true;

                tip.SetToolTip(CB_Texturing_Toggle, "Minimize");
            }
        }

        private void CB_TA_Toggle_CheckedChanged(object sender, EventArgs e)
        {
            if (!CB_TA_Toggle.Checked)
            {
                CB_TA_Toggle.Text = "minimize";

                TV_TA_Selection.Height = 200;
                GB_Texturing.Height += 200;

                // need to move down NM group
                CB_EnableNM.Top = CB_EnableNM.Top + 195;
                CB_NM_Toggle.Top = CB_NM_Toggle.Top + 195;
                TV_NM_Selection.Top = TV_NM_Selection.Top + 195;

                // need to move down SM group
                CB_EnableSM.Top = CB_EnableSM.Top + 195;
                CB_SM_Toggle.Top = CB_SM_Toggle.Top + 195;
                TV_SM_Selection.Top = TV_SM_Selection.Top + 195;

                // need to minimize other toggles
                CB_NM_Toggle.Checked = true;
                CB_SM_Toggle.Checked = true;
            }
            else
            {
                CB_TA_Toggle.Text = "maximize";

                TV_TA_Selection.Height = 1;
                GB_Texturing.Height -= 200;

                // need to move up NM group
                CB_EnableNM.Top = CB_EnableNM.Top - 195;
                CB_NM_Toggle.Top = CB_NM_Toggle.Top - 195;
                TV_NM_Selection.Top = TV_NM_Selection.Top - 195;

                // need to move up SM group
                CB_EnableSM.Top = CB_EnableSM.Top - 195;
                CB_SM_Toggle.Top = CB_SM_Toggle.Top - 195;
                TV_SM_Selection.Top = TV_SM_Selection.Top - 195;
            }
        }

        private void CB_NM_Toggle_CheckedChanged(object sender, EventArgs e)
        {
            if (!CB_NM_Toggle.Checked)
            {
                CB_NM_Toggle.Text = "minimize";

                TV_NM_Selection.Height = 200;
                GB_Texturing.Height += 200;

                // need to move down SM group
                CB_EnableSM.Top = CB_EnableSM.Top + 195;
                CB_SM_Toggle.Top = CB_SM_Toggle.Top + 195;
                TV_SM_Selection.Top = TV_SM_Selection.Top + 195;
                
                // need to minimize other toggles
                CB_TA_Toggle.Checked = true;
                CB_SM_Toggle.Checked = true;
            }
            else
            {
                CB_NM_Toggle.Text = "maximize";

                TV_NM_Selection.Height = 1;
                GB_Texturing.Height -= 200;

                // need to move up SM group
                CB_EnableSM.Top = CB_EnableSM.Top - 195;
                CB_SM_Toggle.Top = CB_SM_Toggle.Top - 195;
                TV_SM_Selection.Top = TV_SM_Selection.Top - 195;
            }
        }

        private void CB_SM_Toggle_CheckedChanged(object sender, EventArgs e)
        {
            if (!CB_SM_Toggle.Checked)
            {
                CB_SM_Toggle.Text = "minimize";

                TV_SM_Selection.Height = 200;
                GB_Texturing.Height += 200;

                // need to minimize other toggles
                CB_NM_Toggle.Checked = true;
                CB_TA_Toggle.Checked = true;
            }
            else
            {
                CB_SM_Toggle.Text = "maximize";

                TV_SM_Selection.Height = 1;
                GB_Texturing.Height -= 200;
            }
        }

        private void CB_EnableTA_CheckedChanged(object sender, EventArgs e)
        {
            if (CB_EnableTA.Checked)
            {
                CB_SM_Toggle.Checked = true;
                CB_NM_Toggle.Checked = true;

                CB_EnableNM.Enabled = true;

                CB_TA_Toggle.Enabled = true;
                CB_TA_Toggle.Checked = false;

                TV_TA_Selection.Enabled = true;
            }
            else
            {
                CB_EnableNM.Enabled = false;
                CB_NM_Toggle.Enabled = false;
                TV_NM_Selection.Enabled = false;

                CB_EnableSM.Enabled = false;
                CB_SM_Toggle.Enabled = false;
                TV_SM_Selection.Enabled = false;

                CB_TA_Toggle.Enabled = false;
                TV_TA_Selection.Enabled = false;

                CB_TA_Toggle.Checked = true;
                CB_NM_Toggle.Checked = true;
                CB_SM_Toggle.Checked = true;
                CB_EnableNM.Checked = false;
                CB_EnableSM.Checked = false;
            }
        }

        private void CB_EnableNM_CheckedChanged(object sender, EventArgs e)
        {
            if (CB_EnableNM.Checked)
            {
                CB_EnableSM.Enabled = true;
                CB_SM_Toggle.Checked = true;

                CB_TA_Toggle.Checked = true;

                CB_NM_Toggle.Enabled = true;
                CB_NM_Toggle.Checked = false;

                TV_NM_Selection.Enabled = true;
            }
            else
            {
                CB_EnableSM.Enabled = false;
                CB_SM_Toggle.Enabled = false;
                TV_SM_Selection.Enabled = false;

                CB_NM_Toggle.Enabled = false;
                TV_NM_Selection.Enabled = false;

                CB_NM_Toggle.Checked = true;
                CB_SM_Toggle.Checked = true;
                CB_EnableSM.Checked = false;
            }
        }

        private void CB_EnableSM_CheckedChanged(object sender, EventArgs e)
        {
            if (CB_EnableSM.Checked)
            {
                CB_TA_Toggle.Checked = true;
                CB_NM_Toggle.Checked = true;

                CB_SM_Toggle.Enabled = true;
                CB_SM_Toggle.Checked = false;

                TV_SM_Selection.Enabled = true;
            }
            else
            {
                CB_SM_Toggle.Enabled = false;
                TV_SM_Selection.Enabled = false;

                CB_SM_Toggle.Checked = true;
            }
        }

        #endregion

        #endregion

        #region Tree views

        private void TV_MeshSelection_AfterSelect(object sender, TreeViewEventArgs e)
        {
            selectedMesh = TV_MeshSelection.SelectedNode.ToString();
            selectedMesh = selectedMesh.Replace("TreeNode: ", "");

            if (selectedMesh.LastIndexOf(".") == -1)
                selectedMesh = String.Empty;
            else
            {
                if (Directory.Exists(ProjectFolders.ModelsPath + selectedMesh))
                    selectedMesh = String.Empty;
            }
        }

        private void GB_Texturing_Enter(object sender, EventArgs e)
        {

        }

        private void TV_TA_AfterSelect(object sender, TreeViewEventArgs e)
        {
            selectedTA = TV_TA_Selection.SelectedNode.ToString();
            selectedTA = selectedTA.Replace("TreeNode: ", "");

            if (selectedTA.LastIndexOf(".") == -1)
                selectedTA = String.Empty;
            else
            {
                if (Directory.Exists(ProjectFolders.ModelsPath + selectedTA))
                    selectedTA = String.Empty;
            }
        }

        private void TV_NM_AfterSelect(object sender, TreeViewEventArgs e)
        {
            selectedNM = TV_NM_Selection.SelectedNode.ToString();
            selectedNM = selectedNM.Replace("TreeNode: ", "");

            if (selectedNM.LastIndexOf(".") == -1)
                selectedNM = String.Empty;
            else
            {
                if (Directory.Exists(ProjectFolders.ModelsPath + selectedNM))
                    selectedNM = String.Empty;
            }
        }

        private void TV_SM_AfterSelect(object sender, TreeViewEventArgs e)
        {
            selectedSM = TV_SM_Selection.SelectedNode.ToString();
            selectedSM = selectedSM.Replace("TreeNode: ", "");

            if (selectedSM.LastIndexOf(".") == -1)
                selectedSM = String.Empty;
            else
            {
                if (Directory.Exists(ProjectFolders.ModelsPath + selectedSM))
                    selectedSM = String.Empty;
            }
        }

        #endregion

        #region Data checkers

        private bool CheckMeshData()
        {
            if (selectedMesh == String.Empty)
                return false;
            else
                return true;
        }

        private bool CheckTextureAtlasData()
        {
            if (selectedTA == String.Empty)
                return false;
            else
                return true;
        }

        private bool CheckMeshSettings()
        {
            if ((tranlation_x == String.Empty) ||
                (tranlation_y == String.Empty) ||
                (tranlation_z == String.Empty) ||
                (rotation_x == String.Empty) ||
                (rotation_y == String.Empty) ||
                (rotation_z == String.Empty) ||
                (scale_x == String.Empty) ||
                (scale_y == String.Empty) ||
                (scale_z == String.Empty))
                return false;
            else
                return true;
        }

        #endregion

        #region Menu items and resizability

        private void Close_Click(object sender, EventArgs e)
        {
            Hide();
        }

        private void Pin_Click(object sender, EventArgs e)
        {
            if (windowBorderTrigger)
            {
                base.FormBorderStyle = FormBorderStyle.None;
                TSMI_Pin.ToolTipText = "Detach";
                // need to allow resize component
                Resize_Label.Enabled = true;
            }
            else
            {
                base.FormBorderStyle = FormBorderStyle.Sizable;
                TSMI_Pin.ToolTipText = "Attach";
                // need to restrict extra resizable component
                Resize_Label.Enabled = false;
            }

            windowBorderTrigger = !windowBorderTrigger;

            TSMI_Pin.Image.RotateFlip(RotateFlipType.Rotate270FlipX);
        }

        private void Resize_Label_OnMouseDown(object sender, MouseEventArgs e)
        {
            minWidth = Location.X + MinimumSize.Width;
            maxWidth = Location.X + MaximumSize.Width;
        }

        private void Resize_Label_OnMouseUp(object sender, MouseEventArgs e)
        {
            int currentMousePos = Cursor.Position.X;
            if (currentMousePos < minWidth)
                this.Width = minWidth - Location.X;
            else if (currentMousePos > maxWidth)
                this.Width = maxWidth - Location.X;
            else
                this.Width = currentMousePos - Location.X;
        }

        #endregion

        private void B_Preview_Click(object sender, EventArgs e)
        {
            StreamWriter sw_previewData = new StreamWriter("preview.tmp");

            sw_previewData.WriteLine((GetModelDirectory() == String.Empty) ? "none" : GetModelDirectory());
            sw_previewData.WriteLine((GetTexturePaths() == String.Empty) ? "none" : GetTexturePaths());

            sw_previewData.Close();

            MeshCreationMaster previewForm = new MeshCreationMaster();
            previewForm.ShowDialog();
        }

        private void B_Add_Click(object sender, EventArgs e)
        {
            if (CheckMeshData())
            {
                if (CheckTextureAtlasData())
                {
                    if (CheckMeshSettings())
                    {
                        WriteMeshData();
                    }
                    else
                    {
                        MessageBox.Show("'Translation', 'Rotation' and 'Scaling' must be INSERTED!", "Warning!");
                    }
                }
                else
                {
                    MessageBox.Show("'Texture Atlas' must be SELECTED!", "Warning!");
                }
            }
            else
            {
                MessageBox.Show("'Mesh' must be SELECTED!", "Warning!");
            }
        }
    }
}
