using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEvent : MonoBehaviour {
    public void TurnOffIK(string msg)
    {
        PlayerManager.Instance.EnableIK(false);
        Debug.Log(msg);
    }


}
