using System.Collections;
using UnityEngine;
using System;

public class ShipMover : MonoBehaviour
{
    [Header("Velocidade (unidades / segundo)")]
    [SerializeField] private float minSpeed = 3f;      // destino colado à nave
    [SerializeField] private float maxSpeed = 10f;     // destino no limite do mapa

    [Header("Factor de escala")]
    [Tooltip("Distância que resulta em maxSpeed. Se 0 usa a diagonal da câmara.")]
    [SerializeField] private float referenceDistance = 15f;

    private Coroutine currentMove;
    private Action onArrive;

    /* ---------- API ---------- */
    public void MoveTo(Vector3 destination, Action arriveCallback)
    {
        onArrive = arriveCallback;

        if (currentMove != null) StopCoroutine(currentMove);
        currentMove = StartCoroutine(MoveRoutine(destination));
    }

    /* ---------- corrotina ---------- */
    private IEnumerator MoveRoutine(Vector3 target)
    {
        while (Vector3.Distance(transform.position, target) > 0.01f)
        {
            float dist = Vector3.Distance(transform.position, target);
            float speed = Mathf.Lerp(minSpeed, maxSpeed,
                              Mathf.Clamp01(dist / referenceDistance));
            transform.position = Vector3.MoveTowards(
                                     transform.position, target, speed * Time.deltaTime);
            yield return null;
        }

        transform.position = target;   // ❷ garante posição exacta
        onArrive?.Invoke();            // ❸ avisa quem chamou

    }

    /* ---------- lógica de velocidade ---------- */
    private float CalculateSpeed(float distance)
    {
        // Se referenceDistance = 0, calcula automáticamente diagonal da câmara
        float refDist = (referenceDistance > 0)
                        ? referenceDistance
                        : GetCameraDiagonal();

        // Normaliza 0-1 e clamp
        float t = Mathf.Clamp01(distance / refDist);

        // LINEAR:  speed = min + t*(max-min)
        return Mathf.Lerp(minSpeed, maxSpeed, t);

        // Quer curva exponencial? troca por:
        // return Mathf.Lerp(minSpeed, maxSpeed, t * t);      // acelera mais devagar no início
    }

    private float GetCameraDiagonal()
    {
        if (Camera.main == null || !Camera.main.orthographic) return 20f;

        float h = Camera.main.orthographicSize * 2f;
        float w = h * Camera.main.aspect;
        return Mathf.Sqrt(w * w + h * h);   // diagonal visível
    }
}
