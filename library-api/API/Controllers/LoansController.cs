using library_api.Application.DTOs.Loan;
using library_api.Application.Interfaces;
using library_api.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace library_api.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoansController : ControllerBase
    {
        private readonly ILoanService _loanService;

        public LoansController(ILoanService loanService)
        {
            _loanService = loanService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LoanDto>>> GetAll()
        {
            var loans = await _loanService.GetAllAsync();
            return Ok(loans);
        }

        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<LoanDto>>> GetActiveLoans()
        {
            var loans = await _loanService.GetActiveLoansAsync();
            return Ok(loans);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<LoanDto>> GetById(int id)
        {
            var loan = await _loanService.GetByIdAsync(id);
            if (loan == null)
            {
                return NotFound(new { message = $"Préstamo con ID {id} no encontrado." });
            }
            return Ok(loan);
        }

        [HttpGet("student/{studentName}")]
        public async Task<ActionResult<IEnumerable<LoanDto>>> GetByStudent(string studentName)
        {
            var loans = await _loanService.GetLoansByStudentAsync(studentName);
            return Ok(loans);
        }

        [HttpPost]
        public async Task<ActionResult<LoanDto>> Create([FromBody] CreateLoanDto dto)
        {
            try
            {
                var loan = await _loanService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = loan.Id }, loan);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (BusinessRuleException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}/return")]
        public async Task<ActionResult<LoanDto>> ReturnLoan(int id)
        {
            try
            {
                var loan = await _loanService.ReturnLoanAsync(id);
                return Ok(loan);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (BusinessRuleException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}

