using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace BackEnd.Models
{
    /// <summary>
    /// ���. ���������� �������������
    /// </summary>
    public class AppUser : IdentityUser
    {
        /// <summary>
        /// ���
        /// </summary>
        [Required]
        [MinLength(3)]
        [MaxLength(50)]
        public string Name { get; set; }

        /// <summary>
        /// �������
        /// </summary>
        [Required]
        [MinLength(3)]
        [MaxLength(100)]
        public string SurName { get; set; }

        /// <summary>
        /// ��������
        /// </summary>
        [MaxLength(100)]
        public string MiddleName { get; set; }
    }
}