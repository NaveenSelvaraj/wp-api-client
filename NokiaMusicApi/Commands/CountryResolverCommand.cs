﻿// -----------------------------------------------------------------------
// <copyright file="CountryResolverCommand.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json.Linq;
using Nokia.Music.Internal;
using Nokia.Music.Internal.Request;
using Nokia.Music.Internal.Response;

namespace Nokia.Music.Commands
{
    /// <summary>
    /// Defines the country resolver request
    /// </summary>
    internal class CountryResolverCommand : MusicClientCommand<Response<bool>>
    {
        public CountryResolverCommand(string appId, IApiRequestHandler handler)
        {
            this.MusicClientSettings = new CountryResolverSettings(appId);
            this.RequestHandler = handler;
        }

        /// <summary>
        /// Gets or sets the supplied country code
        /// </summary>
        public string CountryCode { get; set; }

        internal override bool RequiresCountryCode
        {
            get { return false; }
        }

        protected override void Execute()
        {
            this.RequestHandler.SendRequestAsync(
                this,
                this.MusicClientSettings,
                new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>("countrycode", this.CountryCode) },
                new JsonResponseCallback(
                (Response<JObject> rawResult) =>
                {
                    Response<bool> response = null;

                    // Parse the result if we got one...
                    if (rawResult.StatusCode != null)
                    {
                        switch (rawResult.StatusCode.Value)
                        {
                            case HttpStatusCode.OK:
                                if (rawResult.Result != null)
                                {
                                    JArray items = rawResult.Result.Value<JArray>("items");
                                    if (items != null && items.Count == 1)
                                    {
                                        response = new Response<bool>(rawResult.StatusCode, true, RequestId);
                                    }
                                }

                                break;

                            case HttpStatusCode.NotFound:
                                response = new Response<bool>(rawResult.StatusCode, false, RequestId);
                                break;

                            case HttpStatusCode.Forbidden:
                                response = new Response<bool>(rawResult.StatusCode, new InvalidApiCredentialsException(), null, RequestId);
                                break;
                        }
                    }

                    // If the API return an expected result, set an error...
                    if (response == null)
                    {
                        response = new Response<bool>(rawResult.StatusCode, new ApiCallFailedException(), null, RequestId);
                    }

                    if (Callback != null)
                    {
                        Callback(response);
                    }
                }));
        }

        /// <summary>
        /// Implementation of MusicClientSettings for use with country resolver request
        /// </summary>
        private class CountryResolverSettings : IMusicClientSettings
        {
            private readonly string _appId;
            
            public CountryResolverSettings(string appId)
            {
                this._appId = appId;
            }

            public string AppId
            {
                get { return this._appId; }
            }

            public string CountryCode
            {
                get { return null; }
            }

            public bool CountryCodeBasedOnRegionInfo
            {
                get { return false; }
            }
        }
    }
}
