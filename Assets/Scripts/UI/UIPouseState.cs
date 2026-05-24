using UnityEngine;
using UnityEngine.EventSystems;

public class UIPouseState : StateMachine.State<UIController>
{
    public UIPouseState(UIController stateMachine) : base(stateMachine) { }
    private EventTrigger.Entry entry;
    override public void Enter()
    {
        stateMachine.PousePanel.SetActive(true);
        
        entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        entry.callback.AddListener((arg0) => {stateMachine.SetState(new UISettingsState(stateMachine)); });
        
        stateMachine.SettingsButton.triggers.Add(entry);
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public override void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape)) stateMachine.Dispose();
    }

    override public void Exit()
    {
        stateMachine.PousePanel.SetActive(false);
        stateMachine.SettingsButton.triggers.Remove(entry); //RemoveAll(entry => entry.eventID == EventTriggerType.PointerClick);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
