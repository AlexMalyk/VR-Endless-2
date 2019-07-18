using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleDifference : Difference {

    public Material[] possibleColors;
    public bool isChanged;

    public override void Spawn(TrackSegment segment, bool toChange)
    {
        GameObject obj = Instantiate(gameObject, 
            new Vector3(-transform.position.x, transform.position.y, transform.position.z), 
            new Quaternion(transform.rotation.x, transform.rotation.y + 180, transform.rotation.z, transform.rotation.w));

         obj.transform.SetParent(segment.objectRoot, true);

    }

    void ChangeColor(GameObject obj) {
        if (possibleColors.Length > 0)
        {
            Renderer rend = obj.GetComponent<Renderer>();
            rend.material = possibleColors[Random.Range(0, possibleColors.Length)];
            isChanged = true;
        }
    }
}
