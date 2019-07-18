using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class MeshesCombiner : MonoBehaviour {

    public MeshFilter[] meshFilters;
    public CombineInstance[] combine;
    public int amountOfMeshes;
    public bool combineOnStart = false;
    public bool testReduction = false;
    bool combined = false;

    void Start() {
        if (combineOnStart)
        {
            CombineMeshes();
        }
    }

    public void CombineMeshes() {
        if (combined) {
            transform.GetComponent<MeshFilter>().mesh.Clear();
            transform.GetComponent<MeshFilter>().mesh = null;
        }
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];
        int i = 0;
        while (i < meshFilters.Length)
        {
            meshFilters[i].gameObject.GetComponent<MeshRenderer>().enabled = true;


            combine[i].mesh = meshFilters[i].sharedMesh;

            //fix wrong position of combined mesh by saving parent`s worldToLocalMatrix
            Matrix4x4 myTransform = transform.worldToLocalMatrix;
            combine[i].transform = myTransform * meshFilters[i].transform.localToWorldMatrix;

            meshFilters[i].gameObject.GetComponent<MeshRenderer>().enabled = false;


            i++;
        }

        if (gameObject.GetComponent<MeshFilter>() == null)
        {
            gameObject.AddComponent<MeshFilter>();
        }
        gameObject.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
        gameObject.GetComponent<MeshRenderer>().enabled = true;

        combined = true;
        //DetectColor();
    }

    //public void DetectColor()
    //{
    //    Vector2[] uvs = GetComponent<MeshFilter>().mesh.uv;
    //    Dictionary<string, List<Vector2>> dict = new Dictionary<string, List<Vector2>>();
    //    string colorName;
    //    Vector2[] position = new Vector2[2];

    //    for (int i = 0; i < uvs.Length; i++)
    //    {
    //        colorName = ColorsDB.instance.GetColorDataByVector(uvs[i].x, uvs[i].y);


    //        if (colorName != null)
    //        {
    //            if (dict.ContainsKey(colorName))
    //            {
    //                dict[colorName].Add(uvs[i]);
    //            }
    //            else
    //            {
    //                dict.Add(colorName, new List<Vector2>());
    //                dict[colorName].Add(uvs[i]);
    //            }
    //        }
    //        else {
    //            Debug.Log("No color for such uv" + uvs[i].x + uvs[i].y);
    //        }
    //    }

        //Dictionary<string, int> test = new Dictionary<string, int>();
        //test.Add("one", 1);
        //test.Add("two", 2);

        //List<string> keyList = new List<string>(dict.Keys);
        //foreach (string key in keyList) {
        //    Debug.Log(key);
        //}
        //Debug.Log(keyList.Count);
    //}

}


