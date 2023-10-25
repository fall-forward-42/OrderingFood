using Microsoft.AspNetCore.Mvc.RazorPages;

namespace OrderingFood.Models
{
    public class IndexModel: PageModel   
    {
        public const string sessionIdUser = "_idUser";
        public const string SessionKeyAge = "_Age";

        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString(sessionIdUser)))
            {
                HttpContext.Session.SetString(sessionIdUser, "none");
            }
            var idUser = HttpContext.Session.GetString(sessionIdUser);
            var age = HttpContext.Session.GetInt32(SessionKeyAge).ToString();

            _logger.LogInformation("Session Name: {Name}", idUser);
            _logger.LogInformation("Session Age: {Age}", age);
        }
    }
}
