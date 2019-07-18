using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectsDB : MonoBehaviour {

    public static ObjectsDB instance;
    
    public House[] allHouses;
    public Window[] allWindows;
    public Door[] allDoors;
    public Roof[] allRoofs;
    public Sidewall[] allSidewalls;
    public Sidewalk[] allSidewalks;
    public Water[] allWater;
    

    //Awake is always called before any Start functions
    void Awake()
    {
        //Check if instance already exists
        if (instance == null)

            //if not, set instance to this
            instance = this;

        //If instance already exists and it's not this:
        else if (instance != this)

            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);

        //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);
    }
}
