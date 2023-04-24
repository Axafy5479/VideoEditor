using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace WB.Pool
{
    /// <summary>
    /// プール本体
    /// (MonoBehaviourでないインスタンス用のプール)
    /// </summary>
    public class Pool<T> where T : IPoolObject

    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="number">プールのサイズ</param>
        /// <param name="getNewInstance">インスタンス化の方法</param>
        internal Pool(int number, Func<T> getNewInstance)
        {
            //引数の保持
            GetNewInstance = getNewInstance;
            InstanceNum = number;

            //queueの初期化
            Queue = new Queue<T>();
        }

        /// <summary>
        /// 未使用のインスタンスを保持
        /// </summary>
        private Queue<T> Queue { get; }

        /// <summary>
        /// インスタンスの総数
        /// </summary>
        private int InstanceNum { get; set; }

        /// <summary>
        /// インスタンス化の方法
        /// (ユーザーが設定した関数)
        /// </summary>
        protected Func<T> GetNewInstance { get; }



        /// <summary>
        /// プールの初期化直後に呼ぶべきメソッド
        /// (この操作をコンストラクタに置くと、MonoPoolのParentが決定する前にインスタンスが生成され、ヒエラルキーのroot上に配置されてしまう)
        /// (継承先のコンストラクタは継承もとより後に実行されることによる問題)
        ///
        /// 指定の数のインスタンスを生成
        /// </summary>
        internal void MakeInitialObject()
        {
            for (int i = 0; i < InstanceNum; i++) InstantiatePoolObj(i);
        }

        /// <summary>
        /// インスタンスを生成し、プールに入れる
        /// </summary>
        /// <param name="id"></param>
        protected virtual T InstantiatePoolObj(int id)
        {
            //ユーザーから指定された方法でインスタンス化を行う
            T instance = GetNewInstance();

            //インスタンス化された順番を通知
            instance.Id = id;

            //プールに返す方法を伝える
            instance.SetReturnMethod(() => Return(instance));

            //オブジェクトの初期化
            instance.Initialize();

            //Queueに追加
            Queue.Enqueue(instance);

            return instance;
        }

        /// <summary>
        /// プール内のインスタンスを一つ取り出す
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public virtual T Rent()
        {
            //プールにインスタンスが残っている場合、そのうちの一つを取り出す
            if (Queue.Count > 0)
            {
                return Queue.Dequeue();
            }


            //Queue内のインスタンスの個数が0の場合 = 未使用のインスタンスが残っていない場合

            if (InstanceNum < 1e8)
            {
                //生成されたインスタンス数が10^8未満の場合(オーバーフローを避ける目的)

                //プールサイズを二倍にする
                for (int i = InstanceNum; i < 2 * InstanceNum; i++)
                {
                    InstantiatePoolObj(i);
                }

                InstanceNum *= 2;

                //警告を表示
                Debug.WriteLine($"ObjectPoolの最大数を{InstanceNum}に拡張しました");

                //プール内のインスタンスを一つ取り出す
                return Queue.Dequeue();
            }
            else
            {
                //10^8を超えている場合は、これ以上拡張できないため例外を投げる
                throw new Exception(
                    $"ObjectPool内のインスタンスがなくなりました。\n現状ObjectPool内のインスタンス数が{InstanceNum}個であるため、これ以上拡張できません");
            }


        }

        /// <summary>
        /// インスタンスをプールに戻す
        /// </summary>
        /// <param name="instance"></param>
        public virtual void Return(T instance)
        {
            //オブジェクトの初期化
            instance.Initialize();

            //Queueに戻す
            Queue.Enqueue(instance);
        }


    }
}
