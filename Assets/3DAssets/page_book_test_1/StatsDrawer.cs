using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class StatsDrawer : MonoBehaviour
{
    [SerializeField] ManualCameraRender cameraRender;
    [SerializeField] TMP_Text Text;
    [SerializeField] int index;
    
    public static List<StatsDrawer> pages = new List<StatsDrawer>();

    private void Awake()
    {
        pages.Add(this);
        pages = pages.OrderBy(n => n.index).ToList();
    }

    private void OnDestroy()
    {
        pages.Remove(this);
    }

    public void Draw(PlayerStats stats)
    {
        print("Draw");
        Text.text = stats.ToString();   
        cameraRender.RenderCameraNow();
    }
}
