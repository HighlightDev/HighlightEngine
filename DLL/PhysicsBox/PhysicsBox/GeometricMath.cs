using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using VMath;
using PhysicsBox.MathTypes;

namespace PhysicsBox
{
    public static class GeometricMath
    {
        const float FLT_EPSILON = 0.00005f;

        public static float CMP(float x, float y)
        {
            float substr = Math.Abs(x - y);
            return substr <= FLT_EPSILON ? Math.Max(1.0f, Math.Max(Math.Abs(x), Math.Abs(y))) : 0.0f;
        }  
    
        public struct Interval
        {
            public float min;
            public float max;
        }

        public static Interval GetInterval(AABB rect, Vector3 axis)
        {
            Interval result;
            Vector3 min = rect.GetMin();
            Vector3 max = rect.GetMax();

            Vector3[] vertex =
                {
                new Vector3(min.X, max.Y, max.Z),
                new Vector3(min.X, max.Y, min.Z),
                new Vector3(min.X, min.Y, max.Z),
                new Vector3(min.X, min.Y, min.Z),
                new Vector3(max.X, max.Y, max.Z),
                new Vector3(max.X, max.Y, min.Z),
                new Vector3(max.X, min.Y, max.Z),
                new Vector3(max.X, min.Y, min.Z)
                };

            result.min = result.max = ProjectVectorOnNormalizedVector(vertex[0] , axis);

            for (Int32 i = 1; i < vertex.Length; i++)
            {
                float projection = ProjectVectorOnNormalizedVector(vertex[i], axis);
                result.min = projection < result.min ? projection : result.min;
                result.max = projection > result.max ? projection : result.max;
            }

            return result;
        }

        public static Interval GetInterval(OBB obb, Vector3 axis)
        {
            Interval result;
            Vector3[] vertices = obb.GetWorldSpaceVertices();

            result.min = result.max = ProjectVectorOnNormalizedVector(vertices[0], axis);
            for (Int32 i = 1; i < vertices.Length; i++)
            {
                float projection = ProjectVectorOnNormalizedVector(vertices[i], axis);
                result.min = projection < result.min ? projection : result.min;
                result.max = projection > result.max ? projection : result.max;
            }

            return result;
        }

        public static bool OverlapOnAxis(AABB aabb, OBB obb, Vector3 axis)
        {
            var intervalAABB = GetInterval(aabb, axis);
            var intervalOBB = GetInterval(obb, axis);
            return ((intervalOBB.min <= intervalAABB.max) && (intervalAABB.min <= intervalOBB.max));
        }

        public static bool OverlapOnAxis(OBB obb1, OBB obb2, Vector3 axis)
        {
            Interval obb1Interval = GetInterval(obb1, axis);
            Interval obb2Interval = GetInterval(obb2, axis);
            return ((obb1Interval.min <= obb2Interval.max) && (obb2Interval.min <= obb1Interval.max));
        }

        public static bool AABBOBB(AABB aabb, OBB obb)
        {
            Matrix3 RotationMatrix = new Matrix3(obb.TransformationMatrix);
            Vector3[] testAxes = new Vector3[15];
            testAxes[0] = aabb.GetTangetX();
            testAxes[1] = aabb.GetTangetY();
            testAxes[2] = aabb.GetTangetZ();
            testAxes[3] = obb.GetTangetX(); ;
            testAxes[4] = obb.GetTangetY();
            testAxes[5] = obb.GetTangetZ();

            for (Int32 i = 0; i < 3; i++)
            {
                testAxes[6 + i * 3 + 0] = Vector3.Cross(testAxes[0], testAxes[i]);
                testAxes[6 + i * 3 + 1] = Vector3.Cross(testAxes[1], testAxes[i]);
                testAxes[6 + i * 3 + 2] = Vector3.Cross(testAxes[2], testAxes[i]);
            }

            for (Int32 i = 0; i < testAxes.Length; i++)
            {
                if (!OverlapOnAxis(aabb, obb, testAxes[i]))
                    return false;
            }
            return true;
        }

        public static bool OBBOBB(OBB obb1, OBB obb2)
        {
            Matrix3 RotationMatrix1 = new Matrix3(obb1.TransformationMatrix);
            Matrix3 RotationMatrix2 = new Matrix3(obb2.TransformationMatrix);
            Vector3[] testAxes = new Vector3[15];
            // axes of bounding box
            testAxes[0] = RotationMatrix1.Row0.Normalized();
            testAxes[1] = RotationMatrix1.Row1.Normalized();
            testAxes[2] = RotationMatrix1.Row2.Normalized();
            testAxes[3] = RotationMatrix2.Row0.Normalized();
            testAxes[4] = RotationMatrix2.Row1.Normalized();
            testAxes[5] = RotationMatrix2.Row2.Normalized();

            for (Int32 i = 0; i < 3; i++)
            {
                testAxes[6 + i * 3 + 0] = Vector3.Cross(testAxes[i], testAxes[0]);
                testAxes[6 + i * 3 + 1] = Vector3.Cross(testAxes[i], testAxes[1]);
                testAxes[6 + i * 3 + 2] = Vector3.Cross(testAxes[i], testAxes[2]);
            }

            for (Int32 i = 0; i < testAxes.Length; i++)
            {
                if (!OverlapOnAxis(obb1, obb2, testAxes[i]))
                    return false;
            }
            return true;
        }

        public static bool AABBAABB(AABB aabb1, AABB aabb2)
        {
            Vector3 max1 = aabb1.GetMax();
            Vector3 min1 = aabb1.GetMin();
            Vector3 max2 = aabb2.GetMax();
            Vector3 min2 = aabb2.GetMin();

            return (min1.X <= max2.X && max1.X >= min2.X) &&
                    (min1.Y <= max2.Y && max1.Y >= min2.Y) &&
                    (min1.Z <= max2.Z && max1.Z >= min2.Z);
        }

        public static float Intersection_RayAABBExt(FRay ray, Vector3 max, Vector3 min)
        {
            Vector3 rayDirection = ray.Direction;

            // safety check to avoid zero division
            rayDirection.X = System.Math.Abs(rayDirection.X) < 0.00005f ? 0.00005f : rayDirection.X;
            rayDirection.Y = System.Math.Abs(rayDirection.Y) < 0.00005f ? 0.00005f : rayDirection.Y;
            rayDirection.Z = System.Math.Abs(rayDirection.Z) < 0.00005f ? 0.00005f : rayDirection.Z;

            // find time of getting to each bounding box position
            float t1 = (min.X - ray.StartPosition.X) / rayDirection.X;
            float t2 = (max.X - ray.StartPosition.X) / rayDirection.X;
            float t3 = (min.Y - ray.StartPosition.Y) / rayDirection.Y;
            float t4 = (max.Y - ray.StartPosition.Y) / rayDirection.Y;
            float t5 = (min.Z - ray.StartPosition.Z) / rayDirection.Z;
            float t6 = (max.Z - ray.StartPosition.Z) / rayDirection.Z;

            // find minimal from max values
            float tmax = System.Math.Min(
                System.Math.Min(System.Math.Max(t1, t2), System.Math.Max(t3, t4)),
                System.Math.Max(t5, t6));
            // find maximal from min values
            float tmin = System.Math.Max(
                System.Math.Max(System.Math.Min(t1, t2), System.Math.Min(t3, t4)),
                System.Math.Min(t5, t6));

            // If AABB is behind the origin of the ray or if ray doesn't intersect AABB
            if (tmax < 0 || tmin > tmax)
                return -1;

            // If origin of ray is inside the AABB
            if (tmin < 0.0f)
                return tmax;

            // Intersection point
            return tmin;
        }

        public static float Intersection_RayOBBExt(FRay ray, Vector3 RotationX, Vector3 RotationY, Vector3 RotationZ, Vector3 Origin, Vector3 Extent)
        {

            Vector3 p = Origin - ray.StartPosition;

            // Project obb tangent vectors on ray direction
            Vector3 f = new Vector3(
                ProjectVectorOnNormalizedVector(RotationX, ray.Direction),
                ProjectVectorOnNormalizedVector(RotationY, ray.Direction),
                ProjectVectorOnNormalizedVector(RotationZ, ray.Direction)
                );

            // Project tangent vectors on p
            Vector3 e = new Vector3(
                ProjectVectorOnNormalizedVector(RotationX, p),
                ProjectVectorOnNormalizedVector(RotationY, p),
                ProjectVectorOnNormalizedVector(RotationZ, p)
                );


            float[] tparameter = new float[6];
            for (Int32 i = 0; i < 3; i++)
            {
                if (CMP(f[i], 0) > 0)
                {
                    if (-e[i] - Extent[i] > 0 || -e[i] + Extent[i] < 0)
                        return -1;
                    f[i] = 0.00001f;
                }

                tparameter[i * 2] = (e[i] + Extent[i]) / f[i]; // min
                tparameter[i * 2 + 1] = (e[i] - Extent[i]) / f[i]; // max
            }

            float tmin = Math.Max(
                Math.Max(Math.Min(tparameter[0], tparameter[1]), Math.Min(tparameter[2], tparameter[3])),
                Math.Min(tparameter[4], tparameter[5]));

            float tmax = System.Math.Min(
                 System.Math.Min(System.Math.Max(tparameter[0], tparameter[1]), System.Math.Max(tparameter[2], tparameter[3])),
                 System.Math.Max(tparameter[4], tparameter[5]));

            // If OBB is behind the origin of the ray or if ray doesn't intersect OBB
            if (tmax < 0 || tmin > tmax)
                return -1;

            // If origin of ray is inside the OBB
            if (tmin < 0.0f)
                return tmax;

            // Intersection point
            return tmin;
        }

        public static float Intersection_RayAABB(FRay ray, AABB aabb)
        {
            Vector3 max = aabb.GetMax();
            Vector3 min = aabb.GetMin();

            Vector3 rayDirection = ray.Direction;

            // safety check to avoid zero division
            rayDirection.X = System.Math.Abs(rayDirection.X) < 0.00005f ? 0.00005f : rayDirection.X;
            rayDirection.Y = System.Math.Abs(rayDirection.Y) < 0.00005f ? 0.00005f : rayDirection.Y;
            rayDirection.Z = System.Math.Abs(rayDirection.Z) < 0.00005f ? 0.00005f : rayDirection.Z;

            // find time of getting to each bounding box position
            float t1 = (min.X - ray.StartPosition.X) / rayDirection.X;
            float t2 = (max.X - ray.StartPosition.X) / rayDirection.X;
            float t3 = (min.Y - ray.StartPosition.Y) / rayDirection.Y;
            float t4 = (max.Y - ray.StartPosition.Y) / rayDirection.Y;
            float t5 = (min.Z - ray.StartPosition.Z) / rayDirection.Z;
            float t6 = (max.Z - ray.StartPosition.Z) / rayDirection.Z;

            // find minimal from max values
            float tmax = System.Math.Min(
                System.Math.Min(System.Math.Max(t1, t2), System.Math.Max(t3, t4)),
                System.Math.Max(t5, t6));
            // find maximal from min values
            float tmin = System.Math.Max(
                System.Math.Max(System.Math.Min(t1, t2), System.Math.Min(t3, t4)),
                System.Math.Min(t5, t6));

            // If AABB is behind the origin of the ray or if ray doesn't intersect AABB
            if (tmax < 0 || tmin > tmax)
                return -1;
        
            // If origin of ray is inside the AABB
            if (tmin < 0.0f)
                return tmax;

            // Intersection point
            return tmin;
        }

        public static float Intersection_RayOBB(FRay ray, OBB obb)
        {
            Vector3 RotationX = obb.GetTangetX();
            Vector3 RotationY = obb.GetTangetY();
            Vector3 RotationZ = obb.GetTangetZ();

            Vector3 p = obb.GetOrigin() - ray.StartPosition;

            // Project obb tangent vectors on ray direction
            Vector3 f = new Vector3(
                ProjectVectorOnNormalizedVector(RotationX, ray.Direction),
                ProjectVectorOnNormalizedVector(RotationY, ray.Direction),
                ProjectVectorOnNormalizedVector(RotationZ, ray.Direction)
                );

            // Project tangent vectors on p
            Vector3 e = new Vector3(
                ProjectVectorOnNormalizedVector(RotationX, p),
                ProjectVectorOnNormalizedVector(RotationY, p),
                ProjectVectorOnNormalizedVector(RotationZ, p)
                );

            Vector3 extent = obb.GetExtent();

            float[] tparameter = new float[6];
            for (Int32 i = 0; i < 3; i++)
            {
                if (CMP(f[i], 0) > 0)
                {
                    if (-e[i] - extent[i] > 0 || -e[i] + extent[i] < 0)
                        return -1;
                    f[i] = 0.00001f;
                }

                tparameter[i * 2] = (e[i] + extent[i]) / f[i]; // min
                tparameter[i * 2 + 1] = (e[i] - extent[i]) / f[i]; // max
            }

            float tmin = Math.Max(
                Math.Max(Math.Min(tparameter[0], tparameter[1]), Math.Min(tparameter[2], tparameter[3])),
                Math.Min(tparameter[4], tparameter[5]));

            float tmax = System.Math.Min(
                 System.Math.Min(System.Math.Max(tparameter[0], tparameter[1]), System.Math.Max(tparameter[2], tparameter[3])),
                 System.Math.Max(tparameter[4], tparameter[5]));

            // If OBB is behind the origin of the ray or if ray doesn't intersect OBB
            if (tmax < 0 || tmin > tmax)
                return -1;

            // If origin of ray is inside the OBB
            if (tmin < 0.0f)
                return tmax;

            // Intersection point
            return tmin;
        }
        
    
        /// <summary>
        /// Возвращает расстояние плоскости от начала координат 
        /// </summary>
        /// <param name="planeNormal">Нормаль к плоскости</param>
        /// <param name="planePoint">Любая точка на плоскости</param>
        /// <returns>Расстояние от начала координат</returns>
        public static float planeDistance(Vector3 planeNormal, Vector3 planePoint)
        {
            float distance = 0.0f; // Переменная хранит дистанцию плоскости от начала координат

            // Используем уравнение плоскости для нахождения дистанции (Ax + By + Cz + D = 0).
            // Нам нужно найти D.
            // Основное: A B C - это значения X Y Z нашей нормали, а x y z - это x y z нашей точки.
            // D - дистанция от начала координат.
            distance = -((planeNormal.X * planePoint.X) + (planeNormal.Y * planePoint.Y) + (planeNormal.Z * planePoint.Z));
            return distance;    // Возвратим дистанцию
        }

        /// <summary>
        /// Возвращает истину при пересечении плоскости и линии
        /// </summary>
        /// <param name="vPolygon">Треугольник (плоскость)</param>
        /// <param name="vLine">Линия</param>
        /// <returns>Истина при пересечении, иначе - ложь</returns>
        public static bool intersectedPlane(Vector3[] vPolygon, Vector3[] vLine, Vector3 vNormal, ref float originDistance)
        {
            bool intersection = false;
            float distance1 = 0, distance2 = 0;     // Дистанция 2х точек линии

            vNormal = VectorMath.Normal(vPolygon);        // Рассчитываем нормаль плоскости

            // Найдем дистанцию плоскости от начала координат:
            originDistance = planeDistance(vNormal, vPolygon[0]);

            // Получим дистанции от первой и второй точек:
            distance1 = ((vNormal.X * vLine[0].X) +                    // Ax +
                         (vNormal.Y * vLine[0].Y) +                    // Bx +
                         (vNormal.Z * vLine[0].Z)) + originDistance;    // Cz + D

            distance2 = ((vNormal.X * vLine[1].X) +                    // Ax +
                         (vNormal.Y * vLine[1].Y) +                    // Bx +
                         (vNormal.Z * vLine[1].Z)) + originDistance;    // Cz + D


            // Проверим на пересечение
            if (distance1 * distance2 >= 0)
            {
                intersection = false;
            }
            else
            {
                intersection = true;
            }
            return intersection;
        }

        /// <summary>
        /// Возвращает точку пересечения полигона и линии (подразумевается пересечение плоскости)
        /// </summary>
        /// <param name="vNormal"></param>
        /// <param name="vLine"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        public static Vector3 intersectionPoint(Vector3 vNormal, Vector3[] vLine, float distance)
        {
            Vector3 vPoint = new Vector3(), vLineDir = new Vector3();      // Переменные для точки пересечения и направления линии
            float Numerator = 0.0f, Denominator = 0.0f, dist = 0.0f;


            // Здесь немного сложная часть. Нам нужно найти 3д точку, находящуюся на плоскости.
            // Вот шаги для реализации этого:

            // 1) Сначала нам нужно получить вектор нашей линии, затем нормализовать его, чтобы длинна была 1
            vLineDir = vLine[1] - vLine[0];      // Получим вектор линии
            vLineDir = Vector3.Normalize(vLineDir);         // Нормализуем его


            // 2) Используем формулу плоскости (дистанция = Ax + By + Cz + D) чтобы найти дистанцию от одной из
            // точек до плоскости. Делаем дистанцию отрицательной, т.к. нам нужно идти НАЗАД от нашей точки
            // к плоскости. Это действие просто возвращает назад к плоскости, чтобы найти точку пересечения.

            Numerator = -(vNormal.X * vLine[0].X +     // Используем формулу плоскости с нормалью и линией
                           vNormal.Y * vLine[0].Y +
                           vNormal.Z * vLine[0].Z + distance);

            // 3) Если мы получим Dot product между вектором нашей линии и нормалью полигона,
            // это даст нам косинус угла между 2мя (т.к. они обе нормализованы - длинна 1).
            // Затем мы разделим Numerator на это значение чтобы найти отстояние плоскости от начальной точки.

            Denominator = Vector3.Dot(vNormal, vLineDir);   // Получаем Dot pruduct между линией и нормалью

            // Так как мы используем деление, нужно уберечься от ошибки деления на ноль. Если мы получим 0, 
            // это значит, это НЕДОСТИЖИМАЯ точка, т.к. линая находится на плоскости (нормаль перпендикулярна
            // к линии - (Normal.Vector = 0)).
            // В этом случае просто вернем любую точку на линии.

            if (Denominator == 0.0)
            {     // Проверим, не делим ли мы на ноль
                return vLine[0];    // Вернем любую точку линии
            }

            // Мы делим (дистанция от тoчки до плоскости) на (dot product), чтобы получить дистанцию
            // (dist), которая нужна нам для движения от начальной точки линии. Нам нужно умножить эту дистанцию (dist)
            // на вектор линии (направление). Когда вы умножаете scalar на веkтор, вы двигаетесь вдоль
            // этого вектора. Это и есть то, что мы делаем. Мы двигаемся от нашей начальной точки, выбранной
            // на линии, НАЗАД к плоскости вдоль вектора линии. Логично было бы просто получить Numerator,
            // который является дистанцией от точки до линии, а потом просто двигаться назад вдоль вектора линии.
            // Дистанция от плоскости - имеется в виду САМАЯ КОРОТКАЯ дистанция. Что если линия почти параллельна
            // полигону, и не пересекается с ним на протяжении своей длинны? Расстояние до плоскости мало, но 
            // расстояние до точки пересечения вектора линии с плоскостью очень велико. Если мы разделим
            // дистанцию на dot product из вектора линии и нормали плоскости, то получим правильную длинну.
            dist = Numerator / Denominator;
            // Теперь, как и говорилось выше, делим дистанцию на вектор, потом добавляем точку линии.
            // Это переместит точку вдоль вектора на некую дистанцию. Это в свою очередь даст
            // нам точку пересечения.
            vPoint.X = (vLine[0].X + (vLineDir.X * dist));
            vPoint.Y = (vLine[0].Y + (vLineDir.Y * dist));
            vPoint.Z = (vLine[0].Z + (vLineDir.Z * dist));
            return vPoint;          // Вернем точку пересечения.
        }

        /// <summary>
        ///  Возвращает true если точка пересечения находится внутри полигона
        /// </summary>
        /// <param name="vIntersection"></param>
        /// <param name="Polygon"></param>
        /// <param name="verticeCount"></param>
        /// <returns></returns>
        public static bool insidePolygon(Vector3 vIntersection, Vector3[] Polygon, long verticeCount)
        {
            const float MATCH_FACTOR = 0.9f;     // Исп. для покрытия ошибки плавающей точки
            float Angle = 0.0f;             // Инициализируем угол
            Vector3 vA, vB;                // Временные векторы

            // Одно то, что линия пересекает плоскость, ещё не значит, что она пересекает полигон в 
            // этой плоскости. Эта функция проверяет точку пересечения на предмет того, находится ли
            // она внутри полигона. 
            // На самом деле мы используем замечательный метод. Он создает треугольники внутри
            // полигона от точки пересечения, проводя линии к каждой вершине полигона. Потом все углы 
            // созданных треугольников складываются. И если сумма углов равна 360, то мы внутри! 
            // Если же значение меньше 360, мы снаружи полигона. 

            for (int i = 0; i < verticeCount; i++)      // Проходим циклом по каждой вершине и складываем их углы
            {
                vA = Polygon[i] - vIntersection;    // Вычитаем точку пересечения из текущей вершины

                // Вычитаем точку пересечения из следующей вершины:
                vB = Polygon[(i + 1) % verticeCount] - vIntersection;

                // Находим угол между 2мя векторами и складываем их все
                Angle += Vector3.CalculateAngle(vA, vB);
            }

            // Теперь имея сумму углов, нам нужно проверить, равны ли они 360. Так как мы используем
            // Dot product, мы работаем в радианах, так что проверим, равны ли углы 2*PI.
            // Вы заметите, что мы используем MATH_FACTOR. Мы используем его из-за неточности в рассчетах 
            // с плавающей точкой. Обычно результат не будет ровно 2*PI, так что нужно учесть маленькую
            // погрешность. Я использовал .9999, но вы можете изменить это на ту погрешность, которая вас 
            // устроит.

            if (Angle >= (MATCH_FACTOR * (2.0 * Math.PI)))   // Если угол >= 2PI (360 градусов)
            {
                return true;                // Точка находится внутри полигона
            }
            return false;       // Иначе - снаружи
        }

        /// <summary>
        /// Возвращает истину при пересечении линии и полигона
        /// </summary>
        /// <param name="vPoly"></param>
        /// <param name="vLine"></param>
        /// <param name="verticeCount"></param>
        /// <returns></returns>
        public static bool intersectedPolygon(Vector3[] vPoly, Vector3[] vLine, int verticeCount)
        {
            Vector3 vNormal = new Vector3();
            float originDistance = 0;

            // Сначала проверяем, пересекает ли наша линия плоскость. Если нет - то не нужно
            // продолжать, полигон на плоскости она тоже не пересекает.
            // Передаем в функцию адрес vNormal и originDistance, IntersectedPlane вычислит их для нас.

            if (!intersectedPlane(vPoly, vLine, vNormal, ref originDistance))
            {
                return false;
            }
            // Теперь, имея нормаль и дистанцию, мы можем использовать их для нахождения точки
            // пересечения. Точка пересечения - это точка, находящаяся НА плоскости и НА линии.
            // Чтобы найти точку пересечения, передаем в функцию нормаль плоскости, точки линии и 
            // ближайшую дистанцию до плоскости.

            Vector3 vIntersection = intersectionPoint(vNormal, vLine, originDistance);

            // Теперь, имея точку пересечения, нужно проверить, лежит ли она внутри полигона.
            // Чтобы сделать это, передаём:
            // (точка пересечения, полигон, количество вершин полигона).

            if (insidePolygon(vIntersection, vPoly, verticeCount))
            {
                return true;            // Есть пересечение! Вернём true
            }

            // Если мы дошли досюда, пересечения нет, вернём false

            return false;
        }


        ////////////////////////////// SPHERE POLYGON COLLISION """""""""\\*
        /////
        /////	 возвращает true если сфера пересекает переданный полигон.
        /////
        ////////////////////////////// SPHERE POLYGON COLLISION """""""""\\*

        public static bool SpherePolygonCollision(Vector3[] vPolygon, Vector3 vCenter, int vertexCount, float radius)
        {
            //
            // 1) Сначала нужно проверить, пересекается ли сфера с плоскостью, на которой находится 
            //    полигон. Помните, что плоскости бесконечны, и сфера может быть хоть в пятистах
            //    единицах от полигона, если сфера пересекает его плоскость - триггер сработает.
            //    Нам нужно написать функцию, возвращающую положение сферы: либо она полностью
            //    с одной стороны плоскости, либо с другой, либо пересекает плоскость.
            //    Для этого мы создали функцию ClassifySphere(), которая возвращает BEHIND, FRONT
            //    или INTERSECTS. Если она вернёт INTERSECTS, переходим ко второму шагу, иначе - мы
            //    не пересекаем плоскость полигона.
            //    
            //  2) Второй шаг - получить точку пересечения. Это одна из хитрых частей. Мы знаем,
            //    что имея точку пересечения с плоскостью, нужно просто вызвать функцию InsidePolygon(),
            //    чтобы увидеть, находится ли эта точка внутри полигона, точно так же, как мы делали
            //    в уроке "Коллизия линии и полигона". Итак, как получить точку пересечения? Это
            //    не так просто, как кажется. Поскольку на сфере может распологатся бесконечное 
            //    число точек, могут быть миллионы точек пересечения. Мы попробуем немного другой путь.
            //    Мы знаем, что можем найти нормаль полигона, что скажет нам направление, куда 
            //    он "смотрит". ClassifyPoly() кроме всего прочего вернёт дистанцию от центра сферы до 
            //    плоскости. И если мы умножим нормаль на эту дистанцию, то получим некое смещение.
            //    Это смещение может затем быть вычтено из центра сферы. Хотите верьте, хотите нет,
            //    но теперь у нас есть точка на плоскости в направлении плоскости. Обычно эта точка 
            //    пересечения работает хорошо, но если мы пересечем ребра полигона, она не сработает.
            //    То, что мы только что сделали, называется "проекция центра сферы на плоскость". 
            //    Другой путь - "выстрелить" луч от центра сферы в направлении, противоположном
            //    нормали плоскости, тогда мы найдем точку пересечения линии (этого луча) и плоскости.
            //    Мой способ занимает 3 умножения и одно вычитание. Выбирайте сами.
            //    
            // 3) Имея нашу псевдо-точку пересечения, просто передаём её в InsidePolygon(),
            //    вместе с вершинами полигона и их числом. Функция вернёт true, если точка
            //    пересечения находится внутри полигона. Запомните, одно то, что функция
            //    вернёт false, не значит, что мы на этом остановимся! Если мы ещё не "пересеклись",
            //    переходим к шагу 4.
            //    
            // 4) Если мы дошли досюда, значит, мы нашли точку пересечения, и она находится
            //    вне периметра полигона. Как так? Легко. Подумайте, если центр сферы находится
            //    вне треугольника, но есть пересечение - остаётся ещё её радиус. Последняя
            //    проверка нуждается в нахождении точка на каждом ребре полигона, которая 
            //    ближе всего к центру сферы. У нас есть урок "ближайшая точка на линии", так что
            //    убедитесь, что вы его поняли, прежде, чем идти дальше. Если мы имеем дело
            //    с треугольником, нужно пройти три ребра и найти на них ближайшие точки к центру
            //    сферы. После этого рассчитываем дистанцию от этих точек до центра сферы. Если
            //    дистанция меньше, чем радиус, есть пересечение. Этот способ очень быстр.
            //    Вым не нужно рассчитывать всегда все три ребра, так как первая или вторая 
            //    дистанция может быть меньше радиуса, и остальные рассчеты можно будет не производить.
            //
            //    Это было вступление, *уфф!*. Надеюсь, вам ещё не хочется плакать от такого обилия
            //    теории, так как код на самом деле будет не слишком большим.


            // 1) ШАГ ОДИН - Найдем положение сферы

            // Сначала найдем нормаль полигона
            Vector3 vNormal = VectorMath.Normal(vPolygon);

            // Переменная для хранения дистанции от сферы
            float distance = 0.0f;

            // Здесь мы определяем, находится ли сфера спереди, сзади плоскости, или пересекает её.
            // Передаём центр сферы, нормаль полигона, точку на плоскости (любую вершину), радиус
            // сферы и пустой float для сохранения дистанции.
            int classification = ClassifySphere(vCenter, vNormal, vPolygon[0], radius, ref distance);

            // Если сфера пересекает плоскость полигона, нам нужно проверить, пересекает ли
            // она сам полигон.
            if (classification == 1)
            {
                // 2) ШАГ ДВА - Находим псевдо точку пересечения.

                // Теперь нужно спроецировать центр сфера на плоскость полигона, в направлении
                // его номали. Это делается умножением нормали на расстояние от центра сферы
                // до плоскости. Расстояние мы получили из ClassifySphere() только что.
                // Если вы не понимаете суть проекции, представьте её примерно так:
                // "я стартую из центра сферы и двигаюсь в направлении плоскости вдоль её нормали
                // Когда я должен остановится? Тогда, когда моя дистанция от центра сферы станет
                // равной дистанции от центра сферы до плоскости."
                Vector3 vOffset = new Vector3(vNormal.X * distance, vNormal.Y * distance, vNormal.Z * distance);


                // Получив смещение "offset", просто вычитаем его из центра сферы. "vPosition"
                // теперь точка, лежащая на плоскости полигона. Внутри ли она полигона - это
                // другой вопрос.
                Vector3 vPosition = vCenter - vOffset;

                // 3) ШАГ ТРИ - Проверим, находится ли точка пересечения внутри полигона

                //  Если точка пересечения внутри
                // полигона, ф-я вернёт true, иначе false.
                if (insidePolygon(vPosition, vPolygon, vertexCount))
                    return true;	// Есть пересечение!
                else		// Иначе
                {
                    // 4) ШАГ ЧЕТЫРЕ - Проверим, пересекает ли сфера рёбра треугольника

                    // Если мы дошли досюда, центр сферы находится вне треугольника.
                    // Если хоть одна часть сферы пересекает полигон, у нас есть пересечение.
                    // Нам нужно проверить расстояние от центра сферы до ближайшей точки на полигоне.
                    if (EdgeSphereCollision(vCenter, vPolygon, vertexCount, radius))
                    {
                        return true;	// We collided!
                    }
                }
            }

            // Если мы здесь, пересечения нет
            return false;
        }


        /////////////////////////////////// DISTANCE """"""""""""\\*
        /////
        /////	Возвращает дистанцию между двумя 3D точками
        /////
        /////////////////////////////////// DISTANCE """"""""""""\\*

        static float Distance(Vector3 vPoint1, Vector3 vPoint2)
        {

            // Distance = sqrt(  (P2.X - P1.X)^2 + (P2.Y - P1.Y)^2 + (P2.Z - P1.Z)^2 )

            float distance = Convert.ToSingle(Math.Sqrt((vPoint2.X - vPoint1.X) * (vPoint2.X - vPoint1.X) +
                                    (vPoint2.Y - vPoint1.Y) * (vPoint2.Y - vPoint1.Y) +
                                    (vPoint2.Z - vPoint1.Z) * (vPoint2.Z - vPoint1.Z)));

            // Вернём дистанцию между двумя точками
            return distance;
        }


        ////////////////////////////// CLOSET POINT ON LINE """""""""""\*
        /////
        /////	Возвращает точку на линии vA_vB, которая ближе всего к точке vPoint
        /////
        ////////////////////////////// CLOSET POINT ON LINE """""""""""\*
        static Vector3 ClosestPointOnLine(Vector3 vA, Vector3 vB, Vector3 vPoint)
        {
            // Эта функция принимает сегмент линии, от vA до vB, затем точку в пространстве,
            // vPoint. Мы хотим найти ближайшую точку отрезка vA_vB к точке в пространстве.
            // Или это будет одна из двух крайних точек линии, или точка где-то между
            // vA и vB. В отношении определения пересечений это очень важная функция.

            // Вот как это работает. Сначала это всё кажется немного запутанным, так что постарайтесь
            // сосредоточится. Сначала нам нужно найти вектор от "vA" к точке в пространстве.
            // Затем нужно нормализовать вектор от "vA" к "vB", так как нам не нужна его полная длинна,
            // только направление. Запомните это, так как позже мы будем использовать скалярное
            // произведение (dot product) при рассчетах. Итак, сейчас у нас есть 2 вектора, образующие
            // угол воображаемого треугольника на плоскости (2 точки линии и точка пространства).

            // Далее нам нужно найти величину (magnitude) сегмента линии. Это делается простой
            // формулой дистанции. Затем вычисляем dot между "vVector2" и "vVector1". Используя
            // это скалярное произведение, мы можем по существу спроэцировать vVector1 на нормализованный
            // вектор сегмента линии, "vVector2". Если результат скалярного произведения равен нулю,
            // это значит, что векторы были перпендикулярны и имели между собой угол в 90 градусов.
            // 0 - это дистанция нового спроэцированного вектора от vVector2. Если результат - 
            // отрицательный, значит угол между двумя векторами более 90 градусов, что в свою очередь
            // означает, что ближайшая точка - "vA", так как этот спроэцированный вектор находится
            // снаружи линии. Если же результат - положительное число, спроэцированный вектор будет
            // находится с правой стороны "vA", но возможно и справа от "vB". Чтобы это проверить,
            // мы убедимся, что результат скалярного произведения НЕ больше дистанции "d". Если
            // он больше, то ближайшая точка - "vB".

            // Итак, мы можем найти ближайшую точку довольно просто, если это одна из крайних точек линии.
            // Но как мы найдём точку между двумя краями линии? Это просто. Посколько у нас есть
            // дистанция "t" от точки "vA" (полученная из скалярного произведения двух векторов),
            // мы просто используем наш вектор направления сегмента линии, "vVector2", и умножим его
            // на дистанцию "t". Это создаст вектор, идущий в направлении сегмента линии, с величиной
            // (magnitude) спроецированного вектора, "vVector1", от точки "vA". Затем прибавляем
            // этот вектор к "vA", что даст нам точку на линии, которая ближе всего к нашей точке
            // пространства, "vPoint".

            // Наверно, это всё очень сложно представить на основе комментариев, пока у вас 
            // нет хорошего понимания линейной алгебры.


            // Создаём вектор от точки vA к точке пространства vPoint.
            Vector3 vVector1 = vPoint - vA;

            // Создаём нормализированный вектор направления от точки vA до vB.
            Vector3 vVector2 = Vector3.Normalize(vB - vA);

            // Используем формулу дистанции, чтобы найти величину (magnitude) сегмента линии.
            float d = Distance(vA, vB);

            // Используя скалярное произведение, проэцируем vVector1 на vVector2. 
            // Это, по существу, даст нам расстояние от нашего спроецированного вектора до vA.
            float t = Vector3.Dot(vVector2, vVector1);

            // Если наша спроецированная дистанция от vA, "t", меньше или равна нулю, ближайшая
            // точка к vPoint - vA. Возвращаем эту точку.
            if (t <= 0)
                return vA;

            // Если спроецированная дистанция от vA, "t", Больше или равна длинне сегмента линии,
            // ближайшая точка на линии - vB. Вернём её.
            if (t >= d)
                return vB;

            // Здесь мы создаём вектор с длинной t и направлением vVector2.
            Vector3 vVector3 = new Vector3(vVector2.X * t, vVector2.Y * t, vVector2.Z * t);

            // Чтобы найти ближайшую точку на отрезке линии, просто прибавляем vVector3 к точке vA.
            Vector3 vClosestPoint = vA + vVector3;

            // Вернём ближайшую точку на линии
            return vClosestPoint;
        }


        ///////////////////////////////// CLASSIFY SPHERE """"""""""\\*
        /////
        /////	Новая функция: вычисляет положение сферы относительно плоскости, а так же расстояние
        /////
        ///////////////////////////////// CLASSIFY SPHERE """"""""""\\*

        static int ClassifySphere(Vector3 vCenter,
                Vector3 vNormal, Vector3 vPoint, float radius, ref float distance)
        {
            // Сначала нужно найти расстояние плоскости от начала координат.
            // Это нужно в дальнейшем для формулы дистанции.
            float d = planeDistance(vNormal, vPoint);

            // Здесь мы используем знаменитую формулу дистанции, чтобы найти расстояние
            // центра сферы от плоскости полигона.
            // Напоминаю саму формулу: Ax + By + Cz + d = 0 with ABC = Normal, XYZ = Point
            distance = (vNormal.X * vCenter.X + vNormal.Y * vCenter.Y + vNormal.Z * vCenter.Z + d);

            // Теперь используем только что найденную информацию. Вот как работает коллизия
            // сферы и плоскости. Если расстояние от центра до плоскости меньше, чем радиус
            // сферы, мы знаем, что пересекли сферу. Берём модуль дистанции, так как если
            // сфера находится за плоскостью, дистанция получится отрицательной.

            // Если модуль дистанции меньше радиуса, сфера пересекает плоскость.
            if (Math.Abs(distance) < radius)
                return 1;

            // Если дистанция больше или равна радиусу, сфера находится перед плоскостью.
            else if (distance >= radius)
                return 2;

            // Если и не спереди, и не пересекает - то сзади
            return 0;
        }


        ///////////////////////////////// EDGE SPHERE COLLSIION """"""""""\\*
        /////
        /////	Новая ф-я: определяет, пересекает ли сфера какое-либо ребро треугольника
        /////
        ///////////////////////////////// EDGE SPHERE COLLSIION """"""""""\\*

        static bool EdgeSphereCollision(Vector3 vCenter,
                     Vector3[] vPolygon, int vertexCount, float radius)
        {
            Vector3 vPoint;

            // Эта ф-я принимает центр сферы, вершины полигона, их чичло и радиус сферы. Мы вернём
            // true, если сфера пересекается с каким-либо ребром. 

            // Проходим по всем вершинам
            for (int i = 0; i < vertexCount; i++)
            {
                // Это вернёт ближайшую к центру сферы точку текущего ребра.
                vPoint = ClosestPointOnLine(vPolygon[i], vPolygon[(i + 1) % vertexCount], vCenter);

                // Теперь нужно вычислить расстояние между ближайшей точкой и центром сферы
                float distance = Distance(vPoint, vCenter);

                // Если расстояние меньше радиуса, должно быть пересечение
                if (distance < radius)
                    return true;
            }

            // Иначе пересечения не было
            return false;
        }

        public static bool IsSphereVsSphereIntersection(FSphere sphere1, FSphere sphere2)
        {
            Vector3 SphereOriginDistance = sphere1.Origin - sphere2.Origin;
            float SquaredDistance = Vector3.Dot(SphereOriginDistance, SphereOriginDistance);
            float SquaredRadiuses = (sphere1.Radius * sphere1.Radius) + (sphere2.Radius * sphere2.Radius);
            return (SquaredDistance <= SquaredRadiuses);
        }

        public static bool IsPointInsideSphere(Vector3 Point, Vector3 SphereOrigin, float Radius)
        {
            float distance = (Point - SphereOrigin).Length;
            return (distance <= Radius);
        }

        public static bool IsRayIntersectionSphere(Vector3 RayOrigin, Vector3 NormalizedRayDirection, Vector3 SphereOrigin, float Radius)
        {
            bool bIntersects = false;
            Vector3 SphereProjection = SphereOrigin - RayOrigin;
            float projectionDirection = Vector3.Dot(NormalizedRayDirection, SphereProjection);
            if (projectionDirection > 0 && projectionDirection <= 1.0f)
            {
                Vector3 ClosestPointToSphereOrigin = RayOrigin + NormalizedRayDirection * projectionDirection;
                bIntersects = IsPointInsideSphere(ClosestPointToSphereOrigin, SphereOrigin, Radius);
            }
            return bIntersects;
        }

        public static float GetDistanceFromPlaneToPoint(FPlane plane, Vector3 point)
        {
            Vector3 planeNormal = (Vector3)plane;
            float projectPointOnNormal = Vector3.Dot(planeNormal, point);
            float distance = projectPointOnNormal - plane.D;
            distance /= planeNormal.Length;
            return distance;
        }

        public static bool IsPointIsidePlane(FPlane plane, Vector3 point, ConvexVolume.RelativePosition relativePosition)
        {
            bool bResult = false;
            float distancePointToPlane = GetDistanceFromPlaneToPoint(plane, point);
            if (relativePosition == ConvexVolume.RelativePosition.BEYOND_PLANE)
            {
                bResult = distancePointToPlane > 0.0f;
            }
            else if (relativePosition == ConvexVolume.RelativePosition.BEFORE_PLANE)
            {
                bResult = distancePointToPlane < 0.0f;
            }
            else
            {
                bResult = true;
            }
            return bResult;
        }

        public static bool IsPointInsideConvexVolume(ConvexVolume convexVolume, Vector3 point)
        {
            bool bResult = false;
            for (Int32 i = 0; i < 6; i++)
            {
                bResult = IsPointIsidePlane(convexVolume[i], point, convexVolume.relativePosition);
                if (!bResult)
                    break;
            }
            return bResult;
        }

        public static Vector3 GetIntersectionRayPlane(FPlane plane, FRay ray)
        {
            Vector3 result;
            Vector3 planeNormal = new Vector3(plane.X, plane.Y, plane.Z);
            float timeParam = (plane.D + (Vector3.Dot(planeNormal, ray.StartPosition))) / Vector3.Dot(planeNormal, ray.Direction);
            result = ray.StartPosition + ray.Direction * timeParam;
            return result;
        }

        public static float ProjectVectorOnUnnormalizedVector(Vector3 projectedV, Vector3 projectOnV)
        {
            return Vector3.Dot(projectedV, projectOnV) / projectOnV.Length;
        }

        public static float ProjectVectorOnNormalizedVector(Vector3 projectedV, Vector3 projectOnV)
        {
            return Vector3.Dot(projectedV, projectOnV);
        }

    }
}
