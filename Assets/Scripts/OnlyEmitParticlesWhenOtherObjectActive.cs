using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class OnlyEmitParticlesWhenOtherObjectActive : MonoBehaviour {
	[SerializeField] private GameObject otherObject;

	private ParticleSystem.EmissionModule psEM;
	private bool emitting;

	void Start() {
		psEM = GetComponent<ParticleSystem>().emission;
		emitting = true;
	}

	void Update() {
		if(emitting && !otherObject.activeInHierarchy) {
			psEM.enabled = false; // particle emission is disables instead of using ParticleSystem.Play() because Play() will be delayed until all previously created particles dissipate, which we don't want here
			emitting = false;
		} else if(!emitting && otherObject.activeInHierarchy) {
			Invoke("EnableEmitter", 0.05f); // Emission activation is delayed, otherwise you can get a line of particals from the object's previous location to it's new location when it is re-enabled
		}
	}

	void EnableEmitter() {
		psEM.enabled = true;
		emitting = true;
	}
}
