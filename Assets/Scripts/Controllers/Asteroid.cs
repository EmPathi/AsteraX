using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Asteroid : MonoBehaviour {
	[SerializeField] AsteroidSO _asteroidConfig;

	private Rigidbody _rb;
	
	private int _size;
	public int Size {
		get { return _size; }
		set {
			_size = value;
			float scale = _asteroidConfig.initialScale * _size / this.gameObject.transform.lossyScale .x;
			this.gameObject.transform.localScale = new Vector3(scale, scale, scale);
		} 
	}
	
	private void Awake () {
		_rb = this.gameObject.GetComponent<Rigidbody>();
	}

	public void AddRandomVelocity() {
		if(_rb != null) {
			float speed = _asteroidConfig.initialSpeed;
			_rb.AddForce(new Vector3((Random.value-0.5f)*speed/Size, (Random.value-0.5f)*speed/Size, 0), ForceMode.Impulse);
		}
	}

	public void AddRandomTorque() {
		if(_rb != null) {
			float torque = _asteroidConfig.initialTorque;
			_rb.AddTorque(new Vector3((Random.value-0.5f)*torque/Size, (Random.value-0.5f)*torque/Size, (Random.value-0.5f)*torque/Size));
		}
	}

	private void OnCollisionEnter(Collision collision) {
		// if the layer the object we collided with is a layer that should break this asteroid
		if((_asteroidConfig.layersThatDestroyAsteroids.value & (1<<collision.gameObject.layer)) != 0) {
			Break();

			// if the layer the object we collided with is a layer where points should be granted
			if((_asteroidConfig.layersThatGivePointsWhenDestroying.value & (1<<collision.gameObject.layer)) != 0) {
				GameEvents.AddScore(_asteroidConfig.pointsPerSize[this.Size]);
				GameEvents.AsteroidShot();
			}
		}
	}

	private void Break() {
		ReleaseChildren();
		CreateExplosionEffect();
		this.gameObject.SetActive(false); // return asteroid to pool
		GameEvents.AsteroidDestroyed();
	}

	private void ReleaseChildren() {
		int childCount = this.gameObject.transform.childCount; // child count needs to be cached here because as we remove children during the loop, transform.childCount changes
		for(int i = 0; i < childCount; i++) {
			Asteroid childAster = this.gameObject.transform.GetChild(0).gameObject.GetComponent<Asteroid>();
			childAster.Release();
		}
	}

	public void Release() {
		// release from parent if parent is an asteroid (and not the object pool)
		if((transform.parent != null) && (transform.parent.tag == "Asteroid")) {
			transform.parent = null;
		}

		// set z back to 0, while rotating around it's prior parent it likely has a z value
		transform.position = new Vector3(transform.position.x, transform.position.y, 0);

		// "enable" the rigidbody
		_rb.isKinematic = false;
		AddRandomVelocity();
		AddRandomTorque();

		// Enable the screenwrap component
		GetComponent<ScreenWrap>().enabled = true;
	}

	public void CreateExplosionEffect() {
		GameObject go = Instantiate(_asteroidConfig.listExplosionEffectPrefabs[Random.Range(0, _asteroidConfig.listExplosionEffectPrefabs.Count)], this.transform.position, Quaternion.identity);
		go.transform.localScale = Vector3.one * this._size*0.33f;
		Destroy(go, 2);

	}

	public void Init() {
		_rb.isKinematic = true;
		_rb.velocity = Vector3.zero;
		this.transform.localScale = Vector3.one;
		
		GetComponent<ScreenWrap>().enabled = false;
	}
}
