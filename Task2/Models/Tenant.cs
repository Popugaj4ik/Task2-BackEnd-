namespace Task2.Models
{
    public class Tenant
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string PersonalID { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string? phoneNumber { get; set; }
        public string? EMail { get; set; }
        public long? FlatID { get; set; }
        public long UserOwnerID { get; set; }
    }
}
