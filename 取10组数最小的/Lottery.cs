namespace 取10组数最小的
{
    /// <summary>
    /// 彩票
    /// </summary>
    class Lottery
    {
        public int Number { get; set; }
        public decimal Amount { get; set; }
        public int row { get; set; }

        public override bool Equals(object obj)
        {
            Lottery lottery = (Lottery)obj;
            return base.Equals(lottery.Amount.Equals(this.Amount) && lottery.Number.Equals(this.Number));
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return "{number=" + Number + ",amount=" + Amount + ",row=" + row + "}";
        }
    }
}
