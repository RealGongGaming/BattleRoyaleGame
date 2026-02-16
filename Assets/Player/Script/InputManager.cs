using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    [Header("Input Actions Asset")]
    public InputActionAsset inputActions;

    [Header("Player Controller")]
    public PlayerController player1;
    public PlayerController player2;
    public PlayerController player3;
    public PlayerController player4;

    [Header("Lobby Controller")]
    public LobbyController lobbyPlayer1;
    public LobbyController lobbyPlayer2;
    public LobbyController lobbyPlayer3;
    public LobbyController lobbyPlayer4;

    [Header("Misc")]
    public StartBattle startBattle;

    private InputActionMap p1Map, p2Map, p3Map, p4Map;
    private InputActionMap UIMap;

    void Awake()
    {
        p1Map = inputActions.FindActionMap("Player1");
        p2Map = inputActions.FindActionMap("Player2");
        p3Map = inputActions.FindActionMap("Player3");
        p4Map = inputActions.FindActionMap("Player4");
        UIMap = inputActions.FindActionMap("UI");

        var gamepads = Gamepad.all;

        InputDevice keyboard = Keyboard.current;
        InputDevice g1 = gamepads.Count > 0 ? gamepads[0] : null;
        InputDevice g2 = gamepads.Count > 1 ? gamepads[1] : null;

        if (player1 != null)
        {
            SetupPlayer(p1Map, player1, keyboard);
            SetupPlayer(p2Map, player2, g1);
            SetupPlayer(p3Map, player3, g2);
            SetupPlayer(p4Map, player4, keyboard);
        }
        else
        {
            SetupPlayer(p1Map, lobbyPlayer1, keyboard);
            SetupPlayer(p2Map, lobbyPlayer2, g1);
            SetupPlayer(p3Map, lobbyPlayer3, g2);
            SetupPlayer(p4Map, lobbyPlayer4, keyboard);

            SetupUI(UIMap, startBattle);
        }
    }

    // UI
    void SetupUI(InputActionMap map, StartBattle handler)
    {
        if (map == null || handler == null) return;

        map.FindAction("Start Game").performed += handler.Play;
        map.Enable();
    }

    void CleanupUI(InputActionMap map, StartBattle handler)
    {
        if (map == null || handler == null) return;

        map.FindAction("Start Game").performed -= handler.Play;
        map.Disable();
    }

    // PlayerController
    void SetupPlayer(InputActionMap map, PlayerController player, InputDevice device)
    {
        if (map == null || player == null) return;
        if (device != null)
        {
            map.devices = new[] { device };
        }
        else
        {
            map.devices = new InputDevice[] { };
        }

        map.FindAction("Move").performed += player.OnMove;
        map.FindAction("Move").canceled += player.OnMove;
        map.FindAction("Jump").performed += player.OnJump;
        map.FindAction("Attack").performed += player.OnAttack;
        map.FindAction("Parry").performed += player.OnParry;
        map.FindAction("Dodge").performed += player.OnDodge;
        map.Enable();
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

    // LobbyController
    void SetupPlayer(InputActionMap map, LobbyController player, InputDevice device)
    {
        if (map == null || player == null) return;
        if (device != null)
        {
            map.devices = new[] { device };
        }
        else
        {
            map.devices = new InputDevice[] { };
        }

        map.FindAction("Move").performed += player.OnMove;
        map.FindAction("Move").canceled += player.OnMove;
        map.FindAction("Jump").performed += player.OnJump;
        map.Enable();
    }

    void CleanupPlayer(InputActionMap map, LobbyController player)
    {
        if (map == null || player == null) return;

        map.FindAction("Move").performed -= player.OnMove;
        map.FindAction("Move").canceled -= player.OnMove;
        map.FindAction("Jump").performed -= player.OnJump;
        map.Disable();
    }

    void OnDestroy()
    {
        if (player1 != null)
        {
            CleanupPlayer(p1Map, player1);
            CleanupPlayer(p2Map, player2);
            CleanupPlayer(p3Map, player3);
            CleanupPlayer(p4Map, player4);
        }
        else
        {
            CleanupPlayer(p1Map, lobbyPlayer1);
            CleanupPlayer(p2Map, lobbyPlayer2);
            CleanupPlayer(p3Map, lobbyPlayer3);
            CleanupPlayer(p4Map, lobbyPlayer4);

            CleanupUI(UIMap, startBattle);
        }
    }
}