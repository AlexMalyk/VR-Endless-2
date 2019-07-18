using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
#if UNITY_ANALYTICS
using UnityEngine.Analytics;
#endif

/// <summary>
/// The TrackManager handles creating track segments, moving them and handling the whole pace of the game.
/// 
/// The cycle is as follows:
/// - Begin is called when the game starts.
///     - if it's a first run, init the controller, collider etc. and start the movement of the track.
///     - if it's a rerun (after watching ads on GameOver) just restart the movement of the track.
/// - Update moves the character and - if the character reaches a certain distance from origin (given by floatingOriginThreshold) -
/// moves everything back by that threshold to "reset" the player to the origin. This allow to avoid floating point error on long run.
/// It also handles creating the tracks segements when needed.
/// 
/// If the player has no more lives, it pushes the GameOver state on top of the GameState without removing it. That way we can just go back to where
/// we left off if the player watches an ad and gets a second chance. If the player quits, then:
/// 
/// - End is called and everything is cleared and destroyed, and we go back to the Loadout State.
/// </summary>
public class TrackManager : MonoBehaviour
{
	static public TrackManager instance { get { return s_Instance; } }
	static protected TrackManager s_Instance;

	[Header("Character & Movements")]
	public CharacterInputController characterController;
    public GameObject cameraPlaceholder;
	public float speed = 5.0f;

	public float worldDistance {  get { return m_TotalWorldDistance; } }

	public TrackSegment currentSegment { get { return m_Segments[0]; } }
    public TrackSegment lastSpawned { get {
            Debug.Log(m_Segments.Count - 1);
            return m_Segments[m_Segments.Count-1]; } }
	public List<TrackSegment> segments { get { return m_Segments; } }

    public TrackSegment[] prefabList;

    protected float m_CurrentSegmentDistance;
	protected float m_TotalWorldDistance;

	protected List<TrackSegment> m_Segments = new List<TrackSegment>();
	protected List<TrackSegment> m_PastSegments = new List<TrackSegment>();
	protected int m_SafeSegementLeft;

	protected int m_CurrentZone;
	protected float m_CurrentZoneDistance;
	protected int m_PreviousSegment = -1;

    const float k_FloatingOriginThreshold = 10000f;

    protected const float k_StartingSegmentDistance = 2f;
    protected const int k_StartingSafeSegments = 2;
    public  int k_DesiredSegmentCount = 5;
    public float k_SegmentRemovalDistance = -60f;//-150f;


    public TrackSegment trackSeg;

    public int totalSegmentCount=0;


    public TextMesh scoreText;
    public int score = 0;

    protected void Awake()
	{
		s_Instance = this;
    }

	public void Begin()
	{
			// Since this is not a rerun, init the whole system (on rerun we want to keep the states we had on death)
			m_CurrentSegmentDistance = k_StartingSegmentDistance;
			m_TotalWorldDistance = 0.0f;

			m_CurrentZone = 0;
			m_CurrentZoneDistance = 0;
            
            gameObject.SetActive(true);
            m_SafeSegementLeft = k_StartingSafeSegments;
	}

	public void End()
	{
	    foreach (TrackSegment seg in m_Segments)
	    {
	        Destroy(seg.gameObject);
	    }

	    for (int i = 0; i < m_PastSegments.Count; ++i)
	    {
	        Destroy(m_PastSegments[i].gameObject);
	    }

		m_Segments.Clear();
		m_PastSegments.Clear();


		gameObject.SetActive(false);
	}


	void Update ()
	{
        while (m_Segments.Count < k_DesiredSegmentCount)
		{
			SpawnNewSegment();
		}

		float scaledSpeed = speed * Time.deltaTime;
		m_CurrentZoneDistance += scaledSpeed;

		m_TotalWorldDistance += scaledSpeed;
		m_CurrentSegmentDistance += scaledSpeed;

		if(m_CurrentSegmentDistance > m_Segments[0].worldLength)
		{
			m_CurrentSegmentDistance -= m_Segments[0].worldLength;

            // m_PastSegments are segment we already passed, we keep them to move them and destroy them later 
            // but they aren't part of the game anymore 


            m_PastSegments.Add(m_Segments[0]);
			m_Segments.RemoveAt(0);

		}

		Vector3 currentPos;
		Quaternion currentRot;
		Transform characterTransform = characterController.transform;

		m_Segments[0].GetPointAtInWorldUnit(m_CurrentSegmentDistance, out currentPos, out currentRot);


		// Floating origin implementation
        // Move the whole world back to 0,0,0 when we get too far away.
		bool needRecenter = currentPos.sqrMagnitude > k_FloatingOriginThreshold;

		if (needRecenter)
        {
			int count = m_Segments.Count;
			for(int i = 0; i < count; i++)
            {
				m_Segments[i].transform.position -= currentPos;
			}

			count = m_PastSegments.Count;
			for(int i = 0; i < count; i++)
            {
				m_PastSegments[i].transform.position -= currentPos;
			}

			// Recalculate current world position based on the moved world
			m_Segments[0].GetPointAtInWorldUnit(m_CurrentSegmentDistance, out currentPos, out currentRot);
		}

		characterTransform.rotation = currentRot;
		characterTransform.position = currentPos;
        
        // Still move past segment until they aren't visible anymore.
        for (int i = 0; i < m_PastSegments.Count; ++i)
		{
            if ((m_PastSegments[i].transform.position - currentPos).z < k_SegmentRemovalDistance)
			{
				m_PastSegments[i].Cleanup();
				m_PastSegments.RemoveAt(i);
				i--;
			}
		}
    }

    //public void SpawnNewSegment()
    //{
    //	int segmentUse = Random.Range(0, prefabList.Length);

    //	TrackSegment segmentToUse = prefabList[segmentUse];
    //       TrackSegment newSegment = Instantiate(segmentToUse, Vector3.zero, Quaternion.identity);


    //       Vector3 currentExitPoint;
    //	Quaternion currentExitRotation;
    //	if (m_Segments.Count > 0)
    //	{
    //		m_Segments[m_Segments.Count - 1].GetPointAt(1.0f, out currentExitPoint, out currentExitRotation);
    //	}
    //	else
    //	{
    //		currentExitPoint = transform.position;
    //		currentExitRotation = transform.rotation;
    //	}

    //	newSegment.transform.rotation = currentExitRotation;

    //	Vector3 entryPoint;
    //	Quaternion entryRotation;
    //	newSegment.GetPointAt(0.0f, out entryPoint, out entryRotation);


    //	Vector3 pos = currentExitPoint + (newSegment.transform.position - entryPoint);
    //	newSegment.transform.position = pos;
    //	newSegment.manager = this;

    //	newSegment.transform.localScale = new Vector3(1, 1, 1);
    //	newSegment.objectRoot.localScale = new Vector3(1.0f/newSegment.transform.localScale.x, 1, 1);

    //       if (m_SafeSegementLeft <= 0)
    //           SpawnDifferencies(newSegment);
    //       else
    //           m_SafeSegementLeft -= 1;

    //       m_Segments.Add(newSegment);
    //}

    //   public void SpawnDifferencies(TrackSegment segment)
    //   {

    //       if (segment.possibleDifferencies.Length != 0)
    //       {
    //           for (int i = 0; i < segment.possibleDifferencies.Length; ++i)
    //           {
    //               //segment.possibleDifferencies[Random.Range(0, segment.possibleDifferencies.Length)].Spawn(segment, segment.differencePositions[i]);
    //               segment.possibleDifferencies[i].Spawn(segment, Random.value > 0.5 ? true : false);
    //           }
    //       }
    //   }

    public void SpawnNewSegment() {
        totalSegmentCount++;

        TrackSegment newSegment = Instantiate(trackSeg);
        newSegment.name = totalSegmentCount.ToString();
        newSegment.GenerateSegment();


        Vector3 currentExitPoint;
        Quaternion currentExitRotation;
        if (m_Segments.Count > 0)
        {
            m_Segments[m_Segments.Count - 1].GetPointAt(1.0f, out currentExitPoint, out currentExitRotation);
        }
        else
        {
            currentExitPoint = transform.position;
            currentExitRotation = transform.rotation;
        }

        newSegment.transform.rotation = currentExitRotation;

        Vector3 entryPoint;
        Quaternion entryRotation;
        newSegment.GetPointAt(0.0f, out entryPoint, out entryRotation);


        Vector3 pos = currentExitPoint + (newSegment.transform.position - entryPoint);
        newSegment.transform.position = pos;
        newSegment.manager = this;

        m_Segments.Add(newSegment);
    }

    public void AddScore() {
        score++;
        scoreText.text = "Score" + score.ToString();
    }

    public void Hint() {
        currentSegment.ShowHint();
        Debug.Log(currentSegment);
    }
    public void StopOrPlay() {
        if (speed > 0)
        {
            speed = 0;
        }
        else {
            speed = 2;
        }
    }
    public void SpeedUp() {
        speed *= 2;
    }
}