using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Pathfinding 
{

    private static List<Node> nodeMap = new List<Node>();
    
    public class Node {
        public float g=0;
        public float h=0;
        public MapSpace space;
        public Node parent;
        public List<Node> adjacentNodes = new List<Node>();

        public float F() {
            return g + h;
		}
	}

    public static void GenerateNodeMap(LevelMap levelMap) {
        nodeMap.Clear();
        nodeMap = new List<Node>();
        foreach(var space in levelMap.spaces) {
			if (space.HasObstacle) {
                continue;
			}
            Node newNode = new Node {
                space = space
            };
            nodeMap.Add(newNode);
		}
        foreach(var node in nodeMap) {
            List<MapSpace> adjacentSpaces = levelMap.GetAdjacentSpaces(node.space);
            foreach(var aSpace in adjacentSpaces) {
                Node aNode = GetNodeBySpace(aSpace);
                if(aNode != null) {
                    node.adjacentNodes.Add(aNode);
				}
			}
		}

	}

    public static List<MapSpace> GetPath(MapSpace start, MapSpace goal, bool avoidVision = false, bool allowImpassable = false) {
        if (start == null || goal == null) {
            throw new System.Exception("Cannot pathfind between null spaces");
        }
        List<MapSpace> path = new List<MapSpace>();
        if(start.HasObstacle || goal.HasObstacle) {
            return path;
		}
        if (nodeMap == null) {
            Debug.LogError("no node map");
            return path;
		}

        Node startNode = GetNodeBySpace(start);
        Node goalNode = GetNodeBySpace(goal);
        if (goalNode == null || startNode == null) {
            throw new System.Exception("Cannot pathfind with a null node");
        }
        List<Node> openSet = new List<Node>();
        List<Node> closedSet = new List<Node>();
        GuessH(startNode, goalNode);
        openSet.Add(startNode);
        while (openSet.Count > 0){
            Node currentNode = openSet[0];
            foreach(var node in openSet) {
                if(node.F() < currentNode.F()) {
                    currentNode = node;
				}
			}
            if(currentNode == goalNode) {
                closedSet.Add(currentNode);
                break;
			}
            foreach(var node in currentNode.adjacentNodes) {
                if (node.space != null && node.space.Passable || allowImpassable) {
                    float newG = currentNode.g + 1;
					if (avoidVision) {
                        newG += node.space.CurrentHitCount/100f;
					}
                    if (closedSet.Contains(node)) {
                        if (node.g > newG) {
                            closedSet.Remove(node);
                            openSet.Add(node);
                        }
                    }

                    else if (!openSet.Contains(node)) {
                        openSet.Add(node);
                        GuessH(node, goalNode);
                        node.g = newG;
                        node.parent = currentNode;
                    }
                    if(node.g > newG) {

                        node.g = newG;
                        node.parent = currentNode;
                    }
                }
			}
            closedSet.Add(currentNode);
            openSet.Remove(currentNode);
		}

        if(!closedSet.Contains(goalNode)) {
            if (allowImpassable) {
                return path;
            }
            return GetPath(start, goal, avoidVision, true);

        }
		if (!closedSet.Contains(startNode)) {
            throw new System.Exception("How the hell is the start node not in the path?");
		}
        Node pathNode = goalNode;
        while (pathNode != startNode) {
            path.Add(pathNode.space);
            pathNode = pathNode.parent;
		}
        path.Reverse();
        return path;
	}

    private static void GuessH(Node hNode, Node goalNode) {
        if(goalNode == null || hNode == null) {
            throw new System.Exception("Cannot pathfind with a null node");
		}
        int deltaR = goalNode.space.Row - hNode.space.Row;
        int deltaC = goalNode.space.Column - hNode.space.Column;
        float h = Mathf.Sqrt((Mathf.Pow((float)deltaR, 2f) + Mathf.Pow((float)deltaC, 2f)));
        hNode.h = h;
	}

    private static Node GetNodeBySpace(MapSpace space) {
        foreach(Node node in nodeMap) {
            if(node.space == space) {
                return node;
			}
		}
        return null;
	}
    
}
