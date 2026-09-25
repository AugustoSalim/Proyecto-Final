using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs
{
    public class RefreshRequestDto
    {
        [Required(ErrorMessage = "El Access Token es obligatorio.")]
        public string AccessToken { get; set; } = string.Empty;

        [Required(ErrorMessage = "El Refresh Token es obligatorio.")]
        public string RefreshToken { get; set; } = string.Empty;
    }
}