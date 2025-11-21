using UnityEngine;

public class UIController : StateMachine.StateMachine
{
    public GameObject PousePanel;
    private void Start()
    {
        Begin(new UIGameState(this));
    }
}
