==================================
UI Sleuth v1.1.1
==================================

UI Sleuth is a Xamarin.Forms debugging tool. 
If you’ve ever made a web site, it’s similar to Microsoft’s F12 tools or Chrome Developer Tools. 
You can use it to efficiently track down layout issues, prototype a new design, and remotely control a device.

Now that UI Sleuth runs on multiple platforms, you can inspect any app that has this nuget package installed and initialized, regardless of the platform.
Please take a few minutes and read the installation steps below.

Michael
@mykldavis
https://www.uisleuth.com

==================================
iOS Installation Steps
==================================

Step 1. Code Changes.
In your iOS project's AppDelegate class, add the following lines after the call to LoadApplication():

#if DEBUG
    UISleuth.Inspector.Init();

    // Optional*
    UISleuth.Inspector.ShowAcceptingConnections();
#endif

*  If you're deploying to a local iOS simulator, this line is especially optional.
   UI Sleuth can detect apps running on a local iOS simulator that are using the default configuration values set by the call to
   UISleuth.Inspector.Init();

*  UI Sleuth searches for iOS apps every 10 seconds.
   If your app still doesn't display in the "Discovery" tab, try using these settings on the "Manual Connection" tab:

   address: localhost
   port: 9099

*  If you're on a remote machine, even a Windows PC, you can connect to a iOS simulator running on a network connected macOS.
   Enter similiar information on the "Manual Connection" tab:

   address: mymac.local
   port: 9099

Step 2. Launch UI Sleuth.
If you don't have the desktop application, download the macOS or Windows (x64) binaries from https://www.uisleuth.com/.

Step 3. Connect.
Using the "Discovery" or "Manual Connection" tab, connect to your running mobile application.

---------------
Example
---------------
[Register("AppDelegate")]
public partial class AppDelegate : FormsApplicationDelegate
{
    public override bool FinishedLaunching(UIApplication app, NSDictionary options)
    {
        global::Xamarin.Forms.Forms.Init ();
        LoadApplication (new MyXamarinFormsApp());

        #if DEBUG
	    UISleuth.Inspector.Init();
            UISleuth.Inspector.ShowAcceptingConnections();
        #endif

	return base.FinishedLaunching (app, options);
    }
}

==================================
Android Installation Steps
==================================

Step 1. Code Changes.
In your Android project's MainActivity class, add the following lines after the call to LoadApplication():

#if DEBUG
    UISleuth.Inspector.Init();

    // Optional*
    UISleuth.Inspector.ShowAcceptingConnections();
#endif

*  If you're deploying to a local Android Emulator and you have ADB in your environment's path,
   UI Sleuth should be able to automatically detect it. You should see the device listed on the "Discovery" tab.

*  UI Sleuth can also detect physical devices connected via USB.
   If your device is not shown on the "Discovery" tab, try using the "Manual Connection" tab.
   UISleuth.Inspector.ShowAcceptingConnections() simply displays the local devices' IP Address.
   Use this information to connect to the device.

Step 2. Launch UI Sleuth.
If you don't have the desktop application, download the macOS or Windows (x64) binaries from https://www.uisleuth.com/.

Step 3. Connect.
Using the "Discovery" or "Manual Connection" tab, connect to your running mobile application.

---------------
Example
---------------
public class MainActivity : FormsApplicationActivity
{
    protected override void OnCreate(Bundle bundle)
    {
        base.OnCreate(bundle);
        Forms.Init(this, bundle);

        LoadApplication(new YourApp());

        #if DEBUG
            UISleuth.Inspector.Init();
            UISleuth.Inspector.ShowAcceptingConnections();
        #endif
    }
}

==================================
FAQ
==================================

Q: UI Sleuth did not detect any Android Devices.
A: Ensure you have the Android SDK installed and ADB.exe in your environment's path.

Android SDK Download:
https://developer.android.com/studio/index.html

Q: My device was detected, but I can't connect to it. What do I do?
A: Ping the device's IP address. If the ping command is not successful, you are having a network related issue. If your device and desktop are
on different subnets, you may not be able to start a connection.

Q: How do I get the IP Address of my device?
A: You have many options.

Option 1. Code Changes.
In your Android project's MainActivity class, add the following lines after the call to LoadApplication():

#if DEBUG
    UISleuth.Inspector.ShowAcceptingConnections();
#endif

Option 2. Debug Output.
If you have a debugger attached to your application, launch Visual Studio's debug output. Look for a line similiar to:

UI Sleuth: Listening on ws://169.254.190.187:9099/

Option 3. ADB.
At a command prompt, type:
> adb devices

Option 4. See your emulator's documentation.