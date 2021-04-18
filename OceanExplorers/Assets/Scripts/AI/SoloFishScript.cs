using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//script controls the solo fish movements
public class SoloFishScript : MonoBehaviour
{
    public FlockData data; //scriptable object
    [HideInInspector]
    public GameObject[] fish;//game object array
    public AudioSource sound;//audio source connection
    public AudioClip swishClip;//sound effect for fish moving
    public AudioClip bubbleClip; //sound effect for bubbles

    [HideInInspector] public Vector3 goalPos = Vector3.zero;//goal pos for the solo fish to head towards
    private int counter = 0;//counter to count the cycles through a loop
    private GameObject bubble;//bubble prefab
    private bool turning = false;//if the solo fish is turning
    float speed;//solo fish speed

    void Start()
    {
        speed = Random.Range(1, data.maxSpeed); //sets speed to random number from 0 to the set max speed
        foreach (GameObject f in data.soloFishprefab) //loops through fish prefabs
        {
            goalPos = data.setPoint;//sets goal pos to set point
            for (int i = 0; i < data.predatorsAmount[counter]; i++)//loops through all the prefabs of current fish
            {
                Vector3 position = new Vector3(//creates a random vector within bounds
                                        Random.Range((data.setPoint.x - data.swimLimits.x), (data.setPoint.x + data.swimLimits.x)),
                                        Random.Range((data.setPoint.y - data.swimLimits.y), (data.setPoint.y + data.swimLimits.y)),
                                        Random.Range((data.setPoint.z - data.swimLimits.z), (data.setPoint.z + data.swimLimits.z)));
                fish[i] = (GameObject)Instantiate(f, position, Quaternion.identity); //creates an instance of current solo fish
            }
            counter++;//increases counter
        }
    }

    void Update()
    {
        data.setPoint = transform.position;//sets set point to this scripts pos
        GoalPosRandom();
        Bounds b = new Bounds(data.setPoint, data.swimLimits * 2);//creates a new bound the size of the swim limit
        foreach (GameObject f in fish)//loops through game objects
        {
            if (!b.Contains(f.transform.position))
            { //if not in bound
                turning = true;
            }
            else
            {//if in bounds
                turning = false;
            }

            if (turning)
            {//if out of bound turn the predator towards the centre at a new speed 
                Vector3 direction = data.setPoint - f.transform.position;
                f.transform.rotation = Quaternion.Slerp(f.transform.rotation, Quaternion.LookRotation(direction), data.rotationSpeed * Time.deltaTime);
                sound.PlayOneShot(swishClip, 0.5f);//plays the fish swimming sound effect
                speed = Random.Range(1, data.maxSpeed);

            }
            else
            {//if in bounds head towards goal
                f.transform.rotation = Quaternion.Slerp(f.transform.rotation, Quaternion.LookRotation(goalPos), data.rotationSpeed * Time.deltaTime);
            }
            if (Random.Range(0, data.randomAmount) < 10)
            {// randomly create bubble and play sound effect
                Bubbles(data.bubblePrefab, f.transform);
                sound.PlayOneShot(bubbleClip, 0.5f);
            }
            f.transform.Translate(0, 0, Time.deltaTime * speed);//move the solo fish at its speed
        }

    }
    //if trigger is entered by object tagged as terrain then direction is inversed
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "terrain")
        {
            foreach (GameObject f in fish) //loops through created game objs 
            {
                float floorDist = Vector3.Distance(f.transform.position, other.transform.position);

                if (floorDist <= 4)
                {
                    f.transform.rotation = Quaternion.Slerp(f.transform.rotation, Quaternion.Inverse(f.transform.rotation), data.rotationSpeed * Time.deltaTime);
                }
            }
        }

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
    //destroys instance of fish after 10f
    public void DestroyFish(GameObject fishInstance)
    {
        Destroy(fishInstance, 10f);       
    }
}
