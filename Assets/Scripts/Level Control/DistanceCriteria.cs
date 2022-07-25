using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceCriteria 
{
    public MapSpace space;
    public float distance;
    public Comparison comparison;
    public CheckType checkType;

    private List<MapSpace> avoidSpaces = new List<MapSpace>();

    public DistanceCriteria(MapSpace space, float distance, Comparison comparison, CheckType checkType = CheckType.MapSpace) {
        this.space = space;
        this.distance = distance;
        this.comparison = comparison;
        this.checkType = checkType;
	}

    public DistanceCriteria(float distance, CheckType checkType) {
        this.distance = distance;
        this.checkType = checkType;
    }

    public DistanceCriteria(List<MapSpace> avoidSpaces) {
        this.avoidSpaces = avoidSpaces;
        checkType = CheckType.IgnoreSpaces;
	}

    public enum CheckType {
        MapSpace,
        MapEdge,
        Structure,
        IgnoreSpaces,
	}

    public enum Comparison {
        LessThan,
        LessThanOrEqual,
        GreaterThan,
        GreaterThanOrEqual,
        Equals
	}

    public bool CheckStraightLine(MapSpace otherSpace) {
        switch (checkType) {
            case CheckType.MapSpace:
                return CheckStraightLineToSpace(otherSpace);
            case CheckType.MapEdge:
                return CheckStraightLineFromEdge(otherSpace);
            case CheckType.Structure:
                return CheckStraightLineFromStructure(otherSpace);
            case CheckType.IgnoreSpaces:
                return CheckIgnoreSpaces(otherSpace);
            default:
                return true;
        }
    }

    public bool CheckIgnoreSpaces(MapSpace otherSpace) {
        return !avoidSpaces.Contains(otherSpace);
	}

    public bool CheckStraightLineFromEdge(MapSpace otherSpace) {
        var map = otherSpace.map;
        for (var i = 0; i <= distance; i++) {
            for (var j = 0; j <= distance; j++) {
                var space = map.GetSpaceFromCoordinates(otherSpace.row + i, otherSpace.column + i);
                if (space == null) {
                    return false;
                }

            }
        }
        return true;
    }

    public bool CheckStraightLineFromStructure(MapSpace otherSpace) {
        var map = otherSpace.map;
        for(var i=0; i<=distance; i++) {
            for(var j=0; j<=distance; j++) {
                var space = map.GetSpaceFromCoordinates(otherSpace.row + i, otherSpace.column + i);
                if(space != null && space.HasStructure) {
                    return false;
				}
                
			}
		}
        return true;
	}

    public bool CheckStraightLineToSpace(MapSpace otherSpace) {
        var rowDelta = space.row - otherSpace.row;
        var columnDelta = space.column - otherSpace.column;
        float delta = Mathf.Sqrt(rowDelta * rowDelta + columnDelta * columnDelta);
        switch (comparison) {
            case Comparison.LessThan:
                if (delta < distance) {
                    return true;
                }
                break;
            case Comparison.LessThanOrEqual:
                if (delta <= distance) {
                    return true;
                }
                break;
            case Comparison.GreaterThan:
                if (delta > distance) {
                    return true;
                }
                break;
            case Comparison.GreaterThanOrEqual:
                if (delta >= distance) {
                    return true;
                }
                break;
            case Comparison.Equals:
                if (delta == distance) {
                    return true;
                }
                break;
        }
        return false;
    }

    public bool CheckSteps(MapSpace otherSpace) {
        var rowDelta = space.row - otherSpace.row;
        var columnDelta = space.column - otherSpace.column;
        float delta = Math.Abs(rowDelta) + Math.Abs(columnDelta);
        switch (comparison) {
            case Comparison.LessThan:
                if (delta < distance) {
                    return true;
                }
                break;
            case Comparison.LessThanOrEqual:
                if (delta <= distance) {
                    return true;
                }
                break;
            case Comparison.GreaterThan:
                if (delta > distance) {
                    return true;
                }
                break;
            case Comparison.GreaterThanOrEqual:
                if (delta >= distance) {
                    return true;
                }
                break;
            case Comparison.Equals:
                if (delta == distance) {
                    return true;
                }
                break;
        }
        return false;
    }

    public static explicit operator List<object>(DistanceCriteria v) {
		throw new NotImplementedException();
	}
}
