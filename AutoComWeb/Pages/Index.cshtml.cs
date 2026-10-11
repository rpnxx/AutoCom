using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AutoComWeb.Pages;

public class IndexModel : PageModel
{
    public IActionResult OnGet()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            if (User.IsInRole("Administrator"))
                return RedirectToPage("/Admin/Index");
            else if (User.IsInRole("Client"))
                return RedirectToPage("/Client/Index");
        }
        return Page();
    }
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

}
