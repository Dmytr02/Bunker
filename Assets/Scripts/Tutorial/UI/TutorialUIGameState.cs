using Unity.VisualScripting;
using UnityEngine;

public class TutorialUIGameState : StateMachine.State
{
    public TutorialUIGameState(TutorialUIController stateMachine) : base(stateMachine) { }

    public override void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            stateMachine.SetState(new TutorialUIPouseState(stateMachine as TutorialUIController));
        }
        
        if (Input.GetKeyDown(KeyCode.T))
        {
            stateMachine.SetState(new TutorialUIChatState(stateMachine as TutorialUIController));
        }
    }
    override public void Enter()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    override public void Exit()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
