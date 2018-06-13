using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Grid
{
    public class TableGrid                    //class to work with table
    {
        public Int32 TableSize { set; get; }
        public float[,] Table { set { _tableGrid = value; } get { return _tableGrid; } }
        public double GridStep { get; private set; }          //step between cells in table

        public float this[Int32 i, Int32 j]
        {
            set { _tableGrid[i, j] = value; }
            get { return _tableGrid[i, j]; }
        }

        public void loadTableFromFile(String filePath)
        {
            StreamReader sr = new StreamReader(filePath);
        }

        private float[,] _tableGrid;
        
        public TableGrid()                  //basic constructor
        {
            TableSize = 0;
            _tableGrid = new float[TableSize, TableSize];
            GridStep = 0.0;
        }
        public TableGrid(Int32 TableSize, double GridStep)    //constructor
        {
            this.TableSize = TableSize;
            _tableGrid = new float[TableSize, TableSize];
            this.GridStep = GridStep;
        }   
    }
}
