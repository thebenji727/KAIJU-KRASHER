using UnityEngine;
using UnityEngine.Events;

public class KaijuGrowth : MonoBehaviour
{
    [Header("Growth")]
    public float currentGrowth = 0f;
    public float growthPerStage = 100f;
    public int currentStage = 0;
    public int maxStage = 3;                 // Pest → Menace → Titan

    [Header("Scale")]
    public Transform scaleTarget;            // Assign the XR Origin (or the city root if you invert scale)
    public float baseScale = 1f;
    public float scalePerStage = 1.8f;       // Multiplier applied each stage
    public bool invertScale = false;         // If true, scales the world down instead of the player up

    [Header("Events")]
    public UnityEvent<int> onStageUp;        // Fires with the new stage number
    public UnityEvent onGrowthChanged;

    private float targetScale;

    void Start()
    {
        if (scaleTarget == null)
            scaleTarget = transform;

        ApplyScaleImmediate();
    }

    void Update()
    {
        // Smooth scale change (optional but feels better)
        if (scaleTarget != null)
        {
            float current = invertScale ? 1f / scaleTarget.localScale.x : scaleTarget.localScale.x;
            float desired = invertScale ? 1f / targetScale : targetScale;

            if (Mathf.Abs(current - desired) > 0.01f)
            {
                float newScale = Mathf.Lerp(current, desired, Time.deltaTime * 2f);
                if (invertScale)
                    scaleTarget.localScale = Vector3.one / newScale;
                else
                    scaleTarget.localScale = Vector3.one * newScale;
            }
        }
    }

    /// <summary>
    /// Call this whenever the kaiju destroys or eats something.
    /// </summary>
    public void AddGrowth(float amount)
    {
        if (currentStage >= maxStage) return;

        currentGrowth += amount;
        onGrowthChanged?.Invoke();

        while (currentGrowth >= growthPerStage && currentStage < maxStage)
        {
            currentGrowth -= growthPerStage;
            LevelUp();
        }
    }

    void LevelUp()
    {
        currentStage++;
        ApplyScaleImmediate();
        onStageUp?.Invoke(currentStage);

        Debug.Log($"Kaiju grew to stage {currentStage}");
    }

    void ApplyScaleImmediate()
    {
        targetScale = baseScale * Mathf.Pow(scalePerStage, currentStage);

        if (scaleTarget != null)
        {
            if (invertScale)
                scaleTarget.localScale = Vector3.one / targetScale;
            else
                scaleTarget.localScale = Vector3.one * targetScale;
        }
    }

    // Handy for UI or debugging
    public float GetStageProgress()
    {
        return Mathf.Clamp01(currentGrowth / growthPerStage);
    }

    public float GetTotalHeightMultiplier()
    {
        return targetScale;
    }
}