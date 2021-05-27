//  Copyright 2021 Jonguk Kim
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using StockApi.Data;

namespace StockApi.Controllers
{
    [Route("api/stock-info")]
    [ApiController]
    public class StockInfoController : ControllerBase
    {
        private static readonly ConcurrentBag<string> Addresses = new()
        {
            "https://kr.investing.com/indices/kospi",
            "https://kr.investing.com/indices/kosdaq",
            "https://kr.investing.com/indices/us-30-futures",
            "https://kr.investing.com/indices/nq-100-futures-streaming-chart"
        };

        private static readonly List<string> ItemNames = new() {"코스피", "코스닥", "다우존스 선물", "나스닥 선물"};

        [HttpGet]
        public IActionResult Get([FromQuery(Name = "key")] string apiKey)
        {
            var stockInfoList = new ConcurrentBag<Stock>();
            Parallel.ForEach(Addresses, address =>
            {
                using var wc = new WebClient
                {
                    Encoding = Encoding.UTF8,
                    Headers =
                    {
                        ["Accept"] =
                            "application/x-ms-application, image/jpeg, application/xaml+xml, image/gif, image/pjpeg, application/x-ms-xbap, application/x-shockwave-flash, application/vnd.ms-excel, application/vnd.ms-powerpoint, application/msword, */*",
                        ["User-Agent"] =
                            "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.1; WOW64; Trident/4.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0; MDDC)"
                    }
                };
                var document = new HtmlDocument();

                document.LoadHtml(wc.DownloadString(address));

                var stock = new Stock
                {
                    StockItem = ItemNames[Array.IndexOf(Addresses.ToArray(), address)],
                    Price = float.Parse(document.DocumentNode.SelectSingleNode("//span[@id='last_last']").InnerText
                        .Replace(",", "")),
                    Fluctuation = float.Parse(document.DocumentNode
                        .SelectSingleNode(
                            "/html[1]/body[1]/div[5]/section[1]/div[4]/div[1]/div[1]/div[1]/div[1]/div[2]/span[2]")
                        .InnerText.Replace("+", "").Replace(",", "")),
                    FluctuationRate = float.Parse(document.DocumentNode
                        .SelectSingleNode(
                            "/html[1]/body[1]/div[5]/section[1]/div[4]/div[1]/div[1]/div[1]/div[1]/div[2]/span[4]")
                        .InnerText.Replace("+", "").Replace("%", "")),
                };

                stockInfoList.Add(stock);
            });

            var stockInfo = new StockInfoDto
            {
                Stocks = stockInfoList.OrderByDescending(stock => stock.StockItem).ToList(),
                UpdateTime = $"{DateTime.Now.AddHours(9):yyyy-MM-dd-HH-mm}"
            };

            return StatusCode(200, stockInfo);
        }
    }
}