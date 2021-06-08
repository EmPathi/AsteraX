using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class ShipPartToggle : MonoBehaviour {
	public ShipPartType ShipPartType;
	public int ShipPartIndex;
	public Toggle Toggle { get; private set; }
	
	void Awake () {
		this.Toggle = GetComponent<Toggle>();
	}

	public void OnSelect() {
		CustomizationManager.Instance.ChangePartSelected(ShipPartType, ShipPartIndex);
	}
}
