using CollisionEditor.Core;
using MassiveGame.Core.ComponentCore;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using MassiveGame.Settings;
using MassiveGame.CollisionEditor.Core.SerializeAPI;

namespace MassiveGame.UI
{
    public partial class CollisionEditorWindow : Form
    {
        private EditorCore Editor;
        private Point PrevCursorPosition;
        private System.Threading.Timer timer;

        private Component selectedComponent;

        private bool[] enabledKeys;

        private bool bTreeViewDirty = false;
        private bool bTransformationPanelDirty = false;
        private bool bMultipleAxisScale = false;

        private float m_scalingScale = 5;
        private const float m_translationScale = 0.1f;
        private const float m_invertedTranslationScale = 10.0f;

        public CollisionEditorWindow()
        {
            Application.EnableVisualStyles();
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

        private float ChangeMinMaxBoundsAndScale(float minBound, float maxBound, float currentValue)
        {
            if (currentValue < minBound || currentValue > maxBound)
                throw new ArgumentException("Wrong bounds.");

            float delta = maxBound - minBound;
            return ((currentValue * 2) - delta);
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
                    translation = selectedComponent.ComponentTranslation * m_invertedTranslationScale;
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

                    this.trackBarScaleX.Value = (Int32)(scale.X * (100f / m_scalingScale));
                    this.trackBarScaleY.Value = (Int32)(scale.Y * (100f / m_scalingScale));
                    this.trackBarScaleZ.Value = (Int32)(scale.Z * (100f / m_scalingScale));
                }));

                bTransformationPanelDirty = false;
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData) // arrow keys event
        {
            switch (keyData)
            {
                case Keys.Shift: break;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void TrackBarValueChanged(object sender, EventArgs e)
        {
            if (sender == trackBarTranslationX)
            {
                float value = trackBarTranslationX.Value;
                value = ChangeMinMaxBoundsAndScale(0, 100, value) * m_translationScale;
                if (selectedComponent != null)
                {
                    selectedComponent.ComponentTranslation = new Vector3(value, selectedComponent.ComponentTranslation.Y, selectedComponent.ComponentTranslation.Z);
                    Editor.bComponentHierarchyIsDirty = true;
                }
            }
            else if (sender == trackBarTranslationY)
            {
                float value = trackBarTranslationY.Value;
                value = ChangeMinMaxBoundsAndScale(0, 100, value) * m_translationScale;
                if (selectedComponent != null)
                {
                    selectedComponent.ComponentTranslation = new Vector3(selectedComponent.ComponentTranslation.X, value, selectedComponent.ComponentTranslation.Z);
                    Editor.bComponentHierarchyIsDirty = true;
                }
            }
            else if (sender == trackBarTranslationZ)
            {
                float value = trackBarTranslationZ.Value;
                value = ChangeMinMaxBoundsAndScale(0, 100, value) * m_translationScale;
                if (selectedComponent != null)
                {
                    selectedComponent.ComponentTranslation = new Vector3(selectedComponent.ComponentTranslation.X, selectedComponent.ComponentTranslation.Y, value);
                    Editor.bComponentHierarchyIsDirty = true;
                }
            }
            else if (sender == trackBarRotationX)
            {
                float value = trackBarRotationX.Value * 3.6f;
                if (selectedComponent != null)
                {
                    selectedComponent.ComponentRotation = new Vector3(value, selectedComponent.ComponentRotation.Y, selectedComponent.ComponentRotation.Z);
                    Editor.bComponentHierarchyIsDirty = true;
                }
            }
            else if (sender == trackBarRotationY)
            {
                float value = trackBarRotationY.Value * 3.6f;
                if (selectedComponent != null)
                {
                    selectedComponent.ComponentRotation = new Vector3(selectedComponent.ComponentRotation.X, value, selectedComponent.ComponentRotation.Z);
                    Editor.bComponentHierarchyIsDirty = true;
                }
            }
            else if (sender == trackBarRotationZ)
            {
                float value = trackBarRotationZ.Value * 3.6f;
                if (selectedComponent != null)
                {
                    selectedComponent.ComponentRotation = new Vector3(selectedComponent.ComponentRotation.X, selectedComponent.ComponentRotation.Y, value);
                    Editor.bComponentHierarchyIsDirty = true;
                }
            }
            else if (bMultipleAxisScale && (sender == trackBarScaleX || sender == trackBarScaleY || sender == trackBarScaleZ))
            {
                float value = (sender as TrackBar).Value * (0.01f * m_scalingScale);
                if (selectedComponent != null)
                {
                    selectedComponent.ComponentScale = new Vector3(value, value, value);
                    Editor.bComponentHierarchyIsDirty = true;
                }
            }
            else if (sender == trackBarScaleX)
            {
                float value = trackBarScaleX.Value * (0.01f * m_scalingScale);
                if (selectedComponent != null)
                {
                    selectedComponent.ComponentScale = new Vector3(value, selectedComponent.ComponentScale.Y, selectedComponent.ComponentScale.Z);
                    Editor.bComponentHierarchyIsDirty = true;
                }
            }
            else if (sender == trackBarScaleY)
            {
                float value = trackBarScaleY.Value * (0.01f * m_scalingScale);
                if (selectedComponent != null)
                {
                    selectedComponent.ComponentScale = new Vector3(selectedComponent.ComponentScale.X, value, selectedComponent.ComponentScale.Z);
                    Editor.bComponentHierarchyIsDirty = true;
                }
            }
            else if (sender == trackBarScaleZ)
            {
                float value = trackBarScaleZ.Value * (0.01f * m_scalingScale);
                if (selectedComponent != null)
                {
                    selectedComponent.ComponentScale = new Vector3(selectedComponent.ComponentScale.X, selectedComponent.ComponentScale.Y, value);
                    Editor.bComponentHierarchyIsDirty = true;
                }
            }
        }

        private string GetNameOfComponent(Component component)
        {
            string result = "CollisionComponent_" + ComponentCreator.GetIdByComponent(component);
            return result;
        }

        private void AddTreeViewNodes(Int32 parentNodeIndex, TreeNodeCollection parentNodeCollection, List<Component> childrenComponents)
        {
            for (var i = 0; i < childrenComponents.Count; i++, parentNodeIndex++)
            {
                string componentName = GetNameOfComponent(childrenComponents[i]);
                parentNodeCollection.Add(componentName);
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

        private CollisionComponentsWrapper SetMementoComponent(Component serializableComponent)
        {
            CollisionComponentsWrapper wrapper = new CollisionComponentsWrapper(serializableComponent);
            return wrapper;
        }

        private void Serialize(CollisionComponentsWrapper wrappedComponent, string pathToDir)
        {
            CollisionComponentSerializer serializer = new CollisionComponentSerializer();
            serializer.Serialize(wrappedComponent, pathToDir);
        }

        private CollisionComponentsWrapper Deserialize(string pathToFile)
        {
            CollisionComponentsWrapper deserializedComponent = null;
            CollisionComponentSerializer serializer = new CollisionComponentSerializer();
            deserializedComponent = serializer.Deserialize(pathToFile) as CollisionComponentsWrapper;
            return deserializedComponent;
        }

        private List<Component> GetMemento(Component parent, CollisionComponentsWrapper wrappedComponent)
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

        private void createMesh_B_click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.InitialDirectory = ProjectFolders.ModelsPath;
                //Environment.CurrentDirectory;
            
            var result = dlg.ShowDialog();
            if (result == DialogResult.OK)
            {
                selectedComponent = Editor.CreateActor(dlg.FileName);
                bTreeViewDirty = true;
            }
            dlg.RestoreDirectory = true;
            dlg.Dispose();
        }

        private void setMeshTexture_B_click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.InitialDirectory = ProjectFolders.TextureAtlasPath;
           
            var result = dlg.ShowDialog();
            if (result == DialogResult.OK)
            {
                Editor.SetTexture(dlg.FileName);
            }
            dlg.RestoreDirectory = true;
            dlg.Dispose();
        }

        private void createCollisionComponent_B_click(object sender, EventArgs e)
        {
            if (selectedComponent != null)
            {
                selectedComponent.AttachComponent(Editor.CreateCollisionComponent());
                bTreeViewDirty = true;
                Editor.bComponentHierarchyIsDirty = true;
            }
        }

        private void serialize_B_click(object sender, EventArgs e)
        {
            if (Editor.actor == null)
                return;

            SaveFileDialog dlg = new SaveFileDialog();
            dlg.OverwritePrompt = false;
            var result = dlg.ShowDialog();
            if (result == DialogResult.OK)
            {
                CollisionComponentsWrapper wrappedComponents = SetMementoComponent(Editor.actor);
                Serialize(wrappedComponents, dlg.FileName);
            }
            dlg.Dispose();
        }

        private void deserialize_B_click(object sender, EventArgs e)
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

        private void check_multipleAxistScale_CheckedChanged(object sender, EventArgs e)
        {
            bMultipleAxisScale = (sender as CheckBox).Checked;
        }
    }
}
