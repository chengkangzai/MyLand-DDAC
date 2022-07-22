using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using MyLand.Models;

namespace MyLand.Areas.Identity.Data
{
    // Add profile data for application users by adding properties to the MyLandUser class
    public class MyLandUser : IdentityUser
    {
        public const int ROLE_CUSTOMER = 0;
        public const int ROLE_MODERATOR = 1;
        public const int ROLE_AGENT = 2;
        [PersonalData]
        public string FirstName { get; set; }
        [PersonalData]
        public string LastName { get; set; }
        [PersonalData]
        public string Address { get; set; }
        [PersonalData]
        public int Telephone { get; set; }
        [PersonalData]
        public int Role { get; set; }
        [PersonalData]
        public virtual ICollection<Property> Properties { get; set; }
    }
}
