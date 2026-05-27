using UnityEngine;

public class InventoryButton : MonoBehaviour
{
    [SerializeField] public CardsController CardsController;
    public void Click()
    {
        CardsController.isOpend = !CardsController.isOpend;
    }
}
