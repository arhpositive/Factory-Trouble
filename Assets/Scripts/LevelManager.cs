using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Assertions;

public class LevelDesign
{
    public int[,] LevelLayout;
    public int CameraSize;
    public Vector3 CameraPos;

    public LevelDesign(int[,] levelLayout, int cameraSize, Vector3 cameraPos)
    {
        LevelLayout = levelLayout;
        CameraSize = cameraSize;
        CameraPos = cameraPos;
    }
}

public class LevelManager : MonoBehaviour
{
    [Header("Tile Prefabs")]
    public GameObject SpawnpointPrefab;
    public GameObject TransparentSpawnpointPrefab;
    public GameObject WallPrefab;
    public GameObject TransparentWallPrefab;
    public GameObject CornerPrefab;
    public GameObject TransparentCornerPrefab;
    public GameObject FloorPrefab;
    public GameObject GoalPrefab;
    public GameObject Goal2Prefab;
    public GameObject Goal3Prefab;
    public GameObject RobotPrefab;

    [Header("Factory Prefabs")]
    public GameObject FactoryRotPrefab;
    public GameObject FactoryAccPrefab;
    public GameObject FactoryDecPrefab;

    public AudioClip NewLevelAudioClip;
    public GameObject EndPanelGameObject;

    public int[,] CurrentLevel { get; private set; }
    public bool MissionAccomplished { get; private set; }

    private List<LevelDesign> _levelDesigns;
    private int _curLevelIndex; 
    public List<GameObject> GoalGameObjects { get; set; }
    private GameObject[,] _levelGameObjectGrid;
    private List<GameObject> _otherGameObjectsList;
    private List<GameObject> _outsideLevelObjectsList;
    private GameObject _robotGameObject;
    

    // Use this for initialization
	private void Start ()
	{
        GoalGameObjects = new List<GameObject>();
        _otherGameObjectsList = new List<GameObject>();
        _outsideLevelObjectsList = new List<GameObject>();
        _levelGameObjectGrid = new GameObject[1,1];
        _levelDesigns = new List<LevelDesign>();
	    
	    _curLevelIndex = 0;
        CreateLevelDesigns();
        StartNextLevel();
	}

    private void Update()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }
        }
        else if (Time.timeScale != 0.0f)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                StartNextLevel();
            }
            else if (Input.GetKeyDown(KeyCode.Escape))
            {
                StartFromBeginning();
            }
        }
    }

    public void StartNextLevel()
    {
        MissionAccomplished = false;
        CleanLevel();
        AudioSource.PlayClipAtPoint(NewLevelAudioClip, transform.position);
        SetCurrentLevel(_levelDesigns[_curLevelIndex]);
        GenerateOutsideLevel();
    }

    public void StartFromBeginning()
    {
        _curLevelIndex = 0;
        StartNextLevel();
    }

    private void EndGame()
    {
        EndPanelGameObject.GetComponent<EndMenu>().OpenEndMenu();
    }

    private void CreateLevelDesigns()
    {
        //Here lies at least 5 level designs for proof of concept.

        //Level 1
        //Introduction to Rotators
        int[,] level1 =
        {
            {-8,-7,-7,-7,-7,-7,-7,-7,-8 },
            {-7, 1, 1, 1, 1, 1, 1, 1,-7 },
            {-7, 1, 1, 1, 1, 1, 1, 1,-7 },
            {-7, 1,13, 1, 1, 1, 1, 1,-4 },
            {-7, 1, 1, 1, 1, 2, 1, 1,-7 },
            {-7, 1, 1, 6, 1, 1, 1, 1,-7 },
            {-7, 1, 1, 1, 1, 1, 1, 1,-7 },
            {-7, 1, 1, 1, 1, 1, 1, 1,-7 },
            {-8,-7,-7,-7,-7,-4,-7,-7,-8 }
        };
        AddLevelDesign(level1, 4);

        //Level 2
        //Introduction to Accelerators
        int[,] level2 =
        {
            {-8,-7,-7,-7,-7,-7,-7,-7,-8 },
            {-7, 1, 1, 1, 1, 1, 1, 1,-7 },
            {-7, 1, 1, 1, 1, 1, 1, 1,-7 },
            {-7, 1, 2, 1, 1, 1, 1, 1,-4 },
            {-7, 1, 1, 1, 1,13, 1, 1,-7 },
            {-7, 1, 1, 9, 1, 1, 1, 1,-7 },
            {-7, 1, 1, 1, 1, 1, 1, 1,-7 },
            {-7, 1, 1, 1, 1, 1, 1, 1,-7 },
            {-8,-7,-4,-7,-7,-7,-7,-7,-8 }
        };
        AddLevelDesign(level2, 4);

        //Level 3
        //Combining Decelerators and a Rotator
        int[,] level3 = {
            {-8,-7,-7,-7,-7,-7,-7,-7,-8 },
            {-7, 1, 1, 1, 1, 1, 1, 1,-7 },
            {-7, 1, 1, 0, 1, 1, 1, 1,-7 },
            {-7, 1, 1, 0, 1, 1,11, 1,-7 },
            {-7, 1, 2, 1, 1, 1, 1, 1,-4 },
            {-7, 1, 0, 0, 1, 1, 1, 1,-7 },
            {-7, 1, 1, 0, 0, 0, 1, 1,-7 },
            {-7, 1, 1, 1, 1, 1, 1, 1,-4 },
            {-7, 1, 1, 5, 1, 1,13, 1,-7 },
            {-7, 1, 1, 1, 1, 1, 1, 1,-7 },
            {-8,-7,-7,-7,-7,-7,-7,-7,-8 }
        };
        AddLevelDesign(level3, 5);

        //Level 4
        //Three balls!
        int[,] level4 = {
            {-8,-7,-7,-7,-7,-7,-7,-7,-7,-8 },
            {-7, 1, 1, 1, 1, 1, 1, 1, 1,-7 },
            {-7, 1, 1, 1, 1, 1, 0, 0, 1,-7 },
            {-5, 1, 1, 1, 1, 1, 1, 1, 1,-7 },
            {-5, 1, 1, 1, 3, 1, 1, 1, 1,-7 },
            {-7, 1,13, 1, 1, 1, 1, 1, 1,-7 },
            {-7, 1, 1, 1, 1,11, 7, 1, 1,-7 },
            {-7, 1, 1, 1, 1, 1, 1, 0, 1,-7 },
            {-7, 1, 1, 1, 1, 1, 1, 0, 1,-7 },
            {-8,-7,-7,-7,-5,-7,-7,-7,-7,-8 }
        };
        AddLevelDesign(level4, 4);

        //Level 5
        //Two goals!
        int[,] level5 = {
            {-8,-7,-7,-7,-7,-7,-7,-7,-7,-7,-7,-8 },
            {-7, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,-7 },
            {-7, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,-7 },
            {-7, 1, 1, 1, 1, 1, 1, 8, 1, 1, 1,-6 },
            {-7, 1, 1, 2, 0, 1, 1, 1, 1, 1, 1,-7 },
            {-7, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1,-7 },
            {-7, 1, 6, 1, 0, 2, 1, 1, 1, 1, 1,-6 },
            {-7, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,-7 },
            {-7, 1,13, 1, 1, 9, 1, 1, 1, 1, 1,-7 },
            {-7, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,-7 },
            {-8,-7,-7,-6,-7,-7,-7,-7,-7,-7,-7,-8 }
        };
        AddLevelDesign(level5, 5);

        //Level 8
        //Catch it!
        int[,] level8 = {
            {-8,-7,-7,-7,-7,-7,-7,-7,-7,-8 },
            {-7, 1, 1, 1, 1, 1, 1, 1, 1,-7 },
            {-7, 1, 2, 1, 0, 0, 1, 1, 1,-7 },
            {-7, 1, 1, 1, 1,13, 1, 1, 1,-7 },
            {-7, 1, 1, 5, 1, 1, 1, 1, 1,-7 },
            {-7, 1, 1, 1, 9, 1, 1, 1, 1,-7 },
            {-7, 1, 1, 1, 1, 1, 1, 1, 1,-2 },
            {-7, 1, 1, 1, 1, 1, 1, 1, 1,-7 },
            {-7, 1, 1, 1, 1, 1, 1, 1, 1,-7 },
            {-7, 1, 1, 1, 1, 0, 0, 1, 1,-7 },
            {-8,-7,-7,-7,-7,-7,-7,-7,-7,-8 }
        };
        AddLevelDesign(level8, 5);
    }

    private void AddLevelDesign(int[,] levelDesign, int cameraSize)
    {
        float xPos = (float)levelDesign.GetLength(1) / 2;
        float zPos = (float)levelDesign.GetLength(0) / 2;
        if (Application.platform == RuntimePlatform.Android)
        {
            xPos += .5f;
            zPos -= .5f;
        }
        _levelDesigns.Add(new LevelDesign(levelDesign, cameraSize, new Vector3(xPos, 0, zPos)));
    }

    private void CleanLevel()
    {
        //clean old level
        foreach (GameObject go in _levelGameObjectGrid)
        {
            Destroy(go);
        }
        foreach (GameObject go in _otherGameObjectsList)
        {
            Destroy(go);
        }
        GoalGameObjects.Clear();
        _otherGameObjectsList.Clear();
        foreach (GameObject go in _outsideLevelObjectsList)
        {
            Destroy(go);
        }
        _outsideLevelObjectsList.Clear();
    }

    private void SetCurrentLevel(LevelDesign levelDesign)
    {
        CurrentLevel = levelDesign.LevelLayout;
        _levelGameObjectGrid = new GameObject[CurrentLevel.GetLength(1),CurrentLevel.GetLength(0)];
        Camera.main.orthographicSize = levelDesign.CameraSize;
        GameObject.FindGameObjectWithTag("camTarget").transform.position = levelDesign.CameraPos;
        MissionAccomplished = false;

        int iDim = CurrentLevel.GetLength(0);
        for (int i = 0; i < iDim; ++i)
        {
            int jDim = CurrentLevel.GetLength(1);
            for (int j = 0; j < jDim; ++j)
            {
                GameObject go = new GameObject();
                int[] coordinates = {j, iDim - (i + 1)};
                Vector3 coordsVec = new Vector3(coordinates[0], 0, coordinates[1]);
                bool isBottomCoords = coordinates[0] == 0 || coordinates[1] == 0;
                bool setObjectToGrid = false;
                switch (CurrentLevel[i, j])
                {
                    case -8:
                        if (coordinates[0] == 0 && coordinates[1] == 0)
                        {
                            go = Instantiate(TransparentCornerPrefab, coordsVec, Quaternion.identity) as GameObject;
                        }
                        else
                        {
                            go = Instantiate(CornerPrefab, coordsVec, Quaternion.identity) as GameObject;
                        }
                        setObjectToGrid = true;
                        break;
                    case -7:
                        if (isBottomCoords)
                        {
                            go = Instantiate(TransparentWallPrefab, coordsVec, Quaternion.identity) as GameObject;
                        }
                        else
                        {
                            go = Instantiate(WallPrefab, coordsVec, Quaternion.identity) as GameObject;
                        }
                        RotateWallAccordingToPos(go, coordinates[0], coordinates[1]);
                        go.transform.Translate(0.25f * Vector3.back);
                        setObjectToGrid = true;
                        break;
                    case -6:
                    case -5:
                    case -4:
                    case -3:
                    case -2:
                        if (isBottomCoords)
                        {
                            go = Instantiate(TransparentSpawnpointPrefab, coordsVec, Quaternion.identity) as GameObject;
                        }
                        else
                        {
                            go = Instantiate(SpawnpointPrefab, coordsVec, Quaternion.identity) as GameObject;
                        }
                        go.GetComponent<Spawnpoint>().BallSpawnInterval = (-50) * CurrentLevel[i, j];
                        RotateWallAccordingToPos(go, coordinates[0], coordinates[1]);
                        go.transform.Translate(0.25f * Vector3.back);
                        setObjectToGrid = true;
                        break;
                    case 0:
                        //0 means empty space
                        break;
                    case 1:
                        //nothing but floor
                        break;
                    case 2:
                        go = Instantiate(Goal2Prefab, coordsVec, Quaternion.identity) as GameObject;
                        go.GetComponent<Goal>().TargetBallCount = 2;
                        break;
                    case 3:
                        go = Instantiate(Goal3Prefab, coordsVec, Quaternion.identity) as GameObject;
                        go.GetComponent<Goal>().TargetBallCount = 3;
                        break;
                    case 5:
                        go = Instantiate(FactoryRotPrefab, coordsVec, Quaternion.identity) as GameObject;
                        setObjectToGrid = true;
                        break;
                    case 6:
                        go = Instantiate(FactoryRotPrefab, coordsVec, Quaternion.identity) as GameObject;
                        go.transform.Rotate(Vector3.up, 90);
                        setObjectToGrid = true;
                        break;
                    case 7:
                        go = Instantiate(FactoryRotPrefab, coordsVec, Quaternion.identity) as GameObject;
                        go.transform.Rotate(Vector3.up, 180);
                        setObjectToGrid = true;
                        break;
                    case 8:
                        go = Instantiate(FactoryRotPrefab, coordsVec, Quaternion.identity) as GameObject;
                        go.transform.Rotate(Vector3.up, 270);
                        setObjectToGrid = true;
                        break;
                    case 9:
                        //accelerator
                        go = Instantiate(FactoryAccPrefab, coordsVec, Quaternion.identity) as GameObject;
                        setObjectToGrid = true;
                        break;
                    case 10:
                        //accelerator 90
                        go = Instantiate(FactoryAccPrefab, coordsVec, Quaternion.identity) as GameObject;
                        go.transform.Rotate(Vector3.up, 90);
                        setObjectToGrid = true;
                        break;
                    case 11:
                        //decelerator
                        go = Instantiate(FactoryDecPrefab, coordsVec, Quaternion.identity) as GameObject;
                        setObjectToGrid = true;
                        break;
                    case 12:
                        //decelerator 90
                        go = Instantiate(FactoryDecPrefab, coordsVec, Quaternion.identity) as GameObject;
                        go.transform.Rotate(Vector3.up, 90);
                        setObjectToGrid = true;
                        break;
                    case 13:
                        go = Instantiate(RobotPrefab, coordsVec, Quaternion.identity) as GameObject;
                        setObjectToGrid = true;
                        _robotGameObject = go;
                        break;
                    default:
                        throw new NotImplementedException();
                }
                if (setObjectToGrid)
                {
                    _levelGameObjectGrid[coordinates[0], coordinates[1]] = go;
                }
                
                if (CurrentLevel[i, j] != 0)
                {
                    GameObject floor = Instantiate(FloorPrefab, coordsVec + Vector3.down, Quaternion.identity) as GameObject; //always add floor
                    _otherGameObjectsList.Add(floor);
                }
            }
        }
    }

    private void GenerateOutsideLevel()
    {
        int xSize = CurrentLevel.GetLength(1);
        int zSize = CurrentLevel.GetLength(0);

        for (int i = xSize - 1; i < xSize + 5; ++i)
        {
            for (int j = zSize + 1; j < zSize + 4; ++j)
            {
                Vector3 coords = new Vector3(i, 0, j);
                GameObject floor = Instantiate(FloorPrefab, coords + Vector3.down, Quaternion.identity) as GameObject; //always add floor
                _outsideLevelObjectsList.Add(floor);
            }
        }

        for (int i = xSize + 1; i < xSize + 5; ++i)
        {
            for (int j = 3; j < zSize -2; ++j)
            {
                Vector3 coords = new Vector3(i, 0, j);
                GameObject floor = Instantiate(FloorPrefab, coords + Vector3.down, Quaternion.identity) as GameObject; //always add floor
                _outsideLevelObjectsList.Add(floor);
            }
        }

        for (int i = 1; i < xSize - 4; ++i)
        {
            for (int j = zSize + 1; j < zSize + 7; ++j)
            {
                Vector3 coords = new Vector3(i, 0, j);
                GameObject floor = Instantiate(FloorPrefab, coords + Vector3.down, Quaternion.identity) as GameObject; //always add floor
                _outsideLevelObjectsList.Add(floor);
            }
        }

    }

    private void RotateWallAccordingToPos(GameObject go, int xPos, int zPos)
    {
        if (xPos == 0)
        {
            // 270
            go.transform.Rotate(Vector3.up, 270);
        }
        else if (xPos == CurrentLevel.GetLength(1) - 1)
        {
            // 90
            go.transform.Rotate(Vector3.up, 90);
        }
        else if (zPos == 0)
        {
            //180
            go.transform.Rotate(Vector3.up, 180);
        }
        else
        {
            Assert.IsTrue(zPos == CurrentLevel.GetLength(0) - 1);
        }
    }

    public bool IsSucceeded()
    {
        bool success = true;
        foreach (GameObject go in GoalGameObjects)
        {
            if (!go.GetComponent<Goal>().GoalAccomplished)
            {
                success = false;
                break;
            }
        }
        return success;
    }

    public void CheckForLevelSuccessTrigger()
    {
        if (IsSucceeded())
        {
            MissionAccomplished = true;
            //end level
            StartCoroutine(GoToNextLevel());
        }
    }

    public void AddObjectToLevelObjectList(GameObject go)
    {
        _otherGameObjectsList.Add(go);
    }

    public void MoveRobotWithCommand(int moveTo)
    {
        if (_robotGameObject)
        {
            int targetDirX = 0;
            int targetDirZ = 0;
            if (moveTo == 1)
            {
                targetDirZ = 1;
            }
            else if (moveTo == 2)
            {
                targetDirX = 1;
            }
            else if (moveTo == 3)
            {
                targetDirZ = -1;
            }
            else if (moveTo == 4)
            {
                targetDirX = -1;
            }
            _robotGameObject.GetComponent<Robot>().GiveMoveCommand(targetDirX, targetDirZ);
        }
    }

    private IEnumerator GoToNextLevel()
    {
        yield return new WaitForSeconds(1);
        //load next level here
        if (MissionAccomplished)
        {
            ++_curLevelIndex;
            if (_curLevelIndex == _levelDesigns.Count)
            {
                EndGame();
            }
            else
            {
                StartNextLevel();
            }
        }
    }

    public bool CheckForMovement(GameObject go, int moveTargetX, int moveTargetZ)
    {
        int targetSquareCode =
            CurrentLevel[CurrentLevel.GetLength(0) - (1 + moveTargetZ), moveTargetX];

        GameObject targetGameObject = _levelGameObjectGrid[moveTargetX, moveTargetZ];

        if (targetSquareCode < 1)
        {
            return false;
        }

        if (targetGameObject)
        {
            //only robots can push factories
            if (go.tag == "robot" && targetGameObject.tag == "factory")
            {
                int[] factoryPos = targetGameObject.GetComponent<Factory>().FactoryPosition;
                int[] robotPos = go.GetComponent<Robot>().RobotPosition;
                bool result = targetGameObject.GetComponent<Factory>().TryToMove(
                    moveTargetX + factoryPos[0] - robotPos[0],
                    moveTargetZ + factoryPos[1] - robotPos[1]);
                return result;
            }
            return false;
        }
        return true;
    }

    public void SendMovementRecord(GameObject go, int moveTargetX, int moveTargetZ)
    {
        int[] objectPos;
        if (go.tag == "robot")
        {
            objectPos = go.GetComponent<Robot>().RobotPosition;
        }
        else
        {
            Assert.IsTrue(go.tag == "factory");
            objectPos = go.GetComponent<Factory>().FactoryPosition;
        }
        
        //swap these two squares
        _levelGameObjectGrid[objectPos[0], objectPos[1]] = _levelGameObjectGrid[moveTargetX, moveTargetZ];
        _levelGameObjectGrid[moveTargetX, moveTargetZ] = go;
    }

    public void SetLevelGameObjectGridMember(GameObject go, int x, int z)
    {
        _levelGameObjectGrid[x, z] = go;
    }
}
