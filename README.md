# ![logo](https://user-images.githubusercontent.com/5454041/56529994-f8a79780-6551-11e9-887d-5787a5576595.png) Authui Patcher


## Screenshots
![unpatched screenshot](https://user-images.githubusercontent.com/5454041/56529894-ce55da00-6551-11e9-828a-f217004a6002.png)

![patched screenshot](https://user-images.githubusercontent.com/5454041/56529906-d4e45180-6551-11e9-843c-3ef6aab07dfb.png)


## Description
This software patches the authui.dll file on Windows 7 (which is located in the System32 folder) to bypass the arbitrary 256 KB limit which has been set for custom logon images. A backup of the original file is made before patching, and it's possible to restore the original file after patching. Credit for the patching method goes to [this forum post](https://opencarnage.net/index.php?/topic/4444-custom-windows-7-loginlock-background-with-256kb-limit-removal/) and [this superuser answer](https://superuser.com/a/1014847).

**Use the x64 version on 64 bit Windows 7.**

**Use the x86 version on 32 bit Windows 7.**

**.NET Framework 4.6.1 or later is required.**

## Usage
- [Download](https://github.com/Wyse-/authui-patcher/releases) the right release for your OS (x64 if your OS is 64 bit, x86 if your OS is 32 bit).
- Run the executable file and click the patch button.
- [Enable custom logon screens](https://www.howtogeek.com/112110/how-to-set-a-custom-logon-screen-background-on-windows-7/), both the regedit and group policy methods will work.
- Place your custom logon image in `C:\Windows\System32\oobe\info\backgrounds` and name it `backgroundDefault.jpg`, if your image is a .png file just rename it to .jpg, it will work.
- Done!

It's possible the authui.dll file will get overwritten with updated versions by future Windows updates, if this happens just patch it again the same way. I doubt the patching method will break anytime soon considering it's been the same since 2015.

## Compiling & testing
This project has been compiled with Visual Studio Community 2017 and tested on Windows 7 x64/x86.
