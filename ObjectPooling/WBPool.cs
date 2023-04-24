using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;

namespace WB.Pool
{
    /// <summary>
    /// オブジェクトをプールに関するAPIを提供するクラス
    /// </summary>
    public static class WBPool
    {
        /// <summary>
        /// 型Tのインスタンスを保持するプールを生成し取得する
        /// </summary>
        /// <param name="number"></param>
        /// <param name="getNewInstance"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Pool<T> MakePool<T>(int number,Func<T> getNewInstance) where T : class, IPoolObject
        {
            return PoolManager.Instance.MakePool(number, getNewInstance);
        }

        /// <summary>
        /// 型Tのインスタンスを保持するプールを生成し取得する
        /// </summary>
        /// <param name="number"></param>
        /// <param name="getNewInstance"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static UIElementPool<T> MakeUIElementPool<T>(int number, Func<T> getNewInstance) where T : UIElement, IPoolObject
        {
            return PoolManager.Instance.MakeUIElementPool(number, getNewInstance);
        }

    }
}
