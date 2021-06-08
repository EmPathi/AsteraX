using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class CustomizationManager : MonoBehaviour {
	public static CustomizationManager Instance { get; private set; }

	[SerializeField] GameObject _customizationPanel;
	[SerializeField] GameObject _customizationCamera;
	[SerializeField] List<ShipPartOptionsSO> _shipPartOptions;
	[SerializeField] List<ShipPartToggle> _listShipPartToggles;

	private Dictionary<ShipPartType, ShipPartOptionsSO> _dictShipPartOptions;
	private Dictionary<ShipPartType, int> _dictShipPartOptionEquipped;
	private Dictionary<ShipPartType, List<ShipPartToggle>> _dictShipPartTypeToShipPartToggles;

	private void Awake() {
		if(Instance == null) {
			Instance = this;

			InitDictShipPartOptions();
			InitDictShipPartEquipped();
			InitDictShipPartTypeToShipPartToggles();
			LoadSavedCustomizationsEquipped();
		} else {
			Destroy(this.gameObject);
		}
	}

	private void OnEnable() {
		_customizationPanel.SetActive(true);
		_customizationCamera.SetActive(true);
		SetAvailableOptions();
	}

	private void OnDisable() {
		_customizationPanel.SetActive(false);
		_customizationCamera.SetActive(false);
	}

	public void ChangePartSelected(ShipPartType spt, int partIndex) {
		_dictShipPartOptionEquipped[spt] = partIndex;
		GameEvents.ShipPartChanged(spt);
		SaveGameManager.SetCustomizationsEquipped(_dictShipPartOptionEquipped);
	}

	public ShipPartDetails GetCurrentlyEquipedCustomizationOption(ShipPartType spt) {
		return _dictShipPartOptions[spt].shipPartDetails[_dictShipPartOptionEquipped[spt]];
	}

	public void InitDictShipPartEquipped() {
		_dictShipPartOptionEquipped = new Dictionary<ShipPartType, int>();

		foreach(ShipPartType spt in Enum.GetValues(typeof(ShipPartType))) {
			_dictShipPartOptionEquipped.Add(spt, 0);
		}
	}

	private void InitDictShipPartOptions() {
		_dictShipPartOptions = new Dictionary<ShipPartType, ShipPartOptionsSO>();

		foreach(ShipPartOptionsSO spoSO in _shipPartOptions) {
			_dictShipPartOptions.Add(spoSO.shipPartType, spoSO);
		}
	}

	private void InitDictShipPartTypeToShipPartToggles () {
		_dictShipPartTypeToShipPartToggles = new Dictionary<ShipPartType, List<ShipPartToggle>>();

		foreach(ShipPartToggle spt in _listShipPartToggles) {
			if(!_dictShipPartTypeToShipPartToggles.ContainsKey(spt.ShipPartType)) {
				_dictShipPartTypeToShipPartToggles.Add(spt.ShipPartType, new List<ShipPartToggle>());
			}
			_dictShipPartTypeToShipPartToggles[spt.ShipPartType].Add(spt);
		}
	}

	private void LoadSavedCustomizationsEquipped() {
		if(SaveGameManager.SaveData.CustomizationsEquipped != null) {
			foreach(SaveGameManager.CustomizationEquipped ce in SaveGameManager.SaveData.CustomizationsEquipped) {
				_dictShipPartOptionEquipped[ce.ShipPartType] = ce.OptionEquipped;
			}
		}
	}

	public void SetAvailableOptions() {
		if(AchievementManager.Instance != null) {
			foreach(ShipPartType spt in Enum.GetValues(typeof(ShipPartType))) {
				for(int i = 0; i<_dictShipPartTypeToShipPartToggles[spt].Count; i++) {
					ShipPartDetails spd = _dictShipPartOptions[spt].shipPartDetails[i];
					_dictShipPartTypeToShipPartToggles[spt][i].GetComponentInChildren<Text>().text = spd.name;
					_dictShipPartTypeToShipPartToggles[spt][i].Toggle.interactable = ((spd.achievementNameToUnlock == "") || (AchievementManager.Instance.IsAchievementComplete(spd.achievementNameToUnlock)));
					if(_dictShipPartOptionEquipped[spt] == _dictShipPartTypeToShipPartToggles[spt][i].ShipPartIndex) {
						_dictShipPartTypeToShipPartToggles[spt][i].Toggle.isOn = true;
					}
				}
			}
		}
	}

}
