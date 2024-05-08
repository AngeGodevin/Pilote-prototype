using System;

using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
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
    public int numberOfSize;


    private float currentTime;
    private RecordData recordData;
    private List<UserData> userData;
    private System.Random rng = new System.Random();


    // Start is called before the first frame update
    void Start()
    {
        // state = State.IDLE;
        state = State.EXP_ON_GOING;
        gameObjects = new List<GameObject>();
        this.userData = new List<UserData>();
        generateTargets();
        Shuffle(this.gameObjects);
        this.currentTime = 0;
        this.gameObjects[0].SetActive(true);
        this.recordData = new RecordData(duration, numberOfAngles, distances);

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
                    this.userData.Add(this.isTargetHit());
                    NextTarget();
                }
                if (!isTimerRunning())
                {
                    this.state = State.IDLE;
                    //register data
                    this.registerData();
                }
                break;
        }
    }

    private bool isTimerRunning()
    {
        if (currentTime < duration)
        {
            currentTime += Time.deltaTime;
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
                for (int k = 0; k < numberOfSize; k++)
                {
                    //adapt vector3 coordinates
                    double angle = ConvertDegreesToRadians(i * (360 / numberOfAngles));
                    float x = distances[j];
                    float z = distances[j] * Mathf.Cos(i);
                    float y = distances[j] * Mathf.Sin(i);

                    //create the object, and set it inactive
                    GameObject tmp = Instantiate(shape, new Vector3(x, y, z), Quaternion.identity);
                    tmp.SetActive(false);

                    //Scaling the object to its size
                    tmp.transform.localScale += new Vector3(k,k,k);

                    //giving object a name, and adding it to the list of objects
                    int name = i * distances.Count + j + k;
                    tmp.name = name.ToString();
                    gameObjects.Add(tmp);
                }
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

    UserData isTargetHit()
    {
        RaycastHit hit;
        bool isTargetHit;
        Vector3 hitCoord = Vector3.zero;
        Ray rayOrigin = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(rayOrigin, out hit))
        {
            hitCoord = hit.transform.position;
            isTargetHit = true;
        }else
        {
            isTargetHit = false;
        }
        return new UserData { targetId= Int32.Parse(this.gameObjects[0].name) , isTargetHit = isTargetHit, hitCoords= hitCoord  , targetCoords= this.gameObjects[0].transform.position, time=this.currentTime};
    }


    //register data to Csv
    void registerData()
    {
        Debug.Log("writing");

        //writing the parameters of the experience 
        TextWriter csv = new StreamWriter(Application.dataPath + "/test.csv", false);
        csv.WriteLine("Duration, Number of angles");
        csv.WriteLine(recordData.duration.ToString() + "," + recordData.numberOfAngles.ToString());
        csv.WriteLine("");
        csv.WriteLine("Distances");
        for (int i = 0; i < recordData.listOfDistances.Count; i++)
        {
            csv.WriteLine(recordData.listOfDistances[i]);
        }

        csv.WriteLine("");
        csv.WriteLine("");
        csv.WriteLine("");

        //writing the user data
        csv.WriteLine("TargetId, IsTargetHit?, HitCoordsX, HitCoordsY, HitCoordsZ, TargetCoordsX, TargetCoordsY, TargetCoordsZ, Time");
        for (int i = 0; i < userData.Count; i++)
        {
            csv.WriteLine(userData[i].targetId.ToString() + "," + userData[i].isTargetHit.ToString() + "," + userData[i].hitCoords.x.ToString() + "," + userData[i].hitCoords.y.ToString() + "," + userData[i].hitCoords.z.ToString() + "," + userData[i].targetCoords.x.ToString() + "," + userData[i].targetCoords.y.ToString() + "," + userData[i].targetCoords.z.ToString() + ","+ userData[i].time.ToString());
        }


        csv.Close();

        Debug.Log("end of writing");
    }


}
public class UserData
{
    public int targetId { get; set; }
    public bool isTargetHit { get; set; }
    public Vector3 hitCoords { get; set; }
    public Vector3 targetCoords { get; set; }
    public float time { get; set; }
}
public class RecordData
{
    public int duration { get; set; }
    public int numberOfAngles { get; set; }
    public List<int> listOfDistances { get; set; }

    public RecordData(int duration, int numberOfAngles, List<int> listOfDistances)
    {
        this.duration = duration;
        this.numberOfAngles = numberOfAngles;
        this.listOfDistances = listOfDistances;
    }
}
