using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveOnlyInEditor : MonoBehaviour {
	void Start () {
		if(!Application.isEditor) {
			this.gameObject.SetActive(false);
		}
	}
}
