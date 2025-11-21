using Unity.VisualScripting;
using UnityEngine;

public class UIGameState : StateMachine.State
{
    public UIGameState(StateMachine.StateMachine stateMachine) : base(stateMachine) { }

    public override void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            stateMachine.SetState(new UIPouseState(stateMachine));
        }
    }
}
