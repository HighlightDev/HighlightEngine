using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CParser.OBJ_Parser
{
    public class OBJ_MaterialDataLoader : DataLoaderAbstract
    {
        public OBJ_MaterialDataLoader(string modelName, string materialDataFilePath)
        {
            // names of obj file and mtl file must be same
            if (equalityCheckOfModelName(modelName, materialDataFilePath))
                LoadData(materialDataFilePath);
        }


        #region Variables with descriptions
        public string[] materialName = null;
        /* map_Ka(-options args filename) - Specifies that a color texture file or a color procedural texture file is 
              applied to the ambient reflectivity of the material. During rendering, the map_Ka value is multiplied by the Ka value.
              "filename" is the name of a color texture file (.mpc), a color procedural texture file (.cxc), or an image file. */
        public string[] materialPath = null;
        // Ka - The Ka statement specifies the ambient reflectivity.
        public double[,] ambientReflectivity = null;
        // Kd - To specify the diffuse reflectivity of the current material.
        public double[,] diffuseReflectivity = null;
        // Ks - The Ks statement specifies the specular reflectivity.
        public double[,] specularReflectivity = null;
        // illum - The "illum" statement specifies the illumination model to use in the material.
        public int[] illuminationNumber = null;
        // d factor - Specifies the dissolve for the current material. 
        public double[] dissolveFactor = null;
        // Ns exponent - Specifies the specular exponent for the current material. This defines the focus of the specular highlight.
        public double[] specularExponent = null;
        // Ni optical_density - Specifies the optical density for the surface. This is also known as index of refraction.
        public double[] opticalDensity = null;
        // Tf - To specify the transmission filter of the current material.
/*if it'll be needed, it'll be added here*/
        /* map_Kd(-options args filename) - Specifies that a color texture file or color procedural texture file is 
              linked to the diffuse reflectivity of the material. During rendering, the map_Kd value is multiplied by the Kd value.
              "filename" is the name of a color texture file (.mpc), a color procedural texture file (.cxc), or an image file.*/
/*if it'll be needed, it'll be added here*/
        /* map_Ks(-options args filename) - Specifies that a color texture file or color procedural texture file is 
              linked to the specular reflectivity of the material. During rendering, the map_Ks value is multiplied by the Ks value.
              "filename" is the name of a color texture file (.mpc), a color procedural texture file (.cxc), or an image file. */
/*if it'll be needed, it'll be added here*/
        /* map_Ns(-options args filename) - Specifies that a scalar texture file or scalar procedural texture file is 
              linked to the specular exponent of the material. During rendering, the map_Ns value is multiplied by the Ns value. 
              "filename" is the name of a scalar texture file (.mps), a scalar procedural texture file (.cxs), or an image file. */
/*if it'll be needed, it'll be added here*/
        /*map_d(-options args filename) - Specifies that a scalar texture file or scalar procedural texture file is 
              linked to the dissolve of the material. During rendering, the map_d value is multiplied by the d value.
              "filename" is the name of a scalar texture file (.mps), a scalar procedural texture file (.cxs), or an image file. */
/*if it'll be needed, it'll be added here*/
        //map_aat(on) - Turns on anti-aliasing of textures in this material without anti-aliasing all textures in the scene.
/*if it'll be needed, it'll be added here*/
        /*decal(-options args filename) - Specifies that a scalar texture file or a scalar procedural texture file is 
              used to selectively replace the material color with the texture color.
              "filename" is the name of a scalar texture file (.mps), a scalar procedural texture file (.cxs), or an image file. */
        /*if it'll be needed, it'll be added here*/
        #endregion


        public override void LoadData(string dataFilePath)
        {
            FileInfo file = new FileInfo(dataFilePath);
            if (file.Exists)
            {
                int index = -1;
                string[] tempVault = new string[3];
                string mtlFileString = "", mtlFilePrefix = "";
                StreamReader sr_mtlFile = new StreamReader(dataFilePath, Encoding.UTF8);

                while (!sr_mtlFile.EndOfStream)
                {
                    mtlFileString = sr_mtlFile.ReadLine();
                    if (mtlFileString == "") continue;
                    mtlFilePrefix = mtlFileString.Remove(mtlFileString.IndexOf(' '));
                    mtlFileString = mtlFileString.Remove(0, mtlFilePrefix.Length + 1);

                    switch (mtlFilePrefix)
                    {
                        #region Parsing of file
                        case "#":
                            if (mtlFileString.StartsWith("Blender MTL File: "))
                            {
                                mtlFileString = mtlFileString.Replace("Blender MTL File: ", "");
                                _modelName = mtlFileString.Trim(' ', '\'', '\"');
                            }
                            else if (mtlFileString.StartsWith("Material Count: "))
                            {
                                mtlFileString = mtlFileString.Replace("Material Count: ", "");
                                materialCount = Convert.ToInt32(mtlFileString.Trim(' '));
                                initVariables(materialCount);
                            }
                            break;
                        case "newmtl":
                            ++index;
                            materialName[index] = mtlFileString;
                            materialNumber[index] = index;
                            break;
                        case "Ns":
                            specularExponent[index] = Convert.ToDouble(mtlFileString.Replace('.', ','));
                            break;
                        case "Ka":
                            tempVault = mtlFileString.Split(' ');
                            ambientReflectivity[index, 0] = Convert.ToDouble(tempVault[0].Replace('.', ','));
                            ambientReflectivity[index, 1] = Convert.ToDouble(tempVault[1].Replace('.', ','));
                            ambientReflectivity[index, 2] = Convert.ToDouble(tempVault[2].Replace('.', ','));
                            break;
                        case "Kd":
                            tempVault = mtlFileString.Split(' ');
                            diffuseReflectivity[index, 0] = Convert.ToDouble(tempVault[0].Replace('.', ','));
                            diffuseReflectivity[index, 1] = Convert.ToDouble(tempVault[1].Replace('.', ','));
                            diffuseReflectivity[index, 2] = Convert.ToDouble(tempVault[2].Replace('.', ','));
                            break;
                        case "Ks":
                            tempVault = mtlFileString.Split(' ');
                            specularReflectivity[index, 0] = Convert.ToDouble(tempVault[0].Replace('.', ','));
                            specularReflectivity[index, 1] = Convert.ToDouble(tempVault[1].Replace('.', ','));
                            specularReflectivity[index, 2] = Convert.ToDouble(tempVault[2].Replace('.', ','));
                            break;
                        case "Ni":
                            opticalDensity[index] = Convert.ToDouble(mtlFileString.Replace('.', ','));
                            break;
                        case "d":
                            dissolveFactor[index] = Convert.ToDouble(mtlFileString.Replace('.', ','));
                            break;
                        case "illum":
                            illuminationNumber[index] = Convert.ToInt32(mtlFileString);
                            break;
                        case "map_Kd":
                            materialPath[index] = mtlFileString;
                            break;
                        #endregion
                    }
                }

                sr_mtlFile.Close();
            }
        }

        private void initVariables(int materialQuantity)
        {
            materialNumber = new int[materialQuantity];
            materialName = new string[materialQuantity];
            materialPath = new string[materialQuantity];
            ambientReflectivity = new double[materialQuantity, 3];
            diffuseReflectivity = new double[materialQuantity, 3];
            specularReflectivity = new double[materialQuantity, 3];
            specularExponent = new double[materialQuantity];
            opticalDensity = new double[materialQuantity];
            dissolveFactor = new double[materialQuantity];
            illuminationNumber = new int[materialQuantity];
        }

    }
}

/*
    posible values of illum: 

0	Color on and Ambient off 
1	Color on and Ambient on 
2	Highlight on 
3	Reflection on and Ray trace on 
4	Transparency: Glass on 
    Reflection: Ray trace on 
5	Reflection: Fresnel on and Ray trace on 
6	Transparency: Refraction on 
    Reflection: Fresnel off and Ray trace on 
7	Transparency: Refraction on 
    Reflection: Fresnel on and Ray trace on 
8	Reflection on and Ray trace off 
9	Transparency: Glass on 
    Reflection: Ray trace off 
10	Casts shadows onto invisible surfaces 
*/