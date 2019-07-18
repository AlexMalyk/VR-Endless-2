using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class House : TrackPiece {
    public HouseType type;
    public int numberOfLevels;
    public int numberOfDoors;
    public Vector3[] doorsPositions;
    public int numberOfWindows;
    public Vector3[] windowsPositions;
    public int numberOfRoofs;
    public Vector3[] roofsPositions;

    public ColorData mainColor;         //main color of building
    public ColorData secondColor;       //color of window shutters and doors

    bool isChanged = false;

    void Awake()
    {
        
    }

    public void SetColors() {
        mainColor = ColorsDB.instance.RandomColor;
        secondColor = ColorsDB.instance.RandomColor;
        while(secondColor.name == mainColor.name)
            secondColor = ColorsDB.instance.RandomColor;
        Debug.Log(TrackManager.instance.totalSegmentCount+" "+mainColor.name + " "+ secondColor.name);
    }

    public void GenerateHouse()
    {
        SetColors();
        SetColor(mainColor);

        GeneratePiece(ObjectsDB.instance.allDoors, numberOfDoors, doorsPositions, secondColor);
        GeneratePiece(ObjectsDB.instance.allWindows, numberOfWindows, windowsPositions, secondColor);
        GeneratePiece(ObjectsDB.instance.allRoofs, numberOfRoofs, roofsPositions, secondColor);

    }

    public void GeneratePiece(TrackPiece[] allPieces, int piecesAmount, Vector3[] positions, ColorData color) {
        int i;
        int rand;
        GameObject piecePrefab;
        GameObject piece;
        for (i = 0; i < piecesAmount; i++)
        {
            rand = Random.Range(0, allPieces.Length);

            piecePrefab = allPieces[rand].gameObject;
            piece = InstantiateAndSetParent(piecePrefab);
            piece.transform.localPosition = positions[i];
            piece.GetComponent<TrackPiece>().SetColor(color);
        }
    }

    public GameObject InstantiateAndSetParent(GameObject prefab) {
        GameObject instanciatedObject;
        instanciatedObject = Instantiate(prefab, transform);
        return instanciatedObject;
    }

    public new void SetupDifference()
    {
        meshFilter = GetComponent<MeshFilter>();
        collider = GetComponentInParent<Collider>();

        if (meshFilter == null)
        {
            Debug.LogError("Cannot setup difference. No mesh filter");
        }
        else
        {
            SetColor(ColorsDB.instance.RandomColor);
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
}
public enum HouseType
{
    Standart, Special, Close, Far
}