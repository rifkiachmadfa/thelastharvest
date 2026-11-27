#if UNITY_EDITOR
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

/// <summary>
/// Menu Editor (perapian): (1) panel yang masih bernama persis "SolarPanel" diberi nama "Solar Panel N" (urut dari
/// kiri ke kanan) agar pesan TARS dan UI menyebut panel yang rusak; (2) menghapus RepairPoint tambahan per panel yang
/// dibuat versi menu sebelumnya (anak SolarPanel bernama "RepairPoint" yang hanya berisi Collider2D + RepairPoint).
/// Satu RepairPoint melayani semua panel, jadi RepairPoint asli tidak disentuh. Aman dijalankan berulang kali.
/// Mars > Setup Solar Panel Names
/// </summary>
public static class MarsSolarRepairSetup
{
    [MenuItem("Mars/Setup Solar Panel Names")]
    public static void Setup()
    {
        StringBuilder report = new StringBuilder();

        List<SolarPanel> panels = new List<SolarPanel>(Object.FindObjectsByType<SolarPanel>(FindObjectsSortMode.None));
        if (panels.Count == 0)
        {
            EditorUtility.DisplayDialog("Mars Setup", "Tidak ada SolarPanel di scene.", "OK");
            return;
        }

        panels.Sort((a, b) =>
        {
            int byX = a.transform.position.x.CompareTo(b.transform.position.x);
            return byX != 0 ? byX : a.transform.position.y.CompareTo(b.transform.position.y);
        });

        int renamed = 0;
        for (int i = 0; i < panels.Count; i++)
        {
            if (panels[i].gameObject.name != "SolarPanel") continue;

            Undo.RecordObject(panels[i].gameObject, "Rename SolarPanel");
            panels[i].gameObject.name = "Solar Panel " + (i + 1);
            renamed++;
        }

        // Hapus RepairPoint anak panel buatan menu lama (hanya Transform + BoxCollider2D + RepairPoint).
        int removed = 0;
        foreach (SolarPanel panel in panels)
        {
            for (int c = panel.transform.childCount - 1; c >= 0; c--)
            {
                Transform child = panel.transform.GetChild(c);
                if (child.name != "RepairPoint") continue;
                if (child.GetComponent<RepairPoint>() == null || child.GetComponent<BoxCollider2D>() == null) continue;
                if (child.GetComponents<Component>().Length != 3 || child.childCount != 0) continue;

                Undo.DestroyObjectImmediate(child.gameObject);
                removed++;
            }
        }

        int remaining = Object.FindObjectsByType<RepairPoint>(FindObjectsSortMode.None).Length;
        report.AppendLine("- " + panels.Count + " Solar Panel di scene; " + renamed + " diganti nama menjadi \"Solar Panel N\".");
        report.AppendLine("- " + removed + " RepairPoint tambahan (buatan menu lama) dihapus.");
        report.AppendLine("- RepairPoint tersisa di scene: " + remaining + (remaining == 0 ? " (PERINGATAN: perlu minimal satu; jalankan Mars > Setup Interactive Objects)." : " (satu RepairPoint melayani semua panel)."));
        report.AppendLine();
        report.Append("Tekan Ctrl+S untuk menyimpan scene.");

        EditorSceneManager.MarkSceneDirty(panels[0].gameObject.scene);
        Debug.Log("MarsSolarRepairSetup:\n" + report);
        EditorUtility.DisplayDialog("Mars Setup", report.ToString(), "OK");
    }
}
#endif
