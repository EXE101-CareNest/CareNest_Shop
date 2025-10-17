using Shop.Domain.Commons;
using Shop.Domain.Commons.Enum;

namespace Shop.Domain.Entitites
{
    public class Shop : BaseEntity
    {
        /// <summary>
        /// Id tài khoản chủ shop
        /// </summary>
        public required string OwnerId { get; set; }
        /// <summary>
        /// tên shop
        /// </summary>
        public string? Name { get; set; }
        /// <summary>
        /// mô tả shop
        /// </summary>
        public string? Description { get; set; }
        /// <summary>
        ///   trạng thái shop
        /// </summary>
        public Status Status { get; set; }
        /// <summary>
        /// hình ảnh của shop
        /// </summary>
        public string? ImgUrl { get; set; }
        /// <summary>
        /// ngày làm việc 
        /// </summary>
        public string? WorkingDays { get; set; }
                // Thông tin tài khoản ngân hàng (1 shop = 1 tài khoản)
        public string? BankAccountName { get; set; }     // Tên chủ tài khoản
        public string? BankAccountNumber { get; set; }   // Số tài khoản
        public string? BankName { get; set; }            // Tên ngân hàng (VD: Vietcombank)
        public string? BankCode { get; set; }            // Mã ngân hàng (nếu dùng chuẩn nội bộ/SDK)
        public string? BankBranch { get; set; }          // Chi nhánh (nếu cần)
        public string? SwiftCode { get; set; }           // SWIFT (nếu có giao dịch quốc tế)
        public string? Note { get; set; }
    }
}
