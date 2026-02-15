using UnityEngine;
using System.Linq;

public class PlayerSpawner : MonoBehaviour
{
    void Start()
    {
        InitializeScenePlayers();
    }

    private void InitializeScenePlayers()
    {
        if (DataManager.instance == null) return;

        WeaponSelector[] existingPlayers = FindObjectsByType<WeaponSelector>(FindObjectsSortMode.None);

        foreach (WeaponSelector player in existingPlayers)
        {
            string id = player.gameObject.name;

            PlayerData data = DataManager.instance.players.FirstOrDefault(p => p.playerID == id);

            if (data == null || !data.isReady)
            {
                
                Destroy(player.gameObject);
            }
            else
            {
             
                player.currentWeapon = data.weaponType;
                player.ApplyWeapon();
            }
        }
    }
}