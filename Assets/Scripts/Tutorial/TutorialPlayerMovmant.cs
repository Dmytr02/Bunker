using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class TutorialPlayerMovmant : MonoBehaviour
{
    [SerializeField] private float sensivity = 1;
    [SerializeField] Animator playerMashAnimator;
    public Vector2 lookAngelRangeX;
    public Vector2 lookAngelRangeY;
    
    

    void Start()
    {
        GetComponent<Interactor>().enabled = true;
        //CommandManager.Instance.AddInstance(this);
            
        Camera.main.transform.SetParent(transform);
        Camera.main.transform.localPosition = Vector3.zero;
        Camera.main.transform.localRotation = Quaternion.identity;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (TutorialUIController.instance.CurrentState is TutorialUIGameState)
        {
            //Camera.main.transform.localRotation = Quaternion.Euler(Mathf.Clamp((Camera.main.transform.localRotation.eulerAngles.x - Input.mousePositionDelta.y+180)%360-180, -60, 60), Mathf.Clamp((Camera.main.transform.localRotation.eulerAngles.y + Input.mousePositionDelta.x+180)%360-180, -90, 90), 0);
            Camera.main.transform.localRotation = Quaternion.Euler(new Vector3(Mathf.Lerp(lookAngelRangeY.x, lookAngelRangeY.y, 1 - Mathf.Clamp01(Input.mousePosition.y / Screen.height)), Mathf.Lerp(lookAngelRangeX.x, lookAngelRangeX.y, Mathf.Clamp01(Input.mousePosition.x / Screen.width)), 0));
        }
    }   
}