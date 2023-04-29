using System.ComponentModel.DataAnnotations;

namespace SampleLogin.Models
{
    public class Store
    {
        [Key]
        public int id { get; set; }


        public string name { get; set; }
        public string destination { get; set; }

        public string StoreImage { get; set; }

    }
}
