﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics;

namespace ShaderPattern
{
    public abstract class Shader : IShaderEditable, IShaderDefine
    {
        #region DefineParams

        public struct DefineParams
        {
            public string Name;
            public string Value;

            public DefineParams(string name, string value)
            {
                Name = name;
                Value = value;
            }
        }

        #endregion

        #region Definitions

        private string vsPath;
        private string fsPath;
        private string gsPath;

        private int vertexShaderID;
        private int fragmentShaderID;
        private int geometryShaderID;
        private int shaderProgramID;
        protected bool ShaderLoaded { set; get; }

        private Dictionary<ShaderType, DefineParams> DefineParameters;

        #endregion

        #region Shader load

        private bool loadShaders()
        {
            /*Vertex shader load*/
            StreamReader streamer = null;
            try
            {
                streamer = new StreamReader(vsPath);
            }
            catch (IOException)
            {
                return false;
            }
            vertexShaderID = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShaderID, streamer.ReadToEnd());
            streamer.Close();

            /*Fragment shader load*/
            try
            {
                streamer = new StreamReader(fsPath);
            }
            catch (IOException)
            {
                return false;
            }
            fragmentShaderID = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShaderID, streamer.ReadToEnd());
            streamer.Close();

            /*Geometry shader load*/
            if (gsPath != "")
            {
                try
                {
                    streamer = new StreamReader(gsPath);
                }
                catch (IOException)
                {
                    return false;
                }
                geometryShaderID = GL.CreateShader(ShaderType.GeometryShader);
                GL.ShaderSource(geometryShaderID, streamer.ReadToEnd());
                streamer.Close();

                GL.CompileShader(geometryShaderID);
            }
            GL.CompileShader(vertexShaderID);
            GL.CompileShader(fragmentShaderID);

            return true;
        }

        private void linkShaders()
        {
            GL.AttachShader(shaderProgramID, vertexShaderID);
            GL.AttachShader(shaderProgramID, fragmentShaderID);
            if (geometryShaderID != -1) { GL.AttachShader(shaderProgramID, geometryShaderID); }
            GL.LinkProgram(shaderProgramID);
            GL.DetachShader(shaderProgramID, vertexShaderID);
            GL.DetachShader(shaderProgramID, fragmentShaderID);
            if (geometryShaderID != -1) { GL.DetachShader(shaderProgramID, geometryShaderID); }
        }

        #endregion

        #region Show_log

        protected void showCompileLogInfo(string ShaderName)
        {
            int capacity = 0;
            /*Vertex shader log info*/
            unsafe { GL.GetShader(vertexShaderID, ShaderParameter.InfoLogLength, &capacity); }
            StringBuilder info = new StringBuilder(capacity);
            unsafe { GL.GetShaderInfoLog(vertexShaderID, Int32.MaxValue, null, info); }
            if (info.Length != 0)
            {
                Console.WriteLine("Unsolved mistakes at :" + ShaderName + "\n" + info);
            }

            /*Fragment shader log info*/
            unsafe { GL.GetShader(fragmentShaderID, ShaderParameter.InfoLogLength, &capacity); }
            info = new StringBuilder(capacity);
            unsafe { GL.GetShaderInfoLog(fragmentShaderID, Int32.MaxValue, null, info); }
            if (info.Length != 0)
            {
                Console.WriteLine("Unsolved mistakes at :" + ShaderName + "\n" + info);
            }

            /*Geometry shader log info*/
            if (geometryShaderID != -1)
            {
                unsafe { GL.GetShader(geometryShaderID, ShaderParameter.InfoLogLength, &capacity); }
                info = new StringBuilder(capacity);
                unsafe { GL.GetShaderInfoLog(geometryShaderID, Int32.MaxValue, null, info); }
                if (info.Length != 0)
                {
                    Console.WriteLine("Unsolved mistakes at  :" + ShaderName + "\n" + info);
                }
            }
        }

        protected void showLinkLogInfo(string ShaderName)
        {
            int capacity = 0;
            /*Shader program link log info*/
            unsafe { GL.GetProgram(shaderProgramID, GetProgramParameterName.InfoLogLength, &capacity); }
            StringBuilder info = new StringBuilder(capacity);
            unsafe { GL.GetProgramInfoLog(shaderProgramID, Int32.MaxValue, null, info); }
            if (info.Length != 0)
            {
                Console.WriteLine("Unsolved mistakes at :" + ShaderName + "\n" + info);
            }
        }

        public string getCompileLogInfo(string ShaderName)
        {
            int capacity = 0;
            /*Vertex shader log info*/
            unsafe { GL.GetShader(vertexShaderID, ShaderParameter.InfoLogLength, &capacity); }
            StringBuilder info = new StringBuilder(capacity);
            unsafe { GL.GetShaderInfoLog(vertexShaderID, Int32.MaxValue, null, info); }
            
            /*Fragment shader log info*/
            unsafe { GL.GetShader(fragmentShaderID, ShaderParameter.InfoLogLength, &capacity); }
            StringBuilder info1 = new StringBuilder(capacity);
            unsafe { GL.GetShaderInfoLog(fragmentShaderID, Int32.MaxValue, null, info1); }

            /*Geometry shader log info*/
            StringBuilder info2 = new StringBuilder();
            if (geometryShaderID != -1)
            {
                unsafe { GL.GetShader(geometryShaderID, ShaderParameter.InfoLogLength, &capacity); }
                info2 = new StringBuilder(capacity);
                unsafe { GL.GetShaderInfoLog(geometryShaderID, Int32.MaxValue, null, info); }
            }

            if (info.Length > 0 || info1.Length > 0 || info2.Length > 0)
                return ("Unsolved mistakes at :" + ShaderName + "\n" + Convert.ToString(info) +
                    "\n" + Convert.ToString(info1) + "\n" + Convert.ToString(info2));
            else return (ShaderName + ": No errors found.");
        }

        public string getLinkLogInfo(string ShaderName)
        {
            int capacity = 0;
            /*Shader program link log info*/
            unsafe { GL.GetProgram(this.shaderProgramID, GetProgramParameterName.InfoLogLength, &capacity); }
            StringBuilder info = new StringBuilder(capacity);
            unsafe { GL.GetProgramInfoLog(this.shaderProgramID, Int32.MaxValue, null, info); }
            if (info.Length > 0)
                return ("Unsolved mistakes at :" + ShaderName + "\n" + Convert.ToString(info));
            else return (ShaderName + ": No errors found.");
        }

        #endregion

        #region Using

        public void startProgram()
        {
            GL.UseProgram(shaderProgramID);
        }
        
        public void stopProgram()
        {
            GL.UseProgram(0);
        }

        #endregion

        #region Cleaning

        public void cleanUp()
        {
            stopProgram();
            GL.DetachShader(shaderProgramID, vertexShaderID);
            GL.DetachShader(shaderProgramID, fragmentShaderID);
            if (geometryShaderID != -1) { GL.DetachShader(shaderProgramID, geometryShaderID); GL.DeleteShader(geometryShaderID); }
            GL.DeleteShader(vertexShaderID);
            GL.DeleteShader(fragmentShaderID);
            
            GL.DeleteProgram(shaderProgramID);
        }

        #endregion

        #region Getters

        protected int getUniformLocation(string uniformName)
        {
            return GL.GetUniformLocation(shaderProgramID, uniformName);
        }

        protected int getSubroutineIndex(ShaderType type, string indexName)
        {
            return GL.GetSubroutineIndex(this.shaderProgramID, type, indexName);
        }

        protected virtual void getAllUniformLocations()
        {

        }

        #endregion

        #region Load_uniforms

        protected void loadFloat(int location, float value)
        {
            GL.Uniform1(location, value);
        }

        protected void loadVector(int location, Vector3 vector)
        {
            GL.Uniform3(location, vector);
        }

        protected void loadVector(int location, Vector4 vector)
        {
            GL.Uniform4(location, vector);
        }

        protected void loadVector(int location ,Vector2 vector)
        {
            GL.Uniform2(location, vector);
        }

        protected void loadBool(int location, bool value)
        {
            GL.Uniform1(location, value ? 1.0f : 0.0f);
        }

        protected void loadMatrix(int location, bool transpose, Matrix4 matrix)
        {
            GL.UniformMatrix4(location, transpose, ref matrix);
        }

        protected void loadNormalMatrix(int location, bool transpose, Matrix3 matrix)
        {
            GL.UniformMatrix3(location, transpose, ref matrix);
        }

        protected void loadInteger(int location, int value)
        {
            GL.Uniform1(location, value);
        }

        protected void loadSubroutineIndex(ShaderType type, int countIndices , int subroutineIndex)
        {
            GL.UniformSubroutines(type, countIndices, ref subroutineIndex);
        }

        #endregion

        #region Constructor

        public Shader(string VertexShaderFile, string FragmentShaderFile, string GeometryShaderFile = "")
        {
            DefineParameters = new Dictionary<ShaderType, DefineParams>();
            vsPath = VertexShaderFile;
            fsPath = FragmentShaderFile;
            gsPath = GeometryShaderFile;

            geometryShaderID = -1;
            ShaderLoaded = false;
            // start precompilation shader customization
            PrecompileEdit();
            AddPrecompiledEditToShader();
            ClearAfterPrecompilationEditStage();
            // end
            ShaderLoaded = loadShaders();
            shaderProgramID = GL.CreateProgram();
            if (ShaderLoaded)
            {
                linkShaders();
                getAllUniformLocations();
            }
        }

        #endregion

        #region PrecompilationEdit

        private void EditShader(ShaderType type, string shaderPath)
        {
            if (shaderPath == string.Empty)
                return;

            StreamReader reader = new StreamReader(shaderPath);
            List<string> code = new List<string>();
            while (!reader.EndOfStream)
            {
                code.Add(reader.ReadLine());
            }
            reader.Close();
            reader.Dispose();

            // code only with macos
            List<DefineParams> macros = new List<DefineParams>();
            code.ForEach((str) =>
            {
                if (str.StartsWith("#define"))
                {
                    int indexName = str.IndexOf(' ');
                    int indexValue = str.IndexOf(' ', indexName + 1);
                    var name = str.Substring(indexName, indexValue - indexName + 1);
                    var value = str.Substring(indexValue + 1);
                    macros.Add(new DefineParams(name, value));
                }
            });
            // remove all macros from code
            code.RemoveAll((str) => str.StartsWith("#define"));

            // get macros only for current shader
            List<DefineParams> input = new List<DefineParams>();
            foreach (var def in DefineParameters)
            {
                if (def.Key == type)
                    input.Add(def.Value);
            }

            // update values for existing macros
            for (int i = 0; i < input.Count; i++)
            {
                if (macros.Any(def => def.Name == input[i].Name))
                    macros.RemoveAll(def => def.Name == input[i].Name);

                macros.Add(input[i]);
            }

            string macroResult = String.Empty;
            macros.ForEach(def =>
            {
                macroResult += string.Format("{0}#define {1} {2}", "\n\r", def.Name, def.Value);
            });

            int startIndex = code.IndexOf("void main()") - 1;
            code[startIndex] = code[startIndex] + macroResult;

            string codeResult = string.Empty;
            code.ForEach(str =>
            {
                codeResult += str;
            });

            StreamWriter writer = new StreamWriter(shaderPath, false);
            writer.WriteLine(codeResult);
            writer.Close();
            writer.Dispose();
        }

        private void AddPrecompiledEditToShader()
        {
            if (DefineParameters.Count > 0)
            {
                if (DefineParameters.Any(def => def.Key == ShaderType.VertexShader))
                    EditShader(ShaderType.VertexShader, vsPath);
                if (DefineParameters.Any(def => def.Key == ShaderType.FragmentShader))
                    EditShader(ShaderType.FragmentShader, fsPath);
                if (gsPath != string.Empty)
                    if (DefineParameters.Any(def => def.Key == ShaderType.GeometryShader))
                        EditShader(ShaderType.GeometryShader, gsPath);
            }
        }

        public virtual void PrecompileEdit()
        {

        }

        public void SetDefine(ShaderType shaderType, string name, string formatValue)
        {
            DefineParameters.Add(shaderType, new DefineParams(name, formatValue));
        }

        private void ClearAfterPrecompilationEditStage()
        {
            DefineParameters.Clear();
            DefineParameters = null;
        }

        #endregion
    }
}