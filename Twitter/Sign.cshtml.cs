using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Twitter
{
    public class SignModel : PageModel
    {

		private readonly SampleAppContext _db;
		private readonly ILogger<SignModel> _log;
		IFlasher f;
		public SignModel(SampleAppContext db, ILogger<SignModel> log)
		{
			_db = db;
			_log = log;
			this.f = f;
		}

		public void OnGet()
        {


        } 

		public IActionResult OnPost(User user)
		{
			try
			{
				_db.Users.Add(user);
				_db.SaveChanges();
				_log.LogInformation($"Пользователь {user.Name} успешно зарегистрирован!");
				return RedirectToPage("./Index");
			}
			catch (Exception ex)
			{
				_log.LogError($"Ошибка: {ex.InnerException}");
				return RedirectToPage("./Sign");
			}
		}

		

		public IActionResult YourAction()
		{
			f.Flash(Types.Success, "Flash message system for ASP.NET MVC Core", dismissable: true);
			f.Flash(Types.Danger, "Flash message system for ASP.NET MVC Core", dismissable: false);
			return RedirectToAction("AnotherAction");
		}
	}
}
