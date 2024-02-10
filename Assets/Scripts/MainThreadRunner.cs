using System;
using System.Collections.Concurrent;
using UnityEngine;

public class MainThreadRunner : MonoBehaviour
{
    public readonly ConcurrentQueue<Action> RunOnMainThread = new ConcurrentQueue<Action>();

    void Update()
    {
        if (!RunOnMainThread.IsEmpty)
        {
            while (RunOnMainThread.TryDequeue(out var action))
            {
                action?.Invoke();
            }
        }
    }
}
