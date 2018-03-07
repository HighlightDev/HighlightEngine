using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace CParser.ASE_Parser
{
    // класс, выполняющий загрузку 3D модели    
    public class ASE_ModelLoader : ModelLoaderAbstract
    {
        #region Constructors
        public ASE_ModelLoader()
        {
            gFileName = "";
            _countLimbs = 0;
            _countReadedCharsInStrFromFile = 0;
            _limbs = null;
            Verts = null;
            N_Verts = null;
            T_Verts = null;
        }

        public ASE_ModelLoader(bool useConvertionToVBO, string modelFilePath)
            : this()
        {
            LoadModel(modelFilePath);
            if (useConvertionToVBO)
                ConvertModelData();
        }
        #endregion


        public string gFileName;        // имя файла
        private int _countLimbs;        // счетчик под-объектов
        private int _countReadedCharsInStrFromFile;   // будет указывать на количество прочитанных символов в строке при чтении информации из файла
        private ASE_Limb[] _limbs;          // массив под-объектов

        public float[,] Verts { get; private set; }
        public float[,] T_Verts { get; private set; }
        public float[,] N_Verts { get; private set; }


        /// <summary>
        /// Загрузчик файлов ASE. Загружает все вершины, нормали и текстурные координаты в Limbs, где каждый limb - 1 под-объект.
        /// </summary>
        /// <param name="modelFilePath">Путь к файлу модели.</param>
        public override int LoadModel(string modelFilePath)
        {
            gFileName = modelFilePath;
            objectCounter();
            _limbs = new ASE_Limb[_countLimbs];

            StreamReader sr_modelFile = File.OpenText(modelFilePath);

            // временные буферы
            string readerBuff = "";
            string asteriskCheck = "";
            string buff = "";
            // счетчики
            int limb_ = -1;
            int faceCounter = 0;
            int tFaceCounter = 0;
            // количество вершин и полигонов
            int vertex = 0, face = 0;

            while ((readerBuff = sr_modelFile.ReadLine()) != null)
            {
                asteriskCheck = GetFirstWord(readerBuff, 0);

                #region Parsing of file
                if (asteriskCheck[0] == '*')       // определеям, является ли первый символ звездочкой
                {
                    switch (asteriskCheck)         // если да, то проверяем какое управляющее слово содержится в первом прочитаном слове
                    {
                        case "*MATERIAL_COUNT": continue;
                        case "*MATERIAL_REF": continue;
                        case "*MATERIAL": continue;
                        case "*GEOMOBJECT":
                        {
                            limb_++; // увеличиваем счетчик под-объектов
                            continue;
                        }
                        // количество вершин в под-объекте
                        case "*MESH_NUMVERTEX":
                        {
                            buff = GetFirstWord(readerBuff, _countReadedCharsInStrFromFile);
                            vertex = Convert.ToInt32(buff);
                            continue;
                        }
                        case "*BITMAP": continue;
                        // количество текстурных координат
                        case "*MESH_NUMTVERTEX":
                        {
                            buff = GetFirstWord(readerBuff, _countReadedCharsInStrFromFile);
                            if (_limbs[limb_] != null)
                            {
                                _limbs[limb_].CreateTextureVertexMemory(Convert.ToInt32(buff));
                            }
                            continue;
                        }
                        // кол-во текстурных координат (faces)
                        case "*MESH_NUMTVFACES":
                        {
                            buff = GetFirstWord(readerBuff, _countReadedCharsInStrFromFile);
                            if (_limbs[limb_] != null)
                            {
                                _limbs[limb_].CreateTextureFaceMemory(Convert.ToInt32(buff));
                            }
                            continue;
                        }
                        // количество полигонов в под-объекте
                        case "*MESH_NUMFACES":
                        {
                            buff = GetFirstWord(readerBuff, _countReadedCharsInStrFromFile);
                            face = Convert.ToInt32(buff);

                            // если было объвляющее слово *GEOMOBJECT и были указаны количство вершин
                            if (limb_ > -1 && vertex > -1 && face > -1)
                            {
                                _limbs[limb_] = new ASE_Limb(vertex, face);
                                faceCounter = 0;
                                tFaceCounter = 0;
                            }
                            else
                            {
                                return -1;
                            }
                            continue;
                        }
                        // информация о вершине
                        case "*MESH_VERTEX":
                        {
                            // под-объект создан в памяти
                            if (limb_ == -1)
                                return -2;
                            if (_limbs[limb_] == null)
                                return -3;

                            string a1 = "", a2 = "", a3 = "", a4 = "";

                            // полчучаем информацию о кооринатах и номере вершины
                            // (получаем все слова в строке)
                            a1 = GetFirstWord(readerBuff, _countReadedCharsInStrFromFile);
                            a2 = GetFirstWord(readerBuff, _countReadedCharsInStrFromFile);
                            a3 = GetFirstWord(readerBuff, _countReadedCharsInStrFromFile);
                            a4 = GetFirstWord(readerBuff, _countReadedCharsInStrFromFile);

                            // преобразовываем в целое цисло
                            int NumVertex = System.Convert.ToInt32(a1);

                            // заменяем точки в представлении числа с плавающей точкой, на запятые, чтобы правильно выполнилась функция 
                            // преобразования строки в дробное число
                            a2 = a2.Replace('.', ',');
                            a3 = a3.Replace('.', ',');
                            a4 = a4.Replace('.', ',');

                            // записываем информацию о вершине
                            _limbs[limb_].vert[NumVertex, 0] = Convert.ToSingle(a2); // x
                            _limbs[limb_].vert[NumVertex, 1] = Convert.ToSingle(a3); // y
                            _limbs[limb_].vert[NumVertex, 2] = Convert.ToSingle(a4); // z

                            continue;
                        }
                        // информация о полигоне
                        case "*MESH_FACE":
                        {
                            // под-объект создан в памяти
                            if (limb_ == -1)
                                return -2;
                            if (_limbs[limb_] == null)
                                return -3;

                            // временные перменные
                            string a1 = "", a2 = "", a3 = "", a4 = "", a5 = "", a6 = "", a7 = "";

                            // получаем все слова в строке
                            a1 = GetFirstWord(readerBuff, _countReadedCharsInStrFromFile);
                            a2 = GetFirstWord(readerBuff, _countReadedCharsInStrFromFile);
                            a3 = GetFirstWord(readerBuff, _countReadedCharsInStrFromFile);
                            a4 = GetFirstWord(readerBuff, _countReadedCharsInStrFromFile);
                            a5 = GetFirstWord(readerBuff, _countReadedCharsInStrFromFile);
                            a6 = GetFirstWord(readerBuff, _countReadedCharsInStrFromFile);
                            a7 = GetFirstWord(readerBuff, _countReadedCharsInStrFromFile);

                            // получаем номер полигона из первого слова в строке, заменив последний символ ":" после номера на флаг окончания строки.
                            int NumFace = Convert.ToInt32(a1.Replace(':', '\0'));

                            // записываем номера вершин, которые нас интересуют
                            _limbs[limb_].face[faceCounter + 0] = Convert.ToInt32(a3);
                            _limbs[limb_].face[faceCounter + 1] = Convert.ToInt32(a5);
                            _limbs[limb_].face[faceCounter + 2] = Convert.ToInt32(a7);

                            faceCounter += 3;
                            continue;
                        }
                        // текстурые координаты
                        case "*MESH_TVERT":
                        {
                            // под-объект создан в памяти
                            if (limb_ == -1)
                                return -2;
                            if (_limbs[limb_] == null)
                                return -3;

                            // временные перменные
                            string a1 = "", a2 = "", a3 = "", a4 = "";

                            // получаем все слова в строке
                            a1 = GetFirstWord(readerBuff, _countReadedCharsInStrFromFile);
                            a2 = GetFirstWord(readerBuff, _countReadedCharsInStrFromFile);
                            a3 = GetFirstWord(readerBuff, _countReadedCharsInStrFromFile);
                            a4 = GetFirstWord(readerBuff, _countReadedCharsInStrFromFile);

                            // преобразуем первое слово в номер вершины
                            int NumVertex = Convert.ToInt32(a1);

                            // заменяем точки в представлении числа с плавающей точкой, на запятые, чтобы правильно выполнилась функция 
                            // преобразования строки в дробное число
                            a2 = a2.Replace('.', ',');
                            a3 = a3.Replace('.', ',');
                            a4 = a4.Replace('.', ',');

                            // записываем значение вершины
                            _limbs[limb_].t_vert[NumVertex, 0] = Convert.ToSingle(a2); // u
                            _limbs[limb_].t_vert[NumVertex, 1] = Convert.ToSingle(a3); // v

                            continue;
                        }
                        // привязка текстурных координат к полигонам
                        case "*MESH_TFACE":
                        {
                            // под-объект создан в памяти
                            if (limb_ == -1)
                                return -2;
                            if (_limbs[limb_] == null)
                                return -3;

                            // временные перменные
                            string a1 = "", a2 = "", a3 = "", a4 = "";

                            // получаем все слова в строке
                            a1 = GetFirstWord(readerBuff, _countReadedCharsInStrFromFile);
                            a2 = GetFirstWord(readerBuff, _countReadedCharsInStrFromFile);
                            a3 = GetFirstWord(readerBuff, _countReadedCharsInStrFromFile);
                            a4 = GetFirstWord(readerBuff, _countReadedCharsInStrFromFile);

                            int NumFace = Convert.ToInt32(a1);

                            // записываем номера вершин, которые опиывают полигон
                            _limbs[limb_].t_face[tFaceCounter + 0] = Convert.ToInt32(a2);
                            _limbs[limb_].t_face[tFaceCounter + 1] = Convert.ToInt32(a3);
                            _limbs[limb_].t_face[tFaceCounter + 2] = Convert.ToInt32(a4);

                            tFaceCounter += 3;
                            continue;
                        }
                    
                    } // end of switch
                }
                #endregion
            }

            isLoad = true;      // загрузка завершена
            return 0;
        }

        #region Getter of first word and object quantity
        // функиця получения первого слова строки
        // from указывает на позицию, начиная с которой будет выполнятся чтение файла
        private string GetFirstWord(string word, int from)  
        {
            char firstChar = word[from];    // первый символ
            string resBuff = "";   // временный буффер
            int wordLength  = word.Length;   // длина слова

            if (word[from] == ' ' || word[from] == '\t') // если первый символ, с которого предстоит искать слово является пробелом или знаком табуляции
            {
                // необходимо вычислить наличие секции пробелов или знаков табуляции и откинуть их
                int ax = 0;

                for (ax = from; ax < wordLength; ax++)  // проходим до конца слова
                {
                    firstChar = word[ax];
                    if(firstChar != ' ' && firstChar != '\t') // если встречаем символ пробела или табуляции
                        break;
                    // таким образом мы откидываем все последовательности пробелов или знаков табуляции, с которых могла начинатся переданная строка
                }

                if(ax == wordLength) // если вся представленная строка является набором пробелов или знаков табуляции
                    return resBuff;
                else
                    from = ax;
            }
            int bx = 0;

            for (bx = from; bx < wordLength; bx++)  // вычисляем слово
            {
                // если встретили знак пробела или табуляции - завершаем чтение слова
                if (word[bx] == ' ' || word[bx] == '\t')
                    break;
                resBuff += word[bx];    // записываем символ в бременный буффер, постепенно получая таким образом слово
            }

            if (bx == wordLength)   // если дошли до конца строки
                bx--;               // убераем последнее значение

            _countReadedCharsInStrFromFile = bx; // позиция в данной строке, для чтения следующего слова в данной строке
            
            return resBuff; // возвращаем слово
        }

        private void objectCounter()
        {
            string currentLine = String.Empty;
            StreamReader sr_modelFile = File.OpenText(gFileName);

            while (!sr_modelFile.EndOfStream) 
            {
                currentLine = sr_modelFile.ReadLine();
                currentLine.Trim();

                if (currentLine.StartsWith("*GEOMOBJECT"))
                {
                    _countLimbs++; // увеличиваем счетчик под-объектов
                    continue;
                }
                else continue;
            }
        }
        #endregion

        #region Data converter
        protected override void ConvertModelData()
        {
            uint overallVertexQuantity = 0;

            foreach (ASE_Limb limb in _limbs)
            {
                overallVertexQuantity += Convert.ToUInt32(limb.quantityOfV_F_TV_TF[1]);
            }

            Verts = new float[overallVertexQuantity, 3];
            N_Verts = new float[overallVertexQuantity, 3];
            T_Verts = new float[overallVertexQuantity, 2];

            for (int iGlobal = 0, iLocal = 0, objectCounter = 0; (objectCounter < _limbs.Length)
                && (iLocal < _limbs[objectCounter].quantityOfV_F_TV_TF[1])
                && (iGlobal < overallVertexQuantity); ++iLocal, ++iGlobal)
            {
                Verts[iGlobal, 0] = _limbs[objectCounter].vert[_limbs[objectCounter].face[iLocal], 0]; // X pos of vertex
                Verts[iGlobal, 1] = _limbs[objectCounter].vert[_limbs[objectCounter].face[iLocal], 1]; // Y pos of vertex
                Verts[iGlobal, 2] = _limbs[objectCounter].vert[_limbs[objectCounter].face[iLocal], 2]; // Z pos of vertex

                try // Might not be some of texture coordinates
                {
                    T_Verts[iGlobal, 0] = _limbs[objectCounter].t_vert[_limbs[objectCounter].t_face[iLocal], 0]; // U pos of tex_vertex
                    T_Verts[iGlobal, 1] = _limbs[objectCounter].t_vert[_limbs[objectCounter].t_face[iLocal], 1]; // V pos of tex_vertex
                }
                catch // to avoid errors
                {
                    T_Verts[iGlobal, 0] = 0;
                    T_Verts[iGlobal, 1] = 0;
                }

                if ((iLocal + 1) == _limbs[objectCounter].quantityOfV_F_TV_TF[1])
                {
                    iLocal = -1;
                    ++objectCounter;
                }
            }

            NormalVertexArrayCreation();

            overallVertexQuantity = 0;
            CleanUp();
        }

        private void NormalVertexArrayCreation()
        {
            // временные переменные, чтобы код был более понятен 
            float x1, x2, x3, y1, y2, y3, z1, z2, z3;

            for (int iLocal = 0, iGlobal = 0, objectCounter = 0; (objectCounter < _limbs.Length)
                && (iLocal + 3 < _limbs[objectCounter].quantityOfV_F_TV_TF[1])
                && (iGlobal < N_Verts.Length / 3); ++iLocal, ++iGlobal)
            {
                // вытаскиваем координаты треугольника (полигона) 
                x1 = _limbs[objectCounter].vert[_limbs[objectCounter].face[iLocal + 0], 0];
                x2 = _limbs[objectCounter].vert[_limbs[objectCounter].face[iLocal + 1], 0];
                x3 = _limbs[objectCounter].vert[_limbs[objectCounter].face[iLocal + 2], 0];
                y1 = _limbs[objectCounter].vert[_limbs[objectCounter].face[iLocal + 0], 1];
                y2 = _limbs[objectCounter].vert[_limbs[objectCounter].face[iLocal + 1], 1];
                y3 = _limbs[objectCounter].vert[_limbs[objectCounter].face[iLocal + 2], 1];
                z1 = _limbs[objectCounter].vert[_limbs[objectCounter].face[iLocal + 0], 2];
                z2 = _limbs[objectCounter].vert[_limbs[objectCounter].face[iLocal + 1], 2];
                z3 = _limbs[objectCounter].vert[_limbs[objectCounter].face[iLocal + 2], 2];

                // рассчитываем нормаль 
                N_Verts[iGlobal, 0] = (y2 - y1) * (z3 - z1) - (y3 - y1) * (z2 - z1);
                N_Verts[iGlobal, 1] = (z2 - z1) * (x3 - x1) - (z3 - z1) * (x2 - x1);
                N_Verts[iGlobal, 2] = (x2 - x1) * (y3 - y1) - (x3 - x1) * (y2 - y1);

                if ((iLocal + 1) == _limbs[objectCounter].quantityOfV_F_TV_TF[1])
                {
                    iLocal = 0;
                    ++objectCounter;
                }
            }
            
        }
        #endregion

        protected override void CleanUp()
        {
            base.isLoad = false;
            gFileName = "";
            _countLimbs = 0;
            _countReadedCharsInStrFromFile = 0;
            _limbs = null;
        }

    }
}
