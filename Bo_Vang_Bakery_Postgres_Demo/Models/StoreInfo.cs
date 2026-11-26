using System.ComponentModel.DataAnnotations;
namespace Web_barkery1.Models;
public class StoreInfo
{
    public int Id { get; set; }
    [Required, StringLength(100)] public string Name { get; set; } = "Quầy bánh Vạn Phúc";
    [Required, StringLength(300)] public string Address { get; set; } = "Hẻm 1, ngách 22/18, làng Lụa Vạn Phúc";
    [StringLength(30)] public string? Phone { get; set; }
    [StringLength(100)] public string? OpeningHours { get; set; }
}
