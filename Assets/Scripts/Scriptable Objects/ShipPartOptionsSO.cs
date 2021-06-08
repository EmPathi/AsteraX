using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/ShipPartOptionsSO", fileName = "ShipPartOptionsSO.asset")]
[System.Serializable]
public class ShipPartOptionsSO: ScriptableObject {
	public ShipPartType shipPartType;
	public ShipPartDetails[] shipPartDetails;
}

[System.Serializable]
public class ShipPartDetails {
	public string name;
	public GameObject prefab;
	public string achievementNameToUnlock;
}

public enum ShipPartType {
	Body,
	Turret
}
