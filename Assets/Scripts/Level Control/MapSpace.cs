using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Structures;

[System.Serializable]
public class MapSpace : ITargetable
{

    public delegate void CallbackFunction();
    public int row;
    public int column;

    public string tileObjectGUID;

    public bool BlockedOnLeft => hasStructure && structure.blockedOnLeft;
    public bool BlockedOnRight => hasStructure && structure.blockedOnRight;
    public bool BlockedOnTop => hasStructure && structure.blockedOnTop;
    public bool BlockedOnBottom => hasStructure && structure.blockedOnBottom;
    public bool hasStructure = false;
    public bool hasObstacle=false;
    public bool hasContainer = false;
    public bool possibleEnemyLocation = false;
    public bool possibleObjectiveLocation = false;

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

    private bool occupied = false;
    private bool passable = true;
    public bool visible = true;
    private GameObject tileObject;
    public UnityEvent tileHovered;
    public CallbackFunction tileClicked;
    [SerializeField]
    public StructureSpace structure;
    private StructureSceneObject structureObject;
    private LevelInteractableContainer container;
    public bool InteriorSpace => hasStructure && structure.isInterior;

    public Material hoverMaterial;
    private Material originalMaterial;
    public readonly LevelMap map;
    private Dictionary<GameObject, int> occupantsAndLayers = new Dictionary<GameObject, int>();
    private bool spotted = false;
    private Threshold threshold;

    public TileIndicator Indicator => tileObject.GetComponentInChildren<TileIndicator>();

    public MapSpace(int row, int column, LevelMap map) {
        this.row = row;
        this.column = column;
        this.map = map;
        map.ReportOccupancyChange(this);
    }

    public MapSpace(SerializableMapSpace serializedSpace, LevelMap map) {
        row = serializedSpace.row;
        column = serializedSpace.column;
        hasStructure = serializedSpace.hasStructure;
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
            hasStructure = hasStructure,
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
	}

    public SerializeMapReference Reference() {
        var serialized = new SerializeMapReference() {
            row = row,
            column = column,
        };
        return serialized;
	}

    public int CurrentHitCount => Indicator.GetCurrentHitCount();

    public Threshold Threshold => threshold;

    public Vector2 SubtractFrom(MapSpace other) {
        int dRow = row - other.row;
        int dCol = column - other.column ;
        return new Vector2(dRow, dCol);
	}

    public void PlaceStructure(StructureSpace space, StructureSceneObject structureObject) {
        hasStructure = true;
        structure = space;
        this.structureObject = structureObject;
        possibleEnemyLocation = space.enemyLocation;
        possibleObjectiveLocation = space.objectiveLocation;
	}

    public void PlaceContainer(LevelInteractableContainer container) {
        hasContainer = true;
        this.container = container;
	}

	public GameObject ClickableObject() {
        return tileObject;
	}

    public StructureSpace GetStructureSpace() {
        return structure;
	}

	public void HandleClick() {
        tileClicked?.Invoke();

	}
    public void SetTileObject(GameObject gameobject) {
        tileObject = gameobject;
	}
    public GameObject GetTileObject() {
        return tileObject;
	}

    public bool ClaimPositionImpassable(GameObject gameObject, int layer) {
        if(gameObject == null) {
            return false;
		}
        if (!Passable) {
            return false;
        }
        occupantsAndLayers.Add(gameObject, layer); 
        if (spotted) {
            SetLayerRecursive(gameObject, layer);
        }
        Passable = false;
        return true;
    }

    public bool ClaimPositionPassable(GameObject gameObject, int layer) {
        if (gameObject == null) {
            return false;
        }
        if (!Passable) {
            return false;
		}
        occupantsAndLayers.Add(gameObject, layer);
		if (spotted) {
            SetLayerRecursive(gameObject, layer);
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
            SetLayerRecursive(pair.Key, pair.Value);
		}
        SetLayerRecursive(tileObject,"Tile");
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
            string destinationLayer = "";
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

            SetLayerRecursive(pair.Key,destinationLayer);
        }
        SetLayerRecursive(tileObject,"CulledTiles");
    }

    private void SetLayerRecursive(GameObject gameObject, string layer) {
        SetLayerRecursive(gameObject, LayerMask.NameToLayer(layer));
	}

    private void SetLayerRecursive(GameObject gameObject, int layer) {
        gameObject.layer = layer;
        foreach(Transform transform in gameObject.transform) {
            SetLayerRecursive(transform.gameObject, layer);
		}
	}

    private bool IsActivePlayerLocation() {
        if(LevelController.Get().ActivePlayer == null)
        {
            return false;
        }
        return LevelController.Get().ActivePlayer.GetPosition() == this;
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
        this.threshold = threshold;
		switch (threshold) {
            case Threshold.Complete:
                Indicator.SetCompleteIndicator(hits);
                break;
            case Threshold.Partial:
                Indicator.SetPartialIndicator(hits);
                break;
        }
	}

    public void ClearVisionIndicator() {
        Indicator.ClearIndicators();
        threshold = Threshold.Hidden;
    }

    private MeshRenderer Renderer() {
        return tileObject.GetComponentInChildren<MeshRenderer>();
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
