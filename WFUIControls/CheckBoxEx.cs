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
    public partial class CheckBoxEx : CheckBox
    {
        public CheckBoxEx()
        {
            InitializeComponent();
            InitializeVariables();
        }


        #region Variables
        public new string Text
        {
            get
            {
                return CB_Label.Text;
            }
            set
            {
                CB_Label.Text = "      " + value.Trim();
                base.Text = CB_Label.Text;
            }
        }
        public Image CheckBoxInactive { get; set; }
        public Image CheckBoxActive { get; set; }
        public Image CheckBoxChecked { get; set; }
        public Color CheckBoxInactiveForeColor { get; set; }
        public Color CheckBoxActiveForeColor { get; set; }
        /*public Color BackgroundColor
        {
            get
            {
                return this.BackColor;
            }
            set
            {
                this.BackColor = value;
                CB_Label.BackColor = value;
            }
        }*/
        #endregion


        #region Common functions

        private void InitializeVariables()
        {
            CheckBoxInactive = Properties.Resources.check_box_inactive;
            CheckBoxActive = Properties.Resources.check_box_active;
            CheckBoxChecked = Properties.Resources.check_box_checked;
            CheckBoxInactiveForeColor = Color.FromKnownColor(KnownColor.ButtonFace);
            CheckBoxActiveForeColor = Color.FromKnownColor(KnownColor.MenuHighlight);
            Checked = true;
            AutoSize = false;
            CB_Label.AutoSize = false;
        }

        private void CheckBoxClickAction()
        {
            if (!Checked) // if CB is true
            {
                CB_Label.Image = CheckBoxChecked;
                Checked = !Checked;
            }
            else // if CB is false
            {
                CB_Label.Image = CheckBoxActive;
                Checked = !Checked;
            }
        }

        #endregion

        #region Mouse Events
        private void CB_Label_Click(object sender, EventArgs e)
        {
            CheckBoxClickAction();
        }
        private void CB_Label_DoubleClick(object sender, EventArgs e)
        {
            CheckBoxClickAction();
        }
        private void CB_Label_MouseEnter(object sender, EventArgs e)
        {
            if (!Checked)
            {
                CB_Label.Image = CheckBoxActive;
            }
            CB_Label.ForeColor = CheckBoxActiveForeColor;
        }
        private void CB_Label_MouseLeave(object sender, EventArgs e)
        {
            if (!Checked)
            {
                CB_Label.Image = CheckBoxInactive;
            }
            CB_Label.ForeColor = CheckBoxInactiveForeColor;
        }


        #endregion

        private void CheckBoxEx_CheckedChanged(object sender, EventArgs e)
        {
            if (Checked)
            {
                CB_Label.Image = CheckBoxChecked;
            }
            else
            {
                CB_Label.Image = CheckBoxInactive;
            }
        }
    }
}
