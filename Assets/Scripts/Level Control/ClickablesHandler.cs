using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class ClickablesHandler : MonoBehaviour
{
    [HideInInspector]
    public GameObject hitObject;
    [HideInInspector]
    public UnityEvent objectClicked = new UnityEvent();

    [HideInInspector]
    public GameObject hoveredObject;
    [HideInInspector]
    public GameObject unHoveredObject;
    [HideInInspector]
    public UnityEvent objectHovered = new UnityEvent();

    private Camera sceneCamera;
    // Start is called before the first frame update
    void Start()
    {
        sceneCamera = FindObjectOfType<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        var mouse = Mouse.current;
        if(mouse == null) {
            return;
		}
        if (EventSystem.current.IsPointerOverGameObject()) {
            return;
		}
        Ray ray = sceneCamera.ScreenPointToRay(mouse.position.ReadValue());
        if (Physics.Raycast(ray,  out RaycastHit hit, Mathf.Infinity, ~0, QueryTriggerInteraction.Ignore)) {
            if (hoveredObject != hit.transform.parent.gameObject) {
                unHoveredObject = hoveredObject;
                hoveredObject = hit.transform.parent.gameObject;    //collider is in a child object of the objects
                objectHovered?.Invoke();
            }
            if (mouse.leftButton.wasPressedThisFrame) {
                hitObject = hoveredObject;
                objectClicked?.Invoke();
            }
        }
    }
}
