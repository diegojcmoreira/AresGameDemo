using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTarget : MonoBehaviour {

    // Define the movement pattern of the object between 
    // horizontally(0)
    // Circular(1)
    // Sinus Wave(2)
    private int movementPattern;


    private Rigidbody rb;
    public int enemySpeed;

    //Check if the target is dead
    private bool dead;

    //Variables for horizontal movement
    Vector3 horizontalMovement;

    //Variables for circular movement
    Vector3 centerPoint;
    private float timeCounter;
    private float circularMovementRadius;

    //Variables for sinus movement
    Vector3 startPosition;
    private int sinusWaveFrequency;
    private float sinusWaveMagnitude;



    // varible to mark if the enemy movement has to reversed. turned on when object get to end of valid space
    private bool reverse;

    // Start is called before the first frame update
    void Start() {
        movementPattern = Random.Range(0,3);
        reverse = false;
        rb = GetComponent<Rigidbody>();
        dead = false;
        circularMovementRadius = 500;
        sinusWaveMagnitude = Random.Range(7,13);
        sinusWaveFrequency = Random.Range(2,7);

        enemySpeed = 50;

        //Define the center of the circular movement as the start position with the radius defined in the begining of the game
        centerPoint = new Vector3(transform.position.x + circularMovementRadius,transform.position.y,transform.position.z + circularMovementRadius);
        horizontalMovement = new Vector3(1, 0, 0);

        startPosition = transform.position;


        

    }

    // Update is called once per frame
    void Update(){

        if (movementPattern == 0){
            //horizontally
            HorizontalMovement();
            
        }
        if (movementPattern == 1){
            //Circular
            CircularMovement();

        }
        if (movementPattern == 2){
            //Circular
            SinusMovement();

        }
    }

    void OnCollisionEnter(Collision collision){
        if(collision.gameObject.name.Contains("Wall")){
            reverse = !reverse;
        }

        
    }

    void HorizontalMovement(){
        // Adds horizontal acceleration
            if(reverse){
                
                rb.AddForce(Vector3.left);

                //Control movement speed, maintaning constant
                rb.velocity = Vector3.left * enemySpeed;
           
            } else {
               
                rb.AddForce(Vector3.right);

                //Control movement speed, maintaning constant
                rb.velocity = Vector3.right * enemySpeed;
           
            }

    }

    void CircularMovement(){
        if(reverse){
            
            rb.transform.RotateAround(centerPoint, Vector3.up*-1, enemySpeed*Time.deltaTime);
        
        } else {
        
            rb.transform.RotateAround(centerPoint, Vector3.up, enemySpeed*Time.deltaTime);
        
        }
    }

    void SinusMovement(){
        Vector3 newPosition;

        if(reverse){
            newPosition = transform.position + Vector3.left * Time.deltaTime * enemySpeed; // Lateral movement
        
        } else {
            newPosition = transform.position + Vector3.right * Time.deltaTime * enemySpeed; // Lateral movement 
        
        }
        newPosition += Vector3.forward * Mathf.Sin(Time.time * sinusWaveFrequency) * sinusWaveMagnitude; //Up-Down Movement
        
            rb.MovePosition(newPosition);
        
        
        
        

    }

    public void KillTarget(){
        dead = true;
        transform.gameObject.SetActive(false);
    }
}
