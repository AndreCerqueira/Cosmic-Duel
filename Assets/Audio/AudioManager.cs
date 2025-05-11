using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Garante que só existe um
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Mantém entre cenas
        }

    }
}
