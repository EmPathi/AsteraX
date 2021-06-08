using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyIfStandalone : MonoBehaviour {
	void Awake () {
#if !MOBILE_INPUT
		Destroy(this.gameObject);
#endif
	}
}
