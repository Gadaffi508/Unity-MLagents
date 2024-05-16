using System;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using Random = UnityEngine.Random;

public class Agents : Agent
{
    public Transform target;

    public float multiplier = 5.0f;

    [field:NonSerialized] public Rigidbody _rb;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        if (transform.localPosition.y<0)
        {
            _rb.angularVelocity = Vector3.zero;
            _rb.velocity = Vector3.zero;
            transform.localPosition = new Vector3(0,0,0);
        }

        target.localPosition = new Vector3(Random.value * 8.5f - 4,0.5f,Random.value * 8.5f - 4);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(target.localPosition);
        sensor.AddObservation(transform.localPosition);

        sensor.AddObservation(_rb.velocity.x);
        sensor.AddObservation(_rb.velocity.z);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = actions.ContinuousActions[0];
        controlSignal.z = actions.ContinuousActions[1];
        _rb.AddForce(controlSignal * multiplier);

        float targetDistance = Vector3.Distance(transform.localPosition,target.localPosition);

        if (targetDistance < 1.5f)
        {
            SetReward(1.0f);
            EndEpisode();
        }

        if (transform.localPosition.y < 0f)
        {
            SetReward(-1.0f);
            EndEpisode();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuonusActionsOut = actionsOut.ContinuousActions;
        continuonusActionsOut[0] = Input.GetAxis("Horizontal");
        continuonusActionsOut[1] = Input.GetAxis("Vertical");
    }
}
