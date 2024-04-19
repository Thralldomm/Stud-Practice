using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Twitter
{
    public class _HeaderModel : PageModel
    {
        SamplaAppContext _db= new SamplaAppContext();
        private string? sessionId;

        public IEnumerable<User> Followeds { get; set; }
        public List<User> Users { get; set; } = new();
        public List<Micropost> Messages { get; set; } = new();
        public User CurrentUser { get;  set; }

        public void OnGet()
        {
            var sessionId = HttpContext.Session.GetString("SampleSession");
            CurrentUser = _db.Users.Where(u => u.Id == Convert.ToInt32(sessionId)).Include(u => u.Microposts).FirstOrDefault();
        }

        public async Task<IActionResult> OnGetAsync()
        {

            sessionId = HttpContext.Session.GetString("SampleSession");

            if (sessionId != null)
            {
                User = await _db.Users.Include(u => u.Microposts)
                                      .Include(u => u.RelationFollowers).ThenInclude(r => r.Followed)
                                      .FirstOrDefaultAsync(m => m.Id == Convert.ToInt32(sessionId));

                Followeds = User.RelationFollowers.Select(item => item.Followed).ToList();

                Users.AddRange(Followeds);
                Users.Add(User);

                foreach (var u in Users)
                {
                    _db.Entry(u).Collection(u => u.Microposts).Load();
                    Messages.AddRange(u.Microposts);
                }
                return Page();
            }
            else
            {
                return RedirectToPage("Auth");
            }

        }
        public async Task<IActionResult> OnPostAsync(string message)
        {
            var sessionId = HttpContext.Session.GetString("SampleSession");
            CurrentUser = await _db.Users.Include(u => u.Microposts).FirstOrDefaultAsync(m => m.Id == Convert.ToInt32(sessionId));

            if (!string.IsNullOrWhiteSpace(message))
            {
                var m = new Micropost()
                {
                    Content = message,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    UserId = CurrentUser.Id,
                    //User = this.User
                };

                try
                {
                    _db.Microposts.Add(m);
                    _db.SaveChanges();
                    _f.Flash(Types.Success, $"Tweet!", dismissable: true);
                    return RedirectToPage();
                }
                catch (Exception ex)
                {
                    _log.Log(LogLevel.Error, $"Ошибка создания сообщения: {ex.InnerException.Message}");
                }


                return Page();
            }
            else
            {
                return Page();
            }

        }

        public async Task<IActionResult> OnGetDeleteAsync([FromQuery] int messageid)
        {
            var sessionId = HttpContext.Session.GetString("SampleSession");
            CurrentUser = await _db.Users.Include(u => u.Microposts).FirstOrDefaultAsync(m => m.Id == Convert.ToInt32(sessionId));

            try
            {
                Micropost m = _db.Microposts.Find(messageid);
                _db.Microposts.Remove(m);
                _db.SaveChanges();
                _log.Log(LogLevel.Error, $"Удалено сообщение \"{m.Content}\" пользователя {CurrentUser.Name}!");
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                _log.Log(LogLevel.Error, $"Ошибка удаления сообщения: {ex.InnerException}");
                _log.Log(LogLevel.Error, $"Модель привязки из маршрута: {messageid}");
            }

            return Page();

        }
    }
}
