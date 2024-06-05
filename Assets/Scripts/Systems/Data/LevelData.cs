using System.Collections.Generic;

namespace BlockShift
{
    public class LevelData
    {
        public LevelState state;
        public string ID;
        public int levelMapCount;
        public int totalXp;
        public List<CellData> cellDatas;
        public LevelData(Level level,GridManager gridManager)
        {
            levelMapCount = level.levelMapCount;
            state = level.state;
            ID = level.ID;
            totalXp = level.totalXp;
            cellDatas = gridManager.CreateCellDatas();
        }

        public Level GetSavedLevel()
        {
            return new Level(state, levelMapCount, ID,totalXp);
        }

        public List<CellData> GetCellDatas()
        {
            return cellDatas;
        }

    }
}
