using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;

namespace WB.Pool
{
    /// <summary>
    /// 複数のプールをまとめるクラス
    /// </summary>
    internal class PoolManager
    {
        #region Singleton

        private static PoolManager instance;
        private PoolManager() { }
        public static PoolManager Instance => instance ??= new();
        #endregion


        /// <summary>
        /// 型(T)に対応するプールを作成する
        /// </summary>
        /// <param name="number">プールのサイズ</param>
        /// <param name="getNewInstance">型Tのインスタンスを生成する方法</param>
        /// <typeparam name="T">プールの対象となる型</typeparam>
        /// <exception cref="Exception">既に型Tに対応するプールが存在する場合</exception>
        internal Pool<T> MakePool<T>(int number, Func<T> getNewInstance) where T : class, IPoolObject
        {

            //型TがMonoBehaviourを継承しているか否かで、生成するプールを分ける
            Pool<T> pool =  new Pool<T>(number, getNewInstance);

            //指定の数だけプール内にインスタンスを生成する
            pool.MakeInitialObject();

            return pool;
        }

        /// <summary>
        /// 型(T)に対応するプールを作成する
        /// </summary>
        /// <param name="number">プールのサイズ</param>
        /// <param name="getNewInstance">型Tのインスタンスを生成する方法</param>
        /// <typeparam name="T">プールの対象となる型</typeparam>
        /// <exception cref="Exception">既に型Tに対応するプールが存在する場合</exception>
        internal UIElementPool<T> MakeUIElementPool<T>(int number, Func<T> getNewInstance) where T : UIElement, IPoolObject
        {

            //型TがMonoBehaviourを継承しているか否かで、生成するプールを分ける
            var pool = new UIElementPool<T>(number, getNewInstance);

            //指定の数だけプール内にインスタンスを生成する
            pool.MakeInitialObject();

            return pool;
        }

    }
}
