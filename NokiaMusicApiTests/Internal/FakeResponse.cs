﻿// -----------------------------------------------------------------------
// <copyright file="FakeResponse.cs" company="Nokia">
// Copyright (c) 2012, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Net;
using System.Text;
using Newtonsoft.Json.Linq;
using Nokia.Music.Internal;
using Nokia.Music.Internal.Response;
using Nokia.Music.Tests.Properties;

namespace Nokia.Music.Tests.Internal
{
    internal class FakeResponse
    {
        private const string TestContentType = "application/vnd.nokia.ent.test.json";
        private readonly object _response;

        /// <summary>
        /// Private constructor, use a helper method to create
        /// </summary>
        /// <param name="response">The response to callback with</param>
        private FakeResponse(object response)
        {
            this._response = response;
        }

        public static FakeResponse InternalServerError()
        {
            return new FakeResponse(new Response<JObject>(HttpStatusCode.InternalServerError, (JObject)null, Guid.Empty));
        }

        public static FakeResponse Success(byte[] successResponse)
        {
            return new FakeResponse(new Response<JObject>(HttpStatusCode.OK, TestContentType, JObject.Parse(Encoding.UTF8.GetString(successResponse)), Guid.Empty));
        }

        public static FakeResponse RawSuccess(string successResponse)
        {
            return new FakeResponse(new Response<string>(HttpStatusCode.OK, TestContentType, successResponse, Guid.Empty));
        }

        public static FakeResponse GatewayTimeout()
        {
            return new FakeResponse(new Response<JObject>(HttpStatusCode.GatewayTimeout, (JObject)null, Guid.Empty));
        }

        public static FakeResponse NotFound(string response = null)
        {
            if (response != null)
            {
                return new FakeResponse(new Response<JObject>(HttpStatusCode.NotFound, JObject.Parse(response), Guid.Empty));
            }

            return new FakeResponse(new Response<JObject>(HttpStatusCode.NotFound, (JObject)null, Guid.Empty));
        }

        public static FakeResponse Forbidden()
        {
            return new FakeResponse(new Response<JObject>(HttpStatusCode.Forbidden, (JObject)null, Guid.Empty));
        }

        /// <summary>
        /// Performs the callback specified with the previously queued up response
        /// </summary>
        /// <typeparam name="T">The type of response</typeparam>
        /// <param name="callback">The callback action</param>
        public void DoCallback<T>(IResponseCallback<T> callback)
        {
            var resp = this._response as Response<T>;
            callback.Callback(resp);
        }
    }
}
