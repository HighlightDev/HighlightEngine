using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OGLMatrixMath
{
    public class Vector3f
    {
        private float _x, _y, _z;
        
        public float x
        {
            set
            {
                this._x = value;
            }
            get { return this._x; }
        }
        public float y
        {
            set
            {
                this._y = value;
            }
            get { return this._y; }
        }
        public float z
        {
            set
            {
                this._z = value;
            }
            get { return this._z; }
        }
        public float this[int index]
        {
            set
            {
                if (index == 0) { this._x = value; }
                else if (index == 1) { this._y = value; }
                else if (index == 2) { this._z = value; }
                else throw new Exception("Out of range.");
            }
            get
            {
                if (index == 0) { return this._x; }
                else if (index == 1) { return this._y; }
                else if (index == 2) { return this._z; }
                else throw new Exception("Out of range.");
            }
        }

        public static Vector3f operator -(Vector3f vec1, Vector3f vec2)
        {
            return Vector3f.vectorBinaryOperations(vec1, vec2, BinaryOperations.SUBSTRACTION);
        }
        public static Vector3f operator -(Vector3f leftVec, float value)
        {
            return new Vector3f(leftVec._x - value, leftVec._y - value, leftVec._z - value);
        }
        public static Vector3f operator +(Vector3f vec1, Vector3f vec2)
        {
            return Vector3f.vectorBinaryOperations(vec1, vec2, BinaryOperations.ADDITION);
        }
        public static Vector3f operator +(Vector3f leftVec, float value)
        {
            return new Vector3f(leftVec._x + value, leftVec._y + value, leftVec._z + value);
        }
        public static Vector3f operator *(Vector3f vec1, Vector3f vec2)
        {
            return Vector3f.vectorBinaryOperations(vec1, vec2, BinaryOperations.MULTIPLICATION);
        }
        public static Vector3f operator *(Vector3f leftVec, float value)
        {
            return new Vector3f(leftVec._x * value, leftVec._y * value, leftVec._z * value);
        }
        public static Vector3f operator *(float value, Vector3f rightVec)
        {
            return new Vector3f(rightVec._x * value, rightVec._y * value, rightVec._z * value);
        }
        public static Vector3f operator /(Vector3f vec1, float value)
        {
            return new Vector3f(vec1._x / value, vec1._y / value, vec1._z / value);
        }
        public static Vector3f operator -(Vector3f vector)
        {
            Vector3f operatorVector = new Vector3f(vector);
            operatorVector.x = -vector.x;
            operatorVector.y = -vector.y;
            operatorVector.z = -vector.z;
            return operatorVector;
        }

        public float[] toArray()
        {
            return new float[3] { _x, _y, _z };
        }

        /// <summary>
        /// Возвращает нормаль к плоскости
        /// </summary>
        /// <param name="vTriangle">Массив из трех вершин треугольников в виде вектора</param>
        /// <returns>Вектор нормали</returns>
        public static Vector3f vectorNormal(Vector3f[] vTriangle)
        {
            Vector3f vVector1 = Vector3f.vectorDirection(vTriangle[1], vTriangle[2]);
            Vector3f vVector2 = Vector3f.vectorDirection(vTriangle[1], vTriangle[0]);

            // В функцию передаются три вектора - треугольник. Мы получаем vVector1 и vVector2 - его
            // стороны. Теперь, имея 2 стороны треугольника, мы можем получить из них cross().
            // (*ЗАМЕЧАНИЕ*) Важно: первым вектором мы передаём низ треугольника, а вторым - левую
            // сторону. Если мы поменяем их местами, нормаль будет повернута в противоположную
            // сторону. В нашем случае мы приняли решение всегда работать против часовой.

            Vector3f vNormal = Vector3f.vectorCross(vVector1, vVector2);

            // Теперь, имея направление нормали, осталось сделать последнюю вещь. Сейчас её
            // длинна неизвестна, она может быть очень длинной. Мы сделаем её равной 1, это
            // называется нормализация. Чтобы сделать это, мы делим нормаль на её длину.

            vNormal = Vector3f.vectorNormalize(vNormal);

            // Теперь вернём "нормализованную нормаль" 
            // Неважно, какова длина нормали 
            // (конечно, кроме (0,0,0)), если мы её нормализуем, она всегда будет равна 1.
            return vNormal;
        }
        /// <summary>
        /// Находит вектор направления 
        /// </summary>
        /// <param name="vector1">Вектор начала</param>
        /// <param name="vector2">Вектор конца</param>
        /// <returns>Вектор направления</returns>
        public static Vector3f vectorDirection(Vector3f vector1, Vector3f vector2)
        {
            return new Vector3f(Vector3f.vectorBinaryOperations(vector1, vector2, BinaryOperations.SUBSTRACTION));
        }
        /// <summary>
        /// Математические операции с двумя векторами
        /// </summary>
        /// <param name="vector1">Первый вектор</param>
        /// <param name="vector2">Второй вектор</param>
        /// <param name="operation">Тип операции</param>
        /// <returns></returns>
        public static Vector3f vectorBinaryOperations(Vector3f vector1, Vector3f vector2, BinaryOperations operation)
        {
            Vector3f resultVector = new Vector3f();
            switch (operation)
            {
                case BinaryOperations.ADDITION:
                    {
                        resultVector.x = vector1.x + vector2.x;
                        resultVector.y = vector1.y + vector2.y;
                        resultVector.z = vector1.z + vector2.z;
                        break;
                    }
                case BinaryOperations.SUBSTRACTION:
                    {
                        resultVector.x = vector1.x - vector2.x;
                        resultVector.y = vector1.y - vector2.y;
                        resultVector.z = vector1.z - vector2.z;
                        break;
                    }
                case BinaryOperations.MULTIPLICATION:
                    {
                        resultVector.x = vector1.x * vector2.x;
                        resultVector.y = vector1.y * vector2.y;
                        resultVector.z = vector1.z * vector2.z;
                        break;
                    }
                case BinaryOperations.DIVISION:
                    {
                        resultVector.x = vector1.x / vector2.x;
                        resultVector.y = vector1.y / vector2.y;
                        resultVector.z = vector1.z / vector2.z;
                        break;
                    }
            }
            return resultVector;
        }
        /// <summary>
        /// Математические операции с одним вектором
        /// </summary>
        /// <param name="vector">Вектор</param>
        /// <param name="operation">Тип операции</param>
        /// <returns></returns>
        public static Vector3f vectorSingleOperations(Vector3f vector, UnoOperations operation)
        {
            Vector3f resultVector = new Vector3f();
            switch (operation)
            {
                case UnoOperations.ABS:
                    {
                        resultVector.x = Math.Abs(vector.x);
                        resultVector.y = Math.Abs(vector.y);
                        resultVector.z = Math.Abs(vector.z);
                        break;
                    }
                case UnoOperations.SQR:
                    {
                        resultVector.x = Convert.ToSingle(Math.Pow(vector.x, 2));
                        resultVector.y = Convert.ToSingle(Math.Pow(vector.y, 2));
                        resultVector.z = Convert.ToSingle(Math.Pow(vector.z, 2));
                        break;
                    }
                case UnoOperations.SQRT:
                    {
                        resultVector.x = Convert.ToSingle(Math.Sqrt(vector.x));
                        resultVector.y = Convert.ToSingle(Math.Sqrt(vector.y));
                        resultVector.z = Convert.ToSingle(Math.Sqrt(vector.z));
                        break;
                    }
            }
            return resultVector;
        }
        /// <summary>
        /// Функция производит "рассчет оператора точки" (Dot product)
        /// </summary>
        /// <param name="vVector1"></param>
        /// <param name="vVector2"></param>
        /// <returns></returns>
        public static float dot(Vector3f vVector1, Vector3f vVector2)
        {
            // Вот формула Dot product: V1.V2 = (V1.x * V2.x  +  V1.y * V2.y  +  V1.z * V2.z)
            // В математическом представлении она выглядит так: V1.V2 = ||V1|| ||V2|| cos(theta)
            // '.' называется DOT. || || - величина, она всегда положительна. То есть величина V1
            // умножить на величину V2 умножить на косинус угла. 
            //    (V1.x * V2.x        +        V1.y * V2.y        +        V1.z * V2.z)
            return ((vVector1.x * vVector2.x) + (vVector1.y * vVector2.y) + (vVector1.z * vVector2.z));
        }
        /// <summary>
        /// Возвращает общий угол между двумя векторами
        /// </summary>
        /// <param name="vector1">Вектор 1</param>
        /// <param name="vector2">Вектор 2</param>
        /// <returns>Angle</returns>
        public static float angleBetweenVectors(Vector3f Vector1, Vector3f Vector2)
        {
            // Помните, выше мы говорили, что Dot product возвращает косинус угла между 
            // двумя векторами? Подразумевается, что векторы нормализованы. И, если у нас нет 
            // нормализованного вектора, то просто делаем arcCos(DotProduct(A, B))
            // Нам нужно разделить dot product на величину двух умноженных друг на друга
            // векторов. Вот формула: arc cosine of (V . W / || V || * || W || )
            // ||V|| - это величина V. Это "отменит" величины dot product.
            // Но если вы уже нормализовали векторы, вы можете забыть о величинах.

            // Получаем Dot от обоих векторов
            float dotProduct = Vector3f.dot(Vector1, Vector2);

            // Получаем умножение величин обоих векторов
            float vectorsMagnitude = Vector3f.vectorMagnitude(Vector1) * Vector3f.vectorMagnitude(Vector2);


            // Получаем аркосинус от (dotProduct / vectorsMagnitude), что есть угол в градусах.
            float angle = Convert.ToSingle(Math.Acos(dotProduct / vectorsMagnitude));


            // Теперь убедимся, что угол не -1.#IND0000000, что означает "недостижим". acos() видимо
            // считает прикольным возвращать -1.#IND0000000. Если мы не сделаем этой проверки, результат
            // проверки пересечения будет иногда показывать true, когда на самом деле пересечения нет.
            // я выяснил эту фичу тяжким трудом после МНОГИХ часов и уже написанных неверных уроков ;)
            // Обычно это значение возвращается, когда dot product и величина имеют одинаковое значение.
            // Мы вернём 0 если это случается.

            if (Double.IsNaN(angle))
            {
                return 0;
            }

            // Вернем угол в градусах
            return (angle);
        }
        /////////////////////////////////////// CROSS \\\\\\\\\\\\\\\\\\\\\*
        /////
        /////   Возвращает перпендикулярный вектор от 2х переданных векторов.
        /////   2 любых пересекающихся вектора образуют плоскость, от котороый мы
        /////   и ищем перпендикуляр.
        /////
        /////////////////////////////////////// CROSS \\\\\\\\\\\\\\\\\\\\\*
        public static Vector3f vectorCross(Vector3f vector1, Vector3f vector2)
        {
            Vector3f vNormal = new Vector3f();

            // Если у нас есть 2 вектора (вектор взгляда и вертикальный вектор), 
            // у нас есть плоскость, от которой мы можем вычислить угол в 90 градусов.
            // Рассчет cross'a прост, но его сложно запомнить с первого раза. 
            // Значение X для вектора : (V1.y * V2.z) - (V1.z * V2.y)
            vNormal.x = ((vector1.y * vector2.z) - (vector1.z * vector2.y));

            // Значение Y : (V1.z * V2.x) - (V1.x * V2.z)
            vNormal.y = ((vector1.z * vector2.x) - (vector1.x * vector2.z));

            // Значение Z : (V1.x * V2.y) - (V1.y * V2.x)
            vNormal.z = ((vector1.x * vector2.y) - (vector1.y * vector2.x));

            // Нам нужно найти ось, вокруг которой вращаться. Вращение камеры
            // влево и вправо простое - вертикальная ось всегда (0,1,0). 
            // Вращение камеры вверх и вниз отличается, так как оно происходит вне 
            // глобальных осей. 
            return vNormal;
        }
        /////////////////////////////////////// MAGNITUDE \\\\\\\\\\\\\\\\\\\\\*
        /////
        /////               Возвращает величину вектора
        /////
        /////////////////////////////////////// MAGNITUDE \\\\\\\\\\\\\\\\\\\\\*
        public static float vectorMagnitude(Vector3f vector)
        {
            return Convert.ToSingle(Math.Sqrt((vector.x * vector.x) +
                    (vector.y * vector.y) +
                    (vector.z * vector.z)));
        }
        /////////////////////////////////////// NORMALIZE \\\\\\\\\\\\\\\\\\\\\*
        /////
        /////   Возвращает нормализированный вектор, длина которого==1
        /////
        /////////////////////////////////////// NORMALIZE \\\\\\\\\\\\\\\\\\\\\*
        public static Vector3f vectorNormalize(Vector3f vector)
        {
            // Вычислим величину нормали
            float magnitude = Vector3f.vectorMagnitude(vector);
            // Теперь у нас есть величина, и мы можем разделить наш вектор на его величину.
            // Это сделает длинну вектора равной единице, так с ним будет легче работать.
            vector.x = vector.x / magnitude;
            vector.y = vector.y / magnitude;
            vector.z = vector.z / magnitude;
            return vector;
        }
        public static bool vectorEpsilonCompare(Vector3f vector1, Vector3f vector2, float Epsilon)
        {
            Vector3f compareVec = Vector3f.vectorSingleOperations(vector1 - vector2, UnoOperations.ABS);
            if (compareVec.x < Epsilon && compareVec.y < Epsilon && compareVec.z < Epsilon)
            {
                return true;
            }
            return false;
        }

        public Vector3f()
        {
            x = 0.0f;
            y = 0.0f;
            z = 0.0f;
        }
        public Vector3f(float x, float y, float z)
        {
            this._x = x;
            this._y = y;
            this._z = z;
        }
        public Vector3f(Vector3f clone)
        {
            this.x = clone.x;
            this.y = clone.y;
            this.z = clone.z;
        }
        public Vector3f(Vector4f vector)
        {
            this.x = vector.x;
            this.y = vector.y;
            this.z = vector.z;
        }
        public Vector3f(Vector2f xy, float z)
        {
            this._x = xy.x;
            this._y = xy.y;
            this._z = z;
        }
        public Vector3f(float value)
        {
            this._x = this._y = this._z = value;
        }
    }
}
