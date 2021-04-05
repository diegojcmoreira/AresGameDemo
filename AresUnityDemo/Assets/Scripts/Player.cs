using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ClientController;
using System.Threading;
using System.Text;
using System;

public class Player : MonoBehaviour {


    private Rigidbody rb;
    
    public List<WheelCollider> throttleWheels;
    public List<WheelCollider> steeringWheels;

    public int motorForce;
    public int brakeForce;
    

    private int steerForce;

    private float throttle;
    private float steer;
    private float breakCommand;

    public float pointCenter;

    //Weapon controller

    private int range;
    public Camera mainCam;
    public ParticleSystem muzzleFlash;
    public GameObject weapon;

    private float mouseX;
    private float mouseY;
    private float rotationOnX;
    private float rotationOnY;

    public float mouseSensitivity;
    public bool mouseLeftClick;

    private Client client;
    private Thread inputThread;

    private int shotsFired;
    private int shotsHit;
    private int enemiesAlive;

    private float timeRemaining;



    // Controls player, request input for movement of the car and the weapon
    void Start() {
        steerForce = 15;
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = new Vector3(0 , -5, 0);

        //initiate weapon variables
        range = 500;
        rotationOnX = 0;
        rotationOnY = 0;
        mouseSensitivity = 90f;
        mouseLeftClick = false;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        shotsFired = 0;
        shotsHit = 0;
        enemiesAlive = 5;
        timeRemaining = 120;
        muzzleFlash.Stop();

        client = new Client(8081);
        client.init();


        // Starts thread to request player input       
        inputThread = new Thread(getInputFromServer);
        inputThread.Start();
        
    }

    void Update() {

        //Throttle
        foreach (WheelCollider wheel in throttleWheels) {
            wheel.motorTorque = throttle * motorForce;
            
        }

        //Steering
        foreach (WheelCollider wheel in steeringWheels) {
            wheel.steerAngle = steer * steerForce;

            
        }

        //Hard Break
        foreach (WheelCollider wheel in throttleWheels) {
            wheel.brakeTorque = breakCommand*brakeForce;

                
            
        }

        // Control Weapon
        RotateWeapon();
        mouseY = 0;
        mouseX = 0;       

        // Shoot weapon
        if(mouseLeftClick){
            muzzleFlash.Emit(1000);
            Shoot();
        }

        if (timeRemaining > 0) {
            
            timeRemaining -= Time.deltaTime;

        } else {
            EndGame();
        }



    }

    void getInputFromServer(){


        sbyte[] receiveBytes;
        string receivedString = "";

        // check exit signal
        while(!receivedString.Equals("exit")){
            receiveBytes = client.getKeyboardInput(); //[a|d|w|s|space|left button|mouse.deltaX|mouse.deltaY]

            if(receiveBytes.Length == 8){
                throttle = -1*receiveBytes[3] + 1*receiveBytes[2];
                steer = -1*receiveBytes[0] + 1*receiveBytes[1];
                breakCommand = receiveBytes[4];
                mouseLeftClick = receiveBytes[5] == 1;
                mouseX = receiveBytes[6];
                mouseY = receiveBytes[7];
                
            }

            receivedString = Encoding.ASCII.GetString((byte[]) (Array)receiveBytes);

        }


    }

    void getInput(){
        throttle = Input.GetAxis("Vertical");
        steer = Input.GetAxis("Horizontal");


    }

    void Shoot(){
        RaycastHit hit;
        
        if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0)), out hit, range)){
            
            //If hits target
            if (hit.transform.name.Contains("Target")){

                hit.transform.gameObject.gameObject.GetComponent<EnemyTarget>().KillTarget();
                shotsHit++;
                enemiesAlive--;

                if(enemiesAlive == 0){
                    EndGame();
                }

            }
        }
        mouseLeftClick = false;
        shotsFired++;
    }

    void RotateWeapon(){
        rotationOnX -= mouseY*mouseSensitivity*Time.deltaTime;
        rotationOnX = Mathf.Clamp(rotationOnX,-60f,10f);

        rotationOnY += mouseX*mouseSensitivity*Time.deltaTime;

        weapon.transform.localEulerAngles = new Vector3(rotationOnX, rotationOnY, 0f);

    }

    void EndGame(){
        client = new Client(8089);
        try {
            byte[] receiveBytes = client.receiveMessage();
            string receivedString = Encoding.ASCII.GetString(receiveBytes);
            Debug.Log(receivedString);

            int[] stats = {-9, shotsFired, shotsHit};
            byte[] sendStats =  new byte[stats.Length * sizeof(int)];
            Buffer.BlockCopy(stats, 0, sendStats, 0, sendStats.Length);;
            client.sendMessage(sendStats);

            Application.Quit();
                

        }
        catch(Exception e)
        {
            Debug.Log("Exception thrown " + e.Message);
        }
       
        
    }
}
