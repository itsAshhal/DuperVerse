﻿

namespace CBS.Models
{
    public class CBSGrantTicketResult : CBSBaseResult
    {
        public string BattlePassID;
        public string BattlePassInstanceID;
        public string TicketID;
        public string TicketCatalogID;
        public BattlePassTicket Ticket;
    }
}