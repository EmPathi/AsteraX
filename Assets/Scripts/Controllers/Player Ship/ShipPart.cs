using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipPart : MonoBehaviour {
	public ShipPartType ShipPartType;
	private GameObject _spawnedPart;

	void Awake() {
		GameEvents.OnShipPartChanged += OnShipPartChanged;
	}

	private void Start() {
		OnShipPartChanged(ShipPartType);
	}

	private void OnDestroy() {
		GameEvents.OnShipPartChanged -= OnShipPartChanged;
	}

	private void OnShipPartChanged(ShipPartType spt) {
		if(spt == this.ShipPartType) {
			Destroy(_spawnedPart);
			GameObject prefab = CustomizationManager.Instance.GetCurrentlyEquipedCustomizationOption(ShipPartType).prefab;
			_spawnedPart = Instantiate(prefab, this.transform.position, Quaternion.Euler(0,0,0), this.transform);
			_spawnedPart.transform.localRotation = prefab.transform.rotation;
		}
	}
}
