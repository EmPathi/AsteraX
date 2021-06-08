using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class LevelAdvancePanel : MonoBehaviour {
	[SerializeField] float _fadeDuration = 0.5f;
	[SerializeField] Text _levelText;
	[SerializeField] Text _detailText;

	private void OnEnable() {
		UpdateText();
		StartCoroutine("FadeIn");
	}

	private void UpdateText() {
		if(AsteraX.Instance != null) {
			LevelSO.LevelConfig currentLevel = AsteraX.Instance.LevelConfig.levels[AsteraX.Instance.CurrentLevel];
			_levelText.text = "Level " + (AsteraX.Instance.CurrentLevel + 1);
			_detailText.text = "Asteroids: " + currentLevel.numberOfAsteroids + "   Children: " + currentLevel.numberOfChildrenPerAsteroid;
		}
	}

	private IEnumerator FadeIn() {
		this.GetComponent<CanvasGroup>().alpha = 0;

		while(this.GetComponent<CanvasGroup>().alpha < 1) {
			this.GetComponent<CanvasGroup>().alpha += Time.unscaledDeltaTime/_fadeDuration;
			yield return null;
		}
		yield return new WaitForSecondsRealtime(AsteraX.Instance.GameConfig.levelAdvancePanelDuration - _fadeDuration*2);
		while(this.GetComponent<CanvasGroup>().alpha > 0) {
			this.GetComponent<CanvasGroup>().alpha -= Time.unscaledDeltaTime/_fadeDuration;
			yield return null;
		}
	}
}
