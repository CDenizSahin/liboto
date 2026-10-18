using KutuphaneOtomasyon.Core.Models;
using NUnit.Framework;

namespace KutuphaneOtomasyon.Tests;

/// <summary>
/// Kutuphane sınıfının kitap yönetimi, üye yönetimi ve raporlama özelliklerini test eder.
/// </summary>
[TestFixture]
[Category("Kutuphane")]
public class KutuphanelTests
{
    private Kutuphane _kutuphane = null!;
    private Kitap     _kitap1   = null!;
    private Kitap     _kitap2   = null!;
    private Ogrenci   _ogrenci  = null!;

    [SetUp]
    public void Setup()
    {
        _kutuphane = new Kutuphane("Test Kütüphane", "Test Adres");
        _kitap1    = new Kitap("KIT-001", "978-A", "Yazılım Mühendisliği",
                               "Ian Sommerville", "Mühendislik", 3, 2016);
        _kitap2    = new Kitap("KIT-002", "978-B", "Temiz Kod",
                               "Robert Martin", "Programlama", 2, 2008);
        _ogrenci   = new Ogrenci("USR-001", "Ali", "Yılmaz",
                                 "ali@test.com", "ali123",
                                 "20210001", "Bilgisayar Müh.");
    }

    // ── KitapEkle ────────────────────────────────────────────────────────────

    [Test]
    public void KitapEkle_GecerliKitap_ListeyeEklenir()
    {
        _kutuphane.KitapEkle(_kitap1);
        Assert.That(_kutuphane.TumKitaplariGetir(), Has.Count.EqualTo(1));
    }

    [Test]
    public void KitapEkle_NullKitap_ArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => _kutuphane.KitapEkle(null!));
    }

    [Test]
    public void KitapEkle_IkiKitap_ListedeDortKayit()
    {
        _kutuphane.KitapEkle(_kitap1);
        _kutuphane.KitapEkle(_kitap2);
        Assert.That(_kutuphane.TumKitaplariGetir(), Has.Count.EqualTo(2));
    }

    // ── KitapSil ─────────────────────────────────────────────────────────────

    [Test]
    public void KitapSil_VarOlanId_TrueDoner()
    {
        _kutuphane.KitapEkle(_kitap1);
        var sonuc = _kutuphane.KitapSil("KIT-001");
        Assert.That(sonuc, Is.True);
    }

    [Test]
    public void KitapSil_VarOlanId_ListedenKalkar()
    {
        _kutuphane.KitapEkle(_kitap1);
        _kutuphane.KitapSil("KIT-001");
        Assert.That(_kutuphane.TumKitaplariGetir(), Is.Empty);
    }

    [Test]
    public void KitapSil_OlmayanId_FalseDoner()
    {
        var sonuc = _kutuphane.KitapSil("KIT-YOOK");
        Assert.That(sonuc, Is.False);
    }

    // ── KitapAra ─────────────────────────────────────────────────────────────

    [Test]
    public void KitapAra_BasligeGore_DogruSonuc()
    {
        _kutuphane.KitapEkle(_kitap1);
        _kutuphane.KitapEkle(_kitap2);
        var sonuc = _kutuphane.KitapAra("Yazılım");
        Assert.That(sonuc, Has.Count.EqualTo(1));
        Assert.That(sonuc[0].Baslik, Is.EqualTo("Yazılım Mühendisliği"));
    }

    [Test]
    public void KitapAra_YazaraGore_DogruSonuc()
    {
        _kutuphane.KitapEkle(_kitap1);
        _kutuphane.KitapEkle(_kitap2);
        var sonuc = _kutuphane.KitapAra("Sommerville");
        Assert.That(sonuc, Has.Count.EqualTo(1));
    }

    [Test]
    public void KitapAra_OlmayanKriter_BosListe()
    {
        _kutuphane.KitapEkle(_kitap1);
        var sonuc = _kutuphane.KitapAra("XYZBilinmeyen");
        Assert.That(sonuc, Is.Empty);
    }

    // ── IslemKaydet / TumIslemleri ────────────────────────────────────────────

    [Test]
    public void IslemKaydet_GecerliIslem_ListeyeEklenir()
    {
        _kutuphane.KitapEkle(_kitap1);
        _kutuphane.UyeEkle(_ogrenci);
        var islem = _ogrenci.KitapOduncAl(_kitap1, _kutuphane);
        Assert.That(_kutuphane.TumIslemleriGetir(), Has.Count.EqualTo(1));
    }

    [Test]
    public void IslemKaydet_NullIslem_ArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => _kutuphane.IslemKaydet(null!));
    }

    // ── TumGecikmeleriRaporla ────────────────────────────────────────────────

    [Test]
    public void TumGecikmeleriRaporla_HiçGecikmeyoksa_BosListe()
    {
        _kutuphane.KitapEkle(_kitap1);
        _kutuphane.UyeEkle(_ogrenci);
        _ogrenci.KitapOduncAl(_kitap1, _kutuphane, oduncSuresiGun: 14);
        Assert.That(_kutuphane.TumGecikmeleriRaporla(), Is.Empty);
    }

    [Test]
    public void TumGecikmeleriRaporla_GecikmeliIslemVarsa_DondurulUr()
    {
        _kutuphane.KitapEkle(_kitap1);
        // Gecikmeli işlemi manuel kaydet
        var gecikmeliIslem = new OduncIslemi("ISL-GEC", _ogrenci, _kitap1,
            DateTime.Today.AddDays(-20), DateTime.Today.AddDays(-3));
        _kutuphane.IslemKaydet(gecikmeliIslem);

        var sonuc = _kutuphane.TumGecikmeleriRaporla();
        Assert.That(sonuc, Has.Count.EqualTo(1));
    }
}
