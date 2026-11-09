using System.Threading;
using UnityEngine;

/// <summary>
/// Attach to any GameObject. Drag the "Load" slider in the Inspector to increase
/// CPU pressure and watch FPS drop in the Stats window.
/// </summary>
public class CpuLoadController : MonoBehaviour
{
    [Header("Load Control")]
    [Range(0f, 1f)]
    [Tooltip("0 = idle, 1 = maximum CPU load. Drag this in the Inspector at runtime.")]
    public float load = 0f;

    [Header("Load Settings")]
    [Tooltip("Maximum number of background worker threads to spawn at full load.")]
    [Range(1, 32)]
    public int maxWorkerThreads = 8;

    [Tooltip("How many busy-work iterations each worker performs per cycle.")]
    public int iterationsPerWorker = 500000;

    // ── private state ──────────────────────────────────────────────────────────
    private int   _activeWorkers = 0;
    private float _previousLoad  = -1f; // force first-frame evaluation

    // ── Unity lifecycle ────────────────────────────────────────────────────────
    void Update()
    {
        // Detect inspector changes and adjust worker count accordingly
        if (!Mathf.Approximately(load, _previousLoad))
        {
            _previousLoad = load;
            SyncWorkers();
        }

        // Main-thread busy work — directly pressures the game loop, drops FPS
        if (load > 0f)
        {
            int mainIterations = Mathf.RoundToInt(load * iterationsPerWorker * 0.5f);
            DoWork(mainIterations);
        }

        Debug.Log("Hello IAC, we are doing profillin sessionm truing to overload our CPU and memory by puttin gthis long string into RAM");
        Debug.Log("Hello IAC, we are doing profillin sessionm truing to overload our CPU and memory by puttin gthis long string into RAM");
        Debug.Log("Hello IAC, we are doing profillin sessionm truing to overload our CPU and memory by puttin gthis long string into RAM");
        Debug.Log("Hello IAC, we are doing profillin sessionm truing to overload our CPU and memory by puttin gthis long string into RAM");
    }

    void OnDestroy()
    {
        // Signal all workers to stop
        load = 0f;
    }

    // ── Worker management ──────────────────────────────────────────────────────
    private void SyncWorkers()
    {
        int target = Mathf.RoundToInt(load * maxWorkerThreads);
        int diff   = target - _activeWorkers;

        if (diff > 0)
            for (int i = 0; i < diff; i++) SpawnWorker();
        // Workers self-terminate when they detect _activeWorkers > target
    }

    private void SpawnWorker()
    {
        Interlocked.Increment(ref _activeWorkers);

        Thread t = new Thread(() =>
        {
            while (true)
            {
                int target = Mathf.RoundToInt(load * maxWorkerThreads);

                if (_activeWorkers > target)
                {
                    Interlocked.Decrement(ref _activeWorkers);
                    return;
                }

                DoWork(iterationsPerWorker);
                Thread.Sleep(0); // yield to avoid starving the OS scheduler
            }
        });

        t.IsBackground = true;
        t.Priority     = System.Threading.ThreadPriority.BelowNormal;
        t.Start();
    }

    // ── CPU-intensive dummy work ───────────────────────────────────────────────
    private static void DoWork(int iterations)
    {
        double acc = 1.0;
        for (int i = 1; i < iterations; i++)
            acc += System.Math.Sqrt(i) * System.Math.Sin(i);

        // Prevent the compiler from optimizing the loop away
        if (acc == double.MinValue) Debug.Log("noop");
    }
}
