
#load "../../common.cake"

var TARGET = Argument ("t", Argument ("target", "Default"));

var buildSpec = new BuildSpec () {
	Libs = new [] {
		new DefaultSolutionBuilder {
			SolutionPath = "./source/Xamarin.SimplePing.sln",
			OutputFiles = new [] {
				new OutputFileCopy { FromFile = "./source/Xamarin.SimplePing.iOS/bin/Release/Xamarin.SimplePing.iOS.dll" },
				new OutputFileCopy { FromFile = "./source/Xamarin.SimplePing.Mac/bin/Release/Xamarin.SimplePing.Mac.dll" },
			}
		},
	},

	Samples = new [] {
		new IOSSolutionBuilder { SolutionPath = "./samples/SimplePingSample.iOS/SimplePingSample.iOS.sln" },
		new IOSSolutionBuilder { SolutionPath = "./samples/SimplePingSample.Mac/SimplePingSample.Mac.sln", Platform = "x64" },
	},

	NuGets = new [] {
		new NuGetInfo { NuSpec = "./nuget/Xamarin.SimplePing.nuspec" },
	},

	Components = new [] {
		new Component { ManifestDirectory = "./component" },
	},
};

Task ("externals")
	.IsDependentOn ("externals-base")
	.Does (() =>
{
	if (!IsRunningOnUnix ()) {
		return;
	}

	EnsureDirectoryExists ("./externals/");

	// iOS
	BuildXCodeFatLibrary (
		xcodeProject: "./SimplePing.xcodeproj",
		target: "SimplePing",
		workingDirectory: "./externals/Xamarin.SimplePing/");

	// macOS
	if (!FileExists ("./externals/Xamarin.SimplePing/SimplePingMac.dylib")) {
		XCodeBuild (new XCodeBuildSettings {
			Project = "./externals/Xamarin.SimplePing/SimplePing.xcodeproj",
			Target = "SimplePingMac",
			Sdk = "macosx",
			Arch = "x86_64",
			Configuration = "Release",
		});
		CopyFile (
			"./externals/Xamarin.SimplePing/build/Release/SimplePingMac.dylib",
			"./externals/Xamarin.SimplePing/SimplePingMac.dylib");
	}
});

Task ("clean")
	.IsDependentOn ("clean-base")
	.Does (() =>
{
});

SetupXamarinBuildTasks (buildSpec, Tasks, Task);

RunTarget (TARGET);
