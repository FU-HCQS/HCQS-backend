using HCQS.BackEnd.DAL.Models;
using Microsoft.AspNetCore.Identity;

namespace HCQS.BackEnd.Service.Dto
{
    public class AccountResponse
    {
        public Account User { get; set; }
        public IEnumerable<IdentityRole> Role { get; set; } = new List<IdentityRole>();
    }
}