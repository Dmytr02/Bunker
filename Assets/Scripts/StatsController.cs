using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StatsController : MonoBehaviour
{
    [SerializeField] private EventTrigger statPrefab;
    [SerializeField] private Color showedColor;
    [SerializeField] private Sprite showedSprite;
    
    public static StatsController Instance;

    Dictionary<object, string> stats = new()
    {
        {Professions.Doctor, "Doctor"},
        {Professions.engineer, "Enginee"},
        {Professions.scientist, "Scientist"},
        {Professions.biologistChemist, "Biologist Chemist"},
        {Professions.psychologist, "Psychologist"},
        {Professions.Farmer, "Farmer"},
        {Professions.Soldier, "Soldier"},
        {Professions.Electrician, "Electrician"},
        {Professions.RescueWorker, "Rescue Worker"},
        {Professions.Journalist, "Journalist"},
        {Professions.Teacher, "Teacher"},
        {Professions.SocialWorker, "Social Worker"},
        {Professions.Actor, "Actor"},
        {Professions.Artist, "Artist"},
        {Professions.Student, "Student"},
        {Healthe.excellent, "Excellent"},
        {Healthe.average, "Average"},
        {Healthe.poor, "Poor"},
        {Healthe.critical, "Critical"},
        {Phobias.Claustrophobia, "Claustrophobia"},
        {Phobias.FearOfBlood, "Fear Of Blood"},
        {Phobias.FearOfTheDark, "Fear Of The Dark"},
        {Phobias.Anxiety, "Anxiety"},
        {Phobias.FearOfPublicSpeaking, "Fear Of Public Speaking"},
        {Phobias.NoPhobias, "No Phobias"},
        {Hobby.Fishing_Hunting, "Fishing/Hunting"},
        {Hobby.Drawing, "Drawing"},
        {Hobby.Chemistry, "Chemistry"},
        {Hobby.Writing, "Writing"},
        {Hobby.Fitness, "Fitness"},
        {Hobby.Music, "Music"},
        {Hobby.Knitting, "Knitting"},
        {Hobby.ComputerGames, "Computer Games"},
        {Hobby.NoHobbies, "No Hobbies"},
        {Personality.Leader, "Leader"},
        {Personality.Logical, "Logical"},
        {Personality.Stress_resistant, "Stress resistant"},
        {Personality.Communicator, "Communicator"},
        {Personality.Rational, "Rational"},
        {Personality.Reliable, "Reliable"},
        {Personality.Adaptable, "Adaptable"},
        {Personality.Observant, "Observant"},
        {Personality.Panicker, "Panicker"},
        {Personality.Unstable, "Unstable"},
        {Personality.Egoist, "Egoist"},
        {Personality.Impulsive, "Impulsive"},
        {Personality.Withdrawn, "Withdrawn"}
    };

    private void Awake()
    {
        Instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Start_()
    {
        foreach (var i in PlayerMovmant.player.stats.list)
        {
            if(i.Key == "Name") continue;
            EventTrigger trigger = Instantiate(statPrefab, transform);
            TMP_Text text = trigger.GetComponent<TMP_Text>();
            text.text = $"{i.Key}: {(stats.ContainsKey(i.Value) ? stats[i.Value] : i.Value)}";
            
            EventTrigger.Entry e = new EventTrigger.Entry();
            e.eventID = EventTriggerType.PointerClick;
            e.callback.AddListener((data) => { if(!PlayerMovmant.player.stats.isShowed[i.Key]) PlayerMovmant.player.SendStat(i.Key); text.color = showedColor; text.fontStyle = FontStyles.Underline; text.GetComponentInChildren<Image>().sprite = showedSprite; });
            
            trigger.triggers.Add(e);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
