using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {
	public bool IsTrick { get; private set; }
	[SerializeField] BulletSO _bulletConfig;

	private Rigidbody _rb;
	private ScreenWrap _sw;

	private void Awake() {
		_rb = this.GetComponent<Rigidbody>();
		_sw = this.GetComponent<ScreenWrap>();
		_sw.OnWrapOccurred += SetTrickShot;
		GameEvents.OnGameStateChanged += OnStateChanged;
	}

	private void OnEnable() {
		IsTrick = false;
		_rb.velocity = Vector3.zero;
		_rb.AddForce(transform.forward * _bulletConfig.speed, ForceMode.Impulse);
		StartCoroutine("DelayedInactivate");
	}

	private void OnDestroy() {
		_sw.OnWrapOccurred -= SetTrickShot;
		GameEvents.OnGameStateChanged -= OnStateChanged;
	}

	private void OnCollisionEnter(Collision collision) {
		if(collision.gameObject.tag == "Asteroid") {
			this.gameObject.SetActive(false); // return bullet to pool
			if(IsTrick) {
				GameEvents.TrickshotHit();
			}
		}
	}

	private void OnStateChanged(GameState previousState, GameState newState) {
		if(previousState == GameState.Game) {
			this.gameObject.SetActive(false); // return bullet to pool
		}
	}

	private void SetTrickShot() {
		IsTrick = true;
	}

	private IEnumerator DelayedInactivate() {
		yield return new WaitForSeconds(_bulletConfig.lifetime);

		if(isActiveAndEnabled) {
			this.gameObject.SetActive(false);
		}
	}	
}
