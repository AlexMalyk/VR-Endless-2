using UnityEngine;
using UnityEngine.EventSystems;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// This defines a "piece" of the track. This is attached to the prefab and contains data such as what obstacles can spawn on it.
/// It also defines places on the track where obstacles can spawn. The prefab is placed into a ThemeData list.
/// </summary>
public class TrackSegment : MonoBehaviour
{
    public Transform pathParent;
    public TrackManager manager;

	public Transform objectRoot;

    public float worldLength { get { return m_WorldLength; } }

    public GameObject waterZone;
    public GameObject sidewalkZone;
    public GameObject sidewalkZoneLeft;
    public GameObject sidewalkZoneRight;
    public GameObject houseZone;
    public GameObject houseZoneLeft;
    public GameObject houseZoneRight;
    public GameObject differenceZone;
    public GameObject colidersZone;
    public bool withWater;
    public bool withSidewalls;

    protected float m_WorldLength;

    public static bool withSidewall;


    public void GenerateSegment() {

        int rand = Random.Range(0, ObjectsDB.instance.allHouses.Length);        
        House housePrefab = ObjectsDB.instance.allHouses[rand];

        GameObject houseLeft = Instantiate(housePrefab.gameObject, houseZoneLeft.transform);
        houseLeft.GetComponent<House>().GenerateHouse();


        GameObject houseRight = Instantiate(houseLeft, houseZoneRight.transform);
        PairObjects(houseLeft, houseRight);


        if (Random.Range(0, 100) > 50)
        {
            SetDifferenceInZone(houseLeft);
            Debug.Log("Difference on the left of " + TrackManager.instance.totalSegmentCount+" tile");
        }
        else {
            SetDifferenceInZone(houseRight);
            Debug.Log("Difference on the right of " + TrackManager.instance.totalSegmentCount + " tile");
        }

        
        rand = Random.Range(0, ObjectsDB.instance.allSidewalks.Length);
        Sidewalk sidewalkPrefab = ObjectsDB.instance.allSidewalks[rand];
        GameObject sidewalkLeft = Instantiate(sidewalkPrefab.gameObject, sidewalkZoneLeft.transform);
        GameObject sidewalkRight = Instantiate(sidewalkPrefab.gameObject, sidewalkZoneRight.transform);

        if (withWater)
        {
            rand = Random.Range(0, ObjectsDB.instance.allWater.Length);
            Water waterPrefab = ObjectsDB.instance.allWater[rand];
            GameObject water = Instantiate(waterPrefab.gameObject, waterZone.transform);
        }
        if (gameObject.GetComponentInChildren<MeshesCombiner>().isActiveAndEnabled)
        {
            gameObject.GetComponentInChildren<MeshesCombiner>().CombineMeshes();
        }
    }

    void SetDifferenceInZone(GameObject zone) {
        TrackPiece[] zonePieces = zone.GetComponentsInChildren<TrackPiece>();
        int pieceIndex;
        while (true) {
            if (Random.Range(0, 100) > 50)
            {
                pieceIndex = Random.Range(0, zonePieces.Length);
                zonePieces[pieceIndex].isDifference = true;
                zonePieces[pieceIndex].SetupDifference();
            }
            else {
                break;
            }
        }
    }

    void PairObjects(GameObject first, GameObject second) {
        TrackPiece[] firstTrackPieces = first.GetComponentsInChildren<TrackPiece>();
        TrackPiece[] secondTrackPieces = second.GetComponentsInChildren<TrackPiece>();

        if (firstTrackPieces.Length == secondTrackPieces.Length)
        {
            int i = 0;
            while (i < firstTrackPieces.Length)
            {
                firstTrackPieces[i].pairedObject = secondTrackPieces[i];
                secondTrackPieces[i].pairedObject = firstTrackPieces[i];

                i++;
            }
        }
        else {
            Debug.Log("Wrong generated track segments");
        }
    }

    void OnEnable()
    {
        UpdateWorldLength();
        if (!objectRoot)
        {
            GameObject obj = new GameObject("ObjectRoot");
            obj.transform.SetParent(transform);
            objectRoot = obj.transform;
        }
    }



    public void ShowHint() {
        TrackPiece[] pieces = GetComponentsInChildren<TrackPiece>();
    }


    // Same as GetPointAt but using an interpolation parameter in world units instead of 0 to 1.
    public void GetPointAtInWorldUnit(float wt, out Vector3 pos, out Quaternion rot)
    {
        float t = wt / m_WorldLength;
        GetPointAt(t, out pos, out rot);
    }


	// Interpolation parameter t is clamped between 0 and 1.
	public void GetPointAt(float t, out Vector3 pos, out Quaternion rot)
    {
        float clampedT = Mathf.Clamp01(t);
        float scaledT = (pathParent.childCount - 1) * clampedT;
        int index = Mathf.FloorToInt(scaledT);
        float segmentT = scaledT - index;

        Transform orig = pathParent.GetChild(index);
        if (index == pathParent.childCount - 1)
        {
            pos = orig.position;
            rot = orig.rotation;
            return;
        }

        Transform target = pathParent.GetChild(index + 1);

        pos = Vector3.Lerp(orig.position, target.position, segmentT);
        rot = Quaternion.Lerp(orig.rotation, target.rotation, segmentT);
    }

    protected void UpdateWorldLength()
    {
        m_WorldLength = 0;

        for (int i = 1; i < pathParent.childCount; ++i)
        {
            Transform orig = pathParent.GetChild(i - 1);
            Transform end = pathParent.GetChild(i);

            Vector3 vec = end.position - orig.position;
            m_WorldLength += vec.magnitude;
        }
    }

	public void Cleanup()
	{
		Destroy(gameObject);
	}

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        if (pathParent == null)
            return;

        Color c = Gizmos.color;
        Gizmos.color = Color.red;
        for (int i = 1; i < pathParent.childCount; ++i)
        {
            Transform orig = pathParent.GetChild(i - 1);
            Transform end = pathParent.GetChild(i);

            Gizmos.DrawLine(orig.position, end.position);
        }

        Gizmos.color = c;
    }
#endif
}