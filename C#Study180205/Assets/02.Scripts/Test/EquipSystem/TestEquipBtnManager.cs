using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEquipBtnManager : MonoBehaviour {
    public UILabel WeaponModeLabel;

    public GameObject ShootBtn;

    public bool isFired
    {
        get;
        private set;
    }

    private static TestEquipBtnManager instance = null;

    public static TestEquipBtnManager Instance
    {
        get
        {
            return instance;
        }
    }

    void Awake()
    {
        if (instance != null)
        {
            DestroyImmediate(gameObject);
            return;
        }

        instance = this;

        DontDestroyOnLoad(this);
    }

    void Start()
    {
        UIEventListener.Get(ShootBtn).onPress = OnPressShootBtn;
    }

    void Update()
    {
        if (ShootBtn.GetComponent<UIButton>().state == UIButtonColor.State.Normal)
        {
            isFired = false;
        }
        //else
        //{
        //    isFired = true;
        //}
    }

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
        isFired = false;
        //EquipPlayerManager.Instance.StartFire();
    }

    public void OnPressShootBtn(GameObject sender, bool isPressed)
    {
        if(!isFired)
        {
            isFired = true;
            EquipPlayerManager.Instance.StartFire();
        }
    }
}
