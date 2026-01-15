using System;
namespace STOCKWEBAPI.DataEntities.Portfolio
{
    public class PortfolioEnqueryRequest
    {
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string Message { get; set; }
    }

}

