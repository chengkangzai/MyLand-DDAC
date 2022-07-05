using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace MyLand.Areas.Identity.Data
{
    // Add profile data for application users by adding properties to the MyLandUser class
    public class MyLandUser : IdentityUser
    {
        [PersonalData]
        public string UserFirstName { get; set; }
        [PersonalData]
        public string UserLastName { get; set; }
        [PersonalData]
        public string UserAddress { get; set; }
        [PersonalData]
        public int UserTelephone { get; set; }
        [PersonalData]
        public int UserRole { get; set; }
    }
}
