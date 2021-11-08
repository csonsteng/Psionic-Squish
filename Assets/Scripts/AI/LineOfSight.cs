using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class LineOfSight : MonoBehaviour
{
    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;
    private LayerMask tileMask;
    private LayerMask sightMask;

    public UnityEvent spottedPlayer = new UnityEvent();

    private Mesh mesh;

    private float fov = 90f;
    private int rayCount = 50;
    private Vector3 origin;

    private float drawHeight = 0.35f;

    private float viewDistance = 5f;

    

    private void OnEnable()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.enabled = false;
        meshFilter = GetComponent<MeshFilter>();
        mesh = new Mesh();
        meshFilter.mesh = mesh;
        origin = new Vector3(0f, drawHeight, 0f);
        sightMask = LayerMask.GetMask("Obstacles") | LayerMask.GetMask("CulledObstacles");
        tileMask = LayerMask.GetMask("Tile") | LayerMask.GetMask("CulledTiles");

    }

    public void InitializeFOV(float FOV, float distance) {
        fov = FOV;
        viewDistance = distance;
	}

	public VisionProfile UpdateVision(bool obscured = true)
    {
        Dictionary<GameObject, int> tileHits = new Dictionary<GameObject, int>();
        float angle = -45;
        float deltaAngle = fov / rayCount;
        origin = new Vector3(0f, drawHeight, 0f);
        Vector3[] vertices = new Vector3[rayCount + 2];
        Vector2[] uv = new Vector2[vertices.Length];
        int[] tris = new int[rayCount * 3];

        vertices[0] = origin;

        int triangleIndex = 0;
        int vertexIndex = 1;
        for (int i = 0; i <= rayCount; i++) {
            Vector3 vertex;
            float angleRadians = angle * Mathf.PI / 180f;
            Vector3 direction = new Vector3(Mathf.Cos(angleRadians), 0f, Mathf.Sin(angleRadians));
            float rayAngleRadians = (angle - transform.rotation.eulerAngles.y) * Mathf.PI / 180f;
            float rotation = transform.rotation.eulerAngles.y;
            float xSign = 1;
            float zSign = 1;
            while (rotation < 0) {
                rotation += 360;
            }
            while (rotation > 360) {
                rotation -= 360;
            }
            Vector3 rayDirection = new Vector3(xSign * Mathf.Cos(rayAngleRadians), 0f, zSign * Mathf.Sin(rayAngleRadians));
            if (obscured) {
                Physics.Raycast(transform.position, rayDirection, out RaycastHit obstacleHit, viewDistance, sightMask);

                if (obstacleHit.collider != null) {
                    vertex = obstacleHit.point - transform.position;
                    vertex = Quaternion.Euler(new Vector3(0f, -rotation, 0f)) * vertex;
                    vertex = new Vector3(vertex.x, drawHeight, vertex.z);

                    //            if(obstacleHit.collider.gameObject.layer == LayerMask.NameToLayer("Player")) {
                    //                spottedPlayer?.Invoke();
                    //}
                }
                else {
                    vertex = new Vector3(origin.x + direction.x * viewDistance, origin.y, origin.z + direction.z * viewDistance);
                }
            }
			else {
                vertex = new Vector3(origin.x + direction.x * viewDistance, origin.y, origin.z + direction.z * viewDistance);
            }
            vertices[vertexIndex] = vertex;
            if (i > 0) {
                tris[triangleIndex] = 0;
                tris[triangleIndex + 1] = vertexIndex - 1;
                tris[triangleIndex + 2] = vertexIndex;

                triangleIndex += 3;
            }
            vertexIndex++;
            angle -= deltaAngle;

            float lineOfSight = Vector3.Magnitude(vertex);
            var hits = Physics.RaycastAll(transform.position, rayDirection, lineOfSight, tileMask, QueryTriggerInteraction.Collide);
            foreach (var tileHit in hits) {
                var tileObject = tileHit.collider.gameObject;
				if (!tileHits.ContainsKey(tileObject)) {
                    tileHits.Add(tileObject, 1);
                    continue;
				}
                tileHits[tileObject]++;
            }
        }
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = tris;
        meshFilter.mesh = mesh;
        mesh.RecalculateBounds();

        return new VisionProfile() {
            tileHits = tileHits
        };
    }

}
