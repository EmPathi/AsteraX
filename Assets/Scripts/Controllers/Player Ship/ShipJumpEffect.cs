using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ShipJumpEffect : MonoBehaviour {
	private void Awake() {
		GameEvents.OnJumpUsed += OnJumpUsed;
	}

	private void OnDestroy() {
		GameEvents.OnJumpUsed -= OnJumpUsed;
	}

	private void OnJumpUsed(int jumpsRemaining) {
		this.GetComponent<ParticleSystem>().Play();
	}
}
