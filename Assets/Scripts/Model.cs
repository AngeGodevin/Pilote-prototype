using System;
using System.Collections;

using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
public class Model : MonoBehaviour
{
    enum State
    {
        IDLE,
        EXP_ON_GOING,
    }
    State state;
    public GameObject shape;
    List<GameObject> gameObjects;
    public List<int> distances;
    public int numberOfAngles;
    public int duration;
    private float currentTime;

    private System.Random rng = new System.Random();


    // Start is called before the first frame update
    void Start()
    {
        // state = State.IDLE;
        state = State.EXP_ON_GOING;
        gameObjects = new List<GameObject>();
        generateTargets();
        Shuffle(this.gameObjects);
        this.currentTime = 0;
        this.gameObjects[0].SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        switch (this.state)
        {
            case State.IDLE:
                //start the exp button
                break;
            case State.EXP_ON_GOING:
                Vector3 vec = Input.mousePosition;
                if (Input.GetMouseButtonDown(0))
                {
                    Debug.Log(IsTargetHit().ToString());
                    NextTarget();
                }
                if (!isTimerRunning())
                {
                    this.state = State.IDLE;
                    //register data
                }
                break;
        }
    }


    /*   private void OnTriggerEnter(Collider other)
       {
           if (other.tag == "collectable")
           {
               Debug.Log("clicked");
               Destroy(this.gameObject);
           }
       }*/

    private bool isTimerRunning()
    {
        if (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            Debug.Log(currentTime.ToString());
            return true;
        }
        return false;
    }
    private void Shuffle(List<GameObject> list)
    {
        List<GameObject> temp = new List<GameObject>();
        temp.AddRange(list);
        list.Clear();
        list.AddRange(temp.OrderBy(_ => rng.Next()).ToList());

        // DisplayList(list);
    }



    //generate targets and put them into gameObjects list
    void generateTargets()
    {
        for (int i = 0; i < numberOfAngles; i++)
        {
            for (int j = 0; j < distances.Count; j++)
            {
                //adapt vector3 coordinates
                double angle = ConvertDegreesToRadians(i * (360 / numberOfAngles));
                float x = distances[j];
                float z = distances[j] * Mathf.Cos(i);
                float y = distances[j] * Mathf.Sin(i);

                //create the object, and set it inactive
                GameObject tmp = Instantiate(shape, new Vector3(x, y, z), Quaternion.identity);
                tmp.SetActive(false);

                //giving object a name, and adding it to the list of objects
                int name = i * distances.Count + j;
                tmp.name = name.ToString();
                gameObjects.Add(tmp);
            }
        }

    }

    void DisplayList(List<GameObject> list)
    {
        foreach (GameObject obj in list)
        {
            Debug.Log(obj.name);
        }
    }
    private double ConvertDegreesToRadians(double degrees)
    {
        double radians = (Math.PI / 180) * degrees;
        return (radians);
    }

    // Remove last target from the scene and make the next one spawn
    void NextTarget()
    {
        if (this.gameObjects.Count > 0)
        {
            this.gameObjects[0].SetActive(false);
            this.gameObjects.RemoveAt(0);
            this.gameObjects[0].SetActive(true);
        }
    }

    bool IsTargetHit()
    {
        RaycastHit hit;
        Ray rayOrigin = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(rayOrigin, out hit))
        {
            return true;
        }
        return false;
    }
    //calculate ID based on the target
    void getTargetId()
    {
    }


}
