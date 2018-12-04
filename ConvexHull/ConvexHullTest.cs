using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hont
{
    public class ConvexHullTest : MonoBehaviour
    {
        public ConvexHull convexCull;


        void Update()
        {
            var mat = GetComponent<MeshRenderer>().material;
            if (convexCull.IsInRange(transform.position))
                mat.color = Color.red;
            else
                mat.color = Color.white;
        }
    }
}

