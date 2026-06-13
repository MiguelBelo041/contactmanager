using ContactManager.Api.Data;
using ContactManager.Api.DTOs;
using ContactManager.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ContactManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContactsController : ControllerBase
{
    private readonly AppDbContext _db;

    public ContactsController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Contact>>> GetAll()
    {
        var contacts = await _db.Contacts
            .OrderBy(c => c.Nome)
            .ThenBy(c => c.Sobrenome)
            .ToListAsync();

        return Ok(contacts);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Contact>> GetById(Guid id)
    {
        var contact = await _db.Contacts.FindAsync(id);
        if (contact is null)
            return NotFound();

        return Ok(contact);
    }

    [HttpPost]
    public async Task<ActionResult<Contact>> Create([FromBody] ContactCreateDto dto)
    {
        var contact = new Contact
        {
            Id = Guid.NewGuid(),
            Nome = dto.Nome.Trim(),
            Sobrenome = dto.Sobrenome.Trim(),
            Telefone = dto.Telefone.Trim()
        };

        _db.Contacts.Add(contact);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = contact.Id }, contact);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] ContactUpdateDto dto)
    {
        var contact = await _db.Contacts.FindAsync(id);
        if (contact is null)
            return NotFound();

        contact.Nome = dto.Nome.Trim();
        contact.Sobrenome = dto.Sobrenome.Trim();
        contact.Telefone = dto.Telefone.Trim();

        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var contact = await _db.Contacts.FindAsync(id);
        if (contact is null)
            return NotFound();

        _db.Contacts.Remove(contact);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
