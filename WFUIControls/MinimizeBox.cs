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
    public partial class MinimizeBox : UserControl
    {
        public MinimizeBox()
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
        public int MaxHeight
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
        public bool IsMinimized { get; set; }
        public Image Maximized { get; set; }
        public Image Minimized { get; set; }
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
                    TLP.Controls.Add(value, 0, 1);
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
                else if ((value.MinimumSize != Size.Empty) && (value.MaximumSize == Size.Empty))
                {
                    TLP.Controls.Add(value, 0, 1);
                    TLP.Controls[TLP.Controls.Count - 1].Dock = DockStyle.Fill;
                    this.MinimumSize = new Size(
                        TLP.Controls[TLP.Controls.Count - 1].MinimumSize.Width + 8,
                        27
                    );
                }
                else if ((value.MinimumSize == Size.Empty) && (value.MaximumSize != Size.Empty))
                {
                    TLP.Controls.Add(value, 0, 1);
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
                    TLP.Controls.Add(value, 0, 1);
                }
            }
        }
        public RightToLeft RightToLeftAlign
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

        private bool _clicked = false;
        private int _minimizeBoxHeight;
        private int _tblHeight;
        #endregion


        #region Common functions

        private void InitializeVariables()
        {
            Maximized = Properties.Resources.maximized;
            Minimized = Properties.Resources.minimized;
            HeaderInactiveForeColor = Color.FromKnownColor(KnownColor.ButtonFace);
            HeaderActiveForeColor = Color.FromKnownColor(KnownColor.MenuHighlight);
            this.MinimumSize = new Size(0, 27);
        }

        private void HeaderClickAction()
        {
            if (!_clicked)
            {
                if (MaxHeight == 0)
                {
                    _minimizeBoxHeight = this.Height;
                    _tblHeight = TLP.Height;
                }

                TLP.Height = 25;
                this.Height = 27;

                IsMinimized = true;
                _clicked = true;
            }
            else
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

                IsMinimized = false;
                _clicked = false;
            }
        }

        public void Minimize()
        {
            if (MaxHeight == 0)
            {
                _minimizeBoxHeight = this.Height;
                _tblHeight = TLP.Height;
            }

            TLP.Height = 25;
            this.Height = 27;

            IsMinimized = true;
            _clicked = true;
        }

        #endregion

        private void OnLoad(object sender, EventArgs e)
        {
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
        #endregion

    }
}
