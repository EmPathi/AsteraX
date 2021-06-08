using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour {
	[SerializeField] GameObject bulletPrefab;
	[SerializeField] int poolSize;

	private List<GameObject> objectPool;
	
	void Start () {
		GameEvents.OnCreateBullet += OnCreateBullet;

		InitializePool();
	}

	private void OnDestroy() {
		GameEvents.OnCreateBullet -= OnCreateBullet;
	}

	private void OnCreateBullet(Vector3 position, Vector3 direction) {
		GameObject bulletToUse = objectPool.Find(x => !x.activeSelf);

		if(bulletToUse != null) {
			bulletToUse.transform.position = position;
			bulletToUse.transform.rotation = Quaternion.LookRotation(direction);
			bulletToUse.SetActive(true);
		}
	}

	private void InitializePool() {
		objectPool = new List<GameObject>();

		for(int i = 0; i < poolSize; i++) {
			GameObject newBullet = Instantiate(bulletPrefab, Vector3.zero, Quaternion.identity);
			newBullet.transform.parent = this.transform;
			newBullet.SetActive(false);

			objectPool.Add(newBullet);
		}
	}
}
