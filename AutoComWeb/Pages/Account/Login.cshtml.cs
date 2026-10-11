using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using AutoComWeb.Services;

namespace AutoComWeb.Pages.Account;

public class LoginModel : PageModel
{
    private readonly UserService _userService;

    public LoginModel(UserService userService)
    {
        _userService = userService;
    }

    [BindProperty]
    public string Phone { get; set; } = string.Empty;

    [BindProperty]
    public string Password { get; set; } = string.Empty;

    public string? ErrorMessage { get; set; }

    public void OnGet()
    {
        // Срабатывает при простом открытии страницы
    }

    public async Task<IActionResult> OnPostAsync()
    {
        // Срабатывает при нажатии на кнопку "Войти"
         string cleanPhone = Phone.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "");
        var user = await _userService.AuthenticateAsync(cleanPhone, Password);

        if (user == null)
        {
            ErrorMessage = "Неверный номер телефона или пароль";
            return Page(); // Оставляем на странице с ошибкой
        }
        string displayName = user.Phone;
        if (user.Client != null && !string.IsNullOrEmpty(user.Client.FirstName))
        {
            displayName = user.Client.FirstName;
        }
        // 1. Создаем "паспортные данные" (Claims)
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, displayName),
            new Claim(ClaimTypes.Role, user.Role), 
            new Claim("UserId", user.UserId.ToString()) 
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        // 2. Выдаем куки и пускаем на сайт
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity));

        // 3. Перенаправляем по ролям (пока кидаем всех на главную, админку сделаем позже)
        if (user.Role == "Administrator")
        {
            return RedirectToPage("/Index"); // В будущем заменим на /Admin/Index
        }

        return RedirectToPage("/Index"); 
    }
}