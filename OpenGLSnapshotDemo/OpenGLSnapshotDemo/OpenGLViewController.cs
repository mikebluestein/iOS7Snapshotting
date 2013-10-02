using System;
using System.Drawing;
using MonoTouch.CoreAnimation;
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
using MonoTouch.OpenGLES;
using MonoTouch.UIKit;
using OpenTK;
using OpenTK.Graphics.ES20;
using OpenTK.Platform.iPhoneOS;

namespace OpenGLSnapshotDemo
{
	[Register ("OpenGLViewController")]
	public partial class OpenGLViewController : UIViewController
	{
		UIButton button;

		public OpenGLViewController (string nibName, NSBundle bundle) : base (nibName, bundle)
		{
		}

		new EAGLView View { get { return (EAGLView)base.View; } }

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			NSNotificationCenter.DefaultCenter.AddObserver (UIApplication.WillResignActiveNotification, a => {
				if (IsViewLoaded && View.Window != null)
					View.StopAnimating ();
			}, this);
			NSNotificationCenter.DefaultCenter.AddObserver (UIApplication.DidBecomeActiveNotification, a => {
				if (IsViewLoaded && View.Window != null)
					View.StartAnimating ();
			}, this);
			NSNotificationCenter.DefaultCenter.AddObserver (UIApplication.WillTerminateNotification, a => {
				if (IsViewLoaded && View.Window != null)
					View.StopAnimating ();
			}, this);

			float buttonWidth = 120;
			float buttonHeight = 50;

			button = UIButton.FromType (UIButtonType.System);

			button.TintColor = UIColor.Purple;
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

			//pre-iOS 7 using layer to snapshot render an empty image when used with OpenGL
			//View.Layer.RenderInContext (UIGraphics.GetCurrentContext ());

			//new iOS 7 method to snapshot works with OpenGL
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

		protected override void Dispose (bool disposing)
		{
			base.Dispose (disposing);
			
			NSNotificationCenter.DefaultCenter.RemoveObserver (this);
		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			View.StartAnimating ();
		}

		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);
			View.StopAnimating ();
		}
	}
}
