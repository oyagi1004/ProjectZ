using UnityEngine;
using System.Collections;

public class MyGizmo : MonoBehaviour {
	public enum MyGizmoType { mark, waypoint};
	
	public MyGizmoType type = MyGizmoType.mark;
	public float _radius = 0.3f;
	public Color _color = Color.yellow;
	
	void OnDrawGizmos(){
		Gizmos.color = _color;
		
		switch( type ){
		case MyGizmoType.mark:
				Gizmos.DrawSphere (transform.position, _radius);
			break;
		case MyGizmoType.waypoint:
				//Gizmos.DrawSphere (transform.position, _radius);
				Gizmos.DrawIcon ( transform.position, "wayPoint");
				Gizmos.DrawWireSphere (transform.position, _radius *4.0f);
			break;
		}
	}
	
}
