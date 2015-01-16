# eaTopic
<p align="center">
<a href="https://travis-ci.org/Otupus/eaTopic"><img alt="Build Status" src="https://travis-ci.org/pleonex/eaTopic.svg?branch=master" align="left" /></a>
<a href="http://www.gnu.org/copyleft/gpl.html"><img alt="license" src="https://img.shields.io/badge/license-GPL%20V3-blue.svg?style=flat" /></a>
<a href="https://github.com/fehmicansaglam/progressed.io"><img alt="progressed.io" src="http://progressed.io/bar/80" align="right" /></a>
</p>

<br>
<p align="center"><b>A Publication - Subscription protocol implementation.</b></p>


## Compilation
It has been developed and tested with *mono 3.10.0* in *Fedora 20*.

### Linux
You need to install *git* using your package manager (ie *apt-get*, *yum*, *pacman*...) and the last stable mono version from [here](http://www.mono-project.com/docs/getting-started/install/linux/).
``` shell
# Clone the repository
git clone https://github.com/pleonex/eaTopic
cd eaTopic
```

Now, you can either open the solution with *MonoDevelop* or compile from the terminal:
``` shell
# Restore NuGet packages
wget http://nuget.org/nuget.exe
mono nuget.exe eaTopic.sln

# Compile
xbuild eaTopic.sln

# [Optional] Run test
# Install nunit-console from your package manager
nunit-console eaTopic.Tests/bin/Debug/eaTopic.Tests.dll
```

### Windows
1. Clone the repository with the [GitHub client](https://windows.github.com/) or download the [zip](https://github.com/pleonex/eaTopic/archive/master.zip).
2. Download and install *Xamarin Studio* from [here](http://www.monodevelop.com/download/) and open the solution. It should work with *Visual Studio* and [*SharpDevelop*](http://www.icsharpcode.net/OpenSource/SD/Download/) too.
3. Compile!
