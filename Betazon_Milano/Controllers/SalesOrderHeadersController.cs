﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Betazon_Milano.Models;
using AutoAPI.autenticator;

namespace Betazon_Milano.Controllers
{
    [BasicAutorization]
    [Route("api/[controller]")]
    [ApiController]
    public class SalesOrderHeadersController : ControllerBase
    {
        private readonly AdventureWorksLt2019Context _context;

        public SalesOrderHeadersController(AdventureWorksLt2019Context context)
        {
            _context = context;
        }

        // GET: api/SalesOrderHeaders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SalesOrderHeader>>> GetSalesOrderHeaders()
        {
            return await _context.SalesOrderHeaders.ToListAsync();
        }

        // GET: api/SalesOrderHeaders/5
        [HttpGet("{idC}")]
        public async Task<ActionResult<IEnumerable<SalesOrderHeader>>> GetSalesOrderHeader(int idC)
        {
            var salesOrderHeader = _context.SalesOrderHeaders.Where(a => a.CustomerId == idC);

            if (salesOrderHeader == null)
            {
                return NotFound();
            }

            return await salesOrderHeader.ToListAsync();
        }

        // PUT: api/SalesOrderHeaders/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSalesOrderHeader(int id, SalesOrderHeader salesOrderHeader)
        {
            if (id != salesOrderHeader.SalesOrderId)
            {
                return BadRequest();
            }

            _context.Entry(salesOrderHeader).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SalesOrderHeaderExists(id))
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

        // POST: api/SalesOrderHeaders
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<SalesOrderHeader>> PostSalesOrderHeader(SalesOrderHeader salesOrderHeader)
        {
            _context.SalesOrderHeaders.Add(salesOrderHeader);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSalesOrderHeader", new { id = salesOrderHeader.SalesOrderId }, salesOrderHeader);
        }

        // DELETE: api/SalesOrderHeaders/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSalesOrderHeader(int id)
        {
            var salesOrderHeader = await _context.SalesOrderHeaders.FindAsync(id);
            if (salesOrderHeader == null)
            {
                return NotFound();
            }

            _context.SalesOrderHeaders.Remove(salesOrderHeader);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SalesOrderHeaderExists(int id)
        {
            return _context.SalesOrderHeaders.Any(e => e.SalesOrderId == id);
        }
    }
}
