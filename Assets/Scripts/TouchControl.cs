using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchControl : MonoBehaviour
{
    public PlayerController playerController;

    private bool isPressed = false;
    private bool isReleased = false;
    private void Update()
    {
        if (isPressed)
        {
            playerController.ReadySwing();
        }
        
    }

    public void OnPress()
    {
        isPressed = true;
    }

    public void OnRelease()
    {
        isPressed = false;
        playerController.StopSwing();
    }
}
