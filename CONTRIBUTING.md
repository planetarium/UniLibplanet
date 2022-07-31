Contributor guide
=================

# Setup
First. Submodule needs to be update
```
git submodule update --recursive
```

You must run a script to **select a unity versions**. Now we recommend using the `2021.3.0f1` version.  
Please run the scripts below in the root folder.

If you never set execution policy on Powershell, your execution policy would be set `Restricted`, which is default execution policy for Windows client computers. 
You need to change policy to execute Powershell scripts.
```powershell
Set-ExecutionPolicy RemoteSigned
```

___if you use PowerShell 7.2.x versions check this [Issue](https://github.com/PowerShell/PowerShell/issues/17322)___

PowerShell
```
.\scripts\create-dotenv.ps1
```

Bash
```
Sorry, we're getting ready.
```

And we need to proceed with the build for the Unity development environment.  

PowerShell
```
.\scripts\build.ps1
```

Bash
```
Sorry, we're getting ready.
```

When the script is complete, several Unity dependencies will be installed.
If you want `UniLibplanet` to be developed by Unity, please open `UniLibplanet` to Unity and develop.

# Build

We have already build during the above Setup stage.  
When you run the build, you install `libplanet` dependencies and pack them into SDKs. (See [build.ps1](./scripts/build.ps1) for more information.)  
The `.unitypackage` file created in the [out/](./out/) directory.

PowerShell
```
.\scripts\build.ps1
```

Bash
```
Sorry, we're getting ready.
```
