using Grid;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MassiveGame
{
    public class GridManager
    {
        //Translates object coordinates into grid coordinates and fills it
        /// <summary>
        /// Функция заносит в таблицу столкновений ID объектов в те клетки ,где они находятся 
        /// </summary>
        /// <param name="obj">Объект, ID которого будет заноситься в таблицу столкновений</param>
        /// <param name="table">Непосредственно сама таблица столкновений</param>
        public static void rectangleFillObjectToGrid(MotionEntity obj, TableGrid table) //polymorph function
        {
            // local variables  
            double left, right, far, near;
            left = obj.Box.LBNCoordinates.X / table.GridStep;
            right = obj.Box.RTFCoordinates.X / table.GridStep;
            far = obj.Box.RTFCoordinates.Z / table.GridStep;
            near = obj.Box.LBNCoordinates.Z / table.GridStep;
            //
            for (double i = left; i <= right; i += table.GridStep)
            {
                for (double j = near; j <= far; j += table.GridStep)
                {
                      table.Table[(int)i, (int)j] = obj.Box.ID;
                }
            }
        }
        /// <summary>
        /// Функция заполняет таблицу столкновений сложными объектами
        /// </summary>
        /// <param name="obj">Объект , фигура которого и будет заполняться в таблицу столкновений</param>
        /// <param name="table">Таблица столкновений</param>
        /// <param name="vectorCoordinates">Массив координат объекта</param>
        /// <param name="YLevel">Уровень по оси У</param>
        public static void polygonalFillObjectToGrid(MotionEntity obj, TableGrid table, double[,] vectorCoordinates,double YLevel)
        {
            double left, right, far, near;
            int counter = 0;
            for (int i = 0; i < vectorCoordinates.GetLength(0); i++)
            {
                if (vectorCoordinates[i, 1] == YLevel)
                {
                   counter++;
                }
            }
            double[,] tempVector = new double[counter, 3];          //Создаем массив координат , которые 
            counter--; // Делаем счетчик на единицу меньше , чтобы не выйти за пределы массива (i = 0;i < counter)
            for (int i = 0; i < vectorCoordinates.GetLength(0); i++)
            {
                if (vectorCoordinates[i, 1] == YLevel)      //Если на уровне по У одинаковы - записываем координату в массив
                {
                    tempVector[counter, 0] = vectorCoordinates[i, 0];
                    tempVector[counter, 1] = vectorCoordinates[i, 1];
                    tempVector[counter--, 2] = vectorCoordinates[i, 2]; //Уменьшаем индекс на 1
                }
            }

            ///////////////////////Определяем наличие диагоналей///////////////////
            ////     Обработка диагоналей и прямоугольников разная             ////
            ///////////////////////////////////////////////////////////////////////
            // local variables  
            for (int i = 0; i < tempVector.GetLength(0); i++)
            {
                if (i + 1 >= tempVector.GetLength(0)) return;
                if ((tempVector[i, 0] == tempVector[i + 1, 0]) && (tempVector[i, 2] == tempVector[i + 1, 2]))//Если у координат оси X 
                //и Z равны - одна и таже точка, идем на след итерацию
                {
                    continue;
                }
                if (((tempVector[i, 0] == tempVector[i + 1, 0]) && (tempVector[i, 2] != tempVector[i + 1, 2])) || ((tempVector[i, 0] != tempVector[i + 1, 0]) && (tempVector[i, 2] == tempVector[i + 1, 2])))
                //если X не равны и Z не равны, или наоборот - обычная прямоугольная фигура.
                {
                    if (tempVector[i, 0] == tempVector[i + 1, 0])   //Проходим по оси Z
                    {
                        if (tempVector[i, 2] > tempVector[i + 1, 2])    //Определяем где far и near
                        {
                            far = tempVector[i, 2] / table.GridStep;
                            near = tempVector[i + 1, 2] / table.GridStep;
                        }
                        else
                        {
                            far = tempVector[i + 1, 2] / table.GridStep;
                            near = tempVector[i, 2] / table.GridStep;
                        }
                        for (double k = near; k <= far; k += table.GridStep)  //Проходим все значения в интервале от 1 координаты до 2-рой
                        {
                            table.Table[(int)(tempVector[i, 0] / table.GridStep), (int)k] = obj.Box.ID; ;
                        }
                    }
                    else    //Проходим по оси X
                    {
                        if (tempVector[i, 0] > tempVector[i + 1, 0])    //Определяем где left и right
                        {
                            right = tempVector[i, 0] / table.GridStep;
                            left = tempVector[i + 1, 0] / table.GridStep;
                        }
                        else
                        {
                            right = tempVector[i + 1, 0] / table.GridStep;
                            left = tempVector[i, 0] / table.GridStep;
                        }
                        for (double k = left; k <= right; k += table.GridStep)    //Проходим все значения в интервале от 1 координаты до 2-рой
                        {
                            table.Table[(int)k, (int)(tempVector[i, 2] / table.GridStep)] = obj.Box.ID; ;
                        }
                    }
                    
                }
                else if ((tempVector[i, 0] != tempVector[i + 1, 0]) && (tempVector[i, 2] != tempVector[i + 1, 2]))
                // X не равны и Z не равны - найдена диагональ.
                {
                    //Переведем из мировых координат в табличные
                    if (tempVector[i, 0] > tempVector[i + 1, 0])    //Определяем где left и right
                    {
                        right = tempVector[i, 0] / table.GridStep;
                        left = tempVector[i + 1, 0] / table.GridStep;
                    }
                    else
                    {
                        right = tempVector[i + 1, 0] / table.GridStep;
                        left = tempVector[i, 0] / table.GridStep;
                    }
                    if (tempVector[i, 2] > tempVector[i + 1, 2])    //Определяем где far и near
                    {
                        far = tempVector[i, 2] / table.GridStep;
                        near = tempVector[i + 1, 2] / table.GridStep;
                    }
                    else
                    {
                        far = tempVector[i + 1, 2] / table.GridStep;
                        near = tempVector[i, 2] / table.GridStep;
                    }
                    //Найдем по какой оси длиннее диагональ 
                    double temp_x = Math.Abs(right - left);
                    double temp_z = Math.Abs(far - near);
                    if (temp_x > temp_z)  //Если длина полигона по оси X больше чем по оси Z 
                    {
                        if ((tempVector[i, 0] < tempVector[i + 1, 0])&&(tempVector[i, 2] < tempVector[i + 1, 2]))//Проходим снизу вверх и слева направо
                        {
                            for (double k = left; k < right; k += table.GridStep)
                            {
                                double step_z = (k - left) / temp_z;
                                table.Table[(int)k, (int)(step_z + near)] = obj.Box.ID; ;
                            }
                        }
                        else if ((tempVector[i, 0] > tempVector[i + 1, 0]) && (tempVector[i, 2] < tempVector[i + 1, 2]))//Проходим снизу вверх и справа налево
                        {
                            for (double k = right; k > left; k -= table.GridStep)
                            {
                                double step_z = (right - k) / temp_z;
                                table.Table[(int)k, (int)(step_z + near)] = obj.Box.ID; ;
                            }
                        }
                        else if ((tempVector[i, 0] > tempVector[i + 1, 0]) && (tempVector[i, 2] > tempVector[i + 1, 2]))//Проходим сверху вниз и справа налево
                        {
                            for (double k = right; k > left; k -= table.GridStep)
                            {
                                double step_z = (right - k) / temp_z;
                                table.Table[(int)k, (int)(far - step_z)] = obj.Box.ID; ;
                            }
                        }
                        else if ((tempVector[i, 0] < tempVector[i + 1, 0]) && (tempVector[i, 2] > tempVector[i + 1, 2]))//Проходим снизу вверх и справа налево
                        {
                            for (double k = left; k < right; k += table.GridStep)
                            {
                                double step_z = (k - left) / temp_z;
                                table.Table[(int)k, (int)(far - step_z)] = obj.Box.ID; ;
                            }
                        }
                    }
                    else//Соответственно по оси Z больше чем по X
                    {
                        
                    }
                }
            }
        }
        public static void rectangleFillEmpty(MotionEntity obj, TableGrid table) //polymorph function
        {
            // local variables  
            double left, right, far, near;
            left = obj.Box.LBNCoordinates.X / table.GridStep;
            right = obj.Box.RTFCoordinates.X / table.GridStep;
            far = obj.Box.RTFCoordinates.Z / table.GridStep;
            near = obj.Box.LBNCoordinates.Z / table.GridStep;
            //
            for (double i = left; i <= right; i += table.GridStep)
            {
                for (double j = near; j <= far; j += table.GridStep)
                {
                    table.Table[(int)i, (int)j] = 0;
                }
            }
        }
    }
}
