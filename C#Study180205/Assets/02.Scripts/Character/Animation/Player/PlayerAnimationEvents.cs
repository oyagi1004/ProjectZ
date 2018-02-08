using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationEvents : MonoBehaviour {
    public void MuzzleFire()
    {
        EquipPlayerManager.Instance.ShootToGun();
        Debug.Log("Shoot");
    }
}
