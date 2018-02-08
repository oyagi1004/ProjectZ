using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEquipBtnManager : MonoBehaviour {
    public UILabel WeaponModeLabel;

    public void OnTouchEquipModeChangeBtn()
    {
        if(EquipPlayerManager.Instance.weaponMode == EquipPlayerManager.WeaponMode.NONE)
        {
            EquipPlayerManager.Instance.weaponMode = EquipPlayerManager.WeaponMode.PRIMARY;
            WeaponModeLabel.text = "WeaponMode: Primary";
        }
        else if (EquipPlayerManager.Instance.weaponMode == EquipPlayerManager.WeaponMode.PRIMARY)
        {
            EquipPlayerManager.Instance.weaponMode = EquipPlayerManager.WeaponMode.SUB;
            WeaponModeLabel.text = "WeaponMode: Sub";
        }
        else if (EquipPlayerManager.Instance.weaponMode == EquipPlayerManager.WeaponMode.SUB)
        {
            EquipPlayerManager.Instance.weaponMode = EquipPlayerManager.WeaponMode.NONE;
            WeaponModeLabel.text = "WeaponMode: None";
        }
        EquipPlayerManager.Instance.ChangeWeaponMode();
    }
	
    public void OnTouchShootBtn()
    {
        EquipPlayerManager.Instance.anim.SetTrigger("Shoot");
        //EquipPlayerManager.Instance.anim.SetBool("ShootSwitch", true);
    }
}
