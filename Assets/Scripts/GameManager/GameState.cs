using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// Pushed on top of the GameManager during gameplay. Takes care of initializing all the UI and start the TrackManager
/// Also will take care of cleaning when leaving that state.
/// </summary>
public class GameState : AState
{
    public Canvas canvas;
    public TrackManager trackManager;

	protected bool m_Finished;
    protected float m_TimeSinceStart;

    protected RectTransform m_CountdownRectTransform;
    protected bool m_WasMoving;

    public override void Enter(AState from)
    {
        canvas.gameObject.SetActive(true);

        m_TimeSinceStart = 0;
		trackManager.Begin();

		m_Finished = false;
	}

    public override void Exit(AState to)
    {
        canvas.gameObject.SetActive(false);
    }

    public override string GetName()
    {
        return "Game";
    }

    public override void Tick()
    {
		if (m_Finished)
			return;

        m_TimeSinceStart += Time.deltaTime;
    }
}
