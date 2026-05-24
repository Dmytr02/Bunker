using Unity.VisualScripting;
using UnityEngine;

public class TutorialUIGameState : StateMachine.State<TutorialUIController>
{
    public TutorialUIGameState(TutorialUIController stateMachine) : base(stateMachine) { }

    public override void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            stateMachine.SetState(new TutorialUIPouseState(stateMachine));
        }
        
        if (Input.GetKeyDown(stateMachine.chatKey.key))
        {
            stateMachine.SetState(new TutorialUIChatState(stateMachine));
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
