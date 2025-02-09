﻿#if CSHARP_7_OR_LATER
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BestHTTP
{
    public static class AsyncExtensions
    {
        public static Task<T> GetFromJsonResultAsync<T>(this HTTPRequest request, CancellationToken token = default)
        {
            return HTTPRequestAsyncExtensions.CreateTask<T>(request, token, (req, resp, tcs) =>
            {
                switch (req.State)
                {
                    // The request finished without any problem.
                    case HTTPRequestStates.Finished:
                        if (resp.IsSuccess)
                            tcs.TrySetResult(LitJson.JsonMapper.ToObject<T>(resp.DataAsText));
                        else
                            tcs.TrySetException(new Exception(
                                $"Request finished Successfully, but the server sent an error. Status Code: {resp.StatusCode}-{resp.Message} Message: {resp.DataAsText}"));
                        break;

                    // The request finished with an unexpected error. The request's Exception property may contain more info about the error.
                    case HTTPRequestStates.Error:
                        HTTPRequestAsyncExtensions.VerboseLogging(request, "Request Finished with Error! " + (req.Exception != null ? (req.Exception.Message + "\n" + req.Exception.StackTrace) : "No Exception"));

                        tcs.TrySetException(req.Exception ?? new Exception("No Exception"));
                        break;

                    // The request aborted, initiated by the user.
                    case HTTPRequestStates.Aborted:
                        HTTPRequestAsyncExtensions.VerboseLogging(request, "Request Aborted!");

                        tcs.TrySetCanceled();
                        break;

                    // Connecting to the server is timed out.
                    case HTTPRequestStates.ConnectionTimedOut:
                        HTTPRequestAsyncExtensions.VerboseLogging(request, "Connection Timed Out!");

                        tcs.TrySetException(new Exception("Connection Timed Out!"));
                        break;

                    // The request didn't finished in the given time.
                    case HTTPRequestStates.TimedOut:
                        HTTPRequestAsyncExtensions.VerboseLogging(request, "Processing the request Timed Out!");

                        tcs.TrySetException(new Exception("Processing the request Timed Out!"));
                        break;
                }
            });
        }
    }
}

#endif