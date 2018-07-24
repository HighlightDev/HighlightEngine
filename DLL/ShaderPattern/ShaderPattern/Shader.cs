using System;
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

        [Flags]
        public enum ShaderTypeFlag
        {
            VertexShader = 0x001,
            FragmentShader = 0x010,
            GeometryShader = 0x100
        }

        #region DefineParams

        public struct DefineParams
        {
            public string Name;
            public string Value;
            public ShaderTypeFlag shaderType;

            public DefineParams(string name, string value, ShaderTypeFlag shaderType)
            {
                Name = name;
                Value = value;
                this.shaderType = shaderType;
            }

            public DefineParams(string name, string value)
            {
                Name = name;
                Value = value;
                this.shaderType = ShaderTypeFlag.VertexShader;
            }
        }

        #endregion

        #region Definitions

        private string vsPath;
        private string fsPath;
        private string gsPath;

        private Int32 vertexShaderID;
        private Int32 fragmentShaderID;
        private Int32 geometryShaderID;
        private Int32 shaderProgramID;
        protected bool ShaderLoaded { set; get; }

        private List<DefineParams> DefineParameters;

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
            Int32 capacity = 0;
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
            Int32 capacity = 0;
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
            Int32 capacity = 0;
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
            else return null;
        }

        public string getLinkLogInfo(string ShaderName)
        {
            Int32 capacity = 0;
            /*Shader program link log info*/
            unsafe { GL.GetProgram(this.shaderProgramID, GetProgramParameterName.InfoLogLength, &capacity); }
            StringBuilder info = new StringBuilder(capacity);
            unsafe { GL.GetProgramInfoLog(this.shaderProgramID, Int32.MaxValue, null, info); }
            if (info.Length > 0)
                return ("Unsolved mistakes at :" + ShaderName + "\n" + Convert.ToString(info));
            else return null;
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

        protected Int32 getUniformLocation(string uniformName)
        {
            return GL.GetUniformLocation(shaderProgramID, uniformName);
        }

        protected Int32 getSubroutineIndex(ShaderType type, string indexName)
        {
            return GL.GetSubroutineIndex(this.shaderProgramID, type, indexName);
        }

        protected virtual void getAllUniformLocations() { }

        #endregion

        #region Load_uniforms

        protected Uniform GetUniform(string uniformName)
        {
           return new Uniform(shaderProgramID, uniformName);
        }

        protected void loadSubroutineIndex(ShaderType type, Int32 countIndices, Int32 subroutineIndex)
        {
            GL.UniformSubroutines(type, countIndices, ref subroutineIndex);
        }

        #endregion

        #region Constructor

        public Shader(string VertexShaderFile, string FragmentShaderFile, string GeometryShaderFile = "")
        {
            DefineParameters = new List<DefineParams>();
            vsPath = VertexShaderFile;
            fsPath = FragmentShaderFile;
            gsPath = GeometryShaderFile;

            geometryShaderID = -1;
            ShaderLoaded = false;

            SetShaderMacros(); // start precompilation shader customization
            PrecompileEdit();
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

        private void EditShader(ShaderTypeFlag type, string shaderPath)
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

            // code only with macros
            List<DefineParams> macros = new List<DefineParams>();
            code.ForEach((str) =>
            {
                if (str.StartsWith("#define"))
                {
                    Int32 indexName = str.IndexOf(' ');
                    Int32 indexValue = str.IndexOf(' ', indexName + 1);
                    var name = str.Substring(indexName + 1, indexValue - indexName - 1);
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
                if ((def.shaderType & ShaderTypeFlag.VertexShader) == type ||
                    (def.shaderType & ShaderTypeFlag.FragmentShader) == type ||
                    (def.shaderType & ShaderTypeFlag.GeometryShader) == type)
                    input.Add(def);
            }

            // update values for existing macros
            for (Int32 i = 0; i < input.Count; i++)
            {
                if (macros.Any(def => def.Name == input[i].Name))
                    macros.RemoveAll(def => def.Name == input[i].Name);

                macros.Add(input[i]);
            }

            string macroResult = String.Empty;
            macros.ForEach(def =>
            {
                macroResult += string.Format("#define {0} {1} \n", def.Name, def.Value);
            });

            Int32 startIndex = code.FindIndex(new Predicate<string>(s => s.StartsWith("#version"))) + 2;
            code.Insert(startIndex, macroResult);

            string codeResult = string.Empty;
            for (Int32 i = 0; i < code.Count; i++)
            {
                string str = code[i];
                str = str.TrimEnd();
                if (code.Count - 1 > i)
                    codeResult += str + "\n";
                else
                    codeResult += str;
            }

            Int32 indexNewLine = codeResult.LastIndexOf("\n");
            indexNewLine = codeResult.LastIndexOf("\n") > indexNewLine ? codeResult.LastIndexOf("\n") : indexNewLine;

            codeResult = codeResult.EndsWith("\n") || codeResult.EndsWith("\r") ? codeResult.Remove(indexNewLine) : codeResult;

            StreamWriter writer = new StreamWriter(shaderPath, false);
            writer.WriteLine(codeResult);
            writer.Close();
            writer.Dispose();
        }

        private void AddPrecompiledEditToShader()
        {
            if (DefineParameters.Count > 0)
            {
                if (DefineParameters.Any(def => (def.shaderType & ShaderTypeFlag.VertexShader) == ShaderTypeFlag.VertexShader))
                    EditShader(ShaderTypeFlag.VertexShader, vsPath);
                if (DefineParameters.Any(def => (def.shaderType & ShaderTypeFlag.FragmentShader) == ShaderTypeFlag.FragmentShader))
                    EditShader(ShaderTypeFlag.FragmentShader, fsPath);
                if (gsPath != string.Empty)
                    if (DefineParameters.Any(def => (def.shaderType & ShaderTypeFlag.GeometryShader) == ShaderTypeFlag.GeometryShader))
                        EditShader(ShaderTypeFlag.GeometryShader, gsPath);
            }
        }

        protected abstract void SetShaderMacros();

        public virtual void PrecompileEdit()
        {
            AddPrecompiledEditToShader();
            DefineParameters.Clear();
        }

        public void SetDefine<T>(ShaderTypeFlag shaderType, string name, T value) where T : struct
        {
            string formatedValue = ShaderMacrosConverter<T>.ConvertToString(value);
            DefineParameters.Add(new DefineParams(name, formatedValue, shaderType));
       } 

        #endregion
    }
}
