using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace BlockShift
{
    public enum SaveLoad
    {
        LastLevel=0,
    }
    public static class SaveLoadSystem
    {
        public static bool SaveData(Level data,GridManager gridManager)
        {
            try
            {
                LevelData levelData = new LevelData(data,gridManager);
                string json = JsonUtility.ToJson(levelData, false);
                PlayerPrefs.SetString("LastLevel", json);
                PlayerPrefs.Save();

                return true;
            }
            catch
            {
                Debug.LogError("Error occured during saving!");
                return false;
            }
        }
        


        public static LevelData LoadData()
        {
            try
            {

                LevelData levelData;
                string json = PlayerPrefs.GetString("LastLevel");
                levelData = JsonUtility.FromJson<LevelData>(json);
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
                if (key == SaveLoad.LastLevel)
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