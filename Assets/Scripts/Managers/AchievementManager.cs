using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementManager: MonoBehaviour {
	public static AchievementManager Instance { get; private set; }

	[SerializeField] AchievementSO _achievementConfig;

	private Dictionary<Achievment.AchievementType, int> _mapAchievementTypeToCurrentStepCount;
	private Dictionary<string, bool> _mapAchievementToCompletion;

	private void Awake() {
		if(Instance == null) {
			Instance = this;

			GameEvents.OnCreateBullet += OnBulletFired;
			GameEvents.OnTrickshotHit += OnTrickshotHit;
			GameEvents.OnAsteroidShot += OnAsteroidShot;
			GameEvents.OnScoreUpdated += OnScoreUpdated;
			GameEvents.OnLevelChange += OnLevelChange;

			InitAchievementData();
			LoadFromSaveData();
		} else {
			Destroy(this.gameObject);
		}
	}

	private void OnDestroy() {
		GameEvents.OnCreateBullet -= OnBulletFired;
		GameEvents.OnTrickshotHit -= OnTrickshotHit;
		GameEvents.OnAsteroidShot -= OnAsteroidShot;
		GameEvents.OnScoreUpdated -= OnScoreUpdated;
		GameEvents.OnLevelChange -= OnLevelChange;
	}

	private void OnBulletFired(Vector3 pos, Vector3 rot) {
		_mapAchievementTypeToCurrentStepCount[Achievment.AchievementType.ShotsFired] += 1;
		CheckAchievements();
	}

	private void OnTrickshotHit() {
		_mapAchievementTypeToCurrentStepCount[Achievment.AchievementType.TrickshotsHit] += 1;
		CheckAchievements();
	}

	private void OnAsteroidShot() {
		_mapAchievementTypeToCurrentStepCount[Achievment.AchievementType.AsteroidsShot] += 1;
		CheckAchievements();
	}

	private void OnScoreUpdated(int newScore) {
		_mapAchievementTypeToCurrentStepCount[Achievment.AchievementType.ScoreReached] = newScore;
		CheckAchievements();
	}

	private void OnLevelChange(int level) {
		_mapAchievementTypeToCurrentStepCount[Achievment.AchievementType.LevelReached] = level + 1;
		CheckAchievements();
	}

	private void CheckAchievements() {
		foreach(Achievment a in _achievementConfig.achievments) {
			if(!_mapAchievementToCompletion[a.title]) {
				if(_mapAchievementTypeToCurrentStepCount[a.type]  >= a.valueToReach) {
					CompleteAchievement(a);
				}
			}
		}

		SaveGameManager.SetAchievementSteps(_mapAchievementTypeToCurrentStepCount);
		SaveGameManager.SetAchievementCompletion(_mapAchievementToCompletion);
	}

	private void CompleteAchievement(Achievment achievement) {
		_mapAchievementToCompletion[achievement.title] = true;
		ToastDisplay.Instance.Toast(achievement.title, achievement.description);
		AnalyticsManager.SendAchievmentAcquired(achievement.title);
	}

	public void InitAchievementData() {
		// Initialize all achievements to incomplete
		_mapAchievementToCompletion = new Dictionary<string, bool>();
		foreach(Achievment a in _achievementConfig.achievments) {
			_mapAchievementToCompletion.Add(a.title, false);
		}

		// Initialize all achievement steps to 0
		_mapAchievementTypeToCurrentStepCount = new Dictionary<Achievment.AchievementType, int>();
		foreach(Achievment.AchievementType at in Enum.GetValues(typeof(Achievment.AchievementType))) {
			_mapAchievementTypeToCurrentStepCount.Add(at, 0);
		}
	}

	public bool IsAchievementComplete(string title) {
		return _mapAchievementToCompletion.ContainsKey(title) && _mapAchievementToCompletion[title];
	}

	private void LoadFromSaveData() {
		if(SaveGameManager.SaveData.AchievementCompletion != null) {
			// Load completion data from save file if it exists
			foreach(SaveGameManager.AchievementCompletion ac in SaveGameManager.SaveData.AchievementCompletion) {
				_mapAchievementToCompletion[ac.Title] = ac.Complete;
			}
		}

		if(SaveGameManager.SaveData.AchievementSteps != null) {
			// Load step data from save file if it exists
			foreach(SaveGameManager.AchievementStep aStep in SaveGameManager.SaveData.AchievementSteps) {
				_mapAchievementTypeToCurrentStepCount[aStep.Type] = aStep.Steps;
			}
		}
	}
}
