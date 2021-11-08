using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TileIndicator : MonoBehaviour
{
    public GameObject partialIndicator;
    public GameObject completeIndicator;
    public GameObject stepIndicator;
    public GameObject stepCount;
    public TextMeshPro textObject;

    private int hitCount=0;
    private Quaternion originalStepIndicatorRotation;
    private Quaternion originalStepCountRotation;
    private Camera _camera;


	private void Start() {
        originalStepIndicatorRotation = stepIndicator.transform.rotation;
        originalStepCountRotation = stepCount.transform.rotation;
        _camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

	private void Update() {
		if (stepCount.activeSelf) {
            if (_camera == null) {
                Debug.Log("No camera");
                return;
            }
            stepCount.transform.LookAt(_camera.transform, Vector3.up);
            stepCount.transform.Rotate(Vector3.up, 180f);
        }
	}

	public void ClearIndicators() {
        partialIndicator.SetActive(false);
        completeIndicator.SetActive(false); 
        stepCount.SetActive(false);
        //stepIndicator.SetActive(false);
        hitCount = 0;
        //textObject.text = "";

    }

    private bool SetHitCount(int count) {
        if(count <= hitCount) {
            return false;
		}
        hitCount = count;
        //textObject.text = count.ToString();
        return true;
	}

    public void SetPartialIndicator(int count) {
        if (SetHitCount(count)) {
            completeIndicator.SetActive(false);
            partialIndicator.SetActive(true);
        }
	}
    public void SetCompleteIndicator(int count) {
        if (SetHitCount(count)) {
            partialIndicator.SetActive(false);
            completeIndicator.SetActive(true);
        }
    }
    
    public void SetStepCount(int step) {

		if (stepIndicator.activeSelf){
            return;
		}
        stepCount.GetComponentInChildren<TextMeshPro>().text = step.ToString();
        stepCount.SetActive(true);
	}

    public void ClearStepCount() {

        stepCount.SetActive(false);
    }

    public void ClearStep() {
        stepIndicator.transform.rotation = originalStepIndicatorRotation;
        stepIndicator.SetActive(false);
	}

    public int GetCurrentHitCount() {
        return hitCount;
	}

    public void ShowStep(Direction facing) {
        stepIndicator.SetActive(true);
        stepIndicator.transform.Rotate(Vector3.forward, 180-facing.Degrees());
        //stepIndicator.transform.Rotate(Vector3.up, facing.Degrees());
        //stepIndicator.transform.SetPositionAndRotation(stepIndicator.transform.position, Quaternion.Euler(new Vector3(, facing.Degrees(), 180f)));
    }

}
