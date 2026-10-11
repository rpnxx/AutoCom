using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AutoComWeb.Pages.Account;

public class LogoutModel : PageModel
{
    
    public async Task<IActionResult> OnGetAsync()
    {
        // уничтожить куки-файл
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        // на главную 
        return RedirectToPage("/Index");
    }
}