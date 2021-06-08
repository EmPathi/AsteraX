using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Scriptable Object", menuName = "ScriptableObjects/AsteroidSO", order = 1)]
public class AsteroidSO : ScriptableObject {
	public List<GameObject> asteroidPrefabsStandalone;
	public List<GameObject> asteroidPrefabsMobile;
	public LayerMask layersThatDestroyAsteroids;
	public LayerMask layersThatGivePointsWhenDestroying;
	public float initialScale;
	public float initialSpeed;
	public float initialTorque;
	public List<int> pointsPerSize;
	public List<GameObject> listExplosionEffectPrefabs;

	public GameObject GetRandomAsteroid() {
#if MOBILE_INPUT
		return asteroidPrefabsMobile[UnityEngine.Random.Range(0, asteroidPrefabsMobile.Count)];
#else
		return asteroidPrefabsStandalone[UnityEngine.Random.Range(0, asteroidPrefabsStandalone.Count)];
#endif
	}
}
