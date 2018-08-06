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

        private string m_vsPath;
        private string m_fsPath;
        private string m_gsPath;
        private string m_shaderName;

        private Int32 m_vertexShaderID;
        private Int32 m_fragmentShaderID;
        private Int32 m_geometryShaderID;
        private Int32 m_shaderProgramID;
        protected bool m_shaderLoaded { set; get; }

        private List<DefineParams> m_defineParameters;

        #endregion

        #region Shader load

        private bool loadShaders()
        {
            /*Vertex shader load*/
            StreamReader streamer = null;
            try
            {
                streamer = new StreamReader(m_vsPath);
            }
            catch (IOException)
            {
                return false;
            }
            m_vertexShaderID = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(m_vertexShaderID, streamer.ReadToEnd());
            streamer.Close();

            /*Fragment shader load*/
            try
            {
                streamer = new StreamReader(m_fsPath);
            }
            catch (IOException)
            {
                return false;
            }
            m_fragmentShaderID = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(m_fragmentShaderID, streamer.ReadToEnd());
            streamer.Close();

            /*Geometry shader load*/
            if (m_gsPath != "")
            {
                try
                {
                    streamer = new StreamReader(m_gsPath);
                }
                catch (IOException)
                {
                    return false;
                }
                m_geometryShaderID = GL.CreateShader(ShaderType.GeometryShader);
                GL.ShaderSource(m_geometryShaderID, streamer.ReadToEnd());
                streamer.Close();

                GL.CompileShader(m_geometryShaderID);
            }
            GL.CompileShader(m_vertexShaderID);
            GL.CompileShader(m_fragmentShaderID);

            return true;
        }

        private void linkShaders()
        {
            GL.AttachShader(m_shaderProgramID, m_vertexShaderID);
            GL.AttachShader(m_shaderProgramID, m_fragmentShaderID);
            if (m_geometryShaderID != -1) { GL.AttachShader(m_shaderProgramID, m_geometryShaderID); }
            GL.LinkProgram(m_shaderProgramID);
            GL.DetachShader(m_shaderProgramID, m_vertexShaderID);
            GL.DetachShader(m_shaderProgramID, m_fragmentShaderID);
            if (m_geometryShaderID != -1) { GL.DetachShader(m_shaderProgramID, m_geometryShaderID); }
        }

        #endregion

        #region Show_log

        protected void showCompileLogInfo(string ShaderName)
        {
            Int32 capacity = 0;
            /*Vertex shader log info*/
            unsafe { GL.GetShader(m_vertexShaderID, ShaderParameter.InfoLogLength, &capacity); }
            StringBuilder info = new StringBuilder(capacity);
            unsafe { GL.GetShaderInfoLog(m_vertexShaderID, Int32.MaxValue, null, info); }
            if (info.Length != 0)
            {
                Console.WriteLine("Unsolved mistakes at :" + ShaderName + "\n" + info);
            }

            /*Fragment shader log info*/
            unsafe { GL.GetShader(m_fragmentShaderID, ShaderParameter.InfoLogLength, &capacity); }
            info = new StringBuilder(capacity);
            unsafe { GL.GetShaderInfoLog(m_fragmentShaderID, Int32.MaxValue, null, info); }
            if (info.Length != 0)
            {
                Console.WriteLine("Unsolved mistakes at :" + ShaderName + "\n" + info);
            }

            /*Geometry shader log info*/
            if (m_geometryShaderID != -1)
            {
                unsafe { GL.GetShader(m_geometryShaderID, ShaderParameter.InfoLogLength, &capacity); }
                info = new StringBuilder(capacity);
                unsafe { GL.GetShaderInfoLog(m_geometryShaderID, Int32.MaxValue, null, info); }
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
            unsafe { GL.GetProgram(m_shaderProgramID, GetProgramParameterName.InfoLogLength, &capacity); }
            StringBuilder info = new StringBuilder(capacity);
            unsafe { GL.GetProgramInfoLog(m_shaderProgramID, Int32.MaxValue, null, info); }
            if (info.Length != 0)
            {
                Console.WriteLine("Unsolved mistakes at :" + ShaderName + "\n" + info);
            }
        }

        public string getCompileLogInfo(string ShaderName)
        {
            Int32 capacity = 0;
            /*Vertex shader log info*/
            unsafe { GL.GetShader(m_vertexShaderID, ShaderParameter.InfoLogLength, &capacity); }
            StringBuilder info = new StringBuilder(capacity);
            unsafe { GL.GetShaderInfoLog(m_vertexShaderID, Int32.MaxValue, null, info); }

            /*Fragment shader log info*/
            unsafe { GL.GetShader(m_fragmentShaderID, ShaderParameter.InfoLogLength, &capacity); }
            StringBuilder info1 = new StringBuilder(capacity);
            unsafe { GL.GetShaderInfoLog(m_fragmentShaderID, Int32.MaxValue, null, info1); }

            /*Geometry shader log info*/
            StringBuilder info2 = new StringBuilder();
            if (m_geometryShaderID != -1)
            {
                unsafe { GL.GetShader(m_geometryShaderID, ShaderParameter.InfoLogLength, &capacity); }
                info2 = new StringBuilder(capacity);
                unsafe { GL.GetShaderInfoLog(m_geometryShaderID, Int32.MaxValue, null, info); }
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
            unsafe { GL.GetProgram(this.m_shaderProgramID, GetProgramParameterName.InfoLogLength, &capacity); }
            StringBuilder info = new StringBuilder(capacity);
            unsafe { GL.GetProgramInfoLog(this.m_shaderProgramID, Int32.MaxValue, null, info); }
            if (info.Length > 0)
                return ("Unsolved mistakes at :" + ShaderName + "\n" + Convert.ToString(info));
            else return null;
        }

        #endregion

        #region Using

        public void startProgram()
        {
            GL.UseProgram(m_shaderProgramID);
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
            GL.DetachShader(m_shaderProgramID, m_vertexShaderID);
            GL.DetachShader(m_shaderProgramID, m_fragmentShaderID);
            if (m_geometryShaderID != -1) { GL.DetachShader(m_shaderProgramID, m_geometryShaderID); GL.DeleteShader(m_geometryShaderID); }
            GL.DeleteShader(m_vertexShaderID);
            GL.DeleteShader(m_fragmentShaderID);

            GL.DeleteProgram(m_shaderProgramID);
        }

        #endregion

        #region Getters

        protected Int32 getUniformLocation(string uniformName)
        {
            return GL.GetUniformLocation(m_shaderProgramID, uniformName);
        }

        protected Int32 getSubroutineIndex(ShaderType type, string indexName)
        {
            return GL.GetSubroutineIndex(this.m_shaderProgramID, type, indexName);
        }

        protected virtual void getAllUniformLocations() { }

        #endregion

        #region Load_uniforms

        protected Uniform GetUniform(string uniformName)
        {
            try
            {
                return new Uniform(m_shaderProgramID, uniformName);
            }
            catch (ArgumentNullException innerEx)
            {
                String message = String.Format("Shader with name {0} could not bind uniform(s); {1}Inner exception message : {2}{1}", m_shaderName, Environment.NewLine, innerEx.Message);
                throw new ArgumentNullException(message, innerEx);
            }
        }

        protected void loadSubroutineIndex(ShaderType type, Int32 countIndices, Int32 subroutineIndex)
        {
            GL.UniformSubroutines(type, countIndices, ref subroutineIndex);
        }

        #endregion

        #region Constructor

        public Shader(string shaderName, string VertexShaderFile, string FragmentShaderFile, string GeometryShaderFile = "")
        {
            m_shaderName = shaderName;
            m_defineParameters = new List<DefineParams>();
            m_vsPath = VertexShaderFile;
            m_fsPath = FragmentShaderFile;
            m_gsPath = GeometryShaderFile;

            m_geometryShaderID = -1;
            m_shaderLoaded = false;

            SetShaderMacros(); // start precompilation shader customization
            PrecompileEdit();
            m_shaderLoaded = loadShaders();
            m_shaderProgramID = GL.CreateProgram();
            if (m_shaderLoaded)
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
            foreach (var def in m_defineParameters)
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
            if (m_defineParameters.Count > 0)
            {
                if (m_defineParameters.Any(def => (def.shaderType & ShaderTypeFlag.VertexShader) == ShaderTypeFlag.VertexShader))
                    EditShader(ShaderTypeFlag.VertexShader, m_vsPath);
                if (m_defineParameters.Any(def => (def.shaderType & ShaderTypeFlag.FragmentShader) == ShaderTypeFlag.FragmentShader))
                    EditShader(ShaderTypeFlag.FragmentShader, m_fsPath);
                if (m_gsPath != string.Empty)
                    if (m_defineParameters.Any(def => (def.shaderType & ShaderTypeFlag.GeometryShader) == ShaderTypeFlag.GeometryShader))
                        EditShader(ShaderTypeFlag.GeometryShader, m_gsPath);
            }
        }

        protected abstract void SetShaderMacros();

        public virtual void PrecompileEdit()
        {
            AddPrecompiledEditToShader();
            m_defineParameters.Clear();
        }

        public void SetDefine<T>(ShaderTypeFlag shaderType, string name, T value) where T : struct
        {
            string formatedValue = ShaderMacrosConverter<T>.ConvertToString(value);
            m_defineParameters.Add(new DefineParams(name, formatedValue, shaderType));
       } 

        #endregion
    }
}
