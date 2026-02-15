using UnityEngine;
using UnityEngine.EventSystems;

public class UIPouseState : StateMachine.State
{
    public UIPouseState(UIController stateMachine) : base(stateMachine) { }

    override public void Enter()
    {
        (stateMachine as UIController).PousePanel.SetActive(true);
        
        EventTrigger.Entry  entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        entry.callback.AddListener((arg0) => {stateMachine.SetState(new UISettingsState(stateMachine as UIController)); });
        
        (stateMachine as UIController).SettingsButton.triggers.Add(entry);
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public override void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape)) stateMachine.Dispose();
    }

    override public void Exit()
    {
        (stateMachine as UIController).PousePanel.SetActive(false);
        (stateMachine as UIController).SettingsButton.triggers.Clear();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
