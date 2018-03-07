using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using OpenTK;

using KeyPressEventArgs = System.Windows.Forms.KeyPressEventArgs;


namespace UIControls
{
    public partial class EntityTransformation : UserControl
    {
        public EntityTransformation()
        {
            InitializeComponent();
            InitializeVariables();
            InitializeMouseWheelEvents();
        }


        #region Variables
        private ToolTip _tip;
        private int _gb_transformation_height;

        #region Entity Data
        public Vector3 Translation
        {
            get
            {
                return new Vector3(
                    Convert.ToSingle(TB_Translation_X.Text),
                    Convert.ToSingle(TB_Translation_Y.Text),
                    Convert.ToSingle(TB_Translation_Z.Text)
                );
            }
            set
            {
                TB_Translation_X.Text = value.X.ToString();
                TB_Translation_Y.Text = value.Y.ToString();
                TB_Translation_Z.Text = value.Z.ToString();
            }
        }
        public Vector3 Rotation
        {
            get
            {
                return new Vector3(
                    Convert.ToSingle(TB_Rotation_X.Text),
                    Convert.ToSingle(TB_Rotation_Y.Text),
                    Convert.ToSingle(TB_Rotation_Z.Text)
                );
            }
            set
            {
                TB_Rotation_X.Text = value.X.ToString();
                TB_Rotation_Y.Text = value.Y.ToString();
                TB_Rotation_Z.Text = value.Z.ToString();
            }
        }
        public new Vector3 Scale
        {
            get
            {
                return new Vector3(
                    Convert.ToSingle(TB_Scale_X.Text),
                    Convert.ToSingle(TB_Scale_Y.Text),
                    Convert.ToSingle(TB_Scale_Z.Text)
                );
            }
            set
            {
                TB_Scale_X.Text = value.X.ToString();
                TB_Scale_Y.Text = value.Y.ToString();
                TB_Scale_Z.Text = value.Z.ToString();
            }
        }
        #endregion

        public Color LabelForeColor
        {
            get
            {
                return P_Transformation.ForeColor;
            }
            set
            {
                P_Transformation.ForeColor = value;
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
                P_Transformation.BackColor = value;

                B_Left_R_X.BackColor = value;
                B_Left_R_Y.BackColor = value;
                B_Left_R_Z.BackColor = value;
                B_Left_T_X.BackColor = value;
                B_Left_T_Y.BackColor = value;
                B_Left_T_Z.BackColor = value;
                B_Left_S_X.BackColor = value;
                B_Left_S_Y.BackColor = value;
                B_Left_S_Z.BackColor = value;
                B_Right_R_X.BackColor = value;
                B_Right_R_Y.BackColor = value;
                B_Right_R_Z.BackColor = value;
                B_Right_T_X.BackColor = value;
                B_Right_T_Y.BackColor = value;
                B_Right_T_Z.BackColor = value;
                B_Right_S_X.BackColor = value;
                B_Right_S_Y.BackColor = value;
                B_Right_S_Z.BackColor = value;
            }
        }
        public Color TextBoxBackColor
        {
            get
            {
                return TB_Rotation_X.BackColor;
            }
            set
            {
                TB_Translation_X.BackColor = value;
                TB_Translation_Y.BackColor = value;
                TB_Translation_Z.BackColor = value;
                TB_Rotation_X.BackColor = value;
                TB_Rotation_Y.BackColor = value;
                TB_Rotation_Z.BackColor = value;
                TB_Scale_X.BackColor = value;
                TB_Scale_Y.BackColor = value;
                TB_Scale_Z.BackColor = value;
                TB_Bias.BackColor = value;
            }
        }
        public Color TextBoxForeColor
        {
            get
            {
                return TB_Rotation_X.ForeColor;
            }
            set
            {
                TB_Translation_X.ForeColor = value;
                TB_Translation_Y.ForeColor = value;
                TB_Translation_Z.ForeColor = value;
                TB_Rotation_X.ForeColor = value;
                TB_Rotation_Y.ForeColor = value;
                TB_Rotation_Z.ForeColor = value;
                TB_Scale_X.ForeColor = value;
                TB_Scale_Y.ForeColor = value;
                TB_Scale_Z.ForeColor = value;
                TB_Bias.ForeColor = value;
            }
        }
        public Color ErrorColor { get; set; }
        public Image InactiveLeftButton { get; set; }
        public Image InactiveRightButton { get; set; }
        public Image ActiveLeftButton { get; set; }
        public Image ActiveRightButton { get; set; }
        public bool ShowTips { get; set; }
        #endregion


        #region Common functions

        private void InitializeVariables()
        {
            _tip = new ToolTip();
            if (ShowTips)
            {
                SetArrowButtonToolTips("1");
            }

            InactiveLeftButton = Properties.Resources.button_left_arrow_inactive;
            InactiveRightButton = Properties.Resources.button_right_arrow_inactive;
            ActiveLeftButton = Properties.Resources.button_left_arrow_active;
            ActiveRightButton = Properties.Resources.button_right_arrow_active;

            _gb_transformation_height = P_Transformation.Height;
        }

        private void InitializeMouseWheelEvents()
        {
            TB_Translation_X.MouseWheel += TB_Translation_X_MouseWheel;
            TB_Translation_Y.MouseWheel += TB_Translation_Y_MouseWheel;
            TB_Translation_Z.MouseWheel += TB_Translation_Z_MouseWheel;
            TB_Rotation_X.MouseWheel += TB_Rotation_X_MouseWheel;
            TB_Rotation_Y.MouseWheel += TB_Rotation_Y_MouseWheel;
            TB_Rotation_Z.MouseWheel += TB_Rotation_Z_MouseWheel;
            TB_Scale_X.MouseWheel += TB_Scale_X_MouseWheel;
            TB_Scale_Y.MouseWheel += TB_Scale_Y_MouseWheel;
            TB_Scale_Z.MouseWheel += TB_Scale_Z_MouseWheel;
            TB_Bias.MouseWheel += TB_Bias_MouseWheel;
        }

        private void ChangeOfButtonImages(Label button, bool activated, bool isLeft = true)
        {
            if (activated)
            {
                if (isLeft)
                {
                    button.Image = ActiveLeftButton;
                }
                else
                {
                    button.Image = ActiveRightButton;
                }
            }
            else
            {
                if (isLeft)
                {
                    button.Image = InactiveLeftButton;
                }
                else
                {
                    button.Image = InactiveRightButton;
                }
            }
        }

        private void SetArrowButtonToolTips(string tips)
        {
            _tip.SetToolTip(B_Left_T_X, "-" + tips);
            _tip.SetToolTip(B_Left_T_Y, "-" + tips);
            _tip.SetToolTip(B_Left_T_Z, "-" + tips);
            _tip.SetToolTip(B_Left_R_X, "-" + tips);
            _tip.SetToolTip(B_Left_R_Y, "-" + tips);
            _tip.SetToolTip(B_Left_R_Z, "-" + tips);
            _tip.SetToolTip(B_Left_S_X, "-" + tips);
            _tip.SetToolTip(B_Left_S_Y, "-" + tips);
            _tip.SetToolTip(B_Left_S_Z, "-" + tips);

            _tip.SetToolTip(B_Right_T_X, "+" + tips);
            _tip.SetToolTip(B_Right_T_Y, "+" + tips);
            _tip.SetToolTip(B_Right_T_Z, "+" + tips);
            _tip.SetToolTip(B_Right_R_X, "+" + tips);
            _tip.SetToolTip(B_Right_R_Y, "+" + tips);
            _tip.SetToolTip(B_Right_R_Z, "+" + tips);
            _tip.SetToolTip(B_Right_S_X, "+" + tips);
            _tip.SetToolTip(B_Right_S_Y, "+" + tips);
            _tip.SetToolTip(B_Right_S_Z, "+" + tips);
        }

        private void TextBoxRules(KeyPressEventArgs e, bool useDecimal = false)
        {
            if (!useDecimal)
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '-'))
                {
                    e.Handled = true;
                }
            }
            else
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.') && (e.KeyChar != '-'))
                {
                    e.Handled = true;
                }
            }
        }

        private void TextBoxIncrementation(TextBox textBox, bool useIncrement = false, bool useDecimal = false)
        {
            if (!useDecimal)
            {
                if (!useIncrement)
                {
                    textBox.Text = (Convert.ToInt32(textBox.Text) - Convert.ToInt32(TB_Bias.Text)).ToString();
                }
                else
                {
                    textBox.Text = (Convert.ToInt32(textBox.Text) + Convert.ToInt32(TB_Bias.Text)).ToString();
                }
            }
            else
            {
                if (!useIncrement)
                {
                    textBox.Text = (Convert.ToSingle(textBox.Text) - Convert.ToInt32(TB_Bias.Text)).ToString();
                }
                else
                {
                    textBox.Text = (Convert.ToSingle(textBox.Text) + Convert.ToInt32(TB_Bias.Text)).ToString();
                }
                
            }
        }

        #endregion

        #region Decrement buttons

        private void B_Left_T_X_Click(object sender, EventArgs e)
        {
            TextBoxIncrementation(TB_Translation_X);
        }

        private void B_Left_T_Y_Click(object sender, EventArgs e)
        {
            TextBoxIncrementation(TB_Translation_Y);
        }

        private void B_Left_T_Z_Click(object sender, EventArgs e)
        {
            TextBoxIncrementation(TB_Translation_Z);
        }

        private void B_Left_R_X_Click(object sender, EventArgs e)
        {
            TextBoxIncrementation(TB_Rotation_X);
        }

        private void B_Left_R_Y_Click(object sender, EventArgs e)
        {
            TextBoxIncrementation(TB_Rotation_Y);
        }

        private void B_Left_R_Z_Click(object sender, EventArgs e)
        {
            TextBoxIncrementation(TB_Rotation_Z);
        }

        private void B_Left_S_X_Click(object sender, EventArgs e)
        {
            TextBoxIncrementation(TB_Scale_X);
        }

        private void B_Left_S_Y_Click(object sender, EventArgs e)
        {
            TextBoxIncrementation(TB_Scale_Y);
        }

        private void B_Left_S_Z_Click(object sender, EventArgs e)
        {
            TextBoxIncrementation(TB_Scale_Z);
        }

        private void B_Left_T_X_DoubleClick(object sender, EventArgs e)
        {
            TextBoxIncrementation(TB_Translation_X);
        }

        private void B_Left_T_Y_DoubleClick(object sender, EventArgs e)
        {
            TextBoxIncrementation(TB_Translation_Y);
        }

        private void B_Left_T_Z_DoubleClick(object sender, EventArgs e)
        {
            TextBoxIncrementation(TB_Translation_Z);
        }

        private void B_Left_R_X_DoubleClick(object sender, EventArgs e)
        {
            TextBoxIncrementation(TB_Rotation_X);
        }

        private void B_Left_R_Y_DoubleClick(object sender, EventArgs e)
        {
            TextBoxIncrementation(TB_Rotation_Y);
        }

        private void B_Left_R_Z_DoubleClick(object sender, EventArgs e)
        {
            TextBoxIncrementation(TB_Rotation_Z);
        }

        private void B_Left_S_X_DoubleClick(object sender, EventArgs e)
        {
            TextBoxIncrementation(TB_Scale_X);
        }

        private void B_Left_S_Y_DoubleClick(object sender, EventArgs e)
        {
            TextBoxIncrementation(TB_Scale_Y);
        }

        private void B_Left_S_Z_DoubleClick(object sender, EventArgs e)
        {
            TextBoxIncrementation(TB_Scale_Z);
        }

        #endregion

        #region Increment buttons

        private void B_Right_T_X_Click(object sender, EventArgs e)
        {
            TextBoxIncrementation(TB_Translation_X, true);
        }

        private void B_Right_T_Y_Click(object sender, EventArgs e)
        {
            TextBoxIncrementation(TB_Translation_Y, true);
        }

        private void B_Right_T_Z_Click(object sender, EventArgs e)
        {
            TextBoxIncrementation(TB_Translation_Z, true);
        }

        private void B_Right_R_X_Click(object sender, EventArgs e)
        {
            TextBoxIncrementation(TB_Rotation_X, true);
        }

        private void B_Right_R_Y_Click(object sender, EventArgs e)
        {
            TextBoxIncrementation(TB_Rotation_Y, true);
        }

        private void B_Right_R_Z_Click(object sender, EventArgs e)
        {
            TextBoxIncrementation(TB_Rotation_Z, true);
        }

        private void B_Right_S_X_Click(object sender, EventArgs e)
        {
            TextBoxIncrementation(TB_Scale_X, true);
        }

        private void B_Right_S_Y_Click(object sender, EventArgs e)
        {
            TextBoxIncrementation(TB_Scale_Y, true);
        }

        private void B_Right_S_Z_Click(object sender, EventArgs e)
        {
            TextBoxIncrementation(TB_Scale_Z, true);
        }

        private void B_Right_T_X_DoubleClick(object sender, EventArgs e)
        {
            TextBoxIncrementation(TB_Translation_X, true);
        }

        private void B_Right_T_Y_DoubleClick(object sender, EventArgs e)
        {
            TextBoxIncrementation(TB_Translation_Y, true);
        }

        private void B_Right_T_Z_DoubleClick(object sender, EventArgs e)
        {
            TextBoxIncrementation(TB_Translation_Z, true);
        }

        private void B_Right_R_X_DoubleClick(object sender, EventArgs e)
        {
            TextBoxIncrementation(TB_Rotation_X, true);
        }

        private void B_Right_R_Y_DoubleClick(object sender, EventArgs e)
        {
            TextBoxIncrementation(TB_Rotation_Y, true);
        }

        private void B_Right_R_Z_DoubleClick(object sender, EventArgs e)
        {
            TextBoxIncrementation(TB_Rotation_Z, true);
        }

        private void B_Right_S_X_DoubleClick(object sender, EventArgs e)
        {
            TextBoxIncrementation(TB_Scale_X, true);
        }

        private void B_Right_S_Y_DoubleClick(object sender, EventArgs e)
        {
            TextBoxIncrementation(TB_Scale_Y, true);
        }

        private void B_Right_S_Z_DoubleClick(object sender, EventArgs e)
        {
            TextBoxIncrementation(TB_Scale_Z, true);
        }

        #endregion

        #region Right Button MouseEnter Event

        private void B_Right_T_X_MouseEnter(object sender, EventArgs e)
        {
            ChangeOfButtonImages(B_Right_T_X, true, false);
        }

        private void B_Right_T_Y_MouseEnter(object sender, EventArgs e)
        {
            ChangeOfButtonImages(B_Right_T_Y, true, false);
        }

        private void B_Right_T_Z_MouseEnter(object sender, EventArgs e)
        {
            ChangeOfButtonImages(B_Right_T_Z, true, false);
        }

        private void B_Right_R_X_MouseEnter(object sender, EventArgs e)
        {
            ChangeOfButtonImages(B_Right_R_X, true, false);
        }

        private void B_Right_R_Y_MouseEnter(object sender, EventArgs e)
        {
            ChangeOfButtonImages(B_Right_R_Y, true, false);
        }

        private void B_Right_R_Z_MouseEnter(object sender, EventArgs e)
        {
            ChangeOfButtonImages(B_Right_R_Z, true, false);
        }

        private void B_Right_S_X_MouseEnter(object sender, EventArgs e)
        {
            ChangeOfButtonImages(B_Right_S_X, true, false);
        }

        private void B_Right_S_Y_MouseEnter(object sender, EventArgs e)
        {
            ChangeOfButtonImages(B_Right_S_Y, true, false);
        }

        private void B_Right_S_Z_MouseEnter(object sender, EventArgs e)
        {
            ChangeOfButtonImages(B_Right_S_Z, true, false);
        }

        #endregion

        #region Right Button MouseLeave Event

        private void B_Right_T_X_MouseLeave(object sender, EventArgs e)
        {
            ChangeOfButtonImages(B_Right_T_X, false, false);
        }

        private void B_Right_T_Y_MouseLeave(object sender, EventArgs e)
        {
            ChangeOfButtonImages(B_Right_T_Y, false, false);
        }

        private void B_Right_T_Z_MouseLeave(object sender, EventArgs e)
        {
            ChangeOfButtonImages(B_Right_T_Z, false, false);
        }

        private void B_Right_R_X_MouseLeave(object sender, EventArgs e)
        {
            ChangeOfButtonImages(B_Right_R_X, false, false);
        }

        private void B_Right_R_Y_MouseLeave(object sender, EventArgs e)
        {
            ChangeOfButtonImages(B_Right_R_Y, false, false);
        }

        private void B_Right_R_Z_MouseLeave(object sender, EventArgs e)
        {
            ChangeOfButtonImages(B_Right_R_Z, false, false);
        }

        private void B_Right_S_X_MouseLeave(object sender, EventArgs e)
        {
            ChangeOfButtonImages(B_Right_S_X, false, false);
        }

        private void B_Right_S_Y_MouseLeave(object sender, EventArgs e)
        {
            ChangeOfButtonImages(B_Right_S_Y, false, false);
        }

        private void B_Right_S_Z_MouseLeave(object sender, EventArgs e)
        {
            ChangeOfButtonImages(B_Right_S_Z, false, false);
        }

        #endregion

        #region Left Button MouseEnter Event

        private void B_Left_T_X_MouseEnter(object sender, EventArgs e)
        {
            ChangeOfButtonImages(B_Left_T_X, true);
        }

        private void B_Left_T_Y_MouseEnter(object sender, EventArgs e)
        {
            ChangeOfButtonImages(B_Left_T_Y, true);
        }

        private void B_Left_T_Z_MouseEnter(object sender, EventArgs e)
        {
            ChangeOfButtonImages(B_Left_T_Z, true);
        }

        private void B_Left_R_X_MouseEnter(object sender, EventArgs e)
        {
            ChangeOfButtonImages(B_Left_R_X, true);
        }

        private void B_Left_R_Y_MouseEnter(object sender, EventArgs e)
        {
            ChangeOfButtonImages(B_Left_R_Y, true);
        }

        private void B_Left_R_Z_MouseEnter(object sender, EventArgs e)
        {
            ChangeOfButtonImages(B_Left_R_Z, true);
        }

        private void B_Left_S_X_MouseEnter(object sender, EventArgs e)
        {
            ChangeOfButtonImages(B_Left_S_X, true);
        }

        private void B_Left_S_Y_MouseEnter(object sender, EventArgs e)
        {
            ChangeOfButtonImages(B_Left_S_Y, true);
        }

        private void B_Left_S_Z_MouseEnter(object sender, EventArgs e)
        {
            ChangeOfButtonImages(B_Left_S_Z, true);
        }

        #endregion

        #region Left Button MouseLeave Event

        private void B_Left_T_X_MouseLeave(object sender, EventArgs e)
        {
            ChangeOfButtonImages(B_Left_T_X, false);
        }

        private void B_Left_T_Y_MouseLeave(object sender, EventArgs e)
        {
            ChangeOfButtonImages(B_Left_T_Y, false);
        }

        private void B_Left_T_Z_MouseLeave(object sender, EventArgs e)
        {
            ChangeOfButtonImages(B_Left_T_Z, false);
        }

        private void B_Left_R_X_MouseLeave(object sender, EventArgs e)
        {
            ChangeOfButtonImages(B_Left_R_X, false);
        }

        private void B_Left_R_Y_MouseLeave(object sender, EventArgs e)
        {
            ChangeOfButtonImages(B_Left_R_Y, false);
        }

        private void B_Left_R_Z_MouseLeave(object sender, EventArgs e)
        {
            ChangeOfButtonImages(B_Left_R_Z, false);
        }

        private void B_Left_S_X_MouseLeave(object sender, EventArgs e)
        {
            ChangeOfButtonImages(B_Left_S_X, false);
        }

        private void B_Left_S_Y_MouseLeave(object sender, EventArgs e)
        {
            ChangeOfButtonImages(B_Left_S_Y, false);
        }

        private void B_Left_S_Z_MouseLeave(object sender, EventArgs e)
        {
            ChangeOfButtonImages(B_Left_S_Z, false);
        }

        #endregion

        #region Text Box Events

        #region Text Box KeyPress Event

        private void TB_Translation_X_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBoxRules(e);
        }

        private void TB_Translation_Y_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBoxRules(e);
        }

        private void TB_Translation_Z_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBoxRules(e);
        }

        private void TB_Rotation_X_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBoxRules(e);
        }

        private void TB_Rotation_Y_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBoxRules(e);
        }

        private void TB_Rotation_Z_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBoxRules(e);
        }

        private void TB_Scale_X_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBoxRules(e);
        }

        private void TB_Scale_Y_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBoxRules(e);
        }

        private void TB_Scale_Z_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBoxRules(e);
        }

        private void TB_Bias_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar == '-'))
            {
                e.Handled = true;
            }
        }

        #endregion

        #region Text Box MouseWheel Event

        private void TB_Bias_MouseWheel(object sender, MouseEventArgs e)
        {
            int delta = Convert.ToInt32(TB_Bias.Text) + e.Delta / 120;
            if (delta > 0)
            {
                TB_Bias.Text = delta.ToString();
            }
        }

        private void TB_Translation_X_MouseWheel(object sender, MouseEventArgs e)
        {
            TextBoxIncrementation(TB_Translation_X, (e.Delta > 0) ? true : false);
        }

        private void TB_Translation_Y_MouseWheel(object sender, MouseEventArgs e)
        {
            TextBoxIncrementation(TB_Translation_Y, (e.Delta > 0) ? true : false);
        }

        private void TB_Translation_Z_MouseWheel(object sender, MouseEventArgs e)
        {
            TextBoxIncrementation(TB_Translation_Z, (e.Delta > 0) ? true : false);
        }

        private void TB_Rotation_X_MouseWheel(object sender, MouseEventArgs e)
        {
            TextBoxIncrementation(TB_Rotation_X, (e.Delta > 0) ? true : false);
        }

        private void TB_Rotation_Y_MouseWheel(object sender, MouseEventArgs e)
        {
            TextBoxIncrementation(TB_Rotation_Y, (e.Delta > 0) ? true : false);
        }

        private void TB_Rotation_Z_MouseWheel(object sender, MouseEventArgs e)
        {
            TextBoxIncrementation(TB_Rotation_Z, (e.Delta > 0) ? true : false);
        }

        private void TB_Scale_X_MouseWheel(object sender, MouseEventArgs e)
        {
            TextBoxIncrementation(TB_Scale_X, (e.Delta > 0) ? true : false);
        }

        private void TB_Scale_Y_MouseWheel(object sender, MouseEventArgs e)
        {
            TextBoxIncrementation(TB_Scale_Y, (e.Delta > 0) ? true : false);
        }

        private void TB_Scale_Z_MouseWheel(object sender, MouseEventArgs e)
        {
            TextBoxIncrementation(TB_Scale_Z, (e.Delta > 0) ? true : false);
        }

        #endregion

        private void TB_Bias_TextChanged(object sender, EventArgs e)
        {
            if ((TB_Bias.Text == "0") || (TB_Bias.Text == String.Empty))
            {
                TB_Bias.Text = "1";
            }
        }

        #endregion

    }
}
