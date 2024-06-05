using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.Serialization;
namespace BlockShift
{
    [Flags]
    public enum LevelState
    {
        Completed = 1,
        UnCompleted = 2
    }

    [Serializable]
    public class Level
    {
        public LevelState state;

        //public LevelDifficulty difficulty;
        public string ID;
        public int levelMapCount;
        public int totalXp;
        public Level(LevelState state,int levelMap, string id,int totalXp)
        {
            this.state = state;
            levelMapCount = levelMap;
            ID = id;
            this.totalXp = totalXp;
        }
    }

    public class LevelManager : Singleton<LevelManager>
    {
        public Level level;
        public int levelCount = 1;
        public int totalXpNeeded;
        public int xpEarned;
        public List<CellData> savedCellDatas;
        private void Start()
        {
            LevelData data = SaveLoadSystem.LoadData();
            if (data==null)
            {
                Debug.Log("No last saved games");
                SetLevel(LevelState.UnCompleted, 1, GenerateLevelID(1),0);
            }
            else
            { 
                Level tempLevel = data.GetSavedLevel();
                SetLevel(tempLevel.state,tempLevel.levelMapCount,tempLevel.ID,tempLevel.totalXp);
                savedCellDatas = data.GetCellDatas();
            }
        }

        public void SaveLevel()
        {
            Level tempLevel;
            if (level == null)
            {
                return;
            }
            
            tempLevel = GetCopyLevel();

            SaveLoadSystem.SaveData(tempLevel, GameManager.instance.GetGridManager());
        }

        public Level GetCopyLevel()
        {
            Level tempLevel = new Level(level.state,level.levelMapCount,GenerateLevelID(levelCount),level.totalXp);
            return tempLevel;
        }

        public Level GetCurrentLevel()
        {
            return level;
        }

        public void SetLevel(LevelState state,int levelMapCount, string id,int totalXp)
        {
            int curr_level;
            string str =  Regex.Match(id, @"\d+").Value;
            Int32.TryParse(str, out curr_level);
            if (level == null)
            {
                level = new Level(state, levelMapCount,id,totalXp);
                
            }
            else
            {
                SetLevelItems(state,levelMapCount,id,totalXp);
            }

            levelCount = curr_level;
            xpEarned = 0;
            level.levelMapCount = levelMapCount;
            totalXpNeeded = GenerateXp(curr_level);
        }
        public void SetLevelItems(LevelState state, int levelMapCount, string id,int totalXp)
        {
            level.state = state;
            level.levelMapCount = levelMapCount;
            level.ID = id;
            level.totalXp = totalXp;
        }
        public string GenerateLevelID(int count)
        {
            string id = "Level_" + count;
            return id;
        }

        public int GenerateXp(int levelCount)
        {
            int totalxp = 0;

            if (levelCount < 10)
            {
                totalxp = levelCount * 1000;
            }
            else if (levelCount < 20)
            {
                totalxp = (levelCount % 10) * 2000 + 10000;
            }
            else if (levelCount < 30)
            {
                totalxp = (levelCount % 10) * 3000 + 20000;
            }
            else
            {
                totalxp = 50000;
            }

            return totalxp;
        }

        public void GoToNextLevel()
        {
            levelCount++;
            totalXpNeeded = GenerateXp(levelCount);
            level.totalXp = 0;
            UIManager manager = GameManager.instance.GetUIManager();
            if (!manager)
            {
                Debug.Log("Set Mask stat21312312321e");
                GameManager.instance.OnLevelUp?.Invoke();
            }
            else
            {
                Debug.Log("Set Mask state");
                manager.SetMaskState(manager.GetMainMask(),true,GameManager.instance.OnLevelUp);    
            }
        }

        public void LoadNextLevel()
        {
            DOTween.KillAll();
            PoolManager.instance.ResetAllPools();
            SceneManager.LoadScene(0);
        }

        public int GetTotalXp()
        {
            return level.totalXp;
        }

        public void InsertEarnedXp(int earned,int multiplier)
        {
            xpEarned = earned*multiplier;

            int tempXp = level.totalXp + xpEarned;

            if (tempXp >= totalXpNeeded) // LEVELUP
            {
                GoToNextLevel();
            }
            else
            {
                level.totalXp += xpEarned;
            }

            
        }

        public List<CellData> GetSavedCellData()
        {
            return savedCellDatas;
        }

        public void SetCellData(List<CellData> datas)
        {
            savedCellDatas = datas;
        }

        private void OnApplicationQuit()
        {
            Debug.Log("Quit");
            SaveLevel();
        }

        private void OnEnable()
        {
            GameManager.instance.OnEarnedXp += InsertEarnedXp;
        }
        private void OnDisable()
        {
            GameManager.instance.OnEarnedXp -= InsertEarnedXp;
        }
    }
}