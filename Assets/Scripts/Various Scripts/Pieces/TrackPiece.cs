using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


[RequireComponent(typeof(EventTrigger))]
[RequireComponent(typeof(MeshFilter))]
public class TrackPiece : MonoBehaviour
{
    public Vector3 position;
    public ColorData color;
    public bool isDifference;
    public bool isInteractable;
    public GameObject[] conflictingbjects;
    public TrackPiece pairedObject;


    protected MeshFilter meshFilter;
    protected EventTrigger trigger;
    protected Collider collider;

    public void SetColor(ColorData c)
    {
        meshFilter = GetComponent<MeshFilter>();
        if (meshFilter == null)
        {
            Debug.Log(this.gameObject.name);
        }
            Vector2[] uvs = meshFilter.mesh.uv;
            for (int i = 0; i < uvs.Length; i++)
            {
                uvs[i] = c.textureCoordinates;
            }
            meshFilter.mesh.uv = uvs;

            color = c;
    }


    public void SetupDifference()
    {
        meshFilter = GetComponent<MeshFilter>();
        collider = GetComponentInParent<Collider>();

        if (meshFilter == null)
        {
            Debug.LogError("Cannot setup difference. No mesh filter");
        }
        else {
            SetColor(ColorsDB.instance.RandomColor);
            Debug.Log(this.gameObject.name + " "+ color.name);
        }
        if (collider = null)
        {
            Debug.LogError("Wrong hierarchy. No collider to detect the difference");
            return;
        }

        SetEventClick();
        pairedObject.SetEventClick();

        isDifference = true;
        pairedObject.isDifference = true;
    }

    public void SetEventClick() {
        trigger = GetComponentInParent<EventTrigger>();

        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        entry.callback.AddListener((data) => { ColorDifference(); });
        trigger.triggers.Add(entry);
    }

    public void ColorDifference() {
        if (isDifference)
        {
            Debug.Log("color difference");
            if (this.color.textureCoordinates != pairedObject.color.textureCoordinates)
            {
                Debug.Log("Change color from " + 
                    ColorsDB.instance.GetColorDataByVector(color.textureCoordinates.x, color.textureCoordinates.y) + 
                    " to " + 
                    ColorsDB.instance.GetColorDataByVector(pairedObject.color.textureCoordinates.x, pairedObject.color.textureCoordinates.y));
                SetColor(pairedObject.color);
            }
            transform.GetComponentInParent<MeshesCombiner>().CombineMeshes();

            isDifference = false;
            pairedObject.isDifference = false;

            TrackManager.instance.AddScore();
        }
    }


}