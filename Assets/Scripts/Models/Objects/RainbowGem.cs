using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace BlockShift
{
    public class RainbowGem : Block
    {
        public TextMeshPro textMeshPro;
        // Start is called before the first frame update
        void Start()
        {
            textMeshPro.text = multiplier + "X";
        }

        public void ChangeMultiplier(int multiplier)
        {
            this.multiplier = multiplier;
            textMeshPro.text = multiplier + "X";
        }

    }

}
