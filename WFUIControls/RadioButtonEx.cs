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
    public partial class RadioButtonEx : RadioButton
    {
        public RadioButtonEx()
        {
            InitializeComponent();
            InitializeVariables();
        }

        
        #region Variables
        public new string Text
        {
            get
            {
                return RB_Label.Text;
            }
            set
            {
                RB_Label.Text = "      " + value.Trim();
                base.Text = RB_Label.Text;
            }
        }
        public Image RadioButtonInactive { get; set; }
        public Image RadioButtonActive { get; set; }
        public Image RadioButtonChecked { get; set; }
        public Color RadioButtonInactiveForeColor { get; set; }
        public Color RadioButtonActiveForeColor { get; set; }
        /*public Color BackgroundColor
        {
            get
            {
                return this.BackColor;
            }
            set
            {
                this.BackColor = value;
                RB_Label.BackColor = value;
            }
        }*/
        #endregion


        #region Common functions

        private void InitializeVariables()
        {
            RadioButtonInactive = Properties.Resources.radio_button_inactive;
            RadioButtonActive = Properties.Resources.radio_button_active;
            RadioButtonChecked = Properties.Resources.radio_button_checked;
            RadioButtonInactiveForeColor = Color.FromKnownColor(KnownColor.ButtonFace);
            RadioButtonActiveForeColor = Color.FromKnownColor(KnownColor.MenuHighlight);
            Checked = true;
            AutoSize = false;
            RB_Label.AutoSize = false;
        }

        private void CheckBoxClickAction()
        {
            if (!Checked) // if CB is true
            {
                RB_Label.Image = RadioButtonChecked;
                Checked = !Checked;
            }
            else // if CB is false
            {
                RB_Label.Image = RadioButtonActive;
                Checked = !Checked;
            }
        }

        #endregion

        #region Mouse Events
        private void RB_Label_Click(object sender, EventArgs e)
        {
            CheckBoxClickAction();
        }
        private void RB_Label_DoubleClick(object sender, EventArgs e)
        {
            CheckBoxClickAction();
        }
        private void RB_Label_MouseEnter(object sender, EventArgs e)
        {
            if (!Checked)
            {
                RB_Label.Image = RadioButtonActive;
            }
            RB_Label.ForeColor = RadioButtonActiveForeColor;
        }
        private void RB_Label_MouseLeave(object sender, EventArgs e)
        {
            if (!Checked)
            {
                RB_Label.Image = RadioButtonInactive;
            }
            RB_Label.ForeColor = RadioButtonInactiveForeColor;
        }
        #endregion

        private void RadioButtonEx_CheckedChanged(object sender, EventArgs e)
        {
            if (Checked)
            {
                RB_Label.Image = RadioButtonChecked;
            }
            else
            {
                RB_Label.Image = RadioButtonInactive;
            }
        }

    }
}
