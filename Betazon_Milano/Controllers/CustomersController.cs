using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Betazon_Milano.Models;
using System.Collections;
using NuGet.Protocol;
using Microsoft.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Security.Cryptography.Xml;
using AutoAPI.autenticator;

namespace Betazon_Milano.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly AdventureWorksLt2019Context _context;
        private object where;

        public CustomersController(AdventureWorksLt2019Context context)
        {
            _context = context;
        }

        // GET: api/Customers
        [BasicAutorization]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers()
        {
            return await _context.Customers.ToListAsync();
        }

        // GET: api/Customers/nome/cognome
        [BasicAutorization]
        [HttpGet("{nome}/{cognome}")]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomer(string nome, string cognome)
        {
            var customer = _context.Customers.Where(a => a.FirstName == nome && a.LastName == cognome);
            if (customer == null)
            {
                return NotFound();
            }

            return await customer.ToListAsync();
        }

        // GET: api/Customers/5
        [BasicAutorization]
        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> GetCustomerID(int id)
        {
            var customer = _context.Customers.FindAsync(id);

            if (customer == null)
            {
                return NotFound();
            }

            return await customer;
        }

        /*[HttpGet("{nome}/{cognome}")]
        public async Task<ActionResult<IEnumerable<CustomerVending>>> GetCustomer(string nome, string cognome)
        {
            var customer = _context.Database.SqlQuery<CustomerVending>($"exec sp_shareName {nome},{cognome}").ToListAsync();
            if (customer == null)
            {
                return NotFound();
            }

            return await customer;
        }*/

        // PUT: api/Customers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [BasicAutorization]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCustomer(int id, Customer customer)
        {
            if (id != customer.CustomerId)
            {
                return BadRequest();
            }

            _context.Entry(customer).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Customers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Customer>> PostCustomer(CustomerPass customer1)
        {
            Customer customer = new Customer();
            customer.CustomerId = customer1.id;
            customer.FirstName = customer1.FirstName;
            customer.LastName = customer1.LastName;
            customer.EmailAddress = customer1.mailAddress;
            customer.Phone = customer1.Phone;
            customer.NameStyle = false;
            customer.Rowguid = Guid.NewGuid();
            customer.ModifiedDate = customer1.ModifiedDate;
            customer.Suffix = customer1.Admin;
            string salt = Convert.ToBase64String(RandomNumberGenerator.GetBytes(5));
            string HashPasword(string password, string salt)
            {
                customer.PasswordSalt = salt;
                SHA512 sHA512 = SHA512.Create();
                sHA512.ComputeHash(Encoding.UTF8.GetBytes(password + salt));
                return Convert.ToBase64String(sHA512.Hash);
            }

            customer.PasswordHash = HashPasword(customer1.Password, salt);

            if (customer1.OperatorCode == 1111)
            {
                _context.Customers.Add(customer);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetCustomerID", new { id = customer.CustomerId }, customer);
            }

            return NotFound();
        }

        // DELETE: api/Customers/5
        [BasicAutorization]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.CustomerId == id);
        }
    }
}
