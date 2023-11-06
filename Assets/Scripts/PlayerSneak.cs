using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSneak : MonoBehaviour
{
    private PlayerInput playerInput;
    private float originalSpeed;
    private PlayerController playerController;
    private bool isSneaking = false;
    private float originalHeight;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        playerController = GetComponent<PlayerController>();
        playerInput.actions["Player/Sneak"].performed += context => ToggleSneak();
        originalSpeed = playerController.speed;
        originalHeight = transform.localScale.y;
    }
  private void ToggleSneak()
    {
        Debug.Log("Pressed sneak");
        isSneaking = !isSneaking;
        
        // Adjust player's height based on crouch state
        if (isSneaking)
        {
            // Reduce the player's height when crouching
            playerController.speed = 365;
            transform.localScale = new Vector3(1, 0.6f, 1);
        }
        else
        {
            // Restore the player's original height when standing up
            transform.localScale = new Vector3(1, originalHeight, 1);
        }
    }
}