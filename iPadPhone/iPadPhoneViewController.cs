﻿using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

using TwilioClient.iOS;
using System.Net.Http;

namespace iPadPhone
{
	public partial class iPadPhoneViewController : UIViewController
	{
		TCDevice _device;
		TCConnection _connection;

		public iPadPhoneViewController (IntPtr handle) : base (handle)
		{
		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}

		#region View lifecycle

		public async override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// Create an HTTPClient object and use it to fetch
			// a capability token from our site on Azure. By default this
			// will give us a client name of 'alice'
			var client = new HttpClient ();
			var token = await client.GetStringAsync("http://<your_webapp_url>/Client/Token");

			// Create a new TCDevice object passing in the token.
			_device = new TCDevice (token, null);

			// Set up the event handlers for the device
			SetupDeviceEvents ();

			// Add the Twilio logo
			var logo = new UIImageView (new RectangleF (200, 120, 400, 120));
			logo.Image = UIImage.FromBundle ("Twilio_logo_red");
			this.View.Add (logo);

			// Add a UITextField for the phone number to call
			var numberField = new UITextField (new RectangleF (200, 300, 400, 30));
			numberField.BorderStyle = UITextBorderStyle.RoundedRect;
			numberField.TextAlignment = UITextAlignment.Center;
			numberField.Placeholder = "Phone number";
			this.View.Add (numberField);

			// Add a UIButton to initiate the call
			var callButton = new UIButton (UIButtonType.Custom);
			callButton.SetBackgroundImage(UIImage.FromBundle("call_button"), UIControlState.Normal);
			callButton.Frame = new RectangleF (200, 380, 400, 40);
			this.View.Add (callButton);

			// Add code to TouchUpInside to place the call when the user taps the button
			callButton.TouchUpInside += (sender, e) => {
				// Setup the numbers to use for the call
				// TODO: Update this code to include your Twilio number
				NSDictionary parameters = NSDictionary.FromObjectsAndKeys (
					new object[] { "*** Your Twilio Number ***", numberField.Text },
					new object[] { "Source", "Target" }
				);

				// Make the call
				_connection = _device.Connect(parameters, null);
			};

			// Add a UIButton to press any key
			var anyKeyButton = new UIButton (UIButtonType.Custom);
			anyKeyButton.SetBackgroundImage(UIImage.FromBundle("any_key"), UIControlState.Normal);
			anyKeyButton.Frame = new RectangleF (200, 430, 400, 40);
			this.View.Add (anyKeyButton);

			anyKeyButton.TouchUpInside += (sender, e) => {
				if (_connection != null && _connection.State == TCConnectionState.Connected) {
					//We can't just press 'any' key so we'll press 1
					_connection.SendDigits("1");
				}
			};

			// Add a UIButton to hang up
			var hangupButton = new UIButton (UIButtonType.Custom);
			hangupButton.SetBackgroundImage(UIImage.FromBundle("hangup_button"), UIControlState.Normal);
			hangupButton.Frame = new RectangleF (200, 480, 400, 40);
			this.View.Add (hangupButton);

			// Hang up the call
			hangupButton.TouchUpInside += (sender, e) => {
				_connection.Disconnect();
			};
		}

		void SetupDeviceEvents ()
		{
			if (_device != null) 
			{
				// When a new connection comes in, store it and use it to accept the incoming call.
				_device.ReceivedIncomingConnection += (sender, e) => {
					_connection = e.Connection;
					_connection.Accept();
				};
			}
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
		}

		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);
		}

		public override void ViewDidDisappear (bool animated)
		{
			base.ViewDidDisappear (animated);
		}

		#endregion
	}
}

