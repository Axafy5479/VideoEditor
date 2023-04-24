using System;
using System.Windows.Controls;

namespace WB.Pool
{
    /// <summary>
    /// オブジェクトプールを使用したいクラスは
    /// 必ずこのinterfaceを実装する
    /// </summary>
    public interface IPoolObject
    {
        /// <summary>
        /// オブジェクトがインスタンス化された際に1度だけ呼ばれるメソッド
        /// </summary>
        /// <param name="id">id(何番目に生成されたか)</param>
        int Id { get; set; }
        
        /// <summary>
        /// プールに入った際(インスタンス化直後 & プールに返却された際)に呼ばれるメソッド
        /// </summary>
        void Initialize();

        /// <summary>
        /// インスタンスをプールに返すメソッドを渡す
        /// </summary>
        void SetReturnMethod(Action returnMethod);

        /// <summary>
        /// 表示中のpoolオブジェクトを保持している親コンポーネントを取得する
        /// </summary>
        /// <returns></returns>
        Panel GetParent();
    }
}
