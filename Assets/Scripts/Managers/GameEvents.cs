using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvents : MonoBehaviour {
	public delegate void BasicEvent();
	public delegate void IntEvent(int value);
	public delegate void InstantiateEvent(Vector3 position, Vector3 direction);
	public delegate void StateChangeEvent(GameState previousState, GameState newState);
	public delegate void ShipPartChangeEvent(ShipPartType spt);

	public static event InstantiateEvent OnCreateBullet;
	public static event IntEvent OnAddScore;
	public static event IntEvent OnScoreUpdated;
	public static event IntEvent OnJumpUsed;
	public static event BasicEvent OnPlayerShipDestroyed;
	public static event BasicEvent OnPlayerShipDamaged;
	public static event BasicEvent OnPlayerShipSpawned;
	public static event BasicEvent OnGamePaused;
	public static event BasicEvent OnGameUnpaused;
	public static event BasicEvent OnAsteroidDestroyed;
	public static event BasicEvent OnAsteroidShot;
	public static event BasicEvent OnTrickshotHit;
	public static event IntEvent OnLevelChange;
	public static event StateChangeEvent OnGameStateChanged;
	public static event ShipPartChangeEvent OnShipPartChanged;

	public static void CreateBullet(Vector3 position, Vector3 direction) {
		if(OnCreateBullet != null) {
			OnCreateBullet.Invoke(position, direction);
		}
	}

	public static void AddScore(int scoreDelta) {
		if(OnAddScore != null) {
			OnAddScore.Invoke(scoreDelta);
		}
	}

	public static void ScoreUpdated(int newScore) {
		if(OnScoreUpdated != null) {
			OnScoreUpdated.Invoke(newScore);
		}
	}

	public static void JumpUsed(int jumpsRemaining) {
		if(OnJumpUsed != null) {
			OnJumpUsed.Invoke(jumpsRemaining);
		}
	}

	public static void PlayerShipDestroyed() {
		if(OnPlayerShipDestroyed != null) {
			OnPlayerShipDestroyed.Invoke();
		}
	}

	public static void PlayerShipDamaged() {
		if(OnPlayerShipDamaged != null) {
			OnPlayerShipDamaged.Invoke();
		}
	}

	public static void PlayerShipSpawned() {
		if(OnPlayerShipSpawned != null) {
			OnPlayerShipSpawned.Invoke();
		}
	}

	public static void GamePaused() {
		if(OnGamePaused != null) {
			OnGamePaused.Invoke();
		}
	}

	public static void GameUnpaused() {
		if(OnGameUnpaused != null) {
			OnGameUnpaused.Invoke();
		}
	}

	public static void GameStateChanged(GameState previousState, GameState newState) {
		if(OnGameStateChanged != null) {
			OnGameStateChanged.Invoke(previousState, newState);
		}
	}

	public static void AsteroidDestroyed() {
		if(OnAsteroidDestroyed != null) {
			OnAsteroidDestroyed.Invoke();
		}
	}

	public static void AsteroidShot() {
		if(OnAsteroidShot != null) {
			OnAsteroidShot.Invoke();
		}
	}

	public static void TrickshotHit() {
		if(OnTrickshotHit != null) {
			OnTrickshotHit.Invoke();
		}
	}

	public static void LevelChange(int level) {
		if(OnLevelChange != null) {
			OnLevelChange.Invoke(level);
		}
	}
	
	public static void ShipPartChanged(ShipPartType spt) {
		if(OnShipPartChanged != null) {
			OnShipPartChanged.Invoke(spt);
		}
	}
}
