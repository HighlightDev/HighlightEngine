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

using Tao.DevIl;
using Tao.FreeGlut;
using Tao.OpenGl;
using Tao.Platform.Windows;

using CParser;
using CParser.ASE_Parser;
using CParser.OBJ_Parser;
using CParser.DAE_Parser;


namespace RenderWindow
{
    public partial class RenderForm : Form
    {
        public RenderForm()
        {
            InitializeComponent();
            OpenGlWindow.InitializeContexts();
        }
        
        double x = 0, y = 0, z = -5, angle = 0, zoom = 1;
        //int axisX = 1, axisY = 0, axisZ = 0;
        //int currentFrame = 0;

        ASE_ModelLoader model = null;
        OBJ_ModelLoader objModel = null;
        OBJ_SimpleAnimationLoader animation = null;
        DAE_Geometries animEx = null;


        private void FormLoad(object sender, EventArgs e)
        {
            Glut.glutInit();
            Glut.glutInitDisplayMode(Glut.GLUT_RGB | Glut.GLUT_DOUBLE | Glut.GLUT_DEPTH);
            Il.ilInit();

            Il.ilEnable(Il.IL_ORIGIN_SET);
            
            Gl.glClearColor(255, 255, 255, 1);

            Gl.glViewport(0, 0, OpenGlWindow.Width, OpenGlWindow.Height);

            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glLoadIdentity();

            Glu.gluPerspective(45, (float)OpenGlWindow.Width / (float)OpenGlWindow.Height, 0.1, 200);

            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glLoadIdentity();

            Gl.glEnable(Gl.GL_DEPTH_TEST);
            Gl.glEnable(Gl.GL_LIGHTING);
            Gl.glEnable(Gl.GL_LIGHT0);

            Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);
            Gl.glEnable(Gl.GL_BLEND);
            Gl.glEnable(Gl.GL_LINE_SMOOTH);
            Gl.glLineWidth(1.0f);

            // опиции для загрузки файла
            openFileDialogForModel.Filter = "obj files (*.obj)|*.obj|dae files (*.dae)|*.dae|ase files (*.ase)|*.ase|All files (*.*)|*.*";
        }

        private void loadASEModelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialogForModel.ShowDialog() == DialogResult.OK)
            {
                if (openFileDialogForModel.FileName.EndsWith(".ase") || openFileDialogForModel.FileName.EndsWith(".ASE"))
                {
                    model = new ASE_ModelLoader(true, openFileDialogForModel.FileName);
                }
            }
        }

        private void loadOBJModelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialogForModel.ShowDialog() == DialogResult.OK)
            {
                if (openFileDialogForModel.FileName.EndsWith(".obj") || openFileDialogForModel.FileName.EndsWith(".OBJ"))
                {
                    objModel = new OBJ_ModelLoaderEx(true, openFileDialogForModel.FileName);
                }
            }
        }

        private void loadSimpleAnimationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            folderBrowserDialogForAnimation.ShowNewFolderButton = false;

            if (folderBrowserDialogForAnimation.ShowDialog() == DialogResult.OK)
            {
                animation = new OBJ_SimpleAnimationLoader();
                animation.LoadAnimation(folderBrowserDialogForAnimation.SelectedPath);
            }
        }

        private void loadAnimExToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialogForModel.ShowDialog() == DialogResult.OK)
            {
                if (openFileDialogForModel.FileName.EndsWith(".dae") || openFileDialogForModel.FileName.EndsWith(".DAE"))
                {
                    animEx = new DAE_Geometries(openFileDialogForModel.FileName);
                }
            }
        }

        private void assimpMLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialogForModel.ShowDialog() == DialogResult.OK)
            {
                AssimpModelLoader model = new AssimpModelLoader(openFileDialogForModel.FileName);
            }
        }


        private void RenderTimerTick(object sender, EventArgs e)
        {
            //Draw();
        }

        private void trackBarZoomScroll(object sender, EventArgs e)
        {
            zoom = (double)trackBarZoom.Value / 1000.0;
            lZoom.Text = zoom.ToString();
            //Draw();
        }

        private void trackBarAngleScroll(object sender, EventArgs e)
        {
            angle = (double)trackBarAngle.Value;
            lAngle.Text = angle.ToString();
            //Draw();
        }

        private void trackBarAxisXScroll(object sender, EventArgs e)
        {
            x = (double)trackBarAxisX.Value / 1000.0;
            lAxisX.Text = x.ToString();
            //Draw();
        }

        private void trackBarAxisYScroll(object sender, EventArgs e)
        {
            y = (double)trackBarAxisY.Value / 1000.0;
            lAxisY.Text = y.ToString();
            //Draw();
        }

        private void trackBarAxisZScroll(object sender, EventArgs e)
        {
            z = (double)trackBarAxisZ.Value / 1000.0;
            lAxisZ.Text = z.ToString();
            //Draw();
        }

        private void OnBtnExit(object sender, EventArgs e)
        {
            Close();
        }

        // to test smt
        private void testToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

    }
}