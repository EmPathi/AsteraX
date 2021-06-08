using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class ScoreText : MonoBehaviour {

	private void Awake() {
		GameEvents.OnScoreUpdated += OnScoreUpdated;
	}

	private void OnDestroy() {
		GameEvents.OnScoreUpdated -= OnScoreUpdated;
	}

	public void OnScoreUpdated(int newScore) {
		gameObject.GetComponent<Text>().text = string.Format("{0, 6:N0}", newScore);
	}
}
