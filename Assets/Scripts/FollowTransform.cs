using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTransform : MonoBehaviour {
	[SerializeField] private Transform transformToFollow;
	[SerializeField] private bool lookAtTransform = false;

	private Vector3 startingPositionOffset;

	private void Start() {
		startingPositionOffset = this.transform.position;
	}

	void Update () {
		if(transformToFollow != null) {
			this.transform.position = startingPositionOffset + transformToFollow.position;

			if(lookAtTransform) {
				this.transform.LookAt(transformToFollow, -Vector3.forward);
			}
		}
	}
}
