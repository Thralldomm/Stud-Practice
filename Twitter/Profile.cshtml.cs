using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Twitter
{
    public class ProfileModel : PageModel
    {
        public void OnGet([FromRoute] int id)
        {
            var sessionId = HttpContext.Session.GetString("SampleSession");
            ProfileUser = _db.Users.Where(u => u.Id == id).Include(u => u.Microposts).FirstOrDefault();
            CurrentUser = _db.Users.Where(u => u.Id == Convert.ToInt32(sessionId)).FirstOrDefault();
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
                    UserId = CurrentUser.Id
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

        public async Task<IActionResult> OnPostAsync([FromRoute] int? id)
        {
            var sessionId = HttpContext.Session.GetString("SampleSession");
            ProfileUser = await _context.Users.Include(u => u.Microposts).FirstOrDefaultAsync(m => m.Id == id) as User;
            CurrentUser = await _context.Users.Include(u => u.Microposts).FirstOrDefaultAsync(m => m.Id.ToString() == sessionId) as User;

            // если текущий пользователь подписан на профиль пользователя
            var result = _context.Relations.Where(r => r.Follower == CurrentUser && r.Followed == ProfileUser).FirstOrDefault();

            if (result != null)
            {
                IsFollow = true;
            }
            else
            {
                IsFollow = false;
            }

            if (IsFollow == false)
            {
                try
                {
                    _context.Relations.Add(new Relation() { FollowerId = CurrentUser.Id, FollowedId = ProfileUser.Id });
                    _context.SaveChanges();
                    _f.Flash(Types.Success, $"Пользователь {CurrentUser.Name} подписался на {ProfileUser.Name}!", dismissable: true);
                }
                catch (Exception ex)
                {
                    _f.Flash(Types.Success, $"{ex.InnerException.Message}", dismissable: true);
                }
            }
            else
            {

                try
                {
                    var result2 = _context.Relations.Where(r => r.Follower == CurrentUser && r.Followed == ProfileUser).FirstOrDefault();
                    _context.Relations.Remove(result2);
                    _context.SaveChanges();
                    _f.Flash(Types.Warning, $"Пользователь {CurrentUser.Name} отписался от {ProfileUser.Name}!", dismissable: true);
                }
                catch (Exception ex)
                {
                    _f.Flash(Types.Success, $"{ex.Message}", dismissable: true);
                }


            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnGetAsync([FromRoute] int? id)
        {
            var sessionId = HttpContext.Session.GetString("SampleSession");
            ProfileUser = await _db.Users.Include(u => u.Microposts).FirstOrDefaultAsync(m => m.Id == id) as User;
            CurrentUser = await _db.Users.Include(u => u.Microposts).FirstOrDefaultAsync(m => m.Id.ToString() == sessionId) as User;

            // если текущий пользователь подписан на профиль пользователя
            var result = _db.Relations.Where(r => r.Follower == CurrentUser && r.Followed == ProfileUser).FirstOrDefault();

            if (result != null)
            {
                IsFollow = true;
            }
            else
            {
                IsFollow = false;
            }

            return Page();
        }
    }
}
