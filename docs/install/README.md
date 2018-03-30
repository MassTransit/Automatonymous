# Installing Automatonymous

## Requirements

Automatonymous requires .NET 4.5.2 (or later). Due to the extensive use of 
the TPL and *async/await*, earlier .NET versions are not supported.

## How to install

### NuGet

The simplest way to install MassTransit into your solution/project is to use
NuGet:

```
Install-Package Automatonymous
```

### Compiling From Source

If you want to hack on Automatonymous or just want to have the actual source
code you can clone the source from github.com.

To clone the repository using git try the following::

```
git clone https://github.com/MassTransit/Automatonymous.git
```

If you want the development branch (where active development happens)::

```
git clone https://github.com/MassTransit/Automatonymous.git
git checkout develop
```

**Compiling**

Automatonymous uses FAKE to build itself. FAKE is installed from NuGet,
so all you have to do is to drop to the command line and type:

```
build.bat
```

If you look in the `.\build_output` folder you should see the binaries.
