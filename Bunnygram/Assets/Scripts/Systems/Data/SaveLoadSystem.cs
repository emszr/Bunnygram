using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Nonogram.System
{
    public enum SaveLoad
    {
        DataList=0,
    }
    public static class SaveLoadSystem
    {
        public static bool SaveData(LevelManager data)
        {
            try
            {
                DataLevelList saveData = new DataLevelList(data);
                string json = JsonUtility.ToJson(saveData, false);
                PlayerPrefs.SetString("DataList", json);
                PlayerPrefs.Save();

                return true;
            }
            catch
            {
                Debug.LogError("Error occured during saving!");
                return false;
            }
        }
        


        public static DataLevelList LoadData()
        {
            try
            {

                DataLevelList levelData;
                string json = PlayerPrefs.GetString("DataList");
                levelData = JsonUtility.FromJson<DataLevelList>(json);
                return levelData;
            }
            catch
            {
                return null;
            }


        }
        public static bool DeleteData(SaveLoad key)
        {
            try
            {
                if (key == SaveLoad.DataList)
                {
                    PlayerPrefs.DeleteKey("DataList");
                }
                return true;

            }
            catch
            {
                return false;
            }

        }
    }
}
