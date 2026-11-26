using System.ComponentModel.DataAnnotations;
namespace Web_barkery1.Models;
public class ProfileViewModel
{
    [StringLength(100)] public string? FullName { get; set; }
    [StringLength(300)] public string? Address { get; set; }
    [StringLength(30), Phone] public string? PhoneNumber { get; set; }
}
