using UnityEngine;
using UnityEngine.Events;

public class KaijuLocomotionManager : MonoBehaviour
{
    [Header("References")]
    public KaijuGrowth kaijuGrowth;

    [Header("Locomotion Mode GameObjects")]
    public GameObject pestLocomotion;
    public GameObject menaceLocomotion;
    public GameObject titanLocomotion;

    [Header("Speed Scaling")]
    public float baseMoveSpeed = 2.5f;
    public float speedMultiplierPerStage = 1.5f;

    public UnityEvent<int> onLocomotionStageChanged;

    private int lastStage = -1;

    void Start()
    {
        if (kaijuGrowth == null)
            kaijuGrowth = FindObjectOfType<KaijuGrowth>();

        if (kaijuGrowth != null)
        {
            kaijuGrowth.onStageUp.AddListener(OnStageChanged);
            UpdateLocomotionForStage(kaijuGrowth.currentStage);
        }
        else
        {
            UpdateLocomotionForStage(0);
        }
    }

    void OnDestroy()
    {
        if (kaijuGrowth != null)
            kaijuGrowth.onStageUp.RemoveListener(OnStageChanged);
    }

    void OnStageChanged(int newStage)
    {
        UpdateLocomotionForStage(newStage);
    }

    void Update()
    {
        if (kaijuGrowth != null && kaijuGrowth.currentStage != lastStage)
            UpdateLocomotionForStage(kaijuGrowth.currentStage);
    }

    void UpdateLocomotionForStage(int stage)
    {
        lastStage = stage;

        if (pestLocomotion != null) pestLocomotion.SetActive(false);
        if (menaceLocomotion != null) menaceLocomotion.SetActive(false);
        if (titanLocomotion != null) titanLocomotion.SetActive(false);

        GameObject active = stage == 0 ? pestLocomotion :
                            stage == 1 ? menaceLocomotion : titanLocomotion;

        if (active != null)
            active.SetActive(true);
        else if (pestLocomotion != null)
            pestLocomotion.SetActive(true);

        onLocomotionStageChanged?.Invoke(stage);
    }

    [ContextMenu("Refresh Locomotion")]
    public void Refresh()
    {
        if (kaijuGrowth != null)
            UpdateLocomotionForStage(kaijuGrowth.currentStage);
    }
}