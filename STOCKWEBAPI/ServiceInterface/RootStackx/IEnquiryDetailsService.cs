using System;
using STOCKWEBAPI.DataEntities.RootStackx;

namespace STOCKWEBAPI.ServiceInterface.RootStackx
{
	public interface IEnquiryDetailsService
	{
        public Task<string> ViewEnquiry();
    }
}

