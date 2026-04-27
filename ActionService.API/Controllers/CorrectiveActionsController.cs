using Microsoft.AspNetCore.Mvc;
using ActionService.API.DTOs;
using ActionService.API.Services;

namespace ActionService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CorrectiveActionsController : ControllerBase
    {
        private readonly ICorrectiveActionService _service;

        public CorrectiveActionsController(ICorrectiveActionService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var actions = await _service.GetAllActionsAsync();
            return Ok(actions);
        }

        [HttpGet("{actionId}")]
        public async Task<IActionResult> GetById(int actionId)
        {
            var action = await _service.GetActionByIdAsync(actionId);
            if (action == null) return NotFound();
            return Ok(action);
        }

        [HttpGet("by-observation/{observationId}")]
        public async Task<IActionResult> GetByObservationId(int observationId)
        {
            var actions = await _service.GetActionsByObservationIdAsync(observationId);
            return Ok(actions);
        }

        [HttpGet("by-user/{userId}")]
        public async Task<IActionResult> GetByAssignedUserId(int userId)
        {
            var actions = await _service.GetActionsByAssignedUserIdAsync(userId);
            return Ok(actions);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCorrectiveActionDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var created = await _service.CreateActionAsync(dto);
            return CreatedAtAction(nameof(GetById), new { actionId = created.ActionId }, created);
        }

        [HttpPut("{actionId}")]
        public async Task<IActionResult> Update(int actionId, [FromBody] UpdateCorrectiveActionDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var updated = await _service.UpdateActionAsync(actionId, dto);
                if (updated == null) return NotFound();
                return Ok(updated);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("{actionId}/request-closure")]
        public async Task<IActionResult> RequestClosure(int actionId)
        {
            try
            {
                var result = await _service.RequestClosureAsync(actionId);
                if (!result) return NotFound();
                return Ok(new { message = "Closure requested successfully." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpDelete("{actionId}")]
        public async Task<IActionResult> SoftDelete(int actionId)
        {
            var result = await _service.SoftDeleteAsync(actionId);
            if (!result) return NotFound();
            return NoContent();
        }

        [HttpGet("{actionId}/history")]
        public async Task<IActionResult> GetHistory(int actionId)
        {
            var history = await _service.GetActionHistoryAsync(actionId);
            return Ok(history);
        }
    }
}
