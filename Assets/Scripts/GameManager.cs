using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static DateTime NextRound = DateTime.MaxValue;
    [SerializeField] Notepad notepad;
    [SerializeField] Vector3 notepadPosition;
    [SerializeField] Quaternion notepadRotation =  Quaternion.identity;
    [SerializeField] Vote vote;
    [SerializeField] Vector3 votePosition;
    [SerializeField] Quaternion voteRotation = Quaternion.identity;
    [SerializeField] TMP_Text timerText;
    [SerializeField] TMP_Text resultText;

    public static GameManager Instance;
    
    public UnityEvent OnStartRound;
    public UnityEvent OnEndRound;
    
    private static readonly Dictionary<(string, object), int> costs = new Dictionary<(string, object), int>()
    {
        {("Profession", Professions.Doctor), 6},
        {("Profession", Professions.enginee), 6},
        {("Profession", Professions.Actor), 1},
        {("Profession", Professions.Artist), 1},
        {("Profession", Professions.biologistChemist), 5},
        {("Profession", Professions.Electrician), 3},
        {("Profession", Professions.Farmer), 4},
        {("Profession", Professions.Journalist), 2},
        {("Profession", Professions.psychologist), 4},
        {("Profession", Professions.RescueWorker), 3},
        {("Profession", Professions.scientist), 5},
        {("Profession", Professions.SocialWorker), 2},
        {("Profession", Professions.Soldier), 3},
        {("Profession", Professions.Student), 0},
        {("Profession", Professions.Teacher), 2},
        {("Healthe", Healthe.excellent), 3},
        {("Healthe", Healthe.average), 1},
        {("Healthe", Healthe.poor), -2},
        {("Healthe", Healthe.critical), -3},
        {("Phobias", Phobias.Claustrophobia), -4},
        {("Phobias", Phobias.Anxiety), -1},
        {("Phobias", Phobias.FearOfBlood), -2},
        {("Phobias", Phobias.FearOfPublicSpeaking), 0},
        {("Phobias", Phobias.FearOfTheDark), -1},
        {("Phobias", Phobias.NoPhobias), 1},
        {("Hobby", Hobby.Drawing), 1},
        {("Hobby", Hobby.Fishing_Hunting), 1},
        {("Hobby", Hobby.Chemistry), 1},
        {("Hobby", Hobby.Writing), 1},
        {("Hobby", Hobby.Fitness), 1},
        {("Hobby", Hobby.Music), 0},
        {("Hobby", Hobby.Knitting), 0},
        {("Hobby", Hobby.ComputerGames), 0},
        {("Hobby", Hobby.NoHobbies), 0},
        {("Personality", Personality.Leader), 3},
        {("Personality", Personality.Logical), 2},
        {("Personality", Personality.Stress_resistant), 2},
        {("Personality", Personality.Communicator), 2},
        {("Personality", Personality.Rational), 1},
        {("Personality", Personality.Reliable), 1},
        {("Personality", Personality.Adaptable), 0},
        {("Personality", Personality.Observant), 0},
        {("Personality", Personality.Panicker), -3},
        {("Personality", Personality.Unstable), -2},
        {("Personality", Personality.Egoist), -2},
        {("Personality", Personality.Impulsive), -1},
        {("Personality", Personality.Withdrawn), -1},
    };
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);
        CommandManager.Instance.AddInstance(this);
    }

    [CommandAtribute("/TimeToEndRound", "description")]
    public void TimeToEndRound(float seconds)
    {
        NextRound = DateTime.Now.AddSeconds(seconds);
    }

    [CommandAtribute("/Start", "description")]
    public void StartGameForAll()
    {
        Debug.Log("Starting game local");
        photonView.RPC("StartGame", RpcTarget.All);
    }

    [PunRPC]
    public void StartGame()
    {
        Debug.Log("Starting game");
        NextRound = DateTime.Now.AddSeconds(5);
        
        PhotonNetwork.Instantiate(notepad.name, PlayerMovmant.player.transform.position + PlayerMovmant.player.transform.rotation * notepadPosition, PlayerMovmant.player.transform.rotation * notepadRotation, 0).transform.SetParent(PlayerMovmant.player.transform);
        //Instantiate(vote);
        PlayerMovmant.player.SendStat("Name");
        //PlayerMovmant.player.SendStat("Age");

        //StartRound();
        if(PhotonNetwork.IsMasterClient) PhotonNetwork.CurrentRoom.IsOpen = false;
    }
    
    
    
    [PunRPC]
    public void StartRound()
    {
        if (PlayerMovmant.players.Count <= 2)
        {
            PlayerMovmant.player.SendAllStats();
            Invoke("CalculatePoints", 5);
        }
        else
        {
            Debug.Log("Starting round");
            OnStartRound.Invoke();
        }
    }
    
    void CalculatePoints()
    {
        int points = 0;
        foreach (var player in PlayerMovmant.players)
        {
            points += costs[("Profession", (Professions)player.stats.list["Profession"])];
            points += (int)player.stats.list["Experience"] > 15 ? 4 : (int)player.stats.list["Experience"] > 8 ? 3 : (int)player.stats.list["Experience"] > 3 ? 2 : 1;
            points += (int)player.stats.list["Age"] > 60 ? -1 : (int)player.stats.list["Age"] > 40 ? 1 : (int)player.stats.list["Age"] > 25 ? 2 : 0;
            points += costs[("Healthe", (Healthe)player.stats.list["Healthe"])];
            points += costs[("Phobias", (Phobias)player.stats.list["Phobias"])];
            points += costs[("Hobby", (Hobby)player.stats.list["Hobby"])];
            points += costs[("Personality", (Personality)player.stats.list["Personality"])];
        }
        
        List<int> sinergies = new List<int>();
        List<int> antiSinergies = new List<int>();

        checkSinergies(PlayerMovmant.players[0].stats.list, PlayerMovmant.players[1].stats.list, ref sinergies, ref antiSinergies);
        checkSinergies(PlayerMovmant.players[1].stats.list, PlayerMovmant.players[0].stats.list, ref sinergies, ref antiSinergies);
        
        sinergies.Sort();
        antiSinergies.Sort();

        if(sinergies.Count > 0) points += sinergies[^1];
        if(sinergies.Count > 1) points += sinergies[^2];
        
        if(antiSinergies.Count > 0) points += antiSinergies[0];
        
        Debug.Log(points);
        resultText.text = "you have " + points + " points.\n"
            + (points > 15 ? "You Win!" : points > 10 ? "You survive" : "You lost :(");
    }

    void checkSinergies(Dictionary<string, object>  player1, Dictionary<string, object> player2, ref List<int> sinergies, ref List<int> antiSinergies)
    {
        switch (player1["Profession"])
        {
            case Professions.Doctor:
                switch (player2["Profession"])
                {
                    case Professions.scientist:
                        sinergies.Add(2);
                        break;
                    case Professions.biologistChemist:
                        sinergies.Add(2);
                        break;
                    case Professions.psychologist:
                        sinergies.Add(1);
                        break;
                }
                switch (player2["Personality"])
                {
                    case Personality.Reliable:
                        sinergies.Add(1);
                        break;
                    case Personality.Stress_resistant:
                        sinergies.Add(1);
                        break;
                }
                break;
            case Professions.enginee:
                switch (player2["Profession"])
                {
                    case Professions.Electrician:
                        sinergies.Add(2);
                        break;
                    case Professions.scientist:
                        sinergies.Add(1);
                        break;
                }
                switch (player2["Personality"])
                {
                    case Personality.Logical:
                        sinergies.Add(1);
                        break;
                }
                break;
            case Professions.scientist:
                switch (player2["Profession"])
                {
                    case Professions.biologistChemist:
                        sinergies.Add(2);
                        break;
                }
                switch (player2["Personality"])
                {
                    case Personality.Logical:
                        sinergies.Add(1);
                        break;
                }
                break;
            case Professions.psychologist:
                switch (player2["Profession"])
                {
                    case Professions.SocialWorker:
                        sinergies.Add(2);
                        break;
                    case Professions.Teacher:
                        sinergies.Add(2);
                        break;
                }
                switch (player2["Personality"])
                {
                    case Personality.Communicator:
                        sinergies.Add(1);
                        break;
                }
                break;
            case Professions.Farmer:
                switch (player2["Profession"])
                {
                    case Professions.RescueWorker:
                        sinergies.Add(1);
                        break;
                    case Professions.Soldier:
                        sinergies.Add(1);
                        break;
                }
                break;
            case Professions.RescueWorker:
                switch (player2["Profession"])
                {
                    case Professions.Soldier:
                        sinergies.Add(1);
                        break;
                }
                switch (player2["Personality"])
                {
                    case Personality.Adaptable:
                        sinergies.Add(1);
                        break;
                }
                break;
            case Professions.Journalist:
                switch (player2["Profession"])
                {
                    case Professions.scientist:
                        sinergies.Add(1);
                        break;
                }
                switch (player2["Personality"])
                {
                    case Personality.Observant:
                        sinergies.Add(1);
                        break;
                }
                break;
            case Professions.Teacher:
                switch (player2["Profession"])
                {
                    case Professions.Student:
                        sinergies.Add(1);
                        break;
                }
                break;
            case Professions.biologistChemist:
                switch (player2["Personality"])
                {
                    case Personality.Rational:
                        sinergies.Add(1);
                        break;
                }
                break;
            case Professions.Soldier:
                switch (player2["Personality"])
                {
                    case Personality.Stress_resistant:
                        sinergies.Add(1);
                        break;
                }
                break;
            case Professions.Actor:
                switch (player2["Personality"])
                {
                    case Personality.Communicator:
                        sinergies.Add(1);
                        break;
                }
                break;
        }

        switch (player1["Personality"])
        {
            case Personality.Leader:
                switch (player2["Personality"])
                {
                    case Personality.Reliable:
                        sinergies.Add(2);
                        break;
                    case Personality.Communicator:
                        sinergies.Add(1);
                        break;
                    case Personality.Panicker:
                        antiSinergies.Add(-2);
                        break;
                }
                break;
            case Personality.Logical:
                switch (player2["Personality"])
                {
                    case Personality.Rational:
                        sinergies.Add(1);
                        break;
                    case Personality.Unstable:
                        sinergies.Add(-1);
                        break;
                }
                break;
            case Personality.Stress_resistant:
                switch (player2["Personality"])
                {
                    case Personality.Reliable:
                        sinergies.Add(1);
                        break;
                    case Personality.Panicker:
                        sinergies.Add(-1);
                        break;
                }
                break;
            case Personality.Communicator:
                switch (player2["Personality"])
                {
                    case Personality.Observant:
                        sinergies.Add(1);
                        break;
                    case Personality.Egoist:
                        sinergies.Add(-1);
                        break;
                    case Personality.Withdrawn:
                        sinergies.Add(-1);
                        break;
                }
                break;
            case Personality.Rational:
                switch (player2["Personality"])
                {
                    case Personality.Impulsive:
                        sinergies.Add(1);
                        break;
                }
                break;
        }

        if ((int)player1["Age"] >= 41 &&
            (int)player1["Age"] <= 60 &&
            (int)player2["Age"] >= 18 &&
            (int)player2["Age"] <= 25)
        {
            sinergies.Add(1);
        }
        if ((int)player1["Age"] >= 26 &&
            (int)player1["Age"] <= 40 &&
            (int)player2["Age"] >= 26 &&
            (int)player2["Age"] <= 40)
        {
            sinergies.Add(1);
        }
        if ((int)player1["Age"] >= 61 &&
            (int)player2["Age"] >= 61)
        {
            antiSinergies.Add(-1);
        }if ((int)player1["Experience"] >= 1 &&
            (int)player1["Experience"] <= 3 &&
            (int)player2["Experience"] >= 9 &&
            (int)player2["Experience"] <= 15)
        {
            sinergies.Add(1);
        }
        if ((int)player1["Experience"] >= 4 &&
            (int)player1["Experience"] <= 8 &&
            (int)player2["Experience"] >= 16)
        {
            sinergies.Add(1);
        }
        if ((int)player1["Experience"] >= 16 &&
            (int)player2["Experience"] >= 16)
        {
            antiSinergies.Add(-1);
        }
    }
    
    [PunRPC]
    public void EndRound()
    {
        OnEndRound.Invoke();
    }

    [PunRPC]
    public void UpdateTimer(string message)
    {
        timerText.text = message;
    }
    void Update()
    {
        if(!PhotonNetwork.IsMasterClient) return;
        photonView.RPC("UpdateTimer", RpcTarget.All, new object[] { (NextRound-DateTime.Now).ToString(@"mm\:ss") + " to end of voting" });
        if (NextRound < DateTime.Now)
        {
            if (Vote.votes.Count > 0)
            {
                var groups = Vote.votes
                    .GroupBy(n => n)
                    .Select(g => new { Value = g.Key, Count = g.Count() })
                    .OrderByDescending(g => g.Count)
                    .ToList();

                var chosenPlayer = groups.First();

                if (groups.Count < 2 || chosenPlayer.Count != groups[1].Count)
                    PhotonView.Find(chosenPlayer.Value).RPC("RPC_expel", /*PhotonView.Find(chosenPlayer.Value).Owner*/ RpcTarget.All);
                Vote.votes = new List<int>();
                photonView.RPC("EndRound", RpcTarget.All);
                NextRound = DateTime.Now.AddSeconds(5);
            }
            else
            {
                NextRound = DateTime.Now.AddSeconds(60);
                photonView.RPC("StartRound", RpcTarget.All);
            }
        }
    }
}
