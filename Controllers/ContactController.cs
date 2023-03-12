using ContactManager.Data;
using ContactManager.Models;
using Microsoft.AspNetCore.Mvc;

namespace ContactManager.Controllers;

[ApiController]
[Route("[controller]")]
public class ContactController : ControllerBase
{
    private readonly DataContext _context;
    private readonly IConfiguration _config;

    public ContactController(DataContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> GetContactByID(int id)
    {
        try
        {
            var contact = _context.tbl_Contact.Where(contact => contact.Id == id);
            return Ok(contact);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }
    [HttpGet]
    public async Task<ActionResult<List<TBLContact>>> GetAllContactList()
    {
        List<TBLContact> objTAllContactList = _context.tbl_Contact.OrderByDescending(i => i.Id).ToList();
        return Ok(objTAllContactList);
    }
  
    [HttpPost]
        public async Task<ActionResult<TBLContact>> CreateContact (TBLContact contact)
   
    {
        // Validate the input
        if ((string.IsNullOrEmpty(contact.Salutation) && contact.Salutation.Length < 3) ||
            (string.IsNullOrEmpty(contact.FirstName) && contact.FirstName.Length < 3) ||
            (string.IsNullOrEmpty(contact.LastName) && contact.LastName.Length < 3))
        {
            return BadRequest("Invalid input");
        }

        // Set the display name if it is empty
        if (string.IsNullOrEmpty(contact.DisplayName))
        {
            contact.DisplayName = contact.Salutation + " " + contact.FirstName + " " + contact.LastName;
        }

        // Set the creation timestamp
        contact.CreationTimestamp = DateTime.UtcNow;

        // Add the contact to the list
        // _contacts.Add(contact);
          await _context.tbl_Contact.AddAsync(contact);
            await _context.SaveChangesAsync();

        return Ok(contact);
    }
    [HttpPut("{id}")]
    public async Task<ActionResult<TBLContact>> UpdateContact (int id,TBLContact contact)
    // public IActionResult UpdateContact(int id, Contact contact)
    {
        // Find the contact with the given ID
        var existingContact = _context.tbl_Contact.FirstOrDefault(c => c.Id == id);
        if (existingContact == null)
        {
            return NotFound();
        }

        // Validate the input
      if ((string.IsNullOrEmpty(contact.Salutation) && contact.Salutation.Length < 3) ||
            (string.IsNullOrEmpty(contact.FirstName) && contact.FirstName.Length < 3) ||
            (string.IsNullOrEmpty(contact.LastName) && contact.LastName.Length < 3))
        {
            return BadRequest("Invalid input");
        }
        // Update the contact
        existingContact.Salutation = contact.Salutation;
        existingContact.FirstName = contact.FirstName;
        existingContact.LastName = contact.LastName;
        existingContact.DisplayName = string.IsNullOrEmpty(contact.DisplayName) ?
            contact.Salutation + " " + contact.FirstName + " " + contact.LastName : contact.DisplayName;
        existingContact.BirthDate = contact.BirthDate;

        // Set the last change timestamp
        existingContact.LastChangeTimestamp = DateTime.UtcNow;
          _context.tbl_Contact.Update(existingContact);
            await _context.SaveChangesAsync();
        return Ok(existingContact);
    }
[HttpDelete("{id}")]
public async Task<IActionResult> DeleteContact(int id)
{
    // Find the contact with the given ID
    var contact = await _context.tbl_Contact.FindAsync(id);
    if (contact == null)
    {
        return NotFound();
    }

    // Remove the contact from the list
    _context.tbl_Contact.Remove(contact);
    await _context.SaveChangesAsync();

    return Ok();
}


}


