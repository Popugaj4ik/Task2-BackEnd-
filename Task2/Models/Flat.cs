namespace Task2.Models
{
    public class Flat
    {
        public long Id { get; set; }
        public int Floor { get; set; }
        public string? FlatNumber { get; set; }
        public int RoomsCount { get; set; }
        public float FullSpace { get; set; }
        public float LivingSpace { get; set; }
        public long? HouseID { get; set; }
        public long UserOwnerID { get; set; }
    }
}
