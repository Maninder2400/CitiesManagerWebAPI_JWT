using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace CitiesManager.Core.Identity
{
    public class ApplicationUser  : IdentityUser<Guid>
    {
        public string? PersonName { get; set; }
        public string? RefreshToken { get; set; }
         
        public  DateTime RefreshTokenExpirationDateTime { get; set; }

    }
}
