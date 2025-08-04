using EventManagementSystem.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventManagementSystem.Application.DTO
{
    internal class UserDTO : IsDto
    {
        public Guid UserId { get; set; }

        public string Email { get; set; } = string.Empty;

        public string UserName { get; set; } = string.Empty;

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string userRole { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;
    }
}
