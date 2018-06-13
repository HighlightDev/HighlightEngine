using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CParser.DAE_Parser
{
    public class DAE_Geometries
    {
        #region Constructors
        private DAE_Geometries()
        {
            faceCollection = new List<Int32[,]>();
            _limbs = new List<DAE_Limb>();
            _limb = null;
            Verts = null;
            T_Verts = null;
            N_Verts = null;
        }

        /// <summary>
        /// Обработчик данных геометрии модели у файлов COLLADA (*.DAE).
        /// Загружает все вершины, нормали и текстурные координаты с возможностью их преобразования для VBO.
        /// </summary>
        /// <param name="pathToAnimation">Путь к файлу модели.</param>
        /// <param name="convertToVBO">Флаг использования конвертирования данных для VBO</param>
        /// <param name="afterConvetionToVBO">Флаг очистки исходных данных после конвертирования. Если <b>true</b> - исходные данные будут удалены.</param>
        /// <param name="additionalData">Флаг очистки дополнительных данных (не критические). Если <b>true</b> - данные будут удалены.</param>
        public DAE_Geometries(string pathToAnimation, bool convertToVBO = true,
            bool afterConvetionToVBO = true, bool additionalData = true) : this()
        {
            LoadGeometry(pathToAnimation);
            if (convertToVBO)
                ConvertDataToVBO(afterConvetionToVBO, additionalData);
        }
        #endregion


        private List<Int32[,]> faceCollection;
        private List<DAE_Limb> _limbs;
        private DAE_Limb _limb;
        public float[,] Verts { get; private set; }
        public float[,] T_Verts { get; private set; }
        public float[,] N_Verts { get; private set; }


        private Int32 LoadGeometry(string pathToAnimation)
        {
            string currentLine = String.Empty;
            Int32 startPos = 0,
                polylistCounter = 0;
            bool linesTagUsage = false;
            StreamReader sr_animFile = new StreamReader(pathToAnimation, Encoding.UTF8);

            while (!sr_animFile.EndOfStream)
            {
                currentLine = sr_animFile.ReadLine();
                currentLine = currentLine.Trim();

                if (currentLine == "<library_geometries>") continue;
                if (currentLine.StartsWith("<geometry "))
                {
                    _limb = new DAE_Limb();
                    startPos = currentLine.IndexOf("\"", currentLine.IndexOf(" id=")) + 1;
                    _limb.objectID = currentLine.Substring(startPos, currentLine.IndexOf("\"", startPos + 1) - startPos);
                    startPos = currentLine.IndexOf("\"", currentLine.IndexOf(" name=")) + 1;
                    _limb.objectName = currentLine.Substring(startPos, currentLine.IndexOf("\"", startPos + 1) - startPos);

                    continue;
                }
                if (currentLine == "<mesh>") continue;
                if (currentLine.StartsWith("<source "))
                {
                    startPos = currentLine.IndexOf("\"", currentLine.IndexOf(" id=")) + 1;
                    string dataType = currentLine.Substring(startPos, currentLine.IndexOf("\"", startPos + 1) - startPos);

                    if (dataType.StartsWith(_limb.objectID))
                    {
                        dataType = dataType.Replace(_limb.objectID, "").Remove(0, 1);
                        sourceParser(dataType, sr_animFile);
                    }

                    dataType = String.Empty;
                    continue;
                }
                if (currentLine == "</source>") continue;
                if (currentLine.StartsWith("<lines ")) // make this data compatible vith face's
                {
                    linesTagUsage = true;
                    linesParser(currentLine, sr_animFile);
                    continue; 
                }
                if (currentLine == "</lines>") continue;
                if (currentLine.StartsWith("<vertices ")) continue;
                if (currentLine == "</vertices>") continue;
                if (currentLine.StartsWith("<polylist "))
                {
                    ++polylistCounter;
                    if (polylistParser(currentLine, sr_animFile) == -1)
                        return -1;
                    continue;
                }
                if (currentLine == "</polylist>") continue;
                if (currentLine == "</mesh>") continue;
                if (currentLine == "</geometry>")
                {
                    // convert faceCollection list to _limb.face array
                    if ((ConvertFaceListToArray(polylistCounter) == -1) && linesTagUsage == false)
                        return -1;

                    if (linesTagUsage == true)
                        if (ConvertLinesToFaces(polylistCounter) == -1)
                            return -1;

                    _limbs.Add(_limb);
                    polylistCounter = 0;
                    linesTagUsage = false;
                    continue;
                }
                if (currentLine == "</library_geometries>")
                {
                    sr_animFile.Close();
                    break;
                }
            }

            currentLine = String.Empty;
            startPos = 0;
            polylistCounter = 0;
            return 0;
        }

        #region Additional parsers of tags
        private void sourceParser(string dataType, StreamReader sr_animFile)
        {
            string currentLine = String.Empty;
            float[] tempArrayFStorage = null;
            uint elementQuantity = 0;
            uint elementQuantityPerAxis = 0;

            while (!sr_animFile.EndOfStream)
            {
                currentLine = sr_animFile.ReadLine();
                currentLine = currentLine.Trim();
//-----------------------------------------------------------------------------------------------------------//
                if (currentLine.StartsWith("<float_array "))
                {
                    if (currentLine.EndsWith(" count=\"0\"/>"))
                    {
                        break;
                    }

                    Int32 startPos = currentLine.IndexOf("\"", currentLine.IndexOf(" id=")) + 1;
                    string sourceID = currentLine.Substring(startPos, currentLine.IndexOf("\"", startPos + 1) - startPos);
                    
                    startPos = currentLine.IndexOf("\"", currentLine.IndexOf(" count=")) + 1;
                    elementQuantity = Convert.ToUInt32(currentLine.Substring(startPos, currentLine.IndexOf("\"", startPos + 1) - startPos));
                    tempArrayFStorage = new float[elementQuantity];

                    while ((currentLine.IndexOf("<") != -1) || (currentLine.IndexOf(">") != -1))
                    {
                        currentLine = currentLine.Remove(currentLine.IndexOf("<"),
                            currentLine.IndexOf(">") - currentLine.IndexOf("<") + 1);
                    }

                    string[] tempArrayS = currentLine.Split(' ');

                    if (elementQuantity >= UInt32.MaxValue)
                    {
                        for (long i = 0; i < tempArrayFStorage.Length; ++i)
                        {
                            tempArrayFStorage[i] = Convert.ToSingle(tempArrayS[i].Replace('.', ','));
                        }
                    }
                    else
                    {
                        for (uint i = 0; i < tempArrayFStorage.Length; ++i)
                        {
                            tempArrayFStorage[i] = Convert.ToSingle(tempArrayS[i].Replace('.', ','));
                        }
                    }

                    tempArrayS = null;
                    startPos = 0;
                    sourceID = String.Empty;

                    continue;
                }
//-----------------------------------------------------------------------------------------------------------//
                if (currentLine.StartsWith("<accessor "))
                {
                    Int32 startPos = currentLine.IndexOf("\"", currentLine.IndexOf(" count=")) + 1;
                    elementQuantityPerAxis = Convert.ToUInt32(currentLine.Substring(startPos, currentLine.IndexOf("\"", startPos + 1) - startPos));
                    startPos = currentLine.IndexOf("\"", currentLine.IndexOf(" stride=")) + 1;
                    uint stride = Convert.ToUInt32(currentLine.Substring(startPos, currentLine.IndexOf("\"", startPos + 1) - startPos));

                    if (elementQuantityPerAxis * stride == elementQuantity)
                    {
                        switch (dataType)
                        {
                            case "positions":
                                {
                                    _limb.verts = new float[elementQuantityPerAxis, stride];

                                    if (elementQuantity >= UInt32.MaxValue)
                                    {
                                        for (long i = 0, j = 0; (j < tempArrayFStorage.Length) && (i < elementQuantityPerAxis); ++i, j += 3)
                                        {
                                            _limb.verts[i, 0] = tempArrayFStorage[j + 0];
                                            _limb.verts[i, 1] = tempArrayFStorage[j + 1];
                                            _limb.verts[i, 2] = tempArrayFStorage[j + 2];
                                        }
                                    }
                                    else
                                    {
                                        for (uint i = 0, j = 0; (j < tempArrayFStorage.Length) && (i < elementQuantityPerAxis); ++i, j += 3)
                                        {
                                            _limb.verts[i, 0] = tempArrayFStorage[j + 0];
                                            _limb.verts[i, 1] = tempArrayFStorage[j + 1];
                                            _limb.verts[i, 2] = tempArrayFStorage[j + 2];
                                        }
                                    }
                                }
                                break;
                            case "normals":
                                {
                                    _limb.n_verts = new float[elementQuantityPerAxis, stride];

                                    if (elementQuantity >= UInt32.MaxValue)
                                    {
                                        for (long i = 0, j = 0; (j < tempArrayFStorage.Length) && (i < elementQuantityPerAxis); ++i, j += 3)
                                        {
                                            _limb.n_verts[i, 0] = tempArrayFStorage[j + 0];
                                            _limb.n_verts[i, 1] = tempArrayFStorage[j + 1];
                                            _limb.n_verts[i, 2] = tempArrayFStorage[j + 2];
                                        }
                                    }
                                    else
                                    {
                                        for (uint i = 0, j = 0; (j < tempArrayFStorage.Length) && (i < elementQuantityPerAxis); ++i, j += 3)
                                        {
                                            _limb.n_verts[i, 0] = tempArrayFStorage[j + 0];
                                            _limb.n_verts[i, 1] = tempArrayFStorage[j + 1];
                                            _limb.n_verts[i, 2] = tempArrayFStorage[j + 2];
                                        }
                                    }
                                }
                                break;
                            case "map-0":
                                {
                                    _limb.t_verts = new float[elementQuantityPerAxis, stride];

                                    if (elementQuantity >= UInt32.MaxValue)
                                    {
                                        for (long i = 0, j = 0; (j < tempArrayFStorage.Length) && (i < elementQuantityPerAxis); ++i, j += 2)
                                        {
                                            _limb.t_verts[i, 0] = tempArrayFStorage[j + 0];
                                            _limb.t_verts[i, 1] = tempArrayFStorage[j + 1];
                                        }
                                    }
                                    else
                                    {
                                        for (uint i = 0, j = 0; (j < tempArrayFStorage.Length) && (i < elementQuantityPerAxis); ++i, j += 2)
                                        {
                                            _limb.t_verts[i, 0] = tempArrayFStorage[j + 0];
                                            _limb.t_verts[i, 1] = tempArrayFStorage[j + 1];
                                        }
                                    }
                                }
                                break;
                        }

                        tempArrayFStorage = null;
                        
                        continue;
                    }

                    startPos = 0;
                    stride = 0;
                }
//-----------------------------------------------------------------------------------------------------------//
                if (currentLine.StartsWith("<param ")) continue;
                if (currentLine == "<technique_common>") continue;
                if (currentLine == "</accessor>") continue;
                if (currentLine == "</technique_common>") break;
            }

            currentLine = String.Empty;
            tempArrayFStorage = null;
            elementQuantity = 0;
            elementQuantityPerAxis = 0;
        }

        private void linesParser(string currentLine, StreamReader sr_animFile)
        {
            Int32 startPos = currentLine.IndexOf("\"", currentLine.IndexOf(" count=")) + 1;
            Int32 countLines = Convert.ToInt32(currentLine.Substring(startPos, currentLine.IndexOf("\"", startPos + 1) - startPos));
            _limb.lines = new Int32[countLines, 2];

            while (true)
            {
                currentLine = sr_animFile.ReadLine();

                if (currentLine.IndexOf("<p>") != -1)
                {
                    currentLine = currentLine.Trim();
                    currentLine = currentLine.Replace("<p>", "").Replace("</p>", "");
                    string[] tempStr = currentLine.Split(' ');

                    for (uint i = 0, j = 0; i < countLines; ++i, j += 2)
                    {
                        _limb.lines[i, 0] = Convert.ToInt32(tempStr[j + 0]);
                        _limb.lines[i, 1] = Convert.ToInt32(tempStr[j + 1]);
                    }

                    tempStr = null;
                    break;
                }
            }

            startPos = 0;
            countLines = 0;
        }

        private Int32 polylistParser(string currentLine, StreamReader sr_animFile)
        {
            Int32[,] faceLocal;
            Int32 startPos = currentLine.IndexOf("\"", currentLine.IndexOf(" count=")) + 1;
            Int32 countVertices = Convert.ToInt32(currentLine.Substring(startPos, currentLine.IndexOf("\"", startPos + 1) - startPos));
            string semantic = String.Empty;
            Int32 vertexOffset = -1,
                normalOffset = -1,
                texcoordOffset = -1;

            currentLine = sr_animFile.ReadLine();
            currentLine = currentLine.Trim();
//-----------------------------------------------------------------------------------------------------------//
            while (currentLine.StartsWith("<input "))
            {
                startPos = currentLine.IndexOf("\"", currentLine.IndexOf(" semantic=")) + 1;
                semantic = currentLine.Substring(startPos, currentLine.IndexOf("\"", startPos + 1) - startPos);

                switch (semantic)
                {
                    case "VERTEX":
                        startPos = currentLine.IndexOf("\"", currentLine.IndexOf(" offset=")) + 1;
                        vertexOffset = Convert.ToInt32(currentLine.Substring(startPos, currentLine.IndexOf("\"", startPos + 1) - startPos));
                        break;
                    case "NORMAL":
                        startPos = currentLine.IndexOf("\"", currentLine.IndexOf(" offset=")) + 1;
                        normalOffset = Convert.ToInt32(currentLine.Substring(startPos, currentLine.IndexOf("\"", startPos + 1) - startPos));
                        break;
                    case "TEXCOORD":
                        startPos = currentLine.IndexOf("\"", currentLine.IndexOf(" offset=")) + 1;
                        texcoordOffset = Convert.ToInt32(currentLine.Substring(startPos, currentLine.IndexOf("\"", startPos + 1) - startPos));
                        break;
                }

                currentLine = sr_animFile.ReadLine();
                currentLine = currentLine.Trim();
            }
//-----------------------------------------------------------------------------------------------------------//
            while (true)
            {
                currentLine = sr_animFile.ReadLine().Trim();

                if (currentLine.StartsWith("<vcount>"))
                {
                    string[] tempStr = currentLine.Replace("<vcount>", "").Replace("</vcount>", "").Trim().Split(' ');

                    for (uint i = 0; i < countVertices; ++i)
                        if (Convert.ToInt16(tempStr[i]) != 3)
                            return -1;

                    tempStr = null;
                    continue;
                }
                if (currentLine.StartsWith("<p>"))
                {
                    faceLocal = new Int32[countVertices * 3, 3];
                    string[] tempStr = currentLine.Replace("<p>", "").Replace("</p>", "").Trim().Split(' ');

                    if ((vertexOffset == -1) && (normalOffset == -1) && (texcoordOffset == -1))
                        return -1;
                    else if ((vertexOffset == 0) && (normalOffset == -1) && (texcoordOffset == -1))
                    {
                        for (uint i = 0, j = 0; (i < countVertices * 3) && (j < tempStr.Length); ++i)
                        {
                            faceLocal[i, 0] = Convert.ToInt32(tempStr[j + 0]);
                            faceLocal[i, 1] = 0;
                            faceLocal[i, 2] = 0;

                            j += 1;
                        }
                    }
                    else if ((vertexOffset == 0) && (normalOffset == 1) && (texcoordOffset == -1))
                    {
                        for (uint i = 0, j = 0; (i < countVertices * 3) && (j < tempStr.Length); ++i)
                        {
                            faceLocal[i, 0] = Convert.ToInt32(tempStr[j + 0]);
                            faceLocal[i, 1] = Convert.ToInt32(tempStr[j + 1]);
                            faceLocal[i, 2] = 0;

                            j += 2;
                        }
                    }
                    else if ((vertexOffset == 0) && (normalOffset == 1) && (texcoordOffset == 2))
                    {
                        for (uint i = 0, j = 0; (i < countVertices * 3) && (j < tempStr.Length); ++i)
                        {
                            faceLocal[i, 0] = Convert.ToInt32(tempStr[j + 0]);
                            faceLocal[i, 1] = Convert.ToInt32(tempStr[j + 1]);
                            faceLocal[i, 2] = Convert.ToInt32(tempStr[j + 2]);

                            j += 3;
                        }
                    }

                    tempStr = null;
                    break;
                }
            }

            faceCollection.Add(faceLocal);

            faceLocal = null;
            startPos = 0;
            countVertices = 0;
            vertexOffset = -1;
            normalOffset = -1;
            texcoordOffset = -1;
            semantic = String.Empty;

            return 0;
        }
        #endregion

        #region Data converters
        private Int32 ConvertLinesToFaces(Int32 polylistCounter)
        {
            Int32 faceLengthAdditional = (_limb.lines.Length / 2) * 3;

            if (polylistCounter == 0)
            {
                _limb.face = new Int32[faceLengthAdditional, 3];

                for (Int32 i = 0, j = 0; (i < faceLengthAdditional); i += 3, ++j)
                {
                    _limb.face[i + 0, 0] = _limb.lines[j + 0, 0];
                    _limb.face[i + 0, 1] = 0;
                    _limb.face[i + 0, 2] = 0;

                    _limb.face[i + 1, 0] = _limb.lines[j + 0, 1];
                    _limb.face[i + 1, 1] = 0;
                    _limb.face[i + 1, 2] = 0;

                    _limb.face[i + 2, 0] = _limb.lines[(j + 1 < _limb.lines.Length / 2) ? j + 1 : 0, 0];
                    _limb.face[i + 2, 1] = 0;
                    _limb.face[i + 2, 2] = 0;
                }
                
                _limb.lines = null;
                return 0;
            }
            else if (polylistCounter > 0)
            {
                Int32 faceLengthOverall = faceLengthAdditional + _limb.face.Length / 3;
                Int32[,] tempFaceVault = new Int32[faceLengthOverall, 3];

                for (Int32 i = 0; i < _limb.face.Length / 3; ++i)
                {
                    tempFaceVault[i, 0] = _limb.face[i, 0];
                    tempFaceVault[i, 1] = _limb.face[i, 1];
                    tempFaceVault[i, 2] = _limb.face[i, 2];
                }

                for (Int32 i = _limb.face.Length / 3, j = 0; i < faceLengthOverall; i += 3, ++j)
                {
                    tempFaceVault[i + 0, 0] = _limb.lines[j, 0];
                    tempFaceVault[i + 0, 1] = 0;
                    tempFaceVault[i + 0, 2] = 0;

                    tempFaceVault[i + 1, 0] = _limb.lines[j, 1];
                    tempFaceVault[i + 1, 1] = 0;
                    tempFaceVault[i + 1, 2] = 0;

                    tempFaceVault[i + 2, 0] = _limb.lines[j, 1];
                    tempFaceVault[i + 2, 1] = 0;
                    tempFaceVault[i + 2, 2] = 0;
                }

                _limb.face = tempFaceVault;

                tempFaceVault = null;
                _limb.lines = null;
                return 0;
            }
            else
                return -1;
        }

        private Int32 ConvertFaceListToArray(Int32 polylistCounter)
        {
            if (polylistCounter > 1)
            {
                uint overallFaceQuantity = 0;

                foreach (Int32[,] faceCollectionItem in faceCollection)
                {
                    overallFaceQuantity += Convert.ToUInt32(faceCollectionItem.Length / 3);
                }

                _limb.face = new Int32[overallFaceQuantity, 3];

                Int32[,] tempFace = null;
                Int32 objectCounter = 0;
                if (overallFaceQuantity <= Int32.MaxValue)
                {
                    for (Int32 iGlobal = 0, iLocal = 0; (objectCounter < faceCollection.Count)
                            && (iLocal < _limb.face.Length / 3)
                            && (iGlobal < overallFaceQuantity); ++iLocal, ++iGlobal)
                    {
                        tempFace = faceCollection[objectCounter];

                        _limb.face[iGlobal, 0] = tempFace[iLocal, 0];
                        _limb.face[iGlobal, 1] = tempFace[iLocal, 1];
                        _limb.face[iGlobal, 2] = tempFace[iLocal, 2];

                        if ((iLocal + 1) == faceCollection[objectCounter].Length / 3)
                        {
                            iLocal = 0;
                            ++objectCounter;
                        }
                    }
                }
                else
                {
                    for (long iGlobal = 0, iLocal = 0; (objectCounter < faceCollection.Count)
                            && (iLocal < _limb.face.Length / 3)
                            && (iGlobal < overallFaceQuantity); ++iLocal, ++iGlobal)
                    {
                        tempFace = faceCollection[objectCounter];

                        _limb.face[iGlobal, 0] = tempFace[iLocal, 0];
                        _limb.face[iGlobal, 1] = tempFace[iLocal, 1];
                        _limb.face[iGlobal, 2] = tempFace[iLocal, 2];

                        if ((iLocal + 1) == faceCollection[objectCounter].Length / 3)
                        {
                            iLocal = 0;
                            ++objectCounter;
                        }
                    }
                }

                overallFaceQuantity = 0;
                objectCounter = 0;
                tempFace = null;

                return 0;
            }
            else if (polylistCounter == 1)
            {
                _limb.face = faceCollection[faceCollection.Count - 1];
                return 0;
            }
            else 
                return -1;
        }

        private void ConvertDataToVBO(bool afterConvetionToVBO, bool additionalData)
        {
            uint overallVertexQuantity = 0;

            for (Int32 i = 0; i < _limbs.Count; ++i)
            {
                overallVertexQuantity += Convert.ToUInt32(_limbs[i].face.Length / 3);
            }

            Verts = new float[overallVertexQuantity, 3];
            N_Verts = new float[overallVertexQuantity, 3];
            T_Verts = new float[overallVertexQuantity, 2];

            Int32 objectCounter = 0;
            if (overallVertexQuantity <= UInt32.MaxValue)
            {
                for (uint iGlobal = 0, iLocal = 0; (objectCounter < _limbs.Count)
                && (iLocal < _limbs[objectCounter].face.Length / 3)
                && (iGlobal < overallVertexQuantity); ++iLocal, ++iGlobal)
                {
                    Verts[iGlobal, 0] = _limbs[objectCounter].verts[_limbs[objectCounter].face[iLocal, 0], 0]; // X pos of vertex
                    Verts[iGlobal, 1] = _limbs[objectCounter].verts[_limbs[objectCounter].face[iLocal, 0], 1]; // Y pos of vertex
                    Verts[iGlobal, 2] = _limbs[objectCounter].verts[_limbs[objectCounter].face[iLocal, 0], 2]; // Z pos of vertex

                    try // Might not be some of normal vertexes
                    {
                        N_Verts[iGlobal, 0] = _limbs[objectCounter].n_verts[_limbs[objectCounter].face[iLocal, 1], 0]; // X pos of normal_vertex
                        N_Verts[iGlobal, 1] = _limbs[objectCounter].n_verts[_limbs[objectCounter].face[iLocal, 1], 1]; // Y pos of normal_vertex
                        N_Verts[iGlobal, 2] = _limbs[objectCounter].n_verts[_limbs[objectCounter].face[iLocal, 1], 2]; // Z pos of normal_vertex
                    }
                    catch // to avoid errors
                    {
                        N_Verts[iGlobal, 0] = 0;
                        N_Verts[iGlobal, 1] = 0;
                        N_Verts[iGlobal, 2] = 0;
                    } 

                    try // Might not be some of texture coordinates
                    {
                        T_Verts[iGlobal, 0] = _limbs[objectCounter].t_verts[_limbs[objectCounter].face[iLocal, 2], 0]; // U pos of tex_vertex
                        T_Verts[iGlobal, 1] = _limbs[objectCounter].t_verts[_limbs[objectCounter].face[iLocal, 2], 1]; // V pos of tex_vertex
                    }
                    catch // to avoid errors
                    {
                        T_Verts[iGlobal, 0] = 0;
                        T_Verts[iGlobal, 1] = 0;
                    } 

                    if ((iLocal + 1) == _limbs[objectCounter].face.Length / 3)
                    {
                        iLocal = 0;
                        ++objectCounter;
                    }
                }
            }
            else
            {
                for (ulong iGlobal = 0, iLocal = 0; (objectCounter < _limbs.Count)
                && (iLocal < Convert.ToUInt64(_limbs[objectCounter].face.Length / 3))
                && (iGlobal < overallVertexQuantity); ++iLocal, ++iGlobal)
                {
                    Verts[iGlobal, 0] = _limbs[objectCounter].verts[_limbs[objectCounter].face[iLocal, 0], 0]; // X pos of vertex
                    Verts[iGlobal, 1] = _limbs[objectCounter].verts[_limbs[objectCounter].face[iLocal, 0], 1]; // Y pos of vertex
                    Verts[iGlobal, 2] = _limbs[objectCounter].verts[_limbs[objectCounter].face[iLocal, 0], 2]; // Z pos of vertex

                    N_Verts[iGlobal, 0] = _limbs[objectCounter].n_verts[_limbs[objectCounter].face[iLocal, 1], 0]; // X pos of normal_vertex
                    N_Verts[iGlobal, 1] = _limbs[objectCounter].n_verts[_limbs[objectCounter].face[iLocal, 1], 1]; // Y pos of normal_vertex
                    N_Verts[iGlobal, 2] = _limbs[objectCounter].n_verts[_limbs[objectCounter].face[iLocal, 1], 2]; // Z pos of normal_vertex

                    try // Might not be some of texture coordinates
                    {
                        T_Verts[iGlobal, 0] = _limbs[objectCounter].t_verts[_limbs[objectCounter].face[iLocal, 2], 0]; // U pos of tex_vertex
                        T_Verts[iGlobal, 1] = _limbs[objectCounter].t_verts[_limbs[objectCounter].face[iLocal, 2], 1]; // V pos of tex_vertex
                    }
                    catch // to avoid errors
                    {
                        T_Verts[iGlobal, 0] = 0;
                        T_Verts[iGlobal, 1] = 0;
                    }

                    if ((iLocal + 1) == Convert.ToUInt64(_limbs[objectCounter].face.Length / 3))
                    {
                        iLocal = 0;
                        ++objectCounter;
                    }
                }
            }

            overallVertexQuantity = 0;
            CleanUp(afterConvetionToVBO, additionalData);
        }
        #endregion

        public void CleanUp(bool afterConvetionToVBO, bool additionalData)
        {
            if (afterConvetionToVBO)
            {
                _limbs = null;
            }
            else
            {
                if (additionalData)
                {
                    for (Int32 i = 0; i < _limbs.Count; ++i)
                    {
                        _limbs[i].lines = null;
                        _limbs[i].objectID = String.Empty;
                        _limbs[i].objectName = String.Empty;
                    }
                }
            }
        }

    } // end of class
}
