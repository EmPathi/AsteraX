using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public class AnalyticsManager : MonoBehaviour {
	public static AnalyticsManager Instance { get; private set; }
	
	void Awake () {
		if(Instance == null) {
			Instance = this;
		} else {
			Destroy(this.gameObject);
		}
	}

	public static void SendAchievmentAcquired(string title) {
		AnalyticsEvent.AchievementUnlocked(title, new Dictionary<string, object> {
			{ "time", System.DateTime.Now }
		});
	}

	public static void SendGameOver(int finalScore, int finalLevelIndex, bool gotHighScore) {
		AnalyticsEvent.GameOver(null, new Dictionary<string, object> {
			{ "time", System.DateTime.Now },
			{ "finalScore", finalScore },
			{ "finalLevelIndex", finalLevelIndex },
			{ "gotHighScore", gotHighScore }
		});
	}

	public static void SendLevelStart(int levelIndex) {
		AnalyticsEvent.LevelStart(levelIndex, new Dictionary<string, object> {
			{ "time", System.DateTime.Now }
		});
	}

	public static void SendShipPartChoice() {
		Dictionary<string, object> eventData = new Dictionary<string, object>();
		eventData.Add("time", System.DateTime.Now);

		foreach(ShipPartType spt in System.Enum.GetValues(typeof(ShipPartType))) {
			if(eventData.Count == 10) {
				Debug.LogWarning("AnalyticsManager.ShipPartChoice() - Analytics Event has more than 10 values.");
				break; // analytics events are limited to 10 values, stop adding eventData elements if we scale past 10 ship part types
			}
			eventData.Add(spt.ToString(), CustomizationManager.Instance.GetCurrentlyEquipedCustomizationOption(spt).name);
		}

		AnalyticsEvent.Custom("ShipPartChoice", eventData);
	}
}
