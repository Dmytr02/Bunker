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

    public static GameManager Instance;
    
    public UnityEvent OnStartRound;
    public UnityEvent OnEndRound;
    
    private static readonly Dictionary<(string, object), int> costs = new Dictionary<(string, object), int>()
    {
        {("profession", Professions.Doctor), 6},
        {("profession", Professions.enginee), 6},
        {("profession", Professions.Actor), 1},
        {("profession", Professions.Artist), 1},
        {("profession", Professions.biologistChemist), 5},
        {("profession", Professions.Electrician), 3},
        {("profession", Professions.Farmer), 4},
        {("profession", Professions.Journalist), 2},
        {("profession", Professions.psychologist), 4},
        {("profession", Professions.RescueWorker), 3},
        {("profession", Professions.scientist), 5},
        {("profession", Professions.SocialWorker), 2},
        {("profession", Professions.Soldier), 3},
        {("profession", Professions.Student), 0},
        {("profession", Professions.Teacher), 2},
        {("Healthe", Healthe.excellent), 3},
        {("Healthe", Healthe.average), 1},
        {("Healthe", Healthe.poor), -2},
        {("Healthe", Healthe.critical), -3},
        {("phobias", Phobias.Claustrophobia), -4},
        {("phobias", Phobias.Anxiety), -1},
        {("phobias", Phobias.FearOfBlood), -2},
        {("phobias", Phobias.FearOfPublicSpeaking), 0},
        {("phobias", Phobias.FearOfTheDark), -1},
        {("phobias", Phobias.NoPhobias), 1},
        {("hobby", Hobby.Drawing), 1},
        {("hobby", Hobby.Fishing_Hunting), 1},
        {("hobby", Hobby.Chemistry), 1},
        {("hobby", Hobby.Writing), 1},
        {("hobby", Hobby.Fitness), 1},
        {("hobby", Hobby.Music), 0},
        {("hobby", Hobby.Knitting), 0},
        {("hobby", Hobby.ComputerGames), 0},
        {("hobby", Hobby.NoHobbies), 0},
        {("personality", Personality.Leader), 3},
        {("personality", Personality.Logical), 2},
        {("personality", Personality.Stress_resistant), 2},
        {("personality", Personality.Communicator), 2},
        {("personality", Personality.Rational), 1},
        {("personality", Personality.Reliable), 1},
        {("personality", Personality.Adaptable), 0},
        {("personality", Personality.Observant), 0},
        {("personality", Personality.Panicker), -3},
        {("personality", Personality.Unstable), -2},
        {("personality", Personality.Egoist), -2},
        {("personality", Personality.Impulsive), -1},
        {("personality", Personality.Withdrawn), -1},
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
        
        PhotonNetwork.Instantiate(notepad.name, PlayerMovmant.player.transform.position + PlayerMovmant.player.transform.rotation * notepadPosition, PlayerMovmant.player.transform.rotation * notepadRotation, 0);
        PhotonNetwork.Instantiate(vote.name, PlayerMovmant.player.transform.position + PlayerMovmant.player.transform.rotation * votePosition, PlayerMovmant.player.transform.rotation * voteRotation, 0).GetComponent<Vote>().photonView.RPC("RPC_SetPlayer", RpcTarget.All, PlayerMovmant.player.photonView.ViewID);
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
        else OnStartRound.Invoke();
    }
    
    void CalculatePoints()
    {
        int points = 0;
        foreach (var player in PlayerMovmant.players)
        {
            points += costs[("profession", (Professions)player.stats.list["profession"])];
            points += (int)player.stats.list["experience"] > 15 ? 4 : (int)player.stats.list["experience"] > 8 ? 3 : (int)player.stats.list["experience"] > 3 ? 2 : 1;
            points += (int)player.stats.list["Age"] > 60 ? -1 : (int)player.stats.list["Age"] > 40 ? 1 : (int)player.stats.list["Age"] > 25 ? 2 : 0;
            points += costs[("Healthe", (Healthe)player.stats.list["Healthe"])];
            points += costs[("phobias", (Phobias)player.stats.list["phobias"])];
            points += costs[("hobby", (Hobby)player.stats.list["hobby"])];
            points += costs[("personality", (Personality)player.stats.list["personality"])];
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
    }

    void checkSinergies(Dictionary<string, object>  player1, Dictionary<string, object> player2, ref List<int> sinergies, ref List<int> antiSinergies)
    {
        switch (player1["profession"])
        {
            case Professions.Doctor:
                switch (player2["profession"])
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
                switch (player2["personality"])
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
                switch (player2["profession"])
                {
                    case Professions.Electrician:
                        sinergies.Add(2);
                        break;
                    case Professions.scientist:
                        sinergies.Add(1);
                        break;
                }
                switch (player2["personality"])
                {
                    case Personality.Logical:
                        sinergies.Add(1);
                        break;
                }
                break;
            case Professions.scientist:
                switch (player2["profession"])
                {
                    case Professions.biologistChemist:
                        sinergies.Add(2);
                        break;
                }
                switch (player2["personality"])
                {
                    case Personality.Logical:
                        sinergies.Add(1);
                        break;
                }
                break;
            case Professions.psychologist:
                switch (player2["profession"])
                {
                    case Professions.SocialWorker:
                        sinergies.Add(2);
                        break;
                    case Professions.Teacher:
                        sinergies.Add(2);
                        break;
                }
                switch (player2["personality"])
                {
                    case Personality.Communicator:
                        sinergies.Add(1);
                        break;
                }
                break;
            case Professions.Farmer:
                switch (player2["profession"])
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
                switch (player2["profession"])
                {
                    case Professions.Soldier:
                        sinergies.Add(1);
                        break;
                }
                switch (player2["personality"])
                {
                    case Personality.Adaptable:
                        sinergies.Add(1);
                        break;
                }
                break;
            case Professions.Journalist:
                switch (player2["profession"])
                {
                    case Professions.scientist:
                        sinergies.Add(1);
                        break;
                }
                switch (player2["personality"])
                {
                    case Personality.Observant:
                        sinergies.Add(1);
                        break;
                }
                break;
            case Professions.Teacher:
                switch (player2["profession"])
                {
                    case Professions.Student:
                        sinergies.Add(1);
                        break;
                }
                break;
            case Professions.biologistChemist:
                switch (player2["personality"])
                {
                    case Personality.Rational:
                        sinergies.Add(1);
                        break;
                }
                break;
            case Professions.Soldier:
                switch (player2["personality"])
                {
                    case Personality.Stress_resistant:
                        sinergies.Add(1);
                        break;
                }
                break;
            case Professions.Actor:
                switch (player2["personality"])
                {
                    case Personality.Communicator:
                        sinergies.Add(1);
                        break;
                }
                break;
        }

        switch (player1["personality"])
        {
            case Personality.Leader:
                switch (player2["personality"])
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
                switch (player2["personality"])
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
                switch (player2["personality"])
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
                switch (player2["personality"])
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
                switch (player2["personality"])
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
        }if ((int)player1["experience"] >= 1 &&
            (int)player1["experience"] <= 3 &&
            (int)player2["experience"] >= 9 &&
            (int)player2["experience"] <= 15)
        {
            sinergies.Add(1);
        }
        if ((int)player1["experience"] >= 4 &&
            (int)player1["experience"] <= 8 &&
            (int)player2["experience"] >= 16)
        {
            sinergies.Add(1);
        }
        if ((int)player1["experience"] >= 16 &&
            (int)player2["experience"] >= 16)
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
