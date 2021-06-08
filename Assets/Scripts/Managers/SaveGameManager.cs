using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public static class SaveGameManager {
	[Serializable]
	public struct SaveDataStruct {
		public List<AchievementStep> AchievementSteps;
		public List<AchievementCompletion> AchievementCompletion;
		public List<CustomizationEquipped> CustomizationsEquipped;
		public int HighScore;
	}

	[Serializable]
	public struct AchievementStep {
		public Achievment.AchievementType Type;
		public int Steps;
	}

	[Serializable]
	public struct AchievementCompletion {
		public string Title;
		public bool Complete;
	}

	[Serializable]
	public struct CustomizationEquipped {
		public ShipPartType ShipPartType;
		public int OptionEquipped;
	}

	public static SaveDataStruct SaveData;
	private static string filePath;

	static SaveGameManager() {
		filePath = Application.persistentDataPath + "/AsteraX.save";
		SaveData = new SaveDataStruct {
			HighScore = 5000
		};
		Load();
	}

	public static void Save() {
		string strJSONSaveData = JsonUtility.ToJson(SaveData, true);
		File.WriteAllText(filePath, strJSONSaveData);
	}

	public static void Load() {
		if(File.Exists(filePath)) {
			string strJSONSaveData = File.ReadAllText(filePath);
			try {
				SaveData = JsonUtility.FromJson<SaveDataStruct>(strJSONSaveData);
			} catch {
				Debug.LogWarning("SaveGameManager.Load: Exception in JsonUtility.FromJson");
			}
		}
	}

	public static void DeleteSaveData() {
		if(File.Exists(filePath)) {
			File.Delete(filePath);
		}
	}

	public static void SetAchievementSteps(Dictionary<Achievment.AchievementType, int> mapAchievementTypeToCurrentStepCount) {
		SaveData.AchievementSteps = new List<AchievementStep>();

		foreach(Achievment.AchievementType at in mapAchievementTypeToCurrentStepCount.Keys) {
			SaveData.AchievementSteps.Add(new AchievementStep { Type = at, Steps = mapAchievementTypeToCurrentStepCount[at] });
		}
	}

	public static void SetAchievementCompletion(Dictionary<string, bool> mapAchievementToCompletion) {
		SaveData.AchievementCompletion = new List<AchievementCompletion>();

		foreach(string title in mapAchievementToCompletion.Keys) {
			SaveData.AchievementCompletion.Add(new AchievementCompletion { Title = title, Complete = mapAchievementToCompletion[title] });
		}
	}

	public static void SetCustomizationsEquipped(Dictionary<ShipPartType, int> dictShipPartOptionEquipped) {
		SaveData.CustomizationsEquipped = new List<CustomizationEquipped>();

		foreach(ShipPartType spt in dictShipPartOptionEquipped.Keys) {
			SaveData.CustomizationsEquipped.Add(new CustomizationEquipped { ShipPartType = spt, OptionEquipped = dictShipPartOptionEquipped[spt] });
		}
	}
}
