using System;
using UnityEngine;

[DefaultExecutionOrder(0)]
public class Interactor : MonoBehaviour
{
    private void Start()
    {
        if(PlayerMovmant.player.gameObject != gameObject)
        {
            enabled = false;
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit))
            {
                if (hit.collider.gameObject.TryGetComponent<IInteractable>(out IInteractable interactable))
                {
                    interactable.Interact();
                }
            }
        }
    }
}

public interface IInteractable
{
    public void Interact();
}