using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;

namespace WB.Pool
{

    /// <summary>
    /// MonoBehaviour用のプール本体
    /// </summary>
    public class UIElementPool<T> : Pool<T> where T: UIElement,IPoolObject
    {
        
        /// <summary>
        /// コンストラクタ
        /// プール生成とともに、IPoolObjectの配置位置を定める
        /// </summary>
        /// <param name="number"></param>
        /// <param name="getNewInstance"></param>
        /// <param name="parent"></param>
        internal UIElementPool(int number, Func<T> getNewInstance) : base(number, getNewInstance)
        {
        }

        /// <summary>
        /// インスタンスをプールに返却する
        ///
        /// このとき親オブジェクトをParentに設定し、
        /// 非アクティブにする
        /// </summary>
        /// <param name="monoObj">返却するオブジェクト</param>
        public override void Return(T monoObj)
        {
            monoObj.GetParent().Children.Remove(monoObj);
            base.Return(monoObj);
        }
    }
}
