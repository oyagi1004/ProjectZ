using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathHelper : MonoBehaviour {

    //-180~180
    public static float GetAngleByAtan(Vector3 vStart, Vector3 vEnd)
    {
        Vector3 v = vEnd - vStart;
        
        return Mathf.Atan2(v.z, v.x) * Mathf.Rad2Deg;
    }

    //0~180
    public static float GetAngleByAcos(Vector3 vStart, Vector3 vEnd)
    {
        float Dot = Vector3.Dot(vStart, vEnd);

        float Angle = Mathf.Acos(Dot);

        return Angle * Mathf.Rad2Deg;
    }
    
}
