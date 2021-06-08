using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof (CanvasGroup))]
public class GameOverScreen : MonoBehaviour {
	[SerializeField] float _fadeInDuration = 1.0f;
	[SerializeField] Text _gameOverText;
	[SerializeField] Text _scoreDetailsText;
	
	private void OnEnable() {
		UpdateText();
		StartCoroutine("FadeInPanel");
	}

	private IEnumerator FadeInPanel() {
		this.GetComponent<CanvasGroup>().alpha = 0;
		while(this.GetComponent<CanvasGroup>().alpha < 1) {
			yield return null;
			this.GetComponent<CanvasGroup>().alpha += Time.deltaTime / _fadeInDuration;
		}
	}

	private void UpdateText() {
		if(AsteraX.Instance != null) {
			_scoreDetailsText.text = "Final Level: " + (AsteraX.Instance.CurrentLevel+1) + "\nFinal Score: " + string.Format("{0, 6:N0}", ScoreManager.Instance.CurrentScore);
		}
		if(ScoreManager.Instance != null && ScoreManager.Instance.HighScoreReached) {
			_gameOverText.text = "High Score!";
		}
	}
}
