using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SerializableConfiguration 
{
    public int difficulty;
    public float defendLevel;

    public string typeReference;
    public string environmentReference;

    public SerializableConfiguration(LevelConfiguration configuration) {
        difficulty = configuration.difficulty;
        defendLevel = configuration.defendLevel;
        typeReference = configuration.type.uniqueID;
        environmentReference = configuration.environment.uniqueID;

    }
}
