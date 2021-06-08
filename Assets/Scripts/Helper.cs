using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Helper {
	public static Quaternion RandomQuaternion() {
		return Quaternion.Euler(UnityEngine.Random.value*360, UnityEngine.Random.value*360, UnityEngine.Random.value*360);
	}

	public static Vector3 GetRandomVector() {
		float x = Random.value*2-1;
		float y = Random.value*2-1;
		float z = Random.value*2-1;
		return new Vector3(x, y, z);
	}
}
