using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveData
{
    public SerializableLevel level;
    public Party party;

    public SaveData(SerializableLevel level, Party party) {
        this.level = level;
        this.party = party;
	}
}
