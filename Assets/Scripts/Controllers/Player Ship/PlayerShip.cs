using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerShip : MonoBehaviour {
	[SerializeField] private PlayerShipSO _playerShipConfig;
	[SerializeField] private GameObject _turret;

	private Vector3 _moveDirection;
	private bool _fireOnCooldown = false;
	private bool _firing = false;
	private Rigidbody _rb;

	void Start () {
		_moveDirection = Vector3.zero;
		_rb = this.GetComponent<Rigidbody>();
	}

	private void OnEnable() {
		_fireOnCooldown = false;
	}

	void Update () {
		GetInputs();
		CheckFire();
		Move();
	}

	private void OnCollisionEnter(Collision collision) {
		if(gameObject.activeInHierarchy) {
			// if the layer the object we collided with is a layer that should damage the player ship
			if((_playerShipConfig.layersThatDamageShip.value & (1<<collision.gameObject.layer)) != 0) {
				this.gameObject.SetActive(false);
				GameEvents.PlayerShipDamaged();
			}
		}
	}

	private void CheckFire() {
		if(_firing && !_fireOnCooldown) {
			GameEvents.CreateBullet(this.transform.position, _turret.transform.forward);
			_fireOnCooldown = true;
			StartCoroutine("FireCooldown");
		}
	}

	private IEnumerator FireCooldown() {
		yield return new WaitForSeconds(_playerShipConfig.firePeriod);
		_fireOnCooldown = false;
	}

	void GetInputs() {
		if(CrossPlatformInputManager.GetButton("Fire1")) {
			_firing = true;
		} else {
			_firing = false;
		}

		_moveDirection.x = CrossPlatformInputManager.GetAxis("Horizontal");
		_moveDirection.y = CrossPlatformInputManager.GetAxis("Vertical");
		if(_moveDirection.sqrMagnitude > 1) {
			_moveDirection.Normalize();
		}
	}

	void Move() {
		_rb.velocity = _moveDirection * _playerShipConfig.moveSpeed;
	}
}
