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
		GoogleInfo googleInfo;
		const string googleUrl = "https://www.googleapis.com/oauth2/v1/userinfo?access_token={0}"; 

		protected override void OnCreate ( Bundle bundle )
		{
			base.OnCreate ( bundle ); 
			SetContentView ( Resource.Layout.Main ); 
			LoginByGoogle (true); 
		}


		void LoginByGoogle (bool allowCancel)
		{  
			//Google  credentials  for Project, need to register in developer.google.com
			//Client id has to be generated  from google page by providing hash code from  keystore
			//redirecturi : is the page to be shown after successful login,here shown sample page from one of the running application
			//for this any of the youmeus hosted page can be taken 
			var auth = new OAuth2Authenticator ( clientId: "847549521106-35dvaoe5jafmc5tuk2ll5054********.apps.googleusercontent.com" ,
				scope: "https://www.googleapis.com/auth/userinfo.email" ,
				authorizeUrl: new Uri ( "https://accounts.google.com/o/oauth2/auth" ) , 
				redirectUrl: new Uri ( "http://*********.com/********.aspx" ) , 
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
				//bool isValid=;
				if(await fnGetInfo ())
				{
					Toast.MakeText ( this , "Authentcated successfully" , ToastLength.Short ).Show ();
				}
			};  

			var intent = auth.GetUI ( this );
			StartActivity ( intent ); 
		}
 
		async Task<bool> fnGetInfo()
		{ 
			progress =  ProgressDialog.Show (this,"","Please wait..."); 
			bool isValid=false;
			string userInfo = await fnDownloadString (string.Format(googleUrl, access_token ));  
			if ( userInfo != "Exception" )
			{ 
				googleInfo = JsonConvert.DeserializeObject<GoogleInfo> ( userInfo ); 
				Console.WriteLine(string.Format("Email id : {0}", googleInfo.email));   
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


	}
}


