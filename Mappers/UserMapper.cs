using RestAPI.Models;
using RestAPI.DTOs;

namespace RestAPI.Mappers
{
    public static class UserMapper //static for performance , since no data is stored (has downsides though)
    {
        //entity -> DTO (for get requests data to client)
        public static UserResponseDto ToResponseDto(User user)
        {
            return new UserResponseDto
            {
                Id = user.Id,
                Username = user.Username,
                MemberSince = user.CreatedAt //renamed field

            };
        }

        public static User FromSignupDto(SignupDto dto)
        {
            return new User
            {
                Username = dto.Username,
                Password = dto.Password,
                CreatedAt = DateTime.UtcNow //id is auto generated
            };
        }

        //entity -> summary DTO 
        public static UserSummaryDto ToSummaryDto(User user)
        {
            return new UserSummaryDto
            {
                Id = user.Id,
                Username = user.Username,
                MemberSince = user.CreatedAt
            };
        }

        //list mapping  - entity collection -> dto
        public static List<UserResponseDto> ToResponseDtoList(IEnumerable<User> users)
        {
            return users.Select(ToResponseDto).ToList();
        }

        //list mapping for summary
        public static List<UserSummaryDto> ToSummaryDtoList(IEnumerable<User> users)
        {
            return users.Select(ToSummaryDto).ToList();
        }

        //existing entity  to eto (put)
        public static void UpdateFromDto(User user, UpdateUserDto dto)
        {
            if (!string.IsNullOrEmpty(dto.NewUsername))
            {
                user.Username = dto.NewUsername;
            }

            if (!string.IsNullOrEmpty(dto.NewPassword))
            {
                user.Password = dto.NewPassword;

            }
        }


        //creating authentication response with user info
        public static AuthResponseDto ToAuthResponse(User user, string token, string message)
        {
            return new AuthResponseDto
            {
                Message = message,
                Token = token,
                User = ToResponseDto(user)
            };
        }
    }
}