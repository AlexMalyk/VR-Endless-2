using UnityEngine;

/// <summary>
/// This script is the base class of implemented differencies.
/// Derived classes should take care of spawning any object needed for the difference.
/// Possible differencides (only one at time):
/// - change color
/// - missing object
/// - different object
/// - change position(?)
/// - change scale(?)
/// - 
/// </summary>

public abstract class Difference : MonoBehaviour
{
    public abstract void Spawn(TrackSegment segment, bool toChange);
}
