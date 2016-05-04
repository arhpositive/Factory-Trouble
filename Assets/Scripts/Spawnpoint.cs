using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;

public class Spawnpoint : MonoBehaviour
{
    public GameObject BallPrefab;

    public int BallSpawnInterval { get; set; }
    private int _lastBallSpawnUpdateNum;
    private int _fixedUpdateCount;

    private LevelManager _levelManagerScript;

    // Use this for initialization
    void Start ()
    {
        _levelManagerScript = Camera.main.GetComponent<LevelManager>();
        _lastBallSpawnUpdateNum = -BallSpawnInterval * 2;
        _fixedUpdateCount = 0;
    }
    
	void FixedUpdate () {
	    if (_fixedUpdateCount - _lastBallSpawnUpdateNum >= BallSpawnInterval)
	    {
	        SpawnNewBall();
	    }
	    ++_fixedUpdateCount;
	}

    private void SpawnNewBall()
    {
        GameObject ball = Instantiate(BallPrefab, transform.position, Quaternion.identity) as GameObject;
        Assert.IsNotNull(ball);
        _levelManagerScript.AddObjectToLevelObjectList(ball);
        Ball ballScript = ball.GetComponent<Ball>();

        ballScript.BallDirection = -transform.forward;
        ballScript.OriginPosition = transform.position;

        _lastBallSpawnUpdateNum = _fixedUpdateCount;
    }
}
