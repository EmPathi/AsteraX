using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidPool : MonoBehaviour {
	public static AsteroidPool Instance { get; private set; }

	[SerializeField] AsteroidSO _asteroidConfig;
	[SerializeField] int _poolSize;

	private List<GameObject> _objectPool;
	
	void Awake() {
		if(Instance == null) {
			Instance = this;

			InitializePool();
		} else {
			Destroy(this.gameObject);
		}
	}

	private void InitializePool() {
		_objectPool = new List<GameObject>();

		for(int i = 0; i < _poolSize; i++) {
			GameObject newAsteroid = Instantiate(_asteroidConfig.GetRandomAsteroid(), Vector3.zero, Quaternion.identity);
			newAsteroid.transform.parent = this.transform;
			newAsteroid.SetActive(false);

			_objectPool.Add(newAsteroid);
		}
	}

	public Asteroid GetAsteroid() {
		Asteroid asterToUse = _objectPool.Find(x => !x.activeInHierarchy).GetComponent<Asteroid>();

		if(asterToUse != null) {
			asterToUse.gameObject.SetActive(true);
			asterToUse.Init();
		}

		return asterToUse;
	}

	public List<GameObject> GetActiveAsteroids() {
		return _objectPool.FindAll(x => x.activeSelf);
	}
}
