using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatsDrawer : MonoBehaviour
{
    [SerializeField] ManualCameraRender cameraRender;
    [SerializeField] TMP_Text Text;
    [SerializeField] TMP_Text TextName;
    [SerializeField] TMP_Text TextAge;
    [SerializeField] Image IconImage;
    [SerializeField] int index;
    
    public static List<StatsDrawer> pages = new List<StatsDrawer>();

    private void Awake()
    {
        pages.Add(this);
        pages = pages.OrderBy(n => n.index).ToList();
        Text.gameObject.SetActive(false);
        TextName.gameObject.SetActive(false);
        TextAge.gameObject.SetActive(false);
        IconImage.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        pages.Remove(this);
    }

    public void Draw(PlayerStats stats)
    {
        Text.gameObject.SetActive(true);
        TextName.gameObject.SetActive(true);
        TextAge.gameObject.SetActive(true);
        IconImage.gameObject.SetActive(true);
        print("Draw");
        TextName.text = stats.list["Name"].ToString();
        TextAge.text = "Age: " + (stats.list["Age"] is int ? ((int)stats.list["Age"]==-1?"-":stats.list["Age"]) : "-");
        Text.text = stats.ToString(new HashSet<string>{"Profession", "Experience", "Healthe", "Phobias", "Hobby", "Personality"});   
        cameraRender.RenderCameraNow();
    }
    public void Draw(TutorialPlayerStats stats)
    {
        Text.gameObject.SetActive(true);
        TextName.gameObject.SetActive(true);
        TextAge.gameObject.SetActive(true);
        IconImage.gameObject.SetActive(true);
        print("Draw");
        TextName.text = stats.list[EStats.Name].ToString();
        TextAge.text = "Age: " + (stats.list[EStats.Age] is int && stats.showed[EStats.Age] ? ((int)stats.list[EStats.Age]==-1?"-":stats.list[EStats.Age]) : "-");
        Text.text = stats.ToString(new HashSet<EStats>{EStats.Profession, EStats.Experience, EStats.Healthe, EStats.Phobias, EStats.Hobby, EStats.Personality});   
        cameraRender.RenderCameraNow();
    }
}
