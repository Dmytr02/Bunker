using Photon.Pun;
using UnityEngine;

public class PlayerMovmant : MonoBehaviourPunCallbacks
{
    //[SerializeField] CharacterController characterController;
    //[SerializeField] float speed = 2.0f;
    //[SerializeField] float gravity = 9.8f;
    //[SerializeField] float jumpForce = 10;

    [SerializeField] private float sensivity = 1;
    
    float yForce = 0;
    void Start()
    {
        if (photonView.IsMine)
        {
            Camera.main.transform.SetParent(transform);
            Camera.main.transform.localPosition = Vector3.zero;
            Camera.main.transform.localRotation = Quaternion.identity;
            transform.LookAt(Vector3.zero);
            
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        Camera.main.transform.localRotation = Quaternion.Euler(Mathf.Clamp((Camera.main.transform.localRotation.eulerAngles.x - Input.mousePositionDelta.y+180)%360-180, -60, 60), Mathf.Clamp((Camera.main.transform.localRotation.eulerAngles.y + Input.mousePositionDelta.x+180)%360-180, -90, 90), 0);
    }
}
