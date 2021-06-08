using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCursorPosition : MonoBehaviour {
	void Update () {
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hitInfo;

		if(Physics.Raycast(ray, out hitInfo, 1 << 13)) {
			Vector3 direction = hitInfo.point - transform.position;
			direction.z = 0;
			this.transform.rotation = Quaternion.LookRotation(direction, -Vector3.forward);
		}
	}
}
