using Shop.Application.Features.Commands.Create;
using Shop.Application.Features.Commands.Update;
using Shop.Domain.Commons.Constant;
using System.Text.RegularExpressions;

namespace Shop.Application.Exceptions.Validators
{
    public class ShopValidate
    {
        /// <summary>
        /// kiểm tra toàn bộ tạo shop
        /// </summary>
        /// <param name="command"></param>
        public static void ValidateCreate(CreateCommand command)
        {
            ValidateName(command.Name);
            ValidateDescription(command.Description);
            ValidateOwnerId(command.OwnerId);
            ValidateWorkingDays(command.WorkingDays);
        }

        public static void ValidateUpdate(UpdateCommand command)
        {
            ValidateName(command.Name);
            ValidateDescription(command.Description);
            ValidateOwnerId(command.OwnerId);
            ValidateWorkingDays(command.WorkingDays);
        }
        public static void ValidateName(string? name)
        {
            //-Không được để trống.
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new BadRequestException(MessageConstant.MissingName);
            }
            //- Giới hạn độ dài(ví dụ 1 - 100 ký tự).
            if (name.Length == 0 || name.Length > 100)
            {
                throw new BadRequestException(MessageConstant.Exceed100CharsName);
            }
            //- Không chứa ký tự đặc biệt (!@#$^*&<>?)
            if (!Regex.IsMatch(name, @"^[a-zA-Z0-9\s]+$"))
            {
                throw new BadRequestException(MessageConstant.SpecialCharacterName);
            }
        }

        public static void ValidateOwnerId(string? id)
        {
            // id của chủ shop không được trống
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new BadRequestException(MessageConstant.MissingOwnerId);
            }
        }

        public static void ValidateDescription(string? description)
        {
            // mô tả có thể trống 
            // mô tả không quá  500 ký tự 
            if (!string.IsNullOrWhiteSpace(description) && description.Length > 500)
            {
                throw new BadRequestException(MessageConstant.Exceed500CharsDescription);
            }
        }

        public static void ValidateWorkingDays(string? workingDay)
        {
            // ngày làm không được trống
            if (string.IsNullOrWhiteSpace(workingDay))
            {
                throw new BadRequestException(MessageConstant.MissingWorkDay);
            }
            // Regex mô tả:
            // - Một hoặc nhiều nhóm "Ngày or Khoảng-Ngày khoảng trắng  HH:mm-HH:mm;"
            // - Ngày viết tắt: Mon, Tue, Wed, Thu, Fri, Sat, Sun
            // - Khoảng ngày Mon-Fri hoặc đơn lẻ Mon
            // - Giờ 24h định dạng HH:mm
            // - Các cặp cách nhau dấu ;
            // - Ví dụ: Mon-Fri 08:00 - 19:00 hoặc Mon 07:30-17:30
            string pattern = @"^(\s*(Mon|Tue|Wed|Thu|Fri|Sat|Sun)(-(Mon|Tue|Wed|Thu|Fri|Sat|Sun))?\s+\d{2}:\d{2}-\d{2}:\d{2}\s*;)*(\s*(Mon|Tue|Wed|Thu|Fri|Sat|Sun)(-(Mon|Tue|Wed|Thu|Fri|Sat|Sun))?\s+\d{2}:\d{2}-\d{2}:\d{2}\s*)$";
            if (!Regex.IsMatch(workingDay, pattern, RegexOptions.IgnoreCase))
            {
                throw new BadRequestException(MessageConstant.NotMatchFormatWorkDay);
            }
        }
    }
}
