using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AutoComWeb.Services;

namespace AutoComWeb.Pages.Account;

public class RegisterModel : PageModel
{
    private readonly UserService _userService;

    public RegisterModel(UserService userService)
    {
        _userService = userService;
    }

[BindProperty]
public string Phone { get; set; } = string.Empty;

[BindProperty]
public string Password { get; set; } = string.Empty;

[BindProperty]
public string ConfirmPassword { get; set; } = string.Empty;

[BindProperty]
public string FirstName { get; set; } = string.Empty;

[BindProperty]
public string LastName { get; set; } = string.Empty;

    public string? ErrorMessage { get; set; }

    public void OnGet()
    {
        // Срабатывает при простом открытии страницы
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (Password != ConfirmPassword)
        {
            ErrorMessage = "Пароли не совпадают, попробуйте снова.";
            return Page(); // Оставляем на странице с ошибкой
        }
    string cleanPhone = Phone.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", ""); 
    var Role = "Client"; // Роль по умолчанию для новых пользователей
    try
    {
        var user = await _userService.RegisterAsync(cleanPhone, Password, Role, FirstName, LastName);
        return RedirectToPage("/Account/Login");
    }
    catch (InvalidOperationException)
    {
        ErrorMessage = "Пользователь с таким номером телефона уже существует.";
        return Page(); // Оставляем на странице с ошибкой
    }
    }
}