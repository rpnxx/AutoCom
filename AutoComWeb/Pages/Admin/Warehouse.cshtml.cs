using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using AutoComWeb.Data;
using AutoComWeb.Models;
using System.ComponentModel.DataAnnotations;

namespace AutoComWeb.Pages.Admin;

[Authorize(Roles = "Administrator")]
public class WarehouseModel : PageModel
{
    private readonly ApplicationDbContext _context;
    public WarehouseModel(ApplicationDbContext context) => _context = context;

    public List<SparePart> SparePartsList { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string? SearchTerm { get; set; }

    [BindProperty]
    public int? EditingId { get; set; }

    [BindProperty]
    [Required(ErrorMessage = "Введите название запчасти")]
    public string Name { get; set; } = null!;

    [BindProperty]
    [Required(ErrorMessage = "Введите артикул")]
    [RegularExpression(@"^[a-zA-Z0-9\-\/]+$", ErrorMessage = "Только английские буквы, цифры, дефис и слэш")]
    public string Article { get; set; } = null!;

    [BindProperty]
    [Required(ErrorMessage = "Укажите цену")]
    [Range(1, 1000000, ErrorMessage = "Цена: от 1 до 1 000 000 руб.")]
    [RegularExpression(@"^\d+$", ErrorMessage = "Только целые числа")]
    public decimal Price { get; set; }

    [BindProperty]
    [Required(ErrorMessage = "Укажите количество")]
    [Range(0, 10000, ErrorMessage = "Количество: от 0 до 10 000")]
    public int Quantity { get; set; }

    [TempData]
    public string? Message { get; set; }
    [TempData]
    public string? MessageType { get; set; }

    public async Task OnGetAsync()
    {
        var query = _context.SpareParts.AsQueryable();
        
        if (!string.IsNullOrWhiteSpace(SearchTerm))
        {
            query = query.Where(p => 
                p.Name.ToLower().Contains(SearchTerm.ToLower()) || 
                p.Article.ToLower().Contains(SearchTerm.ToLower()));
        }
        
        // Сортируем по названию
        SparePartsList = await query.OrderBy(p => p.Name).ToListAsync();
    }

    public async Task<IActionResult> OnPostSaveAsync()
    {
        Price = Math.Round(Price, 0);

        if (!ModelState.IsValid)
        {
            await OnGetAsync();
            return Page();
        }

        if (EditingId.HasValue && EditingId.Value > 0)
        {
            // РЕЖИМ 1: Прямое редактирование через кнопку "Карандаш"
            var part = await _context.SpareParts.FindAsync(EditingId.Value);
            if (part != null)
            {
                part.Name = Name;
                part.Article = Article;
                part.Price = Price;
                part.Quantity = Quantity; // Здесь жестко ПЕРЕЗАПИСЫВАЕМ количество
                Message = $"Деталь '{Article}' обновлена";
            }
        }
        else
        {
            // РЕЖИМ 2: Добавление через форму слева (Новая деталь или Приход)
            
            // Ищем, есть ли уже деталь с таким же Артикулом (без учета регистра букв)
            var existingPart = await _context.SpareParts
                .FirstOrDefaultAsync(p => p.Article.ToLower() == Article.ToLower());

            if (existingPart != null)
            {
                // УМНОЕ СЛИЯНИЕ (Приход товара)
                existingPart.Quantity += Quantity; // ПРИБАВЛЯЕМ новое количество к старому
                existingPart.Price = Price;        // Обновляем цену до актуальной
                existingPart.Name = Name;          // На всякий случай обновляем название, если админ поправил опечатку
                
                Message = $"Приход на склад! К детали '{Article}' добавлено +{Quantity} шт. (Новый остаток: {existingPart.Quantity})";
            }
            else
            {
                // СОЗДАНИЕ АБСОЛЮТНО НОВОЙ ДЕТАЛИ
                var newPart = new SparePart 
                { 
                    Name = Name, 
                    Article = Article, 
                    Price = Price, 
                    Quantity = Quantity 
                };
                _context.SpareParts.Add(newPart);
                Message = $"Новый артикул '{Article}' добавлен на склад";
            }
        }

        await _context.SaveChangesAsync();
        MessageType = "success";
        return RedirectToPage(new { SearchTerm });
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var part = await _context.SpareParts.Include(p => p.WorkOrderParts).FirstOrDefaultAsync(p => p.SparePartsId == id);
        if (part != null)
        {
            if (part.WorkOrderParts.Any())
            {
                Message = "Нельзя удалить: деталь использовалась в заказ-нарядах!";
                MessageType = "danger";
            }
            else
            {
                _context.SpareParts.Remove(part);
                await _context.SaveChangesAsync();
                Message = "Деталь удалена со склада";
                MessageType = "success";
            }
        }
        return RedirectToPage(new { SearchTerm });
    }
}