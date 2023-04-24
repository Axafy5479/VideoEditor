using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Timeline
{

    public class ContextMenuKeyHandleBehavior : Behavior<ContextMenu>
    {
        /* 依存プロパティの定義 */
        public static readonly DependencyProperty CommandKeyHandleProperty =
         DependencyProperty.Register(nameof(CommandKeyHandle),
          typeof(ICommand),
          typeof(ContextMenuKeyHandleBehavior),
          new PropertyMetadata(null));
        public ICommand CommandKeyHandle
        {
            get=> (ICommand)GetValue(CommandKeyHandleProperty);
            set=> SetValue(CommandKeyHandleProperty, value);
        }

        /* ビヘイビア生成時の挙動 */
        protected override void OnAttached()
        {
            base.OnAttached();

            // キー入力があった場合に、ContextMenuの処理判定を行う
            AssociatedObject.KeyDown += AssociatedObjectOnKeyDown;
        }

        /* ビヘイビア削除時の挙動 */
        protected override void OnDetaching()
        {
            base.OnDetaching();

            AssociatedObject.KeyDown -= AssociatedObjectOnKeyDown;
        }

        /* キーイベント入力 */
        private void AssociatedObjectOnKeyDown(object sender, KeyEventArgs e)
        {
            // 例として、Escapeキーを押すと、ContextMenuが終了するようにする
            if (e.Key == Key.Escape)
            {
                AssociatedObject.IsOpen = false;
                return;
            }

            // もし所定のキーが入力されたとして、
            // コマンドが定義されていて実行可能であれば、
            // 実行し、ContextMenuが終了するようにする
            if (CommandKeyHandle != null && CommandKeyHandle.CanExecute(e))
            {
                CommandKeyHandle.Execute(e);

                AssociatedObject.IsOpen = false;
            }
        }
    }


}
