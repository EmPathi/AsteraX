using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleToFitScreen: MonoBehaviour {
	private Vector2 startingScale;

	void Start() {
		startingScale = new Vector2(this.transform.lossyScale.x, this.transform.lossyScale.y);
		updateRatio();
	}

	void updateRatio() {
		float ratio = Screen.width/(float)Screen.height;
		float ratioCurrent = startingScale.x/startingScale.y;

		this.transform.localScale = new Vector3(startingScale.x * ratio/ratioCurrent, startingScale.y, 1);
	}
}
