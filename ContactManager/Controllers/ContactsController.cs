using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ContactManager.Models;
using System.Text.RegularExpressions;
using System.Globalization;
using System.IO;

namespace ContactManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactsController : ControllerBase
    {
        private readonly ContactContext _context;

        public ContactsController(ContactContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get a contact by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Contact>> GetContact(long id)
        {
            var contact = await _context.Contacts.FindAsync(id);
            if (contact == null) { return NotFound(); }
            return contact;
        }

        /// <summary>
        /// Get contacts filtered by email or phone
        /// </summary>
        /// <param name="email"></param>
        /// <param name="phone"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Contact>>> GetContacts(string email, string phone, int? skip, int? take)
        {
            var contacts = _context.Contacts.AsQueryable();
            if (!string.IsNullOrWhiteSpace(email)) { contacts = contacts.Where(x => EF.Functions.Like(x.Email, "%" + email + "%")); } //wildcard search for email
            if (!string.IsNullOrWhiteSpace(phone))
            {
                contacts = contacts.Where(x => EF.Functions.Like(x.WorkPhone, "%" + phone + "%") || EF.Functions.Like(x.HomePhone, "%" + phone + "%")); //wildcard search for phone
            }
            if (skip != null) { contacts = contacts.Skip((int)skip); }
            if (take != null) { contacts = contacts.Take((int)take); }
            return await contacts.ToListAsync();
        }

        /// <summary>
        /// Get contacts filtered by city OR state OR (city AND state)
        /// </summary>
        /// <param name="state">To search</param>
        /// <param name="city"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        [Route("[action]")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Contact>>> GetByCityState(string state, string city, int? skip, int? take)
        {
            var contacts = _context.Contacts.AsQueryable();
            if (!string.IsNullOrWhiteSpace(state)) { contacts = contacts.Where(x => EF.Functions.Like(x.State, "%" + state + "%")); } //wildcard search for state            
            if (!string.IsNullOrWhiteSpace(city)) { contacts = contacts.Where(x => EF.Functions.Like(x.City, "%" + city + "%")); } //wildcard search for city                         
            if (skip != null) { contacts = contacts.Skip((int)skip); }
            if (take != null) { contacts = contacts.Take((int)take); }
            return await contacts.ToListAsync();
        }

        /// <summary>
        /// Update a contact
        /// </summary>
        /// <param name="id"></param>
        /// <param name="contact"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutContact(long id, Contact contact)
        {
            if (id != contact.ID) { return BadRequest(); }
            if (!string.IsNullOrWhiteSpace(contact.Email))
            {
                if (!ValidateEmail(contact.Email)) { return BadRequest("Email address format is invalid"); }
            }
            _context.Entry(contact).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ContactExists(id)) { return NotFound(); } else { throw; }
            }
            return NoContent();
        }

        /// <summary>
        /// Create a contact
        /// </summary>
        /// <param name="contact"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<Contact>> PostContact(Contact contact)
        {
            if(contact.ID != 0) { if (ContactExists(contact.ID)) { return BadRequest("A contact with this ID already exists"); } }
            if (!string.IsNullOrWhiteSpace(contact.Email))
            {
                if (!ValidateEmail(contact.Email)) { return BadRequest("Email address format is invalid"); }
            }
            _context.Contacts.Add(contact);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetContact), new { id = contact.ID }, contact);
        }

        /// <summary>
        /// Delete a contact
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContact(long id)
        {
            var contact = await _context.Contacts.FindAsync(id);
            if (contact == null) { return NotFound(); }
            _context.Contacts.Remove(contact);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Does contact exist
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private bool ContactExists(long id)
        {
            return _context.Contacts.Any(e => e.ID == id);
        }

        /// <summary>
        /// Returns true if the provided email is in a valid format
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        /// <remarks>
        /// (@)         Match the @ character. This part is the first capturing group.
        /// (.+)        Match one or more occurrences of any character. This part is the second capturing group.
        /// $	        End the match at the end of the string.
        /// 
        ///  ^          Begin the match at the start of the string.
        ///  [^@\s]+    Match one or more occurrences of any character other than the @ character or whitespace.
        ///  @          Match the @ character.
        ///  [^@\s]+    Match one or more occurrences of any character other than the @ character or whitespace.
        ///  \.         Match a single period character.
        ///  [^@\s]+    Match one or more occurrences of any character other than the @ character or whitespace.
        ///  $          End the match at the end of the string.
        /// </remarks>
        private bool ValidateEmail(string email)
        {
            try
            {                
                email = Regex.Replace(email, @"(@)(.+)$", DomainMapper, RegexOptions.None, TimeSpan.FromMilliseconds(200)); //Normalize the domain
                //Examines the domain part of the email and normalizes it
                string DomainMapper(Match match)
                {                  
                    var idn = new IdnMapping(); //Use IdnMapping class to convert Unicode domain names                 
                    string domainName = idn.GetAscii(match.Groups[2].Value); //Pull out and process domain name (throws ArgumentException on invalid)
                    return match.Groups[1].Value + domainName;
                }
            }
            catch (RegexMatchTimeoutException) { return false; }
            catch (ArgumentException) { return false; }

            try
            {
                //Note: A timeout is used to prevent a malicious user from causing a Denial-of-Service attack
                return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            } 
            catch (RegexMatchTimeoutException) { return false; }
        }


        [Route("[action]")]
        [HttpGet]
        public FileContentResult ViewContactImage(long id)
        {
            var contact = _context.Contacts.Find(id);

            //When creating a stream, you need to reset the position, without it you will see that you always write files with a 0 byte length. 
            var imageDataStream = new MemoryStream(contact.ProfileImageBytes);
            imageDataStream.Position = 0;
            //return a file so we can ensure it was uploaded correctly. 
            return File(contact.ProfileImageBytes, "image/png");
        }

    }
}
