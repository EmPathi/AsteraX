using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ShipSpawnEffect : MonoBehaviour {
	private void Awake() {
		GameEvents.OnPlayerShipSpawned += OnPlayerShipSpawned;
	}

	private void OnDestroy() {
		GameEvents.OnPlayerShipSpawned -= OnPlayerShipSpawned;
	}

	private void OnPlayerShipSpawned() {
		this.GetComponent<ParticleSystem>().Play();
	}
}
