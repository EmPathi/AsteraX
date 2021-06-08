using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour {
	public static ScoreManager Instance { get; private set; }

	public int CurrentScore { get; private set; }
	public bool HighScoreReached { get; private set; }

	private void Awake() {
		if(ScoreManager.Instance == null) {
			ScoreManager.Instance = this;
			GameEvents.OnAddScore += OnAddScore;
			HighScoreReached = false;
		} else {
			Destroy(this.gameObject);
		}
	}

	private void OnDestroy() {
		GameEvents.OnAddScore -= OnAddScore;
	}

	public void OnAddScore(int scoreDelta) {
		this.CurrentScore += scoreDelta;
		GameEvents.ScoreUpdated(this.CurrentScore);

		if(this.CurrentScore > SaveGameManager.SaveData.HighScore) {
			SaveGameManager.SaveData.HighScore = this.CurrentScore;

			if(!HighScoreReached) {
				HighScoreReached = true;

				ToastDisplay.Instance.Toast("High Score!", "You've achieved a new high score.");
			}
		}
	}

	public void ResetHighScore() {
		HighScoreReached = false;
		SaveGameManager.SaveData.HighScore = 5000;
	}
}
