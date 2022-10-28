using Identity.API.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Identity.API.Models.DtoModels.Request
{
    public class User
    {
        /// <summary>
        /// Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Имя
        /// </summary>
        [Required]
        [MinLength(3)]
        [MaxLength(50)]
        public string UserName { get; set; }

        /// <summary>
        /// Имя
        /// </summary>
        [Required]
        [MinLength(3)]
        [MaxLength(50)]
        public string Name { get; set; }

        /// <summary>
        /// Фамилия
        /// </summary>
        [Required]
        [MinLength(3)]
        [MaxLength(100)]
        public string SurName { get; set; }

        /// <summary>
        /// Отчество
        /// </summary>
        [MaxLength(100)]
        public string MiddleName { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        [EmailAddress]
        public string Email { get; set; }

        /// <summary>
        /// Номер телефона
        /// </summary>
        [Phone]
        public string PhoneNumber { get; set; }

        [Required]
        [MinLength(8)]
        [MaxLength(50)]
        public string Password { get; set; }
    }
}
