using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PredatorScript : MonoBehaviour
{
    public FlockData data;
    public SoloFishScript fishScript;
    public GameObject[] predatorsPrefab; //shark then tuna
    public GameObject remoraPrefab;
    public AudioSource sound;
    public AudioClip swishClip;
    public AudioClip bubbleClip;
    public AudioClip eatClip;

    private GameObject[] soloFish;
    private GameObject[] predators;
    private Vector3 goalPos = Vector3.zero;
    private int counter = 0;
    private GameObject bubble;
    private bool turning = false;
    private float speed;
    private int State = 0;
    private float distance;
    private GameObject targetedFish;

    void Start()
    {
        if (predators != null) { // overiding to stop the headset lagging out


            soloFish = fishScript.fish;
            speed = Random.Range(1, data.maxSpeed);
            foreach (GameObject p in predatorsPrefab) //loops through prefabs
            {
                goalPos = data.setPoint;
                for (int i = 0; i < data.predatorsAmount[counter]; i++) {
                    Vector3 position = new Vector3(
                                            Random.Range((data.setPoint.x - data.swimLimits.x), (data.setPoint.x + data.swimLimits.x)),
                                            Random.Range((data.setPoint.y - data.swimLimits.y), (data.setPoint.y + data.swimLimits.y)),
                                            Random.Range((data.setPoint.z - data.swimLimits.z), (data.setPoint.z + data.swimLimits.z)));
                    predators[i] = (GameObject)Instantiate(p, position, Quaternion.identity);
                    Vector3 remoraPos = new Vector3(position.x - 5, position.y - 5, position.z);
                    Instantiate(remoraPrefab, remoraPos, Quaternion.identity);
                }
                counter++;
            }
        }
    }

    void Update()
    {
        data.setPoint = transform.position;
        GoalPosRandom();
        CheckDistance(soloFish);
        StateMachine();
              
    }

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
    private void Bubbles(GameObject bubblePrefab, Transform pos)
    {
        bubble = Instantiate(bubblePrefab, pos.transform.position, Quaternion.LookRotation(Camera.main.transform.position));
        Destroy(bubble, 10f);

    }

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
                    sound.PlayOneShot(swishClip, 0.5f);
                    speed = Random.Range(1, data.maxSpeed);

                } else {
                    p.transform.rotation = Quaternion.Slerp(p.transform.rotation, Quaternion.LookRotation(goalPos), data.rotationSpeed * Time.deltaTime);
                }
                if (Random.Range(0, data.randomAmount) < 10) {
                    Bubbles(data.bubblePrefab, p.transform);
                    sound.PlayOneShot(bubbleClip, 0.5f);
                }
                p.transform.Translate(0, 0, Time.deltaTime * speed);
            }
        }
    }

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
                }
                p.transform.Translate(0, 0, Time.deltaTime * (speed + 2.0f));
            }
        }
    }

    private void Eat() {
        fishScript.DestroyFish(targetedFish);
        sound.PlayOneShot(eatClip, 0.5f);
        //change material from -1 to 1
    }

}
