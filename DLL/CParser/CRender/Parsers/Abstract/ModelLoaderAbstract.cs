using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CParser
{
    public abstract class ModelLoaderAbstract
    {
        protected bool isLoad;      // загружен ли (флаг)

        public abstract int LoadModel(string modelFilePath);

        protected abstract void ConvertModelData();

        protected abstract void CleanUp();
    }
}
