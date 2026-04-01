using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FinancialWebApp1.Models;
using FincialWebApp1.Data;

namespace FincialWebApp1.Controllers
{
    public class BankListsController : Controller
    {
        private readonly FincialWebApp1Context _context;

        public BankListsController(FincialWebApp1Context context)
        {
            _context = context;
        }

        // GET: BankLists
        public async Task<IActionResult> Index()
        {
              return _context.BankList != null ? 
                          View(await _context.BankList.ToListAsync()) :
                          Problem("Entity set 'FincialWebApp1Context.BankList'  is null.");
        }

        // GET: BankLists/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.BankList == null)
            {
                return NotFound();
            }

            var bankList = await _context.BankList
                .FirstOrDefaultAsync(m => m.BankListId == id);
            if (bankList == null)
            {
                return NotFound();
            }

            return View(bankList);
        }

        // GET: BankLists/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: BankLists/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BankListId,Name,Branch")] BankList bankList)
        {
            if (ModelState.IsValid)
            {
                _context.Add(bankList);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(bankList);
        }

        // GET: BankLists/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.BankList == null)
            {
                return NotFound();
            }

            var bankList = await _context.BankList.FindAsync(id);
            if (bankList == null)
            {
                return NotFound();
            }
            return View(bankList);
        }

        // POST: BankLists/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BankListId,Name,Branch")] BankList bankList)
        {
            if (id != bankList.BankListId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(bankList);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BankListExists(bankList.BankListId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(bankList);
        }

        // GET: BankLists/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.BankList == null)
            {
                return NotFound();
            }

            var bankList = await _context.BankList
                .FirstOrDefaultAsync(m => m.BankListId == id);
            if (bankList == null)
            {
                return NotFound();
            }

            return View(bankList);
        }

        // POST: BankLists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.BankList == null)
            {
                return Problem("Entity set 'FincialWebApp1Context.BankList'  is null.");
            }
            var bankList = await _context.BankList.FindAsync(id);
            if (bankList != null)
            {
                _context.BankList.Remove(bankList);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BankListExists(int id)
        {
          return (_context.BankList?.Any(e => e.BankListId == id)).GetValueOrDefault();
        }
    }
}
