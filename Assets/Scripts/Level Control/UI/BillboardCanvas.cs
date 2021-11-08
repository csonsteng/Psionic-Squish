using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardCanvas : MonoBehaviour
{
    Camera _camera;
    Quaternion startRotation;
    // Start is called before the first frame update
    void Start()
    {
        _camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        startRotation = transform.rotation;
        gameObject.layer = LayerMask.NameToLayer("WorldUI");
    }

    // Update is called once per frame
    void Update()
    {
        if(_camera == null) {
            Debug.Log("No camera");
            return;
		}
        transform.rotation = _camera.transform.rotation * startRotation;
        gameObject.layer = LayerMask.NameToLayer("WorldUI");
    }

    public void UpdateRotation(Vector3 rotation) {
        transform.Rotate(rotation);
        startRotation = transform.rotation;
    }
}
