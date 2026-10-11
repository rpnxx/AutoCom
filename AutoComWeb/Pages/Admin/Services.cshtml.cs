using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using AutoComWeb.Data;
using AutoComWeb.Models;
using System.ComponentModel.DataAnnotations;

namespace AutoComWeb.Pages.Admin;

[Authorize(Roles = "Administrator")]
public class ServicesModel : PageModel
{
    private readonly ApplicationDbContext _context;
    public ServicesModel(ApplicationDbContext context) => _context = context;

    public List<Service> ServicesList { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string? SearchTerm { get; set; }

    [BindProperty]
    public int? EditingId { get; set; }

    [BindProperty]
    [Required(ErrorMessage = "Введите название")]
    public string Name { get; set; } = null!;

    [BindProperty]
    [Required(ErrorMessage = "Введите цену")]
    [Range(1, 1000000, ErrorMessage = "Цена: от 1 до 1 000 000 руб.")]
    // Регулярное выражение: только целые числа
    [RegularExpression(@"^\d+$", ErrorMessage = "Цена должна быть целым числом")]
    public decimal Price { get; set; } = 1000; // Значение по умолчанию

    [BindProperty]
    [Required(ErrorMessage = "Введите нормо-часы")]
    [Range(0.5, 45.0, ErrorMessage = "Нормо-часы: от 0.5 до 45.0")]
    // Регулярное выражение: число с максимум одним знаком после точки/запятой
    [RegularExpression(@"^\d+([.,]\d{1})?$", ErrorMessage = "Максимум один знак после запятой")]
    public decimal StandartHours { get; set; } = 1.0m; // Значение по умолчанию

    [TempData]
    public string? Message { get; set; }
    [TempData]
    public string? MessageType { get; set; }

    public async Task OnGetAsync()
    {
        var query = _context.Services.AsQueryable();
        if (!string.IsNullOrWhiteSpace(SearchTerm))
            query = query.Where(s => s.Name.ToLower().Contains(SearchTerm.ToLower()));
        
        ServicesList = await query.OrderBy(s => s.Name).ToListAsync();
    }

    public async Task<IActionResult> OnPostSaveAsync()
    {
        // Принудительно округляем перед сохранением, чтобы соответствовать логике
        Price = Math.Round(Price, 0);
        StandartHours = Math.Round(StandartHours, 1);

        if (!ModelState.IsValid)
        {
            await OnGetAsync();
            return Page();
        }

        if (EditingId.HasValue && EditingId.Value > 0)
        {
            var service = await _context.Services.FindAsync(EditingId.Value);
            if (service != null)
            {
                service.Name = Name;
                service.Price = Price;
                service.StandartHours = StandartHours;
                Message = "Услуга обновлена";
            }
        }
        else
        {
            _context.Services.Add(new Service { Name = Name, Price = Price, StandartHours = StandartHours });
            Message = "Услуга добавлена";
        }

        await _context.SaveChangesAsync();
        MessageType = "success";
        return RedirectToPage(new { SearchTerm });
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var service = await _context.Services.Include(s => s.WorkOrderServices).FirstOrDefaultAsync(s => s.ServicesId == id);
        if (service != null)
        {
            if (service.WorkOrderServices.Any())
            {
                Message = "Нельзя удалить: услуга используется в заказ-нарядах";
                MessageType = "danger";
            }
            else
            {
                _context.Services.Remove(service);
                await _context.SaveChangesAsync();
                Message = "Услуга удалена";
                MessageType = "success";
            }
        }
        return RedirectToPage(new { SearchTerm });
    }
}