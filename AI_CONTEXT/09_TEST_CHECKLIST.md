# Testing Checklist

Gunakan checklist ini saat fitur terkait sudah tersedia. Catat versi build, langkah reproduksi, hasil, dan bukti.

## Player & world
- [ ] Gerak 4 arah/diagonal sesuai desain dan tidak menembus wall.
- [ ] Animasi arah dan idle sesuai input terakhir.
- [ ] Kamera mengikuti player dengan benar.
- [ ] Sorting dan perpindahan area tampil benar.
- [ ] Interaksi hanya aktif pada objek/range yang sesuai.

## Survival loop
- [ ] Resource bertambah/berkurang sesuai aksi.
- [ ] Ice dapat diproses menjadi Water.
- [ ] Water dan Energy dikonsumsi mesin secara benar.
- [ ] Habitat Oxygen terpisah dari Suit Oxygen.
- [ ] Suit Oxygen hanya terkuras saat outdoor.
- [ ] Panel rusak mengurangi produksi, bukan mematikan semua panel.

## Farming & progression
- [ ] Potato tumbuh sesuai tahap dan aturan penyiraman.
- [ ] Tanaman tanpa air tidak maju tahap.
- [ ] Tidur menyimpan state dan memajukan hari.
- [ ] Hari 7 menghasilkan ending sesuai kondisi.
- [ ] Kondisi Victory/Game Over dan TARS terpicu benar.

## Quality
- [ ] Tidak ada error/exception di Console.
- [ ] Tidak ada missing reference/asset.
- [ ] Build berjalan pada target.
- [ ] Performa layak pada perangkat spesifikasi rendah.

## Interaksi & TARS (ditambahkan 2026-11-15)
Status [x] berarti dilaporkan developer, belum diverifikasi AI.
- [x] TARS berjalan acak dan tetap di dalam area jelajah.
- [ ] TARS tidak menembus dinding dan tidak macet di sudut.
- [x] Mendekati TARS memunculkan prompt "[E] Bicara dengan TARS".
- [x] Menekan E membuka dialog; TARS berhenti dan menghadap Player.
- [x] E melanjutkan baris dialog; setelah baris terakhir TARS berjalan lagi.
- [ ] E terakhir tidak membuka dialog baru secara langsung.
- [ ] Console tidak menampilkan error atau peringatan `TarsWanderer`/`DialogueUI`.
- [ ] Animasi TARS benar untuk arah kiri, kanan, dan atas.
- [ ] Animasi arah bawah (menunggu sprite Down).

## ResourceManager (ditambahkan 2026-11-15; dilaporkan developer lolos)
- [x] Play: panel uji muncul di kiri atas dengan 6 resource dan nilai awal usulan.
- [x] Tombol +10 pada Water: naik dan berhenti di kapasitas 30.
- [x] Tombol -10 pada Ice (stok 0): nilai tetap 0 dan Console menulis pesan "tidak cukup".
- [x] HabitatOxygen dan SuitOxygen berubah sendiri-sendiri.
- [x] Console tidak menampilkan error (khususnya "Dialogue Root belum diisi" atau missing script).

## TimeManager dan HUD (ditambahkan 2026-11-15; dilaporkan developer lolos, butir tidak dirinci)
- [x] Menu `Mars > Setup HUD and Time` jalan tanpa error; setelah itu ada objek `TimeManager` dan `Canvas/HudRoot`.
- [x] Play: HUD menampilkan "Day 1 / 7", timer 10:00 yang menghitung mundur, dan Food/Water/Oxygen/Energy sesuai nilai awal.
- [x] Tombol +/- di panel uji mengubah angka HUD seketika (Food, Water, Oxygen, Energy).
- [ ] (DIUBAH 2026-11-15; sebelumnya: timer berhenti saat dialog) Buka dialog TARS: timer HUD tetap berjalan selama dialog terbuka; stok resource yang sedang berubah (misalnya Suit O2) tetap berubah.
- [x] Klik kanan komponen TimeManager > Debug: Sisakan 5 detik: timer berhenti di 00:00 dan Console menulis "waktu hari 1 habis" sekali saja.
- [x] Klik kanan > Debug: Mulai hari berikutnya: HUD menjadi "Day 2 / 7" dan timer kembali 10:00.
- [x] Menjalankan menu setup kedua kali tidak membuat HUD ganda.
- [x] Console tidak menampilkan error atau peringatan HudUI/TimeManager.

## Bed, tidur, pergantian hari (ditambahkan 2026-11-15, belum dites di Unity)
- [ ] Menu `Mars > Setup Bed and Fade` jalan tanpa error; ada `Canvas/FadeOverlay` dan `Bed` (dengan anak `SpawnPoint`).
- [ ] Mendekati Bed memunculkan prompt "[E] Tidur".
- [ ] E di awal hari: dialog "Belum waktunya tidur. Sisa waktu hari ini: mm:ss." Setelah dialog ditutup, dialog tidak terbuka lagi sendiri.
- [ ] Setelah "Debug: Sisakan 5 detik" lalu E: layar memudar ke hitam, hari menjadi "Day 2 / 7", timer 10:00, Player muncul di SpawnPoint, layar memudar terang.
- [ ] Selama transisi Player tidak bisa digerakkan dan timer tidak berjalan; setelahnya gerakan normal dan timer mulai dari 10:00.
- [ ] Stok resource tidak berubah setelah tidur.
- [ ] Layar hitam tidak menghalangi klik/tombol setelah fade selesai.
- [ ] Day 7 setelah waktu habis: E memunculkan pesan "Hari terakhir telah selesai", hari tetap 7.
- [ ] Console tidak menampilkan error atau peringatan Bed/FadeUI.

## Suit, Suit Station, gerbang keluar (ditambahkan 2026-11-15, belum dites di Unity)
- [ ] Menu `Mars > Setup Suit and Gate` jalan tanpa error; ada `SuitSystem`, `SuitStation`, `IndoorZone`, `SuitGate`, dan `HudRoot/SuitPanel`. Menjalankannya dua kali tidak membuat objek ganda.
- [ ] Setelah IndoorZone menutupi interior: Play, Console tidak menampilkan peringatan "tidak ada IndoorZone" atau error SuitSystem/SuitGate/SuitStation.
- [ ] Panel "Suit O2" tidak tampil sebelum suit dikenakan.
- [ ] Dekati Suit Station tanpa suit: prompt "[E] Kenakan suit". E: dialog "Suit dikenakan. Oksigen suit berkurang selama suit dikenakan... Oksigen suit terisi ...". Panel Suit O2 muncul.
- [ ] Setelah E: HabitatOxygen turun dan SuitOxygen naik dengan jumlah sama (cek HUD Oxygen dan panel Suit O2; atau ResourceDebugPanel). Total oksigen tidak bertambah/hilang.
- [ ] Suit dikenakan dan tabung belum penuh: prompt "[E] Isi oksigen suit"; E mengisi sampai penuh. Setelah penuh, prompt menjadi "[E] Lepas suit".
- [ ] HabitatOxygen 0 dan tabung belum penuh: prompt "[E] Lepas suit" (tidak macet); E melepas suit.
- [ ] Tanpa suit (belum E di Suit Station): berjalan ke pintu terhalang; TARS menjelaskan perlu suit; pesan tidak muncul lagi sebelum 8 detik jika terus menabrak.
- [ ] Suit dikenakan tetapi oksigen suit < 20: pintu tetap terhalang dengan pesan oksigen minimum.
- [ ] Suit dikenakan, oksigen >= 20: pemain bisa keluar. Suit O2 berkurang sekitar 1 per detik, baik di dalam maupun di luar habitat.
- [ ] Teks Suit O2 putih, lalu kuning di <= 30%, merah di <= 10%.
- [ ] Di luar, oksigen suit turun di bawah 20: pemain tetap bisa masuk kembali ke habitat (tembok mati saat di luar).
- [ ] Buka dialog (dalam atau luar) saat suit dikenakan: oksigen suit tetap berkurang selama dialog terbuka (default); dengan `pauseDuringDialogue` dicentang di TimeManager: berhenti, lalu lanjut setelah dialog ditutup.
- [ ] Oksigen suit mencapai 0 di luar: Console menulis peringatan sekali (belum ada Game Over, itu disengaja). Mencapai 0 di dalam habitat: tidak ada peringatan; pemain bisa melepas suit.
- [ ] Tidur lewat Bed: stok oksigen suit tidak berubah.
- [ ] Lepas suit: E di Suit Station saat prompt "Lepas suit": dialog "Suit dilepas...", panel Suit O2 hilang, oksigen suit tidak berkurang lagi dan nilainya bertahan.
- [ ] Setelah suit dilepas, berjalan ke pintu: terhalang lagi (perlu mengenakan suit).
- [ ] Kenakan suit lagi: tabung diisi dari habitat hingga penuh (jika stok ada).
- [ ] Tidur lewat Bed saat suit dikenakan (Bed di dalam IndoorZone): oksigen suit tidak berkurang selama transisi; setelah bangun suit sudah dilepas (panel Suit O2 hilang), nilai oksigen tabung sama seperti sebelum tidur.
- [ ] Pengingat: kenakan suit, diam di dalam habitat 30 detik: dialog TARS "Anda masih mengenakan suit di dalam habitat..." muncul. Tutup dialog: pengingat berikutnya baru muncul setelah sekitar 90 detik jika suit masih dikenakan.
- [ ] Pengingat tidak muncul saat suit dilepas, saat pemain di luar, atau saat dialog lain sedang terbuka; hitungan mulai ulang dari 30 detik setelah suit dilepas lalu dikenakan lagi.
- [ ] Console tidak menampilkan error SuitReminder. Menu `Mars > Setup Suit and Gate` dijalankan ulang menambahkan SuitReminder tanpa duplikat.

## Dialog: input dan waktu (ditambahkan 2026-11-15, belum dites di Unity)
- [ ] Menu `Mars > Setup Dialogue and Prompt UI` dijalankan ulang: muncul tombol "X" di pojok kanan atas kotak dialog (`DialogueRoot/CloseButton`); menjalankannya lagi tidak menduplikasi.
- [ ] Dialog dengan beberapa baris (TARS): E, Spasi, dan klik kiri di area kosong layar masing-masing memajukan satu baris; pada baris terakhir menutup dialog.
- [ ] Klik tombol X menutup dialog langsung walau masih ada baris tersisa, dan TIDAK melompat ke baris berikutnya dulu (tidak terhitung ganda).
- [ ] Setelah dialog ditutup lewat Spasi/klik/X, TARS berjalan lagi (callback `Resume` jalan) dan dialog tidak terbuka ulang sendiri.
- [ ] Tombol yang membuka dialog (E) tidak langsung menutupnya pada frame yang sama.
- [ ] Selama dialog terbuka, timer HUD terus berkurang dan Food/Water/Oxygen/Energy tetap berjalan sesuai sistemnya.
- [ ] Dialog Bed, Suit Station, SuitGate, dan SuitReminder juga bisa ditutup dengan Spasi/klik/X. Tidak ada error di Console.

## Objek interaktif (ditambahkan 2026-11-15; dilaporkan developer lolos 2026-11-16)
Status [x] = dilaporkan developer 2026-11-16 (seluruh butir bagian ini lolos; butir tidak dirinci), belum diverifikasi AI. Scene hasil setup belum di-commit ke repo (revisi `d96d9b6f`). Butir bertanda "(BARU)" ditambahkan 2026-11-16 dan belum dites.
- [x] Objek interaktif ada di scene (IceDeposit, SolarPanel, RepairPoint, WaterPurifier, OxygenGenerator, FarmingPlot; ResourceMonitor dan CommunicationTerminal sudah dihapus) beserta `HudRoot/IcePanel`.
- [x] Console tidak menampilkan error kompilasi, missing reference, atau error dari script baru saat Play.
- [x] HUD menampilkan "Ice: 0 / 5" di bawah panel resource.

### Ice Deposit
- [x] Tanpa suit: E menampilkan pesan perlu suit; Ice tidak berubah.
- [x] Dengan suit: E membuat Player berhenti; prompt menghitung mundur; setelah sekitar 3 detik Ice +1 dan Player bisa bergerak lagi. HUD Ice berubah.
- [x] Ulangi sampai Ice 5 / 5: E berikutnya menampilkan pesan inventory penuh; Ice tidak melebihi 5.
- [x] Menekan E berulang saat mengumpulkan tidak menggandakan hasil.

### Water Purifier
- [x] Ice 0: pesan tidak ada Ice. Ice ada tetapi Energy < 5: pesan Energy tidak cukup.
- [x] Ice dan Energy cukup: E mengurangi 1 Ice dan 5 Energy seketika; setelah sekitar 4 detik Water +4 (maksimum 30). Prompt menampilkan sisa waktu selama proses.
- [x] E saat proses berjalan: pesan "sedang bekerja"; tidak ada pengurangan ganda.
- [x] Water hampir penuh (muat < 4): ditolak dengan pesan, Ice dan Energy tidak berkurang.
- [x] Indikator aktif (jika diisi) menyala hanya selama proses.

### Oxygen Generator
- [x] Dengan Water dan Energy cukup dan oksigen habitat belum penuh: oksigen habitat naik 5 tiap sekitar 5 detik sambil Water -1 dan Energy -2 per siklus. (Tes lama; sejak 2026-11-17 kenaikan 10 per siklus dan mesin hanya bekerja di bawah 40 persen, lihat bagian "Balancing hemat Water".)
- [x] Water habis atau Energy < 2: mesin berhenti; E menampilkan alasannya. Setelah stok ada lagi, mesin lanjut sendiri.
- [x] Oksigen habitat penuh: mesin siaga dan tidak memakai Water/Energy.
- [x] E mematikan mesin: Water/Energy tidak berkurang lagi; E lagi menyalakan kembali. Oksigen suit tidak tersentuh mesin.
- [ ] (BARU, USANG 2026-11-17) Mesin menyala sejak awal (`startOn`): dengan Energy cukup, Water awal 10 habis dalam sekitar 50 detik bila mesin tidak dimatikan. Perilaku ini adalah bug boros yang diperbaiki patch balancing 2026-11-17 (mesin kini siaga di atas 40 persen).

### Solar Panel dan Repair Point
- [x] Panel normal menambah Energy sekitar 0,5 per detik (naik tiap 1 detik, maksimum 100). Dua panel menambah dua kali lipat.
- [ ] (BARU) Catat selisih Energy: satu panel normal (0,5/dtk) hanya sedikit di atas konsumsi Oxygen Generator (2 Energy per 5 dtk = 0,4/dtk), dan panel rusak (0,125/dtk) tidak cukup menopangnya. Ini hitungan dari angka default; bahan balancing, bukan bug.
- [x] Klik kanan komponen SolarPanel > "Debug: Rusakkan panel": warna berubah kemerahan (atau sprite rusak jika diisi) dan produksi turun ke sekitar 25 persen. Fasilitas lain tetap jalan.
- [x] Repair Point saat panel normal: pesan panel normal. Saat rusak: E memperbaiki, warna kembali, produksi normal.
- [x] Repair Point tanpa field Panel terisi: Console menulis error yang jelas saat Start dan prompt tidak muncul (tidak bisa dipakai).

### Farming Plot
- [x] Petak kosong: prompt "Tanam kentang"; E menanam, warna menjadi hijau muda.
- [x] Siram: E mengurangi 2 Water dan menandai sudah disiram; E lagi hanya menampilkan pesan sudah disiram. Water < 2: pesan Water tidak cukup.
- [x] Tidur lewat Bed (atau Debug TimeManager "Mulai hari berikutnya") pada tanaman yang disiram: maju satu tahap dan status siram direset. Pada tanaman yang TIDAK disiram: tidak maju dan tidak mati.
- [x] Setelah mencapai tahap terakhir (3 tahap = dua pergantian hari dengan siram): prompt "Panen kentang"; E menambah Food 20 dan petak kosong lagi.
- [x] Food penuh: panen ditunda dengan pesan.

### Kerusakan panel acak dan minigame kabel (Milestone 10, BARU, belum dites)
Setup: jalankan `Mars > Setup Panel Damage and Wiring Minigame`, Ctrl+S, tekan Play. Untuk uji cepat: Inspector `SolarPanelDamageScheduler` > klik kanan > "Debug: Picu kerusakan sekarang"; sisakan waktu dengan menu TimeManager.
- [ ] Day 1 (`firstDamageDay` = 2): tidak ada panel yang rusak sendiri sepanjang hari.
- [ ] Day 2: tepat satu panel rusak pada waktu acak (sekitar 15-70 persen hari). Panel berubah merah, dialog TARS "... mengalami gangguan. Produksi energi menurun." muncul, dan tidak muncul lagi di hari yang sama.
- [ ] Tidur ke hari berikutnya: satu kejadian baru dijadwalkan; tidak lebih dari satu per hari. Bila semua panel sudah rusak, Console mencatat kejadian dilewati.
- [ ] Dialog lain terbuka saat panel rusak: pesan TARS menunggu sampai dialog tertutup (tidak menumpuk).
- [ ] Repair Point (panel rusak): E membuka minigame 4 kabel; konektor kanan acak; timer HUD berhenti dan Player tidak bisa bergerak; stok tidak berubah.
- [ ] Seret kabel ke konektor berwarna sama: garis menetap. Seret ke warna salah: pesan "Salah sambung", kabel kembali, bisa diulang. Lepas di tempat kosong: kabel kembali.
- [ ] Semua kabel benar: panel kembali normal (warna dan produksi Energy), minigame menutup, timer lanjut, Player bisa bergerak, dialog "Solar Panel diperbaiki".
- [ ] Esc saat minigame: menutup, panel tetap rusak, timer lanjut, gerak pulih.
- [ ] Prompt Repair Point saat panel normal: pesan "berfungsi normal", minigame tidak terbuka.
- [ ] Catat durasi nyata satu minigame (target 15-30 detik) dan apakah kabel/konektor terbaca jelas pada resolusi Game view yang dipakai.
- [ ] Hapus komponen `WiringMinigameUI` sementara: Console memberi peringatan dan Repair Point memperbaiki langsung (cadangan).

### Resource Monitor dan Terminal (DIHAPUS 2026-11-16; butir di bawah tinggal arsip, jangan dites lagi)
- [x] Resource Monitor: dialog berisi hari/waktu, stok semua resource (nilai sama dengan HUD), status suit, kondisi panel (jumlah normal/rusak), Water Purifier, Oxygen Generator. Fasilitas yang tidak ada di scene tidak muncul. Tidak mengubah stok apa pun.
- [x] Terminal: E menampilkan pesan Day 1; setelah tidur, E menampilkan pesan hari berikutnya; tidak ada pesan yang berisi tugas atau mengubah stok.
- [x] Semua dialog objek baru bisa ditutup dengan E/Spasi/klik/X dan prompt tidak muncul ulang sendiri.

## Hunger dan makan (ditambahkan 2026-11-16, direvisi: makan dari tas; BARU, belum dites di Unity)
Setup: jalankan `Mars > Setup Hunger and Food Storage` dan `Mars > Setup Inventory and Storage`, pindahkan objek Storage ke Control Room, ubah Food di Inspector ResourceManager (Start 10, Max 50), simpan scene.
- [ ] HUD menampilkan "Kenyang: 100 / 100" di kanan atas dan turun pelan saat bermain (sekitar 1 poin per 12 detik).
- [ ] Kenyang tidak berkurang saat transisi tidur.
- [ ] Kentang di tas, kenyang di bawah 100: tekan 1 lalu E mengurangi kentang 1 dan menambah kenyang 12,5. Kenyang penuh: E tidak makan (prompt "Makan kentang" tidak muncul).
- [ ] Debug: ContextMenu "Kenyang jadi 0" pada HungerSystem. Dialog TARS muncul di 30%, 10%, dan 0 (sekali tiap batas); setelah makan, batas bisa diumumkan lagi.
- [ ] Kenyang 0 selama 60 detik tanpa makan: Console menampilkan peringatan kelaparan (Game Over belum ada). Makan sebelum 60 detik membatalkan hitung mundur.
- [ ] Suit Station: prompt hanya "Kenakan suit" lalu "Lepas suit" (tidak ada "Isi oksigen suit"); suit selalu bisa dilepas di dalam habitat dan tabung terisi otomatis; di luar habitat suit tidak bisa dilepas.

## Tas pemain, storage, bibit (ditambahkan 2026-11-16, BARU, belum dites di Unity)
- [ ] Hotbar tiga slot di bawah tengah layar: "1 Kentang 0 / 10", "2 Es 0 / 10", "3 Bibit 0 / 5".
- [ ] Tombol 1, 2, 3 (dan keypad) menyalakan slot; menekan tombol yang sama lagi mematikannya; tidak bisa memilih saat dialog atau panel storage terbuka.
- [ ] Storage: E membuka panel; Ambil 1 / Ambil Semua memindahkan ke tas sampai batas tas; Simpan 1 / Simpan Semua memindahkan ke storage; angka tas, storage, dan Food di HUD (kentang) konsisten; Esc atau Tutup menutup; Player tidak bisa bergerak selama terbuka; waktu hari tetap berjalan.
- [ ] Ambil Semua bibit dari storage: tas terisi maks 5, sisanya tetap di storage. Ambil Semua es: maks 10.
- [ ] Ice Deposit: pakai suit, E mengumpulkan es ke tas (hotbar naik). Tas es penuh (10): pesan "Tas penuh", es tidak bertambah; setelah Simpan di storage bisa mengumpulkan lagi.
- [ ] Water Purifier: tanpa slot terpilih E menampilkan petunjuk "Pilih es"; dengan slot 2 terpilih prompt "Masukkan es ke Water Purifier", E memakai 1 es dari tas + Energy dan menghasilkan Water; tas tanpa es menampilkan pesan.
- [ ] Farming Plot: petak kosong tanpa bibit terpilih menampilkan petunjuk; slot 3 terpilih + E menanam dan mengurangi bibit 1; tanpa bibit di tas ada pesan. Siram dan tumbuh seperti sebelumnya.
- [ ] Panen: 4 kentang masuk tas dan bibit +1; bila ruang tas kentang kurang dari 4, panen ditunda dengan pesan dan tanaman tetap; bibit masuk storage bila tas bibit penuh.
- [ ] Loop lengkap: ambil bibit dari storage, tanam, siram, tidur sampai matang, panen ke tas, simpan atau makan.
- [ ] Interaksi lain (Bed, Suit Station, Repair Point) tetap berfungsi dengan E tanpa slot terpilih; dengan kentang terpilih dan kenyang penuh, E tetap berinteraksi biasa.
- [ ] HUD tidak lagi menampilkan panel Ice lama.

## Konsumsi oksigen habitat (ditambahkan 2026-11-16, BARU, belum dites di Unity)
- [ ] Tekan Play tanpa menambah objek ke scene: di Hierarchy muncul objek `HabitatOxygenDrain` (dibuat otomatis).
- [ ] Matikan Oxygen Generator (E di mesin). Oksigen habitat di HUD turun sekitar 3 unit tiap 60 detik (0,05/detik, usulan).
- [ ] Nyalakan lagi generator dengan Water dan Energy cukup: oksigen habitat naik kembali (+10 per 5 detik sejak 2026-11-17; sebelumnya +5) sampai penuh, lalu mesin siaga; sejak 2026-11-17 mesin baru bekerja lagi setelah oksigen habitat turun ke 40 persen atau kurang (bukan setelah turun sedikit).
- [ ] Transisi tidur (layar hitam): oksigen habitat tidak berkurang selama jeda.
- [ ] Dengan generator mati dan oksigen habitat dibuat 0 (ResourceDebugPanel): Console menampilkan peringatan "oksigen habitat habis" sekali; nilai tetap 0 dan tidak negatif; tidak ada Game Over.
- [ ] Isi suit di Suit Station saat oksigen habitat 0: pesan "Oksigen habitat tidak cukup untuk mengisi suit".
- [ ] Setelah generator mengisi lagi (>0) lalu habis lagi, peringatan muncul sekali lagi (bukan berulang tiap detik).
- [ ] Oksigen suit tidak ikut berkurang oleh script ini (hanya berkurang oleh `SuitSystem` saat suit dikenakan).
- [ ] Satu panel rusak: catat apakah Energy cukup menjaga generator; bahan balancing, bukan bug.

## Penghapusan Resource Monitor dan Terminal (2026-11-16; BARU, belum dites di Unity)
- [ ] Sebelum patch: GameObject `ResourceMonitor` dan `CommunicationTerminal` sudah dihapus dari Hierarchy dan scene disimpan.
- [ ] Setelah patch: Unity mengimpor tanpa error kompilasi; Console tidak memberi peringatan "missing script" saat Play.
- [ ] Menu `Mars > Setup Interactive Objects` dijalankan: tidak membuat ResourceMonitor/CommunicationTerminal; objek lain tidak berubah.
- [ ] Interaksi lain (Bed, Suit Station, Storage, Water Purifier, Oxygen Generator, Repair Point) tetap berfungsi dengan E.

## TARSSystem (2026-11-17; BARU, belum dites di Unity)
- [ ] Menu `Mars > Setup TARS System` dijalankan: objek `TARSSystem` dibuat, `TarsWanderer` terhubung; dijalankan lagi tidak membuat duplikat. Unity mengimpor tanpa error kompilasi.
- [ ] Tekan Play: pesan "Hari 1 dari 7 dimulai" + pesan Bumi (placeholder) muncul sekali; `SuitReminder` tidak lagi memunculkan pengingat ganda (Console tidak ada pengingat dari dua sumber).
- [ ] Turunkan Water (ResourceDebugPanel) sampai di bawah 30 persen: satu pesan "menipis"; sampai 10 persen: pesan "sangat rendah"; sampai 0: pesan "habis". Tiap tingkat hanya muncul sekali, tidak berulang tiap detik. Naikkan lagi di atas 30 persen lalu turunkan: pesan muncul lagi setelah cooldown (45 dtk).
- [ ] Ulangi untuk Oxygen habitat, Energy, dan stok Food di storage; teks petunjuk sesuai resource.
- [ ] Kenakan suit, oksigen suit turun ke 30 persen lalu 10 persen: pesan di dalam habitat menyarankan melepas suit, di luar habitat menyarankan kembali. Lepas suit: pesan oksigen suit tidak muncul lagi.
- [ ] Suit dikenakan di dalam habitat 30 detik: pengingat "lepas suit" muncul (dari `TARSSystem`), lalu ulang tiap 90 detik; tidak muncul bila suit dilepas.
- [ ] Panel rusak (menu konteks `Debug: Picu kerusakan` pada scheduler, atau tunggu kejadian acak Day 2+): satu pesan TARS menyebut nama panel dan Repair Point; setelah diperbaiki sebelum pesan tampil, pesan dibuang.
- [ ] Banyak resource rendah sekaligus: pesan tampil satu per satu, Critical lebih dulu, tidak ada dialog yang saling menimpa; pesan tidak muncul saat dialog lain, panel storage, minigame kabel, atau transisi tidur sedang berjalan, dan tampil setelahnya.
- [ ] Hari: saat sisa waktu <= 60 dtk muncul pengingat akhir hari (varian "masih di luar habitat" bila di luar); saat waktu habis muncul pesan "waktu habis"; setelah tidur, pesan "Hari N" + pesan Bumi muncul setelah fade selesai.
- [ ] Lapar (`HungerReminder`): pesan 30 persen/10 persen/0 tetap muncul, kini lewat antrean TARS; sisa waktu kelaparan di teks adalah nilai terbaru saat tampil.
- [ ] Dialog E pada TARS: urutan pengantar, pesan Bumi hari ini, laporan fasilitas (jumlah panel normal/rusak, Energy per detik, status Water Purifier dan Oxygen Generator), kondisi (suit, kenyang), stok dan sisa waktu. Tidak ada quest atau perintah urutan aksi.
- [ ] Menekan E untuk menutup dialog TARS spontan di dekat NPC TARS tidak langsung membuka dialog TARS baru.
- [ ] Hapus objek `TARSSystem` dari scene: game tetap jalan; `SuitReminder`, `HungerReminder`, dan scheduler kembali memakai dialog langsung.

## Tutorial Day 1 (2026-11-17; BARU, belum dites di Unity)
Siapkan: jalankan `Mars > Setup Tutorial`. Untuk mengulang dari awal, klik kanan `TutorialManager` > `Debug: Hapus status tutorial tersimpan`.
- [ ] Unity mengimpor `Assets/Script/Tutorial/*`, `MarsTutorialSetup`, dan `TarsWanderer` tanpa error kompilasi; menu setup dijalankan ulang tidak membuat duplikat.
- [ ] Play di Day 1: arahan Intro (PLACEHOLDER) muncul lewat TARS setelah dialog lain tertutup. Tidak ada gerak yang terkunci dan tidak ada urutan wajib. Console tidak berisi peringatan "tidak ada" untuk objek yang ada di scene.
- [ ] Intro: bicara dengan TARS (E) saja belum cukup; buka Storage juga: arahan Suit muncul. Urutan sebaliknya sama.
- [ ] Suit: kenakan suit di Suit Station: arahan CollectIce muncul. Melepas suit tidak memundurkan langkah.
- [ ] CollectIce: kumpulkan es di Ice Deposit; garis tetap ke Ice Deposit sampai 5 es terkumpul, lalu arahan PurifyWater muncul dan garis pindah ke Water Purifier. Mengambil es dari storage di dalam habitat TIDAK dihitung. Bila tas es penuh sebelum 5 (kapasitas lebih kecil), langkah tetap selesai. Tes ulang kasus bug: tas penuh tidak lagi membuat garis terus ke Ice Deposit.
- [ ] PurifyWater: masukkan es ke Water Purifier dan tunggu selesai: arahan RemoveSuit ("Lepas suit") muncul (sejak 2026-11-17; sebelumnya Farming) dan garis pindah ke Suit Station.
- [ ] (BARU 2026-11-17) RemoveSuit: sebelum suit dilepas, arahan tidak maju walau pemain menanam/menyiram (syarat Farming dikunci). E di Suit Station ("Lepas suit"): arahan Farming muncul (atau dilewati bila sudah terpenuhi). Suit dilepas lebih awal (sebelum langkah ini aktif): langkah dilewati tanpa arahan. Di Inspector `TutorialManager > Steps` ada 8 elemen dan elemen ke-4 berjudul "Lepas suit" (id 7).
- [ ] Farming: tanam bibit dan siram: arahan OxygenGenerator muncul.
- [ ] OxygenGenerator: dekati mesin sampai prompt "[E] ..." muncul: arahan Sleep muncul.
- [ ] Sleep: tidur ke Day 2: tutorial selesai dan tidak ada arahan di Day 2; Console tidak error.
- [ ] Urutan bebas: kerjakan Farming dan Suit sebelum Intro selesai; setelah Intro selesai langkah yang sudah terpenuhi dilewati berurutan tanpa arahan yang menumpuk.
- [ ] Arahan yang belum terbaca diulang kira-kira tiap 120 dtk selama langkahnya belum selesai (tidak lebih sering dari cooldown TARS).
- [ ] Setelah tutorial selesai penuh, jalankan Play lagi (tanpa menghapus status): TARS memberi tahu tutorial dapat dilewati; Tab melewatinya tanpa error. Tab tidak berpengaruh bila status belum tersimpan. Tutorial yang dilewati tidak menyimpan status.
- [ ] `Debug: Selesaikan langkah saat ini`, `Debug: Lewati tutorial`, dan centang `Skip Tutorial` bekerja tanpa error.
- [ ] Hapus objek `TutorialManager`: game tetap berjalan. Hapus objek `TARSSystem`: arahan masuk Console, tidak ada error.

## Tutorial: garis arah (2026-11-17; BARU, belum dites di Unity)
Siapkan: jalankan ulang `Mars > Setup Tutorial` (memasang `TutorialGuideUI` di Canvas); Console tidak berisi error kompilasi.
- [ ] Play di Day 1: garis berwarna dari tubuh Player ke Storage (langkah Intro) dengan berlian di ujung Storage; garis ikut bergerak saat Player dan kamera bergerak, tidak bergetar atau tertinggal terlihat.
- [ ] Panel Storage terbuka: garis hilang; setelah ditutup dan Storage sudah dibuka, garis pindah ke TARS (bila belum diajak bicara), lalu ke Suit Station setelah Intro selesai.
- [ ] Mendekati tujuan (dalam sekitar 1,5 satuan): garis hilang. Menjauh lagi: muncul.
- [ ] CollectIce: garis ke Ice Deposit terdekat. PurifyWater: ke Ice Deposit bila tas tanpa es; setelah punya es berpindah ke Water Purifier.
- [ ] Farming: tanpa bibit di tas garis ke Storage; setelah ambil bibit ke petak kosong; setelah tanam, ke petak itu sampai disiram.
- [ ] OxygenGenerator dan Sleep: garis ke Oxygen Generator dan Bed.
- [ ] Day 2 (atau `Debug: Lewati tutorial`): garis hilang dan tidak muncul lagi. Garis tampil di bawah HUD, dialog, dan layar fade.
- [ ] Ubah resolusi/ukuran jendela Game: ujung garis tetap tepat di Player dan tujuan (Canvas Scale With Screen Size).
- [ ] Hapus komponen `TutorialGuideUI`: tutorial tetap berjalan tanpa garis dan tanpa error.

## Balancing hemat Water dan oksigen suit (2026-11-17; BARU, belum dites di Unity)
- [ ] Inspector `OxygenGenerator`: `Oxygen Per Cycle` = 10 dan field baru `Restart Below Fraction` = 0,4. Inspector `SuitSystem`: `Drain Per Second` = 0,2. Bila Inspector masih menampilkan 5 / 1, scene lokal belum memakai nilai baru: ubah manual.
- [ ] Mulai game dengan Water 20, oksigen habitat 50, Energy cukup. Tanpa menyentuh apa pun: Water TIDAK berkurang selama oksigen habitat di atas 40 persen. Di HUD oksigen habitat turun pelan (sekitar 3 per 60 detik).
- [ ] Oksigen habitat penuh (100) lalu turun ke 99: generator TIDAK menyala dan Water tidak berkurang (bug lama: mesin langsung memakai 1 Water).
- [ ] Oksigen habitat turun ke 40 atau kurang (misalnya lewat ResourceDebugPanel atau isi suit): generator menyala, oksigen +10 per 5 detik dengan Water -1 dan Energy -2 per siklus, terus sampai penuh, lalu siaga. Siklus terakhir memakai Water kurang dari 1 (sebanding), bukan 1 penuh.
- [ ] E pada Oxygen Generator saat siaga: pesan menyebut "di bawah 40 persen" dan status "siaga, oksigen habitat masih cukup".
- [ ] Water habis saat oksigen habitat di bawah 40 persen: status "berhenti, Water tidak cukup"; setelah Water ada, mesin lanjut sendiri.
- [ ] Kenakan suit (tabung terisi dari habitat): oksigen suit turun 0,2 per detik (sekitar 12 per menit); tabung 100 bertahan sekitar 500 detik. Pemain sempat ke Ice Deposit, mengumpulkan es, dan kembali tanpa terburu-buru.
- [ ] Catat untuk balancing: total Water terpakai untuk oksigen dalam satu hari penuh (600 detik) dengan pola bermain normal, dan Water tersisa untuk menyiram tanaman. Target usulan: oksigen memakai sebagian kecil Water, bukan seluruhnya.

## Tutorial: makan, tidur, dan dialog (2026-11-17; BARU, belum dites di Unity)
Siapkan: hapus status tutorial tersimpan (klik kanan `TutorialManager` > `Debug: Hapus status tutorial tersimpan`). Inspector `TutorialManager` > Steps harus berisi 9 elemen (Eat ID 8 sebelum Sleep ID 6); bila scene lama, cek manual.
- [ ] Eat: setelah OxygenGenerator, TARS mengarahkan makan. Tas tanpa kentang: garis menuju Storage; tas berisi kentang: tidak ada garis. Makan kentang (tombol 1, lalu E): langkah maju ke Sleep.
- [ ] Eat dikunci: makan kentang SEBELUM langkah Eat dimulai, langkah Eat dilewati saat tiba gilirannya.
- [ ] Sleep: selesaikan semua langkah sebelum waktu habis: TIDAK ada dialog tidur dan TIDAK ada garis ke Bed selama timer masih berjalan (tunggu 120 detik pengingat: tetap tidak muncul).
- [ ] Waktu hari habis (gunakan `TimeManager` > `Debug: Sisakan 5 detik`): dialog TARS tampil dulu; garis ke Bed baru muncul setelah semua dialog tertutup.
- [ ] Bila waktu sudah habis sebelum langkah Sleep tiba (mis. langkah lain selesai terlambat): saat Sleep jadi aktif, dialog tidur tampil lalu garis menyusul.
- [ ] Dialog dan garis tidak pernah tampil bersamaan: di setiap langkah, garis hilang saat dialog TARS terbuka (termasuk dialog E pada TARS NPC) dan muncul lagi setelah ditutup; tidak ada kedipan saat pesan TARS mengantre.
- [ ] Tidur ke Day 2: tutorial selesai, tidak ada garis atau arahan di Day 2; Console tidak error.

## Farming waktu nyata (2026-11-17; BARU, belum dites di Unity)
Siapkan: Inspector `FarmingPlot` memakai nilai bawaan (grow 60 sampai 120 detik, kentang 2 sampai 4, bibit 1 sampai 2). Tidak perlu tidur untuk menguji.
- [ ] Tanam bibit, siram SEKALI (Water berkurang 2): prompt berubah menjadi "Periksa tanaman (N%)" dan N naik terus tanpa pindah hari.
- [ ] Tanaman melewati tahap visual (warna hijau muda ke hijau) lalu matang antara 60 dan 120 detik setelah disiram; prompt menjadi "Panen kentang". Ulangi beberapa kali: durasi bervariasi.
- [ ] Tanaman belum disiram: tidak tumbuh, tidak mati (tunggu lebih dari 2 menit).
- [ ] Menyiram lagi saat tumbuh: pesan "sudah disiram", Water tidak berkurang.
- [ ] Tidur (ganti hari) saat tanaman tumbuh: timer berhenti selama transisi tidur dan lanjut setelahnya; tidak ada lompatan tahap.
- [ ] Panen: kentang masuk tas sebanyak 2 sampai 4 dan bibit 1 sampai 2 (bibit ke storage bila tas bibit penuh); petak kosong lagi. Ulangi beberapa kali: jumlah bervariasi.
- [ ] Tas kentang hampir penuh: panen ditolak dengan pesan ruang kurang; jumlah hasil TIDAK berubah setelah dicoba berulang (diundi saat disiram); setelah simpan ke storage, panen berhasil.
- [ ] Tutorial langkah Farming selesai setelah tanam dan siram (tidak perlu menunggu matang); teks arahan baru tampil.

## Popup mengambang ikon + teks (2026-11-17; BARU, belum dites di Unity)
Siapkan: jalankan `Mars > Setup Floating Popups`, simpan scene. Ikon bawaan placeholder tampil bila slot sprite kosong.
- [ ] Console bersih dari error saat Play; tanpa objek `FloatingPopupManager`, hanya satu peringatan dan game tetap jalan.
- [ ] Ice Deposit: setelah pengumpulan selesai muncul ikon es + "Es +1" di atas deposit, naik dan memudar; tas es penuh: tidak ada popup.
- [ ] Farming Plot: tanam = "Bibit -1" (ikon bibit); siram = "Water -2" (ikon water); panen = "Kentang +N" dan "Bibit +N" bertumpuk, tidak saling menimpa; panen ditunda karena tas penuh = tidak ada popup.
- [ ] Water Purifier: saat mulai "Es -1" dan "Energy -5" bertumpuk; setelah proses selesai "Water +4". Mulai ditolak (Es/Energy kurang) = tidak ada popup.
- [ ] Oxygen Generator: tiap siklus kerja muncul "Oxygen +10" (siklus terakhir bisa kurang dari 10); saat siaga atau mati tidak ada popup; centang `Show Cost Popups` menambah "Water -1" dan "Energy -2".
- [ ] Repair Point: setelah minigame sukses muncul "Panel diperbaiki" di atas Solar Panel.
- [ ] Makan (tombol 1, lalu E): "Kentang -1" dan "Kenyang +N" di atas Player; makan saat kenyang penuh = tidak ada popup.
- [ ] Teks terbaca di kamera Cinemachine dan tidak tertutup Tilemap (urutan gambar); ikon dan teks sejajar di tengah titik popup; atur `Character Size` / `Icon World Size` bila perlu.
- [ ] Popup tetap bergerak dan memudar saat layar tidur (timer berhenti); tidak ada popup yang tersangkut setelah lama bermain.
- [ ] Mengganti sprite ikon dan font di Inspector berlaku pada popup berikutnya.
- [ ] Label petak: petak kosong tanpa label; setelah tanam muncul "Belum disiram (M:SS)" (kuning) dengan durasi 1:00 sampai 2:00; setelah siram berubah "Matang M:SS" dan menghitung mundur; selesai "Siap panen!" (hijau); setelah panen label hilang.
- [ ] Hitung mundur berhenti saat timer berhenti (tidur, minigame) dan lanjut sesudahnya; angka cocok dengan waktu matang sebenarnya (selisih maksimal 1 detik).
- [ ] Popup "Water -2" / "Kentang +N" di petak tidak menimpa label; beberapa petak berdampingan tidak saling menimpa label.

## Panel mesin: Water Purifier, Oxygen Generator, Solar Panel (2026-11-18; BARU, belum dites di Unity)
Siapkan: jalankan `Mars > Setup Machine Panels`, simpan scene. Pastikan tidak ada es terpilih di tas (tekan slot sekali lagi bila perlu).
- [ ] Console bersih dari error saat Play. Dekati tiap mesin: prompt "Buka panel ..."; E membuka panel di tengah layar, Player tidak bisa berjalan, Esc dan tombol Tutup menutup panel dan gerak pulih.
- [ ] Water Purifier tanpa es di tas: tombol PROSES nonaktif dan alasan "Tidak ada es di tas" tampil. Dengan es dan Energy cukup: PROSES aktif; klik memulai proses: es di tas berkurang, Energy berkurang, bar proses naik dan sisa detik turun, lalu Water bertambah di bar tangki dan popup "Water +4" muncul.
- [ ] Water Purifier: Energy kurang atau tangki Water penuh = PROSES nonaktif dengan alasan yang sesuai; klik saat sedang memproses tidak memulai proses kedua.
- [ ] Pilih es (tombol 2) lalu E di dekat Water Purifier: jalan pintas lama masih memproses dan menampilkan dialog (tutorial langkah PurifyWater tetap selesai).
- [ ] Oxygen Generator: status berwarna (hijau bekerja, kuning siaga, merah mati/berhenti); bar Oxygen habitat, Water, Energy ikut berubah saat mesin bekerja. NYALAKAN/MATIKAN mengubah status dan teks tombol; mesin mati tidak memakai Water/Energy; Allow Manual Toggle dimatikan di Inspector = tombol nonaktif berlabel OTOMATIS.
- [ ] Solar Panel normal: status NORMAL, output panel dan total sama dengan nominal, tombol PERBAIKI nonaktif. Rusakkan panel (menu konteks Debug pada SolarPanel) saat panel terbuka: status berubah RUSAK, output turun, PERBAIKI aktif.
- [ ] PERBAIKI: panel menutup dan minigame kabel terbuka; berhasil = panel terbuka lagi dengan status NORMAL dan pesan "Panel diperbaiki"; Esc di minigame = kembali bermain tanpa panel (panel tetap rusak).
- [ ] Saat panel terbuka: E tidak memicu interaksi lain, tombol tas 1-3 tidak berfungsi (tidak makan kentang), pesan TARS tidak muncul (muncul setelah panel ditutup), dialog jadwal kerusakan panel menunggu.
- [ ] Teks dan tombol terbaca, tidak terpotong, di resolusi 1920x1080 dan jendela Game yang lebih kecil; panel tampil di atas HUD.
- [ ] Hapus komponen `MachinePanelUI` dari Canvas (uji saja): semua mesin kembali memakai dialog dan tidak ada error.
## Skin HUD Pixel UI Sci-Fi (2026-11-18; BARU, belum dites di Unity)
Siapkan: jalankan `Mars > Setup HUD and Time` (dan Suit/Hunger bila dipakai), lalu `Mars > Apply HUD Skin (Pixel UI Sci-Fi)`, simpan scene.
- [ ] Console bersih dari error/warning saat menjalankan menu dan saat Play. Menu dijalankan dua kali: tidak ada objek ganda (satu `FoodIcon`, satu `FoodBar` per baris).
- [ ] Sprite di `Assets/UI/PixelUISciFi/` terimpor sebagai Sprite, Filter Point, tanpa kompresi; panel dan latar bar tampil tajam (tidak blur) dan border tidak melar.
- [ ] Di Game view 1920x1080: TimePanel (Day + timer), ResourcePanel (4 baris ikon + teks + bar), HungerPanel di bawahnya, SuitPanel kiri bawah tampil tanpa tumpang tindih dan teks tidak terpotong.
- [ ] Bar Food/Water/Oxygen/Energy ikut berubah saat stok berubah (ambil es, proses Water, panen, Energy turun) dan sama dengan angka di teks; bar kosong menyembunyikan isi, bar penuh memenuhi latar.
- [ ] Bar Kenyang turun seiring waktu dan naik saat makan; teks tetap berwarna kuning/merah di ambang peringatan.
- [ ] SuitPanel hanya tampil saat suit dikenakan; bar Suit O2 turun di luar dan naik saat isi ulang.
- [ ] Jendela Game lebih kecil / rasio lain (mis. 1280x720, 16:10): panel tetap di sudutnya dan terbaca.
- [ ] Hapus semua field bar di Inspector `HudUI` (uji saja): HUD tetap berfungsi dengan teks saja, tanpa error.

## HP, HUD kenyang kiri atas, hapus debug panel (2026-11-27; BARU, belum dites di Unity)
- [ ] Setelah menyimpan scene: tidak ada panel debug +/- resource di kiri atas Game view; tidak ada error kompilasi atau komponen hilang di GameObject `ResourceManager`.
- [ ] Panel Kenyang tampil di kiri atas; panel HP tepat di bawahnya (jalankan `Mars > Setup Health and HP HUD` dulu); panel resource tetap di kanan atas, panel Day/timer tetap di tengah atas.
- [ ] Play: HP awal 100 / 100 dan tidak berubah selama kenyang > 0.
- [ ] Buat kenyang 0 (ContextMenu `Debug: Kenyang jadi 0` pada HungerSystem): sekitar 2 detik kemudian HP turun 3, lalu 3 lagi tiap 2 detik; teks HP kuning di bawah 50 persen dan merah di bawah 25 persen.
- [ ] Makan kentang saat HP sudah turun: HP berhenti berkurang. Bila kenyang naik di atas 80 persen, HP naik +2 tiap 2 detik sampai penuh; di bawah 80 persen HP tidak naik.
- [ ] Lapar lagi setelah makan: pengurangan HP mulai lagi dari hitungan 2 detik penuh (bukan langsung).
- [ ] Saat timer berhenti (transisi tidur/minigame): HP tidak berkurang.
- [ ] HP sampai 0: Console menampilkan peringatan "HP habis" satu kali; HP tidak negatif; tidak ada Game Over.

## Solar Panel: satu RepairPoint melayani semua panel (2026-11-27; BARU, belum dites di Unity)
- [ ] (Opsional) `Mars > Setup Solar Panel Names`: panel diberi nama Solar Panel 1..N; RepairPoint tambahan buatan menu lama terhapus; RepairPoint asli tetap ada.
- [ ] Rusakkan panel MANA SAJA (klik kanan komponen `SolarPanel` > Debug: Rusakkan panel), datangi RepairPoint, tekan E: panel mesin menampilkan panel rusak itu (status RUSAK), tombol PERBAIKI aktif; selesaikan minigame; panel jadi NORMAL.
- [ ] Rusakkan dua panel: setelah memperbaiki yang pertama, panel yang sama tetap terbuka dengan pesan "Masih ada 1 panel rusak" dan menawarkan panel rusak berikutnya; prompt E memuat jumlah rusak.
- [ ] Pesan TARS "Solar Panel N mengalami gangguan..." muncul dan tidak hilang sebelum tampil.

## Tutorial bercerita (2026-11-28; BARU, belum dites di Unity)
Siapkan: Play dari Day 1; klik kanan `TutorialManager` > `Debug: Hapus status tutorial tersimpan` bila perlu. Tidak perlu menjalankan menu setup.
- [ ] Console tanpa error kompilasi; teks arahan tidak lagi berawalan `[PLACEHOLDER]`; `TutorialManager > Use Built In Story` tercentang.
- [ ] Urutan arahan: Intro, Suit, CollectIce, PurifyWater, RemoveSuit, Hungry, Farming, OxygenGenerator, Harvest, Eat, Recap; Sleep hanya muncul setelah timer habis.
- [ ] Hungry: setelah suit dilepas, bar Kenyang turun ke sekitar 32 persen dan TARS menjelaskan kenyang, HP, kentang; arahan Farming muncul setelah dialog Hungry ditutup (tidak sebelum).
- [ ] Hungry dilewati tanpa dialog bila pemain sudah menanam dan menyiram sebelum langkah ini.
- [ ] Pengingat lapar standar TARS (30 persen) mungkin muncul sesudahnya; itu normal.
- [ ] Harvest: garis arah ke petak bertanaman; setelah panen arahan Eat muncul; tas kosong tanpa petak matang: garis ke Storage.
- [ ] Recap muncul setelah makan, tidak menyuruh tidur; Sleep tetap menunggu timer habis.
- [ ] Pengingat 120 detik hanya menampilkan baris "Sekarang: ...".
- [ ] Hapus objek `TARSSystem`: Hungry dan Recap tetap selesai (arahan masuk Console), tutorial tidak macet.
- [ ] Hari 2: tidak ada arahan; kenyang tidak diturunkan lagi.

## Kartu panduan tutorial (2026-11-28; BARU, belum dites di Unity)
Siapkan: terapkan setelah `tutorial-story.patch`. Play dari Day 1. Unity akan membuat file `.meta` baru untuk `TutorialDialogueSkin.cs`.
- [ ] Console tanpa error kompilasi dan tanpa error saat kartu pertama muncul.
- [ ] Pesan tutorial tampil di kartu tengah atas (di bawah timer), garis tepi cyan, lencana PANDUAN, "Langkah n dari 12", judul langkah. Tidak menimpa HUD kiri atas, kanan atas, dan timer pada 1920x1080 dan pada Game view lebih kecil.
- [ ] Dialog biasa (ajak TARS bicara, Bed, Suit Station) tetap di kotak bawah seperti sebelumnya.
- [ ] Baris terakhir "Sekarang: ..." tampil di kotak amber TUJUAN SEKARANG; baris sebelumnya tampil sebagai isi biasa; footer menunjukkan "Bagian x dari n" dan [E] lanjut/selesai.
- [ ] E, Spasi, dan klik di mana saja melanjutkan; tombol X pada kartu menutup langsung dan callback tetap jalan (Hungry dan Recap tetap selesai).
- [ ] Pengingat 120 detik: kartu hanya berisi judul dan kotak TUJUAN SEKARANG.
- [ ] Pesan Hungry dan Recap lalu dialog biasa berurutan: kartu hilang sepenuhnya sebelum kotak bawah muncul, tidak ada dua panel terbuka bersamaan.
- [ ] Garis arah tutorial tetap tersembunyi selama kartu terbuka dan muncul lagi setelah ditutup.
- [ ] Day 2: tidak ada kartu.
- [ ] (BARU, gaya terakota) Kartu hanya memakai warna terakota dan cokelat gelap (tanpa cyan/amber/merah); kotak portrait TARS di kiri menampilkan sprite TARS tidak buram (pixel art tajam); tab "TARS | PANDUAN" menempel di tepi atas dan tidak menimpa timer.
- [ ] Teks gelap terbaca jelas di atas kotak terakota; kotak TUJUAN SEKARANG (lebih gelap) terbaca dengan teks krem.
- [ ] Bila `downFrames` TARS kosong: kotak portrait tampil polos tanpa error.

## Pintu otomatis (2026-11-28)
- [ ] Tanpa suit, mendekati pintu: pintu tetap tertutup (tidak ada animasi).
- [ ] Mengenakan suit (oksigen cukup) dan mendekat dalam `openDistance`: pintu berganti pintu1 -> pintu2 -> pintu3 -> pintu4 berurutan.
- [ ] Menjauh melewati `openDistance + closeMargin`: pintu menutup mundur pintu4 -> pintu1; collider penghalang aktif lagi saat tertutup penuh.
- [ ] Pemain di luar (IsOutdoors) mendekat dari luar: pintu terbuka, sama seperti SuitGate mati saat pemain di luar.
- [ ] Mendekat-menjauh cepat: pintu berbalik arah dari frame saat itu, tidak macet atau melompat; tidak berkedip di tepi jarak.
- [ ] Tidak ada warning di Console (Frames terisi, Door Renderer terisi, SuitSystem dan PlayerMovement ditemukan).
