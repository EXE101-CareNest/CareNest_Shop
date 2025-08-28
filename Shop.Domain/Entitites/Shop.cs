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
    }
}
