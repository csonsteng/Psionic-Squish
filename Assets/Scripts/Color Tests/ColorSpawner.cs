using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorSpawner : MonoBehaviour
{

    public GameObject colorRepresentation;
    public int dimension = 10;
    public float spacing = 1.5f;

    // Start is called before the first frame update
    void Start()
    {
        List<Color> colors = new List<Color>();
        for(var i=0; i< dimension; i++) {
            for(var j=0; j < dimension; j++) {
                var colorObject = Instantiate(colorRepresentation, transform);
                colorObject.transform.position = new Vector3(i * spacing, 0, j * spacing);
                ColorRepresentation representation = colorObject.GetComponent<ColorRepresentation>();
                colors.Add(representation.Initialize(colors));
                colorObject.SetActive(true);
			}
		}
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
