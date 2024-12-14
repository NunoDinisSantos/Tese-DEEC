using System.Collections.Generic;
using UnityEngine;

public class FishSpawnerBoundaries : MonoBehaviour
{
    [SerializeField] private List<Transform> Boundaries = new();
    [SerializeField] private Transform[] fishSpawns;

    private void Start()
    {
        Boundaries.Clear();
        for (int i = 1; i < transform.childCount; i++)
        {
            Boundaries.Add(transform.GetChild(i));
        }
    }

    private void OnDrawGizmos()
    {
        if (Boundaries == null || Boundaries.Count < 7) return;

        for (int i = 0; i < Boundaries.Count; i++)
        {
            if (i == 0 || i == 6 || i == 2 || i == 4)
                Gizmos.color = Color.blue;

            else
            {
                Gizmos.color = Color.green;
            }

            Gizmos.DrawSphere(Boundaries[i].position, 0.5f);
            Gizmos.color = Color.red;
        }

        Gizmos.DrawLine(Boundaries[2].position, Boundaries[0].position);
        Gizmos.DrawLine(Boundaries[0].position, Boundaries[6].position);
        Gizmos.DrawLine(Boundaries[6].position, Boundaries[4].position);
        Gizmos.DrawLine(Boundaries[5].position, Boundaries[3].position);
        Gizmos.DrawLine(Boundaries[5].position, Boundaries[7].position);
        Gizmos.DrawLine(Boundaries[3].position, Boundaries[1].position);

        Gizmos.DrawLine(Boundaries[7].position, Boundaries[1].position);
        Gizmos.DrawLine(Boundaries[0].position, Boundaries[1].position);
        Gizmos.DrawLine(Boundaries[0].position, Boundaries[2].position);
        Gizmos.DrawLine(Boundaries[3].position, Boundaries[2].position);
        Gizmos.DrawLine(Boundaries[3].position, Boundaries[5].position);
        Gizmos.DrawLine(Boundaries[4].position, Boundaries[5].position);
        Gizmos.DrawLine(Boundaries[4].position, Boundaries[6].position);
        Gizmos.DrawLine(Boundaries[4].position, Boundaries[2].position);
        Gizmos.DrawLine(Boundaries[6].position, Boundaries[7].position);
    }
}
