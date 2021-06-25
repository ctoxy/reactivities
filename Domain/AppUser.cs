// creation des user appuyé par microsoft Identity

using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Domain
{
    public class AppUser : IdentityUser
    {
        public string DisplayName { get; set; }
        public string Bio { get; set; }
        //relation many to many user / activity
        public ICollection<ActivityAttendee> Activities { get; set; }
        //relation many to many user / photo
        public ICollection<Photo> Photos { get; set; }
    }
}