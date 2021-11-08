using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionPointsUI : MonoBehaviour
{

    public GameObject pointTemplate;

    private List<GameObject> points = new List<GameObject>();
    private List<GameObject> pendingPoints = new List<GameObject>();
    private List<GameObject> spendingPoints = new List<GameObject>();

    private bool showingOpaque = true;
    private float timeSinceLast = 0f;
    private bool updating = false;

	private void Update() {
		if (updating) {
            return;
		}
        FlashPending();
        RemoveSpentPoints();

    }

    private void RemoveSpentPoints() {
        if(spendingPoints.Count == 0) {
            return;
		}
        float updateInterval = 0.25f;

        if (timeSinceLast < updateInterval) {
            timeSinceLast += Time.deltaTime;
            return;
        }
        Destroy(spendingPoints[0]);
        spendingPoints.RemoveAt(0);
        showingOpaque = !showingOpaque;
        timeSinceLast = 0f;
    }

    private void FlashPending() {
        float updateInterval = 0.25f;

        if (timeSinceLast < updateInterval) {
            timeSinceLast += Time.deltaTime;
            return;
        }

        foreach (var point in pendingPoints) {
            if (showingOpaque) {
                SetSemiTransparent(point);
            }
            else {
                SetOpaque(point);
            }
        }
        showingOpaque = !showingOpaque;
        timeSinceLast = 0f;

    }

	public void ShowActionPoints(AbstractCharacter character) {
        updating = true;
        int maxPoints = 15;
        float maxAngle = 135f;
        float deltaAngle = maxAngle / maxPoints;

        ClearPointObjects();

        var count = character.ActionPoints();

        for(int i =0; i< count; i++) {
            var actionPoint = Instantiate(pointTemplate, transform);
            actionPoint.transform.Rotate(new Vector3(0f, 0f, -deltaAngle * i));
            actionPoint.GetComponent<Image>().color = character.colorScheme.Accent1;
            actionPoint.SetActive(true);
            points.Add(actionPoint);
        }
        updating = false;
    }

    public void ShowPointsCost(int cost) {
        updating = true;
        ClearPendingPoints();
        if(cost > points.Count) {
            return;
		}
        for (var i=0; i < cost; i++) {
            var pendingPoint = points[points.Count - 1];
            points.Remove(pendingPoint);
            SetSemiTransparent(pendingPoint);
            pendingPoints.Add(pendingPoint);

        }
        updating = false;
    }

    public void SpendPendingPoints() {
        updating = true;
        foreach (var point in pendingPoints) {
            spendingPoints.Add(point);
		}
        pendingPoints.Clear();
        updating = false;
    }

    private void ClearPointObjects() {
        updating = true;
        ClearPendingPoints();
        foreach(var point in points) {
            Destroy(point);
		}
        points.Clear();
        updating = false;
    }

    public void ClearPendingPoints() {
        updating = true;
        //Add them backwards to keep the order
        for (var i=pendingPoints.Count-1; i>=0; i--) {
            var point = pendingPoints[i];
            SetOpaque(point);
            points.Add(point);
		}
        pendingPoints.Clear();
        updating = false;
    }

    private void SetOpaque(GameObject point) {
        var image = point.GetComponent<Image>();
        var color = image.color;
        color.a = 1;
        image.color = color;
	}

    private void SetSemiTransparent(GameObject point) {
        var image = point.GetComponent<Image>();
        var color = image.color;
        color.a = 0.2f;
        image.color = color;
    }
}
