using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Twitter
{
    public class UsersModel : PageModel
    {
        private readonly SampleContext _db;
        public UsersModel(SampleContext db)
        {
            _db = db;
        }

        public IList<User> Users { get; set; }
        public User User { get; set; }
        public string sessionId { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Users = await _db.Users.ToListAsync();
            return Page();
        }


        public async Task<IActionResult> OnGetRemoveAsync([FromQuery] int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                _context.Users.Remove(user);
                _context.SaveChanges();
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                _log.LogError($"{ex.Message}");
            }
            return Page();
        }
    }
}
