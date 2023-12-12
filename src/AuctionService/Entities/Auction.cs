using System.ComponentModel.DataAnnotations.Schema;

namespace AuctionService.Entities
{
    [Table("Auctions")]
    public class Auction
    {
        public Guid Id { get; set; }

        public int ReservePrice { get; set; } = 0;

        public string Seller { get; set; }

        public string Winner { get; set; }

        public int? SoldAmount { get; set; }

        public int? CurrentHighBid { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime LastUpdatedAt { get; set; } = DateTime.UtcNow;

        public DateTime AuctionEndsAt { get; set; }

        public Status Status { get; set; }

        // Navigation properties
        public Item Item { get; set; }

        public Guid ItemId { get; set; }




    }
}
