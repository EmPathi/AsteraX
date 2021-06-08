using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Scriptable Object", menuName = "ScriptableObjects/AchievementSO", order = 6)]
public class AchievementSO: ScriptableObject {
	public List<Achievment> achievments;
}

[Serializable]
public struct Achievment {
	public enum AchievementType {
		ShotsFired,
		TrickshotsHit,
		AsteroidsShot,
		ScoreReached,
		LevelReached
	}

	public string title;
	public string description;
	public AchievementType type;
	public int valueToReach;
}
