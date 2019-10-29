﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
using VincenzoBot;
using VincenzoBot.Services;
using Newtonsoft.Json.Linq;

namespace VincenzoBot
    {
    public class YouTubeService
    {

        // client configuration
        private static string clientID = null; //BotConfigRepository.Config.YTclientId;
        private static string clientSecret = null;//BotConfigRepository.Config.YTclientSecret; //api key
        private const string authorizationEndpoint = "https://accounts.google.com/o/oauth2/v2/auth";
        private const string scope = "https://www.googleapis.com/auth/youtube.force-ssl";
        private const string tokenEndpoint = "https://www.googleapis.com/oauth2/v4/token";
        private const string userInfoEndpoint = "https://www.googleapis.com/oauth2/v3/userinfo";
        private static string access_token;
        // ref http://stackoverflow.com/a/3978040
        public static int GetRandomUnusedPort()
        {
            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            var port = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();
            return port;
        }

        public static async Task<string> DoOAuth()
        {
            // Generates state and PKCE values.
            string state = randomDataBase64url(32);
            string code_verifier = randomDataBase64url(32);
            string code_challenge = base64urlencodeNoPadding(sha256(code_verifier));
            const string code_challenge_method = "S256";

            // Creates a redirect URI using an available port on the loopback address.
            string redirectURI = string.Format("http://{0}:{1}/", IPAddress.Loopback, GetRandomUnusedPort());
            output("redirect URI: " + redirectURI);

            // Creates an HttpListener to listen for requests on that redirect URI.
            var http = new HttpListener();
            http.Prefixes.Add(redirectURI);
            output("Listening..");
            http.Start();

            // Creates the OAuth 2.0 authorization request.
            string authorizationRequest = string.Format("{0}?response_type=code&scope={1}&redirect_uri={2}&client_id={3}&state={4}&code_challenge={5}&code_challenge_method={6}",
                authorizationEndpoint,
                scope,
                System.Uri.EscapeDataString(redirectURI),
                clientID,
                state,
                code_challenge,
                code_challenge_method);

            // Opens request in the browser.
            System.Diagnostics.Process.Start(authorizationRequest);

            // Waits for the OAuth authorization response.
            var context = await http.GetContextAsync();

            // Brings the Console to Focus.
            BringConsoleToFront();

            // Sends an HTTP response to the browser.
            var response = context.Response;
            string responseString = string.Format("<html><head><meta http-equiv='refresh' content='10;url=https://google.com'></head><body>Please return to the app.</body></html>");
            var buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
            response.ContentLength64 = buffer.Length;
            var responseOutput = response.OutputStream;
            Task responseTask = responseOutput.WriteAsync(buffer, 0, buffer.Length).ContinueWith((task) =>
            {
                responseOutput.Close();
                http.Stop();
                Console.WriteLine("HTTP server stopped.");
            });

            // Checks for errors.
            if (context.Request.QueryString.Get("error") != null)
            {
                output(String.Format("OAuth authorization error: {0}.", context.Request.QueryString.Get("error")));
                return "";
            }
            if (context.Request.QueryString.Get("code") == null
                || context.Request.QueryString.Get("state") == null)
            {
                output("Malformed authorization response. " + context.Request.QueryString);
                return "";
            }

            // extracts the code
            var code = context.Request.QueryString.Get("code");
            var incoming_state = context.Request.QueryString.Get("state");

            // Compares the receieved state to the expected value, to ensure that
            // this app made the request which resulted in authorization.
            if (incoming_state != state)
            {
                output(String.Format("Received request with invalid state ({0})", incoming_state));
                return "";
            }
            output("Authorization code: " + code);

            // Starts the code exchange at the Token Endpoint.
            return await performCodeExchange(code, code_verifier, redirectURI);
        }

        static async Task<string> performCodeExchange(string code, string code_verifier, string redirectURI)
        {
            output("Exchanging code for tokens...");

            // builds the  request
            string tokenRequestURI = "https://www.googleapis.com/oauth2/v4/token";
            string tokenRequestBody = string.Format("code={0}&redirect_uri={1}&client_id={2}&code_verifier={3}&client_secret={4}&scope=&grant_type=authorization_code",
                code,
                System.Uri.EscapeDataString(redirectURI),
                clientID,
                code_verifier,
                clientSecret
                );

            // sends the request
            HttpWebRequest tokenRequest = (HttpWebRequest)WebRequest.Create(tokenRequestURI);
            tokenRequest.Method = "POST";
            tokenRequest.ContentType = "application/x-www-form-urlencoded";
            tokenRequest.Accept = "Accept=text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            byte[] _byteVersion = Encoding.ASCII.GetBytes(tokenRequestBody);
            tokenRequest.ContentLength = _byteVersion.Length;
            Stream stream = tokenRequest.GetRequestStream();
            await stream.WriteAsync(_byteVersion, 0, _byteVersion.Length);
            stream.Close();

            try
            {
                // gets the response
                WebResponse tokenResponse = await tokenRequest.GetResponseAsync();
                using (StreamReader reader = new StreamReader(tokenResponse.GetResponseStream()))
                {
                    // reads response body
                    string responseText = await reader.ReadToEndAsync();
                    Console.WriteLine(responseText);

                    // converts to dictionary
                    Dictionary<string, string> tokenEndpointDecoded = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseText);

                    return access_token = tokenEndpointDecoded["access_token"];

                }
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    var response = ex.Response as HttpWebResponse;
                    if (response != null)
                    {
                        output("HTTP: " + response.StatusCode);
                        using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                        {
                            // reads response body
                            string responseText = await reader.ReadToEndAsync();
                            output(responseText);
                        }
                    }

                }
            }
            return "";
        }
        public static async void SendLiveChatMessage(string access_token, string message, string liveChatID)
        {
            output("Making API Call to LiveBroadcasts and sending message...");
            // builds the  request
            string sendmessageURI = String.Format("https://www.googleapis.com/youtube/v3/liveChat/messages?part=snippet&key={0}",
                clientSecret
            );
            // sends the request
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(sendmessageURI);
            request.Method = "POST";
            request.ContentType = "application/json; charset=utf-8";
            request.Headers.Add(string.Format("Authorization: Bearer {0}", access_token));
            request.Accept = "application/json";
            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                dynamic json = JObject.Parse("{    'snippet': {'liveChatId': '" + liveChatID + "','type': 'textMessageEvent','textMessageDetails': {'messageText': '"+message+"' }} }");

                streamWriter.Write(json);
            }
            // gets the response
            WebResponse userinfoResponse = await request.GetResponseAsync();
        }
            public static async void GetLiveBroadcasts(string access_token)
        {
            output("Making API Call to LiveBroadcasts...");

            // builds the  request
            string userinfoRequestURI = String.Format("https://www.googleapis.com/youtube/v3/liveBroadcasts?part=snippet%2CcontentDetails%2Cstatus&broadcastType=all&maxResults={0}&mine=true&key={1}",
            
                Constants.LIVE_BROADCASTS_RESULTS,
                clientSecret
            );

            // sends the request
            HttpWebRequest userinfoRequest = (HttpWebRequest)WebRequest.Create(userinfoRequestURI);
            userinfoRequest.Method = "GET";
            userinfoRequest.Headers.Add(string.Format("Authorization: Bearer {0}", access_token));
            userinfoRequest.ContentType = "application/x-www-form-urlencoded";
            userinfoRequest.Accept = "Accept=text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";

            // gets the response
            WebResponse userinfoResponse = await userinfoRequest.GetResponseAsync();
            StreamReader userinfoResponseReader = new StreamReader(userinfoResponse.GetResponseStream());
            
                // reads response body
           string liveBroadcastsJSON = await userinfoResponseReader.ReadToEndAsync();
            LiveBroadcastService service = new LiveBroadcastService();
            service.CreateCollection(liveBroadcastsJSON);
            
        }
            static async void userinfoCall(string access_token)
            {
                output("Making API Call to Userinfo...");

                // builds the  request
                string userinfoRequestURI = "https://www.googleapis.com/oauth2/v3/userinfo";

                // sends the request
                HttpWebRequest userinfoRequest = (HttpWebRequest)WebRequest.Create(userinfoRequestURI);
                userinfoRequest.Method = "GET";
                userinfoRequest.Headers.Add(string.Format("Authorization: Bearer {0}", access_token));
                userinfoRequest.ContentType = "application/x-www-form-urlencoded";
                userinfoRequest.Accept = "Accept=text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";

                // gets the response
                WebResponse userinfoResponse = await userinfoRequest.GetResponseAsync();
                using (StreamReader userinfoResponseReader = new StreamReader(userinfoResponse.GetResponseStream()))
                {
                    // reads response body
                    string userinfoResponseText = await userinfoResponseReader.ReadToEndAsync();
                    output(userinfoResponseText);
                }
            }

            /// <summary>
            /// Appends the given string to the on-screen log, and the debug console.
            /// </summary>
            /// <param name="output">string to be appended</param>
            public static void output(string output)
            {
                Console.WriteLine(output);
        }

            /// <summary>
            /// Returns URI-safe data with a given input length.
            /// </summary>
            /// <param name="length">Input length (nb. output will be longer)</param>
            /// <returns></returns>
            public static string randomDataBase64url(uint length)
            {
                RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                byte[] bytes = new byte[length];
                rng.GetBytes(bytes);
                return base64urlencodeNoPadding(bytes);
            }

            /// <summary>
            /// Returns the SHA256 hash of the input string.
            /// </summary>
            /// <param name="inputStirng"></param>
            /// <returns></returns>
            public static byte[] sha256(string inputStirng)
            {
                byte[] bytes = Encoding.ASCII.GetBytes(inputStirng);
                SHA256Managed sha256 = new SHA256Managed();
                return sha256.ComputeHash(bytes);
            }

            /// <summary>
            /// Base64url no-padding encodes the given input buffer.
            /// </summary>
            /// <param name="buffer"></param>
            /// <returns></returns>
            public static string base64urlencodeNoPadding(byte[] buffer)
            {
                string base64 = Convert.ToBase64String(buffer);

                // Converts base64 to base64url.
                base64 = base64.Replace("+", "-");
                base64 = base64.Replace("/", "_");
                // Strips padding.
                base64 = base64.Replace("=", "");

                return base64;
            }

            // Hack to bring the Console window to front.
            // ref: http://stackoverflow.com/a/12066376

            [DllImport("kernel32.dll", ExactSpelling = true)]
            public static extern IntPtr GetConsoleWindow();

            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool SetForegroundWindow(IntPtr hWnd);

            public static void BringConsoleToFront()
            {
                SetForegroundWindow(GetConsoleWindow());
            }

        }
    }


