using UnityEngine;
using System.Collections;

public class Robot : MonoBehaviour
{
    public float MoveSpeed;
    public AudioClip WalkAudioClip;
    public float RotationSpeed { get; private set; }
    public bool IsOnTheMove { get; private set; }
    public int[] RobotPosition { get; private set; }

    private Vector3 _targetSquare;
    private Vector3 _targetRotation;
    private LevelManager _levelManagerScript;

    // Use this for initialization
    void Start ()
    {
        IsOnTheMove = false;
        _levelManagerScript = Camera.main.GetComponent<LevelManager>();
        RobotPosition = new [] {Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z)};
        RotationSpeed = MoveSpeed*3.5f;
    }
	
	// Update is called once per frame
	void Update ()
	{
	    if (!IsOnTheMove)
	    {
	        if (Application.platform == RuntimePlatform.Android)
	        {
                //no keyboard input on android
	            return;
	        }
	        float horizontalInput = Input.GetAxisRaw("Horizontal");
	        float verticalInput = Input.GetAxisRaw("Vertical");
	        if (verticalInput == 0.0f)
	        {
	            if (horizontalInput == 1.0f)
	            {
	                TryToMove(RobotPosition[0] + 1, RobotPosition[1]);
	            }
	            else if (horizontalInput == -1.0f)
	            {
	                TryToMove(RobotPosition[0] - 1, RobotPosition[1]);
	            }
	        }
	        if (horizontalInput == 0.0f)
	        {
	            if (verticalInput == 1.0f)
	            {
	                TryToMove(RobotPosition[0], RobotPosition[1] + 1);
	            }
	            else if (verticalInput == -1.0f)
	            {
	                TryToMove(RobotPosition[0], RobotPosition[1] - 1);
	            }
	        }
	    }
	    else
	    {
            float step = MoveSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, _targetSquare, step);

	        float rotateStep = RotationSpeed*Time.deltaTime;
	        Vector3 newDir = Vector3.RotateTowards(transform.forward, _targetRotation, rotateStep, 0.0f);
            transform.rotation = Quaternion.LookRotation(newDir);

            if (transform.position == _targetSquare)
            {
                RobotPosition[0] = Mathf.RoundToInt(_targetSquare.x);
                RobotPosition[1] = Mathf.RoundToInt(_targetSquare.z);
                IsOnTheMove = false;
            }
        }
    }

    public void GiveMoveCommand(int targetDirX, int targetDirZ)
    {
        if (!IsOnTheMove)
        {
            TryToMove(RobotPosition[0] + targetDirX, RobotPosition[1] + targetDirZ);
        }
    }

    private void TryToMove(int moveTargetX, int moveTargetZ)
    {
        if (Time.timeScale != 0.0f && _levelManagerScript.CheckForMovement(gameObject, moveTargetX, moveTargetZ))
        {
            _levelManagerScript.SendMovementRecord(gameObject, moveTargetX, moveTargetZ);
            _targetSquare = new Vector3(moveTargetX, 0, moveTargetZ);
            _targetRotation = _targetSquare - new Vector3(RobotPosition[0], 0, RobotPosition[1]);
            IsOnTheMove = true;
            AudioSource.PlayClipAtPoint(WalkAudioClip, transform.position);
        }
    }
}
