using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading;
using System.ComponentModel;
using System.Text;

namespace Nota3080bot
{


    public class Program
    {
        public static void Main()
        {
            //BestBuy skues in array
            string[] bbySkus = new string[] { "6436196", "6432445", "6436223", "6426149", "6438941", "6438942" };

            //Walmart URI array
            //string[] walURIs = new string[] { "" }; // ip/PlayStation-5-Console/363472942

            //Amazon URI array
            string[] amazonURIs = new string[] { "PlayStation-5-Console/dp/B08FC5L3RG/", "ASUS-Graphics-DisplayPort-Axial-tech-2-9-Slot/dp/B08L8JNTXQ/", "EVGA-10G-P5-3897-KR-GeForce-Technology-Backplate/dp/B08HR3Y5GQ/" };

            //MicroCenter URI array
            string[] mcURIs = new string[] { "628686/asus-geforce-rtx-3080-strix-overclocked-triple-fan-10gb-gddr6x-pcie-40-graphics-card", "628346/evga-geforce-rtx-3080-ftw3-ultra-gaming-triple-fan-10gb-gddr6x-pcie-40-graphics-card", "628330/msi-geforce-rtx-3080-gaming-x-trio-triple-fan-10gb-gddr6x-pcie-40-graphics-card", "628344/evga-geforce-rtx-3080-xc3-ultra-gaming-triple-fan-10gb-gddr6x-pcie-40-graphics-card" };

            //declare loop count var
            int count = 0;
            int mcError = 0;
            int mcSuccess = 0;
            //loop through skus to create objects for each product
            while (true)
            {
                bool inStockEmail;
                try
                {

                    //MicroCenter for each execution
                    foreach (string i in mcURIs)
                    {
                        count++;
                        string storeID = "MicroCenter";
                        MicroCenterCheck mcObj = new MicroCenterCheck
                        {
                            uri = i
                        };
                        try
                        {
                            inStockEmail = mcObj.StockCheck();
                            mcSuccess++;
                        }
                        catch (WebException ex)
                        {
                            //Console.WriteLine(ex.Message);
                            mcError++;
                            Console.WriteLine($"----FAILED-----Microcenter Success count: {mcSuccess}-------Microcenter Fail count: {mcError}---------");
                            continue;
                        }
                        Console.WriteLine($"request number {count}");

                        //send email if instock with sku
                        if (inStockEmail == false)
                        {
                            SendNotification_f(i, storeID);
                        }
                        Thread.Sleep(5000);
                    }


                    //Amazon for each exec
                    foreach (string i in amazonURIs)
                    {
                        count++;
                        string storeID = "Amazon.com";
                        AmazonCheck amazonObj = new AmazonCheck();
                        amazonObj.uri = i;
                        inStockEmail = amazonObj.StockCheck();
                        Console.WriteLine($"request number {count}");

                        //send email if instock with sku
                        if (inStockEmail == true)
                        {
                            SendNotification_f(i, storeID);
                        }
                        Thread.Sleep(5000);
                    }

                    //Walmart for each exec
                    /*foreach (string i in walURIs)
                    {
                        count++;
                        string storeID = "Walmart";
                        WalmartCheck walobj = new WalmartCheck();
                        walobj.uri = i;
                        inStockEmail = walobj.StockCheck();
                        Console.WriteLine($"request number {count}");

                        //send email if instock with sku
                        if (inStockEmail == true)
                        {
                            SendNotification_f(i, storeID);
                        }
                        Thread.Sleep(5000);
                    }*/

                    //BBY for each execution
                    foreach (string i in bbySkus)
                    {
                        count++;
                        string storeID = "BestBuyCheck";
                        BestBuyCheck bbyobj = new BestBuyCheck();
                        bbyobj.sku = i;
                        inStockEmail = bbyobj.StockCheck();
                        Console.WriteLine($"request number {count}");

                        //send email if instock with sku
                        if (inStockEmail == false)
                        {
                            SendNotification_f(i, storeID);
                        }
                        Thread.Sleep(8000);
                    }
                    


                }

                //Catch 404 or any error if the if statmenet is commented out error in any of the requests
                catch (WebException ex)
                {
                    Console.WriteLine("an error occured");
                    Console.WriteLine(ex.Status);
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);
                    if (ex.Status == WebExceptionStatus.ProtocolError)
                    {
                        HttpWebResponse response = ex.Response as HttpWebResponse;
                        if (response != null)
                        {
                            if ((int)response.StatusCode == 404) // Not Found
                            {
                                // Handle the 404 Not Found error 
                                // ...
                                Console.WriteLine("404");
                            }
                            /*else
                            {
                                // Could handle other response.StatusCode values here.
                                // ...
                            }*/
                        }
                    }
                    /*else
                    {
                        // Could handle other error conditions here, such as WebExceptionStatus.ConnectFailure.
                        // ...
                    }*/
                }

            }
        }





        public class WalmartCheck
        {
            public string uri;

            public bool StockCheck()
            {
                //request URL for product by SKU
                string url = $"https://www.walmart.com/{this.uri}";
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                //Required Headers for response
                request.Headers.Add("Connection", "close");
                request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.66 Safari/537.36");
                request.Headers.Add("Accept", "*/*");
                request.Headers.Add("Referer", "https://www.walmart.com/");
                request.Headers.Add("Accept", "gzip, deflate");
                request.Headers.Add("Accept-Language", "en-US,en;q=0.9");
                // Get the response.
                var response = (HttpWebResponse)request.GetResponse();

                // Get the stream containing content returned by the server.
                // The using block ensures the stream is automatically closed.
                string responseFromServer = "";
                using (Stream dataStream = response.GetResponseStream())
                {
                    // Open the stream using a StreamReader for easy access.
                    StreamReader reader = new StreamReader(dataStream);
                    // Read the content.
                    responseFromServer = reader.ReadToEnd();
                    //Check cookie as output
                    /*foreach (Cookie cook in response.Cookies)
                    {
                        Console.WriteLine("Cookie:");
                        Console.WriteLine($"{cook.Name} = {cook.Value}");

                        // Show the string representation of the cookie.
                        Console.WriteLine($"String: {cook}");
                    }*/
                }

                //Search for Sold Out ignoring case - sets match varilable to true if it is sold out
                //Walmart returns TRUE if it IS IN STOCK
                bool inStock = responseFromServer.Contains("Add to Cart", System.StringComparison.CurrentCultureIgnoreCase);
                if (inStock == true)
                {
                    Console.WriteLine($"Walmart In STOCK!!!!! {url}");
                    response.Close();
                    return inStock;
                }
                else
                {
                    Console.WriteLine($"Walmart Out of Stock {this.uri}");
                    response.Close();
                    return inStock;
                }

                //Debug - delete
                //Console.WriteLine(responseFromServer);
                //Console.WriteLine(ignoreCaseSearchResult);

                //while loop exit if Sold Out is not present
                //Thread.Sleep(30000);
            }

        }


        public class AmazonCheck
        {
            public string uri;

            public bool StockCheck()
            {
                //request URL for product by SKU
                string url = $"https://www.amazon.com/{this.uri}";
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                //Required Headers for response
                request.Headers.Add("Connection", "close");
                request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.66 Safari/537.36");
                request.Headers.Add("Accept", "*/*");
                request.Headers.Add("Accept-Encoding", "gzip, deflate");
                request.Headers.Add("Accept-Language", "en-US,en;q=0.9");
                // Get the response.
                var response = (HttpWebResponse)request.GetResponse();

                // Get the stream containing content returned by the server.
                // The using block ensures the stream is automatically closed.
                string responseFromServer = "";
                using (Stream dataStream = response.GetResponseStream())
                {
                    // Open the stream using a StreamReader for easy access.
                    StreamReader reader = new StreamReader(dataStream);
                    // Read the content.
                    responseFromServer = reader.ReadToEnd();

                    //Check cookie as output
                    /*foreach (Cookie cook in response.Cookies)
                    {
                        Console.WriteLine("Cookie:");
                        Console.WriteLine($"{cook.Name} = {cook.Value}");

                        // Show the string representation of the cookie.
                        Console.WriteLine($"String: {cook}");
                    }*/
                }

                //Search for Sold Out ignoring case - sets match varilable to true if it is sold out
                //Walmart returns TRUE if it IS IN STOCK
                bool inStock = responseFromServer.Contains("Add to Cart", System.StringComparison.CurrentCultureIgnoreCase);
                if (inStock == true)
                {
                    Console.WriteLine($"Amazon In STOCK!!!!! {url}");
                    response.Close();
                    return inStock;
                }
                else
                {
                    Console.WriteLine($"Amazon Out of Stock {this.uri}");
                    response.Close();
                    return inStock;
                }

                //Debug - delete
                //Console.WriteLine(responseFromServer);
                //Console.WriteLine(ignoreCaseSearchResult);

                //while loop exit if Sold Out is not present
                //Thread.Sleep(30000);
            }

        }


        public class MicroCenterCheck
        {
            public string uri;

            public bool StockCheck()
            {
                //request URL for product by SKU
                string urlMC = $"https://www.microcenter.com/product/{this.uri}";
                var request = (HttpWebRequest)WebRequest.Create(urlMC);
                request.Method = "GET";
                //Required Headers for response
                //request.Headers.Add("Connection", "close");
                //request.Headers.Add("Accept", "*/*");
                //request.Headers.Add("Accept-Encoding", "gzip, deflate");
                //request.Headers.Add("Accept-Language", "en-US,en;q=0.9");
                request.Headers.Add("Cookie", "storeSelected=045");

                //solve for SSL error?


                // Get the response.
                var response = (HttpWebResponse)request.GetResponse();

                // Get the stream containing content returned by the server.
                // The using block ensures the stream is automatically closed.
                string responseFromServer;
                using (Stream dataStream = response.GetResponseStream())
                {
                    // Open the stream using a StreamReader for easy access.
                    StreamReader reader = new StreamReader(dataStream);
                    // Read the content.
                    responseFromServer = reader.ReadToEnd();
                    response.Close();
                    //Console.WriteLine(responseFromServer);
                }

                //Search for Sold Out ignoring case - sets match varilable to true if it is sold out
                //MicroCenter returns FALSE if it IS IN STOCK
                bool notInStock = responseFromServer.Contains("Sold Out", System.StringComparison.CurrentCultureIgnoreCase);
                if (notInStock == true)
                {
                    Console.WriteLine($"MicroCenter Out of Stock {this.uri}");
                    response.Close();
                    return notInStock;
                }
                else
                {
                    Console.WriteLine($"MicroCenter IN STOCK!! {this.uri}");
                    response.Close();
                    return notInStock;
                }

                //Debug - delete
                //Console.WriteLine(responseFromServer);
                //Console.WriteLine(ignoreCaseSearchResult);

                //while loop exit if Sold Out is not present
                //Thread.Sleep(30000);
            }

        }


        public class BestBuyCheck
        {
            public string sku;
            public string name;
            public CookieContainer cookieJar = new CookieContainer();

            public BestBuyCheck()
            {
                this.getCookies_f();
            }

            public bool StockCheck()
            {
                //request URL for product by SKU
                string url = $"https://www.bestbuy.com/api/3.0/priceBlocks?skus={this.sku}";
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                //Required Headers for response
                request.Headers.Add("Connection", "close");
                request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.66 Safari/537.36");
                request.Headers.Add("Accept", "*/*");
                request.Headers.Add("Accept-Encoding", "*/*");
                request.Headers.Add("Accept", "gzip, deflate");
                request.Headers.Add("Accept-Language", "en-US,en;q=0.9");
                request.CookieContainer = cookieJar;
                // Get the response.
                var response = (HttpWebResponse)request.GetResponse();

                // Get the stream containing content returned by the server.
                // The using block ensures the stream is automatically closed.
                string responseFromServer = "";
                using (Stream dataStream = response.GetResponseStream())
                {
                    // Open the stream using a StreamReader for easy access.
                    StreamReader reader = new StreamReader(dataStream);
                    // Read the content.
                    responseFromServer = reader.ReadToEnd();

                    //Check cookie as output
                    /*foreach (Cookie cook in response.Cookies)
                    {
                        Console.WriteLine("Cookie:");
                        Console.WriteLine($"{cook.Name} = {cook.Value}");

                        // Show the string representation of the cookie.
                        Console.WriteLine($"String: {cook}");
                    }*/
                }

                //Search for Sold Out ignoring case - sets match varilable to true if it is sold out
                bool notInStock = responseFromServer.Contains("Sold Out", System.StringComparison.CurrentCultureIgnoreCase);
                if (notInStock == true)
                {
                    Console.WriteLine($"Best Buy Sold Out? {this.sku}");
                    response.Close();
                    return notInStock;
                }
                else
                {
                    Console.WriteLine($"Best Buy In STOCK!!! {this.sku}");
                    response.Close();
                    return notInStock;
                }

                //Debug - delete
                //Console.WriteLine(responseFromServer);
                //Console.WriteLine(ignoreCaseSearchResult);

                //while loop exit if Sold Out is not present
                //Thread.Sleep(30000);
            }

            public void getCookies_f()
            {
                //Start of new get cookie and add to card code

                //GET Cookies
                var cookieRequest = (HttpWebRequest)WebRequest.Create("https://www.bestbuy.com");
                cookieRequest.Method = "GET";
                //Required Headers for response
                cookieRequest.Headers.Add("Connection", "close");
                cookieRequest.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.66 Safari/537.36");
                cookieRequest.Headers.Add("Accept", "*/*");
                cookieRequest.Headers.Add("Accept-Encoding", "*/*");
                cookieRequest.Headers.Add("Accept", "gzip, deflate");
                cookieRequest.Headers.Add("Accept-Language", "en-US,en;q=0.9");

                //create cookie container
                cookieRequest.CookieContainer = this.cookieJar;
                // Get the response.
                var cookieResponse = (HttpWebResponse)cookieRequest.GetResponse();

                //Check cookie as output
                using (cookieResponse)
                {
                    // Print the properties of each cookie.
                    /*foreach (Cookie cook in cookieResponse.Cookies)
                    {
                        Console.WriteLine("Cookie:");
                        Console.WriteLine($"{cook.Name} = {cook.Value}");

                        // Show the string representation of the cookie.
                        Console.WriteLine($"String: {cook}");
                    }*/
                    // Close the response.
                    cookieResponse.Close();
                }

                //Console.WriteLine(cookieJar);
            }

        }




        public static void SendNotification_f(string inStockSKU, string storeID)
        {
            //SEND MAIL NOTIFICATION START

            var fromAddress = new MailAddress("Put your email here", "The Bot");
            var toAddress = new MailAddress("Put your email here", "Your Name");
            const string fromPassword = "put your password here";
            string subject = $"{storeID} stuff in stock! {inStockSKU}";
            string body = $"GOGOGOGOGOGOGOGOGO/n {storeID} {inStockSKU}";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword),
                Timeout = 20000
            };
            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            })
            {
                smtp.Send(message);
            }

        }
        
    }
}