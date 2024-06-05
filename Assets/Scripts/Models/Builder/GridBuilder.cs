using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using DG.Tweening;
using UnityEditor;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;
using Sequence = DG.Tweening.Sequence;

namespace BlockShift
{
    
    public class GridBuilder : MonoBehaviour
    {
        #region Serialized Field
        [SerializeField] private Block block;
        [SerializeField] private GameObject grid;
        [SerializeField] private PoolItemType[] typesOfBlock;
        [SerializeField] private GameObject spawnPoint;
        [SerializeField] private SpriteRenderer gridRenderer;
        [SerializeField] private SpriteRenderer blockRenderer;
        [SerializeField] private SpriteRenderer cellRenderer;
        [SerializeField] private Camera mainCamera;
        [SerializeField] private float dropDelayTime; 
        [SerializeField] private List<GameObject> itemPrefabs;
        #endregion
        
        
        private float _scaleRateWidth;
        private float _scaleRateHeight;

        
        public int numofcells;
        public int numofcol;
        public int numofrow;
        public List<Cell> cellList;
        public Cell prefabCell;
        
        private void Start()
        {
            numofcells = numofcol * numofrow;
            cellList = new List<Cell>();
        }

        public void Generate()
        {
            Random.InitState(GenerateSeed());
            ScaleGrid();
            GenerateCells();
            GenerateBlocks();
            GenerateBlocksOnCell();
            ArrangeBlock();
        }

        public void GenerateFromData(List<CellData> datas)
        {
            Random.InitState(GenerateSeed());
            ScaleGrid();
            GenerateCells();
            GenerateBlocks();
            GenerateGridFromData(datas);
            ArrangeBlock();
        }

        public void GenerateGridFromData(List<CellData> datas)
        {
            if (datas.Count == 0)
            {
                return;
            }
            Block tempBlock;
            for (int x = 0; x < datas.Count; x++)
            {
                tempBlock = GenerateBlockWithData(datas[x].color, x,datas[x].multipilier);
                if (!tempBlock)
                {
                    tempBlock = GenerateBlockFromPool(x);
                }
                cellList[x].SetBlock(tempBlock);
            }
        }


        public void ScaleGrid()
        {
            float newGridScale = (float)((float)mainCamera.pixelWidth / (float)mainCamera.pixelHeight);

            if (newGridScale < Constants.DEFAULTSCREENRATIO)
            {
                newGridScale /= Constants.DEFAULTSCREENRATIO;
                grid.gameObject.transform.localScale = new Vector3(newGridScale, newGridScale);
            }
         
        }

        public void GenerateBlocks()
        {
            if (PoolManager.instance)
            {
                for (int x = 0; x < itemPrefabs.Count; x++)
                {
                    int availableCount = PoolManager.instance
                        .GetObjectPool(itemPrefabs[x].GetComponent<PoolItem>().GetPoolItemType()).GetAvailableCount();
                    int needCount = numofcells;
                    for (int i = availableCount; i < numofcells; i++)
                    {
                        
                        GameObject chosenObj = itemPrefabs[x];
                        GameObject prefabObj = Instantiate(chosenObj, grid.transform);

                        PoolManager.instance.AddToAvailable(prefabObj.GetComponent<PoolItem>());
                    }
                }
                
            }
        }

        public void GenerateCells()
        {
            cellList.Clear();
            for (int i = 0; i < numofcells; i++)
            {
                Cell cell;
                PoolItem itemPool = PoolManager.instance.GetFromPool(PoolItemType.Cell, grid.transform);
                if (!itemPool)
                {
                    cell = Instantiate(prefabCell, grid.transform).GetComponent<Cell>();
                    PoolManager.instance.AddToUsing(cell.GetComponent<PoolItem>(),grid.transform);
                }
                else
                {
                    cell = itemPool.GetComponent<Cell>();
                }
                cell.name = "Cell_" + (i / numofcol) + "_" + (i % numofcol);
                cellList.Add(cell);

            }
        }

        public void GenerateBlocksOnCell()
        {
            for (int i = 0; i < numofcells; i++)
            {
                cellList[i].SetBlock(GenerateBlockFromPool(i));
            }
            GameManager.instance.ChangeGridState(GridState.Filled);
        }
        
        public void ArrangeBlock()
        {
            float gridWidth = gridRenderer.bounds.size.x*0.95f;
            float gridHeight = gridRenderer.bounds.size.y*0.95f;

            float originalBlockWidth = blockRenderer.bounds.size.x;
            float originalBlockHeight = blockRenderer.bounds.size.y;
            
            float newBlockWidth = gridWidth / numofcol;
            float newBlockHeight = gridHeight / numofrow;
            
            float originalCellWidth = cellRenderer.bounds.size.x;
            float originalCellHeight = cellRenderer.bounds.size.y;
            
            float newCellWidth = gridWidth / numofcol;
            float newCellHeight = gridHeight / numofrow;
            
            float scaleCellWidth = newCellWidth / originalCellWidth / grid.transform.localScale.x * prefabCell.transform.localScale.x;
            float scaleCellHeight = newCellHeight / originalCellHeight / grid.transform.localScale.y * prefabCell.transform.localScale.y;

            _scaleRateWidth = newBlockWidth / originalBlockWidth / grid.transform.localScale.x * block.transform.localScale.x;
            _scaleRateHeight = newBlockHeight / originalBlockHeight / grid.transform.localScale.y * block.transform.localScale.y;

            _scaleRateWidth = _scaleRateWidth * 0.95f;
            _scaleRateHeight = _scaleRateHeight * 0.95f;

            Sequence sequence = DOTween.Sequence();
            
            for (int row = 0; row < numofrow;row++)
            {
                for (int col = 0; col < numofcol; col++)
                {
                    int val = numofcol * row + col;
                    if (val >= cellList.Count)
                    {
                        break;
                    }
                    //SET CELL
                    Vector3 cellPosition = new Vector3(grid.transform.position.x - (gridWidth / 2) + (newCellWidth / 2) + (newCellWidth * col),
                        grid.transform.position.y - (gridHeight / 2) + (newCellHeight / 2) + (newCellHeight * row));

                    Cell cell = cellList[row * numofcol + col];
                    cell.posX = row;
                    cell.posY = col;
                    cell.transform.position = cellPosition;
                    cell.transform.localScale = new Vector3(scaleCellWidth, scaleCellHeight,1);
                    //cellList[row * numofcol + col].transform.GetComponent<SpriteRenderer>().

                    Block block = cell.GetBlock();
                    block.posX = cell.posX;
                    block.posY = cell.posY;
                    // SET BLOCK

                    Vector3 spawnPoint = new Vector3(cellPosition.x, this.spawnPoint.transform.position.y+row);

                   
                    block.transform.position = spawnPoint;
                    block.transform.localScale = new Vector3(_scaleRateWidth, _scaleRateHeight,_scaleRateHeight);
                    Tweener tween;
                    if (block.color == BlockColor.Rainbow)
                    {
                        tween =  block.transform.DOMove(cellPosition, 1f).SetDelay(0.002f*row+0.001f*col).OnComplete(
                            ()=>GameManager.instance.GetParticleManager().PlayComboParticleOnItem(block));
                    }
                    else
                    {
                        tween =  block.transform.DOMove(cellPosition, 1f).SetDelay(0.002f*row+0.001f*col);
                    }
                    
                    if (row == 0 && col == 0)
                    {
                        sequence.Append(tween);
                    }
                    else
                    {
                        sequence.Join(tween);
                    }

                }
            }
            
            sequence.OnComplete(
                ()=>GameManager.instance.ChangeGameState(GameState.Playing));
        }

        public void FillGrid(GridState state)
        {
            
            
            if (state == GridState.Filled)
            {
                return;
            }
            List<Cell> list = GameManager.instance.GetGridManager().cellList;
            float gridWidth = gridRenderer.bounds.size.x*0.95f;
            float gridHeight = gridRenderer.bounds.size.y*0.95f;

         
            float originalCellWidth = blockRenderer.bounds.size.x;
            float originalCellHeight = blockRenderer.bounds.size.y;

            float newCellWidth = gridWidth / numofcol;
            float newCellHeight = gridHeight / numofrow;

            _scaleRateWidth = newCellWidth / originalCellWidth / grid.transform.localScale.x * block.transform.localScale.x;
            _scaleRateHeight = newCellHeight / originalCellHeight / grid.transform.localScale.y * block.transform.localScale.y;

            int counter = 0;
            Sequence sequence = DOTween.Sequence();
            
            for (int row = 0; row < numofrow; row++)
            {
                for (int col = 0; col < numofcol; col++)
                {
                    int index = row * numofcol + col;

                    Cell cell = list[index];

                    if (cell.popped)
                    {
                        cell.popped = false;
                        cell.chosenForPop = false;
                        Block blockFilled = GenerateBlockFromPool(index);

                        Vector3 spawnPoint = new Vector3(cell.transform.localPosition.x,
                            this.spawnPoint.transform.position.y + row);


                        blockFilled.transform.position = spawnPoint;
                        blockFilled.transform.localScale = new Vector2(_scaleRateWidth, _scaleRateHeight);
                        list[index].SetBlock(blockFilled);
                        
                        
                        Tweener tween;
                        if (blockFilled.color == BlockColor.Rainbow)
                        {
                            tween = blockFilled.transform.DOLocalMove(cell.transform.localPosition, 0.3f).OnComplete(
                                ()=>GameManager.instance.GetParticleManager().PlayComboParticleOnItem(blockFilled));
                        }
                        else
                        {
                            tween = blockFilled.transform.DOLocalMove(cell.transform.localPosition, 0.3f);
                        }
                        
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

                sequence.OnComplete(() =>
                {
                    GameManager.instance.ChangeGameState(GameState.Playing);
                    GameManager.instance.ChangeGridState(GridState.Filled);
                });
            }
            
        }

        public Block GenerateBlockFromPool(int index)
        {
            int randVal = Random.Range(0, 100);
            PoolItemType type = GenerateWithPossibility(randVal);
            PoolItem poolItem = PoolManager.instance.GetFromPool(type, grid.transform);
            GameObject blockobject = poolItem.gameObject;
            blockobject.name = "Block-" + index;
            Block blockval;
            
            if (type == PoolItemType.RainbowBlock)
            {
                RainbowGem rainbowGem = blockobject.GetComponent<RainbowGem>();
                int multiplier = GenerateRandomMultiplier();
                rainbowGem.ChangeMultiplier(multiplier);
                return rainbowGem;
            }

            blockval = blockobject.GetComponent<Block>();
            return blockval;
        }

        public Block GenerateBlockWithData(BlockColor color,int index,int multiplier)
        {
            PoolItemType type;
            PoolItem poolItem;
            if (color == BlockColor.Blue)
            {
                type = PoolItemType.BlueBlock;
            }
            else if (color == BlockColor.Green)
            {
                type = PoolItemType.GreenBlock;
            }
            else if (color == BlockColor.Yellow)
            {
                type = PoolItemType.YellowBlock;
            }
            else if (color == BlockColor.Red)
            {
                type = PoolItemType.RedBlock;
            }
            else if (color == BlockColor.Rainbow)
            {
                type = PoolItemType.RainbowBlock;
            }
            else
            {
                 return null;
            }
            poolItem = PoolManager.instance.GetFromPool(type, grid.transform);
            GameObject blockobject = poolItem.gameObject;
            blockobject.name = "Block-" + index;
            Block blockval = blockobject.GetComponent<Block>();
            if (type == PoolItemType.RainbowBlock)
            {
                blockval.GetComponent<RainbowGem>().ChangeMultiplier(multiplier);
            }

            return blockval;
        }

        public PoolItemType GenerateWithPossibility(int randVal)
        {
            PoolItemType type;
            if (randVal < 22)
            {
                type = PoolItemType.BlueBlock;
            }
            else if (randVal < 44)
            {
                type = PoolItemType.GreenBlock;
            }
            else if (randVal < 66)
            {
                type = PoolItemType.YellowBlock;
            }
            else if (randVal < 82)
            {
                type = PoolItemType.PurpleBlock;
            }
            else if (randVal < 98)
            {
                type = PoolItemType.RedBlock;
            }
            else
            {
                type = PoolItemType.RainbowBlock;
            }

            return type;
        }
        


        public int GenerateRandomMultiplier()
        {
            int multiplier;
            int randValue = Random.Range(0, 100);
            if (randValue < 55)
            {
                multiplier = 2;
            }
            else if (randValue < 75)
            {
                multiplier = 5;
            }
            else if (randValue < 85)
            {
                multiplier = 10;
            }
            else if (randValue < 91)
            {
                multiplier = 12;
            }
            else if (randValue < 95)
            {
                multiplier = 15;
            }
            else if (randValue < 98)
            {
                multiplier = 25;
            }
            else
            {
                multiplier = 50;
            }

            return multiplier;
        }

        public int GenerateSeed()
        {
            var temp = Random.state;
            Random.InitState(LevelManager.instance.GetCurrentLevel().levelMapCount);
            GameManager.instance.seedGenerator = Random.state;
            Random.state = GameManager.instance.seedGenerator;
            int generatedSeed = Random.Range(int.MinValue, int.MaxValue);
            Random.state = temp;
            return generatedSeed;
        }
        
        private void OnEnable()
        {
            GameManager.instance.OnGridChanged += FillGrid;
            GameManager.instance.SetGridBuilder(this);
        }

        private void OnDisable()
        {
            GameManager.instance.OnGridChanged -= FillGrid;
        }
    }

    
}

