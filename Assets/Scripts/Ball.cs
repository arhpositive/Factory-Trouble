using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Assertions;
using System;

public class Ball : MonoBehaviour
{
    public float BallSpeed;
    public Vector3 BallDirection { get; set; }
    public Vector3 OriginPosition { get; set; }
    
    public List<GameObject> BallsInContact { get; private set; }
    public GameObject AssignedFactory { get; private set; }
    public GameObject BackupFactory { get; private set; }

    public GameObject particlePrefab;
    public AudioClip GetDestroyedAudioClip;
    public AudioClip AccelerateAudioClip;
    public AudioClip DecelerateAudioClip;
    public AudioClip RotateAudioClip;

    private bool _factoryUsedUp;
    private bool _destroyOnSuccessTriggered;
    //private float _successTime;
    //private const float DestroyOnSuccessInterval = 1;

	// Use this for initialization
	void Start ()
	{
	    BallsInContact = new List<GameObject>();
	    _destroyOnSuccessTriggered = false;
        AssignedFactory = null;
        _factoryUsedUp = false;
    }
	
	// Update is called once per frame
	void FixedUpdate () {
	    if (!_destroyOnSuccessTriggered)
	    {
            //we expect the ball to move here
	        Vector3 moveAmount = BallDirection * BallSpeed * Time.fixedDeltaTime;
            if (AssignedFactory != null && !_factoryUsedUp)
            {
	            //we're inside a factory
                //determine if moving by move amount would move us over the center of factory
	            float moveOverDistance = moveAmount.magnitude -
	                                     Vector3.Distance(AssignedFactory.transform.position, gameObject.transform.position);

                if (moveOverDistance >= 0.0f)
	            {
                    _factoryUsedUp = true;
                    //we're moving over the point, first set position to be identical as factory position
                    transform.position = AssignedFactory.gameObject.transform.position;

                    //update properties of the ball according to the factory you're in
	                float oldBallSpeed = BallSpeed;
	                UpdatePropertiesInFactory();

	                //move as much as the moveOverDistance with the new properties of the ball
                    transform.Translate(moveOverDistance * (BallSpeed / oldBallSpeed) * BallDirection);
	            }
                else
                {
                    transform.Translate(moveAmount);
                }
	        }
	        else
	        {
                transform.Translate(moveAmount);
            }
        }
	}

    private void UpdatePropertiesInFactory()
    {
        Factory factoryScript = AssignedFactory.GetComponent<Factory>();
        switch (factoryScript.TypeOfFactory)
        {
            case FactoryType.ft_rotator:
                //update direction
                if (BallDirection == -factoryScript.EntrySides[0])
                {
                    BallDirection = factoryScript.EntrySides[1];
                }
                else
                {
                    Assert.IsTrue(BallDirection == -factoryScript.EntrySides[1]);
                    BallDirection = factoryScript.EntrySides[0];
                }
                AudioSource.PlayClipAtPoint(RotateAudioClip, transform.position);
                break;
            case FactoryType.ft_accelerator:
                BallSpeed *= factoryScript.AcceleratorCoef;
                AudioSource.PlayClipAtPoint(AccelerateAudioClip, transform.position);
                //update speed
                break;
            case FactoryType.ft_decelerator:
                BallSpeed *= factoryScript.DeceleratorCoef;
                AudioSource.PlayClipAtPoint(DecelerateAudioClip, transform.position);
                //update speed
                break;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "wall" || other.gameObject.tag == "robot" ||
            (other.gameObject.tag == "spawnpoint" && other.gameObject.transform.position != OriginPosition))
        {
            //collided with a wall or another spawnpoint, destroy object
            if (AssignedFactory)
            {
                AssignedFactory.GetComponent<Factory>().ReduceBallCount();
            }
            AudioSource.PlayClipAtPoint(GetDestroyedAudioClip, transform.position);
            Destroy(gameObject);
        }

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

    public void DestroyOnGoalSuccess()
    {
        if (AssignedFactory)
        {
            AssignedFactory.GetComponent<Factory>().ReduceBallCount();
        }
        Instantiate(particlePrefab, transform.position, Quaternion.LookRotation(Vector3.up));
        Destroy(gameObject);
        //_destroyOnSuccessTriggered = true;
        //_successTime = Time.time;
        //transform.GetComponent<Rigidbody>().isKinematic = false;
        //transform.GetComponent<Rigidbody>().useGravity = true;
    }

    public void AssignFactory(GameObject factoryGameObject)
    {
        if (AssignedFactory)
        {
            BackupFactory = factoryGameObject;
        }
        else
        {
            AssignedFactory = factoryGameObject;
        }
    }

    public void RemoveFactory(GameObject factoryGameObject)
    {
        if (factoryGameObject == AssignedFactory)
        {
            if (BackupFactory)
            {
                AssignedFactory = BackupFactory;
            }
            else
            {
                AssignedFactory = null;
            }
            _factoryUsedUp = false;
        }
    }
}
