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
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using StockApi.Data;

namespace StockApi.Controllers
{
    [Route("api/news")]
    [ApiController]
    public class NewsController : ControllerBase
    {
        private readonly IList<string> keywords;

        private NewsController()
        {
            if (!System.IO.File.Exists("keywords.json"))
                System.IO.File.WriteAllText("keywords.json", JsonConvert.SerializeObject(new List<string>()));

            keywords = JsonConvert.DeserializeObject<List<string>>(System.IO.File.ReadAllText("keywords.json"));
        }

        [HttpGet("getKeywords")]
        public IActionResult GetKeywords([FromQuery] string apiKey) => StatusCode(200, keywords);

        [HttpPost("updateKeywords")]
        public IActionResult UpdateKeywords([FromQuery] string keywordList)
        {
            keywords.Clear();

            foreach (var keyword in keywordList.Split(',')) keywords.Add(keyword);

            return StatusCode(200);
        }

        [HttpGet("getNews")]
        public IActionResult GetNews()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            using var wc = new WebClient
            {
                Encoding = Encoding.GetEncoding(51949),
                Headers =
                {
                    ["Accept"] =
                        "application/x-ms-application, image/jpeg, application/xaml+xml, image/gif, image/pjpeg, application/x-ms-xbap, application/x-shockwave-flash, application/vnd.ms-excel, application/vnd.ms-powerpoint, application/msword, */*",
                    ["User-Agent"] =
                        "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.1; WOW64; Trident/4.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0; MDDC)"
                }
            };
            var document = new HtmlDocument();
            var newsList = new List<News>();

            document.LoadHtml(wc.DownloadString("https://finance.naver.com/news/mainnews.nhn"));

            for (var a = 1;; a++)
            {
                var title = document.DocumentNode.SelectSingleNode(
                    $"/html[1]/body[1]/div[3]/div[1]/div[2]/div[1]/div[2]/ul[1]/li[{a}]/dl[1]/dd[1]/a[1]");
                var content = document.DocumentNode.SelectSingleNode(
                    $"/html[1]/body[1]/div[3]/div[1]/div[2]/div[1]/div[2]/ul[1]/li[{a}]/dl[1]/dd[2]");
                var thumbnail = document.DocumentNode.SelectSingleNode(
                    $"/html[1]/body[1]/div[3]/div[1]/div[2]/div[1]/div[2]/ul[1]/li[{a}]/dl[1]/dt[1]/a[1]/img[1]");

                title ??= document.DocumentNode.SelectSingleNode(
                    $"/html[1]/body[1]/div[3]/div[1]/div[2]/div[1]/div[2]/ul[1]/li[{a}]/dl[1]/dt[1]/a[1]");
                content ??= document.DocumentNode.SelectSingleNode(
                    $"/html[1]/body[1]/div[3]/div[1]/div[2]/div[1]/div[2]/ul[1]/li[{a}]/dl[1]/dd[1]");

                if (title == null | content == null) break;

                if (keywords.Any(
                    keyword => title.InnerText.Contains(keyword) || content.InnerText.Contains(keyword)))
                {
                    newsList.Add(new News
                    {
                        Title = title.InnerText,
                        Url = $"https://finance.naver.com/{title.GetAttributeValue("href", "")}",
                        ThumbnailUrl = thumbnail == null ? "" : thumbnail.GetAttributeValue("src", "")
                    });
                }
            }

            return StatusCode(200,
                new NewsInfoDto() {NewsList = newsList, UpdateDate = $"{DateTime.Now.AddHours(9):yyyy-MM-dd}"});
        }
    }
}