using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CareerPath.Application.Interfaces;
using CareerPath.Contracts.Dto;
using System.Threading.Tasks;
using System.Security.Claims;
using System;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using AutoMapper;
using CareerPath.Domain.Entities;

namespace CareerPath.Api.Controllers
{
    [ApiController]
    [Route("api/companies")]
    public class CompaniesController : ControllerBase
    {
        private readonly ICompanyService _companyService;
        private readonly ILogger<CompaniesController> _logger;
        private readonly IMapper _mapper;

        public CompaniesController(
            ICompanyService companyService,
            ILogger<CompaniesController> logger,
            IMapper mapper)
        {
            _companyService = companyService ?? throw new ArgumentNullException(nameof(companyService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        /// Get all companies
        /// </summary>
        /// <returns>List of all companies</returns>
        [HttpGet]
        [ProducesResponseType(typeof(CompanyDto[]), 200)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var companies = await _companyService.GetCompanies();
                var companyDtos = _mapper.Map<IEnumerable<CompanyDto>>(companies);
                return Ok(companyDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all companies");
                return StatusCode(500, new { Error = "An error occurred while retrieving companies" });
            }
        }

        /// <summary>
        /// Get a company by ID
        /// </summary>
        /// <param name="id">Company ID</param>
        /// <returns>Company details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CompanyDto), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetById(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return BadRequest(new { Error = "Company ID cannot be empty" });
                }

                var company = await _companyService.GetCompany(id);
                if (company == null)
                {
                    return NotFound(new { Error = $"Company with ID {id} not found" });
                }

                var companyDto = _mapper.Map<CompanyDto>(company);
                return Ok(companyDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving company with ID {CompanyId}", id);
                return StatusCode(500, new { Error = "An error occurred while retrieving the company" });
            }
        }

        /// <summary>
        /// Create a new company
        /// </summary>
        /// <param name="createCompanyDto">Company creation data</param>
        /// <returns>Created company details</returns>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(CompanyDto), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Create([FromBody] CreateCompanyDto createCompanyDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                // Map DTO to entity
                var company = _mapper.Map<Company>(createCompanyDto);
                
                // TODO: Create company in service once implemented
                // var createdCompany = await _companyService.CreateCompanyAsync(company);
                // var createdCompanyDto = _mapper.Map<CompanyDto>(createdCompany);
                
                // For now, return a placeholder since the service method doesn't exist yet
                return StatusCode(501, new { Message = "Create company functionality not yet implemented" });
                
                // return CreatedAtAction(nameof(GetById), new { id = createdCompanyDto.Id }, createdCompanyDto);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Validation error when creating company");
                return BadRequest(new { Error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating company");
                return StatusCode(500, new { Error = "An error occurred while creating the company" });
            }
        }

        /// <summary>
        /// Update an existing company
        /// </summary>
        /// <param name="id">Company ID</param>
        /// <param name="updateCompanyDto">Company update data</param>
        /// <returns>Updated company details</returns>
        [HttpPut("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(CompanyDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateCompanyDto updateCompanyDto)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return BadRequest(new { Error = "Company ID cannot be empty" });
                }

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                // TODO: Update company in service once implemented
                // var existingCompany = await _companyService.GetCompanyByIdAsync(id);
                // if (existingCompany == null)
                // {
                //     return NotFound(new { Error = $"Company with ID {id} not found" });
                // }
                
                // _mapper.Map(updateCompanyDto, existingCompany);
                // var updatedCompany = await _companyService.UpdateCompanyAsync(existingCompany);
                // var updatedCompanyDto = _mapper.Map<CompanyDto>(updatedCompany);
                // return Ok(updatedCompanyDto);

                // For now, return a placeholder since the service method doesn't exist yet
                return StatusCode(501, new { Message = "Update company functionality not yet implemented" });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Validation error when updating company");
                return BadRequest(new { Error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating company with ID {CompanyId}", id);
                return StatusCode(500, new { Error = "An error occurred while updating the company" });
            }
        }

        /// <summary>
        /// Delete a company
        /// </summary>
        /// <param name="id">Company ID</param>
        /// <returns>No content on success</returns>
        [HttpDelete("{id}")]
        [Authorize]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return BadRequest(new { Error = "Company ID cannot be empty" });
                }

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                // TODO: Delete company in service once implemented
                // var existingCompany = await _companyService.GetCompanyByIdAsync(id);
                // if (existingCompany == null)
                // {
                //     return NotFound(new { Error = $"Company with ID {id} not found" });
                // }
                
                // await _companyService.DeleteCompanyAsync(id);
                // return NoContent();

                // For now, return a placeholder since the service method doesn't exist yet
                return StatusCode(501, new { Message = "Delete company functionality not yet implemented" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting company with ID {CompanyId}", id);
                return StatusCode(500, new { Error = "An error occurred while deleting the company" });
            }
        }

        /// <summary>
        /// Get jobs for a specific company
        /// </summary>
        /// <param name="id">Company ID</param>
        /// <returns>List of jobs for the company</returns>
        [HttpGet("{id}/jobs")]
        [ProducesResponseType(typeof(JobDto[]), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetCompanyJobs(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return BadRequest(new { Error = "Company ID cannot be empty" });
                }

                var company = await _companyService.GetCompany(id);
                if (company == null)
                {
                    return NotFound(new { Error = $"Company with ID {id} not found" });
                }

                var jobDtos = _mapper.Map<IEnumerable<JobDto>>(company.Jobs);
                return Ok(jobDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving jobs for company with ID {CompanyId}", id);
                return StatusCode(500, new { Error = "An error occurred while retrieving the company jobs" });
            }
        }
    }
} 