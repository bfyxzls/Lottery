using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace 取10组数最小的
{
    class Program
    {
        static void Main(string[] args)
        {
            Dictionary<decimal, List<Lottery>> dic = new Dictionary<decimal, List<Lottery>>();
             List<List<Lottery>> lotteryList = JsonConvert.DeserializeObject<List<List<Lottery>>>(File.ReadAllText("test1.json"));
            //List<List<Lottery>> lotteryList = new List<List<Lottery>>();
            //Init(lotteryList);
            DisplayAll("原数据", lotteryList);
            int compareCount = 0;

            Run(dic, lotteryList, compareCount);
        }
        static void Run(Dictionary<decimal, List<Lottery>> dic, List<List<Lottery>> lotteryList, int compareCount)
        {

            int count = 0;

            #region 要生成的行数 ，这与你的集合行数关

            //一次比较，从1号到10位位，位置交替，从1到10，1走完放到最后，2开始走。。。
            // List<List<Lottery>> lotteryList = new List<List<Lottery>>();
            // Init(lotteryList);
            #region 先对集合排序
            for (int j = 0; j < lotteryList.Count; j++)
            {
                lotteryList[j] = lotteryList[j].OrderBy(o => o.Amount).ToList();
            }
         

            #endregion

            #region 当前行交替之后与其它行进行计算
            for (int j = 0; j < lotteryList.Count; j++)
            {

                #region 当前行交替换位
                for (int k = 0; k < j; k++)
                {
                    int index = compareCount % 9;
                    var old = lotteryList[index][0];
                    lotteryList[index].RemoveAt(0);
                    lotteryList[index].Add(old);
                }

                #endregion
                Compare(lotteryList);
                int repeatCount = 0;
                Repeat(lotteryList, repeatCount);
                var result = DicToList(lotteryList);
                count++;
                if (result != null)
                {
                    if (!dic.ContainsKey(result.Sum(s => s.Amount)))
                    {
                        dic.Add(result.Sum(s => s.Amount), result);
                    }
                }
            }
            #endregion


            #endregion

            compareCount++;
            if (compareCount < 10)
            {
                Run(dic, lotteryList, compareCount);
            }
        
            Console.WriteLine("字典数：{0}", dic.Count);
            Display("最佳结果", dic[dic.Min(o => o.Key)]);


            Console.ReadKey();
        }
        /// <summary>
        ///  做比较
        /// </summary>
        /// <param name="lotteryList"></param>
        async static Task Compare(List<List<Lottery>> lotteryList)
        {

            for (int i = 0; i < lotteryList.Count - 1; i++)
            {

                for (int j = 1; j < lotteryList.Count; j++)
                {
                    //最小值相同位置比较
                    if (lotteryList[i][0].Number == lotteryList[j][0].Number
                        && lotteryList[i][0].Amount > lotteryList[j][0].Amount) //位置相同，两个数字可能换位
                    {

                        var lottery = lotteryList[i][0];
                        lotteryList[i].RemoveAt(0);
                        lotteryList[i].Insert(1, lottery);
                    }
                    else
                    {
                        var lottery = lotteryList[j][0];
                        lotteryList[j].RemoveAt(0);
                        lotteryList[j].Insert(1, lottery);
                    }


                }
            }


        }

        /// <summary>
        /// 位置去重复
        /// </summary>
        /// <param name="lotteryList"></param>
        async static Task Repeat(List<List<Lottery>> lotteryList, int repeatCount)
        {

            bool repeat = false;
            for (int i = 0; i < lotteryList.Count - 1; i++)
            {
                for (int j = i + 1; j < lotteryList.Count; j++)
                {
                    if (lotteryList[i][0].row == lotteryList[j][0].row)
                    {
                        continue;
                    }
                    //最小值相同位置比较
                    if (lotteryList[i][0].Number == lotteryList[j][0].Number) //位置相同，两个数字可能换位
                    {
                        repeat = true;
                        if (lotteryList[i][0].Amount > lotteryList[j][0].Amount)//第二小的元素也比第二行的大，这时取第二行的数字
                        {
                            var old = lotteryList[i][0];
                            lotteryList[i].RemoveAt(0);//移除，因为位置不能重复
                            lotteryList[i].Add(old);

                        }
                        else
                        {
                            var old = lotteryList[j][0];
                            lotteryList[j].RemoveAt(0);
                            lotteryList[j].Add(old);
                        }
                    }

                }
            }
            if (repeat)
            {
                repeatCount++;
                Repeat(lotteryList, repeatCount);
            }

        }

        static List<Lottery> DicToList(List<List<Lottery>> lotteryList)
        {
            List<Lottery> lotteries = new List<Lottery>();
            lotteryList.ForEach(o =>
            {
                lotteries.Add(o[0]);
            });
            if (lotteries.Select(i => i.Number).Distinct().Count() < lotteries.Count)
            {
                return null;
            }
            return lotteries;
        }

        /// <summary>
        /// 比较最小低，打印结果
        /// </summary>
        /// <param name="lotteryList"></param>
        static void Display(string message, List<Lottery> lotteries)
        {
            Console.WriteLine("----------------------------{0}----------------------------", message);

            decimal total = 0m;

            //打印结果
            lotteries.ToList().ForEach(i =>
            {
                Console.WriteLine("{2} {0}[{1}] ", i.Number, i.Amount, i.row);
                total += i.Amount;
            });
            Console.WriteLine("合计:{0}", total);

        }
        static void DisplayAll(string message, List<List<Lottery>> lotteries)
        {
            Console.WriteLine("----------------------{0}--------------------------------", message);
            for (int j = 0; j < lotteries.Count; j++)
            {
                foreach (var res in lotteries[j])
                {
                    Console.Write("" + res.Number + "[" + res.Amount + "]  ");
                }
                Console.WriteLine();
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="lotteryList"></param>
        static void Init(List<List<Lottery>> lotteryList)
        {
            for (int j = 1; j <= 10; j++)
            {
                List<Lottery> lotteries = new List<Lottery>();
                for (int i = 1; i <= 10; i++)
                {
                    int res = GetRandomCode();
                    Console.Write("" + i + "[" + res + "]  ");
                    lotteries.Add(new Lottery() { Amount = res, Number = i, row = j });
                }
                Console.WriteLine();
                lotteryList.Add(lotteries);
            }
            String file = JsonConvert.SerializeObject(lotteryList);
        }

        static int GetRandomCode()
        {
            byte[] bytes = new byte[4];
            System.Security.Cryptography.RNGCryptoServiceProvider rng = new System.Security.Cryptography.RNGCryptoServiceProvider();
            rng.GetBytes(bytes);
            int res = BitConverter.ToInt32(bytes, 0);
            Random random = new Random(res);
            return random.Next(10, 99);
        }
    }
}
