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
                continue;
            }

            player.currentWeapon = data.weaponType;
            player.ApplyWeapon();

            PlayerStats stats = player.GetComponent<PlayerStats>();
            if (stats != null)
            {
                stats.hpMultiplier = data.hpMultiplier;
                stats.moveSpeedMultiplier = data.moveSpeedMultiplier;
                stats.attackMultiplier = data.attackMultiplier;
                stats.attackSpeedMultiplier = data.attackSpeedMultiplier;
                stats.attackRangeMultiplier = data.attackRangeMultiplier;
                stats.knockbackMultiplier = data.knockbackMultiplier;
                stats.knockbackResistBonus = data.knockbackResistBonus;


                stats.RecalculateStats();
            }
        }
    }
}