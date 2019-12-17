﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutCreateGrid : MonoBehaviour
{
    public Transform gridPreFab;
    public Transform wall;
    public Camera cam;
    public Vector3 camRotatePoint;
    public List<Vector3> positions = new List<Vector3>();
    public float ySpeed = 20.0f;
    Transform wall1;
    Transform wall2;
    Transform wall3;
    Transform wall4;


    // Start is called before the first frame update
    void Start()
    {
        // Populate the positions list with all possible positions for grids
        for (int i = 0; i < GameObject.Find("Game").GetComponent<TutGameMain>().worldSize; i++)
        {
            for (int j = 0; j < GameObject.Find("Game").GetComponent<TutGameMain>().worldSize; j++)
            {
                Vector3 pos = new Vector3(i, 1.0f, j);
                positions.Add(pos);
            }
        }

        // Place grid prefabs at positions from grid.
        for (int i = 0; i < positions.Count; i++)
        {
            Transform t = Instantiate(gridPreFab);
            t.localPosition = positions[i];
        }

        // Move camera into correct location based on the world size.
        UpdateCameraPos();

        // Create retaining walls in world to keep plant eaters and meat eaters from getting stuck.
        wall1 = Instantiate(wall);
        wall2 = Instantiate(wall);
        wall3 = Instantiate(wall);
        wall4 = Instantiate(wall);

        wall1.localScale = new Vector3(GameObject.Find("Game").GetComponent<TutGameMain>().worldSize + 1, 2f, .3f);
        wall1.localPosition = new Vector3((GameObject.Find("Game").GetComponent<TutGameMain>().worldSize / 2) - .5f, 1f, -.5f);
        wall2.localScale = new Vector3(GameObject.Find("Game").GetComponent<TutGameMain>().worldSize + 1, 2f, .3f);
        wall2.localPosition = new Vector3((GameObject.Find("Game").GetComponent<TutGameMain>().worldSize / 2) - .5f, 1f,
            GameObject.Find("Game").GetComponent<TutGameMain>().worldSize - .5f);
        wall3.localScale = new Vector3(.3f, 2f, GameObject.Find("Game").GetComponent<TutGameMain>().worldSize + 1);
        wall3.localPosition = new Vector3(-.5f, 1f, (GameObject.Find("Game").GetComponent<TutGameMain>().worldSize / 2) - .5f);
        wall4.localScale = new Vector3(.3f, 2f, GameObject.Find("Game").GetComponent<TutGameMain>().worldSize + 1);
        wall4.localPosition = new Vector3(GameObject.Find("Game").GetComponent<TutGameMain>().worldSize - .5f, 1f,
            (GameObject.Find("Game").GetComponent<TutGameMain>().worldSize / 2) - .5f);

        // Set the rotation point for the camera to rotate around when the user operates camera.
        camRotatePoint = new Vector3((GameObject.Find("Game").GetComponent<TutGameMain>().worldSize / 2), 0f, (GameObject.Find("Game").GetComponent<TutGameMain>().worldSize / 2));
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(2))
        {
            cam.transform.RotateAround(camRotatePoint, Vector3.up, Input.GetAxis("Mouse X") * ySpeed);
        }
    }

    // Update Grid when user presses the world size button.
    public void UpdateGrid()
    {
        Debug.Log("Add Grids");
        // Populate the news positions when player pushed the world size button.
        for (int i = 0; i < GameObject.Find("Game").GetComponent<TutGameMain>().worldSize; i++)
        {
            for (int j = GameObject.Find("Game").GetComponent<TutGameMain>().worldSize - 1; j < GameObject.Find("Game").GetComponent<TutGameMain>().worldSize; j++)
            {
                Vector3 foodPos = new Vector3(i, 1.0f, j);
                Vector3 plantEaterPos = new Vector3(i, 1.4f, j);
                positions.Add(foodPos);
                GameObject.Find("Game").GetComponent<TutGameMain>().foodPositions.Add(foodPos);
                GameObject.Find("Game").GetComponent<TutGameMain>().plantEaterStartPositions.Add(plantEaterPos);
            }
        }
        for (int i = GameObject.Find("Game").GetComponent<TutGameMain>().worldSize - 1; i < GameObject.Find("Game").GetComponent<TutGameMain>().worldSize; i++)
        {
            for (int j = 0; j < GameObject.Find("Game").GetComponent<TutGameMain>().worldSize; j++)
            {
                Vector3 foodPos = new Vector3(i, 1.0f, j);
                Vector3 plantEaterPos = new Vector3(i, 1.4f, j);
                positions.Add(foodPos);
                GameObject.Find("Game").GetComponent<TutGameMain>().foodPositions.Add(foodPos);
                GameObject.Find("Game").GetComponent<TutGameMain>().plantEaterStartPositions.Add(plantEaterPos);
            }
        }

        // Place grid prefabs at positions from grid.
        for (int i = 2 * (GameObject.Find("Game").GetComponent<TutGameMain>().worldSize - 1); i < positions.Count; i++)
        {
            Transform t = Instantiate(gridPreFab);
            t.localPosition = positions[i];
        }

        // Place retaining walls in world to keep plant eaters and meat eaters from getting stuck.
        wall1.localScale = new Vector3(GameObject.Find("Game").GetComponent<TutGameMain>().worldSize, 2f, .3f);
        wall1.localPosition = new Vector3((GameObject.Find("Game").GetComponent<TutGameMain>().worldSize / 2) - .5f, 1f, -.5f);
        wall2.localScale = new Vector3(GameObject.Find("Game").GetComponent<TutGameMain>().worldSize, 2f, .3f);
        wall2.localPosition = new Vector3((GameObject.Find("Game").GetComponent<TutGameMain>().worldSize / 2) - .5f, 1f,
            GameObject.Find("Game").GetComponent<TutGameMain>().worldSize - .5f);
        wall3.localScale = new Vector3(.3f, 2f, GameObject.Find("Game").GetComponent<TutGameMain>().worldSize);
        wall3.localPosition = new Vector3(-.5f, 1f, (GameObject.Find("Game").GetComponent<TutGameMain>().worldSize / 2) - .5f);
        wall4.localScale = new Vector3(.3f, 2f, GameObject.Find("Game").GetComponent<TutGameMain>().worldSize);
        wall4.localPosition = new Vector3(GameObject.Find("Game").GetComponent<TutGameMain>().worldSize - .5f, 1f,
            (GameObject.Find("Game").GetComponent<TutGameMain>().worldSize / 2) - .5f);


    }

    // Update camera position
    public void UpdateCameraPos()
    {
        // Set the rotation point for the camera to rotate around when the user operates camera.
        camRotatePoint = new Vector3((GameObject.Find("Game").GetComponent<TutGameMain>().worldSize / 2), 0f, (GameObject.Find("Game").GetComponent<TutGameMain>().worldSize / 2));

        Vector3 camPos = new Vector3(GameObject.Find("Game").GetComponent<TutGameMain>().worldSize / 2, GameObject.Find("Game").GetComponent<TutGameMain>().worldSize, -GameObject.Find("Game").GetComponent<TutGameMain>().worldSize / 2);
        cam.transform.localPosition = camPos;
        cam.transform.LookAt(camRotatePoint);


    }
}