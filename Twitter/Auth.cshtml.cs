using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Twitter
{
    public class AuthModel : PageModel
    {
        [BindProperty]
        public User Input { get; set; }
        public void OnGet()
        {
        }

        public IActionResult OnPost(User Input) // либо через свойство, либо через параметр
        {

            //ModelState.AddModelError(string.Empty,"Cannot convert currency to itself");

            if (!ModelState.IsValid)
            {
                _f.Flash(Types.Danger, $"Валидация не пройдена!", dismissable: true);
                return Page();
            }

            if (!Input.IsPasswordConfirmation())
            {
                _f.Flash(Types.Warning, $"Пароли должны совпадать!", dismissable: true);
                return Page();
            }

            try
            {
                _db.Users.Add(Input);
                _db.SaveChanges();

                _f.Flash(Types.Success, $"Пользователь {Input.Name} зарегистрирован!", dismissable: true);
                return RedirectToPage("./Auth");

            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message}");
                return RedirectToPage("./Sign");
            }
        }

        public IActionResult OnGetLogout()
        {
            // сброс сессии
            HttpContext.Session.Clear();

            return RedirectToPage("Auth");
        }
    }
}
