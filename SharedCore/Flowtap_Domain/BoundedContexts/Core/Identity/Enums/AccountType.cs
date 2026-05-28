namespace Flowtap_Domain.BoundedContexts.Core.Identity.Enums;

public enum AccountType
{
    SuperAdmin = 0,   // platform-level — manages all companies and users
    Owner = 1,
    Admin = 2,
    Staff = 3
}
