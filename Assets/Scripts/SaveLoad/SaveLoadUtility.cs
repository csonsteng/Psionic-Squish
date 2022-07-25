using System.Collections;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class SaveLoadUtility
{
	private static int activeSlot=1;
	private static int maxRewind = 5;
	private static BinaryFormatter Formatter => new BinaryFormatter();
	public static void Save(SaveData saveData, string fileName) {
		string path = Application.persistentDataPath + "/" + fileName + ".save";


		var stream = new FileStream(path, FileMode.Create);

		var json = JsonUtility.ToJson(saveData);
		Formatter.Serialize(stream, json);
		stream.Close();
	}

	public static SaveData GetSaveData(string fileName) {
		string path = Application.persistentDataPath + "/" + fileName + ".save";
		if (!File.Exists(path)) {
			Debug.LogError("No save data found");
			return null;
		}

		FileStream stream = new FileStream(path, FileMode.Open);
		var data = Formatter.Deserialize(stream) as string;
		stream.Close();
		return JsonUtility.FromJson<SaveData>(data);
	}

	public static void Delete(string fileName) {
		string path = Application.persistentDataPath + "/" + fileName + ".save";
		if (!File.Exists(path)) {
			return;
		}
		File.Delete(path);
	}

	public static SaveData LoadFromActiveSlot() {
		return GetSaveData("saveSlot" + activeSlot.ToString());
	}

	public static SaveData LoadRewind(int turnNumber) {
		return GetSaveData("rewind" + turnNumber.ToString());
	}

	public static void SaveInActiveSlot(SerializableLevel level, Party party) {
		SaveData saveData = new SaveData(level, party);
		Save(saveData, "saveSlot" + activeSlot.ToString());
	}

	public static void SaveForRewind(SerializableLevel level, Party party) {
		SaveData saveData = new SaveData(level, party);
		Save(saveData, "rewind" + level.GetTurnNumber().ToString());
		Save(saveData, "saveSlot" + activeSlot.ToString());


		var rewindToDelete = level.GetTurnNumber() - maxRewind;
		if(rewindToDelete <= 0) {
			return;
		}
		Delete("rewind" + rewindToDelete.ToString());
	}

	public static void SetSaveSlot(int slotNumber) {
		activeSlot = slotNumber;
	}

}
