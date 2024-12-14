using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public static class CoRoutineAwaiterWrapper 
{
    // Convert an IEnumerator (coroutine) into a Task
    public static Task AsTask(this IEnumerator coroutine, MonoBehaviour monoBehaviour)
    {
        var tcs = new TaskCompletionSource<bool>();  // TaskCompletionSource to track coroutine completion
        monoBehaviour.StartCoroutine(RunCoroutine(coroutine, tcs)); // Start the coroutine
        return tcs.Task; // Return the Task that tracks the coroutine completion
    }

    // Helper coroutine to wrap the IEnumerator and complete the Task when done
    private static IEnumerator RunCoroutine(IEnumerator coroutine, TaskCompletionSource<bool> tcs)
    {
        // Run the coroutine until it finishes
        while (coroutine.MoveNext())
        {
            yield return coroutine.Current;
        }
        // Mark the task as completed
        tcs.SetResult(true);
    }
}
