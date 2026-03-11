using System;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class RandomizeCard : Card
{
    [SerializeField] TMP_Text cardName;
    public string Stat = "";
    private static readonly string[] ststsList = {"Profession", "Age", "Experience", "Healthe", "Phobias", "Hobby", "Personality"};
    private void Start()
    {
        Stat = ststsList[Random.Range(0, ststsList.Length)];
        cardName.text = $"Randomize {Stat}, for selected player";
    }

    protected override void OnUse(RaycastHit hit)
    {
        if (hit.collider.TryGetComponent(out Notepad notepad))
        {
            int charIndex = TMP_TextUtilities.FindIntersectingCharacter(
                notepad.text, 
                Input.mousePosition, 
                Camera.main, 
                true
            );

            if(charIndex == -1) return;
            
            string word = notepad.text.textInfo.wordInfo[notepad.text.GetWordIndexFromCharacter(charIndex)].GetWord();

            switch (word)
            {
                case "Age":
                    notepad.SelectedPlayerStats.list[word] = Random.Range(1, 100);
                    notepad.SetIndex(notepad.index);
                    Destroy(this.gameObject);
                    break;
                case "Score":
                    notepad.SelectedPlayerStats.list[word] = Random.Range(1, 10);
                    notepad.SetIndex(notepad.index);
                    Destroy(this.gameObject);
                    break;
            }
            
            Debug.Log(word);
        }
    }
}
