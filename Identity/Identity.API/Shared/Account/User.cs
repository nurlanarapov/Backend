﻿using Identity.API.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Identity.API.Shared.Account
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
        /// Токены пользователя
        /// </summary>
        public ICollection<RefreshTokens> RefreshTokens { get; set; }
    }
}
