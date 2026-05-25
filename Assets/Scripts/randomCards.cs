using System.Collections.Generic;
using UnityEngine;

public class randomCards : MonoBehaviour
{
    [SerializeField] List<GameObject> cards;
    [SerializeField] private int count = 2;
    void Start()
    {
        for (int i = 0; i < count && 0 < cards.Count; i++)
        {
            int rand = Random.Range(0, cards.Count);
            cards[rand].SetActive(true);
            cards.RemoveAt(rand);
        }
    }
}
