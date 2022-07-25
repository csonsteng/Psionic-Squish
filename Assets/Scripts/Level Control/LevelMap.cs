using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Structures;

/// <summary>
/// Holds references to all mapspaces.
/// </summary>
[System.Serializable]
public class LevelMap
{

    [SerializeReference] public List<MapSpace> spaces = new List<MapSpace>();
    [SerializeField] private readonly int rowCount;
    [SerializeField] private readonly int columnCount;
    
    [SerializeReference] private List<MapSpace> openSpaces = new List<MapSpace>();

    public LevelMap(int rowCount, int columnCount) {
        spaces.Clear();
        openSpaces.Clear();
        for(var r=0; r<rowCount; r++) {
            for(var c=0; c<columnCount; c++) {
                spaces.Add(new MapSpace(r, c, this));
			}
		}
        this.rowCount = rowCount;
        this.columnCount = columnCount;
	}
    /*
    public LevelMap(SerializableMap serializedMap) {
        foreach(var serializedSpace in serializedMap.spaces) {
            var space = new MapSpace(serializedSpace, this);
            spaces.Add(space);
			if (serializedSpace.inOpenSpaces) {
                openSpaces.Add(space);
			}
		}
        rowCount = serializedMap.rowCount;
        columnCount = serializedMap.columnCount;
	}

    public SerializableMap Serialize() {
        var serialized = new SerializableMap();
        foreach(var space in spaces) {
            var serializedSpace = space.Serialize();
            if (openSpaces.Contains(space)) {
                serializedSpace.inOpenSpaces = true;
            }
            serialized.spaces.Add(serializedSpace);
		}
        serialized.rowCount = rowCount;
        serialized.columnCount = columnCount;
        return serialized;
	}*/

    public void Clear() {
        foreach (var space in spaces) {
            if (space.TileObject != null) {
                GameObject.Destroy(space.TileObject);
            }
        }
        spaces.Clear();
    }

    public void ReportOccupancyChange(MapSpace space) {
        if(space == null) {
            return;
		}
		if (openSpaces.Contains(space)) {
            openSpaces.Remove(space);
		}
        if (!space.Occupied) {
            openSpaces.Add(space);
        }
	}

    public List<MapSpace> OpenSpaces() {
        return openSpaces;
	}

    public MapSpace RandomOpenSpace() {
        int index = Random.Range(0, spaces.Count);
        return spaces[index];
	}

    public MapSpace GetRandomOpenSpaceWithDistanceCriteria(DistanceCriteria[] criteria) {
        List<MapSpace> validResults = new List<MapSpace>();
        foreach(var space in OpenSpaces()) {
            bool meetsCriteria = true;
            foreach(var criterion in criteria) {
				if (!criterion.CheckStraightLine(space)) {
                    meetsCriteria = false;
                    break;
				}
            }
            if (meetsCriteria) {
                validResults.Add(space);
            }
        }
        if(validResults.Count == 0) {
            Debug.Log("returning null");
            return null;
		}
        int randomIndex = Random.Range(0, validResults.Count);
        return validResults[randomIndex];
	}

    public bool SpaceOnEdge(MapSpace space) {
		if (!spaces.Contains(space)) {
            return false;
		}

        if(space.row == 0 || space.row == rowCount - 1) {
            return true;
		}

        if(space.column == 0 || space.column == columnCount - 1) {
            return true;
		}

        return false;
	}

    public MapSpace GetSpaceFromCoordinates(int row, int column) {
        foreach(var space in spaces) {
            if(space.row == row && space.column == column) {
                return space;
			}
		}
        return null;
	}

    public MapSpace GetSpaceFromObject(GameObject gameObject) {
        if (gameObject == null) {
            //throw new System.Exception("GameObject cannot be null");
            Debug.Log("GameObject is null");
            return null;
        }
        foreach (var space in spaces) {
            if (space.TileObject == gameObject) {
                return space;
			}
		}
        Debug.Log(gameObject.name + " is not a space");
        return null;
	}

    public MapSpace GetSpaceFromVector2Int(Vector2Int vector) {
        return GetSpaceFromCoordinates(vector.x, vector.y);
	}

    public List<MapSpace> GetAdjacentSpaces(MapSpace space) {

        List<MapSpace> adjacentSpaces = new List<MapSpace>();
        if (space.HasObstacle) {
            return adjacentSpaces;
        }

        if (!space.BlockedOnBottom) {
            var bottomSpace = GetSpaceFromCoordinates(space.row + 1, space.column);
            if (bottomSpace != null && !bottomSpace.BlockedOnTop) {
                TryAddAdjacentSpace(adjacentSpaces,bottomSpace);
            }
        }
        if (!space.BlockedOnTop) {
            var topSpace = GetSpaceFromCoordinates(space.row - 1, space.column);
            if(topSpace != null && !topSpace.BlockedOnBottom) {
                TryAddAdjacentSpace(adjacentSpaces,topSpace);
			}
        }
        if (!space.BlockedOnRight) {
            var rightSpace = GetSpaceFromCoordinates(space.row, space.column + 1);
            if(rightSpace != null && !rightSpace.BlockedOnLeft) {
                TryAddAdjacentSpace(adjacentSpaces,rightSpace);
			}
        }
        if (!space.BlockedOnLeft) {
            var leftSpace = GetSpaceFromCoordinates(space.row, space.column - 1);
            if(leftSpace != null && !leftSpace.BlockedOnRight) {
                TryAddAdjacentSpace(adjacentSpaces,leftSpace);
			}
        }
        return adjacentSpaces;
	}

    private void TryAddAdjacentSpace(List<MapSpace> spaces, MapSpace space) {
        if (space.HasObstacle) {
            return;
        }
        spaces.Add(space);
    }

    public MapSpace GetSpaceWithStructure(StructureSpace structure) {
        foreach(var space in spaces) {
            if (space.HasStructure && space.Structure == structure) {
                return space;
			}
		}
        return null;
	}



}
