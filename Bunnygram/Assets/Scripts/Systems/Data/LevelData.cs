using System.Collections.Generic;

namespace Nonogram
{
    public class LevelData
    {
        public LevelState state;
        public string ID;
        public int currentLevel;
        public int lives;
        public List<CellData> cellList;
        public int earnedStars;

        public LevelData(LevelManager levelManager,GridManager gridManager)
        {
            currentLevel = levelManager.GetCurrentLevel().levelCount;
            state = levelManager.GetCurrentLevel().state;
            ID = levelManager.GetCurrentLevel().ID;
            lives = levelManager.GetCurrentLevel().lives;
            cellList = gridManager.GetCellDataList();
            earnedStars = levelManager.GetCurrentLevel().totalStars;
        }

        public Level GetSavedLevel()
        {
            return new Level(state, currentLevel, ID, lives,earnedStars);
        }

    }
}

