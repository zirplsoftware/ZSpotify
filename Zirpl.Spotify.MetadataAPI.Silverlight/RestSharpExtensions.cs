using System;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using RestSharp;

namespace Zirpl.Spotify.MetadataAPI
{
    public static class RestSharpExtensions
    {
        public static Task<IRestResponse<T>> ExecuteAwait<T>(this RestClient client, RestRequest request)
        {
            var taskCompletionSource = new TaskCompletionSource<IRestResponse<T>>();
            client.ExecuteAsync<T>(request, (response, asyncHandle) =>
            {
                //if (response.ResponseStatus == ResponseStatus.Error)
                //{
                //    taskCompletionSource.SetException(response.ErrorException);
                //}
                //else
                //{
                taskCompletionSource.SetResult(response);
                //}
            });
            return taskCompletionSource.Task;
        }
    }
}
