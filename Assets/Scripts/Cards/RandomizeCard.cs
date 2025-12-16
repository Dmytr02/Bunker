using TMPro;
using UnityEngine;

public class RandomizeCard : Card
{
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
