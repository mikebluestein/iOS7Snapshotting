using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace SnapshotDemo
{
	public partial class SnapshotDemoViewController : UIViewController
	{
		DrawingView drawing;
		UIButton button;

		public override void LoadView ()
		{
			base.LoadView ();

			drawing = new DrawingView {Frame = UIScreen.MainScreen.Bounds};
			View = drawing;
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			float buttonWidth = 120;
			float buttonHeight = 50;

			button = UIButton.FromType (UIButtonType.System);

			button.Frame = new RectangleF (View.Frame.Width / 2 - buttonWidth/2, 
			                                     View.Frame.Bottom - buttonHeight, 
			                                     buttonWidth, 
			                                     buttonHeight);

			button.AutoresizingMask = UIViewAutoresizing.FlexibleTopMargin | 
				UIViewAutoresizing.FlexibleLeftMargin | UIViewAutoresizing.FlexibleRightMargin;

			button.SetTitle ("Take Snapshot", UIControlState.Normal);
			View.AddSubview (button);

			button.TouchUpInside += (sender, e) => {
				Snapshot ();
			};
		}

		void Snapshot()
		{
			UIImage image;

			UIGraphics.BeginImageContext (View.Frame.Size);

			//pre-iOS 7 using layer to snapshot
			//View.Layer.RenderInContext (UIGraphics.GetCurrentContext ());

			//new iOS 7 method to snapshot
			View.DrawViewHierarchy (View.Frame, true);

		    image = UIGraphics.GetImageFromCurrentImageContext ();
			UIGraphics.EndImageContext ();
		
			image.SaveToPhotosAlbum((img, err) => {
				if(err != null)
					Console.WriteLine("error saving image: {0}", err);
				else
					Console.WriteLine ("image saved to photo album");
			});
		}
	}
}

