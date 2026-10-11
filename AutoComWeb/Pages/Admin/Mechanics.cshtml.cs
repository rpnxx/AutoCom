using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using AutoComWeb.Data;
using AutoComWeb.Models;
using System.ComponentModel.DataAnnotations;

namespace AutoComWeb.Pages.Admin;

[Authorize(Roles = "Administrator")]
public class MechanicsModel : PageModel
{
    private readonly ApplicationDbContext _context;
    public MechanicsModel(ApplicationDbContext context) => _context = context;

    public List<Mechanic> MechanicsList { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string? SearchTerm { get; set; }

    [BindProperty]
    public int? EditingId { get; set; }

    [BindProperty]
    [Required(ErrorMessage = "Введите фамилию")]
    public string LastName { get; set; } = null!;

    [BindProperty]
    [Required(ErrorMessage = "Введите имя")]
    public string FirstName { get; set; } = null!;

    [BindProperty]
    public string? Patronymic { get; set; }

    [BindProperty]
    [Required(ErrorMessage = "Введите номер телефона")]
    public string Phone { get; set; } = null!;

    [BindProperty]
    [Required(ErrorMessage = "Укажите специализацию")]
    public string Specialization { get; set; } = null!;

    [TempData]
    public string? Message { get; set; }
    [TempData]
    public string? MessageType { get; set; }

    public async Task OnGetAsync()
    {
        var query = _context.Mechanics.AsQueryable();
        
        if (!string.IsNullOrWhiteSpace(SearchTerm))
        {
            query = query.Where(m => 
                m.LastName.ToLower().Contains(SearchTerm.ToLower()) || 
                m.Specialization.ToLower().Contains(SearchTerm.ToLower()));
        }
        
        // Сортируем по фамилии по алфавиту
        MechanicsList = await query.OrderBy(m => m.LastName).ToListAsync();
    }

    public async Task<IActionResult> OnPostSaveAsync()
    {
        if (!ModelState.IsValid)
        {
            await OnGetAsync();
            return Page();
        }

        // Очищаем номер телефона от маски перед сохранением в БД
        string cleanPhone = Phone.Replace(" ", "").Replace("(", "").Replace(")", "").Replace("-", "");

        if (EditingId.HasValue && EditingId.Value > 0)
        {
            var mechanic = await _context.Mechanics.FindAsync(EditingId.Value);
            if (mechanic != null)
            {
                mechanic.LastName = LastName;
                mechanic.FirstName = FirstName;
                mechanic.Patronymic = Patronymic;
                mechanic.Phone = cleanPhone;
                mechanic.Specialization = Specialization;
                Message = "Данные сотрудника обновлены";
            }
        }
        else
        {
            var newMechanic = new Mechanic 
            { 
                LastName = LastName, 
                FirstName = FirstName, 
                Patronymic = Patronymic, 
                Phone = cleanPhone, 
                Specialization = Specialization 
            };
            _context.Mechanics.Add(newMechanic);
            Message = "Новый сотрудник добавлен в базу";
        }

        await _context.SaveChangesAsync();
        MessageType = "success";
        return RedirectToPage(new { SearchTerm });
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var mechanic = await _context.Mechanics.Include(m => m.WorkOrders).FirstOrDefaultAsync(m => m.MechanicId == id);
        if (mechanic != null)
        {
            if (mechanic.WorkOrders.Any())
            {
                Message = "Нельзя удалить: у сотрудника есть заказ-наряды!";
                MessageType = "danger";
            }
            else
            {
                _context.Mechanics.Remove(mechanic);
                await _context.SaveChangesAsync();
                Message = "Сотрудник удален";
                MessageType = "success";
            }
        }
        return RedirectToPage(new { SearchTerm });
    }
}