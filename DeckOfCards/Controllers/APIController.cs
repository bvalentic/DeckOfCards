using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace DeckOfCards.Controllers
{
    public class APIController : Controller
    {
        const string userAgent = "Mozilla / 5.0(Windows NT 6.1; Win64; x64; rv: 47.0) Gecko / 20100101 Firefox / 47.0";

        public ActionResult GetNewDeck()
        {
            HttpWebRequest request = WebRequest.CreateHttp("https://deckofcardsapi.com/api/deck/new/shuffle/?deck_count=1");

            request.UserAgent = userAgent;

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                StreamReader data = new StreamReader(response.GetResponseStream());
                JObject cardDeck = JObject.Parse(data.ReadToEnd());

                string deckID = cardDeck["deck_id"].ToString();

                //to create an instance of TempData
                if (!TempData.ContainsKey("DeckID"))
                {
                    TempData.Add("DeckID", deckID);
                }

                //to reestablish TempData as itself before switching ActionResults / Views
                TempData["DeckID"] = TempData["DeckID"];

                ViewBag.NewDeckMessage = "A new deck has been shuffled";

            }            

            return View();
        }

        public ActionResult DrawCards(int numDraw)
        {
            //to access TempData in next ActionResult
            if (TempData["DeckID"] != null)
            {
                string deckID = TempData["DeckID"].ToString();

                HttpWebRequest request = WebRequest.CreateHttp($"https://deckofcardsapi.com/api/deck/{deckID}/draw/?count={numDraw}");
                request.UserAgent = userAgent;
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    StreamReader data = new StreamReader(response.GetResponseStream());

                    JObject hand = JObject.Parse(data.ReadToEnd());

                    ViewBag.Cards = hand["cards"];
                }

                TempData["DeckID"] = TempData["DeckID"];
            }

            return View();
        }

        // GET: API
        public ActionResult ShuffleExistingDeck()
        {
            return View();
        }
    }
}