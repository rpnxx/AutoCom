using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using AutoComWeb.Data;
using AutoComWeb.Models;

namespace AutoComWeb.Pages.Client;

[Authorize(Roles = "Client")]
public class AddVehicleModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public AddVehicleModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public string Brand { get; set; } = string.Empty;

    [BindProperty]
    public string Model { get; set; } = string.Empty;

    [BindProperty]
    public int Year { get; set; } = DateTime.Now.Year;

    [BindProperty]
    public string NumberPlate { get; set; } = string.Empty;

    public string? ErrorMessage { get; set; }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        // 1. Узнаем, кто сейчас сидит на сайте
        var userIdClaim = User.FindFirst("UserId")?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
        {
            return RedirectToPage("/Account/Login");
        }

        // 2. Находим профиль этого клиента в базе
        var client = await _context.Clients.FirstOrDefaultAsync(c => c.UserId == userId);
        if (client == null)
        {
            ErrorMessage = "Ошибка: Профиль клиента не найден. Обратитесь к администратору.";
            return Page();
        }

        // 3. Собираем новую машину и ПРИВЯЗЫВАЕМ её к клиенту
        var vehicle = new Vehicle
        {
            Brand = Brand,
            Model = Model,
            Year = Year,
            NumberPlate = NumberPlate,
            ClientId = client.ClientId // Вот она, главная связь в базе данных!
        };

        try
        {
            // 4. Сохраняем в базу и кидаем обратно в гараж
            _context.Vehicles.Add(vehicle);
            await _context.SaveChangesAsync();

            return RedirectToPage("/Client/Index");
        }
        catch (Exception ex)
        {
            ErrorMessage = "Не удалось сохранить автомобиль. Проверьте правильность данных.";
            Console.WriteLine($"Ошибка БД: {ex.Message}");
            return Page();
        }
    }
}