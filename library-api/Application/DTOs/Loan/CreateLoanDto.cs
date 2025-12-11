using System.ComponentModel.DataAnnotations;

namespace library_api.Application.DTOs.Loan
{
    public class CreateLoanDto
    {
        [Required(ErrorMessage = "El libro es requerido")]
        public int BookId { get; set; }

        [Required(ErrorMessage = "El nombre del estudiante es requerido")]
        [StringLength(150, ErrorMessage = "El nombre del estudiante no puede exceder 150 caracteres")]
        public string StudentName { get; set; } = string.Empty;
    }
}
