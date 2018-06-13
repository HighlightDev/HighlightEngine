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
    public partial class MinimizeCheckBox : UserControl
    {
        public MinimizeCheckBox()
        {
            InitializeComponent();
            InitializeVariables();
        }


        #region Variables
        public string HeaderText
        {
            get
            {
                return L_Header.Text;
            }
            set
            {
                L_Header.Text = "   " + value.Trim();
            }
        }
        public string CheckBoxText
        {
            get
            {
                return CB_Label.Text;
            }
            set
            {
                CB_Label.Text = "      " + value.Trim();
            }
        }
        public Int32 MaxHeight
        {
            get
            {
                return this.MaximumSize.Height;
            }
            set
            {
                this.MaximumSize = new Size(0, value);
            }
        }
        public bool Checked { get; set; }
        public bool IsMinimized { get; set; }
        public Image Maximized { get; set; }
        public Image Minimized { get; set; }
        public Image CheckBoxInactive { get; set; }
        public Image CheckBoxActive { get; set; }
        public Image CheckBoxChecked { get; set; }
        public Color HeaderInactiveForeColor { get; set; }
        public Color HeaderActiveForeColor { get; set; }
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
                L_Header.BackColor = value;
            }
        }
        public Control MinimizingPart
        {
            get
            {
                return (TLP.Controls.Count > 0) ? TLP.Controls[TLP.Controls.Count - 1] : null;
            }
            set
            {
                if ((value.MinimumSize != Size.Empty) && (value.MaximumSize != Size.Empty))
                {
                    TLP.Controls.Add(value, 0, 2);
                    TLP.Controls[TLP.Controls.Count - 1].Dock = DockStyle.Fill;
                    this.MaximumSize = new Size(
                        TLP.Controls[TLP.Controls.Count - 1].MaximumSize.Width,
                        TLP.Controls[TLP.Controls.Count - 1].MaximumSize.Height + 50
                    );
                    this.MinimumSize = new Size(
                        TLP.Controls[TLP.Controls.Count - 1].MinimumSize.Width + 8,
                        27
                    );
                }
                else if((value.MinimumSize != Size.Empty) && (value.MaximumSize == Size.Empty))
                {
                    TLP.Controls.Add(value, 0, 2);
                    TLP.Controls[TLP.Controls.Count - 1].Dock = DockStyle.Fill;
                    this.MinimumSize = new Size(
                        TLP.Controls[TLP.Controls.Count - 1].MinimumSize.Width + 8,
                        27
                    );
                }
                else if ((value.MinimumSize == Size.Empty) && (value.MaximumSize != Size.Empty))
                {
                    TLP.Controls.Add(value, 0, 2);
                    TLP.Controls[TLP.Controls.Count - 1].Dock = DockStyle.Fill;
                    this.MaximumSize = new Size(
                        TLP.Controls[TLP.Controls.Count - 1].MaximumSize.Width,
                        TLP.Controls[TLP.Controls.Count - 1].MaximumSize.Height + 50
                    );
                    this.MinimumSize = new Size(
                        0,
                        27
                    );
                }
                else
                {
                    TLP.Controls.Add(value, 0, 2);
                }
            }
        }
        public RightToLeft RightToLeftHeaderAlign
        {
            get
            {
                return L_Header.RightToLeft;
            }
            set
            {
                L_Header.RightToLeft = value;
            }
        }

        private bool _clicked = false; // if false then MCB will be minimized, else - will be maximized
        private Int32 _minimizeBoxHeight;
        private Int32 _tblHeight;
        #endregion


        #region Common functions

        private void InitializeVariables()
        {
            Maximized = Properties.Resources.maximized;
            Minimized = Properties.Resources.minimized;
            CheckBoxInactive = Properties.Resources.check_box_inactive;
            CheckBoxActive = Properties.Resources.check_box_active;
            CheckBoxChecked = Properties.Resources.check_box_checked;
            HeaderInactiveForeColor = Color.FromKnownColor(KnownColor.ButtonFace);
            HeaderActiveForeColor = Color.FromKnownColor(KnownColor.MenuHighlight);
            Checked = true;
        }

        private void HeaderClickAction()
        {
            if (!_clicked) // if first click
            {
                if (Checked) // if CB is true then save height
                {
                    if (MaxHeight == 0)
                    {
                        _minimizeBoxHeight = this.Height;
                        _tblHeight = TLP.Height;
                    }
                }

                TLP.Height = 25;
                this.Height = 27;

                IsMinimized = true;
                _clicked = true;
            }
            else // if second click
            {
                if (Checked) // if CB is true then set height to previously saved one
                {
                    if (MaxHeight == 0)
                    {
                        TLP.Height = _tblHeight;
                        this.Height = _minimizeBoxHeight;
                    }
                    else
                    {
                        TLP.Height = MaxHeight - 2;
                        this.Height = MaxHeight;
                    }
                }
                else // if this case set height equal to first two rows
                {
                    TLP.Height = 50;
                    this.Height = 53;
                }

                IsMinimized = false;
                _clicked = false;
            }
        }

        private void CheckBoxClickAction()
        {
            if (!Checked) // if CB is true
            {
                CB_Label.Image = CheckBoxChecked;
                Checked = !Checked;

                TLP.Height = _tblHeight;
                this.Height = _minimizeBoxHeight; 
            }
            else // if CB is false
            {
                CB_Label.Image = CheckBoxActive;
                Checked = !Checked;

                _minimizeBoxHeight = this.Height;
                _tblHeight = TLP.Height;

                TLP.Height = 50;
                this.Height = 53;
            }
        }

        public void Minimize()
        {
            if (Checked) // if CB is true then save height
            {
                if (MaxHeight == 0)
                {
                    _minimizeBoxHeight = this.Height;
                    _tblHeight = TLP.Height;
                }
            }

            TLP.Height = 25;
            this.Height = 27;

            IsMinimized = true;
            _clicked = true;
        }

        public void Maximize()
        {
            if (Checked) // if CB is true then set height to previously saved one
            {
                if (MaxHeight == 0)
                {
                    TLP.Height = _tblHeight;
                    this.Height = _minimizeBoxHeight;
                }
                else
                {
                    TLP.Height = MaxHeight - 2;
                    this.Height = MaxHeight;
                }
            }
            else // in this case set height equal to first two rows
            {
                TLP.Height = 50;
                this.Height = 53;
            }

            IsMinimized = false;
            _clicked = false;
        }

        #endregion

        private void OnLoad(object sender, EventArgs e)
        {
            if (!Checked)
            {
                CB_Label.Image = Properties.Resources.check_box_inactive;

                _minimizeBoxHeight = this.Height;
                _tblHeight = TLP.Height;

                TLP.Height = 50;
                this.Height = 53;
            }
            if (IsMinimized)
            {
                _minimizeBoxHeight = this.Height;
                _tblHeight = TLP.Height;

                TLP.Height = 25;
                this.Height = 27;

                _clicked = !_clicked;
            }
        }

        #region Mouse Events
        private void l_header_Click(object sender, EventArgs e)
        {
            HeaderClickAction();
        }
        private void L_Header_DoubleClick(object sender, EventArgs e)
        {
            HeaderClickAction();
        }
        private void l_header_MouseEnter(object sender, EventArgs e)
        {
            L_Header.Image = Maximized;
            L_Header.ForeColor = HeaderActiveForeColor;
        }
        private void l_header_MouseLeave(object sender, EventArgs e)
        {
            L_Header.Image = Minimized;
            L_Header.ForeColor = HeaderInactiveForeColor;
        }

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
            CB_Label.ForeColor = HeaderActiveForeColor;
        }
        private void CB_Label_MouseLeave(object sender, EventArgs e)
        {
            if (!Checked)
            {
                CB_Label.Image = CheckBoxInactive;
            }
            CB_Label.ForeColor = HeaderInactiveForeColor;
        }
        #endregion

    }
}
