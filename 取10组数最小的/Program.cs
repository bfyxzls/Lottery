using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
namespace 取10组数最小的
{
    class Program
    {
        static void Main(string[] args)
        {
            Dictionary<decimal, List<Lottery>> dic = new Dictionary<decimal, List<Lottery>>();
            //List<List<Lottery>> lotteryList = new List<List<Lottery>>();
            //Init(lotteryList);
            //DisplayAll("原数据", lotteryList);

            //要生成的行数 ，每行都走一次
            for (int i = 0; i < 10; i++)
            {
                //一次比较，从1号到10位位，位置交替，从1到10，1走完放到最后，2开始走。。。
                List<List<Lottery>> lotteryList = JsonConvert.DeserializeObject<List<List<Lottery>>>(File.ReadAllText("test1.json"));
                DisplayAll("原数据", lotteryList);
                //从新对N条数据进行排序，按从小到大，然后第二次第二大的在第一位，第一大的交替到最后
                lotteryList[i] = lotteryList[i].OrderBy(o => o.Amount).ToList();

                //当前行交替之后与其它行进行计算
                for (int j = 0; j < lotteryList.Count; j++)
                {
                    //当前行交替换位
                    for (int k = 0; k < j; k++)
                    {
                        var old = lotteryList[i][0];
                        lotteryList[i].RemoveAt(0);
                        lotteryList[i].Add(old);
                    }

                    Compare(lotteryList);
                    Repeat(lotteryList);
                    var result = DicToList(lotteryList);

                    if (!dic.ContainsKey(result.Sum(s => s.Amount)))
                    {
                        dic.Add(result.Sum(s => s.Amount), result);
                    }
                }
            }

            Console.WriteLine("计算次数：{0}", dic.Count);
            Display("最佳结果", dic[dic.Min(o => o.Key)]);


            Console.ReadKey();
        }
        /// <summary>
        ///  做比较
        /// </summary>
        /// <param name="lotteryList"></param>
        static void Compare(List<List<Lottery>> lotteryList)
        {
            for (int i = 0; i < lotteryList.Count; i++)
            {
                lotteryList[i] = lotteryList[i].OrderBy(o => o.Amount).ToList();
            }

            for (int i = 0; i < lotteryList.Count - 1; i++)
            {

                for (int j = i + 1; j < lotteryList.Count; j++)
                {
                    //最小值相同位置比较
                    if (lotteryList[i][0].Number == lotteryList[j][0].Number && lotteryList[i][0].Amount > lotteryList[j][0].Amount) //位置相同，两个数字可能换位
                    {
                        if (lotteryList[i][1].Amount > lotteryList[j][1].Amount)
                        {
                            var lottery = lotteryList[i][0];
                            lotteryList[i][0] = lotteryList[i][1];
                            lotteryList[i][1] = lottery;

                        }
                        else
                        {
                            var lottery = lotteryList[j][0];
                            lotteryList[j][0] = lotteryList[j][1];
                            lotteryList[j][1] = lottery;

                        }

                    }

                }
            }

        }

        /// <summary>
        /// 位置去重复
        /// </summary>
        /// <param name="lotteryList"></param>
        static void Repeat(List<List<Lottery>> lotteryList)
        {

            bool repeat = false;
            for (int i = 0; i < lotteryList.Count - 1; i++)
            {
                for (int j = i + 1; j < lotteryList.Count; j++)
                {
                    //最小值相同位置比较
                    if (lotteryList[i][0].Number == lotteryList[j][0].Number) //位置相同，两个数字可能换位
                    {
                        repeat = true;
                        if (lotteryList[i][0].Amount > lotteryList[j][0].Amount)//第二小的元素也比第二行的大，这时取第二行的数字
                        {
                            lotteryList[i].RemoveAt(0);//移除，因为位置不能重复
                            lotteryList[i].Add(lotteryList[j][0]);
                        }
                        else
                        {
                            lotteryList[j].RemoveAt(0);
                            lotteryList[j].Add(lotteryList[j][0]);

                        }
                    }

                }
            }
            if (repeat)
            {
                Repeat(lotteryList);
            }

        }

        static List<Lottery> DicToList(List<List<Lottery>> lotteryList)
        {
            List<Lottery> lotteries = new List<Lottery>();
            lotteryList.ForEach(o =>
            {
                lotteries.Add(o[0]);
            });
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
