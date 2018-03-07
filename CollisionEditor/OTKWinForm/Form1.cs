using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OTKWinForm.Core;
using System.Threading;

namespace OTKWinForm
{
    public partial class OTKWinForm : Form
    {
        private EditorCore Editor;
        private Point PrevCursorPosition;
        private System.Threading.Timer timer;

        private Core.Component selectedComponent;
        private Actor selectedActor;

        private bool[] enabledKeys;

        private bool bTreeViewDirty = false;
        private bool bTransformationPanelDirty = false;

        public OTKWinForm()
        {
            InitializeComponent();
            Editor = new EditorCore();
            timer = new System.Threading.Timer(new TimerCallback(TickWorld));
            timer.Change(0, 10);
            PrevCursorPosition = new Point(this.Width / 2, this.Height / 2);
            enabledKeys = new bool[4] { false, false, false, false };

            treeView1.NodeMouseClick += TreeView1_NodeMouseClick;
        }

        private void TreeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            string nodeText = e.Node.Text;
            if (nodeText == "Actor")
            {
                selectedActor = Editor.actor;
                selectedComponent = null;
                bTransformationPanelDirty = true;
            }
            else
            {
                Int32 itemIndex = nodeText.IndexOf('_');
                string idStr = nodeText.Replace(nodeText.Substring(0, itemIndex + 1), "");
                UInt32 index = UInt32.Parse(idStr);
                selectedActor = null;
                selectedComponent = Core.Component.ComponentCreator.GetComponentById(index);
                bTransformationPanelDirty = true;
            }
        }

        private void ClearBuffers()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
            GL.ClearColor(Color.Beige);
        }

        private void ResizeCallback(object sender, EventArgs e)
        {
            GL.Viewport(0, 0, this.GLControl.Width, this.GLControl.Height);
            GLControl.SwapBuffers();
        }

        private void TickWorld(object sender)
        {
            Editor.TickEditor();
            for (Int32 i = 0; i < enabledKeys.Length; i++)
            {
                if (enabledKeys[i])
                    Editor.EditorCamera.FirstPersonMove((Camera.Direction)(i));
            }

            UpdateEditorGUI();
        }

        private void UpdateEditorGUI()
        {
            if (Editor.actor != null && bTreeViewDirty)
            {
                if (treeView1.InvokeRequired)
                {
                    treeView1.Invoke(new Action(() =>
                    {
                        treeView1.BeginUpdate();
                        treeView1.Nodes.Clear();
                        treeView1.Nodes.Add("Actor");
                        AddTreeViewNodes(0, treeView1.Nodes[0].Nodes, Editor.actor.GetComponents());
                        treeView1.EndUpdate();
                        treeView1.ExpandAll();
                    }));
                    bTreeViewDirty = false;
                }
            }
            if (bTransformationPanelDirty)
            {
                Vector3 translation = new Vector3(), rotation = new Vector3(), scale = new Vector3();
                if (selectedActor != null)
                {
                    translation = selectedActor.Translation;
                    rotation = selectedActor.Rotation;
                    scale = selectedActor.Scale;
                }
                else if (selectedComponent != null)
                {
                    translation = selectedComponent.Translation;
                    rotation = selectedComponent.Rotation;
                    scale = selectedComponent.Scale;
                }
                this.Invoke(new Action(() =>
                {
                    this.trackBarTranslationX.Value = (Int32)(translation.X * 0.5f) + 50;
                    this.trackBarTranslationY.Value = (Int32)(translation.Y * 0.5f) + 50;
                    this.trackBarTranslationZ.Value = (Int32)(translation.Z * 0.5f) + 50;

                    this.trackBarRotationX.Value = (Int32)(rotation.X * 0.27f);
                    this.trackBarRotationY.Value = (Int32)(rotation.Y * 0.27f);
                    this.trackBarRotationZ.Value = (Int32)(rotation.Z * 0.27f);

                    this.trackBarScaleX.Value = (Int32)(scale.X * 100f);
                    this.trackBarScaleY.Value = (Int32)(scale.Y * 100f);
                    this.trackBarScaleZ.Value = (Int32)(scale.Z * 100f);
                }));

                bTransformationPanelDirty = false;
            }
        }

        private void TrackBarValueChanged(object sender, EventArgs e)
        {
            if (sender == trackBarTranslationX)
            {
                float value = (trackBarTranslationX.Value * 2) - 100.0f;
                if (selectedActor != null)
                    selectedActor.Translation = new Vector3(value, selectedActor.Translation.Y, selectedActor.Translation.Z);
                else if (selectedComponent != null)
                    selectedComponent.Translation = new Vector3(value, selectedComponent.Translation.Y, selectedComponent.Translation.Z);
            }
            else if (sender == trackBarTranslationY)
            {
                float value = (trackBarTranslationY.Value * 2) - 100.0f;
                if (selectedActor != null)
                    selectedActor.Translation = new Vector3(selectedActor.Translation.X, value, selectedActor.Translation.Z);
                else if (selectedComponent != null)
                    selectedComponent.Translation = new Vector3(selectedComponent.Translation.X, value, selectedComponent.Translation.Z);
            }
            else if (sender == trackBarTranslationZ)
            {
                float value = (trackBarTranslationZ.Value * 2) - 100.0f;
                if (selectedActor != null)
                    selectedActor.Translation = new Vector3(selectedActor.Translation.X, selectedActor.Translation.Y, value);
                else if (selectedComponent != null)
                    selectedComponent.Translation = new Vector3(selectedComponent.Translation.X, selectedComponent.Translation.Y, value);
            }
            else if (sender == trackBarRotationX)
            {
                float value = trackBarRotationX.Value * 3.6f;
                if (selectedActor != null)
                    selectedActor.Rotation = new Vector3(value, selectedActor.Rotation.Y, selectedActor.Rotation.Z);
                else if (selectedComponent != null)
                    selectedComponent.Rotation = new Vector3(value, selectedComponent.Rotation.Y, selectedComponent.Rotation.Z);
            }
            else if (sender == trackBarRotationY)
            {
                float value = trackBarRotationY.Value * 3.6f;
                if (selectedActor != null)
                    selectedActor.Rotation = new Vector3(selectedActor.Rotation.X, value, selectedActor.Rotation.Z);
                else if (selectedComponent != null)
                    selectedComponent.Rotation = new Vector3(selectedComponent.Rotation.X, value, selectedComponent.Rotation.Z);
            }
            else if (sender == trackBarRotationZ)
            {
                float value = trackBarRotationZ.Value * 3.6f;
                if (selectedActor != null)
                    selectedActor.Rotation = new Vector3(selectedActor.Rotation.X, selectedActor.Rotation.Y, value);
                else if (selectedComponent != null)
                    selectedComponent.Rotation = new Vector3(selectedComponent.Rotation.X, selectedComponent.Rotation.Y, value);
            }
            else if (sender == trackBarScaleX)
            {
                float value = trackBarScaleX.Value * 0.01f;
                if (selectedActor != null)
                    selectedActor.Scale = new Vector3(value, selectedActor.Scale.Y, selectedActor.Scale.Z);
                else if (selectedComponent != null)
                    selectedComponent.Scale = new Vector3(value, selectedComponent.Scale.Y, selectedComponent.Scale.Z);
            }
            else if (sender == trackBarScaleY)
            {
                float value = trackBarScaleY.Value * 0.01f;
                if (selectedActor != null)
                    selectedActor.Scale = new Vector3(selectedActor.Scale.X, value, selectedActor.Scale.Z);
                else if (selectedComponent != null)
                    selectedComponent.Scale = new Vector3(selectedComponent.Scale.X, value, selectedComponent.Scale.Z);
            }
            else if (sender == trackBarScaleZ)
            {
                float value = trackBarScaleZ.Value * 0.01f;
                if (selectedActor != null)
                    selectedActor.Scale = new Vector3(selectedActor.Scale.X, selectedActor.Scale.Y, value);
                else if (selectedComponent != null)
                    selectedComponent.Scale = new Vector3(selectedComponent.Scale.X, selectedComponent.Scale.Y, value);
            }
        }

        private void AddTreeViewNodes(Int32 parentNodeIndex, TreeNodeCollection parentNodeCollection, List<Core.Component> childrenComponents)
        {
            for (var i = 0; i < childrenComponents.Count; i++, parentNodeIndex++)
            {
                parentNodeCollection.Add(childrenComponents[i].ToString());
                AddTreeViewNodes(parentNodeIndex, parentNodeCollection[i].Nodes, childrenComponents[i].GetComponents());
            }
        }

        private void RenderTick(object sender, PaintEventArgs e)
        {
            ClearBuffers();
            GL.Enable(EnableCap.DepthTest);

            Editor.DisplayEditor();  

            this.GLControl.SwapBuffers();
            this.GLControl.Invalidate();
        }

        private void GLControl_MouseDown(object sender, MouseEventArgs e)
        {
            
        }

        private void GLControl_MouseMove(object sender, MouseEventArgs e)
        {
            int xDelta = e.X - PrevCursorPosition.X;
            int yDelta = e.Y - PrevCursorPosition.Y;
            if (e.Button == MouseButtons.Right)
            {
                Editor.EditorCamera.RotateByMouse(xDelta, yDelta);
            }

            PrevCursorPosition.X = e.X;
            PrevCursorPosition.Y = e.Y;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.ShowDialog();
            if (dlg.CheckPathExists)
            {
                Editor.CreateActor(dlg.FileName);
                bTreeViewDirty = true;
            }
            dlg.Dispose();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.ShowDialog();
            if (dlg.CheckPathExists)
            {
                Editor.SetTexture(dlg.FileName);
            }
            dlg.Dispose();
        }

        

        private void GLControl_KeyDown(object sender, KeyEventArgs e)
        {
          
        }

        private void GLControl_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            if (e.KeyChar == 'a' || e.KeyChar == 'A')
                enabledKeys[0] = true;
            else if (e.KeyChar == 'd' || e.KeyChar == 'D')
                enabledKeys[1] = true;
            else if (e.KeyChar == 'w' || e.KeyChar == 'W')
                enabledKeys[2] = true;
            else if (e.KeyChar == 's' || e.KeyChar == 'S')
                enabledKeys[3] = true;
        }

        private void GLControl_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.A)
                enabledKeys[0] = false;
            else if (e.KeyCode == Keys.D)
                enabledKeys[1] = false;
            else if (e.KeyCode == Keys.W)
                enabledKeys[2] = false;
            else if (e.KeyCode == Keys.S)
                enabledKeys[3] = false;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (selectedComponent == null)
                selectedActor.AttachComponent(Editor.CreateCollisionComponent());
            else if (selectedActor == null)
                selectedComponent.AttachComponent(Editor.CreateCollisionComponent());
            bTreeViewDirty = true;
        }

    }
}
