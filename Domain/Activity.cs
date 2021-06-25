using System;
using System.Collections.Generic;

namespace Domain
{
    public class Activity
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public DateTime Date { get; set; }

        public string Description { get; set; }

        public string Category { get; set; }

        public string City { get; set; }

        public string Venue { get; set; }
        // permettra au createur de l activité de l annuller
        public bool IsCancelled { get; set; }
        //relation many to many activity / user le new list permet de remplir dans les requetes sinon erreur 500 car null par défaut dans 
        public ICollection<ActivityAttendee> Attendees { get; set; } = new List<ActivityAttendee>();
        //relation many to many activity / chat
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();

    }
}

