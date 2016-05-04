using UnityEngine;
using System.Collections;

public enum FactoryType
{
    ft_rotator,
    ft_accelerator,
    ft_decelerator
}

public class Factory : MonoBehaviour
{
    public GameObject EntrySpherePrefab;
    public FactoryType TypeOfFactory;
    public bool IsOnTheMove { get; private set; }
    public Vector3[] EntrySides;
    public float AcceleratorCoef;
    public float DeceleratorCoef;
    public float MoveSpeed;
    public int[] FactoryPosition { get; private set; }

    public AudioClip[] DragAudioClips;

    private Vector3 _targetSquare;
    private int _assignedBallCount;

    private LevelManager _levelManagerScript;

    // Use this for initialization
    void Start ()
	{
        _levelManagerScript = Camera.main.GetComponent<LevelManager>();
        IsOnTheMove = false;
        FactoryPosition = new[] { Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z) };
        GameObject go = Instantiate(EntrySpherePrefab, EntrySides[0], Quaternion.identity) as GameObject;
        go.transform.SetParent(gameObject.transform, false);
	    EntrySides[0] = go.transform.position - transform.position;
        go = Instantiate(EntrySpherePrefab, EntrySides[1], Quaternion.identity) as GameObject;
        go.transform.SetParent(gameObject.transform, false);
        EntrySides[1] = go.transform.position - transform.position;
        _assignedBallCount = 0;
	}

    void Update()
    {
        if (IsOnTheMove)
        {
            float step = MoveSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, _targetSquare, step);

            if (transform.position == _targetSquare)
            {
                FactoryPosition[0] = Mathf.RoundToInt(_targetSquare.x);
                FactoryPosition[1] = Mathf.RoundToInt(_targetSquare.z);
                IsOnTheMove = false;
            }
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (!IsOnTheMove && other.gameObject.tag == "ball")
        {
            Vector3 inputDirection = (other.gameObject.transform.position - transform.position).normalized;
            if (inputDirection == EntrySides[0] || inputDirection == EntrySides[1])
            {
                //factory will set ball as belonging to himself, then, ball will move according to factory guidelines
                other.gameObject.GetComponent<Ball>().AssignFactory(gameObject);
                IncreaseBallCount();
            }
            else
            {
                Destroy(other.gameObject);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!IsOnTheMove && other.gameObject.tag == "ball")
        {
            Vector3 inputDirection = (other.gameObject.transform.position - transform.position).normalized;
            if (inputDirection == EntrySides[0] || inputDirection == EntrySides[1])
            {
                other.gameObject.GetComponent<Ball>().RemoveFactory(gameObject);
                ReduceBallCount();
            }
            else
            {
                Destroy(other.gameObject);
            }   
        }
    }

    public bool TryToMove(int moveTargetX, int moveTargetZ)
    {
        if (IsOnTheMove || _assignedBallCount > 0 ||  !_levelManagerScript.CheckForMovement(gameObject, moveTargetX, moveTargetZ))
        {
            //you can't move a working factory and you can't move a factory which has something on the other side of it
            return false;
        }
        _levelManagerScript.SendMovementRecord(gameObject, moveTargetX, moveTargetZ);
        _targetSquare = new Vector3(moveTargetX, 0, moveTargetZ);
        IsOnTheMove = true;
        AudioSource.PlayClipAtPoint(DragAudioClips[Random.Range(0, DragAudioClips.Length)], transform.position);
        return true;
    }

    public void IncreaseBallCount()
    {
        ++_assignedBallCount;
    }

    public void ReduceBallCount()
    {
        --_assignedBallCount;
    }
}
