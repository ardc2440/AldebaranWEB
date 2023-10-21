using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aldebaran.Web.Models.AldebaranDb
{
    [Table("aspnetusers", Schema = "dbo")]
    public partial class Aspnetuser
    {
        [Key]
        [Required]
        public string Id { get; set; }

        [Required]
        public int AccessFailedCount { get; set; }

        public string ConcurrencyStamp { get; set; }

        public string Email { get; set; }

        public bool EmailConfirmed { get; set; }

        public bool LockoutEnabled { get; set; }

        public DateTimeOffset? LockoutEnd { get; set; }

        public string NormalizedEmail { get; set; }

        public string NormalizedUserName { get; set; }

        public string PasswordHash { get; set; }

        public string PhoneNumber { get; set; }

        public bool PhoneNumberConfirmed { get; set; }

        public string SecurityStamp { get; set; }

        public bool TwoFactorEnabled { get; set; }

        public string UserName { get; set; }

        [Required]
        public int IDENTITY_TYPE_ID { get; set; }

        [Required]
        public string IDENTITY_NUMBER { get; set; }

        [Required]
        public string FULL_NAME { get; set; }

        [Required]
        public string POSITION { get; set; }

        [Required]
        public short AREA_ID { get; set; }

        public ICollection<Adjustment> Adjustments { get; set; }

        public Area Area { get; set; }

        public IdentityType IdentityType { get; set; }

    }
}