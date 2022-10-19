using System.Collections.Generic;
using UnityEngine;

public class LerpTesting : MonoBehaviour
{
    public Transform Input;
    public Transform Output;

    private const float SEND_INTERVAL = 1f / 10f; // fps
    private float _sendTimer;

    private (Vector3, float) _receivedLast;
    private (Vector3, float) _receivedTarget;

    private void Receive(Vector3 newTarget)
    {
        // newTarget.z *= -1;
        // Output.position = newTarget;

        _receivedLast = _receivedTarget;
        _receivedTarget = (newTarget, Time.time);
    }

    private void ReceiveUpdate()
    {
        var last = _receivedLast.Item1;
        var target = _receivedTarget.Item1;

        var timeLast = _receivedLast.Item2;
        var timeTarget = _receivedTarget.Item2;
        var timeCurrent = Time.time;

        var duration = timeTarget - timeLast;
        var t = duration == 0 ? 0 : (timeCurrent - timeTarget) / duration;

        var output = Vector3.Lerp(last, target, t);
        output.z *= -1;

        // keep smooth if missing packets
        Output.position = Vector3.Lerp(Output.position, output, 0.5f);
    }

    // value, time to send
    private readonly List<(Vector3, float)> _packetsToSend = new();

    private void Update()
    {
        // Input.position = new Vector3(Mathf.Sin(Time.time * 4) * 2, Input.position.y, Input.position.z);

        var time = Time.time;

        _sendTimer += Time.deltaTime;
        if (_sendTimer >= SEND_INTERVAL)
        {
            // fake packet drops
            if (Random.Range(0f, 1f) > 0.4f)
            {
                // const float latency = 1f / 120f;
                const float latency = 1f / 240f;
                _packetsToSend.Add((Input.position, time + latency));
            }

            _sendTimer -= SEND_INTERVAL;
        }

        while (_packetsToSend.Count > 0 && _packetsToSend[0].Item2 <= time)
        {
            var packet = _packetsToSend[0];
            Receive(packet.Item1);
            _packetsToSend.RemoveAt(0);
        }

        ReceiveUpdate();
    }
}