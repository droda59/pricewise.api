using System;

namespace PriceAlerts.Common.Models
{
    public class PriceChange : IComparable<PriceChange>
    {
        public decimal Price { get; set; }

        public string ProductId { get; set; }

        public DateTime ModifiedAt { get; set; }

        int IComparable<PriceChange>.CompareTo(PriceChange other)
        {
            if (other.Price > this.Price)
            {
                return -1;
            }
            else if (other.Price == this.Price)
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }
    }
}