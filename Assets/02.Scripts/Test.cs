using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public Transform start;
    public Transform end;

    public Vector3 axis;
    public void Update()
    {
        // start.rotation = Quaternion.RotateTowards(start.rotation, Quaternion.LookRotation(end.position - start.position), 5f);

        Vector3 startPos = start.position;
        Vector3 endPos = end.position;

        Vector3 direction = start.forward;
        Vector3 targetDir = end.position - start.position;

        float rot;

        targetDir = targetDir.normalized;

        if (targetDir.y > direction.y) rot = -1;
        else if (targetDir.y < direction.y) rot = 1;
        else rot = 0;

        start.Rotate(rot, 0, 0);

        print($"TargetDir : {targetDir}   Dirction : {direction}");

    }

    public void OnDrawGizmos()
    {
        Vector3 direction = start.forward;
        Vector3 targetDir = end.position - start.position;
        direction = direction.normalized;
        targetDir = targetDir.normalized;


        Gizmos.color = Color.green;
        Gizmos.DrawRay(start.position, direction);

        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(start.position, targetDir);
    }

}
