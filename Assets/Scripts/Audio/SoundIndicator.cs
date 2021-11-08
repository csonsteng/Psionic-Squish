using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundIndicator : MonoBehaviour
{
    public GameObject indicator;
    public float growthRate = 0.3f;
    private float scale = 0;
    // Start is called before the first frame update
    void Start()
    {
        indicator.layer = 0;
        indicator.transform.localScale = Vector3.zero;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (scale < 2.5) {
            scale+=growthRate;
            Vector3 scaleVector = new Vector3(scale, scale, scale);
            indicator.transform.localScale = scaleVector;
        }
        else {
            Destroy(gameObject);
        }
    }
}
