using UnityEngine;
using UnityEngine.UI;

namespace BlockShift
{
    public class DiceButton : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            GridManager gridManager = GameManager.instance.GetGridManager();
            if (!gridManager)
            {
                return;
            }
            Button diceButton = GetComponent<Button>();
            diceButton.onClick.RemoveAllListeners();
            diceButton.onClick.AddListener(()=> gridManager.Respin());
        }
    
    }

}
