using System.Collections;
using UnityEngine;

public class ShipMover : MonoBehaviour
{
    [SerializeField] private float speed = 3f;          // unidades/s
    private Coroutine currentMove;

    public void MoveTo(Vector3 destination)
    {
        if (currentMove != null) StopCoroutine(currentMove);
        currentMove = StartCoroutine(MoveRoutine(destination));
    }

    private IEnumerator MoveRoutine(Vector3 target)
    {
        while (Vector3.Distance(transform.position, target) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
            yield return null;
        }
        transform.position = target;
    }
}
