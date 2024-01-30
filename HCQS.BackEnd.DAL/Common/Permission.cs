namespace HCQS.BackEnd.DAL.Common
{
    public class Permission
    {
        public const string ADMIN = "ADMIN";
        public const string STAFF = "STAFF";
        public const string CUSTOMER = "CUSTOMER";

        public const string ALL = $"{ADMIN}, {STAFF},{CUSTOMER}";
        public const string MANAGEMENT = $"{ADMIN},{STAFF}";
    }
}