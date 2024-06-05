using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlockShift
{
    public enum BlockColor
    {
        Red=0,
        Yellow=1,
        Green=2,
        Blue=3,
        Purple =4,
        Rainbow=5,
    }

    public class Block : MonoBehaviour
    {
        public BlockColor color;
        public GameObject particle;
        public int posX;
        public int posY;
        public int multiplier;
        
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

        public BlockColor GetColor()
        {
            return color;
        }
        public void SetColor(BlockColor color)
        {
            this.color = color;
        }
        
  
    }
}

