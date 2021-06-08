using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TiltWithVelocity : MonoBehaviour {
	[SerializeField] private float _velocityMultiplier;
	private Rigidbody _rb;
	
	private void Start() {
		_rb = this.GetComponent<Rigidbody>();
	}

	void Update () {
		this.transform.rotation = Quaternion.Euler(_rb.velocity.y * _velocityMultiplier, _rb.velocity.x * -_velocityMultiplier, 0);
	}
}
