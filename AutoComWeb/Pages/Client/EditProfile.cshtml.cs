using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using AutoComWeb.Data;

namespace AutoComWeb.Pages.Client;

[Authorize(Roles = "Client")]
public class EditProfileModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public EditProfileModel(ApplicationDbContext context)
    {
        _context = context;
    }

    // Эти атрибуты заставят сервер автоматически проверять правильность ввода
    [BindProperty]
    [Required(ErrorMessage = "Имя обязательно для заполнения")]
    public string FirstName { get; set; } = string.Empty;

    [BindProperty]
    [Required(ErrorMessage = "Фамилия обязательна для заполнения")]
    public string LastName { get; set; } = string.Empty;

    [BindProperty]
    [EmailAddress(ErrorMessage = "Неверный формат электронной почты")]
    public string? Email { get; set; }

    // OnGet срабатывает при открытии страницы. Мы достаем старые данные, чтобы заполнить поля
    public async Task<IActionResult> OnGetAsync()
    {
        var userIdClaim = User.FindFirst("UserId")?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId)) return RedirectToPage("/Account/Login");

        var client = await _context.Clients.FirstOrDefaultAsync(c => c.UserId == userId);
        if (client != null)
        {
            FirstName = client.FirstName ?? "";
            LastName = client.LastName ?? "";
            Email = client.Email;
        }
        
        return Page();
    }

    // OnPost срабатывает при сохранении формы
    public async Task<IActionResult> OnPostAsync()
    {
        // Проверяем, не нарушил ли пользователь правила [Required] и [EmailAddress]
        if (!ModelState.IsValid) 
        {
            return Page(); 
        }

        var userIdClaim = User.FindFirst("UserId")?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId)) return RedirectToPage("/Account/Login");

        var client = await _context.Clients.FirstOrDefaultAsync(c => c.UserId == userId);
        if (client == null) return RedirectToPage("/Account/Login");

        // Обновляем данные
        client.FirstName = FirstName;
        client.LastName = LastName;
        client.Email = Email;

        await _context.SaveChangesAsync();

        // Оставляем сообщение об успехе для Личного кабинета
        TempData["SuccessMessage"] = "Ваш профиль успешно обновлен!";
        
        return RedirectToPage("/Client/Index");
    }
}