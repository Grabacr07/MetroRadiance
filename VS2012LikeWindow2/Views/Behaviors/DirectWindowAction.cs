using System.Windows;
using System.Windows.Interactivity;

namespace VS2012LikeWindow2.Views.Behaviors
{
	class DirectWindowAction : TriggerAction<FrameworkElement>
	{
		#region WindowAction 依存関係プロパティ

		public WindowAction WindowAction
		{
			get { return (WindowAction)this.GetValue(DirectWindowAction.WindowActionProperty); }
			set { this.SetValue(DirectWindowAction.WindowActionProperty, value); }
		}
		public static readonly DependencyProperty WindowActionProperty =
			DependencyProperty.Register("WindowAction", typeof(WindowAction), typeof(DirectWindowAction), new UIPropertyMetadata(WindowAction.Active));

		#endregion

		protected override void Invoke(object parameter)
		{
			var window = Window.GetWindow(this.AssociatedObject);
			if (window != null)
			{
				switch (this.WindowAction)
				{
					case WindowAction.Active:
						window.Activate();
						break;
					case WindowAction.Close:
						window.Close();
						break;
					case WindowAction.Maximize:
						window.WindowState = WindowState.Maximized;
						break;
					case WindowAction.Minimize:
						window.WindowState = WindowState.Minimized;
						break;
					case WindowAction.Normalize:
						window.WindowState = WindowState.Normal;
						break;
					case WindowAction.OpenSystemMenu:
						var point = this.AssociatedObject.PointToScreen(new Point(0, this.AssociatedObject.ActualHeight));
						SystemCommands.ShowSystemMenu(window, point);
						break;
				}
			}
		}
	}
}
