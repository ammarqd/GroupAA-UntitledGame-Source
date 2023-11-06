using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSprint : MonoBehaviour
{
    private PlayerInput playerInput;
    private float originalSpeed;
    private float rotationSpeed;
    private float originalRotationSpeed;
    private PlayerController playerController;
    private bool isSprinting = false;
    private float originalHeight;
    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        playerController = GetComponent<PlayerController>();
        playerInput.actions["Player/Sprint"].performed += context => ToggleSprint();
        originalSpeed = playerController.speed;
        originalRotationSpeed = playerController.rotationSpeed;
        originalHeight = transform.localScale.y;
    }
  private void ToggleSprint()
    {
        isSprinting = !isSprinting;
        
        // Adjust player's height based on Sprint state
        if (isSprinting)
        {
            // Reduce the player's height when Sprinting
            playerController.speed = 500;
            playerController.rotationSpeed = 500;
            transform.localScale = new Vector3(1, 1.2f, 1);
        }
        else
        {
            // Restore the player's original height when standing up
            transform.localScale = new Vector3(1, originalHeight, 1);
            playerController.speed = originalSpeed;
            playerController.rotationSpeed = originalRotationSpeed;
        }
    }
}