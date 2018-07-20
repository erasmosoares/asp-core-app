namespace vega.Models
{
    public class Vehicle
    {
        public int Id { get; set; }
        public int ModelId { get; set; }

        public Model Model {get;set;}

        public bool IsRegistered { get; set; }

        public int ContactId {get;set;}
        public Contact Contact {get;set;}
    }
}