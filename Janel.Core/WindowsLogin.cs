using System;
using System.Security.Principal;

namespace Janel.Core
{
  public class WindowsLogin : IDisposable {
    protected const int LOGON32_PROVIDER_DEFAULT = 0;
    protected const int LOGON32_LOGON_INTERACTIVE = 2;

    public WindowsIdentity Identity = null;
    private IntPtr _accessToken;


    [System.Runtime.InteropServices.DllImport("advapi32.dll", SetLastError = true)]
    private static extern bool LogonUser(string lpszUsername, string lpszDomain,
    string lpszPassword, int dwLogonType, int dwLogonProvider, ref IntPtr phToken);

    [System.Runtime.InteropServices.DllImport("kernel32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
    private extern static bool CloseHandle(IntPtr handle);


    public WindowsLogin() {
      Identity = WindowsIdentity.GetCurrent();
    }
    
    public WindowsLogin(string username, string domain, string password) {
      Login(username, domain, password);
    }
    
    public void Login(string username, string domain, string password) {
      if (Identity != null) {
        Identity.Dispose();
        Identity = null;
      }


      try {
        _accessToken = new IntPtr(0);
        Logout();

        _accessToken = IntPtr.Zero;
        var logonSuccessfull = LogonUser(
           username,
           domain,
           password,
           LOGON32_LOGON_INTERACTIVE,
           LOGON32_PROVIDER_DEFAULT,
           ref _accessToken);

        if (!logonSuccessfull) {
          var error = System.Runtime.InteropServices.Marshal.GetLastWin32Error();
          throw new System.ComponentModel.Win32Exception(error);
        }
        Identity = new WindowsIdentity(_accessToken);
      }
      catch {
        throw;
      }
    } 


    public void Logout() {
      if (_accessToken != IntPtr.Zero)
        CloseHandle(_accessToken);

      _accessToken = IntPtr.Zero;

      if (Identity != null) {
        Identity.Dispose();
        Identity = null;
      }
    }

    void IDisposable.Dispose() {
      Logout();
    }
  }  
}
