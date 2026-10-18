using KutuphaneOtomasyon.Core.Models;
using NUnit.Framework;

namespace KutuphaneOtomasyon.Tests;

/// <summary>
/// Tüm sistemi uçtan uca test eden entegrasyon senaryoları.
/// Bir öğrencinin kitap arayıp ödünç alması ve iade etmesi akışını simüle eder.
/// </summary>
[TestFixture]
[Category("Entegrasyon")]
public class EntegrasyonTests
{
    private Kutuphane _kutuphane = null!;
    private Yonetici  _yonetici  = null!;
    private Ogrenci   _ali       = null!;
    private Ogrenci   _mehmet    = null!;

    [SetUp]
    public void Setup()
    {
        _kutuphane = new Kutuphane("Merkez Kütüphane", "Ana Kampüs, A Blok");

        _yonetici = new Yonetici("YNT-001", "Zeynep", "Kaya",
                                 "zeynep@uni.edu.tr", "admin123",
                                 "PRS-001");
        _ali = new Ogrenci("USR-001", "Ali", "Yılmaz",
                           "ali@uni.edu.tr", "ali123",
                           "2021045023", "Bilgisayar Mühendisliği");
        _mehmet = new Ogrenci("USR-002", "Mehmet", "Çelik",
                              "mehmet@uni.edu.tr", "mehmet123",
                              "2021045024", "Elektrik Mühendisliği");

        _kutuphane.UyeEkle(_ali);
        _kutuphane.UyeEkle(_mehmet);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // SENARYO 1: Yönetici kitap ekler, öğrenci arar ve ödünç alır
    // ─────────────────────────────────────────────────────────────────────────

    [Test]
    [Order(1)]
    public void Senaryo1_YoneticiEkler_OgrenciArar_OduncAlir()
    {
        // ARRANGE
        var kitap = new Kitap("KIT-001", "978-0-13-468599-1",
                              "Yazılım Mühendisliği", "Ian Sommerville",
                              "Mühendislik", 3, 2016);

        // ACT — Yönetici ekler
        _yonetici.KitapEkle(kitap, _kutuphane);

        // ACT — Öğrenci arar
        var aramaResult = _ali.KitapAra("Yazılım",
                                        _kutuphane.TumKitaplariGetir());

        // ACT — Öğrenci ödünç alır
        var islem = _ali.KitapOduncAl(aramaResult[0], _kutuphane);

        // ASSERT
        Assert.Multiple(() =>
        {
            Assert.That(aramaResult,             Has.Count.EqualTo(1));
            Assert.That(islem,                   Is.Not.Null);
            Assert.That(kitap.MevcutSayi,        Is.EqualTo(2));
            Assert.That(_ali.AktifOduncSayisi,   Is.EqualTo(1));
            Assert.That(islem!.Durum,            Is.EqualTo(Durum.Aktif));
        });
    }

    // ─────────────────────────────────────────────────────────────────────────
    // SENARYO 2: İki öğrenci aynı kitabı sırayla ödünç alır (stok azalır)
    // ─────────────────────────────────────────────────────────────────────────

    [Test]
    [Order(2)]
    public void Senaryo2_IkiOgrenci_AyniKitabi_StokAzalir()
    {
        var kitap = new Kitap("KIT-002", "978-B", "Temiz Kod",
                              "Robert Martin", "Programlama", 2, 2008);
        _yonetici.KitapEkle(kitap, _kutuphane);

        _ali.KitapOduncAl(kitap, _kutuphane);
        _mehmet.KitapOduncAl(kitap, _kutuphane);

        Assert.That(kitap.MevcutSayi, Is.EqualTo(0));
        Assert.That(kitap.MusaitMi(), Is.False);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // SENARYO 3: Öğrenci ödünç alır, zamanında iade eder → ceza yok
    // ─────────────────────────────────────────────────────────────────────────

    [Test]
    [Order(3)]
    public void Senaryo3_ZamanindaIade_CezaYok_DurumIadeEdildi()
    {
        var kitap = new Kitap("KIT-003", "978-C", "Algoritma Analizi",
                              "Thomas Cormen", "Bilgisayar", 1, 2009);
        _yonetici.KitapEkle(kitap, _kutuphane);

        var islem = _ali.KitapOduncAl(kitap, _kutuphane, oduncSuresiGun: 14)!;
        _ali.KitapIade(islem);

        Assert.Multiple(() =>
        {
            Assert.That(islem.Durum,          Is.EqualTo(Durum.IadeEdildi));
            Assert.That(islem.CezaMiktari,    Is.EqualTo(0m));
            Assert.That(kitap.MevcutSayi,     Is.EqualTo(1));
            Assert.That(_ali.AktifOduncSayisi, Is.EqualTo(0));
        });
    }

    // ─────────────────────────────────────────────────────────────────────────
    // SENARYO 4: Gecikmeli iade → ceza hesaplanır, yönetici raporda görür
    // ─────────────────────────────────────────────────────────────────────────

    [Test]
    [Order(4)]
    public void Senaryo4_GecikmeliIslem_YoneticiRaporundaGorunur()
    {
        var kitap = new Kitap("KIT-004", "978-D", "İşletim Sistemleri",
                              "Andrew Tanenbaum", "Bilgisayar", 1, 2014);
        _yonetici.KitapEkle(kitap, _kutuphane);

        // Manuel olarak gecikmeli işlem oluştur
        var gecikmeliIslem = new OduncIslemi(
            "ISL-GEC-001", _ali, kitap,
            oduncTarihi: DateTime.Today.AddDays(-20),
            iadeTarihi:  DateTime.Today.AddDays(-5));  // 5 gün gecikmeli

        _kutuphane.IslemKaydet(gecikmeliIslem);

        var raporlar = _yonetici.GecikmeRaporuGor(_kutuphane);

        Assert.Multiple(() =>
        {
            Assert.That(raporlar,                    Has.Count.EqualTo(1));
            Assert.That(gecikmeliIslem.GecikmeHesapla(), Is.EqualTo(5));
            Assert.That(gecikmeliIslem.CezaHesapla(),    Is.EqualTo(10m)); // 5×2
        });
    }

    // ─────────────────────────────────────────────────────────────────────────
    // SENARYO 5: Stok biten kitap başka öğrenciye verilmez
    // ─────────────────────────────────────────────────────────────────────────

    [Test]
    [Order(5)]
    public void Senaryo5_StokTukenince_YeniOduncBasarisiz()
    {
        var kitap = new Kitap("KIT-005", "978-E", "Veri Tabanı",
                              "Ramez Elmasri", "Veritabanı", 1, 2015);
        _yonetici.KitapEkle(kitap, _kutuphane);

        _ali.KitapOduncAl(kitap, _kutuphane);             // Stok: 0
        var islem = _mehmet.KitapOduncAl(kitap, _kutuphane); // Null beklenir

        Assert.That(islem, Is.Null);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // SENARYO 6: Tam sistem akışı — Ekle → Ara → Ödünç → İade → Stok Kontrol
    // ─────────────────────────────────────────────────────────────────────────

    [Test]
    [Order(6)]
    public void Senaryo6_TamAkis_EkldenIadeye()
    {
        // 1. Yönetici kitap ekler
        var kitap = new Kitap("KIT-006", "978-F", "Nesne Yönelimli Tasarım",
                              "Erich Gamma", "Yazılım", 2, 1995);
        _yonetici.KitapEkle(kitap, _kutuphane);
        Assert.That(_kutuphane.TumKitaplariGetir(), Has.Count.EqualTo(1));

        // 2. Öğrenci arar
        var bulunanlar = _ali.KitapAra("Nesne", _kutuphane.TumKitaplariGetir());
        Assert.That(bulunanlar, Has.Count.EqualTo(1));

        // 3. Öğrenci ödünç alır
        var islem = _ali.KitapOduncAl(bulunanlar[0], _kutuphane)!;
        Assert.That(kitap.MevcutSayi, Is.EqualTo(1));

        // 4. İade eder
        _ali.KitapIade(islem);
        Assert.That(kitap.MevcutSayi, Is.EqualTo(2));

        // 5. Stok raporu doğrula
        _yonetici.StokRaporuGor(_kutuphane);  // Konsol çıktısı

        // 6. Gecikme raporu — temiz olmalı
        var gecikmeRapor = _yonetici.GecikmeRaporuGor(_kutuphane);
        Assert.That(gecikmeRapor, Is.Empty);
    }
}
