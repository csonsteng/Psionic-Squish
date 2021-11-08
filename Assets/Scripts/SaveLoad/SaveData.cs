using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveData 
{
    [SerializeField]
    public SerializableLevel level;
    [SerializeField]
    public SerializableParty party;

    public SaveData(SerializableLevel level, Party party) {
        this.level = level;
       this.party = new SerializableParty(party);
	}
}
