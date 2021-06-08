using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Scriptable Object", menuName = "ScriptableObjects/LevelSO", order = 2)]
public class LevelSO: ScriptableObject {
	[Serializable]
	public struct LevelConfig {
		public int numberOfAsteroids;
		public int numberOfChildrenPerAsteroid;
		public int numberOfLayers;

		public LevelConfig(int numberOfAsteroids, int numberOfChildrenPerAsteroid, int numberOfLayers) {
			this.numberOfAsteroids = numberOfAsteroids;
			this.numberOfChildrenPerAsteroid = numberOfChildrenPerAsteroid;
			this.numberOfLayers = numberOfLayers;
		}
	}

	public List<LevelConfig> levels;


	// example string "2/2/3;2/3/3;3/2/3;3/3/3;4/2/3;4/3/3;5/2/3;5/3/3;6/2/3;6/3/3"
	public static LevelSO CreateFromLevelProgressionString(string levelProgression) {
		LevelSO levelSO = (LevelSO)CreateInstance("LevelSO");
		levelSO.levels = new List<LevelConfig>();
		int numAsteroids;
		int numChildren;
		int numLayers;

		foreach(string strLevel in levelProgression.Split(';')) {
			if(strLevel.Length > 0) {
				string[] strDetails = strLevel.Split('/');
				if(strDetails.Length == 3) {
					numAsteroids = int.Parse(strDetails[0]);
					numChildren = int.Parse(strDetails[1]);
					numLayers = int.Parse(strDetails[2]);
					levelSO.levels.Add(new LevelConfig(numAsteroids, numChildren, numLayers));
				}
			}
		}

		return levelSO;
	}
}
