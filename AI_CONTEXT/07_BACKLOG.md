# Task Backlog

Prioritas harus disesuaikan setelah audit repository dan milestone tim.

## Urutan kerja tingkat tinggi
1. Stabilkan player movement, collision, sorting, dan directional animation.
2. Tetapkan interaksi dasar dan perpindahan tiga area.
3. Buat alur hari, tidur, HUD, dan pengelolaan resource.
4. Implementasikan Ice → Water dan Water + Energy → Habitat Oxygen.
5. Implementasikan suit dan aktivitas outdoor.
6. Implementasikan solar panel serta perbaikan kabel.
7. Implementasikan Potato farming dan siklus pertumbuhan.
8. Integrasikan TARS, tutorial, victory/game over.
9. Uji keseluruhan loop, balancing, performa, dan build.

## Aturan backlog
Setiap task baru harus punya: tujuan, prioritas, dependensi, kriteria selesai, dan cara tes. Hindari menambah fitur di luar GDD.

## Progres (2026-11-15)
- Sebagian selesai: langkah 2 (interaksi dasar; perpindahan tiga area belum).
- Belum: langkah 1 (animasi arah player masih bermasalah), 3 dan seterusnya.
- Milestone 1 (ResourceManager): selesai dan dites developer 2026-11-15.
- Milestone 2 (TimeManager + HUD): selesai, dilaporkan developer berjalan baik 2026-11-15.
- Milestone 3 (Bed, tidur, fade, pergantian hari): dibuat 2026-11-15, menunggu tes di Unity.
- Milestone A (Suit, Suit Station, gerbang keluar, oksigen suit): dibuat 2026-11-15, menunggu tes di Unity. Revisi GDD 2026-11-15: oksigen suit berkurang selama dikenakan; lepas suit di Suit Station dan otomatis saat tidur; pengingat TARS "lepas suit" (`SuitReminder`). Tertunda: dokumen GDD docx belum direvisi; peringatan oksigen suit 30%/10% (menunggu TARSSystem).
- Berikutnya (usulan, setelah Milestone A lolos tes): Milestone B = Ice gathering (interaksi tahan, inventory Ice terbatas) lalu Water Purifier; countdown darurat/Game Over oksigen suit bisa digabung dengan milestone Victory/Game Over. Usulan lama: Milestone 4 mulai Game Over/Victory dasar atau perpindahan area dalam satu scene (tiga area, kamera), lalu suit/outdoor. Kecil tapi tertunda: kunci gerakan Player saat dialog (kini ada `MovementEnabled`), animasi kiri/kanan Player.
- (Usulan lama, sudah dikerjakan): ResourceManager (stok Ice/Water/Oxygen/Energy/Food dengan event untuk HUD), lalu HUD dan timer hari.
- Objek interaktif (IceDeposit, WaterPurifier, OxygenGenerator, FarmingPlot, SolarPanel + RepairPoint): script dan menu setup dibuat 2026-11-15, menunggu tes di Unity. Berikutnya (usulan): tes loop Ice -> Water -> tanaman -> Food dan Energy -> Oxygen; minigame kabel menggantikan perbaikan langsung; pemicu kerusakan panel; `TARSSystem` (laporan panel rusak, reminder resource, 30%/10% suit); Food berkurang; Victory/Game Over; Day 1 tutorial. Keputusan terbuka di `08_DECISIONS.md` (saklar Oxygen Generator, deposit es tak terbatas, pemicu kerusakan panel).
- 2026-11-16: tes interaksi objek interaktif dilaporkan developer lolos semua (butir tidak dirinci; scene lokal belum di-commit). Milestone 6–9 dianggap selesai (dilaporkan). Berikutnya menunggu keputusan developer: urutan jadwal (milestone 10 minigame kabel) atau usulan AI (loop menang/kalah minimal dulu). Lihat `PHASE.md` bagian 6.
- 2026-11-16: Milestone 10 (pemicu kerusakan acak 1x per hari + minigame kabel) dibuat, menunggu tes di Unity. Berikutnya: tes M10, lalu `TARSSystem` (M11) yang mengambil alih pengumuman panel rusak, atau loop menang/kalah (M13) sesuai keputusan developer.
- 2026-11-16: Hunger dan makan (HungerSystem, FoodStorage, HungerReminder, HUD) dibuat, menunggu tes di Unity. Berikutnya: tes, Game Over kelaparan bersama milestone Victory/Game Over, simpan kenyang di SaveSystem, Day 1 tutorial menjelaskan makan.
- 2026-11-16: Tas pemain, hotbar (tombol 1-3), storage, bibit, dan makan dari tas dibuat, menunggu tes di Unity. Berikutnya: tes, Game Over kelaparan bersama milestone Victory/Game Over, simpan tas/storage/kenyang di SaveSystem, Day 1 tutorial menjelaskan tas dan storage.

- 2026-11-16: Resource Monitor dan Terminal komunikasi dihapus (permintaan developer). SAAT MEMULAI `TARSSystem` (M11) WAJIB sertakan `PHASE.md` butir 11.4: pesan Bumi harian lewat TARS, laporan panel/mesin/kenyang atas permintaan, pesan penutup Day 7. Tutorial Day 1 (12.1.1) tidak lagi menyebut Resource Monitor.
- 2026-11-17: Milestone 11 (`TARSSystem`, pesan Bumi, laporan dialog E) dibuat, menunggu tes di Unity. Berikutnya: tes M10 dan M11, developer menulis teks `earthMessages`, lalu loop menang/kalah (M13, termasuk pesan penutup Day 7 lewat `TARSSystem.Announce`) atau tutorial Day 1 (M12) sesuai keputusan developer.
- 2026-11-17: Tutorial Day 1 (M12: `TutorialManager`, 7 langkah dengan syarat USULAN, simpan/lewati lewat PlayerPrefs) dibuat, menunggu tes di Unity. Berikutnya: tes M10, M11, M12; developer menulis teks arahan dan `earthMessages`; lalu loop menang/kalah (M13) yang juga menutup butir 12.3.1.
- 2026-11-17: Garis arah tutorial (`TutorialGuideUI`, tujuan per langkah lewat `TutorialManager.GetGuideTarget`) dibuat, menunggu tes di Unity dan persetujuan desain (hanya Day 1). Berikutnya: tes, sesuaikan warna/ketebalan, putuskan apakah panah/garis diperlukan juga untuk panel rusak (di luar tutorial = perubahan GDD).
- 2026-11-17: Popup mengambang ikon + teks (`FloatingPopupManager`, `PHASE.md` 14.3.3) dibuat, menunggu tes di Unity. Berikutnya: developer menjalankan `Mars > Setup Floating Popups`, mengimpor ikon dan font piksel dari itch.io (14.3.4), menyetel ukuran; lalu opsional popup tambahan (Storage, Suit Station, Energy panel, tanaman matang) atas persetujuan developer.
- 2026-11-27: `ResourceDebugPanel` dihapus, HUD Kenyang pindah kiri atas, `HealthSystem` + panel HP dibuat (menunggu tes di Unity). Berikutnya: tes; putuskan Game Over lewat HP 0 atau batas kelaparan 60 dtk (bersama milestone Victory/Game Over); putuskan pemulihan HP; simpan HP di SaveSystem.
