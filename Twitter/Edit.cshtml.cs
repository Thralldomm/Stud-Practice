using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Twitter
{
    public class EditModel : PageModel
    {
        private readonly SampleContext _context;
        private readonly IFlasher _f;
        private readonly ILogger<EditModel> _log;

        public EditModel(SampleContext context, IFlasher f, ILogger<EditModel> log)
        {
            _context = context;
            _f = f;
            _log = log;
        }

        [BindProperty]
        public User User { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {

                return NotFound();
            }

            User = await _context.Users.FirstOrDefaultAsync(m => m.Id == id);

            if (User == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {

            _context.Attach(User).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(User.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
