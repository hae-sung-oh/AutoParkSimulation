using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class SimulationManager : MonoBehaviour {
    [SerializeField] private List<ParkingLot> parkingLots;
    [SerializeField] private List<GameObject> carPrefabs;
    public AutoParkAgent agent;

    private List<GameObject> parkedCars;

    private float spawnXMin = -2f;
    private float spawnXMax = 2f;
    private float spawnZMin = -5f;
    private float spawnZMax = -3f;
    private bool _initComplete = false;

    public bool InitComplete => _initComplete;

    private void Awake() {
        parkedCars = new List<GameObject>();
        agent = GetComponent<AutoParkAgent>();
    }

    public void InitializeSimulation() {
        _initComplete = false;
        StartCoroutine(OccupyParkingSlotsWithRandomCars());
        RepositionAgentRandom();
    }

    public void RepositionAgentRandom() {
        if (agent != null) {
            agent.GetComponent<Rigidbody>().velocity = Vector3.zero;
            agent.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            agent.GetComponent<CarController>().CurrentSteeringAngle = 0f;
            agent.GetComponent<CarController>().CurrentAcceleration = 0f;
            agent.GetComponent<CarController>().CurrentBrakeTorque = 0f;
            agent.transform.rotation = Quaternion.Euler(0, 180, 0);
            if (agent.randomSpawn == 0)
                agent.transform.position = transform.parent.position;
            else
                agent.transform.position = transform.parent.position + new Vector3(Random.Range(spawnXMin, spawnXMax), 0f, Random.Range(spawnZMin, spawnZMax));
        }
    }

    public void ResetSimulation() {
        if (parkedCars != null) {
            foreach (GameObject parkedCar in parkedCars) {
                Destroy(parkedCar);
            }
            foreach (ParkingLot parkingLot in parkingLots) {
                parkingLot.IsOccupied = false;
            }
            parkedCars.Clear();
        }
    }

    public IEnumerator OccupyParkingSlotsWithRandomCars() {
        foreach (ParkingLot parkingLot in parkingLots) {
            parkingLot.IsOccupied = false;
        }
        yield return new WaitForSeconds(1);

        int total = Random.Range(parkingLots.Count - 6, parkingLots.Count - 2);
        for (int i = 0; i < total; i++) {
            ParkingLot lot = parkingLots.Where(r => r.IsOccupied == false).OrderBy(r => Guid.NewGuid()).FirstOrDefault();
            if (lot != null) {
                GameObject carInstance = Instantiate(carPrefabs[Random.Range(0, carPrefabs.Count)]);
                carInstance.transform.position = new Vector3(lot.transform.position.x, 0f, lot.transform.position.z);
                carInstance.transform.Rotate(new Vector3(0, lot.transform.eulerAngles.y+90, 0));
                parkedCars.Add(carInstance);
                lot.IsOccupied = true;
                if (parkedCars.Count >= total)
                    break;
            }
        }

        _initComplete = true;

        //if (Random.Range(0f, 1f) > 0.5f)
        //   PositionAtSafePlace(GetRandomEmptyParkingSlot().gameObject);
    }

    public ParkingLot GetRandomEmptyParkingSlot() {
        return parkingLots.Where(r => r.IsOccupied == false).OrderBy(r => Guid.NewGuid())
            .FirstOrDefault();
    }
    public void PositionAtSafePlace(GameObject nearestLotGameObject) {
        float[] ang = new float[] { -90f, 90f, 180f, -180f, 0f };

        if (agent != null) {
            agent.GetComponent<Rigidbody>().velocity = Vector3.zero;
            agent.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            agent.GetComponent<CarController>().CurrentSteeringAngle = 0f;
            agent.GetComponent<CarController>().CurrentAcceleration = 0f;
            agent.GetComponent<CarController>().CurrentBrakeTorque = 0f;
            Vector3 newPosition = nearestLotGameObject.transform.position;

            newPosition = nearestLotGameObject.transform.position +
                nearestLotGameObject.transform.right * Random.Range(-3f, -7f) +
                nearestLotGameObject.transform.forward * Random.Range(-1f, 1f);

            agent.transform.position = newPosition;
            agent.transform.Rotate(agent.transform.up, ang[Random.Range(0, 4)]);
        }
    }
}
