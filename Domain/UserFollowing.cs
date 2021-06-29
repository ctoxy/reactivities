namespace Domain
{
    public class UserFollowing
    {
        public string ObserverId { get; set; }
        //personne qui suis une autre personne
        public AppUser Observer { get; set; }
        public string TargetId { get; set; }
        public AppUser Target { get; set; }
    }
}