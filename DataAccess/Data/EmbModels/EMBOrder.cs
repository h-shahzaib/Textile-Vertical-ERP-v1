namespace GlobalLib.Data.EmbModels
{
    public class EMBOrder
    {
        public int ID { get; set; }
        public int SerialNo { get; set; }
        public string Brand { get; set; }
        public string OrderNum { get; set; }
        public int DesignID { get; set; }
        public string DesignNum { get; set; }
        public int TotalHeads { get; set; }
        public string Colors { get; set; }
        public string Date { get; set; }
        public string Note { get; set; }
        public bool Finished { get; set; }
    }
}
