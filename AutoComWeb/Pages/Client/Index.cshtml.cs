using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using AutoComWeb.Data;
using AutoComWeb.Models;
using System.Security.Claims;

namespace AutoComWeb.Pages.Client;

// 1. Ставим замок: сюда пускают ТОЛЬКО пользователей с ролью Client
[Authorize(Roles = "Client")]
public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    // Сюда мы запишем данные клиента, чтобы отобразить их на странице
    public Models.Client? ClientInfo { get; set; }
    public List<Vehicle> Vehicles { get; set; } = new();

    [TempData]
    public string? ErrorMessage { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        // 2. Достаем ID пользователя из Claims (наш зашифрованный UserId в куки)
        var userIdClaim = User.FindFirst("UserId")?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
        {
            return RedirectToPage("/Account/Login");
        }

        // 3. Ищем в базе запись клиента, связанную с этим UserId
        // Подтягиваем (Include) сразу и список его автомобилей
        ClientInfo = await _context.Clients
            .Include(c => c.Vehicles)
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (ClientInfo == null)
        {
            // Если учетка есть, а профиль клиента еще не создан (бывает при первой регистрации)
            return Page(); 
        }

        Vehicles = ClientInfo.Vehicles.ToList();

        return Page();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int vehicleId)
    {
        // 1. Проверяем кто удаляет
        var userIdClaim = User.FindFirst("UserId")?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId)) return RedirectToPage("/Account/Login");

        var client = await _context.Clients.FirstOrDefaultAsync(c => c.UserId == userId);
        if (client == null) return RedirectToPage("/Account/Login");

        // 2. Ищем машину. ВАЖНО: проверяем, что она принадлежит именно этому клиенту!
        var vehicle = await _context.Vehicles.FirstOrDefaultAsync(v => v.VehicleId == vehicleId && v.ClientId == client.ClientId);

        if (vehicle != null)
        {
            try
            {
                // 3. Пытаемся удалить
                _context.Vehicles.Remove(vehicle);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                // 4. Если БД выдала ошибку (например, на машину уже заведен Заказ-наряд)
                ErrorMessage = $"Невозможно удалить {vehicle.Brand} {vehicle.NumberPlate}, так как по нему есть история ремонтов. Обратитесь к администратору.";
            }
        }

        // Обновляем текущую страницу
        return RedirectToPage();
    }
}