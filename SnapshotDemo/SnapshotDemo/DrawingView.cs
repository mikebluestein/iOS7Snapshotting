using System;
using MonoTouch.UIKit;
using MonoTouch.CoreGraphics;
using System.Drawing;
using MonoTouch.CoreAnimation;
using MonoTouch.Foundation;

namespace SnapshotDemo
{
	public class DrawingView : UIView
	{
		CGPath path;
		PointF initialPoint;
		PointF latestPoint;
		CALayer layer;

		public DrawingView ()
		{
			BackgroundColor = UIColor.White;

			path = new CGPath ();

			layer = new CALayer ();
			layer.Bounds = new RectangleF (0, 0, 50, 50);
			layer.Position = new PointF (50, 50);
			layer.Contents = UIImage.FromFile ("monkey.png").CGImage;
			layer.ContentsGravity = CALayer.GravityResizeAspect;
		}

		public override void TouchesBegan (NSSet touches, UIEvent evt)
		{
			base.TouchesBegan (touches, evt);

			var touch = touches.AnyObject as UITouch;

			if (touch != null) {
				initialPoint = touch.LocationInView (this);
			}
		}

		public override void TouchesMoved (NSSet touches, UIEvent evt)
		{
			base.TouchesMoved (touches, evt);

			var touch = touches.AnyObject as UITouch;

			if (touch != null) {
				latestPoint = touch.LocationInView (this);
				SetNeedsDisplay ();
			}
		}

		public override void TouchesEnded (NSSet touches, UIEvent evt)
		{
			base.TouchesEnded (touches, evt);

			if (layer.SuperLayer == null)
				Layer.AddSublayer (layer);

			layer.Position = latestPoint;
			CAKeyFrameAnimation animPosition = CAKeyFrameAnimation.GetFromKeyPath ("position");
			animPosition.Path = path;
			animPosition.Duration = 3;
			layer.AddAnimation (animPosition, "position");
		}

		public override void Draw (RectangleF rect)
		{
			base.Draw (rect);

			if (!initialPoint.IsEmpty) {

				using(CGContext g = UIGraphics.GetCurrentContext ()){

					g.SetLineWidth (10);
					UIColor.Purple.SetStroke ();

					if (path.IsEmpty) {
						path.AddLines (new PointF[]{initialPoint, latestPoint});
					} else {
						path.AddLineToPoint (latestPoint);
					}

					g.AddPath (path);		
					g.DrawPath (CGPathDrawingMode.Stroke);
				}
			}
		}	           
	}
}


