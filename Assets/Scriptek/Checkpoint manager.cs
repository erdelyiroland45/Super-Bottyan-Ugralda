using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance { get; private set; }

    public Vector3 LastCheckpointPosition { get; private set; }
    public string LastSceneName { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetCheckpoint(Vector3 position, string sceneName)
    {
        LastCheckpointPosition = position;
        LastSceneName = sceneName;
    }

    public void SetInitialCheckpoint(Vector3 position, string sceneName)
    {
        if (LastCheckpointPosition == Vector3.zero && string.IsNullOrEmpty(LastSceneName))
        {
            SetCheckpoint(position, sceneName);
        }
    }
}