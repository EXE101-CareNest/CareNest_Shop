using Microsoft.EntityFrameworkCore;
using Shop.Domain.Commons.Enum;
using Shop.Domain.Entitites;

namespace Shop.Infrastructure.Persistences.Database
{
    public class DatabaseSeeder
    {
        public static async Task SeedAsync(DatabaseContext context)
        {
            // Kiểm tra xem đã có dữ liệu shop chưa
            if (await context.Shops.AnyAsync())
            {
                return; // Đã có dữ liệu, không cần seed
            }

            // Tạo hai shop mẫu về thú cưng
            var shops = new List<Shop.Domain.Entitites.Shop>
            {
                new Domain.Entitites.Shop
                {
                    Id = Guid.NewGuid().ToString("N"),
                    OwnerId = "owner-001",
                    Name = "Pet Paradise Store",
                    Description = "Cửa hàng thú cưng chuyên cung cấp thức ăn, đồ chơi, phụ kiện và dịch vụ chăm sóc cho chó, mèo và các thú cưng khác",
                    Status = Status.Active,
                    ImgUrl = "https://example.com/images/pet-paradise.jpg",
                    WorkingDays = "Thứ 2 - Chủ nhật: 8:00 - 21:00",
                    CreatedAt = DateTimeOffset.UtcNow,
                    CreatedBy = "system"
                },
                new Domain.Entitites.Shop
                {
                    Id = Guid.NewGuid().ToString("N"),
                    OwnerId = "owner-002",
                    Name = "Furry Friends Shop",
                    Description = "Cửa hàng chuyên về thú cưng với đầy đủ các sản phẩm từ thức ăn cao cấp, đồ chơi, chuồng nuôi đến dịch vụ spa và grooming",
                    Status = Status.Active,
                    ImgUrl = "https://example.com/images/furry-friends.jpg",
                    WorkingDays = "Thứ 2 - Thứ 6: 9:00 - 20:00, Thứ 7 - CN: 10:00 - 19:00",
                    CreatedAt = DateTimeOffset.UtcNow,
                    CreatedBy = "system"
                }
            };

            // Thêm vào database
            await context.Shops.AddRangeAsync(shops);
            await context.SaveChangesAsync();
        }
    }
}
