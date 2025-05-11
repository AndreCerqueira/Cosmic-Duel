using UnityEngine;

/// Flutua suavemente em 2-3 D mantendo rotação nula.
public class SpaceFloat : MonoBehaviour
{
    [Header("Amplitudes do deslocamento")]
    [SerializeField] private Vector2 amplitudeXRange = new(0.1f, 0.4f);
    [SerializeField] private Vector2 amplitudeYRange = new(0.1f, 0.4f);
    [SerializeField] private Vector2 amplitudeZRange = new(0f, 0.1f);

    [Header("Frequências (ciclos/seg)")]
    [SerializeField] private Vector2 frequencyXRange = new(0.3f, 0.7f);
    [SerializeField] private Vector2 frequencyYRange = new(0.3f, 0.7f);
    [SerializeField] private Vector2 frequencyZRange = new(0.3f, 0.7f);

    /* ---------- valores por instância ---------- */
    private Vector3 _floatAmplitude;
    private Vector3 _floatFrequency;
    private Vector3 _phase;
    private Vector3 _startPos;

    /* ---------- SETUP ---------- */
    private void Awake()
    {
        _startPos = transform.position;

        _floatAmplitude = new Vector3(
            Random.Range(amplitudeXRange.x, amplitudeXRange.y),
            Random.Range(amplitudeYRange.x, amplitudeYRange.y),
            Random.Range(amplitudeZRange.x, amplitudeZRange.y));

        _floatFrequency = new Vector3(
            Random.Range(frequencyXRange.x, frequencyXRange.y),
            Random.Range(frequencyYRange.x, frequencyYRange.y),
            Random.Range(frequencyZRange.x, frequencyZRange.y));

        _phase = Random.insideUnitSphere * Mathf.PI * 2f;

        transform.rotation = Quaternion.identity;   // garante vertical no arranque
    }

    /* ---------- LOOP ---------- */
    private void Update()
    {
        float t = Time.time;

        float x = Mathf.Sin((t + _phase.x) * _floatFrequency.x * Mathf.PI * 2f) * _floatAmplitude.x;
        float y = Mathf.Sin((t + _phase.y) * _floatFrequency.y * Mathf.PI * 2f) * _floatAmplitude.y;
        float z = Mathf.Sin((t + _phase.z) * _floatFrequency.z * Mathf.PI * 2f) * _floatAmplitude.z;

        transform.position = _startPos + new Vector3(x, y, z);
        transform.rotation = Quaternion.identity;   // mantém-se sempre vertical
    }
}
