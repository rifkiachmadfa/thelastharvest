using System.Collections;
using UnityEngine;

/// <summary>
/// Sensor pintu otomatis. Pasang pada objek anak "Sensor" (Collider2D Is Trigger) di bawah objek pintu.
/// Pemain masuk sensor: sprite pintu berganti berurutan frames[0] -> frames[1] -> frames[2] (terbuka).
/// Pemain keluar sensor: berganti mundur sampai frames[0] (tertutup). Bila pemain keluar/masuk di tengah
/// animasi, pintu langsung berbalik dari frame saat itu. Collider penghalang (opsional) hanya aktif
/// saat pintu tertutup penuh. Pemain dikenali lewat PlayerMovement (bukan Tag), sama seperti IndoorZone.
/// Hanya tampilan dan collision; tidak menyentuh resource, waktu, atau save.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class DoorSensor : MonoBehaviour
{
    [Header("Visual")]
    [SerializeField] private SpriteRenderer doorRenderer;
    [Tooltip("Urutan frame: primary_pintu1 (tertutup), primary_pintu2, primary_pintu3 (terbuka penuh)")]
    [SerializeField] private Sprite[] frames;
    [SerializeField, Min(0.01f)] private float frameDelay = 0.12f;

    [Header("Collision")]
    [Tooltip("Collider non-trigger yang menghalangi jalan saat pintu tertutup (opsional)")]
    [SerializeField] private Collider2D blockingCollider;

    private int currentFrame;
    private int targetFrame;
    private int playersInside;
    private Coroutine animRoutine;

    private bool HasFrames => doorRenderer != null && frames != null && frames.Length > 0;

    private void Reset()
    {
        Collider2D sensor = GetComponent<Collider2D>();
        if (sensor != null) sensor.isTrigger = true;
    }

    private void Awake()
    {
        Collider2D sensor = GetComponent<Collider2D>();
        if (sensor != null && !sensor.isTrigger)
            Debug.LogWarning("DoorSensor: Collider2D sensor harus Is Trigger agar pintu terdeteksi.", this);
        if (!HasFrames)
            Debug.LogWarning("DoorSensor: isi Door Renderer dan Frames (primary_pintu1, 2, 3) di Inspector.", this);
    }

    private void Start()
    {
        ApplyFrame(0);
    }

    private void OnDisable()
    {
        if (animRoutine != null)
        {
            StopCoroutine(animRoutine);
            animRoutine = null;
        }
        playersInside = 0;
        targetFrame = 0;
        if (HasFrames) ApplyFrame(0);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsPlayer(other)) return;

        playersInside++;
        SetTarget(HasFrames ? frames.Length - 1 : 0);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!IsPlayer(other)) return;

        playersInside = Mathf.Max(0, playersInside - 1);
        if (playersInside == 0) SetTarget(0);
    }

    private static bool IsPlayer(Collider2D other)
    {
        return other.GetComponentInParent<PlayerMovement>() != null;
    }

    private void SetTarget(int index)
    {
        if (!HasFrames) return;

        targetFrame = index;
        if (animRoutine == null)
            animRoutine = StartCoroutine(Animate());
    }

    private IEnumerator Animate()
    {
        while (currentFrame != targetFrame)
        {
            ApplyFrame(currentFrame + (currentFrame < targetFrame ? 1 : -1));
            yield return new WaitForSeconds(frameDelay);
        }
        animRoutine = null;
    }

    private void ApplyFrame(int index)
    {
        currentFrame = index;
        if (HasFrames) doorRenderer.sprite = frames[index];

        // Penghalang aktif hanya saat pintu tertutup penuh (frame 0)
        if (blockingCollider != null) blockingCollider.enabled = (index == 0);
    }
}
