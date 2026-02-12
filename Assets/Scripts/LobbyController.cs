using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
public class LobbyController : MonoBehaviour
{
    private Vector2 IsCharacterCycling;
    public bool isReady = false;
    
    public void OnMove(InputAction.CallbackContext context)
    {
    IsCharacterCycling = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isReady = !isReady;
            Debug.Log(isReady);
        }
    }
}
