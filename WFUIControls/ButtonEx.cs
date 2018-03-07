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
    public partial class ButtonEx : Button
    {
        public ButtonEx()
        {
            InitializeComponent();
            InitializeVariables();
        }


        #region Variables
        public bool EnableImageResize { get; set; }
        public new string Text
        {
            get
            {
                return L_Button.Text;
            }
            set
            {
                L_Button.Text = value;
                base.Text = value;

                if (EnableImageResize)
                { 
                    ButtonInactive = new Bitmap(ButtonInactive, new Size(TextRenderer.MeasureText(value, L_Button.Font).Width, 27));
                    ButtonActive = new Bitmap(ButtonActive, new Size(TextRenderer.MeasureText(value, L_Button.Font).Width, 27));
                    L_Button.Image = ButtonInactive;
                }
            }
        }
        public Image ButtonInactive
        {
            get
            {
                return buttonInactive;
            }
            set
            {
                buttonInactive = value;
                L_Button.Image = value;
            }
        }
        public Image ButtonActive { get; set; }
        public Color ButtonInactiveForeColor { get; set; }
        public Color ButtonActiveForeColor { get; set; }
        public Color BackgroundColor
        {
            get
            {
                return this.BackColor;
            }
            set
            {
                this.BackColor = value;
                L_Button.BackColor = value;
            }
        }
        public new ContentAlignment ImageAlign
        {
            get
            {
                return L_Button.ImageAlign;
            }
            set
            {
                L_Button.ImageAlign = value;
            }
        }
        public new ContentAlignment TextAlign
        {
            get
            {
                return L_Button.TextAlign;
            }
            set
            {
                L_Button.TextAlign = value;
            }
        }

        private Image buttonInactive;
        #endregion


        private void InitializeVariables()
        {
            EnableImageResize = false;
            buttonInactive = Properties.Resources.button_inactive;
            ButtonInactive = Properties.Resources.button_inactive;
            ButtonActive = Properties.Resources.button_active;
            ButtonInactiveForeColor = Color.FromKnownColor(KnownColor.ButtonFace);
            ButtonActiveForeColor = Color.FromKnownColor(KnownColor.MenuHighlight);
        }

        public void EnableClickEvent( EventHandler clickEvent)
        {
            this.L_Button.Click += new EventHandler(clickEvent);
        }

        #region Mouse Events
        private void L_Button_MouseEnter(object sender, EventArgs e)
        {
            L_Button.Image = ButtonActive;
            L_Button.ForeColor = ButtonActiveForeColor;
        }
        private void L_Button_MouseLeave(object sender, EventArgs e)
        {
            L_Button.Image = ButtonInactive;
            L_Button.ForeColor = ButtonInactiveForeColor;
        }
        #endregion

    }
}
