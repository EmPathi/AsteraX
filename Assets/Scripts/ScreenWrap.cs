using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenWrap: MonoBehaviour {
	public delegate void BasicEvent();
	public event BasicEvent OnWrapOccurred;

	[SerializeField] List<ParticleSystem> particleSystemsToDisableDuringWrap;

	private void OnTriggerEnter(Collider other) {
		if(other.tag == "BackupBounds") {
			ScreenBounce();
		}
	}

	private void OnTriggerExit(Collider other) {
		if(this.isActiveAndEnabled) {
			if(other.tag == "OnScreenBounds") {
				CheckScreenWrap();
			}
		}
	}

	private void CheckScreenWrap() {
		if(Mathf.Abs(this.transform.position.x) > 16) {
			StopChildParticleSystems();
			this.transform.position = new Vector3(-this.transform.position.x, this.transform.position.y, this.transform.position.z);
			PlayChildParticleSystems();
			FireWrapOccurred();
		}
		if(Mathf.Abs(this.transform.position.y) > 9) {
			StopChildParticleSystems();
			this.transform.position = new Vector3(this.transform.position.x, -this.transform.position.y, this.transform.position.z);
			PlayChildParticleSystems();
			FireWrapOccurred();
		}
	}

	private void ScreenBounce() {
		Rigidbody rb = this.GetComponent<Rigidbody>();

		if(rb != null) {
			Vector3 direction = Vector3.zero - this.transform.position;
			rb.velocity = direction;
		}
	}

	public void FireWrapOccurred() {
		if(OnWrapOccurred != null) {
			OnWrapOccurred.Invoke();
		}
	}

	public void PlayChildParticleSystems() {
		foreach(ParticleSystem ps in particleSystemsToDisableDuringWrap) {
			ps.Play();
		}
	}

	private void StopChildParticleSystems() {
		foreach(ParticleSystem ps in particleSystemsToDisableDuringWrap) {
			if(ps.emission.enabled) {
				ps.Stop();
			}
		}
	}
}
