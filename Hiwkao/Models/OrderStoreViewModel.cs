namespace Hiwkao.Models
{
    public class OrderStoreViewModel
    {
        public IEnumerable<SampleLogin.Models.Order> Orders { get; set; }
        public IEnumerable<SampleLogin.Models.Store> Stores { get; set; }
    }
}
