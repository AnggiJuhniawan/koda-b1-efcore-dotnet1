using System.ComponentModel.DataAnnotations;

namespace KodaStore.Api.ViewModels;

public sealed class EditProductViewModel
{
    [Required(ErrorMessage = "Nama produk wajib diisi.")]
    [StringLength(120, ErrorMessage = "Maksimal 120 karakter.")]
    [Display(Name = "Nama produk")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Harga wajib diisi.")]
    [Range(typeof(decimal), "0", "1000000000000", ErrorMessage = "Harga tidak boleh negatif.")]
    [Display(Name = "Price (Rp)")]
    public decimal? Price { get; set; }

    [Required(ErrorMessage = "Stok wajib diisi.")]
    [Range(0, int.MaxValue, ErrorMessage = "Stok tidak boleh negatif.")]
    [Display(Name = "Stok")]
    public int? Stock { get; set; }

    [Required(ErrorMessage = "Kategori wajib diisi.")]
    [StringLength(80, ErrorMessage = "Maksimal 80 karakter.")]
    [Display(Name = "Kategori")]
    public string Category { get; set; } = string.Empty;
}
