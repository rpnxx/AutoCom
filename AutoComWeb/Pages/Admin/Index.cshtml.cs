using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AutoComWeb.Pages.Admin;

[Authorize(Roles = "Administrator")]
public class IndexModel : PageModel
{
    public void OnGet()
    {
    }
}