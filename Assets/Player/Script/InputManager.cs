using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    [Header("Input Actions Asset")]
    public InputActionAsset inputActions;

    [Header("Players")]
    public PlayerController player1;
    public PlayerController player2;
    public PlayerController player3;
    public PlayerController player4;

    private InputActionMap p1Map, p2Map, p3Map, p4Map;

    void Awake()
    {
        p1Map = inputActions.FindActionMap("Player1");
        p2Map = inputActions.FindActionMap("Player2");
        p3Map = inputActions.FindActionMap("Player3");
        p4Map = inputActions.FindActionMap("Player4");

        var gamepads = Gamepad.all;

        SetupPlayer(p1Map, player1, null);
        SetupPlayer(p4Map, player4, null);

        SetupPlayer(p2Map, player2, gamepads.Count > 0 ? gamepads[0] : null);
        SetupPlayer(p3Map, player3, gamepads.Count > 1 ? gamepads[1] : null);
    }

    void SetupPlayer(InputActionMap map, PlayerController player, InputDevice device)
    {
        if (map == null || player == null) return;

        if (device != null)
            map.devices = new[] { device };

        map.FindAction("Move").performed += player.OnMove;
        map.FindAction("Move").canceled += player.OnMove;
        map.FindAction("Jump").performed += player.OnJump;
        map.FindAction("Attack").performed += player.OnAttack;
        map.FindAction("Parry").performed += player.OnParry;
        map.FindAction("Dodge").performed += player.OnDodge;
        map.Enable();
    }

    void OnDestroy()
    {
        CleanupPlayer(p1Map, player1);
        CleanupPlayer(p2Map, player2);
        CleanupPlayer(p3Map, player3);
        CleanupPlayer(p4Map, player4);
    }

    void CleanupPlayer(InputActionMap map, PlayerController player)
    {
        if (map == null || player == null) return;

        map.FindAction("Move").performed -= player.OnMove;
        map.FindAction("Move").canceled -= player.OnMove;
        map.FindAction("Jump").performed -= player.OnJump;
        map.FindAction("Attack").performed -= player.OnAttack;
        map.FindAction("Parry").performed -= player.OnParry;
        map.FindAction("Dodge").performed -= player.OnDodge;
        map.Disable();
    }
}