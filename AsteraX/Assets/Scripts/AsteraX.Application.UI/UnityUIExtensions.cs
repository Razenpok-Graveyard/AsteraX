using System;
using TMPro;
using UniRx;

namespace AsteraX.Application.UI
{
    public static class UnityUIExtensions
    {
        public static IDisposable SubscribeToText(this IObservable<string> source, TextMeshProUGUI text)
        {
            return source.SubscribeWithState(text, (x, t) => t.text = x);
        }
        
        public static IDisposable SubscribeToText<T>(this IObservable<T> source, TextMeshProUGUI text)
        {
            return source.SubscribeWithState(text, (x, t) => t.text = x.ToString());
        } 
    }
}