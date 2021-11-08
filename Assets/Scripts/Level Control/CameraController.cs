using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{

    public bool flipScrollDirection = false;
    public float cameraSpeed;
    public float mouseDeadZone;
    public float minXAngle = 0;
    public float maxXAngle = 90;
    public GameObject cameraHolder;

    private Camera mainCamera;
    private Vector3 cameraOffset;
    // Start is called before the first frame update
    void Start()
    {
        mainCamera = GetComponent<Camera>();
        SetCameraOffset();
    }

	private void Update() {

  //      var euler = mainCamera.transform.rotation.eulerAngles;
  //      if (euler.x < minXAngle) {
  //          mainCamera.transform.rotation = (Quaternion.Euler(minXAngle, euler.y, euler.z));
		//}

  //      if (euler.x > maxXAngle) {
  //          mainCamera.transform.rotation = (Quaternion.Euler(maxXAngle, euler.y, euler.z));
  //      }

        var mouse = Mouse.current;
        if (mouse != null) {
			if (mouse.rightButton.isPressed) {
                Vector2 mouseDelta = mouse.delta.ReadValue();
                if(Mathf.Abs(mouseDelta.y) > mouseDeadZone) {
                    Vector3 rotationAxis = new Vector3(Mathf.Cos(Theta().y),0f, -Mathf.Sin(Theta().y));
                    if (mainCamera.transform.rotation.eulerAngles.x > minXAngle || mouseDelta.y < 0 && mainCamera.transform.rotation.eulerAngles.x < maxXAngle || mouseDelta.y > 0) {
                       // mainCamera.transform.RotateAround(GetGroundPosition(), rotationAxis, -cameraSpeed * mouseDelta.y);
					}
                }
                if(Mathf.Abs(mouseDelta.x) > mouseDeadZone) {
                    cameraHolder.transform.RotateAround(GetGroundPosition(), Vector3.up, cameraSpeed*mouseDelta.x);
				}
			}else if (mouse.middleButton.isPressed) {
                Vector2 mouseDelta = mouse.delta.ReadValue();
                if (Mathf.Abs(mouseDelta.y) > mouseDeadZone) {
                    mainCamera.transform.localPosition -= new Vector3(0f, 0f, cameraSpeed*mouseDelta.y/10f);
                }
                if (Mathf.Abs(mouseDelta.x) > mouseDeadZone) {
                    mainCamera.transform.localPosition -= new Vector3(cameraSpeed*mouseDelta.x/10f, 0f, 0f);
                }
            }

            var scrollVal = mouse.scroll.y.ReadValue();
            if (scrollVal != 0) {
                float deltaY = -Mathf.Sin(Theta().x);
                float deltaX = Mathf.Sin(Theta().y);
                float deltaZ = Mathf.Cos(Theta().y);// + Mathf.Cos(Theta().x);

                Vector3 deltaTransform = cameraSpeed * new Vector3(deltaX, deltaY, deltaZ);
                if (flipScrollDirection) {
                    scrollVal = -scrollVal;
                }
                mainCamera.transform.position += Mathf.Sign(scrollVal) * deltaTransform;
            }
        }
    }

	// Update is called once per frame
	void FixedUpdate()
    {


        var keyboard = Keyboard.current;
        if (keyboard != null) {
            if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed) {
                mainCamera.transform.localPosition  += new Vector3(0f, 0f, cameraSpeed);
            }
            if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed) {
                mainCamera.transform.localPosition -= new Vector3(0f, 0f, cameraSpeed);
            }
            if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed) {
                mainCamera.transform.localPosition -= new Vector3(cameraSpeed, 0f, 0f);
            }
            if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) {
                mainCamera.transform.localPosition += new Vector3(cameraSpeed, 0f, 0f);
            }
        }
    }

    public void AlignCamera(Vector3 target) {
        var cameraDestination = target + cameraOffset;
        mainCamera.transform.position = cameraDestination;
	}

    private Vector3 Theta() {
        Vector3 theta = new Vector3() {
            x = mainCamera.transform.rotation.eulerAngles.x * Mathf.PI / 180f,
            y = mainCamera.transform.rotation.eulerAngles.y * Mathf.PI / 180f,
            z = mainCamera.transform.rotation.eulerAngles.z * Mathf.PI / 180f,
        };
        return theta;
	}

    private void SetCameraOffset() {
        cameraOffset = GetCameraOffset();
    }

    private Vector3 GetCameraOffset() {

        return mainCamera.transform.position - GetGroundPosition();
    }

    private Vector3 GetGroundPosition() {
        Ray worldRay = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        groundPlane.Raycast(worldRay, out float distanceToGround);
        return worldRay.GetPoint(distanceToGround);
    }
}
