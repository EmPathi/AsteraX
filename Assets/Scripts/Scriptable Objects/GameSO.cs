using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Scriptable Object", menuName = "ScriptableObjects/GameSO", order = 2)]
public class GameSO: ScriptableObject {
	public int startingJumps;
	public float jumpDuration;
	public float minimumDistanceFromAsteroidToSpawn;
	public float gameOverScreenDuration;
	public int sceneIndexToLoadAfterGameOver;
	public float levelAdvancePanelDuration;
}