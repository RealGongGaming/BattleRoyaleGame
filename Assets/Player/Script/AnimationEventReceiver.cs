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

    bool isRightHand = true;

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
        if (activeHitbox2 != null)
        {
            if (isRightHand)
            {
                activeHitbox1.DisableHitbox();
                activeHitbox2.EnableHitbox();
                isRightHand = false;
            }
            else
            {
                activeHitbox1.EnableHitbox();
                activeHitbox2.DisableHitbox();
                isRightHand = true;
            }
        }
        else
        {
            activeHitbox1.EnableHitbox();
        }
    }

    public void DisableHitbox()
    {
        if (activeHitbox2 != null)
        {
            if (isRightHand)
            {
                activeHitbox1.DisableHitbox();
                activeHitbox2.DisableHitbox();
            }
            else
            {
                activeHitbox1.DisableHitbox();
                activeHitbox2.DisableHitbox();
            }
        }
        else
        {
            activeHitbox1.DisableHitbox();
        }
    }
}