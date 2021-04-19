using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//script controls the predators movements and holds the state machine for eating fish
public class PredatorScript : MonoBehaviour
{
    public FlockData data;//scriptable object
    public SoloFishScript fishScript;//connection to solo fsih script
    public GameObject[] predatorsPrefab; //shark then tuna, prefab array
    public GameObject remoraPrefab; //remora prefab
    public AudioSource sound; //audio source connection
    public AudioClip swishClip; //sound effect for fish moving
    public AudioClip bubbleClip; //sound effect for bubbles
    public AudioClip eatClip; //sound effect for eating a fish
    public GameObject boneFish;//prefab for skelly fish

    private GameObject[] soloFish; //solo fish game object array
    private GameObject[] predators; //predators game object array
    private GameObject skellyFish; //game object of skelly fish
    private GameObject remora; //remora game object
    private Vector3 goalPos = Vector3.zero; //goal pos for the predators to head towards
    private int counter = 0; //counter to count the cycles through a loop
    private GameObject bubble; //bubble prefab
    private bool turning = false; //if the predator is turning
    private float speed; //predator speed
    private int State = 0; //the switch int for the state machine
    private float distance; //distance variable
    private GameObject targetedFish; //specific game object of the targeted fish

    void Start()
    {
        if (predators != null) { // overiding to stop the headset lagging out

            soloFish = fishScript.fish; //array connected to solo fish script
            speed = Random.Range(1, data.maxSpeed); //sets speed to random number from 0 to the set max speed
            foreach (GameObject p in predatorsPrefab) //loops through prefabs
            {
                predators = new GameObject[data.predatorsAmount[counter]]; //set array size to the amount of predators of that prefab
                goalPos = data.setPoint; //sets goal pos to set point
                for (int i = 0; i < data.predatorsAmount[counter]; i++) { //loops through all the prefabs of current predator
                    Vector3 position = new Vector3( //creates a random vector within bounds
                                            Random.Range((data.setPoint.x - data.swimLimits.x), (data.setPoint.x + data.swimLimits.x)),
                                            Random.Range((data.setPoint.y - data.swimLimits.y), (data.setPoint.y + data.swimLimits.y)),
                                            Random.Range((data.setPoint.z - data.swimLimits.z), (data.setPoint.z + data.swimLimits.z)));
                    predators[i] = (GameObject)Instantiate(p, position, Quaternion.identity); //creates an instance of current predator
                    Vector3 remoraPos = new Vector3(position.x - 5, position.y - 5, position.z); //creates a new pos based on the predators location
                    remora = (GameObject)Instantiate(remoraPrefab, remoraPos, Quaternion.identity); //creates a insatnce of remora at that pos
                    remora.transform.parent = predators[i].transform; //creates the predator as the remora parent
                }
                counter++; //increases counter
            }
        }
    }

    void Update()
    {
        data.setPoint = transform.position; //sets set point to this scripts pos
        GoalPosRandom();
        CheckDistance(soloFish);
        StateMachine();
              
    }
    //at random intervaules will create a random vector within bounds and set the goal pos to it
    private void GoalPosRandom()
    {
        if (Random.Range(0, data.randomAmount) < 50)
        {
            goalPos = new Vector3(
                                    Random.Range(data.setPoint.x - data.swimLimits.x, data.setPoint.x + data.swimLimits.x),
                                    Random.Range(data.setPoint.y - data.swimLimits.y, data.setPoint.y + data.swimLimits.y),
                                    Random.Range(data.setPoint.z - data.swimLimits.z, data.setPoint.z + data.swimLimits.z));
        }
    }
    //creates an instance of bubbles for 10f
    private void Bubbles(GameObject bubblePrefab, Transform pos)
    {
        bubble = Instantiate(bubblePrefab, pos.transform.position, Quaternion.LookRotation(Camera.main.transform.position));
        Destroy(bubble, 10f);

    }
    //state machine which switches through the predator states
    private void StateMachine()
    {
        switch(State)
        {
            case 0: //idle swimming
                {
                    Swim();

                    break;
                }
            case 1: //chasing down the prey
                {
                    Chase(targetedFish);

                    break;
                }
            case 2: //eating the prey
                {
                    Eat();

                    break;
                }
        }
    }
    //checks the distance from the solo fish and chnages the state depedning on the distance
    private void CheckDistance(GameObject[] soloFish)
    {
        if (predators != null) { // overiding to stop the headset lagging out
            foreach (GameObject f in soloFish) {
                distance = Vector3.Distance(transform.position, f.transform.position);

                if (distance < data.chaseDistance) {
                    targetedFish = f;
                    State = 1;

                    if (distance < data.eatDistance) {
                        State = 2;
                    }
                } else {
                    State = 0;
                }
            }
        }
        
    }
    //if trigger is entered by object tagged as terrain then direction is inversed
    private void OnTriggerEnter(Collider other)
    {
        if (predators != null) { // overiding to stop the headset lagging out
            if (other.gameObject.tag == "terrain") {
                foreach (GameObject p in predators) //loops through created game objs 
                {
                    float floorDist = Vector3.Distance(p.transform.position, other.transform.position);

                    if (floorDist <= 4) {
                        p.transform.rotation = Quaternion.Slerp(p.transform.rotation, Quaternion.Inverse(p.transform.rotation), data.rotationSpeed * Time.deltaTime);
                    }
                }
            }
        }
        
    }

    private void Swim()
    {
        if (predators != null) { // overiding to stop the headset lagging out
            Bounds b = new Bounds(data.setPoint, data.swimLimits * 2);//creates a new bound the size of the swim limit
            foreach (GameObject p in predators) //loops through created game objs 
            {
                if (!b.Contains(p.transform.position))
                { //if not in bound
                    turning = true;
                } 
                else
                {//if in bounds
                    turning = false;
                }

                if (turning)
                {//if out of bound turn the predator towards the centre at a new speed 
                    Vector3 direction = data.setPoint - p.transform.position;
                    p.transform.rotation = Quaternion.Slerp(p.transform.rotation, Quaternion.LookRotation(direction), data.rotationSpeed * Time.deltaTime);
                    sound.PlayOneShot(swishClip, 0.5f);//plays the fish swimming sound effect
                    speed = Random.Range(1, data.maxSpeed);

                } else
                { //if in bounds head towards goal
                    p.transform.rotation = Quaternion.Slerp(p.transform.rotation, Quaternion.LookRotation(goalPos), data.rotationSpeed * Time.deltaTime);
                }
                if (Random.Range(0, data.randomAmount) < 10)
                {// randomly create bubble and play sound effect
                    Bubbles(data.bubblePrefab, p.transform);
                   // bubble.transform.parent = p.transform; //creates the predator as the bubble parent
                    sound.PlayOneShot(bubbleClip, 0.5f);
                }
                p.transform.Translate(0, 0, Time.deltaTime * speed);//move the predator at its speed
            }
        }
    }
    //similar to swim but the direction is set to that of the solo fish and the speed is slightly increased
    private void Chase(GameObject soloFish) {
        if (predators != null) { // overiding to stop the headset lagging out
            Bounds b = new Bounds(data.setPoint, data.swimLimits * 2);
            foreach (GameObject p in predators) //loops through created game objs 
            {
                if (!b.Contains(p.transform.position)) {
                    turning = true;
                } else {
                    turning = false;
                }

                if (turning) {
                    Vector3 direction = data.setPoint - p.transform.position;
                    p.transform.rotation = Quaternion.Slerp(p.transform.rotation, Quaternion.LookRotation(direction), data.rotationSpeed * Time.deltaTime);
                    speed = Random.Range(1, data.maxSpeed);

                } else {
                    p.transform.rotation = Quaternion.Slerp(p.transform.rotation, Quaternion.LookRotation(soloFish.transform.position), data.rotationSpeed * Time.deltaTime);
                }
                if (Random.Range(0, data.randomAmount) < 10) {
                    Bubbles(data.bubblePrefab, p.transform);
                   // bubble.transform.parent = p.transform; //creates the predator as the bubble parent
                }
                p.transform.Translate(0, 0, Time.deltaTime * (speed + 2.0f));
            }
        }
    }
    //swicthes the material of the solo fish defore destorying after slowing changing its material and plays sound effect
    private void Eat() {
        float materialCount = 0;
        for(int i = 0; i < 1;  i++)
        {
            targetedFish.GetComponent<Material>().SetFloat("Progress", materialCount);
            materialCount = materialCount + 0.1f;
        }
       
        skellyFish = (GameObject)Instantiate(boneFish, targetedFish.transform.position, Quaternion.identity);
        fishScript.DestroyFish(targetedFish);
        sound.PlayOneShot(eatClip, 0.5f);
        Destroy(skellyFish, 10.0f);
        
    }

}
