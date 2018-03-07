using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CParser
{
    public abstract class DataLoaderAbstract
    {
        protected string _modelName = "";
        public int materialCount = 0;
        public int[] materialNumber = null;

        public abstract void LoadData(string dataFilePath);

        public bool equalityCheckOfModelName(string modelName, string dataFilePath)
        {
            if (dataFilePath.Remove(dataFilePath.Length - 1 - 4).EndsWith(modelName.Remove(modelName.Length - 1 - 4)))
                return true;
            else
                return false;
        }
    }
}
