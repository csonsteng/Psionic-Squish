using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Structures;
using UnityEngine.AddressableAssets;

[System.Serializable]
public partial class MapSpace : ITargetable
{
    public readonly int row;
    public readonly int column;

    public string tileObjectGUID;

    public bool BlockedOnLeft => HasStructure && Structure.blockedOnLeft;
    public bool BlockedOnRight => HasStructure && Structure.blockedOnRight;
    public bool BlockedOnTop => HasStructure && Structure.blockedOnTop;
    public bool BlockedOnBottom => HasStructure && Structure.blockedOnBottom;
    public bool HasStructure => Structure != null;

    public bool HasObstacle => obstacle != null;
    public bool hasContainer = false;
    public bool possibleEnemyLocation = false;
    public bool possibleObjectiveLocation = false;

    [SerializeField] public Obstacle obstacle = null;

    public void AddObstacle(AssetReferenceGameObject gameObject, float rotation)
    {
        if (gameObject == null)
            return;
        obstacle = new Obstacle(gameObject, rotation);

    }


    public bool Occupied {
        get {
            return occupied;
		}
		set {
            occupied = value;
            map.ReportOccupancyChange(this);
		}
	}

    public bool Passable {
        get {
            return passable;
		}
		set {
            passable = value;
			if (!passable) {
                Occupied = true;
			}
		}
	}

    [SerializeField] private bool occupied = false;
    [SerializeField] private bool passable = true;
    public bool visible = true;
    [field: NonSerialized] public GameObject TileObject { get; private set; }
    public UnityEvent tileHovered;
    public Action tileClicked;

    public StructureSpace Structure
    {
        get
        {
            return _structure;
        }
        private set
        {
            _structure = value;
        }
    }
    [SerializeField] private StructureSpace _structure;
    [NonSerialized]private StructureSceneObject structureObject;
    private LevelInteractableContainer container;
    public bool InteriorSpace => HasStructure && Structure.isInterior;

    public Material hoverMaterial;
    private Material originalMaterial;
    [SerializeReference] public readonly LevelMap map;
    private readonly Dictionary<GameObject, int> occupantsAndLayers = new Dictionary<GameObject, int>();
    [SerializeField] private bool spotted = false;
    [field: SerializeField] public Threshold Threshold { get; private set; }

    public TileIndicator Indicator => TileObject.GetComponentInChildren<TileIndicator>();

    public MapSpace(int row, int column, LevelMap map) {
        this.row = row;
        this.column = column;
        this.map = map;
        map.ReportOccupancyChange(this);
    }
    /*
    public MapSpace(SerializableMapSpace serializedSpace, LevelMap map) {
        row = serializedSpace.row;
        column = serializedSpace.column;
        hasObstacle = serializedSpace.hasObstacle;
        possibleEnemyLocation = serializedSpace.possibleEnemyLocation;
        possibleObjectiveLocation = serializedSpace.possibleObjectiveLocation;
        occupied = serializedSpace.occupied;
        hasContainer = serializedSpace.hasContainer;
        visible = serializedSpace.visible;

        tileObjectGUID = serializedSpace.tileObjectGUID;
        spotted = serializedSpace.spotted;
        threshold = serializedSpace.threshold;
        this.map = map;

        
    }

    public SerializableMapSpace Serialize() {
        var serialized = new SerializableMapSpace() {
            row = row,
            column = column,
            hasObstacle = hasObstacle,
            possibleEnemyLocation = possibleEnemyLocation,
            possibleObjectiveLocation = possibleObjectiveLocation,
            occupied = occupied,
            hasContainer = hasContainer,
            visible = visible,
            tileObjectGUID = tileObjectGUID,
            threshold = threshold,
            spotted = spotted
        };
        return serialized;
	}*/
    /*
    public SerializeMapReference Reference() {
        var serialized = new SerializeMapReference() {
            row = row,
            column = column,
        };
        return serialized;
	}
    */
    public int CurrentHitCount => Indicator.GetCurrentHitCount();


    public Vector2 SubtractFrom(MapSpace other) {
        int dRow = row - other.row;
        int dCol = column - other.column ;
        return new Vector2(dRow, dCol);
	}

    public void PlaceStructure(StructureSpace space, StructureSceneObject structureObject) {
        Structure = space;
        this.structureObject = structureObject;
        possibleEnemyLocation = space.enemyLocation;
        possibleObjectiveLocation = space.objectiveLocation;
	}

    public void PlaceContainer(LevelInteractableContainer container) {
        hasContainer = true;
        this.container = container;
	}


	GameObject ITargetable.ClickableObject() => TileObject;
	

	void ITargetable.HandleClick() => tileClicked?.Invoke();

	
    public void SetTileObject(GameObject gameobject) => TileObject = gameobject;
	
    public bool ClaimPositionImpassable(GameObject gameObject, int layer) {
        if(gameObject == null || !Passable) {
            return false;
		}
        occupantsAndLayers.Add(gameObject, layer); 
        if (spotted) {
            gameObject.SetLayerRecursive(layer);
        }
        Passable = false;
        return true;
    }

    public bool ClaimPositionPassable(GameObject gameObject, int layer) {
        if (gameObject == null || !Passable) {
            return false;
        }
        occupantsAndLayers.Add(gameObject, layer);
		if (spotted) {
            gameObject.SetLayerRecursive(layer);
        }
        Occupied = true;
        return true;
	}

    public void RelenquishPosition(GameObject gameObject) {
		if (occupantsAndLayers.ContainsKey(gameObject)) {
            occupantsAndLayers.Remove(gameObject);
		}
        Passable = true;
        if(occupantsAndLayers.Count == 0) {
            Occupied = false;
		}
	}

    public void ShowTileAndOccupants() {
        spotted = true;
        foreach(KeyValuePair<GameObject, int> pair in occupantsAndLayers) {
            pair.Key.SetLayerRecursive(pair.Value);
		}
        TileObject.SetLayerRecursive("Tile");
		if (InteriorSpace) {
            structureObject.ShowTransparent();
		}
	}

    public void HideTileAndOccupants() {
        if (InteriorSpace) {
            structureObject.ShowOpaque();
            
        }
        if (spotted) {
            return;
        }

        foreach (KeyValuePair<GameObject, int> pair in occupantsAndLayers) {
            string layerName = LayerMask.LayerToName(pair.Value);
            string destinationLayer;
			switch (layerName) {
                case "Tile":
                    destinationLayer = "CulledTiles";
                    break;
                case "Obstacles":
                    destinationLayer = "CulledObstacles";
                    break;
                default:
                    destinationLayer = "Culled";
                    break;
			}
            pair.Key.SetLayerRecursive(destinationLayer);
        }
        TileObject.SetLayerRecursive("CulledTiles");
    }


    private bool IsActivePlayerLocation() {
        var activePlayer = LevelController.Get().ActivePlayer;
        return activePlayer != null && activePlayer.GetPosition() == this;
	}

    public void HandleHover() {
        
        if (originalMaterial == null) {
            originalMaterial = Renderer().material;
        }
        Renderer().material = hoverMaterial;
        Renderer().receiveShadows = false;
    }

    public void HandleUnHover() {
        if (originalMaterial != null && !IsActivePlayerLocation()) {
            Renderer().material = originalMaterial;
            Renderer().receiveShadows = true;
        }
        Indicator.ClearStepCount();
    }

    public string GetName() {
        return "(" + row.ToString() + ", " + column.ToString() + ")";
    }

    public void SetStepIndicator(Direction facing) {
        Indicator.ShowStep(facing);
	}

    public void ClearStepIndicator() {
        Indicator.ClearStep();
	}

    public void SetThresholdIndicator(Threshold threshold, int hits) {
        this.Threshold = threshold;
		switch (threshold) {
            case global::Threshold.Complete:
				Indicator.SetCompleteIndicator(hits);
                break;
            case global::Threshold.Partial:
				Indicator.SetPartialIndicator(hits);
                break;
        }
	}

    public void ClearVisionIndicator() {
        Indicator.ClearIndicators();
        Threshold = Threshold.Hidden;
    }

    private MeshRenderer Renderer() {
        return TileObject.GetComponentInChildren<MeshRenderer>();
    }

	public MapSpace GetPosition() {
        return this;
	}

    public List<MapSpace> GetAdjacents() {
       return  map.GetAdjacentSpaces(this);
	}

    public bool IsAdjacent(MapSpace space) {
        return GetAdjacents().Contains(space) ;
	}
}
