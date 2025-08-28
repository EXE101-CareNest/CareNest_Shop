using System.ComponentModel;

namespace Shop.Domain.Commons.Enum
{
    public enum Status
    {
        [Description("Active")]
        Active = 1,

        [Description("Inactive")]
        Inactive = 0
    }
}
