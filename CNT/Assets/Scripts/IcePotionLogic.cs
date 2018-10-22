using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IcePotionLogic : MonoBehaviour {

    public float speed;

    public Transform[] selectedPath;
    int activeWaypoint;

    public void StartIcePotion(Transform[] _selectedPath) {
        Debug.Log("asdasd");
        selectedPath = _selectedPath;
        transform.LookAt(selectedPath[0].transform.position);
        transform.position = selectedPath[selectedPath.Length - 1].transform.position;
        activeWaypoint = selectedPath.Length - 2;
	}
	
	void Update () {
        float aux = Vector3.Distance(transform.position, selectedPath[activeWaypoint].transform.position);
        if (aux > 1)
            transform.position = Vector3.MoveTowards(transform.position, selectedPath[activeWaypoint].transform.position, speed * Time.deltaTime);
        else
        {
            if (activeWaypoint == 1)
                gameObject.SetActive(false);
            else
            {
                activeWaypoint--;
            }
        }
	}
}
