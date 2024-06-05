using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using Random = UnityEngine.Random;

namespace BlockShift
{
    public class GridManager : MonoBehaviour
    {
        [SerializeField] private GridBuilder builder;
        [SerializeField] private GameObject levelDiamond;
        [SerializeField] private GameObject dropPoint;
        public List<Cell> cellList;
        public List<Cell> popList; // list of blocks which are ready to pop
        public List<CellData> cellDataList; // the list which is used to check if there is saved grid (continuing game)
        private int minForPop = 2; // min neighbour blocks to pop
        private int numofcol; // number of col
        private int numofrow; // number of row

        void Start()
        {
            //IF NO DATA, THEN CALL INITIALIZE WITH RANDOM SEED OTHERWISE CREATE WITH DATA RECIEVED
            List<CellData> datas;
            datas = LevelManager.instance.GetSavedCellData();
            if (datas == null) 
            {
                InitializeBuilder();
            }
            else
            {
                if (datas.Count > 0)
                {
                    InitializeBuilderWithData(datas);
                   
                }
                else
                {
                    InitializeBuilder();
                }
            }

            LevelManager.instance.SetCellData(null);

        }

        //INITIALIZE THE GRID WITH RANDOM SEED
        public void InitializeBuilder() 
        {
            builder.Generate();
            cellList = new List<Cell>(builder.cellList);
            popList = new List<Cell>();
            numofcol = builder.numofcol;
            numofrow = builder.numofrow;
            GameManager.instance.ChangeGridState(GridState.Filled);
        }

        //INITIALIZE THE GRID WITH DATA
        public void InitializeBuilderWithData(List<CellData> datas) 
        {
            builder.GenerateFromData(datas);
            cellList = new List<Cell>(builder.cellList);
            popList = new List<Cell>();
            numofcol = builder.numofcol;
            numofrow = builder.numofrow;
            GameManager.instance.ChangeGridState(GridState.Filled);
        }
        
        // IF PLAYER HAS MOVES THEN IT POPS THE NEIGHBOURS WHICH IS HIGHER THAN MIN
        public void PopBlocks(Cell cell) 
        {
            if(CurrencyManager.instance.GetItemCount(CurrencyItemType.Move) <= 0){return;}
            popList.Add(cell);
            FindCells(cell);

            int multiplierCount = 0;
            int tempAmount  = popList.Count;
            popList = popList.Distinct().ToList();
            int amountRainbowInCell = FindRainbowInPopList();
            //Debug.Log("After distinct : " + popList.Count + "    Before distinct : " + tempAmount);
            if (popList.Count-amountRainbowInCell < minForPop+1) //IF THE TOTAL POP COUNT LESS THAN GIVEN THAN UNCHECK THOSE BLOCKS
            {
                ClearPopList();
                SetNoChosenForPop();
                return;
            }
            
            CurrencyManager.instance.TryToDecreaseCurrencyAmount(CurrencyItemType.Move, 1);
            BlockColor color=BlockColor.Blue;
            int amount=0;
            foreach (Cell tempCell in popList) //CHECK THE POPPED FOR ITEM IN POPLIST
            {
                Block block = tempCell.GetBlock();
                PoolItem poolItem = block.GetComponent<PoolItem>();

                if (block.color == BlockColor.Rainbow)
                {
                    RainbowGem gem = block.transform.GetComponent<RainbowGem>();
                    multiplierCount += gem.multiplier;
                    GameManager.instance.GetParticleManager().StopComboParticle(block);
                }
                else
                {
                    color = block.GetColor();
                    if (GameManager.instance.GetParticleManager()!= null)
                    {
                        GameManager.instance.GetParticleManager().PlayGemParticle(block);
                    }
                }
                
                block.transform.DOMove(levelDiamond.transform.position, 0.6f).OnStart(
                    () =>
                    {
                        block.transform.DOScale(Vector3.zero, 0.6f);
                    }
                ).OnComplete(() =>
                    PoolManager.instance.ResetPoolItem(poolItem));
                tempCell.popped = true;
                amount++;
            }
            ClearPopList();
            FallBlocks();
            SetNoChosenForPop();
            if (amountRainbowInCell == 0)
            {
                CalculateEarnedXp(color,amount,1);
            }
            else
            {
                CalculateEarnedXp(color,amount,multiplierCount);
            }
        }
        
        // DO FALL AFTER POP
        public void FallBlocks() 
        {
            GameManager.instance.ChangeGameState(GameState.Falling);
            int counter = 0;
            Sequence sequence = DOTween.Sequence();
            int totalLinesFall = 0;
            for (int col = 0; col < numofcol; col++)
            {
                int index = 0;
                int colAmountFall = FindAmountFall(col, numofrow); // FIND THE COL WITH AMOUNT TO FALL (ENTIRE COL)
                
                if(colAmountFall ==0){continue;} // IF NO POPPED IN THE COL THEN GO NEXT COL

                while (index < numofrow)
                {
                    Cell cell = cellList[index * numofcol + col];
                    Block block = cell.GetBlock();
                    
                    int amount = FindAmountFall(col, index); // CHECK THE AMOUNT FALL UNDER THE BLOCK
                    if (amount == 0) 
                    {
                        index++;
                        continue;
                    }
                    
                    if (cell.popped) //IF BLOCK IS POPPED GO UP FOR NOT POPPED BLOCK
                    {
                        index++;
                        continue;
                    }

                    totalLinesFall++; //COUNTER TO CHECK IF FULL GRID IS POPPED
                    
                    //DOING SWITCH TO NEXT ELEMENT
                    Cell fallCell = cellList[(index - amount) * numofcol + col];
                    Block tempBlock = fallCell.GetBlock();
                    cell.SetBlock(tempBlock);
                    fallCell.SetBlock(block);
                    
                    bool tempPopped = cell.popped;
                    bool popped = fallCell.popped;
                    cell.popped = popped;
                    fallCell.popped = tempPopped;

                    Tweener tween = block.transform.DOLocalMove(fallCell.transform.localPosition, 0.3f);
                    
                    if (counter == 0)
                    {
                        sequence.Append(tween);
                    }
                    else
                    {
                        sequence.Join(tween);
                    }

                    //USE DOTWEEN TO MOVE THE EMPTY CELL
                    
                    
                    index++;
                    counter++;
                }

            }

            if (totalLinesFall == 0) // IT MEANS THAT ENTIRE GRID POPPED AND THERE IS NOTHING TO FALL
            {
                GameManager.instance.ChangeGridState(GridState.Empty);
            }

            if (counter > 0)
            {
                sequence.OnComplete(() =>
                {
                    GameManager.instance.ChangeGridState(GridState.Empty);
                    GameManager.instance.ChangeGameState(GameState.Filling);
                });
            }

        }
        
        // FIND ALGORITHM FOR NEIGHBOURS WITH SAME COLOR (RIGHT,LEFT,UP,DOWN) BLOCKS
        public void FindCells(Cell cell) 
        {
            int neighboursCount = GetNeighbours(cell).Count; // changed tempList
            
            if (neighboursCount <= 0)
            {
                return;
            }
            foreach (Cell chosenCell in GetNeighbours(cell)) //MARK THE ALL NEIGHBOURS AND CHECK FOR THEIR NEIGHBOURS
            {
                if (!chosenCell.chosenForPop)
                {
                    cell.chosenForPop = true;
                    popList.Add(chosenCell);
                    BlockColor color = chosenCell.GetBlock().color;
                    if (color == BlockColor.Rainbow)
                    {
                        continue;
                    }
                    FindCells(chosenCell);
                    
                    
                }
            }
        }

        //CHECK THE EMPTY BLOCK CELLS UNDER TARGET BLOCK
        private int FindAmountFall(int col,int targetRow) 
        {
            //IF TOTAL COL NUMBER IS USED FOR TARGET ROW THEN IT GIVES THE ALL EMPTY BLOCK CELL IN THAT ROW
            int counter = 0;
            for (int row = 0; row < numofrow; row++)
            {
                if (targetRow == row)
                {
                    break;
                }

                if (cellList[row * numofcol + col].popped)
                {
                    counter++;
                }
            }
            return counter;
        }

        private int FindRainbowInPopList()
        {
            int amount = 0;
            foreach (Cell cell in popList)
            {
                Block block = cell.GetBlock();
                if (block.color == BlockColor.Rainbow)
                {
                    amount++;
                }
            }

            return amount;
        }



        // GET THE LIST OF NEIGHBOURS FOR GIVEN BLOCK (RIGHT,LEFT,UP,DOWN)
        private List<Cell> GetNeighbours(Cell cell) 
        {
            int positionY = cell.posY;
            int positionX = cell.posX;
            List<Cell> list = new List<Cell>();

            for (int x = positionX  - 1; x <= positionX  + 1; x++) // UP AND DOWN CHECK
            {
                if ((x == positionX) || x < 0 || x >= numofrow)
                {
                    continue;
                }
                Cell tempCell = cellList[x * numofcol + positionY];
                if (!tempCell)
                {
                    continue;
                }
                if (!tempCell.chosenForPop && (cell.GetBlock().color == tempCell.GetBlock().color || tempCell.GetBlock().color == BlockColor.Rainbow)) // if color matches
                {
                    list.Add(tempCell);
                }
            }
            
            for (int y = positionY - 1; y <= positionY + 1; y++) // UP AND DOWN CHECK
            {
                if ((y == positionY) || y < 0 || y >= numofcol)
                {
                    continue;
                }
         
                Cell tempCell = cellList[positionX * numofcol + y];
                if (!tempCell)
                {
                    continue;
                }

                if (!tempCell.chosenForPop && (cell.GetBlock().color == tempCell.GetBlock().color || tempCell.GetBlock().color == BlockColor.Rainbow)) // if color matches
                {
                    list.Add(tempCell);
                }
            }
            return list;
        }
        
        //REPLACE ALL THE BLOCKS WITH NEW BLOCKS FROM POOL
        public void Respin() 
        {
            //RESET ALL ITEMS IN GRID TO POOL THEN GENERATE NEW SEED AND GENERATE THE GRID WITH THAT SEED
            if ((GameManager.instance.currentGameState.HasFlag(GameState.Playing)&& 
                CurrencyManager.instance.GetItemCount(CurrencyItemType.Spin) > 0))
            {
                DOTween.KillAll();
                GameManager.instance.ChangeGameState(GameState.Loading);
                ClearPopped(); 
                
                LevelManager.instance.level.levelMapCount = Random.Range(LevelManager.instance.levelCount+100
                    ,Int32.MaxValue); //TO-DO: CHANGE IT WITH NEW VARIABLES CALLED GRIDMAP VALUE

                Sequence sequence = DOTween.Sequence();
                int counter = 0;
                
                foreach (Cell cell in cellList)
                {
                    Block block = cell.GetBlock();
                    Tweener tween = block.transform.DOLocalMoveY(dropPoint.transform.position.y, 1f);
                    if (counter == 0)
                    {
                        sequence.Append(tween);
                        counter++;
                        continue;
                    }

                    sequence.Join(tween);

                }

                sequence.OnComplete(() =>
                {
                    PoolManager.instance.ResetAllPools();

                    builder.GenerateSeed();
                    builder.GenerateCells();
                    builder.GenerateBlocksOnCell();
                    builder.ArrangeBlock();

                    CurrencyManager.instance.TryToDecreaseCurrencyAmount(CurrencyItemType.Spin, 1);
                });

            }
           
        }


        public void SpinNextLevel()
        {
            DOTween.KillAll();
            GameManager.instance.ChangeGameState(GameState.Loading);
            ClearPopped(); 
                
            LevelManager.instance.level.levelMapCount = LevelManager.instance.levelCount; //TO-DO: CHANGE IT WITH NEW VARIABLES CALLED GRIDMAP VALUE
            
            
            
            Sequence sequence = DOTween.Sequence();
            int counter = 0;
                
            foreach (Cell cell in cellList)
            {
                Block block = cell.GetBlock();
                Tweener tween = block.transform.DOLocalMoveY(dropPoint.transform.position.y, 1f);
                if (counter == 0)
                {
                    sequence.Append(tween);
                    counter++;
                    continue;
                }

                sequence.Join(tween);

            }

            sequence.OnComplete(() =>
            {
                PoolManager.instance.ResetAllPools();

                builder.GenerateSeed();
                builder.GenerateCells();
                builder.GenerateBlocksOnCell();
                builder.ArrangeBlock();
                LevelManager.instance.SaveLevel();
                LevelManager.instance.LoadNextLevel();
            });
            
        }

        public void CheckPositionOnCell(GameState state)
        {
            if (cellList == null || cellList.Count == 0)
            {
                return;
            }

            if (!state.HasFlag(GameState.Playing))
            {
                return;
            }
            
            float distance;
            Sequence sequence = DOTween.Sequence();
            int counter = 0;
            foreach (Cell cell in cellList)
            {
                Block block = cell.block;
                if (!block)
                {
                    return;
                }

                distance = Vector3.Distance(block.transform.position, cell.transform.position);
                if (!Mathf.Approximately(distance, 0f))
                {
                    Tweener tween = block.transform.DOLocalMove(cell.transform.localPosition, 0.2f);
                    if (counter == 0)
                    {
                        sequence.Append(tween);
                    }
                    else
                    {
                        sequence.Join(tween);
                    }
                    counter++;
                }
            }
        }

        public void CalculateEarnedXp(BlockColor color, int amountBlock, int multiplier)
        {
            int totalXp;
            if (color == BlockColor.Blue || color == BlockColor.Green || color == BlockColor.Yellow)
            {
                totalXp = 20 * amountBlock;
            }
            else if (color == BlockColor.Purple || color == BlockColor.Red)
            {
                totalXp = 25 * amountBlock;
            }
            else
            {
                totalXp = 30 * amountBlock;
            }
            
            GameManager.instance.OnEarnedXp.Invoke(totalXp,multiplier);

        }



        public void ClearPopped()
        {
            //CLEAR ALL CHECKS FOR POPPED BLOCKS BEFORE SEND TO POOL
            foreach (Cell cell in cellList)
            {
                if (cell.popped)
                {
                    PoolManager.instance.ResetPoolItem(cell.GetBlock().GetComponent<PoolItem>());
                    cell.popped = false;
                    cell.chosenForPop = false;
                }
            }
        }

        //UNMARK ALL THE BLOCKS FOR CHOSEN TO POP
        public void SetNoChosenForPop() 
        {
            foreach (Cell cell in cellList)
            {
                cell.chosenForPop = false;
            }
        }
        
        //EMPTY THE LIST OF CHOSEN BLOCKS FOR POP
        public void ClearPopList() 
        {
            popList.Clear();
        }

        
        public List<CellData> GetCellDatas()
        {
            return cellDataList;
        }
        
        // CREATE A LIST CONTAINED THE DATA OF BLOCKS IN GAMEPLAY
        public List<CellData> CreateCellDatas() 
        {
            cellDataList = new List<CellData>();
            foreach (Cell cell in cellList)
            {
                cellDataList.Add(cell.GetCellData());
            }
            return cellDataList;
        }
        
        public void UpdateBlockDatas()
        {
            cellDataList.Clear();
            foreach (Cell cell in cellList)
            {
                cellDataList.Add(cell.GetCellData());
            }
        }
        public void SetBlockDataList(List<CellData> cellDatas)
        {
            cellDataList = cellDatas;
        }
        

        private void OnEnable()
        {
            GameManager.instance.OnCellTouched += PopBlocks;
            GameManager.instance.OnGameStateChanged += CheckPositionOnCell;
            GameManager.instance.OnLevelUp += SpinNextLevel;
            GameManager.instance.SetGridManager(this);
        }

        private void OnDisable()
        {
            GameManager.instance.OnCellTouched -= PopBlocks;
            GameManager.instance.OnGameStateChanged -= CheckPositionOnCell;
            GameManager.instance.OnLevelUp -= SpinNextLevel;
        }
    }
}

