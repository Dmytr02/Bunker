using UnityEngine;
using UnityEngine.EventSystems;

public class TutorialUIPouseState : StateMachine.State<TutorialUIController>
{
    public TutorialUIPouseState(TutorialUIController stateMachine) : base(stateMachine) { }

    override public void Enter()
    {
        stateMachine.PousePanel.SetActive(true);
        
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        entry.callback.AddListener((arg0) => { stateMachine.SetState(new TutorialUISettingsState(stateMachine)); });
        
        stateMachine.SettingsButton.triggers.Add(entry);
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public override void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape)) stateMachine.Dispose();
        if(Input.GetKeyDown(KeyCode.S)) stateMachine.SetState(new TutorialUISettingsState(stateMachine));;
    }

    override public void Exit()
    {
        stateMachine.PousePanel.SetActive(false);
        stateMachine.SettingsButton.triggers.RemoveAll(entry => entry.eventID == EventTriggerType.PointerClick);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
