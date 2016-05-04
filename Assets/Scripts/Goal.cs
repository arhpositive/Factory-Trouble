using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;

public class Goal : MonoBehaviour
{
    private LevelManager _levelManagerScript;

    public List<GameObject> BallsInContact { get; private set; }
    public int TargetBallCount { get; set; }
    public bool GoalAccomplished;
    public GameObject FinishedPrefab;
    public AudioClip SuccessAudioClip;

	// Use this for initialization
	void Start ()
	{
	    _levelManagerScript = Camera.main.GetComponent<LevelManager>();
        _levelManagerScript.GoalGameObjects.Add(gameObject);
        _levelManagerScript.SetLevelGameObjectGridMember(gameObject, Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z));
	    if (GoalAccomplished)
	    {
	        AudioSource.PlayClipAtPoint(SuccessAudioClip, transform.position);
	    }
	    StartCoroutine(CheckForWin());
        BallsInContact = new List<GameObject>();
	}
	
	// Update is called once per frame
	void Update () {
	    if (!GoalAccomplished && BallsInContact.Count >= TargetBallCount)
	    {
            //we have enough balls in the goal zone to test success
	        foreach (GameObject go in BallsInContact)
	        {
	            int connectedBallCount = 1;
                Ball ballScript = go.GetComponent<Ball>();
	            foreach (GameObject bgo in ballScript.BallsInContact)
	            {
                    //check if all these balls also connect to our goal tile
	                if (BallsInContact.Contains(bgo))
	                {
	                    ++connectedBallCount;
	                }
	            }
	            if (connectedBallCount >= TargetBallCount)
	            {
                    //succeeded
	                foreach (GameObject bgo in ballScript.BallsInContact)
	                {
                        Assert.IsTrue(bgo);
                        bgo.GetComponent<Ball>().DestroyOnGoalSuccess();
                    }
                    ballScript.DestroyOnGoalSuccess();
                    GoalAccomplished = true;

	                Instantiate(FinishedPrefab, transform.position, Quaternion.identity);
	                _levelManagerScript.GoalGameObjects.Remove(gameObject);
                    Destroy(gameObject);

                    break;
	            }
            }
        }
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "ball")
        {
            BallsInContact.Add(other.gameObject);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "ball")
        {
            BallsInContact.Remove(other.gameObject);
        }
    }

    private IEnumerator CheckForWin()
    {
        yield return new WaitForSeconds(0.5f);
        _levelManagerScript.CheckForLevelSuccessTrigger();
    }
}
