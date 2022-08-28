using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace Nonogram
{
    [Serializable]
    public class DataLevelList
    {
        public List<Level> list;

        public DataLevelList(LevelManager levelManager)
        {
            list = levelManager.dataList;
        }

        public List<Level> GetLevels()
        {
            return list;
        }

    }
}