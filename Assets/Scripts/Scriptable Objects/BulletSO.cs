using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Scriptable Object", menuName = "ScriptableObjects/BulletSO", order = 4)]
public class BulletSO: ScriptableObject {
	public float speed;
	public float lifetime;
}
