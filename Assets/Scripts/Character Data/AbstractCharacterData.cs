using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;


public class AbstractCharacterData : ReferenceData {
    public string characterName;
    public int actionPoints;
    public float fieldOfView;
    public float viewDistance;
    public AssetReferenceGameObject prefab;
    public Material hoverMaterial;

    public SoundEffect step;

    public CharacterClass characterClass;
}
