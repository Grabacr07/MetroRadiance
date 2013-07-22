using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace VS2012LikeWindow2.Views.Chrome
{
	public class CaptionButton : Button
	{
		static CaptionButton()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof (CaptionButton), new FrameworkPropertyMetadata(typeof (CaptionButton)));
		}

		#region WindowAction 依存関係プロパティ

		/// <summary>
		/// ボタンに割り当てるウィンドウ操作を取得または設定します。
		/// </summary>
		public WindowAction WindowAction
		{
			get { return (WindowAction)this.GetValue(WindowActionProperty); }
			set { this.SetValue(WindowActionProperty, value); }
		}

		/// <summary>
		/// <see cref="P:VS2012LikeWindow2.Views.Chrome.CaptionButton.WindowAction"/> 依存関係プロパティを識別します。
		/// </summary>
		public static readonly DependencyProperty WindowActionProperty =
			DependencyProperty.Register("WindowAction", typeof (WindowAction), typeof (CaptionButton), new UIPropertyMetadata(WindowAction.None));

		#endregion

		protected override void OnClick()
		{
			this.WindowAction.Invoke(this);
			base.OnClick();
		}
	}
}
