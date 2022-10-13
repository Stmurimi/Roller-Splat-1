using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    public Rigidbody rb;
    public float speed = 15;

    public int minSwipeRecognition = 500;

    private bool isTraveling;
    private Vector3 travelDirection;

    //To determine distance to swipe on screen to acknowledge as a swipe
    private Vector2 swipePosLastFrame;
    private Vector2 swipePosCurrentFrame;
    private Vector2 currentSwipe;

    private Vector3 nextCollisionPosition;

    private Color solveColor;
    public ParticleSystem dirtParticle;
    public AudioClip rollSound;
    private AudioSource audioSource;

    private void Start()
    {
        //Create random color
        solveColor = Random.ColorHSV(0.5f, 1); 
        //Set the random color to the ball
        GetComponent<MeshRenderer>().material.color = solveColor;
        audioSource = GetComponent<AudioSource>();
        
    }

    private void FixedUpdate()
    {
        // Set the balls speed when it should travel
        if (isTraveling)
        {
            rb.velocity = travelDirection * speed;
        }

        // When the overlap sphere touches the ground, paint it
        Collider[] hitColliders = Physics.OverlapSphere(transform.position - (Vector3.up / 2), .05f);
        int i = 0;
        while (i < hitColliders.Length)
        {
            GroundPiece ground = hitColliders[i].transform.GetComponent<GroundPiece>();

            if (ground && !ground.isColored)
            {
                //Create a random color each time game starts
                ground.ChangeColor(solveColor);
            }

            i++;
        }

        // Check if we have reached our destination
        //if the distance between the ball's 3D position compared to the next collision position is less than 1
        if (nextCollisionPosition != Vector3.zero)
        {
            if (Vector3.Distance(transform.position, nextCollisionPosition) < 1)
            {
                isTraveling = false;
                //Ball moves in no direction
                travelDirection = Vector3.zero;
                //Nullify nextCollisionPosition
                nextCollisionPosition = Vector3.zero;
            }
        }
        //If ball is moving, exit the FixedUpdate method from this point downwards
        if (isTraveling)
            return;

        //If a user presses on screen or click the mouse button
        if (Input.GetMouseButton(0))
        {
            //Get coordinates of where the mouse/finger currently is
            swipePosCurrentFrame = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

            if (swipePosLastFrame != Vector2.zero)
            {

                
                //store where current finger postion is compared to some frames before.
                currentSwipe = swipePosCurrentFrame - swipePosLastFrame;

                //If square root of stored value distance above is below 500, don't recognize as a swipe
                if (currentSwipe.sqrMagnitude < minSwipeRecognition)
                    return;
                // Only get the direction not the distance 
                currentSwipe.Normalize(); 

                // Up/Down swipe
                if (currentSwipe.x > -0.5f && currentSwipe.x < 0.5f)
                {
                    SetDestination(currentSwipe.y > 0 ? Vector3.forward : Vector3.back);
                    dirtParticle.transform.Rotate(180, 0, 0);
                    dirtParticle.Play();
                    audioSource.PlayOneShot(rollSound, 1.0f);
                }

                // Left/Right swipe
                if (currentSwipe.y > -0.5f && currentSwipe.y < 0.5f)
                {
                    SetDestination(currentSwipe.x > 0 ? Vector3.right : Vector3.left);
                    dirtParticle.transform.Rotate(0, 180, 0);
                    dirtParticle.Play();
                    audioSource.PlayOneShot(rollSound, 1.0f);
                }
            }


            swipePosLastFrame = swipePosCurrentFrame;
        }

        if (Input.GetMouseButtonUp(0))
        {
            swipePosLastFrame = Vector2.zero;
            currentSwipe = Vector2.zero;
        }
    }

    private void SetDestination(Vector3 direction)
    {
        travelDirection = direction;

        
        //Check which object will be collided with
        RaycastHit hit;
        if (Physics.Raycast(transform.position, direction, out hit, 100f))
        {
            nextCollisionPosition = hit.point;
        }

        isTraveling = true;
    }
}
