using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Handle everything related to controlling the character. Interact with both the Character (visual, animation) and CharacterCollider
/// </summary>
public class CharacterInputController : MonoBehaviour
{
	public TrackManager trackManager;

    protected Vector3 m_TargetPosition = Vector3.zero;

    protected readonly Vector3 k_StartingPosition = Vector3.forward * 2f;

    public void Init()
    {
  //      transform.position = Vector3.zero;
		//m_TargetPosition = Vector3.zero;
    }
}
