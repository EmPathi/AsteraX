using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveOnlyDuringSomeGameStates : MonoBehaviour {
	[SerializeField] List<GameState> _activeGameStates;
	
	private void Awake () {
		GameEvents.OnGameStateChanged += OnStateChanged;
	}

	private void OnDestroy() {
		GameEvents.OnGameStateChanged -= OnStateChanged;
	}

	private void OnStateChanged(GameState previousState, GameState newState) {
		if((_activeGameStates.Contains(newState)) && (!this.gameObject.activeInHierarchy)) {
			this.gameObject.SetActive(true);
		} else if((!_activeGameStates.Contains(newState)) && (this.gameObject.activeInHierarchy)) {
			this.gameObject.SetActive(false);
		}
	}
}
