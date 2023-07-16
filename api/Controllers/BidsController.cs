using Microsoft.AspNetCore.Mvc;

public class BidModel
{
    public string LotId { get; set; }

    public decimal BidAmount { get; set; }
}

[ApiController]
[Route("[controller]")]
public class BidsController : ControllerBase
{
    private readonly IAuctionLotService _auctionLotService;

    public BidsController(IAuctionLotService auctionLotService)
    {
        _auctionLotService = auctionLotService;
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] BidModel model)
    {
        var response = await _auctionLotService.PlaceBidAsync(model.LotId, model.BidAmount);

        return response ? Ok() : BadRequest();
    }
}

public interface IAuctionLotService
{
    Task<bool> PlaceBidAsync(string lotId, decimal bidAmount);
}

public class AuctionLotService : IAuctionLotService
{
    private readonly IActionLotRepository _repository;

    public AuctionLotService(IActionLotRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> PlaceBidAsync(string lotVersion, string lotId, decimal bidAmount)
    {
        var auctionLot = await _repository.GetAsync(lotId);
        if (auctionLot == null)
        {
            return false;
        }

        var bidPlaced = auctionLot.PlaceBid(new Bidder(), bidAmount, lotVersion);
        if (!bidPlaced) return false;

        _repository.StoreAsync(auctionLot);
    }
}

public interface IActionLotRepository
{
    //maybe we should also pass lotVersion and filter it out directly in db to avoid unnecessary selects
    Task<AuctionLot> GetAsync(string lotId);
    Task<bool> StoreAsync(AuctionLot auctionLot);
}

public class ActionLotRepository : IActionLotRepository
{
    public Task<AuctionLot> GetAsync(string lotId)
    {
        return Task.FromResult(new AuctionLot());
    }

    public Task<bool> StoreAsync(AuctionLot auctionLot)
    {
        throw new NotImplementedException();
    }
}

public class Bidder
{
    public string UserId { get; set; }

    public string Email { get; set; }
}

public class AuctionLot
{
    public string LotVersion { get; set; }

    public string LotId { get; set; }

    public string ItemId { get; set; }

    public decimal StartingPrice { get; set; }

    public decimal WinningPrice { get; set; }

    public Bidder WinningBidder { get; set; }

    public AuctionLot PlaceBid(Bidder bidder, decimal bidAmount, string lotVersion)
    {
        if (lotVersion != LotVersion)
        {
            
        }
    }
}