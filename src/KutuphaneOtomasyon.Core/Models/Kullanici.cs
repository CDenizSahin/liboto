namespace KutuphaneOtomasyon.Core.Models;

/// <summary>
/// Sistemdeki tüm kullanıcı türleri için soyut taban sınıf.
/// Abstraction + Encapsulation prensiplerini uygular.
/// </summary>
public abstract class Kullanici
{
    // ── Private alanlar (Encapsulation) ─────────────────────────────────────
    private string _email;
    private readonly string _sifre;

    // ── Protected alanlar (alt sınıflar erişebilir) ──────────────────────────
    protected string KullaniciId { get; }
    protected string Ad          { get; set; }
    protected string Soyad       { get; set; }

    protected Kullanici(string kullaniciId, string ad, string soyad,
                        string email, string sifre)
    {
        KullaniciId = kullaniciId;
        Ad          = ad;
        Soyad       = soyad;
        _email      = email;
        _sifre      = sifre;
    }

    // ── Public property'ler ──────────────────────────────────────────────────
    public string TamAd => $"{Ad} {Soyad}";

    public string Email
    {
        get => _email;
        set
        {
            if (!value.Contains('@'))
                throw new ArgumentException("Geçersiz e-posta adresi.");
            _email = value;
        }
    }

    // ── Soyut metotlar (Polymorphism için override edilir) ───────────────────
    public abstract bool GirisYap(string email, string sifre);

    public virtual void CikisYap()
        => Console.WriteLine($"[ÇIKIŞ] {TamAd} sistemden çıkış yaptı.");

    public virtual void BilgileriGuncelle(string? yeniAd = null,
                                          string? yeniSoyad = null,
                                          string? yeniEmail = null)
    {
        if (yeniAd    is not null) Ad    = yeniAd;
        if (yeniSoyad is not null) Soyad = yeniSoyad;
        if (yeniEmail is not null) Email = yeniEmail;
    }

    /// <summary>Şifre kontrolü — harici erişime kapalı, sadece GirisYap içinde kullanılır.</summary>
    protected bool SifreDogrula(string girilenSifre) => _sifre == girilenSifre;

    public override string ToString() => $"[{GetType().Name}] {TamAd} <{Email}>";
}
