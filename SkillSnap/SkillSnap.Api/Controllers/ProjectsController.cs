using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SkillSnap.Api.Models;

namespace SkillSnap.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ProjectsController : ControllerBase
{
    private readonly SkillSnapContext _context;
    private readonly IMemoryCache _cache;

    private static int _cacheHits;
    private static int _cacheMisses;

    public ProjectsController(SkillSnapContext context, IMemoryCache cache)
    {
        _context = context;
        _cache = cache;
    }

    // GET: api/projects
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Project>>> GetProjects()
    {
        if (!_cache.TryGetValue("projects", out List<Project> projects))
        {
            var stopwatch = Stopwatch.StartNew();
            projects = await _context.Projects.ToListAsync();
            stopwatch.Stop();
            _cache.Set("projects", projects, TimeSpan.FromMinutes(5));

        _cacheHits++;

    // _logger.LogInformation(
    //     "Cache Hit #{Hits} | Misses #{Misses}",
    //     _cacheHits,
    //     _cacheMisses);
        }
        _cacheMisses++;
        return Ok(projects);
    }

    // POST: api/projects
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<Project>> CreateProject(Project project)
    {
        _context.Projects.Add(project);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetProjects),
            new { id = project.Id },
            project);
    }
}
