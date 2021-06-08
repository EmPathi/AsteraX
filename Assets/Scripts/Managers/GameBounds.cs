using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBounds: MonoBehaviour {
	public static GameBounds Instance {get; private set;}

	private BoxCollider _screenBounds;
	
	void Awake () {
		if(Instance == null) {
			Instance = this;
			_screenBounds = this.GetComponent<BoxCollider>();
		} else {
			Destroy(this.gameObject);
		}
	}
	
	public Vector3 GetRandomLocationWithinBounds() {
		float x = _screenBounds.bounds.min.x + (_screenBounds.bounds.max.x-_screenBounds.bounds.min.x)*Random.value;
		float y = _screenBounds.bounds.min.y + (_screenBounds.bounds.max.y-_screenBounds.bounds.min.y)*Random.value;
		return new Vector3(x, y, 0);
	}
}
