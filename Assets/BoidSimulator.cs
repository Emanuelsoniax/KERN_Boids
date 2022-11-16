using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{
    public int ID;
    public Vector3 position;
    public Vector3 velocity;

    private void Start()
    {
        SetRandomPosition();
        SetRandomVelocity();
    }

    private void Update()
    {
        transform.position = position;
        transform.rotation = Quaternion.LookRotation(position + velocity.normalized);
    }

    public void SetRandomPosition() => position = new Vector3(Random.Range(-10, 10), Random.Range(-10, 10), Random.Range(-10, 10));
    public void SetRandomVelocity() => velocity = new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f));
}
public class BoidSimulator : MonoBehaviour
{
    [Header ("Settings")]
    public int numberOfBoids;
    public float maxboidSpeed;
    public float steeringSpeed;
    public float massPerFrame;
    public float sharedVelocityPerFrame;
    public float goalPerFrame;
    public float boidDistance;
    public float bounds;
    public Vector3 environmentalInfluence;
    public Transform goal;

    [Header("Prefabs")]
    public GameObject boidPrefab;
    List<Boid> boids = new List<Boid>();

    void Start()
    {
        InitializeBoids();
    }
    private void InitializeBoids()
    {
        for(int i = 0; i < numberOfBoids; i++)
        {
            GameObject newBoid = Instantiate(boidPrefab, transform.position, Quaternion.identity, transform);
            newBoid.AddComponent<Boid>();
            newBoid.GetComponent<Boid>().ID = i;
            boids.Add(newBoid.GetComponent<Boid>());
        }
    }

    // Update is called once per frame
    void Update()
    {
        MoveBoids();
    }

    private void MoveBoids()
    {
        foreach (Boid b in boids)
        {
            b.velocity +=   Rule1(b) + 
                            Rule2(b) + 
                            Rule3(b) + 
                            environmentalInfluence +
                            //MoveTowardsGoal(b) +
                            ConformToBounds(b);
                            

            if(b.velocity.magnitude > maxboidSpeed)
            {
                b.velocity = (b.velocity / b.velocity.magnitude) * maxboidSpeed;
            }

            b.position = b.position + b.velocity;
        }
    }

    //center of mass
    Vector3 Rule1(Boid _b)
    {
        Vector3 perceivedCenter = Vector3.zero;

        foreach (Boid b in boids)
        {
            if(b != _b)
            {
                perceivedCenter += b.position;
            }
        }

        perceivedCenter /= (numberOfBoids - 1);

        return (perceivedCenter - _b.position) / massPerFrame;
    }

    //seperation
    Vector3 Rule2(Boid _b)
    {
        Vector3 vector = Vector3.zero;

        foreach(Boid b in boids)
        {
            if (b != _b)
            {
                if (Vector3.Distance(_b.position, b.position) < boidDistance)
                {
                    vector = _b.position - b.position;
                }
            }
        }

        return vector;
    }

    //matching velocity
    Vector3 Rule3(Boid _b)
    {
        Vector3 perceivedVelocity = Vector3.zero;

        foreach(Boid b in boids)
        {
            if(b != _b)
            {
                perceivedVelocity += b.velocity;
            }
        }

        perceivedVelocity /= (numberOfBoids - 1);

        return (perceivedVelocity - _b.velocity) / sharedVelocityPerFrame;

    }

    Vector3 MoveTowardsGoal(Boid _b)
    {
        Vector3 moveTowards = goal.position;
        return (moveTowards - _b.transform.position) / goalPerFrame;
    }

    Vector3 ConformToBounds(Boid _b)
    {
        Vector3 vector = Vector3.zero;

        if (_b.position.x < -bounds) vector.x = 0.2f;
        else if (_b.position.x > bounds) vector.x = -0.2f;
        if (_b.position.y < -bounds) vector.y = 0.2f;
        else if (_b.position.y > bounds) vector.y = -0.2f;
        if (_b.position.z < -bounds) vector.z = 0.2f;
        else if (_b.position.z > bounds) vector.z = -0.2f;

        return vector;
    }

    //draws bounds in sceneview
    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(bounds *2, bounds*2, bounds*2));
    }
}
