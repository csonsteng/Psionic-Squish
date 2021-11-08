using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Structures {
    public class StructureSceneObject : MonoBehaviour {
        public GameObject transparentStructure;
        public GameObject opaqueStructure;

        public void ShowTransparent() {
            transparentStructure.SetActive(true);
            opaqueStructure.SetActive(false);
        }


        public void ShowOpaque() {
            transparentStructure.SetActive(false);
            opaqueStructure.SetActive(true);
        }
    }
}
