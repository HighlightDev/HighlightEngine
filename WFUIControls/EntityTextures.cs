using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace UIControls
{
    public partial class EntityTextures : UserControl
    {
        public EntityTextures()
        {
            InitializeComponent();
            InitializeVariables();
        }


        #region Variables
        public Int32 MinimizeCheckBoxHeight
        {
            get
            {
                return MCB_TA.MaxHeight;
            }
            set
            {
                MCB_TA.MaxHeight = value;
                MCB_NM.MaxHeight = value;
                MCB_SM.MaxHeight = value;
                if (MinimizeCheckBoxHeight != 0)
                {
                    Height = MinimizeCheckBoxHeight + 72;
                    //MaximumSize = new Size( (MaxWidth > 0) ? MaxWidth : 500, Height);
                    //MinimumSize = new Size(0, Height);
                }
            }
        }
        public Size SmallIconsSize { get; set; }
        public Size LargeIconsSize { get; set; }
        public Image Maximized
        {
            get
            {
                return MCB_TA.Maximized;
            }
            set
            {
                MCB_TA.Maximized = value;
                MCB_NM.Maximized = value;
                MCB_SM.Maximized = value;
            }
        }
        public Image Minimized
        {
            get
            {
                return MCB_TA.Minimized;
            }
            set
            {
                MCB_TA.Minimized = value;
                MCB_NM.Minimized = value;
                MCB_SM.Minimized = value;
            }
        }
        public Image CheckBoxInactive
        {
            get
            {
                return MCB_TA.CheckBoxInactive;
            }
            set
            {
                MCB_TA.CheckBoxInactive = value;
                MCB_NM.CheckBoxInactive = value;
                MCB_SM.CheckBoxInactive = value;
            }
        }
        public Image CheckBoxActive
        {
            get
            {
                return MCB_TA.CheckBoxActive;
            }
            set
            {
                MCB_TA.CheckBoxActive = value;
                MCB_NM.CheckBoxActive = value;
                MCB_SM.CheckBoxActive = value;
            }
        }
        public Image CheckBoxChecked
        {
            get
            {
                return MCB_TA.CheckBoxChecked;
            }
            set
            {
                MCB_TA.CheckBoxChecked = value;
                MCB_NM.CheckBoxChecked = value;
                MCB_SM.CheckBoxChecked = value;
            }
        }
        public Color HeaderInactiveForeColor
        {
            get
            {
                return MCB_TA.HeaderInactiveForeColor;
            }
            set
            {
                MCB_TA.HeaderInactiveForeColor = value;
                MCB_NM.HeaderInactiveForeColor = value;
                MCB_SM.HeaderInactiveForeColor = value;
            }
        }
        public Color HeaderActiveForeColor
        {
            get
            {
                return MCB_TA.HeaderActiveForeColor;
            }
            set
            {
                MCB_TA.HeaderActiveForeColor = value;
                MCB_NM.HeaderActiveForeColor = value;
                MCB_SM.HeaderActiveForeColor = value;
            }
        }
        public Color BackgroundColor
        {
            get
            {
                return this.BackColor;
            }
            set
            {
                this.BackColor = value;
                TLP.BackColor = value;
                CMS_LV.BackColor = value;
                TSMI_SmallIcons.BackColor = value;
                TSMI_LargeIcons.BackColor = value;
                TSMI_Refresh.BackColor = value;
                TS_Separator.BackColor = value;
                MCB_TA.BackgroundColor = value;
                MCB_NM.BackgroundColor = value;
                MCB_SM.BackgroundColor = value;
            }
        }
        public Color ListViewBackColor
        {
            get
            {
                return LV_TA.BackColor;
            }
            set
            {
                LV_TA.BackColor = value;
                LV_NM.BackColor = value;
                LV_SM.BackColor = value;
            }
        }
        public Color ListViewForeColor
        {
            get
            {
                return this.ForeColor;
            }
            set
            {
                this.ForeColor = value;
                TLP.ForeColor = value;
                LV_TA.ForeColor = value;
                LV_NM.ForeColor = value;
                LV_SM.ForeColor = value;
            }
        }
        public Color ContextMenuForeColor
        {
            get
            {
                return TSMI_SmallIcons.ForeColor;
            }
            set
            {
                CMS_LV.ForeColor = value;
                TSMI_SmallIcons.ForeColor = value;
                TSMI_LargeIcons.ForeColor = value;
                TSMI_Refresh.ForeColor = value;
                TS_Separator.ForeColor = value;
            }
        }
        public RightToLeft RightToLeftHeaderAlign
        {
            get
            {
                return MCB_TA.RightToLeft;
            }
            set
            {
                MCB_TA.RightToLeft = value;
                MCB_NM.RightToLeft = value;
                MCB_SM.RightToLeft = value;
            }
        }

        public string SelectedTextureAtlas
        {
            get
            {
                try
                {
                    string groupName = LV_TA.SelectedItems[0].Group.Header;
                    return ProjectFolders.TextureAtlasPath +
                        ((groupName == "TextureAtlas") ? LV_TA.SelectedItems[0].Text : groupName + "/" + LV_TA.SelectedItems[0].Text);
                }
                catch
                {
                    return "";
                }
            }
        }
        public string SelectedNormalMap
        {
            get
            {
                try
                {
                    string groupName = LV_NM.SelectedItems[0].Group.Header;
                return ProjectFolders.NormalMapsPath +
                    ((groupName == "NormalMaps") ? LV_NM.SelectedItems[0].Text : groupName + "/" + LV_NM.SelectedItems[0].Text);
                }
                catch
                {
                    return "";
                }
            }
        }
        public string SelectedSpecularMap
        {
            get
            {
                try
                {
                    string groupName = LV_SM.SelectedItems[0].Group.Header;
                return ProjectFolders.SpecularMapsPath +
                    ((groupName == "SpecularMaps") ? LV_SM.SelectedItems[0].Text : groupName + "/" + LV_SM.SelectedItems[0].Text);
                }
                catch
                {
                    return "";
                }
            }
        }
        #endregion


        #region Common Functions

        private void InitializeVariables()
        {
            Maximized = Properties.Resources.maximized;
            Minimized = Properties.Resources.minimized;
            CheckBoxInactive = Properties.Resources.check_box_inactive;
            CheckBoxActive = Properties.Resources.check_box_active;
            CheckBoxChecked = Properties.Resources.check_box_checked;
            HeaderInactiveForeColor = Color.FromKnownColor(KnownColor.ButtonFace);
            HeaderActiveForeColor = Color.FromKnownColor(KnownColor.MenuHighlight);
            SmallIconsSize = new Size(32, 32);
            LargeIconsSize = new Size(96, 96);
        }

        private void IconSizeChange(bool isSmall)
        {
            ListViewEditor.IconSizeChange(ProjectFolders.TextureAtlasPath, LV_TA, SmallIconsSize, LargeIconsSize, isSmall);
            ListViewEditor.IconSizeChange(ProjectFolders.NormalMapsPath, LV_NM, SmallIconsSize, LargeIconsSize, isSmall);
            ListViewEditor.IconSizeChange(ProjectFolders.SpecularMapsPath, LV_SM, SmallIconsSize, LargeIconsSize, isSmall);

            TSMI_LargeIcons.Checked = !isSmall;
            TSMI_SmallIcons.Checked = isSmall;
        }

        #endregion

        private void EntityTextures_Load(object sender, EventArgs e)
        {
            MinimizeCheckBoxHeight = this.Height - 72;

            MCB_TA.Height = MinimizeCheckBoxHeight;
            MCB_NM.Height = MinimizeCheckBoxHeight;
            MCB_SM.Height = MinimizeCheckBoxHeight;

            MCB_TA.Minimize();
            MCB_NM.Minimize();
            MCB_SM.Minimize();

            ListViewEditor.Connect(ProjectFolders.TextureAtlasPath, LV_TA, SmallIconsSize, LargeIconsSize);
            ListViewEditor.Dispose();
            ListViewEditor.Connect(ProjectFolders.NormalMapsPath, LV_NM, SmallIconsSize, LargeIconsSize);
            ListViewEditor.Dispose();
            ListViewEditor.Connect(ProjectFolders.SpecularMapsPath, LV_SM, SmallIconsSize, LargeIconsSize);
            ListViewEditor.Dispose();
        }

        private void EntityTextures_Resize(object sender, EventArgs e)
        {
            MinimizeCheckBoxHeight = this.Height - 72;

            MCB_TA.Height = MinimizeCheckBoxHeight;
            MCB_NM.Height = MinimizeCheckBoxHeight;
            MCB_SM.Height = MinimizeCheckBoxHeight;

            MCB_TA.Minimize();
            MCB_NM.Minimize();
            MCB_SM.Minimize();
        }

        #region AutoHide of MCB
        private void MCB_TA_Resize(object sender, EventArgs e)
        {
            if (!MCB_NM.IsMinimized)
                MCB_NM.Minimize();
            if (!MCB_SM.IsMinimized)
                MCB_SM.Minimize();
        }
        private void MCB_NM_Resize(object sender, EventArgs e)
        {
            if (!MCB_TA.IsMinimized)
                MCB_TA.Minimize();
            if (!MCB_SM.IsMinimized)
                MCB_SM.Minimize();
        }
        private void MCB_SM_Resize(object sender, EventArgs e)
        {
            if (!MCB_TA.IsMinimized)
                MCB_TA.Minimize();
            if (!MCB_NM.IsMinimized)
                MCB_NM.Minimize();
        }
        #endregion

        #region ListView KeyDown Events
        private void LV_TA_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Add)
            {
                if (!TSMI_LargeIcons.Checked)
                    IconSizeChange(false);
            }
            if (e.KeyData == Keys.Subtract)
            {
                if (!TSMI_SmallIcons.Checked)
                    IconSizeChange(true);
            }
            if (e.KeyData == Keys.F5)
            {
                ListViewEditor.Refresh(ProjectFolders.TextureAtlasPath, LV_TA, SmallIconsSize, LargeIconsSize);
                TSMI_LargeIcons.Checked = true;
                TSMI_SmallIcons.Checked = false;
            }
        }
        private void LV_NM_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Add)
            {
                if (!TSMI_LargeIcons.Checked)
                    IconSizeChange(false);
            }
            if (e.KeyData == Keys.Subtract)
            {
                if (!TSMI_SmallIcons.Checked)
                    IconSizeChange(true);
            }
            if (e.KeyData == Keys.F5)
            {
                ListViewEditor.Refresh(ProjectFolders.TextureAtlasPath, LV_TA, SmallIconsSize, LargeIconsSize);
                TSMI_LargeIcons.Checked = true;
                TSMI_SmallIcons.Checked = false;
            }
        }
        private void LV_SM_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Add)
            {
                if (!TSMI_LargeIcons.Checked)
                    IconSizeChange(false);
            }
            if (e.KeyData == Keys.Subtract)
            {
                if (!TSMI_SmallIcons.Checked)
                    IconSizeChange(true);
            }
            if (e.KeyData == Keys.F5)
            {
                ListViewEditor.Refresh(ProjectFolders.TextureAtlasPath, LV_TA, SmallIconsSize, LargeIconsSize);
                TSMI_LargeIcons.Checked = true;
                TSMI_SmallIcons.Checked = false;
            }
        }
        #endregion

        #region ToolStrip Menu Item Click Events
        private void TSMI_LargeIcons_Click(object sender, EventArgs e)
        {
            if (!TSMI_LargeIcons.Checked)
                IconSizeChange(false);
        }
        private void TSMI_SmallIcons_Click(object sender, EventArgs e)
        {
            if (!TSMI_SmallIcons.Checked)
                IconSizeChange(true);
        }
        private void TSMI_Refresh_Click(object sender, EventArgs e)
        {
            if (LV_TA.Focused)
            {
                ListViewEditor.Refresh(ProjectFolders.TextureAtlasPath, LV_TA, SmallIconsSize, LargeIconsSize);
            }
            if (LV_NM.Focused)
            {
                ListViewEditor.Refresh(ProjectFolders.NormalMapsPath, LV_NM, SmallIconsSize, LargeIconsSize);
            }
            if (LV_SM.Focused)
            {
                ListViewEditor.Refresh(ProjectFolders.SpecularMapsPath, LV_SM, SmallIconsSize, LargeIconsSize);
            }

            TSMI_LargeIcons.Checked = true;
            TSMI_SmallIcons.Checked = false;
        }
        #endregion

    }
}

// need to find a way how to change back or fore color of group header
// after scroll in ListView while icons are small some of items are hidden
