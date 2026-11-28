using System.Collections;
using UnityEngine;

/// Pasang di GameObject "Sensor" (anak dari pintu) yang punya Collider2D Is Trigger.
public class DoorSensor : MonoBehaviour
{
    [Header("Visual")]
    [SerializeField] private SpriteRenderer doorRenderer;
    [Tooltip("Urutan: pintu1 (tertutup), pintu2, pintu3 (terbuka penuh)")]
    [SerializeField] private Sprite[] frames;
    [SerializeField] private float frameDelay = 0.12f;

    [Header("Collision")]
    [Tooltip("Collider non-trigger yang menghalangi jalan saat pintu tertutup")]
    [SerializeField] private Collider2D blockingCollider;

    [Header("Sensor")]
    [SerializeField] private string playerTag = "Player";

    private int currentFrame;
    private int targetFrame;
    private int playersInside;
    private Coroutine animRoutine;

    private void Start()
    {
        currentFrame = 0;
        targetFrame = 0;
        if (doorRenderer != null && frames != null && frames.Length > 0)
            doorRenderer.sprite = frames[0];
        if (blockingCollider != null)
            blockingCollider.enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;
        playersInside++;
        SetTarget(frames.Length - 1);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;
        playersInside = Mathf.Max(0, playersInside - 1);
        if (playersInside == 0) SetTarget(0);
    }

    private void SetTarget(int index)
    {
        if (frames == null || frames.Length == 0) return;
        targetFrame = index;
        if (animRoutine == null)
            animRoutine = StartCoroutine(Animate());
    }

    private IEnumerator Animate()
    {
        while (currentFrame != targetFrame)
        {
            currentFrame += currentFrame < targetFrame ? 1 : -1;
            doorRenderer.sprite = frames[currentFrame];

            // Collider penghalang aktif hanya saat pintu tertutup penuh
            if (blockingCollider != null)
                blockingCollider.enabled = (currentFrame == 0);

            yield return new WaitForSeconds(frameDelay);
        }
        animRoutine = null;
    }
}