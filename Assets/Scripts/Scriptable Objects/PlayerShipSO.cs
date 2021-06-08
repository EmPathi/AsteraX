using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Scriptable Object", menuName = "ScriptableObjects/PlayerShipSO", order = 3)]
public class PlayerShipSO: ScriptableObject {
	public LayerMask layersThatDamageShip;
	public float moveSpeed;
	public float firePeriod;
}
