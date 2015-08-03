using System; 
using Android.App;
using Android.Content; 
using Android.Widget;
using Android.OS;
using Xamarin.Auth;
using Newtonsoft.Json;
using System.Net;
using System.Threading.Tasks; 

//LoginbyGoogle : to authenticate google user
//date : July 29 2015
//by suchith madavu
//component : Xamarin.Auth,Json.Net

namespace GoogleLoginSample
{
	[Activity ( Label = "GoogleLogin" , MainLauncher = true , Icon = "@drawable/icon" )]
	public class MainActivity : Activity
	{  
		string access_token;  
		ProgressDialog progress;  
		const string googUesrInfoAccessleUrl = "https://www.googleapis.com/oauth2/v1/userinfo?access_token={0}";  
		GoogleInfo googleInfo;
		protected override void OnCreate ( Bundle bundle )
		{
			base.OnCreate ( bundle ); 
			SetContentView ( Resource.Layout.Main ); 
			LoginByGoogle (true); 
		}
  
		void LoginByGoogle (bool allowCancel)
		{   
			var auth = new OAuth2Authenticator ( clientId: "847549521106-35dvaoe5jafmc5tuk2ll5054********.apps.googleusercontent.com" ,
				scope: "https://www.googleapis.com/auth/userinfo.email" ,
				authorizeUrl: new Uri ( "https://accounts.google.com/o/oauth2/auth" ) , 
				redirectUrl: new Uri ( "https://www.googleapis.com/plus/v1/people/me" ) , 
				getUsernameAsync: null ); 
			
			auth.AllowCancel = allowCancel;    
			auth.Completed += async (sender , e ) =>
			{  
				if ( !e.IsAuthenticated )
				{ 
					Toast.MakeText(this,"Fail to authenticate!",ToastLength.Short).Show(); 
					return;
				} 
				e.Account.Properties.TryGetValue ( "access_token" , out access_token );  

				if(await fnGetProfileInfoFromGoogle ())
				{
					Toast.MakeText ( this , "Authentcated successfully" , ToastLength.Short ).Show ();
				}
			};  

			var intent = auth.GetUI ( this );
			StartActivity ( intent ); 
		}

		async Task<bool> fnGetProfileInfoFromGoogle()
		{ 
			progress =  ProgressDialog.Show (this,"","Please wait..."); 
			bool isValid=false;
			//Google API REST request
			string userInfo = await fnDownloadString (string.Format(googUesrInfoAccessleUrl, access_token ));  
			if ( userInfo != "Exception" )
			{ 
				googleInfo = JsonConvert.DeserializeObject<GoogleInfo> ( userInfo );   
				isValid = true;
			}
			else
			{ 
				if ( progress != null )
				{
					progress.Dismiss ();
					progress = null;
				}   
				isValid = false;
				Toast.MakeText ( this , "Connection failed! Please try again" , ToastLength.Short ).Show (); 
			}
			if ( progress != null )
			{
				progress.Dismiss ();
				progress = null;
			}  
			return isValid;
		}
		async Task<string> fnDownloadString(string strUri)
		{ 
			var webclient = new WebClient ();
			string strResultData;
			try
			{
				strResultData= await webclient.DownloadStringTaskAsync (new Uri(strUri));
				Console.WriteLine(strResultData);
			}
			catch
			{
				strResultData = "Exception";
				RunOnUiThread ( () =>
					Toast.MakeText ( this , "Unable to connect to server!!!" , ToastLength.Short ).Show());
			}
			finally
			{
				if ( webclient!=null )
				{
					webclient.Dispose ();
					webclient = null; 
				}
			}

			return strResultData;
		}
	
		//login in iOS
//		void LoginByGoogle(bool allowCancel)
//		{
//			var auth = new OAuth2Authenticator (clientId , "https://www.googleapis.com/auth/userinfo.email" ,
//				new Uri ( "https://accounts.google.com/o/oauth2/auth" ) , 
//				new Uri ( "http://********.net/aspxLoginSuccess.aspx" ) , null );  
//			auth.AllowCancel = allowCancel;   
//			string access_token;
//			auth.Completed +=async (sender , e ) =>
//			{  	if(e.IsAuthenticated)
//				{
//					e.Account.Properties.TryGetValue ( "access_token" , out access_token ); 
//					await fnGetProfileInfoFromGoogle(access_token);
//				} 
//				DismissViewController(true,null);
//			};
//
//			var intent = auth.GetUI (); 
//			PresentViewController(intent,true,null);
//		}
 
	}
}


