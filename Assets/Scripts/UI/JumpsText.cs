using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class JumpsText: MonoBehaviour {

	private void Awake() {
		GameEvents.OnJumpUsed += OnJumpUsed;
	}

	private void OnDestroy() {
		GameEvents.OnJumpUsed -= OnJumpUsed;
	}

	public void OnJumpUsed(int jumpsRemaining) {
		gameObject.GetComponent<Text>().text = string.Format("{0} Jumps", jumpsRemaining);
	}
}
