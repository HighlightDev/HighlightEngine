using System;
using System.Collections.Generic;
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
using PhysicsBox.ComponentCore;
using OTKWinForm.IOCore;
using System.IO;

namespace OTKWinForm
{
    public partial class OTKWinForm : Form
    {
        private EditorCore Editor;
        private Point PrevCursorPosition;
        private System.Threading.Timer timer;

        private Component selectedComponent;

        private bool[] enabledKeys;

        private bool bTreeViewDirty = false;
        private bool bTransformationPanelDirty = false;

        private float ScaleFactor = 5;

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
                selectedComponent = Editor.actor;
                bTransformationPanelDirty = true;
            }
            else
            {
                Int32 itemIndex = nodeText.IndexOf('_');
                string idStr = nodeText.Replace(nodeText.Substring(0, itemIndex + 1), "");
                UInt32 index = UInt32.Parse(idStr);
                selectedComponent = ComponentCreator.GetComponentById(index);
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
                    Editor.EditorCamera.FirstPersonMove((FirstPersonCamera.Direction)(i));
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
                        AddTreeViewNodes(0, treeView1.Nodes[0].Nodes, Editor.actor.ChildrenComponents);
                        treeView1.EndUpdate();
                        treeView1.ExpandAll();
                    }));
                    bTreeViewDirty = false;
                }
            }
            if (bTransformationPanelDirty)
            {
                Vector3 translation = new Vector3(), rotation = new Vector3(), scale = new Vector3();
                if (selectedComponent != null)
                {
                    translation = selectedComponent.ComponentTranslation;
                    rotation = selectedComponent.ComponentRotation;
                    scale = selectedComponent.ComponentScale;
                }
                this.Invoke(new Action(() =>
                {
                    this.trackBarTranslationX.Value = (Int32)(translation.X * 0.5f) + 50;
                    this.trackBarTranslationY.Value = (Int32)(translation.Y * 0.5f) + 50;
                    this.trackBarTranslationZ.Value = (Int32)(translation.Z * 0.5f) + 50;

                    this.trackBarRotationX.Value = (Int32)(rotation.X * 0.27f);
                    this.trackBarRotationY.Value = (Int32)(rotation.Y * 0.27f);
                    this.trackBarRotationZ.Value = (Int32)(rotation.Z * 0.27f);

                    this.trackBarScaleX.Value = (Int32)(scale.X * (100f / ScaleFactor));
                    this.trackBarScaleY.Value = (Int32)(scale.Y * (100f / ScaleFactor));
                    this.trackBarScaleZ.Value = (Int32)(scale.Z * (100f / ScaleFactor));
                }));

                bTransformationPanelDirty = false;
            }
        }

        private void TrackBarValueChanged(object sender, EventArgs e)
        {
            if (sender == trackBarTranslationX)
            {
                float value = (trackBarTranslationX.Value * 2) - 100.0f;
                if (selectedComponent != null)
                    selectedComponent.ComponentTranslation = new Vector3(value, selectedComponent.ComponentTranslation.Y, selectedComponent.ComponentTranslation.Z);
            }
            else if (sender == trackBarTranslationY)
            {
                float value = (trackBarTranslationY.Value * 2) - 100.0f;
                if (selectedComponent != null)
                    selectedComponent.ComponentTranslation = new Vector3(selectedComponent.ComponentTranslation.X, value, selectedComponent.ComponentTranslation.Z);
            }
            else if (sender == trackBarTranslationZ)
            {
                float value = (trackBarTranslationZ.Value * 2) - 100.0f;
                if (selectedComponent != null)
                    selectedComponent.ComponentTranslation = new Vector3(selectedComponent.ComponentTranslation.X, selectedComponent.ComponentTranslation.Y, value);
            }
            else if (sender == trackBarRotationX)
            {
                float value = trackBarRotationX.Value * 3.6f;
                if (selectedComponent != null)
                    selectedComponent.ComponentRotation = new Vector3(value, selectedComponent.ComponentRotation.Y, selectedComponent.ComponentRotation.Z);
            }
            else if (sender == trackBarRotationY)
            {
                float value = trackBarRotationY.Value * 3.6f;
                if (selectedComponent != null)
                    selectedComponent.ComponentRotation = new Vector3(selectedComponent.ComponentRotation.X, value, selectedComponent.ComponentRotation.Z);
            }
            else if (sender == trackBarRotationZ)
            {
                float value = trackBarRotationZ.Value * 3.6f;
                if (selectedComponent != null)
                    selectedComponent.ComponentRotation = new Vector3(selectedComponent.ComponentRotation.X, selectedComponent.ComponentRotation.Y, value);
            }
            else if (sender == trackBarScaleX)
            {
                float value = trackBarScaleX.Value * (0.01f * ScaleFactor);
                if (selectedComponent != null)
                    selectedComponent.ComponentScale = new Vector3(value, selectedComponent.ComponentScale.Y, selectedComponent.ComponentScale.Z);
            }
            else if (sender == trackBarScaleY)
            {
                float value = trackBarScaleY.Value * (0.01f * ScaleFactor);
                if (selectedComponent != null)
                    selectedComponent.ComponentScale = new Vector3(selectedComponent.ComponentScale.X, value, selectedComponent.ComponentScale.Z);
            }
            else if (sender == trackBarScaleZ)
            {
                float value = trackBarScaleZ.Value * (0.01f * ScaleFactor);
                if (selectedComponent != null)
                    selectedComponent.ComponentScale = new Vector3(selectedComponent.ComponentScale.X, selectedComponent.ComponentScale.Y, value);
            }
        }

        private void AddTreeViewNodes(Int32 parentNodeIndex, TreeNodeCollection parentNodeCollection, List<Component> childrenComponents)
        {
            for (var i = 0; i < childrenComponents.Count; i++, parentNodeIndex++)
            {
                parentNodeCollection.Add(childrenComponents[i].ToString());
                AddTreeViewNodes(parentNodeIndex, parentNodeCollection[i].Nodes, childrenComponents[i].ChildrenComponents);
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
            Int32 xDelta = e.X - PrevCursorPosition.X;
            Int32 yDelta = e.Y - PrevCursorPosition.Y;
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
            dlg.InitialDirectory = Environment.CurrentDirectory;
            dlg.RestoreDirectory = false;
            var result = dlg.ShowDialog();
            if (result == DialogResult.OK)
            {
                selectedComponent = Editor.CreateActor(dlg.FileName);
                bTreeViewDirty = true;
            }
            dlg.Dispose();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            var result = dlg.ShowDialog();
            if (result == DialogResult.OK)
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
            if (selectedComponent != null)
            {
                selectedComponent.AttachComponent(Editor.CreateCollisionComponent());
                bTreeViewDirty = true;
            }
        }

        private SerializedComponentsContainer SetMementoComponent(Component serializableComponent)
        {
            SerializationComponentWrapper wrapper = new SerializationComponentWrapper(serializableComponent);
            SerializedComponentsContainer container = new SerializedComponentsContainer(wrapper);
            return container;
        }

        private void Serialize(SerializedComponentsContainer wrappedComponent, string pathToDir)
        {
            ComponentSerializer serializer = new ComponentSerializer();
            serializer.SerializeComponents(wrappedComponent, pathToDir);
        }

        private SerializedComponentsContainer Deserialize(string pathToFile)
        {
            SerializedComponentsContainer deserializedComponent = null;
            ComponentSerializer serializer = new ComponentSerializer();
            deserializedComponent = serializer.DeserializeComponents(pathToFile);
            return deserializedComponent;
        }

        private List<Component> GetMemento(Component parent, SerializedComponentsContainer wrappedComponent)
        {
            List<Component> result = new List<Component>();
            foreach (var component in wrappedComponent.SerializedComponents)
            {
                if (component.Type == Component.ComponentType.SceneComponent)
                {
                    Component childComponent = Editor.CreateCollisionComponent();
                    childComponent.ParentComponent = parent;
                    CreateCollisionComponent(childComponent, component);
                    result.Add(childComponent);
                }
            }
            return result;
        }

        private void CreateCollisionComponent(Component Src, Component Dest)
        {
            Src.Bound = Dest.Bound;
            Src.ComponentTranslation = Dest.ComponentTranslation;
            Src.ComponentRotation = Dest.ComponentRotation;
            Src.ComponentScale = Dest.ComponentScale;
            Src.Type = Dest.Type;
            foreach (Component destComponent in Dest.ChildrenComponents)
            {
                Component children = null;
                if (destComponent.Type == Component.ComponentType.SceneComponent)
                {
                    children = Editor.CreateCollisionComponent();
                    children.ParentComponent = Src;
                    CreateCollisionComponent(children, destComponent);
                }
                else
                {
                    children = destComponent;
                }
                Src.ChildrenComponents.Add(children);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (Editor.actor == null)
                return;

            SaveFileDialog dlg = new SaveFileDialog();
            dlg.OverwritePrompt = false;
            var result = dlg.ShowDialog();
            if (result == DialogResult.OK)
            {
                SerializedComponentsContainer wrappedComponent = SetMementoComponent(Editor.actor);
                Serialize(wrappedComponent, dlg.FileName);
            }
            dlg.Dispose();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (Editor.actor == null)
                return;

            OpenFileDialog dlg = new OpenFileDialog();
            var result = dlg.ShowDialog();
            if (result == DialogResult.OK)
            {
                var wrappedComponents = Deserialize(dlg.FileName);
                if (wrappedComponents != null)
                {
                    ComponentCreator.ClearRoot();
                    Editor.actor.ChildrenComponents = GetMemento(Editor.actor, wrappedComponents);
                    bTreeViewDirty = true;
                }
            }
            dlg.Dispose();
        }
    }
}
