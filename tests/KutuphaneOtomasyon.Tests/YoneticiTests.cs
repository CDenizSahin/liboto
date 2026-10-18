using KutuphaneOtomasyon.Core.Models;
using NUnit.Framework;

namespace KutuphaneOtomasyon.Tests;

/// <summary>
/// Yonetici sınıfının kitap yönetimi ve raporlama işlemlerini test eder.
/// Ayrıca Kalıtım (Inheritance) ve Polymorphism senaryolarını kapsar.
/// </summary>
[TestFixture]
[Category("Yonetici")]
public class YoneticiTests
{
    private Yonetici  _yonetici  = null!;
    private Kutuphane _kutuphane = null!;
    private Kitap     _kitap     = null!;

    [SetUp]
    public void Setup()
    {
        _yonetici  = new Yonetici("YNT-001", "Zeynep", "Kaya",
                                  "zeynep@uni.edu.tr", "admin123",
                                  "PRS-001", "Tam Yetki");
        _kutuphane = new Kutuphane("Test Kütüphane", "Test Adres");
        _kitap     = new Kitap("KIT-001", "978-X", "Design Patterns",
                               "Gang of Four", "Yazılım", 2, 1994);
    }

    // ── GirisYap ─────────────────────────────────────────────────────────────

    [Test]
    public void GirisYap_DogruBilgiler_True()
        => Assert.That(_yonetici.GirisYap("zeynep@uni.edu.tr", "admin123"), Is.True);

    [Test]
    public void GirisYap_YanlisEmail_False()
        => Assert.That(_yonetici.GirisYap("yanlis@uni.edu.tr", "admin123"), Is.False);

    // ── KitapEkle ────────────────────────────────────────────────────────────

    [Test]
    public void KitapEkle_GecerliKitap_KutuphaneyeEklenir()
    {
        _yonetici.KitapEkle(_kitap, _kutuphane);
        Assert.That(_kutuphane.TumKitaplariGetir(), Has.Count.EqualTo(1));
    }

    // ── KitapSil ─────────────────────────────────────────────────────────────

    [Test]
    public void KitapSil_VarOlanKitap_Siliniyor()
    {
        _yonetici.KitapEkle(_kitap, _kutuphane);
        var sonuc = _yonetici.KitapSil("KIT-001", _kutuphane);
        Assert.Multiple(() =>
        {
            Assert.That(sonuc, Is.True);
            Assert.That(_kutuphane.TumKitaplariGetir(), Is.Empty);
        });
    }

    // ── KitapGuncelle ────────────────────────────────────────────────────────

    [Test]
    public void KitapGuncelle_BaslikDegistir_YeniDegerAtanir()
    {
        _yonetici.KitapGuncelle(_kitap, baslik: "Yeni Başlık");
        Assert.That(_kitap.Baslik, Is.EqualTo("Yeni Başlık"));
    }

    [Test]
    public void KitapGuncelle_YazarVeKategori_IkisiDeBuyuk()
    {
        _yonetici.KitapGuncelle(_kitap, yazar: "Yeni Yazar", kategori: "Yeni Kat.");
        Assert.Multiple(() =>
        {
            Assert.That(_kitap.Yazar,    Is.EqualTo("Yeni Yazar"));
            Assert.That(_kitap.Kategori, Is.EqualTo("Yeni Kat."));
        });
    }

    // ── GecikmeRaporuGor ─────────────────────────────────────────────────────

    [Test]
    public void GecikmeRaporuGor_GecikmesizKutuphane_BosListe()
    {
        var sonuc = _yonetici.GecikmeRaporuGor(_kutuphane);
        Assert.That(sonuc, Is.Empty);
    }

    [Test]
    public void GecikmeRaporuGor_GecikmeliIslem_TekEleman()
    {
        var ogrenci = new Ogrenci("USR-001", "Test", "User",
                                  "test@test.com", "1234",
                                  "20210001", "Bölüm");
        var gecikmeliIslem = new OduncIslemi("ISL-001", ogrenci, _kitap,
            DateTime.Today.AddDays(-20), DateTime.Today.AddDays(-3));
        _kutuphane.IslemKaydet(gecikmeliIslem);

        var sonuc = _yonetici.GecikmeRaporuGor(_kutuphane);
        Assert.That(sonuc, Has.Count.EqualTo(1));
    }

    // ── Polymorphism Senaryosu ────────────────────────────────────────────────

    [Test]
    public void Polymorphism_KullaniciBazindaGirisYap_DogmulDavraniyor()
    {
        // Kullanici referansı ile Yonetici ve Ogrenci nesneleri farklı davranır
        Kullanici yoneticiRef = _yonetici;
        Kullanici ogrenciRef  = new Ogrenci("USR-X", "X", "Y",
                                             "x@x.com", "pw123",
                                             "12345", "Bölüm");

        Assert.Multiple(() =>
        {
            Assert.That(yoneticiRef.GirisYap("zeynep@uni.edu.tr", "admin123"), Is.True);
            Assert.That(ogrenciRef.GirisYap("x@x.com", "pw123"), Is.True);
            Assert.That(ogrenciRef.GirisYap("x@x.com", "yanlis"), Is.False);
        });
    }

    [Test]
    public void Polymorphism_TamAd_DogruFormatlanior()
        => Assert.That(_yonetici.TamAd, Is.EqualTo("Zeynep Kaya"));
}
