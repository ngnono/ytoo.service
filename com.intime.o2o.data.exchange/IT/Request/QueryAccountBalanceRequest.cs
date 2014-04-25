﻿using com.intime.o2o.data.exchange.IT.Response;

namespace com.intime.o2o.data.exchange.IT.Request
{
    public class QueryAccountBalanceRequest : Request<dynamic, AccountBalanceResponse>
    {
        public override string GetResourceUri()
        {
            return "precard-rs/rest/groupcard/queryCuramt";
        }
    }
}