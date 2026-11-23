using UnityEngine;

public class KaijuCollisionGrowth : MonoBehaviour
{
    [Header("References")]
    public KaijuGrowth kaijuGrowth;
    public CharacterController characterController;

    [Header("Growth Amounts")]
    public float propGrowth = 10f;
    public float carGrowth = 30f;
    public float buildingGrowth = 100f;

    [Header("Detection")]
    public float baseDetectionRadius = 2f;
    public float radiusPerStage = 0.8f;
    public float radiusPadding = 1f;
    public LayerMask consumableLayers = ~0;

    [Header("Ignore")]
    public string[] ignoreTags = { "Ground", "Player" };

    [Header("Feedback")]
    public bool destroyOnContact = true;
    public float minTimeBetweenHits = 0.04f;

    private float lastHitTime;

    void Start()
    {
        if (kaijuGrowth == null)
            kaijuGrowth = FindObjectOfType<KaijuGrowth>();

        if (characterController == null)
            characterController = GetComponent<CharacterController>();
    }

    float CurrentDetectionRadius()
    {
        if (kaijuGrowth == null)
            return baseDetectionRadius;

        // Scale detection with the actual visual size so we eat things before the green collider hits them
        float visualScale = Mathf.Max(1f, kaijuGrowth.GetTotalHeightMultiplier());
        return (baseDetectionRadius + kaijuGrowth.currentStage * radiusPerStage) * (0.6f + visualScale * 0.15f);
    }

    void Update()
    {
        if (kaijuGrowth == null || characterController == null) return;
        if (Time.time - lastHitTime < minTimeBetweenHits) return;

        float radius = CurrentDetectionRadius() + radiusPadding;

        Vector3 center = transform.position + characterController.center;
        float height = characterController.height;
        float halfHeight = Mathf.Max(0f, height * 0.5f - characterController.radius);

        Vector3 bottom = center + Vector3.down * halfHeight;
        Vector3 top = center + Vector3.up * halfHeight;

        Collider[] hits = Physics.OverlapCapsule(bottom, top, radius, consumableLayers, QueryTriggerInteraction.Collide);

        foreach (Collider hit in hits)
        {
            if (hit == null) continue;
            if (hit.gameObject == gameObject) continue;
            if (hit.transform.IsChildOf(transform)) continue;
            if (ShouldIgnore(hit.gameObject)) continue;

            Consume(hit);
            return;
        }
    }

    bool ShouldIgnore(GameObject target)
    {
        if (target == null) return true;
        if (target.CompareTag("Ground") || target.CompareTag("Player")) return true;

        string lower = target.name.ToLower();
        if (lower.Contains("ground") || lower.Contains("plane")) return true;

        return false;
    }

    void Consume(Collider hit)
    {
        if (kaijuGrowth == null || hit == null) return;
        if (Time.time - lastHitTime < minTimeBetweenHits) return;

        GameObject target = hit.gameObject;

        // Ignore collision with EVERY collider on this object and its children
        // so the Character Controller can't get stuck on any part of the building
        Collider[] allColliders = target.GetComponentsInChildren<Collider>();
        foreach (Collider c in allColliders)
        {
            if (c != null)
                Physics.IgnoreCollision(characterController, c, true);
        }

        float growthAmount = GetGrowthAmount(target);
        kaijuGrowth.AddGrowth(growthAmount);
        lastHitTime = Time.time;

        Debug.Log($"[KaijuCollision] ATE {target.name} for {growthAmount}");

        if (destroyOnContact)
            Destroy(target);
    }

    float GetGrowthAmount(GameObject target)
    {
        string name = target.name.ToLower();

        if (name.Contains("building") || name.Contains("house") || name.Contains("tower") || name.Contains("eco"))
            return buildingGrowth;
        if (name.Contains("car") || name.Contains("vehicle") || name.Contains("truck") || name.Contains("bus") || name.Contains("van"))
            return carGrowth;

        return propGrowth;
    }

    void OnDrawGizmosSelected()
    {
        if (characterController == null) return;

        float radius = Application.isPlaying ? CurrentDetectionRadius() + radiusPadding : baseDetectionRadius + radiusPadding;
        Gizmos.color = Color.red;

        Vector3 center = transform.position + characterController.center;
        float height = characterController.height;
        float halfHeight = Mathf.Max(0f, height * 0.5f - characterController.radius);

        Vector3 bottom = center + Vector3.down * halfHeight;
        Vector3 top = center + Vector3.up * halfHeight;

        Gizmos.DrawWireSphere(bottom, radius);
        Gizmos.DrawWireSphere(top, radius);

        Gizmos.DrawLine(bottom + Vector3.left * radius, top + Vector3.left * radius);
        Gizmos.DrawLine(bottom + Vector3.right * radius, top + Vector3.right * radius);
        Gizmos.DrawLine(bottom + Vector3.forward * radius, top + Vector3.forward * radius);
        Gizmos.DrawLine(bottom + Vector3.back * radius, top + Vector3.back * radius);
    }
}