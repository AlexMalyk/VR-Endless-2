using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterSwap : MonoBehaviour {

	public GameObject waterLQ;
	public GameObject waterHQ;

	public void Swap(){
		waterLQ.SetActive(!waterLQ.activeSelf);
		waterHQ.SetActive(!waterHQ.activeSelf);
	}
}
