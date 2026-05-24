
using System.Collections.Generic;
using StateMachine;
using UnityEngine;

public class TutorialUISettingsState : State<TutorialUIController>
{
    public TutorialUISettingsState(TutorialUIController stateMachine) : base(stateMachine) {}

    public override void Enter()
    {
        stateMachine.SettingsPanel.gameObject.SetActive(true);
        HashSet<int> changed = new HashSet<int>();
        foreach (var player in PlayerMovmant.players)
        {
            Debug.Log($"Player {player.index}");
            stateMachine.SettingsPanel.playerVolumeSliderNames[player.index].text = (string)player.stats.list["Name"];
            changed.Add(player.index);
        }

        for (int i = 0; i < 7; i++)
        {
            if(!changed.Contains(i)) stateMachine.SettingsPanel.playerVolumeSliderNames[i].text = $"player {i+1}";
        }
        
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public override void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape)) stateMachine.Dispose();
    }
    public override void Exit()
    {
        stateMachine.SettingsPanel.gameObject.SetActive(false);
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
