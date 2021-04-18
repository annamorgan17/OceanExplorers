using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoloFishScript : MonoBehaviour
{
    public FlockData data; 
    [HideInInspector]
    public GameObject[] fish;
    public AudioSource sound;
    public AudioClip swishClip;
    public AudioClip bubbleClip;

    [HideInInspector] public Vector3 goalPos = Vector3.zero;
    private int counter = 0;
    private GameObject bubble;
    private bool turning = false;
    float speed;

    void Start()
    {
        speed = Random.Range(1, data.maxSpeed);
        foreach (GameObject f in data.soloFishprefab)
        {
            goalPos = data.setPoint;
            for (int i = 0; i < data.predatorsAmount[counter]; i++)
            {
                Vector3 position = new Vector3(
                                        Random.Range((data.setPoint.x - data.swimLimits.x), (data.setPoint.x + data.swimLimits.x)),
                                        Random.Range((data.setPoint.y - data.swimLimits.y), (data.setPoint.y + data.swimLimits.y)),
                                        Random.Range((data.setPoint.z - data.swimLimits.z), (data.setPoint.z + data.swimLimits.z)));
                fish[i] = (GameObject)Instantiate(f, position, Quaternion.identity);
            }
            counter++;
        }
    }

    void Update()
    {
        data.setPoint = transform.position;
        GoalPosRandom();
        Bounds b = new Bounds(data.setPoint, data.swimLimits * 2);
        foreach (GameObject f in fish)
        {
            if (!b.Contains(f.transform.position))
            {
                turning = true;
            }
            else
            {
                turning = false;
            }

            if (turning)
            {
                Vector3 direction = data.setPoint - f.transform.position;
                f.transform.rotation = Quaternion.Slerp(f.transform.rotation, Quaternion.LookRotation(direction), data.rotationSpeed * Time.deltaTime);
                sound.PlayOneShot(swishClip, 0.5f);
                speed = Random.Range(1, data.maxSpeed);

            }
            else
            {
                f.transform.rotation = Quaternion.Slerp(f.transform.rotation, Quaternion.LookRotation(goalPos), data.rotationSpeed * Time.deltaTime);
            }
            if (Random.Range(0, data.randomAmount) < 10)
            {
                Bubbles(data.bubblePrefab, f.transform);
                sound.PlayOneShot(bubbleClip, 0.5f);
            }
            f.transform.Translate(0, 0, Time.deltaTime * speed);
        }

    }
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

    public void DestroyFish(GameObject fishInstance)
    {
        Destroy(fishInstance);       
    }
}
