using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Structures;
using System;

[System.Serializable]
public class SerializableLevel 
{
	[SerializeField]
	private LevelConfiguration configuration;
	[SerializeField]
	private LevelMap map;
	[SerializeField]
	private List<EnemyCharacter> enemies = new List<EnemyCharacter>();
	[SerializeField]
	private List<LevelStructure> structures = new List<LevelStructure>();
	[SerializeField]
	private List<LevelInteractableContainer> containers = new List<LevelInteractableContainer>();
	[SerializeField]
	private List<MapSpace> startingSpaces = new List<MapSpace>();


	[SerializeField] private int turnNumber;

	[SerializeField] public int rewindCount;

	public void SetConfiguration(LevelConfiguration configuration) => this.configuration = configuration; 
	

	public void SetTurnNumber(int turnNumber) => this.turnNumber = turnNumber;
	

	public int GetTurnNumber() =>  turnNumber;
	

	public LevelConfiguration GetConfiguration() => configuration;
	

	public void AddEnemy(EnemyCharacter enemy) => enemies.Add(enemy);

	public void AddStructure(LevelStructure structure)  => structures.Add(structure);

	public void AddContainer(LevelInteractableContainer container) => containers.Add(container);
	

	public void SetMap(LevelMap map) => this.map = map;
	

	public LevelMap GetMap() => map;

	public void SetStartingSpaces(List<MapSpace> spaces) {
		startingSpaces.Clear();
		foreach (var space in spaces) {
			startingSpaces.Add(space);
		}
	}

	public IEnumerable<MapSpace> GetStartingSpaces() {
		foreach (var space in startingSpaces) {
			yield return space;
		}
	}

	public IEnumerable<EnemyCharacter> GetEnemies() => enemies;
	public void ClearEnemies() => enemies.Clear();
	

	public IEnumerable<LevelStructure> GetStructures() => structures;
	public IEnumerable<LevelInteractableContainer> GetContainers() => containers;


	public void ClearContainers() => containers.Clear();
}
