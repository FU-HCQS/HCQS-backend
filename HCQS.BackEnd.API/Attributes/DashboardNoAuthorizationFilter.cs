using Hangfire.Dashboard;

namespace HCQS.BackEnd.API.Attributes
{
    public class DashboardNoAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            return true;
        }
    }
}