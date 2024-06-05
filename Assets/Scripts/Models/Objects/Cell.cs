using System;
using Unity.VisualScripting;
using UnityEngine;

namespace BlockShift
{
    [Serializable]
    public class CellData
    {
        public BlockColor color;
        public int multipilier;
        public int posX;
        public int posY;

        public CellData(BlockColor color,int posX,int posY,int multiplier)
        {
            this.color = color;
            this.posX = posX;
            this.posY = posY;
            this.multipilier = multiplier;
        }
    }
    public class Cell : MonoBehaviour
    {
        public Block block;
        public bool chosenForPop;
        public bool popped;
        public int posX;
        public int posY;
        public CellData GetCellData()
        {
            return new CellData(block.color, posX, posY,block.multiplier);
        } 
        
        
        public Block GetBlock()
        {
            return block;
        }
        
        public void SetBlock(Block block)
        {
            this.block = block;
            this.block.SetPositionX(posX);
            this.block.SetPositionY(posY);
        }
        
        public int GetPositionX()
        {
            return posX;
        }

        public int GetPositionY()
        {
            return posY;
        }
        public void SetPositionX(int posX)
        {
            this.posX = posX;
        }

        public void SetPositionY(int posY)
        {
            this.posY = posY;
        }

    }
}