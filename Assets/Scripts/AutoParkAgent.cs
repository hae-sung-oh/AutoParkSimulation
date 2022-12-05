using System.Collections;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CarController))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(SimulationManager))]
public class AutoParkAgent : Agent {
    private Rigidbody _rigidBody;
    private CarController _controller;
    public SimulationManager _simulationManager;
    public ProgramController _programController;
    private ActionSegment<float> _lastActions;
    private ParkingLot _nearestLot = null;

    public MeshRenderer floorRd;
    public Material originMt;
    public Material badMt;
    public Material goodMt;
    
    public int randomSpawn; //0: No, 1: Yes
    public float timer;
    private float stoptimer;

    public override void Initialize() { //
        _rigidBody = GetComponent<Rigidbody>();
        _controller = GetComponent<CarController>();
        _simulationManager = GetComponent<SimulationManager>();
        _simulationManager.InitializeSimulation();
    }
     
    public override void OnEpisodeBegin() {
        _simulationManager.ResetSimulation();
        _simulationManager.InitializeSimulation();
        _nearestLot = null;
        timer = 0f;
        stoptimer = 0f;
        StartCoroutine(RevertMaterial());
    }

    public IEnumerator RevertMaterial() {
        yield return new WaitForSeconds(1f);
        floorRd.material = originMt;
    }

    public override void OnActionReceived(ActionBuffers vectorAction) {
        _lastActions = vectorAction.ContinuousActions;
        _controller.CurrentSteeringAngle = vectorAction.ContinuousActions[0];
        _controller.CurrentAcceleration = vectorAction.ContinuousActions[1];
        _controller.CurrentBrakeTorque = vectorAction.ContinuousActions[2];
    }

    public override void Heuristic(in ActionBuffers actionsOut) {
        var continousActionsOut = actionsOut.ContinuousActions;
        continousActionsOut[0] = Input.GetAxis("Horizontal");
        continousActionsOut[1] = Input.GetAxis("Vertical");
        continousActionsOut[2] = Input.GetAxis("Jump");
    }

    private void OnCollisionEnter(Collision other) {
        if (other.gameObject.CompareTag("barrier") || other.gameObject.CompareTag("car") ||
            other.gameObject.CompareTag("tree")) {
            floorRd.material = badMt;
            AddReward(-0.1f);
            EndEpisode();
        }
    }

    public override void CollectObservations(VectorSensor sensor) {
        if ( !_lastActions.IsEmpty() && _simulationManager.InitComplete && transform != null) {
            if (_nearestLot == null)
                _nearestLot = _simulationManager.GetRandomEmptyParkingSlot();
            Vector3 dirToTarget = (_nearestLot.transform.position - transform.position).normalized;
            sensor.AddObservation(transform.position.normalized);
            sensor.AddObservation(this.transform.InverseTransformPoint(_nearestLot.transform.position));
            sensor.AddObservation(this.transform.InverseTransformVector(_rigidBody.velocity.normalized));
            sensor.AddObservation(this.transform.InverseTransformDirection(dirToTarget));
            sensor.AddObservation(transform.forward);
            sensor.AddObservation(transform.right);
            // sensor.AddObservation(StepCount / MaxStep);
            float velocityAlignment = Vector3.Dot(dirToTarget, _rigidBody.velocity);
            AddReward(0.001f * velocityAlignment);
        }
        else {
            sensor.AddObservation(new float[18]);
        }
    }

    public IEnumerator JackpotReward(float bonus) {
        floorRd.material = goodMt;
        AddReward(0.2f + (bonus/timer));
        yield return new WaitForEndOfFrame();
        //yield return new WaitForSecondsRealtime(0.1f);
        EndEpisode();
    }

    private void LateUpdate() {
        timer += Time.deltaTime;
        if (timer > 2 && _rigidBody.velocity.magnitude < 1) stoptimer += Time.deltaTime;
        else stoptimer = 0;
        if (timer > 30 || (stoptimer > 4 /*&& _programController.manual == 0*/)) {
            floorRd.material = badMt;
            AddReward(-0.05f);
            EndEpisode();
        }
    }
}
