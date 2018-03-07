using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using VMath;

namespace PhysicsBox
{
    public sealed class CollisionDetector
    {
        public event EventHandler WallCollision;
        public event EventHandler BoxCollision;
        public event EventHandler NoCollision;

        private List<CollisionSphereBox> _boxList;
        private List<Vector3[]> _polygonList;
        private List<CollisionSphereBox> _wallAreasList;

        #region Getters 

        // box list

        public void addCollisionBox(CollisionSphereBox box)
        {
            _boxList.Add(box);
        }

        public void addCollisionBoxRange(IEnumerable<CollisionSphereBox> boxRange)
        {
            _boxList.AddRange(boxRange);
        }

        public bool removeLastBox()
        {
            return _boxList.Remove(_boxList.Last());
        }

        public void removeBoxAt(int index)
        {
            _boxList.RemoveAt(index);
        }

        // polygon array list

        public void addPolygonArray(Vector3[] vArray)
        {
            _polygonList.Add(vArray);
            detectWallAreas(vArray);
        }

        public void addPolygonArrayRange(IEnumerable<Vector3[]> vArrayRange)
        {
            _polygonList.AddRange(vArrayRange);
            // add multiple wall areas detection
        }

        public bool removeLastPolygonArray()
        {
            return _polygonList.Remove(_polygonList.Last());
        }

        public void removePolygonArrayAt(int index)
        {
            _polygonList.RemoveAt(index);
        }

        #endregion

        public CollisionDetector()
        {
            _boxList = new List<CollisionSphereBox>();
            _polygonList = new List<Vector3[]>();
            _wallAreasList = new List<CollisionSphereBox>();
        }

      
        private void detectWallAreas(Vector3[] vArray)
        {
            int id = this._polygonList.Count - 1;

            Vector3[] coordinates = this.findWallBoxCoordinates(vArray);   //Находим крайние координаты для каждого набора полигонов
            _wallAreasList.Add(new CollisionSphereBox(coordinates[0].X - 1.0f, coordinates[1].X + 1.0f,
                coordinates[0].Y - 1.0f, coordinates[1].Y + 1.0f,
                coordinates[0].Z - 1.0f, coordinates[1].Z + 1.0f, id));  //Добавляем бокс в список
        }

        public void renderWallBoxes(Matrix4 modelViewMatrix, ref Matrix4 projectionMatrix)
        {
            foreach (CollisionSphereBox box in _wallAreasList)
            {
                box.renderBox(modelViewMatrix, ref projectionMatrix);
            }
        }

        private Vector3[] findWallBoxCoordinates(Vector3[] Vertices)
        {
            Vector3 LBN = new Vector3(Vertices[0][0], Vertices[0][1], Vertices[0][2]);
            Vector3 RTF = new Vector3(Vertices[0][0], Vertices[0][1], Vertices[0][2]);
            foreach (Vector3 vertex in Vertices)
            {
                if (LBN[0] > vertex[0]) LBN[0] = vertex[0];
                if (LBN[1] > vertex[1]) LBN[1] = vertex[1];
                if (LBN[2] > vertex[2]) LBN[2] = vertex[2];
                if (RTF[0] < vertex[0]) RTF[0] = vertex[0];
                if (RTF[1] < vertex[1]) RTF[1] = vertex[1];
                if (RTF[2] < vertex[2]) RTF[2] = vertex[2];
            }
            return new Vector3[2] { LBN, RTF };
        }
        private bool detectWalls(CollisionSphereBox box, Vector3[] polyVertices)
        {
            bool tempDetect = false;
            for (int i = 0; i < polyVertices.Length; i += 3)
            {
                if (GeometricMath.SpherePolygonCollision(new Vector3[] { polyVertices[i], polyVertices[i + 1], polyVertices[i + 2] }, box.getCenter(), 3, box.Radius))
                {
                    tempDetect = true;
                }
            }
            return tempDetect;
        }

        public bool isCollision(CollisionSphereBox invokeBox)
        {
            foreach (CollisionSphereBox box in _boxList) //Перебираем весь список объектов по очереди
            {
                // skip detection of equal boxies
                if (invokeBox.Equals(box)) continue;

                // simple box collision detection
                if (GeometricMath.isBoxCollision(invokeBox, box))
                {
                    this.BoxCollision(invokeBox, new EventArgs());
                    return true;
                }
                
                // wall collision detection
                foreach (CollisionSphereBox wallAreaBox in _wallAreasList)   //Перебираем все боксы областей со стенами
                {
                    if (GeometricMath.isBoxCollision(invokeBox, wallAreaBox))   //Если объект попадает в область со стенами - начинаем проверять на наличие столкновений
                    {
                        if (detectWalls(invokeBox, _polygonList[wallAreaBox.ID]))
                        {
                            this.WallCollision(invokeBox, new EventArgs()); //Столкновение найдено
                            return true;
                        }
                    }
                }
            }

            // no collisions detected
            this.NoCollision(invokeBox, new EventArgs());
            return false;
        }
    }
}
