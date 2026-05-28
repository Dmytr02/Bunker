using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class TutorialGameManager : MonoBehaviour
{
    [SerializeField] Massage[] massages = new Massage[5];
    [SerializeField] public TutorialNotepad notepad;
    [SerializeField] private GameObject EndPanel;
    //[SerializeField] private TutorialHints tutorialHints;
    [SerializeField] public TutorialCommandManager tutorialCommandManager;
    [SerializeField] private Tutorial tutorialCards;
    [SerializeField] private Helper assistant;
    public bool Trigger{get; set;}

    [Serializable]
    abstract class SomeAction
    {
        public abstract IEnumerator Action(TutorialGameManager manager);
    }
    
    [Serializable] class CardList : SomeAction
    {
        public List<Sprite> cards = new List<Sprite>();
        public override IEnumerator Action(TutorialGameManager manager)
        {
            manager.tutorialCards.ShowList(cards);
            yield return new WaitUntil(() => !manager.tutorialCards.isOpened);
        }
    }   
    [Serializable] class BotSay : SomeAction
    {
        public string text;
        public float showTime = -1;
        public int bot;
        public override IEnumerator Action(TutorialGameManager manager)
        {
            manager.massages[bot].showMassage(text);
            manager.tutorialCommandManager.SendMassage(text,  manager.notepad.playersStats[bot].list[EStats.Name].ToString(), manager.notepad.botColors[bot+1]);
            yield break;
        }
    }
    [Serializable] class BotReactOnStat : SomeAction
    {
        [Serializable]
        class StatText
        {
            public string text;
            public EStats stat;
        }
        public float showTime = -1;
        public int bot;
        [SerializeField] List<StatText> texts = new List<StatText>();
        public override IEnumerator Action(TutorialGameManager manager)
        {
            Debug.Log("last " + manager.lastStats + ", list");
            manager.massages[bot].showMassage(texts.FirstOrDefault(n => n.stat == manager.lastStats).text);
            manager.tutorialCommandManager.SendMassage(texts.FirstOrDefault(n => n.stat == manager.lastStats).text, manager.notepad.playersStats[bot].list[EStats.Name].ToString(), manager.notepad.botColors[bot+1]);
            yield break;
        }
    }
    [Serializable] class Wait : SomeAction
    {
        public float time;
        public override IEnumerator Action(TutorialGameManager manager)
        {
            yield return new WaitForSeconds(time);
        }
    }
    [Serializable] class BotShowStat : SomeAction
    {
        public EStats stat;
        public int bot;
        
        public override IEnumerator Action(TutorialGameManager manager)
        {
            manager.notepad.playersStats[bot].showed[stat] = true;
            manager.notepad.DrawAll();
            yield break;
        }
    }
    [Serializable] class DoAction : SomeAction
    {
        public UnityEvent Event;
        public override IEnumerator Action(TutorialGameManager manager)
        {
            Event.Invoke();
            yield break;
        }
    }
    [Serializable] class assistentSay : SomeAction
    {
        [TextArea(2, 100)] public List<string> text;
        public override IEnumerator Action(TutorialGameManager manager)
        {
            manager.assistant.SetPhrases(text);
            yield break;
        }
    }
    [Serializable] class assistentReact : SomeAction
    {
        [Serializable]
        class Result
        {
            [TextArea(2, 100)] public List<string> text;
        }
        [SerializeField] List<Result> results = new List<Result>();
        public override IEnumerator Action(TutorialGameManager manager)
        {
            manager.assistant.SetPhrases(results[assistentLast].text);
            yield break;
        }
    }
    [Serializable] class assistentHide : SomeAction
    {
        public override IEnumerator Action(TutorialGameManager manager)
        {
            manager.assistant.Hide();
            yield break;
        }
    }
    [Serializable] class WaitTrigger : SomeAction
    {
        public override IEnumerator Action(TutorialGameManager manager)
        {
            yield return new WaitUntil(() => manager.Trigger);
            manager.Trigger = false;
        }
    }
    [Serializable] class Voting : SomeAction
    {
        [Serializable] class NameVotes
        {
            public string name;
            public int votes;
            public Sprite voteSprite;
        } 
        public int playersVotes;
        [SerializeField] List<NameVotes> votes = new List<NameVotes>();
        public int botToDestroy;
        public override IEnumerator Action(TutorialGameManager manager)
        {

            TutorialVote.Instance.voteTextButtons[0].text = $"{PlayerPrefs.GetString("name", "Name")}"; 
            for (int i = 1; i < TutorialVote.Instance.voteTextButtons.Length; i++)
            {
                if (i <= votes.Count) 
                {
                    TutorialVote.Instance.voteButtons[i].gameObject.SetActive(true);
                    TutorialVote.Instance.voteBGButtons[i].sprite = votes[i-1].voteSprite;
                    TutorialVote.Instance.voteTextButtons[i].text = votes[i-1].name;
                }
                else
                {
                    TutorialVote.Instance.voteButtons[i].gameObject.SetActive(false);
                }
            }
            
            manager.StartCoroutine(TutorialVote.Instance.StartRound());
            yield return new WaitUntil(() => TutorialVote.votes.Count == 1);
            TutorialVote.Instance.voteTextButtons[0].text = $"{PlayerPrefs.GetString("name", "Name")}\nvotes: {playersVotes + (TutorialVote.votes.Contains(0) ? 1 : 0)}";
            for (int i = 1; i < TutorialVote.Instance.voteTextButtons.Length; i++)
            {
                if (i <= votes.Count)
                {
                    TutorialVote.Instance.voteButtons[i].gameObject.SetActive(true);
                    TutorialVote.Instance.voteBGButtons[i].sprite = votes[i-1].voteSprite;
                    TutorialVote.Instance.voteTextButtons[i].text = $"{votes[i-1].name}\nvotes: {(TutorialVote.votes.Contains(i) ? 1 : 0) + votes[i-1].votes}";
                }
                else
                {
                    TutorialVote.Instance.voteButtons[i].gameObject.SetActive(false);
                }
            }
            
            yield return new WaitForSeconds(7);
            manager.StartCoroutine(TutorialVote.Instance.EndVoting());
            manager.massages[botToDestroy].gameObject.SetActive(false);
        }
    }
    [Serializable] class WaitAssistent : SomeAction
    {
        public override IEnumerator Action(TutorialGameManager manager)
        {
            yield return new WaitUntil(() => !manager.assistant.animator.GetBool("isShowen"));
        }
    }
    [Serializable] class WaitForAny : SomeAction
    {
        [SerializeReference, SelectSubclass] public List<SomeAction> actions;
        private bool flag = false;

        public override IEnumerator Action(TutorialGameManager manager) 
        {
            flag = false;
            List<Coroutine> runningCoroutines = new List<Coroutine>();

            for(int i = 0; i < actions.Count; i++)
            {
                runningCoroutines.Add(manager.StartCoroutine(Routine(manager, i)));
            }

            yield return new WaitUntil(() => flag);

            foreach (var c in runningCoroutines)
            {
                if (c != null) manager.StopCoroutine(c);
            }
        }

        IEnumerator Routine(TutorialGameManager manager, int i)
        {
            yield return manager.StartCoroutine(actions[i].Action(manager));
            flag = true; 
        }
    }

    [Serializable]
    class DoActionDelayed : SomeAction
    {
        [SerializeReference, SelectSubclass] SomeAction action;
        [SerializeField] float delay;

        public override IEnumerator Action(TutorialGameManager manager)
        {
            manager.StartCoroutine(Routine(manager));
            yield break;
        }

        IEnumerator Routine(TutorialGameManager manager)
        {
            
            yield return new WaitForSeconds(delay);
            manager.StartCoroutine(action.Action(manager));
        }
    }
    
    [SerializeReference, SelectSubclass] List<SomeAction> CardLists;
    
    [HideInInspector] public EStats lastStats;
    public static int assistentLast;
    public static TutorialGameManager Instance;
    
    public void OpenStat(int stat)
    {
        notepad.playerStats.showed[(EStats)stat] = true;
        lastStats = (EStats)stat;
        notepad.DrawAll();
    }
    void Start()
    {
        Instance = this;
        StartCoroutine(GameLoop());
    }


    #if UNITY_EDITOR
    private void Update()
    {
        if(Input.GetKey(KeyCode.P)) Time.timeScale = 10;
        else Time.timeScale = 1;
    }
    #endif
    
    IEnumerator GameLoop()
    {
        for (int i = 0; i < CardLists.Count; i++)
        {
            yield return StartCoroutine(CardLists[i].Action(this));
        }
        EndPanel.SetActive(true);
        //yield return new WaitUntil(() => tutorialHints.triger2);
        //tutorialHints.triger2 = false;
        
        /*tutorialCards.ShowList((CardLists[0] as CardList)?.cards);
        yield return new WaitUntil(() => !tutorialCards.isOpened);
        
        yield return new WaitUntil(() => opened || openedCount == 7);
        massages[0].showMassage("I’m 86… well, at least I’ve survived a lot of stuff already.");
        tutorialCommandManager.SendMassage("I’m 86… well, at least I’ve survived a lot of stuff already.", "0");
        notepad.playersStats[0].showed["Age"] = true;
        notepad.SetIndex(notepad.index);
        yield return new WaitForSeconds(7.5f);
        massages[1].showMassage("Claustrophobia… yeah, I see the irony.");
        tutorialCommandManager.SendMassage("Claustrophobia… yeah, I see the irony.", "1");
        notepad.playersStats[1].showed["Phobias"] = true;
        notepad.SetIndex(notepad.index);
        yield return new WaitForSeconds(5.3f);
        massages[2].showMassage("I’m a doctor. Just saying.");
        tutorialCommandManager.SendMassage("I’m a doctor. Just saying.", "2");
        notepad.playersStats[2].showed["Profession"] = true;
        notepad.SetIndex(notepad.index);
        yield return new WaitForSeconds(4.1f);
        massages[3].showMassage("I hunt. If food runs out, don’t panic.");
        tutorialCommandManager.SendMassage("I hunt. If food runs out, don’t panic.", "3");
        notepad.playersStats[3].showed["Hobby"] = true;
        notepad.SetIndex(notepad.index);
        yield return new WaitForSeconds(5.3f);
        massages[4].showMassage("I’m 25. Back still works.");
        tutorialCommandManager.SendMassage("I’m 25. Back still works.", "4");
        notepad.playersStats[4].showed["Age"] = true;
        notepad.SetIndex(notepad.index);
        yield return new WaitForSeconds(4);
        
        
        
        //tutorialHints._triger = true;
        //yield return new WaitUntil(() => tutorialHints.triger2);
        //tutorialHints.triger2 = false;
        TutorialVote.Instance.StartRound();
        yield return new WaitUntil(() => TutorialVote.votes.Count == 1);
        //tutorialHints._triger = true;
        TutorialVote.Instance.voteTextButtons[0].text = $"{PlayerPrefs.GetString("name", "Name")}\nvotes: {(TutorialVote.votes.Contains(0) ? 1 : 0)}";
        TutorialVote.Instance.voteTextButtons[1].text = $"Bob\nvotes: {5 + (TutorialVote.votes.Contains(1) ? 1 : 0)}";
        TutorialVote.Instance.voteTextButtons[2].text = $"Mike\nvotes: {(TutorialVote.votes.Contains(2) ? 1 : 0)}";
        TutorialVote.Instance.voteTextButtons[3].text = $"Chad\nvotes: {(TutorialVote.votes.Contains(3) ? 1 : 0)}";
        TutorialVote.Instance.voteTextButtons[4].text = $"Dave\nvotes: {(TutorialVote.votes.Contains(4) ? 1 : 0)}";
        TutorialVote.Instance.voteTextButtons[5].text = $"Kevin\nvotes: {(TutorialVote.votes.Contains(5) ? 1 : 0)}";

        yield return new WaitForSeconds(7);
        TutorialVote.Instance.EndVoting();
        TutorialVote.Instance.voteButtons[1].gameObject.SetActive(false);
        massages[0].gameObject.SetActive(false);
        
        TutorialVote.Instance.voteTextButtons[0].text = $"{PlayerPrefs.GetString("name", "Name")}"; 
        TutorialVote.Instance.voteTextButtons[2].text = $"Mike";
        TutorialVote.Instance.voteTextButtons[3].text = $"Chad";
        TutorialVote.Instance.voteTextButtons[4].text = $"Dave";
        TutorialVote.Instance.voteTextButtons[5].text = $"Kevin";
        
        opened = false;
        yield return new WaitUntil(() => opened || openedCount == 7);
        massages[1].showMassage("I’m a rescuer. Usually I pull people out of places like this.");
        tutorialCommandManager.SendMassage("I’m a rescuer. Usually I pull people out of places like this.", "1");
        notepad.playersStats[1].showed["Profession"] = true;
        notepad.SetIndex(notepad.index);
        yield return new WaitForSeconds(7.6f);
        massages[2].showMassage("Yeah, I’m a doctor… and yeah, I don’t like blood. It happens.");
        tutorialCommandManager.SendMassage("Yeah, I’m a doctor… and yeah, I don’t like blood. It happens.", "2");
        notepad.playersStats[2].showed["Phobias"] = true;
        notepad.SetIndex(notepad.index);
        yield return new WaitForSeconds(7.6f);
        massages[3].showMassage("Average health. Still holding up.");
        tutorialCommandManager.SendMassage("Average health. Still holding up.", "3");
        notepad.playersStats[3].showed["Health"] = true;
        notepad.SetIndex(notepad.index);
        yield return new WaitForSeconds(4.8f);
        massages[4].showMassage("Engineer. If something breaks, that’s my problem.");
        tutorialCommandManager.SendMassage("Engineer. If something breaks, that’s my problem.", "4");
        notepad.playersStats[4].showed["Profession"] = true;
        notepad.SetIndex(notepad.index);
        yield return new WaitForSeconds(6.4f);
        
        TutorialVote.Instance.StartRound();
        yield return new WaitUntil(() => TutorialVote.votes.Count == 1);
        TutorialVote.Instance.voteTextButtons[0].text = $"{PlayerPrefs.GetString("name", "Name")}\nvotes: {(TutorialVote.votes.Contains(0) ? 1 : 0)}";
        TutorialVote.Instance.voteTextButtons[2].text = $"Mike\nvotes: {(TutorialVote.votes.Contains(2) ? 1 : 0)}";
        TutorialVote.Instance.voteTextButtons[3].text = $"Chad\nvotes: {4 + (TutorialVote.votes.Contains(3) ? 1 : 0)}";
        TutorialVote.Instance.voteTextButtons[4].text = $"Dave\nvotes: {(TutorialVote.votes.Contains(4) ? 1 : 0)}";
        TutorialVote.Instance.voteTextButtons[5].text = $"Kevin\nvotes: {(TutorialVote.votes.Contains(5) ? 1 : 0)}";
        
        yield return new WaitForSeconds(7);
        TutorialVote.Instance.EndVoting();
        TutorialVote.Instance.voteButtons[3].gameObject.SetActive(false);
        massages[2].gameObject.SetActive(false);
        
        TutorialVote.Instance.voteTextButtons[0].text = $"{PlayerPrefs.GetString("name", "Name")}"; 
        TutorialVote.Instance.voteTextButtons[2].text = $"Mike";
        TutorialVote.Instance.voteTextButtons[4].text = $"Dave";
        TutorialVote.Instance.voteTextButtons[5].text = $"Kevin";
        
        opened = false;
        yield return new WaitUntil(() => opened || openedCount == 7);
        massages[1].showMassage("Health’s pretty bad… but I’ve got experience.");
        tutorialCommandManager.SendMassage("Health’s pretty bad… but I’ve got experience.", "1");
        notepad.playersStats[1].showed["Health"] = true;
        notepad.SetIndex(notepad.index);
        yield return new WaitForSeconds(6);
        massages[3].showMassage("36. Old enough to be smart, not old enough to be useless.");
        tutorialCommandManager.SendMassage("36. Old enough to be smart, not old enough to be useless.", "3");
        notepad.playersStats[3].showed["Age"] = true;
        notepad.SetIndex(notepad.index);
        yield return new WaitForSeconds(7.2f);
        massages[4].showMassage("I’m logical. Arguing with me won’t be easy.");
        tutorialCommandManager.SendMassage("I’m logical. Arguing with me won’t be easy.", "4");
        notepad.playersStats[4].showed["Personality"] = true;
        notepad.SetIndex(notepad.index);
        yield return new WaitForSeconds(5.8f);
        
        TutorialVote.Instance.StartRound();
        yield return new WaitUntil(() => TutorialVote.votes.Count == 1);
        TutorialVote.Instance.voteTextButtons[0].text = $"{PlayerPrefs.GetString("name", "Name")}\nvotes: {(TutorialVote.votes.Contains(0) ? 1 : 0)}";
        TutorialVote.Instance.voteTextButtons[2].text = $"Mike\nvotes: {3 + (TutorialVote.votes.Contains(2) ? 1 : 0)}";
        TutorialVote.Instance.voteTextButtons[4].text = $"Dave\nvotes: {(TutorialVote.votes.Contains(4) ? 1 : 0)}";
        TutorialVote.Instance.voteTextButtons[5].text = $"Kevin\nvotes: {(TutorialVote.votes.Contains(5) ? 1 : 0)}";
        
        yield return new WaitForSeconds(7);
        TutorialVote.Instance.EndVoting();
        TutorialVote.Instance.voteButtons[2].gameObject.SetActive(false);
        massages[1].gameObject.SetActive(false);
        
        TutorialVote.Instance.voteTextButtons[0].text = $"{PlayerPrefs.GetString("name", "Name")}"; 
        TutorialVote.Instance.voteTextButtons[4].text = $"Dave";
        TutorialVote.Instance.voteTextButtons[5].text = $"Kevin";
        
        opened = false;
        yield return new WaitUntil(() => opened || openedCount == 7);
        massages[3].showMassage("I’m a leader. Someone has to say ‘we’re doing it this way.");
        tutorialCommandManager.SendMassage("I’m a leader. Someone has to say ‘we’re doing it this way.", "3");
        notepad.playersStats[3].showed["Personality"] = true;
        notepad.SetIndex(notepad.index);
        yield return new WaitForSeconds(7.3f);
        massages[4].showMassage("Excellent health. I can work longer than most.");
        tutorialCommandManager.SendMassage("Excellent health. I can work longer than most.", "4");
        notepad.playersStats[4].showed["Health"] = true;
        notepad.SetIndex(notepad.index);
        yield return new WaitForSeconds(6.1f);
        
        TutorialVote.Instance.StartRound();
        yield return new WaitUntil(() => TutorialVote.votes.Count == 1);
        TutorialVote.Instance.voteTextButtons[0].text = $"{PlayerPrefs.GetString("name", "Name")}\nvotes: {2 + (TutorialVote.votes.Contains(0) ? 1 : 0)}";
        TutorialVote.Instance.voteTextButtons[4].text = $"Dave\nvotes: {(TutorialVote.votes.Contains(4) ? 1 : 0)}";
        TutorialVote.Instance.voteTextButtons[5].text = $"Kevin\nvotes: {(TutorialVote.votes.Contains(5) ? 1 : 0)}";
        
        yield return new WaitForSeconds(7);
        TutorialVote.Instance.EndVoting();
        TutorialVote.Instance.voteButtons[1].gameObject.SetActive(false);
        //massages[1].gameObject.SetActive(false);
        //игрока выгнали, конец
        EndPanel.SetActive(true);
        
        TutorialVote.Instance.voteTextButtons[4].text = $"Dave";
        TutorialVote.Instance.voteTextButtons[5].text = $"Kevin";
        
        opened = false;*/
    }
}
