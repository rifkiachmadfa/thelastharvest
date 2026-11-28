#if UNITY_EDITOR
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

/// <summary>
/// Menu Editor: membuat objek pintu otomatis (SpriteRenderer + DoorSensor), mengisi Frames dari sprite
/// Assets/object/primary_pintu1..4, dan menyambungkan Door Renderer. Pintu baru diletakkan di posisi SuitGate
/// (bila ada), atau di atas Player sebagai placeholder.
/// Aman dijalankan berulang kali; objek dan isian Inspector yang sudah ada tidak ditimpa.
/// Mars > Setup Door
/// </summary>
public static class MarsDoorSetup
{
    private const int FrameCount = 4;
    private const string SpritePathFormat = "Assets/object/primary_pintu{0}.png";

    [MenuItem("Mars/Setup Door")]
    public static void Setup()
    {
        StringBuilder report = new StringBuilder();

        PlayerMovement player = Object.FindFirstObjectByType<PlayerMovement>();
        if (player == null)
        {
            EditorUtility.DisplayDialog("Mars Setup", "PlayerMovement tidak ditemukan di scene.", "OK");
            return;
        }

        if (Object.FindFirstObjectByType<SuitSystem>() == null)
            report.AppendLine("- PERINGATAN: SuitSystem tidak ada di scene; pintu tidak akan terbuka. Jalankan Mars > Setup Suit and Gate.");

        // 1. Sprite frame (urut berdasarkan nama file: primary_pintu1, 2, 3, 4)
        List<Sprite> sprites = new List<Sprite>();
        for (int i = 1; i <= FrameCount; i++)
        {
            Sprite sprite = LoadFirstSprite(string.Format(SpritePathFormat, i));
            if (sprite == null)
            {
                report.AppendLine("- PERINGATAN: sprite " + string.Format(SpritePathFormat, i) +
                                  " tidak ditemukan atau belum bertipe Sprite. Frames tidak diisi otomatis.");
                sprites.Clear();
                break;
            }
            sprites.Add(sprite);
        }

        // 2. Objek pintu (pakai DoorSensor yang sudah ada bila ada)
        DoorSensor door = Object.FindFirstObjectByType<DoorSensor>();
        if (door == null)
        {
            GameObject go = new GameObject("Door", typeof(SpriteRenderer));
            Undo.RegisterCreatedObjectUndo(go, "Create Door");

            SuitGate gate = Object.FindFirstObjectByType<SuitGate>();
            go.transform.position = gate != null
                ? gate.transform.position
                : player.transform.position + new Vector3(0f, 3f, 0f);

            SpriteRenderer renderer = go.GetComponent<SpriteRenderer>();
            if (sprites.Count > 0) renderer.sprite = sprites[0];
            renderer.sortingOrder = 10;

            door = Undo.AddComponent<DoorSensor>(go);
            report.AppendLine(gate != null
                ? "- Door dibuat di posisi SuitGate. Atur posisi dan Scale agar pas dengan pintu."
                : "- Door dibuat di atas Player. WAJIB dipindahkan ke pintu keluar dan di-scale agar pas.");
        }
        else
        {
            report.AppendLine("- DoorSensor sudah ada: " + door.gameObject.name);
        }

        // 3. Door Renderer
        SerializedObject so = new SerializedObject(door);
        SerializedProperty rendererProp = so.FindProperty("doorRenderer");
        if (rendererProp != null && rendererProp.objectReferenceValue == null)
        {
            SpriteRenderer found = door.GetComponent<SpriteRenderer>();
            if (found == null) found = door.GetComponentInParent<SpriteRenderer>();
            if (found == null) found = door.GetComponentInChildren<SpriteRenderer>();

            if (found != null)
            {
                rendererProp.objectReferenceValue = found;
                report.AppendLine("- Door Renderer disambungkan ke " + found.gameObject.name + ".");
            }
            else
            {
                report.AppendLine("- PERINGATAN: tidak ada SpriteRenderer untuk Door Renderer. Isi manual di Inspector.");
            }
        }

        // 4. Frames (hanya diisi bila kosong atau ada elemen kosong)
        SerializedProperty framesProp = so.FindProperty("frames");
        if (framesProp != null && sprites.Count == FrameCount && NeedsFrames(framesProp))
        {
            framesProp.arraySize = FrameCount;
            for (int i = 0; i < FrameCount; i++)
                framesProp.GetArrayElementAtIndex(i).objectReferenceValue = sprites[i];
            report.AppendLine("- Frames diisi: primary_pintu1 (tertutup), 2, 3, 4 (terbuka penuh).");
        }
        else if (framesProp != null && !NeedsFrames(framesProp))
        {
            report.AppendLine("- Frames sudah terisi; tidak diubah.");
        }

        so.ApplyModifiedProperties();

        // 5. Sprite awal = frame tertutup (hanya bila renderer belum punya sprite)
        SpriteRenderer doorRenderer = rendererProp != null ? rendererProp.objectReferenceValue as SpriteRenderer : null;
        if (doorRenderer != null && doorRenderer.sprite == null && sprites.Count > 0)
        {
            Undo.RecordObject(doorRenderer, "Set Door Sprite");
            doorRenderer.sprite = sprites[0];
        }

        report.AppendLine();
        report.AppendLine("Pintu terbuka bila pemain mengenakan suit (oksigen cukup) atau sudah di luar, dan berada dalam Open Distance dari pusat sprite pintu (lingkaran biru muda di Scene view saat Door dipilih).");
        report.AppendLine("Cek juga: Order in Layer Door (bawaan 10) tidak tertutup Tilemap, dan SuitGate tetap berfungsi sebagai tembok.");
        report.AppendLine("Bila pintu belum terbuka saat Play: klik kanan komponen Door Sensor > Cek Status Pintu, lalu lihat Console.");
        report.AppendLine();
        report.Append("Tekan Ctrl+S untuk menyimpan scene.");

        Selection.activeGameObject = door.gameObject;
        EditorSceneManager.MarkSceneDirty(door.gameObject.scene);
        Debug.Log("MarsDoorSetup:\n" + report);
        EditorUtility.DisplayDialog("Mars Setup", report.ToString(), "OK");
    }

    private static bool NeedsFrames(SerializedProperty framesProp)
    {
        if (framesProp.arraySize == 0) return true;
        for (int i = 0; i < framesProp.arraySize; i++)
            if (framesProp.GetArrayElementAtIndex(i).objectReferenceValue == null) return true;
        return false;
    }

    // Satu file PNG bisa berisi beberapa sprite (Sprite Mode Multiple); ambil yang pertama.
    private static Sprite LoadFirstSprite(string path)
    {
        foreach (Object asset in AssetDatabase.LoadAllAssetsAtPath(path))
        {
            Sprite sprite = asset as Sprite;
            if (sprite != null) return sprite;
        }
        return null;
    }
}
#endif
