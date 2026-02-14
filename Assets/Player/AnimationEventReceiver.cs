using UnityEngine;

public class AnimationEventReceiver : MonoBehaviour
{
    [Header("Weapon Hitboxes")]
    public WeaponHitbox hammerHitbox;
    public WeaponHitbox swordHitbox;
    public WeaponHitbox polearmHitbox;
    public WeaponHitbox dualsword1Hitbox;
    public WeaponHitbox dualsword2Hitbox;

    private WeaponHitbox activeHitbox1;
    private WeaponHitbox activeHitbox2;

    public void SetActiveWeapon(WeaponType weapon)
    {
        activeHitbox1 = null;
        activeHitbox2 = null;

        switch (weapon)
        {
            case WeaponType.Hammer:
                activeHitbox1 = hammerHitbox;
                break;
            case WeaponType.SwordAndShield:
                activeHitbox1 = swordHitbox;
                break;
            case WeaponType.Polearm:
                activeHitbox1 = polearmHitbox;
                break;
            case WeaponType.Dualsword:
                activeHitbox1 = dualsword1Hitbox;
                activeHitbox2 = dualsword2Hitbox;
                break;
        }
    }

    public void EnableHitbox()
    {
        if (activeHitbox1 != null) activeHitbox1.EnableHitbox();
        if (activeHitbox2 != null) activeHitbox2.EnableHitbox();
    }

    public void DisableHitbox()
    {
        if (activeHitbox1 != null) activeHitbox1.DisableHitbox();
        if (activeHitbox2 != null) activeHitbox2.DisableHitbox();
    }
}