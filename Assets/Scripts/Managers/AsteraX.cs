using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Analytics;

public enum GameState {
	New,
	MainMenu,
	LevelAdvance,
	Game,
	GameOver
}

public class AsteraX: MonoBehaviour {
	public static AsteraX Instance { get; private set; }

	public GameSO GameConfig;
	public LevelSO LevelConfig;
	
	public int CurrentLevel { get; private set; }
	public int JumpsRemaining { get; private set; }
	public bool IsPaused { get; private set; }

	[SerializeField] PlayerShip _playerShip;

	private GameState _currentGameState = GameState.New;

	private void Awake() {
		if(AsteraX.Instance == null) {
			AsteraX.Instance = this;

			GameEvents.OnPlayerShipDamaged += OnPlayerShipDamaged;
			GameEvents.OnAsteroidDestroyed += OnAsteroidDestroyed;

			UnityEngine.RemoteSettings.Updated += LoadLevelProgressionFromRemoteSettings;

			LoadLevelProgressionFromRemoteSettings();
		} else {
			Destroy(this.gameObject);
		}
	}

	private void Start() {
		ChangeGameState(GameState.MainMenu);
		CurrentLevel = -1;
		JumpsRemaining = GameConfig.startingJumps;
	}

	private void OnDestroy() {
		GameEvents.OnPlayerShipDamaged -= OnPlayerShipDamaged;
		GameEvents.OnAsteroidDestroyed -= OnAsteroidDestroyed;
		UnityEngine.RemoteSettings.Updated -= LoadLevelProgressionFromRemoteSettings;

		SaveGameManager.Save();
		AnalyticsManager.SendShipPartChoice();
	}

	private void OnAsteroidDestroyed() {
		CheckLevelComplete();
	}

	public void OnPlayerShipDamaged() {
		if(AsteraX.Instance.JumpsRemaining > 0) {
			StartCoroutine("JumpShip");
		} else {
			GameOver();
		}
	}
	
	private void ChangeGameState(GameState newGameState) {
		GameEvents.GameStateChanged(_currentGameState, newGameState);
		_currentGameState = newGameState;
	}

	private void CheckLevelComplete() {
		if(AsteroidPool.Instance.GetActiveAsteroids().Count == 0) {
			StartCoroutine("LevelAdvance");
		}
	}

	public void DeleteSaveData() {
		ScoreManager.Instance.ResetHighScore();
		SaveGameManager.DeleteSaveData();
		AchievementManager.Instance.InitAchievementData();
		CustomizationManager.Instance.InitDictShipPartEquipped();
	}

	public void GameOver() {
		ChangeGameState(GameState.GameOver);
		AnalyticsManager.SendGameOver(ScoreManager.Instance.CurrentScore, CurrentLevel, ScoreManager.Instance.HighScoreReached);
		StartCoroutine("WaitThenRestartGame");
	}

	private void InitAsteroid() {
		Asteroid aster = AsteroidPool.Instance.GetAsteroid();

		if(aster != null) {
			aster.gameObject.transform.position = GameBounds.Instance.GetRandomLocationWithinBounds() * 0.8f;
			aster.gameObject.transform.rotation = Helper.RandomQuaternion();
			aster.Size = LevelConfig.levels[CurrentLevel].numberOfLayers;
			aster.Release();

			InitChildrenAsteroids(aster.transform, aster.Size - 1);
		}
	}

	private void InitAsteroids() {
		for(int i = 0; i < LevelConfig.levels[CurrentLevel].numberOfAsteroids; i++) {
			InitAsteroid();
		}
	}

	private void InitChildrenAsteroids(Transform parent, int size) {
		if(size > 0) {
			for(int i = 0; i < LevelConfig.levels[CurrentLevel].numberOfChildrenPerAsteroid; i++) {
				Asteroid aster = AsteroidPool.Instance.GetAsteroid();
				if(aster != null) {
					aster.gameObject.transform.parent = parent;
					aster.gameObject.transform.position = parent.transform.position+Helper.GetRandomVector()/2;
					aster.gameObject.transform.rotation = Helper.RandomQuaternion();
					aster.Size = size;
					InitChildrenAsteroids(aster.transform, size - 1);
				}
			}
		}
	}

	private bool IsLocationSafe(Vector3 location) {
		float distanceFromClosestAsteroid = float.MaxValue;
		float distanceFromAster;

		foreach(GameObject aster in AsteroidPool.Instance.GetActiveAsteroids()) {
			distanceFromAster = Vector3.Distance(aster.transform.position, location);
			if(distanceFromAster < distanceFromClosestAsteroid) {
				distanceFromClosestAsteroid = distanceFromAster;
			}
		}

		return distanceFromClosestAsteroid > GameConfig.minimumDistanceFromAsteroidToSpawn;
	}

	public IEnumerator JumpShip() {
		JumpsRemaining--;
		GameEvents.JumpUsed(JumpsRemaining);
		yield return new WaitForSeconds(GameConfig.jumpDuration);
		StartCoroutine("PlaceShipInSafeLocation");
	}

	private IEnumerator LevelAdvance() {
		_playerShip.gameObject.SetActive(false);

		if(CurrentLevel+1 >= LevelConfig.levels.Count) {
			GameOver();
		} else {
			CurrentLevel++;
			AnalyticsManager.SendLevelStart(CurrentLevel);
			GameEvents.LevelChange(CurrentLevel);
			ChangeGameState(GameState.LevelAdvance);
			yield return new WaitForSeconds(GameConfig.levelAdvancePanelDuration/2);
			InitAsteroids();
			yield return new WaitForSeconds(GameConfig.levelAdvancePanelDuration/2);
			ChangeGameState(GameState.Game);
			StartCoroutine("PlaceShipInSafeLocation");
		}
	}

	private void LoadLevelProgressionFromRemoteSettings() {
		LevelConfig = LevelSO.CreateFromLevelProgressionString(UnityEngine.RemoteSettings.GetString("LevelProgression"));
	}

	public void Pause(bool pause) {
		IsPaused = pause;
		Time.timeScale = (IsPaused ? 0 : 1);
		CustomizationManager.Instance.enabled = IsPaused;
	}

	private IEnumerator PlaceShipInSafeLocation() {
		bool safeLocationFound = false;
		Vector3 testLocation = _playerShip.transform.position;

		while(!safeLocationFound) {
			testLocation = GameBounds.Instance.GetRandomLocationWithinBounds()*0.8f;

			if(IsLocationSafe(testLocation)) {
				safeLocationFound = true;
			}
			yield return null; // try again next frame
		}

		_playerShip.transform.position = testLocation;
		_playerShip.gameObject.SetActive(true);

		GameEvents.PlayerShipSpawned();
	}

	public void Play() {
		GameEvents.GameStateChanged(GameState.MainMenu, GameState.Game);
		StartCoroutine("LevelAdvance");
	}
	
	public IEnumerator WaitThenRestartGame() {
		yield return new WaitForSeconds(GameConfig.gameOverScreenDuration);
		SceneManager.LoadScene(GameConfig.sceneIndexToLoadAfterGameOver);
	}
}


