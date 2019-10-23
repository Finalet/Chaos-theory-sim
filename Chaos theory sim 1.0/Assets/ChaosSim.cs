using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChaosSim : MonoBehaviour
{
    public static ChaosSim instance;

    public float simSpeed;

    public List<Vector2> shapePointsList;
    public List<Vector2> simPointsList;
    public Mode SimMode;
    public bool canCreatePoints = true;

    [Header("Points")]
    public GameObject shapePoint;
    public GameObject simPoint;

    [Header("Folders")]
    public Transform shapePointsFolder;
    public Transform simPointsFolder;


    [Header("Buttons")]
    public GameObject simulationButton;
    public GameObject firstPointButton;
    public GameObject createShapePointButton;
    public GameObject deleteAllPointsButton;
    public Slider speedSlider;

    [Space]
    public GameObject stats;


    IEnumerator sim;
    IEnumerator sim2;
    IEnumerator sim3;
    GameObject startingPoint;
    float startTime;
    public float runTime;

    void Start()
    {
        instance = this;
    }

    void Update()
    {
        if (SimMode == Mode.ShapeCreation)
        {
            if (Input.GetMouseButtonDown(0) && canCreatePoints)
            {
                AddPointsToShape(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
            }

            if (Input.GetKeyDown(KeyCode.Escape))
                SetIdleMode();
        } else if (SimMode == Mode.FirstPointCreation)
        {
            if (Input.GetMouseButtonDown(0) && canCreatePoints)
            {
                CreateStartingPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
            }

            if (Input.GetKeyDown(KeyCode.Escape))
                SetIdleMode();
        } else if (SimMode == Mode.Simulation)
        {
            createShapePointButton.SetActive(false);
            firstPointButton.SetActive(false);
            deleteAllPointsButton.SetActive(false);
            speedSlider.interactable = false;

        } else if (SimMode == Mode.Idle)
        {
            createShapePointButton.SetActive(true);
            deleteAllPointsButton.SetActive(true);
            speedSlider.interactable = true;
        }

        if (shapePointsList.Count >= 2 && SimMode != Mode.ShapeCreation)
        {
            simulationButton.SetActive(true);
        }
        else
        {
            simulationButton.SetActive(false);
        }

        simSpeed = speedSlider.value * 100;
        Statistics();
    }

    public void AddPointsToShape(float x, float y)
    {
        shapePointsList.Add(new Vector2 (x, y));
        Instantiate(shapePoint, new Vector2(x, y), Quaternion.identity, shapePointsFolder);
    }

    bool firstPointCreated;
    public void CreateStartingPoint(float x, float y)
    {
        if (firstPointCreated)
            return;

        simPointsList.Add(new Vector2(x, y));
        startingPoint = Instantiate(simPoint, new Vector2(x, y), Quaternion.identity, simPointsFolder);
        firstPointCreated = true;
        firstPointButton.SetActive(false);

        if (SimMode != Mode.Simulation) SetIdleMode();
    }


    void GenerateNewPoint (int prevPointIndex)
    {

        int numberOfShapePoints = shapePointsList.Count;
        int shapePointIndex = Random.Range(0, numberOfShapePoints);

        Vector2 prevPoint = simPointsList[prevPointIndex];
        Vector2 anglePoint = shapePointsList[shapePointIndex];

        Vector2 newPoint = Vector2.Lerp(prevPoint, anglePoint, 0.5f);
        simPointsList.Add (newPoint);

        Instantiate(simPoint, newPoint, Quaternion.identity, simPointsFolder);
    }
    IEnumerator Simulation ()
    {
        startTime = Time.time;
        int i = 0;
        while (SimMode == Mode.Simulation)
        {
            if (!firstPointCreated)
            {
                CreateStartingPoint(0, 0);
            }
            else
            {
                GenerateNewPoint(i);
                i++;
            }
            yield return new WaitForSeconds(1 / simSpeed);
        }
    }

    public void EraseShape (bool generatedPointsOnly)
    {
        if (!generatedPointsOnly)
        {
            foreach (Transform child in shapePointsFolder)
            {
                Destroy(child.gameObject);
            }
            shapePointsList.Clear();
        }
        simPointsList.Clear();
        foreach (Transform child in simPointsFolder)
        {
            Destroy(child.gameObject);
        }

        Destroy(startingPoint);
        firstPointButton.SetActive(true);
        firstPointCreated = false;
    }

    public void SetShapeCreationMode ()
    {
        if (SimMode == Mode.Idle)
        {
            SimMode = Mode.ShapeCreation;
            createShapePointButton.transform.GetChild(0).GetComponent<Text>().text = "DONE";
        }
        else
        {
            SimMode = Mode.Idle;
            createShapePointButton.transform.GetChild(0).GetComponent<Text>().text = "Create shape";
        }
    }
    public void SetStartingPointCreationMode()
    {
        if (!firstPointCreated)
        {
            SimMode = Mode.FirstPointCreation;
            createShapePointButton.transform.GetChild(0).GetComponent<Text>().text = "Create shape";
        }
    }
    public void SetIdleMode()
    {
        SimMode = Mode.Idle;
    }
    public void RunStopSimulation()
    {
        if (SimMode == Mode.Idle)
        {
            SimMode = Mode.Simulation;
            sim = Simulation();
            StartCoroutine(sim);
            simulationButton.transform.GetChild(0).GetComponent<Text>().text = "STOP";

            if(simSpeed >= 100)
            {
                sim2 = Simulation();
                StartCoroutine(sim2);
            }
            if (simSpeed >= 200)
            {
                sim3 = Simulation();
                StartCoroutine(sim3);
            }
        } else if (SimMode == Mode.Simulation)
        {
            SimMode = Mode.Idle;
            StopAllCoroutines();
            simulationButton.transform.GetChild(0).GetComponent<Text>().text = "Run simulation";
        }
    }

    public void MouseOver ()
    {
        canCreatePoints = false;
    }
    public void MouseExit()
    {
        canCreatePoints = true;
    }

    void Statistics()
    {
        if (SimMode == Mode.Simulation)
            runTime = Mathf.RoundToInt(Time.time - startTime);
        else
            runTime = 0;

        stats.GetComponent<Text>().text = "Speed: " + Mathf.Round(simPointsList.Count/runTime) + "/sec \n" + "Runtime in sec: " + runTime + "\n" + "Number of points: " + simPointsList.Count + "\n";
    }
}


public enum Mode
{
    Idle,
    ShapeCreation,
    FirstPointCreation,
    Simulation
};