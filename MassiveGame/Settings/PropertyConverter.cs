﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MassiveGame.Settings
{
    public interface IPropertyConverter
    {
        object Convert(string property);
    }

    public class PropertyToInt32 : IPropertyConverter
    {
        public object Convert(string property)
        {
            return Int32.Parse(property);
        }
    }

    public class PropertyToBool : IPropertyConverter
    {
        public object Convert(string property)
        {
            return Boolean.Parse(property);
        }
    }

    public class PropertyToFloat : IPropertyConverter
    {
        public object Convert(string property)
        {
            return float.Parse(property);
        }
    }
}